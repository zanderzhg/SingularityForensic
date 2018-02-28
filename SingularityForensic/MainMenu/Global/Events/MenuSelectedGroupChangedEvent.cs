﻿using Prism.Events;
using SingularityForensic.Contracts.Contracts.MainMenu;

namespace SingularityForensic.MainMenu.Global.Events {
    /// <summary>
    /// 菜单选择项变化事件(通知);
    /// </summary>
    public class MenuSelectedGroupChangedEvent : PubSubEvent<MenuItemGroup> {

    }
}
