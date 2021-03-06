﻿using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.Controls;
using SingularityForensic.Contracts.FileExplorer;
using SingularityForensic.Contracts.FileExplorer.Events;
using SingularityForensic.Contracts.FileExplorer.ViewModels;
using SingularityForensic.Contracts.FileSystem;
using SingularityForensic.Contracts.StatusBar;
using System;
using System.ComponentModel.Composition;

namespace SingularityForensic.FileExplorer.Events {
    /// <summary>
    /// 当资源管理器当前文件集合发生变更时,通知状态栏变化;
    /// </summary>
    [Export(typeof(IFolderBrowserDataContextCreatedEventHandler))]
    class OnFolderBrowserDataContextCreatedOnFilesChangedToStatusBarHandler : IFolderBrowserDataContextCreatedEventHandler {
        public int Sort => 7;

        public bool IsEnabled => true;

        public void Handle(IFolderBrowserDataContext args) {
            if (args == null) {
                return;
            }
            if (args.FolderBrowserViewModel == null) {
                return;
            }
            args.FolderBrowserViewModel.FileCollectionChanged += (sender, e) => RefreshFilesCount(args);
        }
        
        private void RefreshFilesCount(IFolderBrowserDataContext dataContext) {
            if(dataContext == null) {
                return;
            }
            var vm = dataContext.FolderBrowserViewModel;
            if(vm == null) {
                return;
            }

            long fileCount = 0;
            long regFileCount = 0;
            long dirCount = 0;

            try {
                foreach (var file in vm.FileRows) {
                    if(file.File is IRegularFile) {
                        regFileCount++;
                    }
                    else if(file.File is IDirectory) {
                        dirCount++;
                    }
                    fileCount++;
                }

                ThreadInvoker.UIInvoke(() => {
                    var fileCountItem = StatusBarService.GetOrCreateStatusBarTextItem(Constants.StatusBarItemText_FileCount, GridChildLength.Auto, 5);
                    var regFileCountItem = StatusBarService.GetOrCreateStatusBarTextItem(Constants.StatusBarItemGUID_RegFileCount, GridChildLength.Auto, 6);
                    var dirCountItem = StatusBarService.GetOrCreateStatusBarTextItem(Constants.StatusBarItemGUID_DirectoryCount, GridChildLength.Auto, 7);

                    fileCountItem.Text = $"{LanguageService.FindResourceString(Constants.StatusBarItemText_FileCount)} {fileCount}";
                    regFileCountItem.Text = $"{LanguageService.FindResourceString(Constants.StatusBarItemText_RegFileCount)} {regFileCount}";
                    dirCountItem.Text = $"{LanguageService.FindResourceString(Constants.StatusBarItemText_DirectoryCount)} {dirCount}";
                });
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
        }
    }
}
