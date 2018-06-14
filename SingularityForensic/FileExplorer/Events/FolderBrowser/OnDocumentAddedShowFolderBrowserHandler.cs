﻿using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.Document;
using SingularityForensic.Contracts.Document.Events;
using SingularityForensic.Contracts.FileExplorer.ViewModels;
using SingularityForensic.Contracts.FileSystem;
using System;
using System.ComponentModel.Composition;

namespace SingularityForensic.FileExplorer.Events {
    /// <summary>
    /// 分区加入文档时呈现主视图;
    /// </summary>
    /// <param name="tuple"></param>
    [Export(typeof(IDocumentAddedEventHandler))]
    public class OnDocumentAddedShowFolderBrowserHandler : IDocumentAddedEventHandler {
        public int Sort => 0;

        public bool IsEnabled => true;

        public void Handle((IDocumentBase tab, IDocumentService owner) tuple) {
            if (!(tuple.tab is IEnumerableDocument enumDoc)) {
                return;
            }

            LoggerService.WriteCallerLine($"OnDocumentAddedShowFolderBrowserHandler handling");

            try {
                var haveFileCollection = enumDoc.GetInstance<IFile>(Contracts.FileExplorer.Constants.DocumentTag_File) as IHaveFileCollection;
                if (haveFileCollection == null) {
                    return;
                }

                //因设备有专门的视图,中止操作;
                if (haveFileCollection is IDevice) {
                    return;
                }

                var vm = FileExplorerViewModelFactory.CreateFolderBrowserViewModel(haveFileCollection);
                
                var folderBrowser = ViewProvider.CreateView(Constants.FolderBrowserView, vm);

                //设定文件资源管理器模型关联实体;
                enumDoc.SetInstance(vm, Contracts.FileExplorer.Constants.DocumentTag_FolderBrowserViewModel);
                enumDoc.MainUIObject = folderBrowser;

                LoggerService.WriteCallerLine($"OnDocumentAddedShowFolderBrowserHandler handled");
            }
            catch(Microsoft.Practices.ServiceLocation.ActivationException ex) {
                LoggerService.WriteException(ex);
            }
            
        }
    }
}