﻿using Microsoft.Practices.ServiceLocation;
using Prism.Mef;
using Prism.Modularity;
using SingularityForensic.Contracts.Common;
using SingularityForensic.Previewers;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace SingularityParser {
    public class SingularityBootStrapper : MefBootstrapper {
        protected override void ConfigureAggregateCatalog() {
            base.ConfigureAggregateCatalog();

            ////主模块;
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(MainModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(CaseModule).Assembly));
            //////取证信息模块;
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(InfoModule).Assembly));

            //////功能模块;
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AdbViewerModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AndroidInfoModule).Assembly));
            //////this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(RelevanceModule).Assembly));

            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ITunesModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(HexModule).Assembly));

            //////默认预览器模块;
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(DefaultPreviewerModule).Assembly));

            //文件系统模块;
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FileSystemModule).Assembly));

            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ExplorerModule).Assembly));

            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AndroidFSModule).Assembly));
        }


        protected override IModuleCatalog CreateModuleCatalog() {
            return new ConfigurationModuleCatalog();
        }

        protected override DependencyObject CreateShell() {
            return this.Container.GetExportedValue<IShell>() as DependencyObject;
        }

        protected override void InitializeShell() {
            ServiceProvider.SetServiceProvider(new SingularityForensic.Common.MefServiceProvider(ServiceLocator.Current));
            base.InitializeShell();
            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }


    }
}
