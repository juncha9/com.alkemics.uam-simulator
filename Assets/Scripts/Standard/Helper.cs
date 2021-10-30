using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UAM
{
    public static class Helper
    {

        public static long LogTimerStart(string message = "")
        {
            if (!string.IsNullOrEmpty(message))
            {
                Debug.Log("[Timer] Start [" + message + "]");
            }
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static void LogTimerEnd(long startTime, string message = "")
        {
            Debug.Log($"[Timer] End [" + message + $"]: <color=yellow>{(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime}</color>ms");
        }

    }
}
