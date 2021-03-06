﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Contracts.Casing {
    public static class Constants {
        //证据项节点类型;
        public const string TreeUnitType_CaseEvidence = nameof(TreeUnitType_CaseEvidence);
        //案件节点类型;
        public const string TreeUnitType_Case = nameof(TreeUnitType_Case);

        //未知镜像案件文件类型;
        public const string UnKnownDeviceImg = nameof(UnKnownDeviceImg);

        //ITunes案件文件(夹)类型;
        public const string ITunesBackUpFolder = nameof(ITunesBackUpFolder);

        /// <summary>
        /// 案件节点类型;
        /// </summary>
        public const string TreeUnitTag_Case = nameof(TreeUnitTag_Case);
        
        /// <summary>
        /// 案件文件节点;
        /// </summary>
        public const string TreeUnitTag_CaseEvidence = nameof(TreeUnitTag_CaseEvidence);

        /// <summary>
        /// 证据项根元素名;
        /// </summary>
        public const string CaseEvidenceRootElemName = nameof(ICaseEvidence);


        public const string RecentCaseRecordsView = nameof(RecentCaseRecordsView);

    }
}
