﻿using CDFCMessageBoxes.MessageBoxes;
using Microsoft.Practices.ServiceLocation;
using SingularityForensic.Android.FileSystem.Models;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.TreeView;
using SingularityForensic.Android.Info.ViewModels;
using SingularityForensic.Info;
using SingularityForensic.Info.Views;
using System.ComponentModel.Composition;
using System.Linq;

namespace SingularityForensic.Android.Info.Global.Services {
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ForensicService {
        ///// <summary>
        ///// 开始对某个案件文件取证;
        ///// </summary>
        ///// <param name="cFile"></param>
        //public void StartForensic(AndroidDeviceCaseEvidence cFile) {
        //    var window = new StartForensicWindow();
        //    var vm = ServiceProvider.Current.GetInstance<AndStartForensicWindowViewModel>();

        //    window.DataContext = vm;
        //    vm.DeviceFile = cFile;
        //    vm.CloseRequest += delegate {
        //        window.Close();
        //    };
            
        //    window.ShowDialog();
        //}

        ///// <summary>
        ///// 加载取证信息节点;
        ///// </summary>
        ///// <param name="adCFile"></param>
        //public void LoadForensicUnit(AndroidDeviceCaseEvidence adCFile) {
        //    var frService = ServiceProvider.Current.GetInstance<ICommonForensicService>();
        //    //加载取证分析节点;
        //    var fUnit = frService?.AddForensicUnit(adCFile);
        //    if (fUnit == null) {
        //        RemainingMessageBox.Tell($"{nameof(fUnit)} can't be null!");
        //        return;
        //    }

        //    foreach (var infoKind in PinKindsDefinitions.ForensicClassTypes) {
        //        if (fUnit.Children.FirstOrDefault(p => p is PinTreeUnit pinUnit && pinUnit.ContentId == infoKind) == null) {
        //            fUnit.Children.Add(
        //                new PinTreeUnit(infoKind, fUnit) {
        //                    Label = PinKindsDefinitions.GetClassLabel(infoKind)
        //                }
        //            );
        //        }
        //    }
        //}

    }
}
