﻿using Prism.Mef.Modularity;
using Prism.Modularity;
using SingularityForensic.Adb.Contracts;

namespace SingularityForensic.Adb {
    [ModuleExport(typeof(AdbViewerModule))]
    public class AdbViewerModule : IModule {
        public void Initialize() {
            RegisterEvents();
        }
        
        //注册相关事件;
        private void RegisterEvents() {
            //PubEventHelper.GetEvent<TreeNodeClickEvent>()?.Subscribe(unit => {
            //        if(fsTabServiceToken?.Value == null) {
            //            return;
            //        }

            //        var fsTabService
            //    = fsTabServiceToken.Value;
            //        if (unit is SingleInfoModelUnit<AdbInfoBasicModel> basicModelUnit) {
            //            var basic = basicModelUnit.InfoModel.Info;
            //            var preTab = fsTabService.CurrentTabs.FirstOrDefault(p => p is AdbBasicTabModel adbBTModel &&
            //            adbBTModel.Basic == basicModelUnit.InfoModel.Info);

            //            if (preTab != null) {
            //                fsTabService.ChangeSelectedTab(preTab);
            //                return;
            //            }
            //            else {
            //                var tab = new AdbBasicTabModel(basicModelUnit.InfoModel.Info) {
            //                    Title = $"{basicModelUnit.Label}-{basicModelUnit.InfoModel.Info?.Model}"
            //                };
            //                fsTabService.AddTab(tab);
            //            }
            //        }
            //        else if(unit is AdbInfoContainerUnit containerUnit) {
            //            AddAdbShowingFile(containerUnit.Container);
            //        }
            //        else if (unit is MultiInfoModelsUnit infoModelsUnit) {
            //            if (infoModelsUnit.InfoType.GetMInfoTypeBox() == MInfoTypeBox.AdbFile) {
            //                //infoModelsUnit.InfoModels
            //                //var vm = new AdbGridViewModel();
            //                //_regionManager?.RequestNavigate(RegionNames.InfoMainRegion, "AdbGrid");
            //            }
            //            else {
            //                var vm = new InfoMainViewModel();
            //                var tab = new AdbMainTabModel(vm) { Title = unit.Label};
            //                vm.LoadInfoes((infoModelsUnit.InfoModels, infoModelsUnit.InfoType));
            //                //_regionManager?.RequestNavigate(RegionNames.InfoMainRegion, "InfoMain");
            //                //CurShowingModel = InfoMainViewModel;
            //                fsTabService.AddTab(tab);
            //            }
            //        }
            //        else if (unit is SingleInfoModelsUnit sInModelsUnit) {
            //            var vm = new InfoMainViewModel();
            //            var tab = new AdbMainTabModel(vm) {
            //                Title = $"{unit.Label}-{sInModelsUnit.InfoType}"
            //            };
            //            vm.LoadInfoes((sInModelsUnit.InfoModels, sInModelsUnit.InfoType), true);
            //            fsTabService.AddTab(tab);
            //            //_regionManager?.RequestNavigate(RegionNames.InfoMainRegion, "InfoMain");
            //        }
            //    });
        }
        
        /// <summary>
        /// 加入Adb节点显示;
        /// </summary>
        /// <param name="container"></param>
        public void AddAdbShowingFile(IInfoModelContainer container) {
            //if(this.fsTabServiceToken?.Value == null) {
            //    EventLogger.Logger.WriteLine($"{nameof(AdbViewerModule)}->{nameof(AddAdbShowingFile)}:{nameof(AdbViewerModule.fsTabServiceToken)} is null!");
            //    return;
            //}

            //var browserItems = this.fsTabServiceToken.Value.CurrentTabs;
            //var preItem = browserItems.FirstOrDefault(p => (p as AdbTabModel)?.AdbTabViewModel.Container == container);
            //if (preItem != null) {
            //    fsTabServiceToken.Value.SelectedTab = preItem;
            //}
            //else {
            //    if (container is IDefaultPhoneInfoContainer) {
            //        var adbTab = new AdbTabModel(new AdbTabViewModel((container as IDefaultPhoneInfoContainer).Parent.Device, container));
            //        //fsTabServiceToken.Value.AddTab()
            //        //fsTabServiceToken.Value.AddTab(adbTab);
            //        //fsTabServiceToken.Value.ChangeSelectedTab(adbTab);
            //    }
            //}
        }

       
    }
}
