﻿namespace SingularityForensic.Casing {
    /// <summary>
    /// 树形相关;
    /// </summary>
    public static partial class Constants {
        public const string ContextCommandItemGUID_OpenCasePathFolder = nameof(ContextCommandItemGUID_OpenCasePathFolder);
        public const string ContextCommandItemGUID_ShowCaseProperty = nameof(ContextCommandItemGUID_ShowCaseProperty);
        public const string ContextCommandItemGUID_ShowCaseEvidenceProperty = nameof(ContextCommandItemGUID_ShowCaseEvidenceProperty);
        public const string ContextCommandItemGUID_RemoveCaseProperty = nameof(ContextCommandItemGUID_RemoveCaseProperty);

        public const string TBButtonGUID__OpenCase = nameof(TBButtonGUID__OpenCase);


        public const string TBButtonGUID__CloseCase = nameof(TBButtonGUID__CloseCase);

        public const string TBButtonGUID__CreateCase = nameof(TBButtonGUID__CreateCase);

    }

    /// <summary>
    /// 语言相关;
    /// </summary>
    public static partial class Constants {
        //打开案件文件路径;
        public const string ContextCommandName_OpenCasePathFolder = nameof(ContextCommandName_OpenCasePathFolder);

        //打开案件文件路径;
        public const string ContextCommandName_RemoveCaseEvidence = nameof(ContextCommandName_RemoveCaseEvidence);

        //显示案件属性;
        public const string ShowCaseProperty = nameof(ShowCaseProperty);

        //案件模块正在被加载;
        public const string CaseModuleBeingLoaded = nameof(CaseModuleBeingLoaded);

        //案件创建失败;
        public const string CaseConstructingFailed = nameof(CaseConstructingFailed);

        //支持的案件文件后缀类型;
        public const string SupportedCaseFileType = nameof(SupportedCaseFileType);

        //确认是否关闭案件;
        public const string ConfirmToCloseAndOpen = nameof(ConfirmToCloseAndOpen);

        //打开案件失败;
        public const string FailedToOpenCase = nameof(FailedToOpenCase);

        //打开案件;
        public const string LoadingCase = nameof(LoadingCase);

        //预设案件文件名称;
        public const string DefaultCaseName = nameof(DefaultCaseName);

        //案件文件属性;
        public const string CaseEvidenceProperty = nameof(CaseEvidenceProperty);

        //案件文件物理后缀;
        public const string CaseFileExtention = ".sfproj";

        
        public const string TBButtonToolTip_OpenCase = nameof(TBButtonToolTip_OpenCase);

        public const string MenuItemText_OpenCase = nameof(MenuItemText_OpenCase);

        public const string TBButtonToolTip_CloseCase = nameof(TBButtonToolTip_CloseCase);

        public const string MenuItemText_CloseCase = nameof(MenuItemText_CloseCase);

        public const string TBButtonToolTip__CreateCase = nameof(TBButtonToolTip__CreateCase);
        
        public const string MenuItemText_CreateCase = nameof(MenuItemText_CreateCase);

        public const string MsgText_ConfirmToRemoveEvidence = nameof(MsgText_ConfirmToRemoveEvidence);

    }
}
