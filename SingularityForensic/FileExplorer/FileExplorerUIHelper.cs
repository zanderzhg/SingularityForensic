﻿using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.Document;
using SingularityForensic.Contracts.FileSystem;
using SingularityForensic.Contracts.Hex;
using System.Linq;

namespace SingularityForensic.FileExplorer {
    public static class FileExplorerUIHelper {
        /// <summary>
        /// 根据流文件创建一个十六进制Tab;
        /// </summary>
        /// <param name="blockedStream"></param>
        /// <returns></returns>
        internal static (IDocument doc, IHexDataContext hexDataContext)?
            GetStreamHexDocument(IStreamFile blockedStream) {

            var mainDocService = DocumentService.MainDocumentService;
            if (mainDocService == null) {
                LoggerService.WriteCallerLine($"{nameof(mainDocService)} can't be null.");
                return null;
            }

            var hexDoc = mainDocService.CreateNewDocument();
            hexDoc.SetInstance(blockedStream,
                Contracts.FileExplorer.Constants.HexDataContextTag_BlockedStream);

            var hexService = ServiceProvider.Current.GetInstance<IHexService>();
            if (hexService == null) {
                LoggerService.WriteCallerLine($"{nameof(hexService)} can't be null.");
                return null;
            }

            var hexDataContext = hexService.CreateNewHexDataContext(blockedStream?.BaseStream);
            hexDoc.SetInstance(hexDataContext, Contracts.Hex.Constants.Tag_HexDataContext);
            hexDoc.UIObject = hexDataContext.UIObject;
            return (hexDoc, hexDataContext);
        }

        /// <summary>
        /// 添加文件(设备/分区)显示到文档区域中;
        /// </summary>
        /// <param name="device"></param>
        internal static void AddFileToDocument(IFile file) {
            //检查文档区域是否已经被添加了相关文件;
            if (CheckTagAddedToDocument(file)) {
                return;
            }

            var mainDocService = DocumentService.MainDocumentService;
            if (mainDocService == null) {
                LoggerService.WriteCallerLine($"{nameof(mainDocService)} can't be null.");
                return;
            }

            var enumDoc = mainDocService.CreateNewEnumerableDocument();
            enumDoc.SetInstance(file,Contracts.FileExplorer.Constants.DocumentTag_File);
            if (file is IPartition part) {
                enumDoc.Title = part.GetPartFixAndName();
            }
            else {
                enumDoc.Title = file.Name;
            }

            mainDocService.AddDocument(enumDoc);
            mainDocService.SelectedDocument = enumDoc;
        }

        /// <summary>
        /// 查找文档区域是否已经添加了File相关文档;
        /// </summary>
        /// <param name="tag"></param>
        internal static bool CheckTagAddedToDocument(IFile file) {
            var mainDocService = DocumentService.MainDocumentService;
            if (mainDocService == null) {
                LoggerService.WriteCallerLine($"{nameof(mainDocService)} can't be null.");
                return true;
            }

            var doc = mainDocService.CurrentDocuments.FirstOrDefault(p =>
            p.GetIntance<IFile>( Contracts.FileExplorer.Constants.DocumentTag_File) == file);

            if (doc != null) {
                mainDocService.SelectedDocument = doc;
                return true;
            }

            return false;
        }
    }
}
