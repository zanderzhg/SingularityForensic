﻿using CDFC.Util;

namespace SingularityForensic.Contracts.FileExplorer {
    public class DefaultRowBuilder : GenericStaticInstance<DefaultRowBuilder>, IRowBuilder {
        public IFileRow BuildRow(IFile file) {
            return new FileRow<IFile>(file);
        }
    }
}
