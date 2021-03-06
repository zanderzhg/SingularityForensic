﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using SingularityForensic.Info.Models;
using EventLogger;
using SingularityForensic.Contracts.Casing;
using SingularityForensic.Contracts.App;

namespace SingularityForensic.Info.ViewModels {
    /// <summary>
    /// 开始取证窗体视图模型;
    /// </summary>
    /// <typeparam name="TCaseEvidence"></typeparam>
    public abstract partial class StartForensicWindowViewModel:BindableBase  {
        public StartForensicWindowViewModel(IEnumerable<CheckGroupTreeItem> groups,
            IEnumerable<CheckItemTreeItem> items) {
            this.items = items;
            foreach (var group in groups) {
                CheckGroups.Add(group);
            }
            BuildTree();
        }

        ////初始化各状态;
        //public void Init() {

        //}
        private IEnumerable<CheckItemTreeItem> items;

        public ObservableCollection<CheckGroupTreeItem> CheckGroups { get; set; } = new ObservableCollection<CheckGroupTreeItem>();

        private void BuildTree() {
            foreach (var group in CheckGroups) {
                group.Children.Clear();
                foreach (var item in items.Where(p => p.Group == group)) {
                    group.Children.Add(item);
                    item.Pro = 0;
                    item.CheckStateChanged += (sender, e) => {
                        var hasE = group.Children.FirstOrDefault(p => p.IsChecked == e) != null;
                        var hasNot = group.Children.FirstOrDefault(p => p.IsChecked != e) != null;
                        if (hasE && hasNot) {
                            group.IsChecked = null;
                        }
                        else {
                            group.IsChecked = hasE?e:!e;
                        }
                    };
                }
                
            }
            
        }

        private ICaseEvidence _deviceFile;
        public ICaseEvidence DeviceFile {
            get => _deviceFile;
            set {
                _deviceFile = value;
                //TraverseItem(item => item.Init(_deviceFile));
            }
        }
    }

    public abstract partial class StartForensicWindowViewModel<TCaseEvidence> {
        //确认命令;
        private DelegateCommand _confirmCommand;
        public DelegateCommand ConfirmCommand =>
            _confirmCommand ?? (_confirmCommand = new DelegateCommand(
                () => {
                    if (!isWorking) {
                        var checkedItems = new List<CheckItemTreeItem>();
                        TraverseItem(item => {
                            if (item.IsChecked) {
                                checkedItems.Add(item);
                            }
                        });
                        
                        //若无选中项,则提示退出;
                        if (checkedItems.Count == 0) {
                            MsgBoxService.Show(LanguageService.FindResourceString("ConfirmToCheckForensicItem"));
                            return;
                        }
                        
                        try {
                            TraverseItem(item => item.IsReadOnly = true);

                            checkedItems.ForEach(p => {
                                //p.Init(DeviceFile);
                            });
                            isWorking = true;
                            ThreadPool.QueueUserWorkItem(cb2 => {
                                var handles = new ManualResetEvent[checkedItems.Count];
                                for (int i = 0; i < handles.Length; i++) {
                                    handles[i] = new ManualResetEvent(false);
                                }

                                var index = 0;
                                checkedItems.ForEach(p => {
                                    Thread.Sleep(50);
                                    var thisIndex = index++;
                                    ThreadPool.QueueUserWorkItem(cb => {
                                        try {
                                            p.StartForensic(() => isCancel);
                                        }
                                        catch (Exception ex) {
                                            Logger.WriteLine($"{nameof(StartForensicWindowViewModel<TCaseEvidence>)} ->{nameof(ConfirmCommand)}({p.Name}):{ex.Message}");
                                            ThreadInvoker.UIInvoke(() => {
                                                MsgBoxService.ShowError(ex.Message);
                                            });
                                        }
                                        finally {
                                            handles[thisIndex].Set();
                                        }
                                    });
                                });
                                WaitHandle.WaitAll(handles);
                                ThreadInvoker.UIInvoke(() => {
                                    isWorking = false;
                                    if (!isCancel && MsgBoxService.Show(LanguageService.FindResourceString("ConfirmToShow"),
                                        MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                                        checkedItems.ForEach(p => p.Setup());
                                        CloseRequest?.Invoke(this, EventArgs.Empty);
                                    }

                                    _confirmCommand.RaiseCanExecuteChanged();
                                    checkedItems.ForEach(p => p.Free());
                                    TraverseItem(item => item.IsReadOnly = false);
                                });
                            });
                        }
                        catch (Exception ex) {
                            Logger.WriteLine($"{nameof(StartForensicWindowViewModel<TCaseEvidence>)} ->{nameof(ConfirmCommand)}:{ex.Message}");
                        }
                    }
                    _confirmCommand.RaiseCanExecuteChanged();
                },
                () => !isWorking
            ));

        private bool isWorking;
        private bool isCancel;

        /// <summary>
        /// 遍历每一个选项;
        /// </summary>
        /// <param name="act"></param>
        void TraverseItem(Action<CheckItemTreeItem> act) {
            //foreach (var group in CheckGroups) {
            //    foreach (var item in group.Children) {
            //        act(item as CheckItemTreeItem<TCaseEvidence>);
            //    }
            //}
        }

        public event EventHandler CloseRequest;
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(
                () => {
                    if(ConfirmCancelWork()) {
                        CloseRequest?.Invoke(this, EventArgs.Empty);
                    }
                }
            ));

        //取消工作;
        private bool ConfirmCancelWork() {
            if (isWorking) {
                if (MsgBoxService.Show(LanguageService.FindResourceString("SureToCancelForensic"),
                    MessageBoxButton.YesNo) == MessageBoxResult.No) {
                    return false;
                }
                isCancel = true;
            }

            isWorking = false;
            TraverseItem(item => {
                item.IsReadOnly = false;
                item.Free();
            });
            return true;
        }

        private DelegateCommand<CancelEventArgs> _closingCommand;
        public DelegateCommand<CancelEventArgs> ClosingCommand =>
            _closingCommand ?? (_closingCommand = new DelegateCommand<CancelEventArgs>(
                e => {
                    if (!ConfirmCancelWork()) {
                        if (isWorking) {
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            ));
    }
}
