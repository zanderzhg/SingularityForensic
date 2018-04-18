﻿using System.Windows;
using SingularityForensic.Previewers.ViewModels;
using SingularityForensic.Previewers.Models;
using SingularityForensic.Contracts.FileExplorer;
using SingularityForensic.Contracts.Previewers;
using System;

namespace SingularityForensic.Previewers {
    /// <summary>
    /// Vlc视频播放器;
    /// </summary>
    public abstract class VideoPreviewer<TVideoPreviewerModel> : IPreviewer 
        where TVideoPreviewerModel : IVideoPreviewerModel<IPlayer> {
        public virtual object DataContext => null;
        
        public abstract UIElement View { get; }

        //~VideoPreviewer() {
        //    Dispose();
        //}
        public abstract TVideoPreviewerModel VideoPreviewerModel { get; }

        FrameworkElement IPreviewer.View => throw new NotImplementedException();

        public virtual void Dispose() => VideoPreviewerModel.Dispose();
    }

    public class VlcVideoPreviewer : VideoPreviewer<VlcVideoPreviewerModel> {
        public VlcVideoPreviewer(string videoFileName) {
            this.FileName = videoFileName;
        }

        public string FileName { get; }

        private Views.VideoPreviewer _previewer;
        public override UIElement View {
            get {
                if(_previewer == null) {
                    _previewer = new Views.VideoPreviewer {
                        ScreenPanelContent = previewerPanel,
                        DataContext = VideoPreviewerModel
                    };
                }
                return _previewer;
            }
        }

        private Views.VlcVideoPreviewer previewerPanel = new Views.VlcVideoPreviewer();

        /// <summary>
        /// Vlc视图模型;
        /// </summary>
        private VlcVideoPreviewerModel _videoPreviewerModel;
        public override VlcVideoPreviewerModel VideoPreviewerModel {
            get {
                if(_videoPreviewerModel == null) {
                    var _player = new VlcPlayer(previewerPanel.Player);
                    _player.FileName = FileName;
                    _videoPreviewerModel = new VlcVideoPreviewerModel(_player);
                }
                return _videoPreviewerModel;
            }
        }


    }
}