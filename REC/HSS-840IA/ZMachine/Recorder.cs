using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Expression.Encoder;
//using Microsoft.Expression.Encoder.Devices;
//using Microsoft.Expression.Encoder.ScreenCapture;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using COMMON;
using SML;


//namespace Machine
//{
//    //Ver 1.0.3.0 모니터 화면 녹화
//    public class Recorder
//    {
//        private ScreenCaptureJob         screenCaptureJob ;
//        private string                   sOutputPath      ;
//        
//        public struct sState
//        {
//            public  bool bRecording; //녹화중이면 True, 녹화종료는 false
//            public  bool bPaused   ;
//            public  bool bStoped   ;
//            
//            public  DateTime dateTime  ;
//
//            private int _iCutHours;
//            public  int  iCutHours
//            {
//                get
//                {
//                         if(_iCutHours < 1  ) _iCutHours =  1;
//                    else if(_iCutHours > 24 ) _iCutHours = 24;
//                    return _iCutHours;
//                }
//                set
//                {
//                    _iCutHours = value;
//                }
//            }
//
//            public void Clear()
//            {
//                bRecording = false;
//                bPaused    = false;
//                bStoped    = false;
//            }
//
//            
//        }
//        public sState state;
//
//        public Recorder()
//        {
//            screenCaptureJob = new ScreenCaptureJob();
//            SetRecordPreferences("");
//            
//        }
//
//        public void Close()
//        {
//            StopRecording();
//            screenCaptureJob.Dispose();
//        }
//
//        /// <summary>
//        /// 화면 녹화 설정
//        /// </summary>
//        /// <param name="_sOutputPath">저장경로</param>
//        /// <param name="_iQuality">1~100</param>
//        private void SetRecordPreferences(String _sOutputPath , int _iQuality = 50)
//        {
//            screenCaptureJob.ShowFlashingBoundary = true;
//            //screenCaptureJob.ShowCountdown      = true;
//            screenCaptureJob.CaptureMouseCursor   = true;
//            screenCaptureJob.ShowCountdown        = false;
//            screenCaptureJob.ScreenCaptureVideoProfile.Quality = _iQuality; //1~100
//            SetOutputPath(_sOutputPath);
//
//            /*
//            * PrimaryMonitorSize - Full screen
//            * WorkingArea.Size - Full screen of the app (current window)
//            */
//
//            Rectangle workingArea = SystemInformation.VirtualScreen;
//            Rectangle capturedRectangle = new Rectangle(0, 0, workingArea.Width, workingArea.Height);
//            //capturedRectangle = new Rectangle(0, 0, 3840, 1080);
//
//            screenCaptureJob.CaptureRectangle = capturedRectangle;
//        }
//
//        public void SetOutputPath(string _sOutputPath = "")
//        {
//            string sOutputPath = _sOutputPath;
//            if (sOutputPath == "") sOutputPath = Directory.GetCurrentDirectory() + @"\Record";
//            DirectoryInfo RecDir = new DirectoryInfo(sOutputPath);
//            if (!RecDir.Exists) RecDir.Create();
//
//            screenCaptureJob.OutputPath = sOutputPath;
//        }
//
//        public void StartRecording(int _iCutHour = 0)
//        {
//            if (screenCaptureJob.Status == RecordStatus.NotStarted) //시작 전이면 Start
//            {
//                //SetRecordPreferences(OM.CmnOptn.sRecordPath);
//                DeleteFileByDay(30);
//                SetOutputPath("D:\\Record\\" + DateTime.Now.ToString("yyyyMMdd"));
//                screenCaptureJob.Start() ;
//                state.bRecording = true  ;
//                state.bPaused    = false ;
//                state.dateTime   = DateTime.Now;
//                state.iCutHours  = _iCutHour   ;
//            }
//            else if(screenCaptureJob.Status == RecordStatus.Paused) //Pause 상태면 Resume
//            {
//                screenCaptureJob.Resume();
//                state.bRecording = true  ;
//                state.bPaused    = false;
//            }
//
//            if(state.bRecording && state.iCutHours > 0){
//                TimeSpan timeDiff = DateTime.Now - state.dateTime;
//                if(timeDiff.TotalHours > state.iCutHours)
//                {
//                    StopRecording();
//                    state.dateTime = DateTime.Now;
//                }
//            }
//        }
//
//        public void StopRecording()
//        {
//            //Running이거나 Pause 상태일때만 Stop
//            if (screenCaptureJob.Status == RecordStatus.Running || 
//                screenCaptureJob.Status == RecordStatus.Paused)
//            {
//                screenCaptureJob.Stop();
//
//                state.Clear();
//            }
//        }
//
//        public void PauseRecording()
//        {
//            //Running 상태일때만 Pause
//            if (screenCaptureJob.Status == RecordStatus.Running)
//            {
//                screenCaptureJob.Pause();
//                state.bPaused = true;
//            }
//        }
//        
//        void DeleteFileByDay(int _iKeepDay)
//        {
//            try
//            {
//                //파일 경로 확인
//                string sDeletePath = "";
//                DirectoryInfo ReportFolder = new DirectoryInfo(sOutputPath);
//                foreach(FileInfo file in ReportFolder.GetFiles())
//                {
//                    //확장자 확인
//                    if(file.Extension != ".xesc") continue;
//                    //파일 생성 일자를 기준으로 유지 일자보다 오래 되었으면 해당 파일 삭제
//                    if(file.CreationTime < DateTime.Now.AddDays(-(_iKeepDay))) file.Delete();
//                }
//                ReportFolder = null;
//            }
//            catch (Exception e)
//            {
//                Debug.WriteLine(e.Message);
//            }
//        }
//        
//
//
//    }
//
//}

