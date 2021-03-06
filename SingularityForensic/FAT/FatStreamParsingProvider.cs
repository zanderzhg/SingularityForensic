﻿using CDFC.Util.PInvoke;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.FileSystem;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using SingularityForensic.Contracts.Common;

namespace SingularityForensic.FAT {
    /// <summary>
    /// FAT分区表解析装置;
    /// </summary>
    [Export(typeof(IStreamParsingProvider))]
    public partial class FatStreamParsingProvider : IStreamParsingProvider {
        public int Order => 32;

        public string GUID => Constants.StreamParserGUID_FAT;

        public bool CheckIsValidStream(Stream stream) {
            if(stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }

            var unManagedStreamAdapter = new UnmanagedStreamAdapter(stream);
            
            try {
                if (unManagedStreamAdapter.StreamPtr == IntPtr.Zero) {
                    return false;
                }
                
                var stPartition = Fat_Init(unManagedStreamAdapter.StreamPtr);
                if (stPartition == IntPtr.Zero) {
                    return false;
                }
                else {
                    Fat_Exit(stPartition);
                    return true;
                }
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
                return false;
            }
            finally {
                unManagedStreamAdapter.Dispose();
            }
        }

        /// <summary>
        /// 获得FAT类型;
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static FATPartType? GetFatType(Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }
            
            var unManagedManager = UnMgdBasicDeviceManagerFactory.Create(stream);
            if(unManagedManager == null) {
                return null;
            }

            try {
                if (unManagedManager.BasicDevicePtr == IntPtr.Zero) {
                    return null;
                }
                if (Partition_B_Fat(unManagedManager.BasicDevicePtr)) {
                    return FATPartType.FAT32;
                }
                else if (Partition_B_Fat16(unManagedManager.BasicDevicePtr)) {
                    return FATPartType.FAT16;
                }
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
            return null;
        }
        
        public IFile ParseStream(Stream stream, string name, IProgressReporter reporter) {
            if(stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }

            var fatPartType = GetFatType(stream);
            if (fatPartType == null) {
                LoggerService.WriteCallerLine($"{nameof(FATPartType)} can't be null.");
                return null;
            }

            var part = FileFactory.CreatePartition(Constants.PartitionKey_FAT);
            var stoken = part.GetStoken(Constants.PartitionKey_FAT);
            stoken.BaseStream = stream;
            stoken.Name = name;
            stoken.Size = stream.Length;
            stoken.TypeGuid = Constants.PartitionType_FAT32;

            if (fatPartType == FATPartType.FAT32) {
                stoken.PartType = ServiceProvider.GetAllInstances<IPartitionType>().FirstOrDefault(p => p.GUID == Constants.PartitionType_FAT32);
            }
            else {
                stoken.PartType = ServiceProvider.GetAllInstances<IPartitionType>().FirstOrDefault(p => p.GUID == Constants.PartitionType_FAT16);
            }

            var unmanagedManager = new UnmanagedFATManager(stream);
            var partInfo = new FATPartInfo {
                UnmanagedFATManager = unmanagedManager
            };
            
            //设定FAT分区详细信息;
            stoken.SetInstance(partInfo,Constants.PartitionStokenTag_FATPartInfo);

            if(unmanagedManager.FATManagerPtr == IntPtr.Zero) {
                unmanagedManager.Dispose();
                throw new InvalidOperationException($"{nameof(unmanagedManager.FATManagerPtr)} can't be nullptr.");
            }
            
            LoadPartInfo(partInfo);
            LoadPartContent(part, reporter);
            
            part.Disposing += OnPartDisposing;

            return part;
        }

        /// <summary>
        /// 释放FAT分区;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPartDisposing(object sender, EventArgs e) {
            if(!(sender is IPartition part)) {
                return;
            }

            if (part.TypeGuid != Constants.PartitionType_FAT32) {
                return;
            }

            PartitionStoken stoken = null;
            try {
                stoken = part.GetStoken(Constants.PartitionKey_FAT);
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }

            if(stoken == null) {
                return;
            }

            var partInfo = stoken.GetInstance<FATPartInfo>(Constants.PartitionStokenTag_FATPartInfo);
            if(partInfo == null) {
                return;
            }

            //释放非托管流;
            try {
                partInfo.UnmanagedFATManager?.Dispose();
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
            
        }
    
        /// <summary>
        /// 加载文件系统基本信息;
        /// </summary>
        /// <param name="info"></param>
        private static void LoadPartInfo(FATPartInfo info) {
            if(info == null) {
                throw new ArgumentNullException(nameof(info));
            }

            try {
                var infoPtr = Fat_Get_FsInfo(info.UnmanagedFATManager.FATManagerPtr);
                var infoPtrBack = Fat_Get_BackupFsInfo(info.UnmanagedFATManager.FATManagerPtr);
                var dbrPtr = Fat_Get_FsDBR(info.UnmanagedFATManager.FATManagerPtr);
                var dbrPtrBack = Fat_Get_BackupFsDBR(info.UnmanagedFATManager.FATManagerPtr);

                //为避免多次编写重复的判断是否为空,赋值语句,减少错误率,使用泛型方法将逻辑统一,编辑DBR或者Info;
                void SetFatDbrOrInfo<TFsEntity,TEntity>(
                    IntPtr fsPtr,Func<TFsEntity,IntPtr> getEntityPtr,
                    Func<TFsEntity,long> getOffset,
                    Action<TEntity,long> setInfoOrDBR) 
                    where TFsEntity : struct 
                    where TEntity : struct{

                    if(fsPtr == IntPtr.Zero) {
                        return;
                    }

                    TEntity? entity = null;
                    TFsEntity? fsEntity = null;
                    try {
                        fsEntity = fsPtr.GetStructure<TFsEntity>();
                    }
                    catch(Exception ex) {
                        LoggerService.WriteCallerLine(ex.Message);
                        return;
                    }
                    
                    if(fsEntity == null) {
                        return;
                    }

                    //获取实体指针;
                    var entPtr = getEntityPtr(fsEntity.Value);
                    if (entPtr == IntPtr.Zero) {
                        LoggerService.WriteCallerLine($"{nameof(entPtr)} of {typeof(TFsEntity)} is nullptr.");
                        return;
                    }

                    try {
                        entity = entPtr.GetStructure<TEntity>();
                        
                    }
                    catch(Exception ex) {
                        LoggerService.WriteCallerLine(ex.Message);
                        return;
                    }

                    if(entity == null) {
                        return;
                    }

                    setInfoOrDBR(entity.Value, getOffset(fsEntity.Value));
                }

                SetFatDbrOrInfo<StFatFSInfo, StFatINFO>(
                    infoPtr, 
                    fsInfo => fsInfo.stFatINFO,
                    fsInfo => (long)fsInfo.nOffset,
                    (fatFsEnt, offset) => info.FatInfo = new FATInfo(fatFsEnt, offset) {
                        InternalDisplayName = LanguageService.FindResourceString(Constants.DisplayName_FATInfo)
                    }
                );

                SetFatDbrOrInfo<StFatFSInfo, StFatINFO>(
                    infoPtrBack,
                    fsInfo => fsInfo.stFatINFO,
                    fsInfo => (long)fsInfo.nOffset,
                    (fatFsEnt, fatInfo) => info.FatInfo_BackUp = new FATInfo(fatFsEnt, fatInfo) {
                        InternalDisplayName = LanguageService.FindResourceString(Constants.DisplayName_FATInfoBackup)
                    }
                );

                SetFatDbrOrInfo<StFatFSDBR, StFatDBR>(
                    dbrPtr,
                    fsDbr => fsDbr.stFatDBR,
                    fsDbr => (long)fsDbr.nOffset,
                    (fatFsDbr,fatDbr) => info.FatDBR = new FATDBR(fatFsDbr, fatDbr) {
                        InternalDisplayName = LanguageService.FindResourceString(Constants.DisplayName_FATDBR)
                    }
                );

                SetFatDbrOrInfo<StFatFSDBR, StFatDBR>(
                    dbrPtrBack,
                    fsDbr => fsDbr.stFatDBR,
                    fsDbr => (long)fsDbr.nOffset,
                    (fatFsDbr, fatDbr) => info.FatDBR_BackUp = new FATDBR(fatFsDbr, fatDbr) {
                        InternalDisplayName = LanguageService.FindResourceString(Constants.DisplayName_FATDBRBackup)
                    }
                );
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
        }

        /// <summary>
        /// 加载文件系统内容;
        /// </summary>
        /// <param name="partInfo"></param>
        /// <param name="reporter">进度回调器</param>
        private static void LoadPartContent(
            IPartition part,
            IProgressReporter reporter) {

            var partStoken = part.GetStoken(Constants.PartitionKey_FAT);
            var partInfo = partStoken.GetInstance<FATPartInfo>(Constants.PartitionStokenTag_FATPartInfo);
            var partManager = partInfo.UnmanagedFATManager;
            if(partManager.FATManagerPtr == IntPtr.Zero) {
                LoggerService.WriteCallerLine($"{nameof(partManager.FATManagerPtr)} can't be nullptr.");
                return;
            }

            try {
                var filePtr = Fat_Get_RootDir(partManager.FATManagerPtr);
                //当前分区已经被加载的大小;
                long partLoadedSize = 0;

                DealWithFileNode(
                    part,
                    partInfo,
                    filePtr,
                    sz => {
                        if(reporter == null) {
                            return;
                        }

                        partLoadedSize += sz;
                        if (partStoken.Size == 0) {
                            return;
                        }
                        var per = (int)(partLoadedSize * 100 / partStoken.Size);
                        if (per > 100) {
                            per = 100;
                        }

                        if (partStoken.Size != 0) {
                            reporter.ReportProgress(per);
                        }
                    },
                    () => reporter?.CancelPending??false
                );
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
        }

        /// <summary>
        /// 处理子文件/目录/其它加载;
        /// </summary>
        /// <param name="haveFileCollection"></param>
        /// <param name="filePtr">非托管文件节点指针</param>
        /// <param name="ntfSzAct"></param>
        /// <param name="isCancel"></param>
        private static void DealWithFileNode(
            IHaveFileCollection haveFileCollection,
            FATPartInfo partInfo,
            IntPtr filePtr,
            Action<long> ntfSzAct,
            Func<bool> isCancel) {

            while (filePtr != IntPtr.Zero) {
                IFile file = null;
                FileStokenBase2 fileStoken2 = null;

                var stFileNode = filePtr.GetStructure<StFatFileNode>();
                var fatFileInfo = new FATFileInfo {
                    StFileNode = stFileNode
                };
                
                //若为目录,则加入目录;
                if ((stFileNode.FileAttrib & StFatFileNode.FATFileAttr.Directory) ==
                    StFatFileNode.FATFileAttr.Directory) {

                    var dir = FileFactory.CreateDirectory(Constants.DirectoryKey_FAT);

                    var dirStoken = dir.GetStoken(Constants.DirectoryKey_FAT);
                    dirStoken.TypeGuid = Constants.DirectoryType_FAT32;


                    file = dir;
                    fileStoken2 = dirStoken;
                    EditFileStoken2(fileStoken2, partInfo, fatFileInfo);

                    //非备份/同级备份目录名不会以'.'开头;
                    if (stFileNode.NameBuffer[0] != '.') {
                        LoadDirectoryContent(
                            dir,
                            partInfo,
                            ntfSzAct,
                            isCancel
                        );
                    }
                    else if (stFileNode.NameBuffer[2] == '.') {
                        dirStoken.IsBack = true;
                    }
                    else {
                        dirStoken.IsLocalBackUp = true;
                    }
                }
                //若为普通文件;
                else if ((stFileNode.FileAttrib & StFatFileNode.FATFileAttr.SDocument) ==
                    StFatFileNode.FATFileAttr.SDocument) {

                    var regFile = FileFactory.CreateRegularFile(Constants.RegularFileKey_FAT);
                    var regFileStoken = regFile.GetStoken(Constants.RegularFileKey_FAT);
                    
                    regFileStoken.TypeGuid = Constants.RegularFileType_FAT32;

                    file = regFile;
                    fileStoken2 = regFileStoken;
                    
                    EditFileStoken2(fileStoken2, partInfo, fatFileInfo);

                    ntfSzAct?.Invoke(fileStoken2.Size);
                }
                
                if (file != null) {
                    haveFileCollection.Children.Add(file);
                }
                
                filePtr = stFileNode.Next;
                if (isCancel?.Invoke() ?? false) {
                    return;
                }
            }
        }

        
        /// <summary>
        /// 编辑目录/文件/其它的时间,簇列表等值;
        /// </summary>
        /// <param name="fileStoken2"></param>
        private static void EditFileStoken2(
            FileStokenBase2 fileStoken2,
            FATPartInfo partInfo,
            FATFileInfo fatFileInfo) {

            if(fileStoken2 == null) {
                throw new ArgumentNullException(nameof(fileStoken2));
            }

            if(fatFileInfo == null) {
                throw new ArgumentNullException(nameof(fatFileInfo));
            }

            if(fatFileInfo.StFileNode == null) {
                throw new InvalidOperationException($"{nameof(fatFileInfo.StFileNode)} of {nameof(FATFileInfo)} can't be nullptr.");
            }

            if(partInfo == null) {
                throw new ArgumentNullException(nameof(partInfo));
            }

            fileStoken2.SetInstance(fatFileInfo,Constants.FileStokenTag_FATFileInfo);

            fileStoken2.Name = fatFileInfo.StFileNode.Value.Name;
            fileStoken2.Size = fatFileInfo.StFileNode.Value.FileSize;
            fileStoken2.Deleted = fatFileInfo.StFileNode.Value.Deleted;
            
            EditTime(fileStoken2, fatFileInfo);
#if DEBUG
            if (fileStoken2.Name == "avcodec-56.dll") {

            }
            if(fileStoken2.Name == "SILK2MP3.EXE") {

            }
#endif
            EditBlockGroups(fileStoken2, partInfo, fatFileInfo);
        }

        /// <summary>
        /// 编辑时间;
        /// </summary>
        /// <param name="fileStoken2"></param>
        /// <param name="fatFileInfo"></param>
        private static void EditTime(
            FileStokenBase2 fileStoken2,
            FATFileInfo fatFileInfo) {

            if (fileStoken2 == null) {
                throw new ArgumentNullException(nameof(fileStoken2));
            }

            if (fatFileInfo == null) {
                throw new ArgumentNullException(nameof(fatFileInfo));
            }

            try {
                //编辑时间;
                fileStoken2.ModifiedTime = fatFileInfo.StFileNode.Value.ModifiedTime;
                fileStoken2.AccessedTime = fatFileInfo.StFileNode.Value.AccessTime;
                fileStoken2.CreateTime = fatFileInfo.StFileNode.Value.CreateTime;
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
#if DEBUG
                //编辑时间;
                //fileStoken2.ModifiedTime = fatFileInfo.StFileNode.Value.ModifiedTime;
#endif
            }
            
        }

        /// <summary>
        /// 编辑块组;
        /// </summary>
        /// <param name="fileStoken2"></param>
        /// <param name="partInfo"></param>
        /// <param name="fatFileInfo"></param>
        private static void EditBlockGroups(
            FileStokenBase2 fileStoken2,
            FATPartInfo partInfo,
            FATFileInfo fatFileInfo) {
            if (fileStoken2 == null) {
                throw new ArgumentNullException(nameof(fileStoken2));
            }

            if (fatFileInfo == null) {
                throw new ArgumentNullException(nameof(fatFileInfo));
            }

            if (fatFileInfo.StFileNode == null) {
                throw new InvalidOperationException($"{nameof(fatFileInfo.StFileNode)} of {nameof(FATFileInfo)} can't be nullptr.");
            }

            if (partInfo == null) {
                throw new ArgumentNullException(nameof(partInfo));
            }
            
            if ((fatFileInfo.StFileNode?.stClusterList ?? IntPtr.Zero) == IntPtr.Zero) {
                //LoggerService.WriteCallerLine($"{nameof(StFatFileNode.stClusterList)} can't be nullptr.");
                return;
            }

            if (partInfo.ClusterSize == null) {
                //LoggerService.WriteCallerLine($"{nameof(partInfo.ClusterSize)} of {nameof(partInfo)} can't be null.");
                return;
            }

            var clusters = MarshalExtensions.GetStructs<StFatClusterNode>(fatFileInfo.StFileNode.Value.stClusterList, stCluster => stCluster.Next);
            int clusterSize = partInfo.ClusterSize.Value;

            //遍历,粘合,将连续的簇列表并入为一个块组;
            StFatClusterNode? lastCluster = null;
            long firstClusterLBA = 0;

            long blockCount = 0;
            ulong lastClusterNum = 0;

            //重置局部变量;参数为本次(循环)最新的独立头块;
            void Reset(StFatClusterNode cluster) {
                lastCluster = cluster;
                blockCount = 1;
                lastClusterNum = cluster.nClusterNum;
                firstClusterLBA = (long) Fat_ClusterNum_Convert(
                    partInfo.UnmanagedFATManager.FATManagerPtr,
                    cluster.nClusterNum
                );
            }

            //压栈;
            void PushBlockGroup() {
                if (lastCluster == null) {
                    return;
                }

                fileStoken2.BlockGroups.Add(
                    BlockGroupFactory.CreateNewBlockGroup((long)lastCluster.Value.nClusterNum, blockCount, clusterSize, firstClusterLBA)
                );
            }
            
            try {
                foreach (var cluster in clusters) {
                    if (lastCluster == null) {
                        Reset(cluster);
                        continue;
                    }

                    //如果是连续的簇号则+1;
                    if (cluster.nClusterNum == lastClusterNum + 1) {
                        lastClusterNum = cluster.nClusterNum;
                        blockCount++;
                    }
                    //否则,构造BlockGroup,插入新建链表,并重置;
                    else {
                        PushBlockGroup();
                        Reset(cluster);
                    }
                }

                if (lastCluster != null) {
                    PushBlockGroup();
                }
            }
            catch (Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
        }

        /// <summary>
        /// 加载目录内容;
        /// </summary>
        /// <param name="fatFileInfo"></param>
        private static void LoadDirectoryContent(
            IDirectory direct,
            FATPartInfo partInfo,
            Action<long> ntfSzAct,
            Func<bool> isCancel) {

            var dirStoken = direct.GetStoken(Constants.DirectoryKey_FAT);
            var fatFileInfo = dirStoken.GetInstance<FATFileInfo>(Constants.FileStokenTag_FATFileInfo) as FATFileInfo;
            var filePtr = Fat_Parse_Dir(partInfo.UnmanagedFATManager.FATManagerPtr, fatFileInfo.StFileNode.Value.stClusterList);

            DealWithFileNode(direct, partInfo , filePtr, ntfSzAct, isCancel);
        }

       
    }
    
    partial class FatStreamParsingProvider {
        private const string partAsm = "PartitionManager.dll";
        [DllImport(partAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static bool Partition_B_Fat(IntPtr stPartition);

        [DllImport(partAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static bool Partition_B_Fat16(IntPtr stPartition);

        private const string fatAsm = "FatRecover.dll";
        
        //StFileList* Fat_Get_RootDir(void* stPartition);
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Get_RootDir(IntPtr stPartition);

        //StFileList* Fat_Parse_Dir(void* stPartition, StClusterList* stCluster);
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Parse_Dir(IntPtr stPartition, IntPtr stCluster);
        
        //引导扇区
        //StFatFSDBR* Fat_Get_FsDBR(void* stPartition);
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Get_FsDBR(IntPtr stPartition);

        //引导扇区备份
        //StFatFSDBR* Fat_Get_BackupFsDBR(void* stPartition);
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Get_BackupFsDBR(IntPtr stPartition);

        //FSINFO信息
        //StFatFSInfo* Fat_Get_BackupFsInfo(void* stPartition);
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Get_BackupFsInfo(IntPtr stPartition);

        //FSINFO信息备份
        //StFatFSInfo* Fat_Get_FsInfo(void* stPartition);
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Get_FsInfo(IntPtr stPartition);

        /// <summary>
        /// 从簇号转换至绝对偏移地址;
        /// </summary>
        /// <param name="stPartition"></param>
        /// <param name="nClusterNum"></param>
        /// <returns></returns>
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static ulong Fat_ClusterNum_Convert(IntPtr stPartition, ulong nClusterNum);

        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Init(IntPtr stStream);

        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static void Fat_Exit(IntPtr stPartition);
    }
}
