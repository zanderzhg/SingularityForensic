﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SingularityForensic.TreeView.Views {
    /// <summary>
    /// Interaction logic for PropertyGridView.xaml
    /// </summary>
    [Export(Contracts.TreeView.Constants.PorpertyListView,typeof(FrameworkElement))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PropertyListView : UserControl {
        public PropertyListView() {
            InitializeComponent();
            
        }
        
    }
}
