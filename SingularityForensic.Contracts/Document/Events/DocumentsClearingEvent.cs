﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Contracts.Document.Events {
    public class DocumentsClearingEvent: PubSubEvent<(CancelEventArgs cancelArgs,IDocumentService owner)> {
    }
}
