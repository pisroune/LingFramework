using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_TankSchool
{
    public static class TimeFormat
    {
        /// <summary>
        /// seconds: 总秒数（可为负；会显示负号）
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string ToHms(int seconds)
        {
            bool neg = seconds < 0;
            long s = Math.Abs((long)seconds);

            long h = s / 3600;
            long m = (s % 3600) / 60;
            long sec = s % 60;

            string core = h > 0
                ? $"{h}:{m:00}:{sec:00}"   // 时:分:秒
                : $"{m}:{sec:00}";         // 分:秒（分钟不补零，秒补零）

            return neg ? "-" + core : core;
        }
    }
}