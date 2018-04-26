﻿using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfHexaEditor;

namespace SingularityForensic.Hex.Views {
    /// <summary>
    /// Interaction logic for InterceptHex.xaml
    /// </summary>
    [
        Export(
            Contracts.Hex.Constants.HexView,
            typeof(FrameworkElement)
        ),
        PartCreationPolicy(
            CreationPolicy.NonShared
        )
    ]

    public partial class HexView : UserControl {
        public HexView() {
            InitializeComponent();
            HexEditor.SetBinding(DrawedHexEditor.CustomBackgroundBlocksProperty, new Binding(nameof(HexEditor.CustomBackgroundBlocks)));
        }
    }
}