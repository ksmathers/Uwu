using System;
using System.Collections.Generic;
using System.Text;

namespace Uwu.Core
{
    /// <summary>
    /// Millisecond deltatime initialized to 0 on the first call, then returns time elapsed from that starting point
    /// on each subsequent call.
    /// </summary>
    public class Runtime
    {
        static int startTime = 0;

        /// <summary>
        /// Returns elapsed run time in milliseconds starting from 0
        /// </summary>
        public static int Milliseconds {
            get {
                int uptime = System.Environment.TickCount;
                if (startTime == 0) startTime = uptime;
                return uptime - startTime;
            }
        }
    }
}
