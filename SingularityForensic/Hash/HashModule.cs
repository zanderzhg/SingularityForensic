﻿using Prism.Mef.Modularity;
using Prism.Modularity;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Contracts.Hash.Events;
using SingularityForensic.Contracts.Helpers;
using SingularityForensic.Contracts.Splash.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Hash {
    [ModuleExport(typeof(HashModule))]
    class HashModule : IModule {
        public void Initialize() {
            CommonEventHelper.GetEvent<SplashMessageEvent>().
                Publish(LanguageService.FindResourceString(Constants.HashModuleLoading));

            try {
                CommonEventHelper.GetEvent<HashModuleLoadingEvent>().Publish();
                CommonEventHelper.PublishEventToHandlers<IHashModuleLoadingEventHandler>();
            }
            catch(Exception ex) {
                LoggerService.WriteException(ex);
            }
        }
    }
}
