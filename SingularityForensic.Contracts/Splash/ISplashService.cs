﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Contracts.Splash {
    /// <summary>
    /// Splash服务;
    /// </summary>
    public interface ISplashService {
        void ReportMessage(string msg);
        void ShowSplash();
        void CloseSplash();
    }
}
