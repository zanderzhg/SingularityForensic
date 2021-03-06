﻿using SingularityForensic.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SingularityForensic.Contracts.Hex {
    public interface IBrushBlockFactory {
        IBrushBlock CreateNewBackgroundBlock(long startOffset,long length,Brush background);
        IBrushBlock CreateNewBackgroundBlock();

        /// <summary>
        /// 区分相邻块的两种颜色;
        /// </summary>
        Brush FirstBrush { get; }
        Brush SecondBrush { get; }
        /// <summary>
        /// 高亮色;
        /// </summary>
        Brush HighLightBrush { get; }
    }

    public class BrushBlockFactory : GenericServiceStaticInstance<IBrushBlockFactory> {
        public static IBrushBlock CreateNewBackgroundBlock(long startOffset, long length, Brush background)
            => Current.CreateNewBackgroundBlock(startOffset, length, background);
        public static IBrushBlock CreateNewBackgroundBlock() => Current.CreateNewBackgroundBlock();

        public static Brush FirstBrush => Current.FirstBrush;
        public static Brush SecondBrush => Current.SecondBrush;
        public static Brush HighLightBrush => Current.HighLightBrush;
    }
}
