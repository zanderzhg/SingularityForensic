﻿using SingularityForensic.Controls.Models.Filtering;
using SingularityForensic.Controls.Windows.Filtering;

namespace SingularityForensic.Controls.MessageBoxes.Filtering {
    //过滤字符串讯息;
    public static class FilterStringMessageBox {
        public static bool? Show(ref FilterStringModel fsModel) {
            if(fsModel == null) {
                fsModel = new FilterStringModel();
            }

            var window = new FilterStringWindow(fsModel);
            window.ShowDialog();
            fsModel.IsEnabled = window.FilterResult == true;
            return window.FilterResult;
        }
    }
}
