﻿using CDFC.Info.Adb;
using SingularityForensic.Contracts.TreeView;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace SingularityForensic.Adb.ViewModels {
    [Export]
    public class InfoTreeModel {
        public ObservableCollection<ITreeUnit> TreeUnits { get; set; } = new ObservableCollection<ITreeUnit>();

        private ITreeUnit _selectedUnit;
        public ITreeUnit SelectedUnit {
            get {
                return _selectedUnit;
            }
            set {
                _selectedUnit = value;
            }
        }

        /// <summary>
        /// 加入ADB查看节点;
        /// </summary>
        /// <param name="container">Adb信息容器</param>
        public void AddAdbUnit(PhoneFullInfoContainer container) {
            ////检查是否已经加载了相同的设备节点;
            //var preContainerUnit = TreeUnits.FirstOrDefault(p => 
            //    p is AdbDeviceCaseFileUnit fullContainer && 
            //    fullContainer.PhoneInfoContainer.Device.Serial == container.Device.Serial
            //);
            ////若存在,则去除之;
            //if (preContainerUnit != null) {
            //    TreeUnits.Remove(preContainerUnit);
            //}

            //var adbUnit = new AdbDeviceCaseFileUnit(container,null);
            //TreeUnits.Add(adbUnit);
            //NotifyUnitExpand(adbUnit);
        }
        
        //通知节点展开;
        public event EventHandler<ITreeUnit> NotifyUnitExpanded;
        //通知节点选中;
        public event EventHandler<ITreeUnit> SelectedUnitChanged;

        protected void NotifyUnitExpand(ITreeUnit unit) {
            NotifyUnitExpanded?.Invoke(this, unit);
        }

        public void NotifySelectionUnitChanged(ITreeUnit unit) {
            SelectedUnit = unit;
            SelectedUnitChanged?.Invoke(this, unit);
            
        }
    }
}
