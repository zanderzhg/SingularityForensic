﻿using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 非托管的各种状态封装,例如分区/设备管理;
/// </summary>
namespace SingularityForensic.FileSystem {
    /// <summary>
    /// 基础设备(Dos/Gpt)信息管理器,用于处理非托管的状态保存;
    /// </summary>
    internal class UnmanagedBasicDeviceManager:IDisposable {
        private const string partAsm = "PartitionManager.dll";
        [DllImport(partAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static void Partition_Exit(IntPtr stPartition);
        [DllImport(partAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Partition_Init(IntPtr stStream);

        private bool _disposed;
        public void Dispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(UnmanagedBasicDeviceManager));
            }

            //释放适配器实例;
            if (BasicDevicePtr != IntPtr.Zero) {
                try {
                    Partition_Exit(BasicDevicePtr);
                }
                catch (Exception ex) {
                    LoggerService.WriteCallerLine(ex.Message);
                }
            }

            //释放非托管接口管理单元;
            try {
                StreamAdpater?.Dispose();
            }
            catch (Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }

            _disposed = true;
        }

        /// <summary>
        /// 通过一个流实例获取非托管管理单元适配器实例以及对应的适配器实例;
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public UnmanagedBasicDeviceManager(Stream stream) {
            if(stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }

            try {
                StreamAdpater = new UnmanagedStreamAdapter(stream);
                BasicDevicePtr = Partition_Init(StreamAdpater.StreamPtr);
            }
            catch(Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
        }

        /// <summary>
        /// 非托管单元指针;
        /// </summary>
        public IntPtr BasicDevicePtr { get; }

        /// <summary>
        /// 流适配器实例;
        /// </summary>
        public UnmanagedStreamAdapter StreamAdpater { get; }
    }

    /// <summary>
    /// FAT信息管理器,用于处理非托管的状态保存;
    /// </summary>
    internal class UnmanagedFATManager : IDisposable {
        private const string fatAsm = "FatRecover.dll";

        //void* Fat_Init(Stream* stStream);
        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr Fat_Init(IntPtr stStream);

        [DllImport(fatAsm, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private extern static void Fat_Exit(IntPtr stPartition);

        private bool _disposed;
        public void Dispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(UnmanagedFATManager));
            }

            //释放适配器实例;
            if (FATManagerPtr != IntPtr.Zero) {
                try {
                    Fat_Exit(FATManagerPtr);
                }
                catch (Exception ex) {
                    LoggerService.WriteCallerLine(ex.Message);
                }
            }

            //释放非托管接口管理单元;
            try {
                StreamAdpater?.Dispose();
            }
            catch (Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }

            _disposed = true;
        }

        /// <summary>
        /// 通过一个流实例获取非托管管理单元适配器实例以及对应的适配器实例;
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public UnmanagedFATManager(Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }

            try {
                StreamAdpater = new UnmanagedStreamAdapter(stream);
                FATManagerPtr = Fat_Init(StreamAdpater.StreamPtr);
            }
            catch (Exception ex) {
                LoggerService.WriteCallerLine(ex.Message);
            }
        }

        /// <summary>
        /// 非托管单元指针;
        /// </summary>
        public IntPtr FATManagerPtr { get; }

        /// <summary>
        /// 流适配器实例;
        /// </summary>
        public UnmanagedStreamAdapter StreamAdpater { get; }
    }
}