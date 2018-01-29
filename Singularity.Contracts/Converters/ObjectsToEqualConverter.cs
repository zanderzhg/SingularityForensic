﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Singularity.Contracts.Converters {
    public class ObjectsToEqualConverter : IValueConverter {
        private static ObjectsToEqualConverter staticInstance;
        public static ObjectsToEqualConverter StaticInstance {
            get {
                return staticInstance ??
                    (staticInstance = new ObjectsToEqualConverter());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != null ? value.Equals(parameter) : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value != null) {
                bool val = false;
                if (bool.TryParse(value.ToString(),out val) && val) {
                    return parameter;
                }
            }
            return null;
        }
    }
}