﻿using Meta.Vlc.Wpf;
using System.Windows.Controls;

namespace SingularityForensic.Previewers.Views {
    /// <summary>
    /// Interaction logic for VideoPreviewer.xaml
    /// </summary>
    public partial class VlcVideoPreviewer : UserControl {
        public VlcVideoPreviewer() {
            InitializeComponent();
            
        }
        public VlcPlayer Player => player;
    }
}
