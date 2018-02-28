﻿using CDFC.Parse.Contracts;
using CDFC.Parse.Modules.DeviceObjects;
using SingularityForensic.Contracts.Case;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.FileSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Dos {
    [Export(typeof(IImgParser))]
    public class DosImgParser : IImgParser {
        public ICaseManager CaseManager => ServiceProvider.Current?.GetInstance<DosCaseManager>();

        public int SortNum => 2;

        public bool CheckIsValid(string path) {
            return DosDevice.LoadFromPath(path, true, tuple => {
                //ntfAct?.Invoke(tuple.)
            }) != null;
        }

        public IFile ParseStream(string path, Action<(int totalPro, int detailPro, string word, string desc)> ntfAct, Func<bool> isCancel) {
            return DosDevice.LoadFromPath(path, true, tuple => {
                //ntfAct?.Invoke(tuple.)
            }, isCancel);
        }
    }
}