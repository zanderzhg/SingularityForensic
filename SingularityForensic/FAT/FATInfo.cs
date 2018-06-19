﻿using SingularityForensic.Contracts.Common;

namespace SingularityForensic.FAT {
    public class FATInfo: StructFieldDecriptorBase<StFatINFO>,ICustomMemerDecriptor {
        public FATInfo(StFatINFO stFatINFO,long offset):base(stFatINFO) {
            this.Offset = offset;
            
        }
        public long Offset { get; }
    }
}
