﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SingularityForensic.Contracts.Common {
    /// <summary>
    /// 命令项;
    /// </summary>
    public interface ICommandItem:ICustomNotify {
        /// <summary>
        /// 是否可见;
        /// </summary>
        bool IsVisible { get; }
        /// <summary>
        /// 是否已经被Check;
        /// </summary>
        bool IsChecked { get; set; }
        /// <summary>
        /// 命令;
        /// </summary>
        ICommand Command { get; }
        /// <summary>
        /// 名称;
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// ICon;
        /// </summary>
        Uri Icon { get; set; }
        bool IsEnabled { get; set; }
        string GUID { get; }

        IEnumerable<ICommandItem> Children { get; }

        void AddChild(ICommandItem commandItem);

        void RemoveChild(ICommandItem commandItem);

        /// <summary>
        /// 排序;
        /// </summary>
        int Sort { get; set; }
    }

    /// <summary>
    /// 命令项工厂;
    /// </summary>
    public interface ICommandItemFactory {
        ICommandItem CreateNew(ICommand command,string guid,Func<bool> isVisible = null);
    }

    public class CommandItemFactory:GenericServiceStaticInstance<ICommandItemFactory> {
        public static ICommandItem CreateNew(ICommand command,string guid = null,Func<bool> isVisible = null) => Current?.CreateNew(command,guid,isVisible);
    }

    //public interface ICommandItem<TData> : ICommandItem {
    //    Func<TData> GetData { get; set; }
    //}

    ///// <summary>
    ///// 需要上下文数据的命令绑定项;
    ///// </summary>
    ///// <typeparam name="TData"></typeparam>
    //public class CommandItem<TData> : CommandItem, ICommandItem<TData> {
    //    public virtual Func<TData> GetData { get; set; }
    //}

    //具有参数的命令绑定项;
    //public class DelegateCommandItem<TPara> : CommandItem {
    //    public DelegateCommandItem(DelegateCommand<TPara> command, TPara para) {
    //        this.Command = command;
    //        this.CommandParameter = para;
    //    }
    //    public TPara CommandParameter { get; }
    //    public override bool HasParameter => true;
    //}
}
