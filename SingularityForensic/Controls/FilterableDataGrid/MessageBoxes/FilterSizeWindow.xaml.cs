﻿using CDFCMessageBoxes.MessageBoxes;
using MahApps.Metro.Controls;
using Singularity.UI.Controls.Controls.FilterableDataGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static CDFCCultures.Managers.ResourceManager;

namespace Singularity.UI.Controls.Controls.FilterableDataGrid.MessageBoxes {
    /// <summary>
    /// Interaction logic for FilterSizeWindow.xaml
    /// </summary>
    public partial class FilterSizeWindow : MetroWindow {
        public bool? FilterResult { get; private set; }
        public FilterSizeModel FilterSizeModel { get; private set; }
        public FilterSizeWindow(FilterSizeModel fzModel) {
            InitializeComponent();
            if (fzModel == null)
                throw new ArgumentNullException(nameof(fzModel));

            this.FilterSizeModel = fzModel;
            if(fzModel.MaxSize != null) {
                txbfewer.Text = (fzModel.MaxSize / 1024).ToString();    
            }
            if(fzModel.MinSize != null) {
                txbLarger.Text = (fzModel.MinSize / 1024).ToString();
            }

            switch (fzModel.Condition) {
                case TwoConditionRule.MinAndMax:
                case TwoConditionRule.MinOrMax:
                    chbfewer.IsChecked = true;
                    chbLarger.IsChecked = true;
                    break;
                case TwoConditionRule.MinOnly:
                    chbLarger.IsChecked = true;
                    break;
                case TwoConditionRule.MaxOnly:
                    chbfewer.IsChecked = true;
                    break;
            }

            if(fzModel.MaxUnitSize != null) {
                foreach (var item in cmbfewer.Items) {
                    var comboItem = item as ComboBoxItem;
                    if (comboItem != null) {
                        if (comboItem.Content.ToString() == fzModel.MaxUnitSize) {
                            cmbfewer.SelectedItem = comboItem;
                            txbfewer.Text = GetSizeStringBy(fzModel.MaxUnitSize,fzModel.MaxSize??0);
                            break;
                        }
                    }
                }
            }
            if(fzModel.MinUnitSize != null) {
                foreach (var item in cmbLarger.Items) {
                    var comboItem = item as ComboBoxItem;
                    if (comboItem != null) {
                        if (comboItem.Content.ToString() == fzModel.MinUnitSize) {
                            cmbLarger.SelectedItem = comboItem;
                            txbLarger.Text = GetSizeStringBy(fzModel.MinUnitSize, fzModel.MinSize ?? 0);
                            break;
                        }
                    }
                }
            }
            
            rtbOr.IsChecked = fzModel.Condition == TwoConditionRule.MinOrMax;
            
        }

        private string GetSizeStringBy(string standard,long size) {
            switch (standard) {
                case "KB":
                    return (size / 1024).ToString();
                case "MB":
                    return (size / ( 1024 * 1024 )).ToString();
                case "GB":
                    return (size / ( 1024 * 1024 * 1024)).ToString();
                default:
                    return size.ToString();
            }
        }
        private void btnAct_Click(object sender, RoutedEventArgs e) {
            if (CheckInput()) {
                this.FilterResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// 检查输入;
        /// </summary>
        /// <param name="required">是否必须输入条件</param>
        /// <returns></returns>
        private bool CheckInput(bool required = true) {
            if (required && chbLarger.IsChecked == false && chbfewer.IsChecked == false) {
                CDFCMessageBox.Show(FindResourceString("InputCondiction"));
                return false;
            }

            long maxSize = 0;
            long minSize = 0;

            var minOk = long.TryParse(txbLarger.Text, out minSize);
            var maxOk = long.TryParse(txbfewer.Text, out maxSize);
            maxSize = GetSizeWithStandard(cmbfewer.SelectionBoxItem.ToString(), maxSize);
            minSize = GetSizeWithStandard(cmbLarger.SelectionBoxItem.ToString(), minSize);
            FilterSizeModel.MaxUnitSize = cmbfewer.SelectionBoxItem.ToString();
            FilterSizeModel.MinUnitSize = cmbLarger.SelectionBoxItem.ToString();

            if (chbfewer.IsChecked == false) {
                if (minOk) {
                    FilterSizeModel.MinSize = minSize;
                    FilterSizeModel.Condition = TwoConditionRule.MinOnly;
                    return true;
                }
                else {
                    CDFCMessageBox.Show(FindResourceString("InvalidMinSize"));
                    return false;
                }
            }
            else if(chbLarger.IsChecked == false) {
                if (maxOk) {
                    FilterSizeModel.MaxSize = maxSize;
                    FilterSizeModel.Condition = TwoConditionRule.MaxOnly;
                    return true;
                }
                else {
                    CDFCMessageBox.Show(FindResourceString("InvalidMaxSize"));
                    return false;
                }
            }
            else {
                Action applyAct = () => {
                    FilterSizeModel.Condition = rtbAnd.IsChecked??false?TwoConditionRule.MinAndMax:TwoConditionRule.MinOrMax;
                    FilterSizeModel.MaxSize = maxSize;
                    FilterSizeModel.MinSize = minSize;
                };

                if (maxOk && minOk) {
                    if(maxSize < minSize) {
                        if(CDFCMessageBox.Show(FindResourceString("ConfirmWhenMaxLessThanMin"),MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                            applyAct();
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        applyAct();
                        return true;
                    }
                }
                else if (!maxOk) {
                    CDFCMessageBox.Show(FindResourceString("InvalidMaxSize"));
                    return false;
                }
                else if(!minOk) {
                    CDFCMessageBox.Show(FindResourceString("InvalidMinSize"));
                    return false;
                }
                return false;
            }
            
        }
        
        private long GetSizeWithStandard(string stand,long size) {
            switch (stand) {
                case "KB":
                    return size * 1024;
                case "MB":
                    return size * 1024 * 1024;
                case "GB":
                    return size * 1024 * 1024 * 1024;
                default:
                    return size;
            }
        }
        private void btnDeact_Click(object sender, RoutedEventArgs e) {
            if (CheckInput(false)) {
                this.FilterResult = false;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.FilterResult = null;
            this.Close();
        }
    }
}
