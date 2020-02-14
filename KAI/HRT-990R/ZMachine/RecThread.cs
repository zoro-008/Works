using COMMON;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Machine
{
    static class ThreadRecorder 
    {
        static Thread RecThread  = new Thread(new ThreadStart(RecUpdate));
        static public double m_dThreadCycleTime ; public static double _dThreadCycleTime{get{return m_dThreadCycleTime ;}}

        public static void Init()
        {
            RecThread.Priority = ThreadPriority.Lowest;
            RecThread.Start();
        }

        public static void Close()
        {
            if(bRecording)StopRec();
            RecThread.Abort();
            RecThread.Join();
        }

        public static long GetTotalFreeSpaceByte(string driveName)//D:\
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }

        public static double GetTotalFreeSpaceGB(string driveName)//D:\
        {
            long lSpaceB = GetTotalFreeSpaceByte(driveName);
            if(lSpaceB < 0) return -1 ;
            double dSpaceGB = lSpaceB / 1000.0 / 1000.0 / 1000.0 ;

            return dSpaceGB;
        }



        public static bool DeleteOldFile(string _sPath , string _sExt , int _iCnt) //D:\ , "*.avi" , 1
        {
            FileInfo[] Files = new DirectoryInfo(_sPath).GetFiles(_sExt);
            
            if (Files.Length != 0)
            {
                FileInfo[] OlestFiles = Files.OrderBy(fi => fi.LastWriteTime).Take(_iCnt).ToArray();
                
                foreach(FileInfo file in OlestFiles)
                {
                    file.Delete();
                }

                return true ;
            }
            return false ;
        }

        public static bool ExistFile(string _sPath , string _sExt) //D:\ , "*.avi" , 1
        {
            FileInfo[] Files = new DirectoryInfo(_sPath).GetFiles(_sExt);
            
            if (Files.Length != 0)
            {
                return true ;
            }
            return false ;
        }


        static FGetImg GetImg ;
        static double dFrameGab = 0.0;
        static VideoWriter OpenCV_video;
        static double dPreTime = CTimer.GetTime_us();
        static double dCrntTime = CTimer.GetTime_us();
        static double dRecStart = CTimer.GetTime_us();
        static bool bRecording = false;
        static int iFrameCnt = 0;
        static int iPreFrameCnt = 0;
        static object StopLock = new object();
        public delegate Mat FGetImg() ;
        public static void StartRec(FGetImg _fGetImg, double _dFrameRate)
        {
            if (_dFrameRate < 1) return;
            if (_fGetImg == null) return;
            GetImg = _fGetImg ;
            if (GetImg() == null) return;
            //bool bEmpty = GetImg().IsEmpty;
            dFrameGab = 1000 / _dFrameRate;
            string sRecPath = "D:\\Rec\\";
            string sPath = sRecPath + DateTime.Now.ToString("yyyyMMdd_HHmmss.avi");
            Size size = new Size();
            size.Width = GetImg().Width;
            size.Height = GetImg().Height;
            StopRec();
            //해당 패스 만들고.
            if (!Directory.Exists(Path.GetDirectoryName(sPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(sPath));
            }

            //디스크 공간 계산 해서 지움.
            while(GetTotalFreeSpaceGB(@"D:\")< 10 && ExistFile(sRecPath , "*.avi"))
            {
                DeleteOldFile(sRecPath , "*.avi" , 1);
            }

            //OpenCV_video = new VideoWriter(sPath, VideoWriter.Fourcc('M', 'J', 'P', 'G'), 30, size, true); // 30초에 타이머인터벌 20 
            OpenCV_video = new VideoWriter(sPath, VideoWriter.Fourcc('X', 'V', 'I', 'D'), 30, size, true); // 30초에 타이머인터벌 20 
            dRecStart = CTimer.GetTime_us();
            iFrameCnt = 0 ;
            iPreFrameCnt = 0 ;
            bRecording = true;
        }        

        public static void StopRec()
        {
            lock(StopLock)//하유 자꾸 스탑할때 싱크 때문에 죽음 ;ㅜㅠ
            {
                bRecording = false ;
                if(OpenCV_video!=null)OpenCV_video.Dispose();
                OpenCV_video = null;
            }
        }
        
        public static void RecUpdate()
        {
            /*
             dCrntTime = CTimer.GetTime_us();
                m_dMainThreadCycleTime = (dCrntTime - dPreTime) / 1000.0;
                dPreTime = dCrntTime;
             */
            
            
            while (true)
            {
                Thread.Sleep(1);
                dCrntTime = CTimer.GetTime_us();
                m_dThreadCycleTime = (dCrntTime-dPreTime)/1000.0 ;
                dPreTime = dCrntTime ;
                if (bRecording)
                {
                    iFrameCnt = (int)((dCrntTime - dRecStart)/1000.0 / dFrameGab) ;
                    if(iFrameCnt != iPreFrameCnt)
                    {
                        lock(StopLock)//하유 자꾸 스탑할때 싱크 때문에 죽음 ;ㅜㅠ
                        {
                            if(OpenCV_video != null)//1440 1080 30프레임 412mb
                            {
                                OpenCV_video.Write(GetImg());
                                
                                iPreFrameCnt = iFrameCnt ;
                            }                        
                        }
                    }
                }
            }
        }
    }
}



/*
 static class ThreadRecorder 
    {
        static Thread RecThread  = new Thread(new ThreadStart(RecUpdate));

        static bool bStartRec= false ;
        public static void StartRec()
        {
            bStartRec = true ;
        }
        static bool bStopRec= false ;
        public static void StopRec()
        {
            bStopRec = true ;

        }
        public static void RecUpdate()
        {

            double dPreTime  = CTimer.GetTime_us();
            double dCrntTime;
            double dRecStart;
            const double dFrameRate = 30.0 ;
            bool   bRecording = false ;
            while (true)
            {
                dCrntTime = CTimer.GetTime_us();
                if(bStartRec)
                {
                    dRecStart = dCrntTime ; 
                    bStartRec = false ;
                    bStopRec  = false ; 
                    bRecording = true ;
                }
                if(bStopRec)
                {
                    bStartRec = false ;
                    bStopRec  = false ;
                    bRecording = false ;
                }
                
                
            }

        }

    }
     */