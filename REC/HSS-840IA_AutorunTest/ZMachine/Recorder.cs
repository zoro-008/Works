using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.ScreenCapture;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using COMMON;
using SML;


namespace Machine
{
    //Ver 1.0.3.0 모니터 화면 녹화
    public class Recorder
    {
        private ScreenCaptureJob         screenCaptureJob;
        //private Size                     workingArea;
        private Rectangle                     workingArea;
        private Rectangle                capturedRectangle;
        private string                   sOutputPath;
        public  bool                     bRecording; //녹화중이면 True, 녹화종료는 false
        public  bool                     bPaused = false; 

        public Recorder()
        {
            screenCaptureJob = new ScreenCaptureJob();
        }

        public void Close()
        {
            screenCaptureJob.Dispose();
        }
        //화면 녹화
        private void SetRecordPreferences(String _sOutputPath)
        {
            screenCaptureJob.ShowFlashingBoundary = true;
            //screenCaptureJob.ShowCountdown = true;
            screenCaptureJob.CaptureMouseCursor = true;
            screenCaptureJob.ShowCountdown = false;
            screenCaptureJob.ScreenCaptureVideoProfile.Quality = 50;//영상 품질 50으로 해야 용량 작아짐.

            screenCaptureJob.OutputPath = _sOutputPath;

            /*
            * PrimaryMonitorSize - Full screen
            * WorkingArea.Size - Full screen of the app (current window)
            */

            //workingArea = SystemInformation.PrimaryMonitorSize;
            workingArea = SystemInformation.VirtualScreen;
            capturedRectangle = new Rectangle(0, 0, workingArea.Width, workingArea.Height);
            //capturedRectangle = new Rectangle(0, 0, 3840, 1080);

            screenCaptureJob.CaptureRectangle = capturedRectangle;
        }
        public void StartRecording()
        {
            sOutputPath = OM.CmnOptn.sRecordPath;
            if (sOutputPath == "")
            {
                sOutputPath = "D:\\Record";
            }
            DirectoryInfo RecDir = new DirectoryInfo(sOutputPath);
            if (!RecDir.Exists)
            {
                RecDir.Create();
            }
            DeleteFileByDay(30);

            if(screenCaptureJob.Status == RecordStatus.NotStarted) //시작 전이면 Start
            {
                SetRecordPreferences(sOutputPath);
                screenCaptureJob.Start();
            }
            else if(screenCaptureJob.Status == RecordStatus.Paused) //Pause 상태면 Resume
            {
                screenCaptureJob.Resume();
            }
            
            bRecording = true;
            bPaused = false;
        }

        public void StopRecording()
        {
            //Running이거나 Pause 상태일때만 Stop
            if (screenCaptureJob.Status == RecordStatus.Running ||
                screenCaptureJob.Status == RecordStatus.Paused)
            {
                screenCaptureJob.Stop();
                bRecording = false;
                bPaused = false;
            }
        }

        public void PauseRecording()
        {
            //Running 상태일때만 Pause
            if (screenCaptureJob.Status == RecordStatus.Running)
            {
                screenCaptureJob.Pause();
                bPaused = true;
            }
        }

        internal void DeleteFileByDay(int _iKeepDay)
        {
            try
            {
                //파일 경로 확인
                string sDeletePath = "";

                DirectoryInfo ReportFolder = new DirectoryInfo(sOutputPath);
               
                foreach(FileInfo file in ReportFolder.GetFiles())
                {
                    //확장자 확인
                    if(file.Extension != ".xesc")
                    {
                        continue;
                    }
                
                    //파일 생성 일자를 기준으로 유지 일자보다 오래 되었으면 해당 파일 삭제
                    if(file.CreationTime < DateTime.Now.AddDays(-(_iKeepDay)))
                    {
                        file.Delete();
                    }
                }
                
                ReportFolder = null;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void RecUpdate()
        {
            switch (SEQ._iSeqStat)
            {
                case EN_SEQ_STAT.Running: StartRecording(); break;
                case EN_SEQ_STAT.Error  : PauseRecording(); break;
                case EN_SEQ_STAT.Stop   : PauseRecording(); break;
                case EN_SEQ_STAT.WorkEnd: StopRecording (); break;

            }
        }
    }

}

