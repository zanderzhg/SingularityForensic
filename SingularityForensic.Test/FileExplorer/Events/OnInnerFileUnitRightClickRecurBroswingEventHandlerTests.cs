﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingularityForensic.Contracts.FileExplorer;
using SingularityForensic.Contracts.FileExplorer.ViewModels;
using SingularityForensic.Contracts.FileSystem;
using SingularityForensic.Contracts.TreeView;
using SingularityForensic.FileExplorer.Events;
using SingularityForensic.FileExplorer.Events.TreeView;
using System.Linq;

namespace SingularityForensic.Test.FileExplorer.Events {
    [TestClass()]
    public class OnInnerFileUnitRightClickRecurBroswingEventHandlerTests {
        [TestInitialize]
        public void Initialize() {
            TestCommon.InitializeTest();
            _handler = new OnFileExplorerModuleLoadingRecurBrowsingHandler();
        }
        OnFileExplorerModuleLoadingRecurBrowsingHandler _handler;

        [TestMethod()]
        public void HandleTest() {
            var device = FileSystemService.Current.MountStream(System.IO.File.OpenRead("E://anli/Fat32_Test.img"), "mmp", null, null) as IDevice;
            Assert.IsNotNull(device);
            var unit = TreeUnitFactory.CreateNew(SingularityForensic.Contracts.FileExplorer.Constants.TreeUnitType_InnerFile);
            unit.SetInstance<IFile>(device.Children.First(), SingularityForensic.Contracts.FileExplorer.Constants.TreeUnitTag_InnerFile);
            _handler.Handle();
            var mainDocService = SingularityForensic.Contracts.Document.DocumentService.MainDocumentService;
            var docs = mainDocService.CurrentDocuments;
            Assert.AreEqual(docs.Count(), 1);

            var fbDoc = docs.First().GetInstance<IFolderBrowserDataContext>(SingularityForensic.Contracts.FileExplorer.Constants.DocumentTag_FolderBrowserDataContext);
            Assert.IsNotNull(fbDoc);
            Assert.AreEqual(fbDoc.FolderBrowserViewModel.FileRows.Count(),device.GetInnerFiles().Count());
        }
    }
}