﻿namespace SingularityForensic.FileExplorer.ViewModels {
    //public class FileDetailTabModel : BindableBase,ITabModel {
    //    public FileDetailTabModel(IFileExplorerServiceProvider fsProvider) {
    //        if(fsProvider == null) {
    //            throw new ArgumentNullException(nameof(fsProvider));   
    //        }

    //        this.FSProvider = fsProvider;
    //    }

    //    public IFileExplorerServiceProvider FSProvider { get; }
    //    public string Header => ServiceProvider.Current?.GetInstance<ILanguageService>()?.FindResourceString("DetailInfo");

    //    private string _text;
    //    public string Text {
    //        get => _text;
    //        set => SetProperty(ref _text, value);
    //    }

    //    private IFileDetailInfoProvider _provider;
    //    public IFileDetailInfoProvider Provider => _provider;
    //        //?? (_provider = FSProvider.CaseEvidenceServiceProvider.GetInstance<IFileDetailInfoProvider>());

    //    private IFile _file;
    //    public IFile File {
    //        get {
    //            return _file;
    //        }
    //        set {
    //            if (value != null) {
    //                var comma = ServiceProvider.Current?.GetInstance<ILanguageService>()?.FindResourceString("Comma");
    //                var sb = new StringBuilder();
    //                sb.AppendLine($"{LanguageService.FindResourceString("BasicFileInfo")}");
    //                sb.AppendLine($"{LanguageService.FindResourceString("FileName")}{comma}{value.Name}");
    //                sb.AppendLine($"{LanguageService.FindResourceString("FileSize")}{comma}{value.Size}{LanguageService.FindResourceString("Byte")}");
    //                sb.AppendLine();

    //                if(Provider != null) {
    //                    sb.Append(Provider.GetAttachedInfo(File));
    //                }
    //                Text = sb.ToString();
    //            }
    //            SetProperty(ref _file, value);
    //        }
    //    }

    //    //public event EventHandler<IFile> FileChanged;
    //}
}
