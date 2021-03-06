﻿using EventLogger;
using Prism.Commands;
using System;
using System.IO;
using System.Linq;
using Prism.Mvvm;
using SingularityForensic.Contracts.Casing;
using SingularityForensic.Contracts.App;

namespace SingularityForensic.Casing.ViewModels {
    /// <summary>
    /// 创建案件窗体视图模型;
    /// </summary>
    public partial class CreateCaseWindowViewModel : BindableBase {
        public CreateCaseWindowViewModel() {
            CaseName = LanguageService.FindResourceString(LanguageService.FindResourceString(Constants.DefaultCaseName));
            CaseTime = DateTime.Now.ToString();
            var bPath = AppDomain.CurrentDomain.BaseDirectory.Replace('\\', '/');
            bPath = bPath.EndsWith("/") ? bPath : $"{bPath}/";
            CasePath = $"{bPath}Cases";
            
        }
    }

    /// <summary>
    /// 创建案件窗体视图模型的状态；
    /// </summary>
    public partial class CreateCaseWindowViewModel {
        private string caseName;
        public string CaseName {
            get {
                return caseName;
            }
            set {
                SetProperty(ref caseName, value);
            }
        }
        
        private string caseTime;
        public string CaseTime {
            get {
                return caseTime;
            }
            set {
                SetProperty(ref caseTime, value);
            }
        }

        private string caseType;
        public string CaseType {
            get {
                return caseType;
            }
            set {
                SetProperty(ref caseType, value);
            }
        }

        private string caseNum;
        public string CaseNum {
            get {
                return caseNum;
            }
            set {
                SetProperty(ref caseNum, value);
            }
        }

        private string casePath;
        public string CasePath {
            get {
                return casePath;
            }
            set {
                try {
                    foreach (var ch in value.ToCharArray()) {
                        if (Path.GetInvalidPathChars().Contains(ch)) {
                            return;
                        }
                    }
                    SetProperty(ref casePath, value);
                }
                catch {

                }
            }
        }

        private string caseDes;
        public string CaseDes {
            get {
                return caseDes;
            }
            set {
                SetProperty(ref caseDes, value);
            }
        }

        private string caseInfo;
        public string CaseInfo {
            get {
                return caseInfo;
            }
            set {
                SetProperty(ref caseInfo, value);
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled {
            get {
                return isEnabled;
            }
            set {
                SetProperty(ref isEnabled, value);
            }
        }
        public ICase Case {
            get {
                try {
                    var sCase = new Case($"{casePath}/{caseName}", caseName) {
                        CaseDes = this.CaseDes,
                        CaseNum = this.CaseNum,
                        CaseInfo = this.CaseInfo,
                        CaseTime = this.CaseTime,
                        CaseType = this.CaseType
                    };
                    return sCase;
                }
                catch(Exception ex) {
                    Logger.WriteLine($"{nameof(CreateCaseWindowViewModel)}->{nameof(Case)}:{ex.Message}");
                    return null;
                }
            }
        }
    }

    /// <summary>
    /// 创建案件窗体视图模型的命令绑定项;
    /// </summary>
    public partial class CreateCaseWindowViewModel {
        private DelegateCommand confirmCommand;
        public DelegateCommand ConfirmCommand {
            get {
                return confirmCommand ??
                    (confirmCommand =
                    new DelegateCommand(
                        () => {
                            if (string.IsNullOrEmpty(CaseName)) {
                                MsgBoxService.Show(LanguageService.FindResourceString("CheckForNullCaseName"));
                                return;
                            }
                            if (CaseName.IndexOfAny(new char[] { '\\', '/' }) != -1) {
                                MsgBoxService.Show(LanguageService.FindResourceString("IllegalCaseName"));
                                return;
                            }
                            IsEnabled = false;
                        }
                        )
                    );
            }
        }

        private DelegateCommand queryPathCommand;
        public DelegateCommand QueryPathCommand {
            get {
                return queryPathCommand ??
                    (queryPathCommand = new DelegateCommand(
                        () => {
                            var direct = DialogService.Current?.OpenDirect();
                            if (string.IsNullOrEmpty(direct)) {
                                return;
                            }

                            CasePath = direct.Replace('\\', '/');
                        }
                    ));
            }
        }
    }

}
