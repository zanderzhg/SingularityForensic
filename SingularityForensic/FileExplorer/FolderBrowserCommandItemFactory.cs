﻿using Prism.Commands;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.FileExplorer;
using SingularityForensic.Contracts.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SysIO = System.IO;
using CDFCCultures.Helpers;
using SingularityForensic.FileExplorer.MessageBoxes;
using SingularityForensic.Contracts.Document;
using SingularityForensic.Contracts.Hex;
using SingularityForensic.Contracts.Hash;
using System.IO;

namespace SingularityForensic.FileExplorer {
    public static partial class FolderBrowserCommandItemFactory {
        /// <summary>
        /// 另存为功能;
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static ICommandItem CreateSaveAsFileCommandItem(IFolderBrowserViewModel vm) {
            if (vm == null) {
                throw new ArgumentNullException(nameof(vm));
            }
            
            var cmi = CommandItemFactory.CreateNew(CreateSaveAsFileCommand(vm));
            cmi.Name = LanguageService.FindResourceString(Constants.ContextCommandName_SaveAs);
            cmi.Sort = 12;
            return cmi;
        }

        private static DelegateCommand CreateSaveAsFileCommand(IFolderBrowserViewModel vm) {
            var comm = new DelegateCommand(() => {
                if (vm.SelectedFile == null) {
                    return;
                }

                if (vm.SelectedFile.File is IRegularFile regFile) {
                    RecoverFiles(new IFile[] { regFile });
                }
                else if (vm.SelectedFile.File is IDirectory dir) {
                    RecoverFiles(dir.Children);
                }
            });
            return comm;
        }

        private static void RecoverFiles(IEnumerable<IFile> files) {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            long readSize = 0;
            long totalSize = 0;

            try {
                #region 统计总大小;
                foreach (var file in files) {
                    if (file is IDirectory direc) {
                        if (!direc.IsBack && !direc.IsLocalBackUp) {
                            totalSize += direc.GetSubSize();
                        }
                    }
                    else if (file is IRegularFile) {
                        totalSize += file.Size;
                    }
                }
                #endregion
            }
            catch (Exception ex) {
                LoggerService.WriteCallerLine($"Computing Size:{ex.Message}");
                MsgBoxService.ShowError($"{LanguageService.FindResourceString("FaileToToComputeTotalSize")}:{ex.Message}");
                return;
            }

            var proDialog = DialogService.Current.CreateLoadingDialog();
            proDialog.WindowTitle = $"{LanguageService.FindResourceString("FilesBeingCopied")}";

            void saveFileFunc(IRegularFile rFile, string drPath, string fileName) {
                try {
                    var fs = System.IO.File.Create($"{drPath}/{fileName ?? rFile.Name}");
                    int read;

                    using (var mulS = rFile.GetInputStream()) {
                        var buffer = new byte[10485760];
                        mulS.Position = 0;
                        while ((read = mulS.Read(buffer, 0, buffer.Length)) != 0
                        && !proDialog.CancellationPending) {
                            fs.Write(buffer, 0, read);
                            readSize += read;
                            var pro = (int)(readSize * 100 / (totalSize != 0 ? totalSize : 4096));
                            proDialog.ReportProgress(pro <= 100 ? pro : 100, null,
                                $"{LanguageService.FindResourceString("CurExtractingFile")}:{fileName}");
                        }
                    }
                    fs.Close();
                }
                catch (Exception ex) {
                    LoggerService.WriteCallerLine(ex.Message);
                    ThreadInvoker.UIInvoke(() => {
                        MsgBoxService.ShowError($"{LanguageService.FindResourceString("FailedToExtractFile")}:{ex.Message}");
                    });
                }
            }

            if (files.Count() == 1 && files.First() is IRegularFile regFile) {
                var saveFileName = DialogService.Current.GetSaveFilePath(regFile.Name);
                if (string.IsNullOrEmpty(saveFileName)) {
                    return;
                }

                var fullPath = saveFileName;
                var drPath = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                var fileName = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);
                proDialog.DoWork += (sender, e) => {
                    if (!System.IO.Directory.Exists(drPath)) {
                        SysIO.Directory.CreateDirectory(drPath);
                    }
                    saveFileFunc(regFile, drPath, fileName);
                };

                proDialog.RunWorkerCompleted += (sender, e) => {
                    if (!e.Cancelled) {
                        Process.Start("explorer.exe", SysIO.Path.GetFullPath(drPath));
                    }
                };

                proDialog.ShowDialog(ViewProvider.GetView(Contracts.Shell.Constants.ShellView));

            }
            else {
                var drPath = DialogService.Current.GetDirect();
                if (string.IsNullOrEmpty(drPath)) {
                    return;
                }


                proDialog.DoWork += (sender, e) => {
                    if (!System.IO.Directory.Exists(drPath)) {
                        System.IO.Directory.CreateDirectory(drPath);
                    }
                    foreach (var file in files) {
                        if (file is IDirectory direct) {
                            TraverseSaveDirectory(direct , drPath, saveFileFunc, () => proDialog.CancellationPending);
                        }
                        else if (file is IRegularFile regFile2) {
                            saveFileFunc(regFile2, drPath, file.Name);
                        }
                        if (proDialog.CancellationPending) {
                            break;
                        }
                    }
                };
                proDialog.RunWorkerCompleted += (sender, e) => {
                    if (!e.Cancelled) {
                        Process.Start("explorer.exe", SysIO.Path.GetFullPath(drPath));
                    }
                };
                proDialog.ShowDialog();

            }
        }

        /// <summary>
        /// 递归保存目录;
        /// </summary>
        /// <param name="dir">目录本体</param>
        /// <param name="drPath">目标路径</param>
        /// <param name="saveFileFunc">文件保存通知进度委托</param>
        /// <param name="isCancel">动作是否取消委托</param>
        private static void TraverseSaveDirectory(IDirectory dir, string drPath,
            Action<IRegularFile, string, string> saveFileFunc, Func<bool> isCancel = null) {
            if (dir.Children != null) {
                foreach (var p in dir.Children) {
                    if (isCancel?.Invoke() == true) { return; }

                    if (p is IDirectory direct) {
                        if (!SysIO.Directory.Exists($"{drPath}/{dir.Name}")) {
                            try {
                                SysIO.Directory.CreateDirectory($"{drPath}/{dir.Name}");
                            }
                            catch (Exception ex) {
                                LoggerService.WriteCallerLine($" Creating Directory:{ex.Message}");
                                ThreadInvoker.UIInvoke(() => {
                                    MsgBoxService.ShowError($"{LanguageService.FindResourceString("FailedToCreateDirectory")}" +
                                        $"{drPath}/{dir.Name}:{ex.Message}");
                                });

                            }
                        }
                        if (!direct.IsBack && !direct.IsLocalBackUp && direct.Name != ".." && direct.Name != ".") {
                            TraverseSaveDirectory(direct, $"{drPath}/{dir.Name}", saveFileFunc, isCancel);
                        }
                    }
                    else if (p is IRegularFile regFile) {
                        saveFileFunc(regFile, $"{drPath}/{dir.Name}", p.Name);
                    }
                }
            }

        }
    }

    public static partial class FolderBrowserCommandItemFactory {
        /// <summary>
        /// 查看文件功能;
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static ICommandItem CreateViewFileCommandItem(IFolderBrowserViewModel vm) {
            var comm = CreateViewFileCommand(vm);
            var cmi = CommandItemFactory.CreateNew(comm);
            cmi.Name = LanguageService.FindResourceString(Constants.ContextCommandName_ViewFile);
            return cmi;
        }

        private static DelegateCommand CreateViewFileCommand(IFolderBrowserViewModel vm) {
            var comm = new DelegateCommand(
                () => {
                    if (vm.SelectedFile == null) {
                        return;
                    }

                    if (!(vm.SelectedFile.File is IBlockGroupedFile blockFile)) {
                        return;
                    }
                    
                    var tempFileName = SaveFileToTempPath(blockFile);
                    if (string.IsNullOrEmpty(tempFileName)) {
                        return;
                    }

                    ExplorerHelper.OpenFile(tempFileName);
                },

                () => {
                    if (vm.SelectedFile == null) {
                        return false;
                    }

                    if (!(vm.SelectedFile.File is IRegularFile regFile)) {
                        return false;
                    }

                    return true;
                }).ObservesProperty(() => vm.SelectedFile);

            return comm;
        }

        /// <summary>
        /// 保存文件到临时目录;
        /// </summary>
        /// <param name="blockFile"></param>
        /// <returns>保存的路径</returns>
        private static string SaveFileToTempPath(IBlockGroupedFile blockFile) {
            var inputStream = blockFile.GetInputStream();
            if (inputStream == null) {
                return string.Empty;
            }

            var tempDirectory = $"{Environment.CurrentDirectory}/{Constants.TempDirectoryName}/";
            var tempFileName = tempDirectory + $"{blockFile.Name}";

            try {
                //创建临时文件夹;
                if (!System.IO.Directory.Exists(tempDirectory)) {
                    System.IO.Directory.CreateDirectory(tempDirectory);
                }

                using (var tempFs = SysIO.File.Create(tempFileName)) {
                    inputStream.CopyTo(tempFs);
                }

                return tempFileName;
            }
            catch (Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
                MsgBoxService.ShowError(ex.Message);
            }

            return string.Empty;
        }
    }

    public static partial class FolderBrowserCommandItemFactory {
        /// <summary>
        /// 打开方式功能;
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static ICommandItem CreateOpenFileWithCommandItem(IFolderBrowserViewModel vm) {
            var comm = CreateOpenFileWithCommand(vm);
            var cmi = CommandItemFactory.CreateNew(comm);
            cmi.Name = LanguageService.FindResourceString(Constants.ContextCommandName_OpenFileWith);
            return cmi;
        }

        private static DelegateCommand CreateOpenFileWithCommand(IFolderBrowserViewModel vm) {
            return null;
        }
    }

    public static partial class FolderBrowserCommandItemFactory {
        /// <summary>
        /// 导航功能;
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static ICommandItem CreateNavigateCommandItem(IFolderBrowserViewModel vm) {
            var cmi = CommandItemFactory.CreateNew(null);
            cmi.Children.Add(CreateListBlockCommandItem(vm));
            cmi.Name = LanguageService.FindResourceString(Constants.ContextCommandName_Navigate);
            return cmi;
        }

        /// <summary>
        /// 列出簇功能;
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private static ICommandItem CreateListBlockCommandItem(IFolderBrowserViewModel vm) {
            var comm = CreateListBlockCommand(vm);
            var cmi = CommandItemFactory.CreateNew(comm);
            cmi.Name = LanguageService.FindResourceString(Constants.ContextCommandName_ListBlock);
            return cmi;
        }

        private static DelegateCommand CreateListBlockCommand(IFolderBrowserViewModel vm) {
            var comm = new DelegateCommand(() => {
                if (vm.SelectedFile == null) {
                    return;
                }

                if (!(vm.SelectedFile.File is IBlockGroupedFile blockGrouped)) {
                    return;
                }

                var lb = new ListBlockMessageBox(blockGrouped);
                lb.SelectedAddressChanged += (sender, e) => {
                    var tab = DocumentService.MainDocumentService.CurrentDocuments.
                        FirstOrDefault(p => p.GetIntance<IFile>(Contracts.FileExplorer.Constants.DocumentTag_File) == vm.Part);
                    if (tab == null) {
                        return;
                    }

                    var partHexDataContext = tab.GetIntance<IHexDataContext>(Contracts.FileExplorer.Constants.HexDataContext_FolderBrowser_Partition);
                    var fileHexDataContext = tab.GetIntance<IHexDataContext>(Contracts.FileExplorer.Constants.HexDataContext_FolderBrowser_File);

                    var blockGroup = blockGrouped.BlockGroups.FirstOrDefault(p => e >= p.BlockAddress);
                    if (blockGroup == null) {
                        return;
                    }

                    partHexDataContext.Position = (blockGroup.BlockSize * e) + blockGroup.Offset;


                };
                lb.Show();
            });

            return comm;
        }
    }

    public static partial class FolderBrowserCommandItemFactory {
        public static ICommandItem CreateComputeHashCommandItem(IFolderBrowserViewModel vm) {
            var cmi = CommandItemFactory.CreateNew(null);
            cmi.Name = LanguageService.FindResourceString(Constants.ContextCommandName_ComputeHash);
            var commandItems = CreateComputeHashCommandItems(vm);
            foreach (var cm in commandItems) {
                cmi.Children.Add(cm);
            }
            return cmi;
        }

        private static IEnumerable<ICommandItem> CreateComputeHashCommandItems(IFolderBrowserViewModel vm) {
            var hashers = ServiceProvider.GetAllInstances<IHasher>();
            foreach (var hasher in hashers) {
                var comm = CreateComputeHashCommand(vm, hasher);
                var cmi = CommandItemFactory.CreateNew(comm);
                cmi.Name = hasher.HashTypeName;
                yield return cmi;
            }
        }

        private static DelegateCommand CreateComputeHashCommand(IFolderBrowserViewModel vm,IHasher hasher) {
            var comm = new DelegateCommand(() => {
                if(vm.SelectedFile == null) {
                    return;
                }

                if(!(vm.SelectedFile.File is IBlockGroupedFile blockFile)) {
                    return;
                }

                var stream = blockFile.GetInputStream();
                if(stream == null) {
                    return;
                }

                
                var loadingDialog = DialogService.Current.CreateLoadingDialog();
                byte[] result = null;

                loadingDialog.DoWork += delegate {
                    result = ComputeHashOnDialog(loadingDialog, hasher,stream);
                };

                loadingDialog.RunWorkerCompleted += delegate {
                    if(result == null) {
                        return;
                    }

                    DialogService.Current.GetInputValue(hasher.HashTypeName, string.Empty, result.ConvertToHexFormat());
                };

                loadingDialog.Show();
            });
            return comm;
        }

        /// <summary>
        /// 在进度窗体当中计算哈希;
        /// </summary>
        /// <param name="loadingDialog"></param>
        /// <param name="hasher"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private static byte[] ComputeHashOnDialog(ILoadingDialog loadingDialog,IHasher hasher,Stream inputStream) {
            var latestPro = 0;

            var reporter = ProgessReporterFactory.CreateNew();
            reporter.ProgressReported += (sender, e) => {
                if (latestPro < e.pro) {
                    latestPro = e.pro;
                    loadingDialog.ReportProgress(latestPro);
                }
            };

            loadingDialog.Canceld += delegate {
                reporter.Cancel();
            };

            return hasher.ComputeHash(inputStream, reporter);
        }

        
    }
}