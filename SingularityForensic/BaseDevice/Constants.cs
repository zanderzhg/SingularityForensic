﻿namespace SingularityForensic.BaseDevice {
    /// <summary>
    /// 模块内部使用的"密码";
    /// </summary>
    public static partial class Constants {
        internal const string DeviceKey_DOS = "2739F2F4-B051-4520-8BF0-7A1511765673";
        internal const string DeviceKey_GPT = "CC4E4F36-0C69-4FD3-919D-BC932AEA0F47";

        internal const string PartEntryKey_DOS = "B96EDB68-2F5C-4711-B7C0-2187C69D9C27";
        internal const string PartEntryKey_GPT = "0EB0D4E8-0313-4849-9803-1E327411525B";

        public const string DeviceStokenTag_DOSDeviceInfo = nameof(DeviceStokenTag_DOSDeviceInfo);
        public const string DeviceStokenTag_GPTDeviceInfo = nameof(DeviceStokenTag_GPTDeviceInfo);

        public const string PartitionEntryStokenTag_DOS = nameof(PartitionEntryStokenTag_DOS);
    }

    public static partial class Constants {
        public const string StreamParser_BaseDevice = nameof(StreamParser_BaseDevice);
        public const string DeviceType_DOS = nameof(DeviceType_DOS);
        public const string DeviceType_GPT = nameof(DeviceType_GPT);

        public const string PartsType_DOS = nameof(PartsType_DOS);
        public const string PartsType_GPT = nameof(PartsType_GPT);
        
        public const string BaseDeviceFieldPrefix_InfoDisk = "BaseDeviceInfoDisk_";

        public const string GptFieldPrefix_EFIInfo = "GPTEFIInfo_";
        public const string GptFieldPrefix_EFIPTable = "GPTEFIPTable_";

        /// <summary>
        /// Dos拓展分区表项;
        /// </summary>
        public const string PartEntryType_DOS_Extended = nameof(PartEntryType_DOS_Extended);
        /// <summary>
        /// Dos主分区表项;
        /// </summary>
        public const string PartEntryType_DOS_Main = nameof(PartEntryType_DOS_Main);
        /// <summary>
        /// Dos逻辑分区表项;
        /// </summary>
        public const string PartEntryType_DOS_Logic = nameof(PartEntryType_DOS_Logic);
        /// <summary>
        /// Dos逻辑分区表项;
        /// </summary>
        public const string PartEntryType_DOS_Error = nameof(PartEntryType_DOS_Error);

        /// <summary>
        /// GPT分区表项;
        /// </summary>
        public const string PartEntryType_GPT = nameof(PartEntryType_GPT);


        public const string DisplayName_EFIInfo = nameof(DisplayName_EFIInfo);
        public const string DisplayName_EFIPTable = nameof(DisplayName_EFIPTable);

        public const string DisplayName_InfoDisk = nameof(DisplayName_InfoDisk);


    }
}
