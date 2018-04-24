﻿using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Casing;
using SingularityForensic.Contracts.Casing.Events;
using SingularityForensic.Contracts.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SingularityForensic.ITunes.Constants;

namespace SingularityForensic.ITunes {
    [Export]
    public class ITunesBackUpService {
        public void Initialize() {
            RegisterEvents();
        }

        private void RegisterEvents() {
            PubEventHelper.GetEvent<CaseEvidenceLoadingEvent>().Subscribe(OnCaseEvidenceLoading);
        }

        /// <summary>
        /// 加载案件文件若为ITunes备份文件夹,则响应镜像解析;
        /// </summary>
        /// <param name="tuple"></param>
        private void OnCaseEvidenceLoading((ICaseEvidence csEvidence, IProgressReporter reporter) tuple) {
            var csEvidence = tuple.csEvidence;
            var reporter = tuple.reporter;

            if (csEvidence == null) {
                return;
            }

            if (!(csEvidence.EvidenceTypeGuids?.Contains(EvidenceType_ITunesBackUpDir) ?? false)) {
                return;
            }

            
        }

        /// <summary>
        /// 添加ITunes备份文件夹;
        /// </summary>
        public void AddITunesBackUpDir() {
            if (CaseService.ConfirmCaseLoaded() != true) {
                return;
            }

            string backUpPath = null;

            while (true) {
                backUpPath = DialogService.Current.OpenDirect();

                if (string.IsNullOrEmpty(backUpPath)) {
                    return;
                }

                if (WordsIScn(backUpPath) == true) {
                    MsgBoxService.Show(LanguageService.FindResourceString(Constants.InvalidItunesBPath));
                    continue;
                }

                break;
            }
            
            var csEvidence = CaseService.Current.CreateNewCaseEvidence(new string[] {
                EvidenceType_ITunesBackUpDir
            }, Path.GetDirectoryName(backUpPath), backUpPath);
            
            csEvidence[ITunesBackUpDir_Path] = Path.GetFullPath(ITunesBackUpDir_Path);
            
            CaseService.Current.CurrentCase.AddNewCaseEvidence(csEvidence);

            CaseService.Current.CurrentCase.LoadCaseEvidence(csEvidence);

        }

        /// <summary>
        /// 判断是否含有非ASCII码;
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool WordsIScn(string str) {
            if (str == null) {
                return false;
            }
            var sa = str.ToCharArray();
            foreach (var ch in sa) {
                if (ch > 127) {
                    return true;
                }
            }
            return false;
        }
    }
}
