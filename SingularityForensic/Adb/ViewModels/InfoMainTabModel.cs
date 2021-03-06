﻿using CDFCUIContracts.Abstracts;
using Prism.Mvvm;
using SingularityForensic.Contracts.Info;
using SingularityForensic.Adb.Contracts;
using SingularityForensic.Adb.Resources;
using SingularityForensic.Controls.Info.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;

namespace SingularityForensic.Adb.ViewModels {
    //主查看器所需的TabModel拓展;
    public abstract class InfoMainTabModel : BindableBase ,ITabModel {
        public string Header { get; set; }
        //默认图标所在位置;
        public Uri Icon { get; set; }
        //高亮图标所在位置;
        public Uri ActiveIcon { get; set; }
    }

    //列表视图;
    [Export]
    public class InfoListViewTabModel : InfoMainTabModel {
        public InfoListViewTabModel() {
            Icon = IconSources.BtnListViewIcon;
            ActiveIcon = IconSources.BtnListViewActiveIcon;
            Header = ServiceProvider.Current?.GetInstance<ILanguageService>()?.FindResourceString("ListView");
        }

        //信息类型;
        private MInfoType _infoType;
        public MInfoType InfoType {
            get => _infoType;
            set => SetProperty(ref _infoType, value);
        }
        
        //展示项;
        public ObservableCollection<InfoModel> InfoModels { get; set; } = new ObservableCollection<InfoModel>();

        //选中项;
        private InfoModel _selectedInfoModel;
        public InfoModel SelectedInfoModel {
            get => _selectedInfoModel;
            set => SetProperty(ref _selectedInfoModel, value);
        }

        private Type _rowType;
        public Type RowType {
            get => _rowType;
            set => SetProperty(ref _rowType, value);
        }
    }

    //对话视图;
    [Export]
    public class TalkTabViewModel : InfoMainTabModel {
        public TalkTabViewModel() {
            Icon = IconSources.BtnTalkViewIcon;
            ActiveIcon = IconSources.BtnTalkViewActiveIcon;
            Header = ServiceProvider.Current?.GetInstance<ILanguageService>()?.FindResourceString("TalkingView");
        }

        public TalkViewModel<ITalkLog> TalkViewModel { get; } = new TalkViewModel<ITalkLog>();

        //public ObservableCollection<ITalkLog> TalkLogs { get; set; } = new ObservableCollection<ITalkLog>();
    }
}
