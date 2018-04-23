﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Prism.Commands;
using SingularityForensic.Contracts.FileExplorer;
using SingularityForensic.Contracts.FileSystem;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Helpers;
using SingularityForensic.Contracts.FileExplorer.Events;
using SingularityForensic.Contracts.Controls;

using SingularityForensic.FileExplorer.Models;
using SingularityForensic.Controls.GridView;
using SingularityForensic.Controls;
using SingularityForensic.Contracts.FileExplorer.Models;
using SingularityForensic.Contracts.FileExplorer.ViewModels;

namespace SingularityForensic.FileExplorer.ViewModels {
    /// <summary>
    /// 目录/资源浏览器模型;
    /// </summary>
    public partial class FolderBrowserViewModel : DataGridExViewModel, IFolderBrowserViewModel {
        /// <summary>
        /// 目录/资源浏览器模型构造方法;
        /// </summary>
        /// <param name="part">模型所属主文件</param>
        public FolderBrowserViewModel(IPartition part) {
            this.Part = part ?? throw new ArgumentNullException(nameof(part));

            InitializeFileRowDescriptors();
            InitializeColumns();
            this.FillWithCollection(part);
        }

        /// <summary>
        /// 初始化行元数据提供器;
        /// </summary>
        private void InitializeFileRowDescriptors() {
            if (FileRowFactory.Current.DescriptorsInitialized) {
                return;
            }

            var fileMetaDataProviders = ServiceProvider.Current?.GetAllInstances<IFileMetaDataProvider>().OrderBy(p => p.Order);
            if (fileMetaDataProviders == null) {
                LoggerService.WriteCallerLine($"{nameof(fileMetaDataProviders)} can't be null.");
                return;
            }

#if DEBUG
            //var arr = fileMetaDataProviders.ToArray();
#endif
            FileRowFactory.Current.InitializeDescriptors(fileMetaDataProviders);
        }

        /// <summary>
        /// 初始化列;比如在<see cref="InitializeFileRowDescriptors"/>后执行
        /// </summary>
        private void InitializeColumns() {
            foreach (var descripter in FileRowFactory.Current.PropertyDescriptors) {
                _files.PropertyDescriptorList.Add(descripter);
            }
        }

        public IPartition Part { get; }                                        //浏览器所属主文件（分区，设备等);

        private CustomTypedListSource<IFileRow> _files = new CustomTypedListSource<IFileRow>();
        public ICollection<IFileRow> Files {
            get => _files;
            set => _files = value as CustomTypedListSource<IFileRow>;
        } 
        

        private IFileRow _selectedRow;
        public IFileRow SelectedRow {
            get => _selectedRow;
            set {
                SetProperty(ref _selectedRow, value);
                if (_selectedRow == null) {
                    return;
                }

                SelectedFile = _selectedRow;
                PubEventHelper.GetEvent<FocusedFileRowChangedEvent>().Publish((this,SelectedFile));
            }
        }

        public IFileRow SelectedFile { get; private set; }

        /// <summary>
        /// 填充行;
        /// </summary>
        /// <param name="files"></param>
        public void FillRows(IEnumerable<IFile> files) {
            if (files == null) {
                return;
            }

            Files.Clear();

            foreach (var file in files) {
                Files.Add(FileRowFactory.Current.CreateFileRow(file));
            }
        }
        
        private ObservableCollection<INavNodeModel> NavNodeModels { get; set; } = new ObservableCollection<INavNodeModel>();
        
        public ICollection<INavNodeModel> NavNodes {
            get => NavNodeModels;
            set {
                NavNodeModels = value as ObservableCollection<INavNodeModel>;
            }
        }
        
        private ObservableCollection<ICommandItem> _viewersCommands;
        public virtual ObservableCollection<ICommandItem> ViewersCommands {
            get {
                if (_viewersCommands == null) {
                    _viewersCommands = new ObservableCollection<ICommandItem>();
                    
                }
                return _viewersCommands;
            }
            set {
                _viewersCommands = value;
            }
        }
        
        private bool viewerEverLoaded;
        

        public void Exit() {
            NavNodeModels.Clear();
        }
        
        //public static ICommandItem CreateViewerProgramCommandItem(ViewerProgramModel viewerProgramModel) {
        //    var commandItem = CommandItemFactory.CreateNew(viewerProgramModel.WatchCommand);
        //    commandItem.Name = viewerProgramModel.Name;
        //    return commandItem;
        //}

#if DEBUG
        ~FolderBrowserViewModel() {

        }
#endif
    }

    /// <summary>
    /// 前台通知相关;
    /// </summary>
    public partial class FolderBrowserViewModel {
        /// <summary>
        /// 双击进入目录动作;
        /// </summary>
        /// <param name="row"></param>
        public override void NotifyDoubleClickOnRow(object row) {
            if (!(row is IFileRow fileRow)) {
                return;
            }

            try {
                var file = fileRow.File;
                if (!(file is IHaveFileCollection haveFileCollection)) {
                    return;
                }
                if (file is IDirectory direct) {
                    if (direct.IsBack) {
                        if (direct.Parent is IHaveFileCollection parentCollection
                            && (parentCollection as IFile)?.Parent is IHaveFileCollection grandCollection) {
                            this.FillWithCollection(grandCollection);

                        }
                    }
                    else if (!direct.IsLocalBackUp) {
                        this.FillWithCollection(haveFileCollection);
                    }

                }
            }
            catch (Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
        }

        /// <summary>
        /// 自动生成动作;
        /// </summary>
        /// <param name="e"></param>
        public override void NotifyAutoGeneratingColumns(GridViewAutoGeneratingColumnEventArgs e) {
            var descriptor = FileRowFactory.Current.PropertyDescriptors.FirstOrDefault(p => p.Name == e.ItemPropertyInfo.Name);
            if (!(descriptor is FileRow.FileRowPropertyDescriptor fileRowPropDescriptor)) {
                return;
            }
            
            e.CellTemplate = fileRowPropDescriptor.FileMetaDataProvider.CellTemplate;
            e.Converter = fileRowPropDescriptor.FileMetaDataProvider.Converter;
        }
    }
    
    ////目录试图资源管理器模型过滤命令部分;
    public partial class FolderBrowserViewModel {
        //一键取消所有的过滤;
        private DelegateCommand _cancelFilteringCommand;
        public DelegateCommand CancelFilteringCommand =>
            _cancelFilteringCommand ?? (_cancelFilteringCommand = new DelegateCommand(() => {
                FilterSettings = Enumerable.Empty<FilterSetting>();
            }));
    }

    
}
