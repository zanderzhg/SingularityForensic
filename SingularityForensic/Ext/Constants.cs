﻿namespace SingularityForensic.Ext {
    public static partial class Constants {
        /// <summary>
        /// 超级块起始地址;
        /// </summary>
        public const int ExtSuperBlockStartIndex = 1024;

        public const string ExtSuperBlockFieldPrefix = "ExtSuperBlock_";

        public const string ExtGroupDescFieldPrefix = "ExtGroupDesc_";

        public const string StreamParserGUID_Ext = nameof(StreamParserGUID_Ext);

        public const string PartitionKey_Ext = nameof(PartitionKey_Ext);

        public const string PartitionType_Ext = nameof(PartitionType_Ext);


        public const string PartitionStokenTag_ExtPartInfo = nameof(PartitionStokenTag_ExtPartInfo);



        public const string DirectoryKey_Ext = nameof(DirectoryKey_Ext);


        public const string DirectoryType_Ext = nameof(DirectoryType_Ext);



        public const string FileStokenTag_ExtFileInfo = nameof(FileStokenTag_ExtFileInfo);


        public const string RegularFileKey_Ext = nameof(RegularFileKey_Ext);


        public const string RegularFileType_Ext = nameof(RegularFileType_Ext);

    }
    /// <summary>
    /// 语言部分;
    /// </summary>
    public static partial class Constants {

        public const string DisplayName_ExtSuperBlock = nameof(DisplayName_ExtSuperBlock);
        public const string DisplayName_ExtGroupDesc = nameof(DisplayName_ExtGroupDesc);
    }
}
