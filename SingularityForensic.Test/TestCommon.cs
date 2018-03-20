﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Casing;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Test.App;
using SingularityForensic.Test.Casing;
using SingularityForensic.Test.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SingularityForensic.Test {
    
    
    //业务逻辑测试时必须调用这个类的InitializeTest方法;
    class TestCommon {
        

        //初始化ServiceProvider等;
        public static void InitializeTest() {
            ServiceProvider.SetServiceProvider(ExportProviderServiceProviderMocker.StaticInstance);
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(SingularityForensic.Dummy).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(SingularityForensic.Contracts.Dummy).Assembly));
            
            var container = new CompositionContainer(catalog);

            ExportProviderServiceProviderMocker.StaticInstance.ExportProvider = container;
           

            //设定案件MessageBoxMocker;
            ExportProviderServiceProviderMocker.StaticInstance.SetInstance(CaseMockers.CaseDialogServiceMocker);

            //设定事件聚合器;
            ExportProviderServiceProviderMocker.StaticInstance.SetInstance(CommonMockers.AggregatorMocker);
            
            //App相关Mocker;
            ExportProviderServiceProviderMocker.StaticInstance.SetInstance(AppMockers.MsgBoxMocker);
            ExportProviderServiceProviderMocker.StaticInstance.SetInstance(AppMockers.ThreadInvokerMocker);
            ExportProviderServiceProviderMocker.StaticInstance.SetInstance(AppMockers.DialogMocker);
            ExportProviderServiceProviderMocker.StaticInstance.SetInstance(AppMockers.LanguageDictObjectMocker);
        }
    }
}
