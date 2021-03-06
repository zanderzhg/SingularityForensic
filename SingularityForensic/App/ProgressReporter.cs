﻿using SingularityForensic.Contracts.App;
using System;
using System.ComponentModel.Composition;

namespace SingularityForensic.App {
    /// <summary>
    /// 进度报告器,便于在调用同一个动作时,便于使用不同的展示方式;
    /// </summary>
    public class ProgressReporter : IProgressReporter {
        public void ReportProgress(int percentProgress) {
            ReportProgress(percentProgress, string.Empty, string.Empty);
        }
        public void ReportProgress(int percentProgress, string text, string descrip) {
            ProgressReported?.Invoke(this, (percentProgress,0, text, descrip));
        }

        public void ReportProgress(int totalPer, int detailPer, string desc, string detail) {
            ProgressReported?.Invoke(this, (totalPer, detailPer, desc, detail));
        }

        public event EventHandler<(int totalPer, int detailPer, string desc, string detail)> ProgressReported;

        private string _title;
        public string Title {
            get => _title;
            set {
                _title = value;
                TitleChanged?.Invoke(this, _title);
            }
        }

        public event EventHandler<string> TitleChanged;
        public event EventHandler Canceld;

        //取消工作;
        public void Cancel() {
            CancelPending = true;
            Canceld?.Invoke(this, EventArgs.Empty);
        }

        //是否正在取消;
        public bool CancelPending { get; private set; }
    }

    [Export(typeof(IProgessReporterFactory))]
    public class ProgressReporterFactoryImpl : IProgessReporterFactory {
        public IProgressReporter CreateNew() => new ProgressReporter();
    }
}
