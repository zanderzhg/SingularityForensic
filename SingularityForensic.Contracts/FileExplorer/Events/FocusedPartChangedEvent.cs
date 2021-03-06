﻿using Prism.Events;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.FileExplorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Contracts.FileExplorer.Events {
    /// <summary>
    /// 聚焦分区(行)变化时发生;
    /// </summary>
    public class FocusedPartitionChangedEvent : PubSubEvent<(IPartitionsBrowserViewModel sender, IPartitionRow part)> {
    }

    public interface IFocusedPartitionChangedEventHandler : IEventHandler<(IPartitionsBrowserViewModel sender, IPartitionRow part)> {

    }

}
