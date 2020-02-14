using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace COMMON
{
    public class CTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out Int64 lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out Int64 lpFrequency);

        private Int64 iStartTime, iEndTime , iPreStartTime;
        static private Int64 freq;

        // Constructor
        public CTimer()
        {
            iStartTime    = 0;
            iEndTime      = 0;
            iPreStartTime = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }
        }

        // Start the timer
        public void Start()
        {
            // lets do the waiting threads there work
            //Thread.Sleep(0);
            iPreStartTime = iStartTime ;
            QueryPerformanceCounter(out iStartTime);
            iEndTime = iStartTime;
        }

        // Stop the timer
        public void End()
        {
            QueryPerformanceCounter(out iEndTime);
        }

        // Returns the duration of the timer (in seconds)
        public double Duration
        {
            get 
            {
                if (iStartTime == iEndTime)
                {
                    return 0.0 ;
                }
                return (double)(iEndTime - iStartTime) / (double)freq;                
            }
        }

        public double RepeatTime
        {
            get 
            {
                if (iPreStartTime == iStartTime)
                {
                    return 0.0 ;
                }
                return (double)(iPreStartTime - iStartTime) / (double)freq;                
            }
        }

        static public double GetTime()
        {
            Int64 CrntTime;
            QueryPerformanceCounter(out CrntTime);
            return ((double)CrntTime / (double)freq) * 1000;
        }
        static public double GetTime_us()
        {
            Int64 CrntTime;
            QueryPerformanceCounter(out CrntTime);
            return ((double)CrntTime / (double)freq) * 1000000;
        }
    }

    public class CDelayTimer
    {
        long m_lPreTickTime;
        public void Clear()
        {
            m_lPreTickTime = System.DateTime.Now.Ticks;
        }
        public bool OnDelay(int _iSetDelay_ms)
        {
            long lCrntTime = System.DateTime.Now.Ticks;
            double dTimeGap = (lCrntTime - m_lPreTickTime) / 10000.0f;

            if (dTimeGap >= _iSetDelay_ms) return true;
            return false;
        }
        public bool OnDelay(bool _bSeq, int _iSetDelay_ms)
        {
            if (_bSeq)
            {
                return OnDelay(_iSetDelay_ms);
            }
            else
            {
                Clear();
                return false;
            }
        }
    }

    public class CCycleTimer
    {
        long m_lPreTickTime;
        public void Clear()
        {
            m_lPreTickTime = System.DateTime.Now.Ticks;
        }
        public double CheckTime_ms(bool _bReset = true)
        {
            long lCrntTime = System.DateTime.Now.Ticks;
            double dTimeGap = (lCrntTime - m_lPreTickTime) / 10000.0f;

            if (_bReset) Clear();

            return dTimeGap;
        }
        public double CheckTime_s(bool _bReset = true)
        {
            return CheckTime_ms(_bReset) / 1000.0f;
        }

    }
}
