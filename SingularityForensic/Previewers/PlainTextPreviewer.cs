﻿using SingularityForensic.Contracts.Previewers;
using System;
using System.IO;
using System.Text;
using System.Windows;
using views = SingularityForensic.Previewers.Views;
namespace SingularityForensic.Previewers {
    public class PlainTextPreviewer : IPreviewer {
        public PlainTextPreviewer(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            this.Stream = stream;
        }

        private Stream stream;
        public Stream Stream {
            get {
                return stream;
            }
            private set {
                stream = value;
                if (stream != null) {
                    var sr = new StreamReader(stream,Encoding.UTF8);
                    var txt= sr.ReadToEnd();
                    (View as views.PlainTextPreviewer).LoadString(txt);
                }
                else {
                    (View as views.PlainTextPreviewer).Clear();
                }
                
            }
        }

        private views.PlainTextPreviewer view;
        public UIElement View => view ?? (view = new views.PlainTextPreviewer());

        FrameworkElement IPreviewer.View => throw new NotImplementedException();

        public void Dispose() {
            Stream?.Close();
            Stream = null;
        }
    }
}