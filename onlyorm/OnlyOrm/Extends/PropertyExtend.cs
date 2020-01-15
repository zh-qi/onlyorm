using System;
using System.Collections.Generic;
using System.Reflection;

namespace OnlyOrm.Exetnds
{
    /// <summary>
    /// 属性拓展类
    /// </summary>
    public static class PropertyExtend
    {
        /// <summary>
        /// 属性拓展方法
        /// </summary>
        public static bool InList<T>(this T t, IEnumerable<T> keys) where T : IComparable
        {
            foreach (var key in keys)
            {
                if (key.Equals(t))
                    return true;
            }
            return false;
        }
    }
}