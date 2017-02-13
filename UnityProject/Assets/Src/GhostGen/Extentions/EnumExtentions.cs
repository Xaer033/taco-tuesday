using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostGen
{
    public static class EnumExtentions
    {
        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            int lValue = System.Convert.ToInt32(value);
            int lFlag = System.Convert.ToInt32(flag);
            return (lValue & lFlag) != 0;
        }
    }
}
