﻿using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.FileSystem;
using SingularityForensic.Contracts.Hex;
using SingularityForensic.Contracts.Hex.Events;
using System.ComponentModel.Composition;
using System.Linq;

namespace SingularityForensic.BaseDevice.Events.Hex {
    /// <summary>
    /// 高亮MBR设备信息;
    /// </summary>
    [Export(typeof(IHexDataContextLoadedEventHandler))]
    class OnHexDataContextLoadedOnMBRDeviceHandler : IHexDataContextLoadedEventHandler {
        public int Sort => 619;

        public bool IsEnabled => true;

        public void Handle(IHexDataContext hexDataContext) {
            if (hexDataContext == null) {
                return;
            }

            if (!(hexDataContext.GetInstance<IFile>(Contracts.FileExplorer.Constants.HexDataContextTag_File) is IDevice device)) {
                return;
            }

            if (device.TypeGuid != Constants.DeviceType_DOS){
                return;
            }

            var dosDeviceInfo = device.GetInstance<DOSDeviceInfo>(Constants.DeviceStokenTag_DOSDeviceInfo);
            if (dosDeviceInfo == null) {
                LoggerService.WriteCallerLine($"{nameof(dosDeviceInfo)} can't be null.");
                return;
            }

            var pTableIndex = 0;
            foreach (var p in dosDeviceInfo.DosPartInfos.OrderBy(p => p.DosPTable.StDosPTable.nOffset)) {
                if (p.InfoDisk != null) {
                    hexDataContext.LoadCustomTypeDescriptor(
                        p.InfoDisk,
                        (long)p.DosPTable.StDosPTable.nOffset,
                        pTableIndex % 2 == 0 ? BrushBlockFactory.FirstBrush : BrushBlockFactory.SecondBrush,
                        BrushBlockFactory.HighLightBrush
                    );
                }
                
                pTableIndex++;
            }
        }
    }
}
