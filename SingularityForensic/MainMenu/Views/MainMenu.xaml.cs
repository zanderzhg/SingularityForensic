﻿using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace SingularityForensic.MainMenu.Views {
    /// <summary>
    /// Interaction logic for MenuWithToolBar.xaml
    /// </summary>
    [Export]
    public partial class MainMenu : UserControl {
        public MainMenu() {
            InitializeComponent();
        }
       
        
    }
}
