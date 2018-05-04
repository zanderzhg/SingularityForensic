﻿using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.Document;
using SingularityForensic.Contracts.Document.Events;
using SingularityForensic.Contracts.FileExplorer;
using SingularityForensic.Contracts.FileExplorer.Events;
using SingularityForensic.Contracts.FileExplorer.ViewModels;
using SingularityForensic.Contracts.FileSystem;
using SingularityForensic.Contracts.Helpers;
using SingularityForensic.Contracts.Hex;
using SingularityForensic.FileExplorer.Helpers;
using SingularityForensic.FileExplorer.ViewModels;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace SingularityForensic.FileExplorer {
    [Export]
    public class FileExplorerUIReactServiceForDevice {
        public void Initialize() {
            RegisterEvents();
        }

        private void RegisterEvents() {
            //设备加入文档时的呈现主视图;
            PubEventHelper.GetEvent<DocumentAddedEvent>().Subscribe(OnDocumentAddedEventOnDevice);

            //设备加入文档时呈现十六进制;
            PubEventHelper.GetEvent<DocumentAddedEvent>().Subscribe(OnDocumentAddedEventOnDeviceHex);

            //设备选中行发生变更时十六进制变化;
            PubEventHelper.GetEvent<FocusedPartitionChangedEvent>().Subscribe(OnFocusedPartChangedOnDeviceHex);

            //双击分区进入分区查看器;
            PubEventHelper.GetEvent<PartitionDoubleClickedEvent>().Subscribe(OnPartitionDoubleClicked);
        }
        
        /// <summary>
        /// 设备选中行发生变更时十六进制变化;
        /// </summary>
        /// <param name="tuple"></param>
        private void OnFocusedPartChangedOnDeviceHex((IPartitionsBrowserViewModel sender, IPartitionRow part) tuple) {
            if (!(tuple.sender is IPartitionsBrowserViewModel vm)) {
                return;
            }

            if (vm.Device == null) {
                return;
            }

            var tab = DocumentService.MainDocumentService.CurrentDocuments.
               FirstOrDefault(p => p.GetIntance<IFile>(Contracts.FileExplorer.Constants.DocumentTag_File) == vm.Device);
            if (tab == null) {
                return;
            }

            vm.Device.GetStartLBA(tuple.part.File);

            var deviceHexDataContext = tab.GetIntance<IHexDataContext>(Constants.HexDataContext_PartitionBrowser_Device);
            var partHexDataContext = tab.GetIntance<IHexDataContext>(Constants.HexDataContext_PartitionBrowser_Partition);

            if (deviceHexDataContext != null) {
                deviceHexDataContext.Position = vm.Device.GetStartLBA(tuple.part.File);
            }

            if (partHexDataContext != null) {
                partHexDataContext.Stream = tuple.part.File.BaseStream;
            }

        }

        /// <summary>
        /// 展现设备及分区十六进制;
        /// </summary>
        /// <param name="tuple"></param>
        private void OnDocumentAddedEventOnDeviceHex((IDocumentBase tab, IDocumentService owner) tuple) {
            if (!(tuple.tab is IEnumerableDocument enumDoc)) {
                return;
            }

            var device = enumDoc.GetIntance<IFile>(Contracts.FileExplorer.Constants.DocumentTag_File) as IDevice;
            if (device == null) {
                return;
            }

            var hexDeviceTuple = FileExplorerUIHelper.GetStreamHexDocument(device);
            if (hexDeviceTuple == null) {
                return;
            }

            hexDeviceTuple.Value.doc.Title =
                LanguageService.FindResourceString(Constants.DocumentTitle_HexDevice);


            var hexPartTuple = FileExplorerUIHelper.GetStreamHexDocument(null);
            if (hexPartTuple == null) {
                return;
            }
            hexPartTuple.Value.doc.Title = LanguageService.FindResourceString(Constants.DocumentTitle_HexPartition);


            enumDoc.AddDocument(hexDeviceTuple.Value.doc);
            enumDoc.AddDocument(hexPartTuple.Value.doc);

            //设备-分区十六进制加入拓展;
            enumDoc.SetInstance(hexDeviceTuple.Value.hexDataContext, Constants.HexDataContext_PartitionBrowser_Device);
            enumDoc.SetInstance(hexPartTuple.Value.hexDataContext, Constants.HexDataContext_PartitionBrowser_Partition);

            enumDoc.SelectedDocument = hexDeviceTuple.Value.doc;
        }

        /// <summary>
        /// 设备加入文档时,呈现主视图;
        /// </summary>
        /// <param name="tuple"></param>
        private void OnDocumentAddedEventOnDevice((IDocumentBase tab, IDocumentService owner) tuple) {
            if (!(tuple.tab is IEnumerableDocument enumDoc)) {
                return;
            }

            var device = enumDoc.GetIntance<IFile>(Contracts.FileExplorer.Constants.DocumentTag_File) as IDevice;
            if (device == null) {
                return;
            }

            var partitionBrowser = ServiceProvider.Current?.
                GetInstance<FrameworkElement>(Constants.PartitionsBrowserView);
            if (partitionBrowser == null) {
                LoggerService.WriteCallerLine($"{nameof(partitionBrowser)} can't be null.");
                return;
            }

            var vm = new PartitionsBrowserViewModel(device);
            partitionBrowser.DataContext = vm;

            enumDoc.SetInstance(vm, Contracts.FileExplorer.Constants.DocumentTag_PartitionsBrowserViewModel);
            enumDoc.MainUIObject = partitionBrowser;
        }

        /// <summary>
        /// 双击分区进入分区查看器;
        /// </summary>
        /// <param name="tuple"></param>
        private void OnPartitionDoubleClicked((IPartitionsBrowserViewModel sender, IPartition part) tuple) {
            if(tuple.part == null) {
                return;
            }

            FileExplorerUIHelper.GetOrAddFileDocument(tuple.part);
        }

    }
}
