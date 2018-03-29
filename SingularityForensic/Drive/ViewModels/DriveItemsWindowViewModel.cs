﻿using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.TreeView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace SingularityForensic.Drive.ViewModels {
    [Export]
    class DriveItemsWindowViewModel:BindableBase {
        public DriveItemsWindowViewModel() {

        }
        
        /// <summary>
        /// 初始化;
        /// </summary>
        /// <param name="reset">是否重置</param>
        public void Initialize(bool reset = false) {
            if(_comObject == null) {
                InitilizeCore();
            }
            else if (reset) {
                InitilizeCore();
            }
        }
        
        //初始化核心;
        private void InitilizeCore() {
            IsLoading = true;

            ThreadInvoker.BackInvoke(() => {
                try {
                    _comObject = ComObject.Current;
                }
                catch (Exception ex) {
                    LoggerService.WriteCallerLine(ex.Message);
                    return;
                }

                var units = GetUnitsFromObject();

                ThreadInvoker.UIInvoke(() => {
                    DriveUnits.AddRange(units);
                });
                IsLoading = false;
            });
        }

        /// <summary>
        /// 根据ComObject获得Units;
        /// </summary>
        private IEnumerable<TreeUnit> GetUnitsFromObject() {
            List<TreeUnit> units = new List<TreeUnit>();

            //各种检查为空;
            if (_comObject == null) {
                LoggerService.Current?.WriteCallerLine($"{nameof(_comObject)} can't be null.");
                return Enumerable.Empty<TreeUnit>();
            }

            if (_comObject.LocalHdds == null) {
                LoggerService.Current?.WriteCallerLine($"{nameof(_comObject.LocalHdds)} can't be null.");
                return Enumerable.Empty<TreeUnit>();
            }

            //遍历添加子节点;
            foreach (var hdd in _comObject.LocalHdds) {
                var hddUnit = new TreeUnit(Constants.DriveType_LocalHDD, hdd) {
                    Label = hdd.SerialNumber
                };
                if (hdd.Volumes == null) {
                    LoggerService.Current?.WriteCallerLine($"{nameof(hdd.Volumes)} can't be null.");
                    continue;
                }

                foreach (var volume in hdd.Volumes) {
                    var volUnit = new TreeUnit(Constants.DriveType_LocalVolume, volume) {
                        Label = $"{volume.Sign}:"
                    };
                    hddUnit.Children.Add(volUnit);
                }

                units.Add(hddUnit);
            }

            return units;
        }

        private ComObject _comObject;
        public InteractionRequest<Notification> CloseRequest { get; private set; } = new InteractionRequest<Notification>();

        private TreeUnit _selectedUnit;
        public TreeUnit SelectedUnit {
            get => _selectedUnit;
            set => SetProperty(ref _selectedUnit, value);
        }

        public ObservableCollection<TreeUnit> DriveUnits { get; set; } = new ObservableCollection<TreeUnit>();

        //当前算选定的设备;
        public (string driveType,object drive)? SelectedDriveTuple { get; private set; }

        private bool _isLoading;
        public bool IsLoading {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        //加载文字;
        private string _loadingText;
        public string LoadingText {
            get => _loadingText;
            set => SetProperty(ref _loadingText, value);
        }

        //是否选择了确定;
        private bool _confirmed;
        /// <summary>
        /// 确认命令;
        /// </summary>
        private DelegateCommand _confirmCommand;
        public DelegateCommand ConfirmCommand => _confirmCommand ??
            (_confirmCommand = new DelegateCommand(
                () => {
                    SelectedDriveTuple = (SelectedUnit.TypeGuid, SelectedUnit.Tag);
                    _confirmed = true;
                    CloseRequest.Raise(new Notification());
                },
                () => SelectedUnit != null).
            ObservesProperty(() => SelectedUnit));

        //窗体已经关闭的命令;
        private DelegateCommand _closedCommand;
        public DelegateCommand ClosedCommand => _closedCommand ??
            (_closedCommand = new DelegateCommand(
                () => {
                    if (!_confirmed) {
                        SelectedDriveTuple = null;
                    }
                    _confirmed = false;
                }
            ));


        /// <summary>
        /// 视图显示后开始初始化;
        /// </summary>
        private DelegateCommand _renderedCommand;
        public DelegateCommand RenderedCommand => _renderedCommand ??
            (_renderedCommand = new DelegateCommand(
                () => {
                    Initialize();
                }
            ));

        ~DriveItemsWindowViewModel(){
            _comObject?.Dispose();
            _comObject = null;
        }


    }
}