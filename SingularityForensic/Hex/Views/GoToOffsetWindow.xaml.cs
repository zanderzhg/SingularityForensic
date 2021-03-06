﻿using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Hex.Models;
using System.Windows;

namespace SingularityForensic.Hex.Views {
    /// <summary>
    /// Interaction logic for GoToOffsetWindow.xaml
    /// </summary>
    public partial class GoToOffsetWindow  {
        public GoToOffsetWindow() {
            InitializeComponent();
        }
        public EscapteMethod EscapeMethod {
            get {
                if (rbStart.IsChecked == true) {
                    return EscapteMethod.FromStart;
                }
                else if(rbCurrent.IsChecked == true) {
                    return EscapteMethod.Current;
                }
                else if(rbBackFromCurrent.IsChecked == true) {
                    return EscapteMethod.CurrentBackFrom;
                }
                else {
                    return EscapteMethod.BackFrom;
                }
            }
        }
        public long? Offset {
            get {
                long offset;
                if(long.TryParse(txbOffset.Text,out offset)) {
                    return offset;
                }
                return null;
            }
        }

        public bool Confirmed { get; set; }         //是否确定了;

        private void ConfirmButton_Click(object sender, RoutedEventArgs e) {
            if(Offset == null) {
                MsgBoxService.Show( ServiceProvider.Current?.GetInstance<ILanguageService>()?.FindResourceString("IncorrectPara"));
                return;
            }
            else {
                Confirmed = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        
    }

}
