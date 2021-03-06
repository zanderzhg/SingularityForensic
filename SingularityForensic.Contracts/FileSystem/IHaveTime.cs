﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Contracts.FileSystem {
    /// <summary>
    /// 具有M/A/C时间;
    /// </summary>
    public interface IHaveFileTime {
        DateTime? ModifiedTime { get; }             //最后修改时间;
        DateTime? AccessedTime { get; }             //最后访问时间;
        DateTime? CreateTime { get; }               //创建时间;

        
    }

   
    public interface IHaveExtendTime {
        /// <summary>
        /// 拓展接口,获得其它时间;
        /// </summary>
        /// <param name="TimeLabel">时间标签</param>
        /// <returns></returns>
        DateTime? GetExtensionTime(string timeLabel);
    }

    
}
