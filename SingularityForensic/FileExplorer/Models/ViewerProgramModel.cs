﻿using System;
using Prism.Commands;
using Prism.Mvvm;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Common;

namespace SingularityForensic.FileExplorer.Models {
    //查看程序模型;
    public class ViewerProgramModel:BindableBase {
        public ViewerProgramModel(ViewerProgram program) {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            this.Program = program;
        }
        public ViewerProgram Program { get; }
        public string Name {
            get {
                return Program?.ProgramName;
            }
        }

        private static ViewerProgramModel _otherProgram;
        public static ViewerProgramModel OtherProgramModel {
            get {
                if(_otherProgram == null) {
                    _otherProgram = new ViewerProgramModel(new ViewerProgram {
                        ProgramName = ServiceProvider.Current?.GetInstance<ILanguageService>()?.FindResourceString("OtherProgram")
                    });
                }
                return _otherProgram;
            }
        }

        public event EventHandler WatchRequired;

        private DelegateCommand _watchCommand;
        public DelegateCommand WatchCommand =>
            _watchCommand ?? (_watchCommand = new DelegateCommand(() => {
                WatchRequired?.Invoke(this, new EventArgs());
            },() => { return CanSee?.Invoke() ?? false; }));

        public Func<bool> CanSee { get; set; }
    }
}
