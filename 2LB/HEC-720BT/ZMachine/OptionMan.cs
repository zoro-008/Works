using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using COMMON;
using SML2;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

//using System.Runtime.InteropServices;
//using SMDll2.CXmlBase;
//using SMD2Define;
//using SMDll2App;

namespace Machine
{
              
    //---------------------------------------------------------------------------
    public static class OM
    {
        enum EN_WORK_MODE { wmNormal = 0, wmHeight = 1 }; //0:정상 작업 1:로더에서 꺼내어 로테이터에서 높이 측정만 하고 다시 넣는다.
        public struct CDevInfo
        {   //device에 대한 Dev_Info
        
        } ;

        public struct CDevOptn             //Device에 따라 바뀌는 옵션.
        {
            public double dSttEmpty    ; //작업 시작 공백
            public double dEndEmpty    ; //작업 종단 공백

            public double dWorkDist    ; //전체 작업의 거리.
            public double dLeftCutMovePitch ; //가시 낼때 미는 거리
            public double dRightCutMovePitch; //가시 낼때 미는 거리

            //public double dTensnBwd    ; //Tension 동작 후 되돌아가는 거리
            public double dCutLtRtDist ; //좌측 칼날 - 우측 칼날 거리
            //public double dSttCutDist  ;   //처음 커팅 할 거리.
            
            //public double dShiftCylStrk;   //Shift Cylinder 스트로크 

            public bool   bShiftWork     ; //넘겨서 작업할건지.
            public double dShiftWorkPos  ; //작업할때 넘길 거리.

            public int    iWorkDelay     ; //칼빵하고 주는 딜레이

            EN_WORK_MODE iWorkMode;

            public double dShiftOfs      ;
            public double dCutBwdOfs     ;//컷팅 하고 뒤로 뺄때 옵셋값

            public int    iTargetCnt     ;//작업 목표수량

            public double dWorkDiameter  ;//자재 지름
        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool  bUsedLine1 ;
            public bool  bUsedLine2 ;
            public bool  bUsedLine3 ;
            public bool  bUsedLine4 ;
            public bool  bUsedLine5 ;
            public bool  bIgnrWork  ;
            public bool  bUseAbsPos ;
        } ;

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode   ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public double dCutIdxDist  ; // 커터에서 인덱스 모터 홈까지의 거리
            public double dTwstCntr    ; //2개 회전 실린더 거리의 중간
            public double dHomeTwstCntr; //Home 센서에서 좌측 회전 실린더 거리의 중간
            public double dTnsnOfs     ; //텐션 오차 범위

            public double dMAXWorkDist ; //옮겨서 작업하는 길이.
            
            //public int iColRowDir;
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public string sModelName;
        } ;
		
		public struct CEqpStat
        {
            public int iNodeCnt; //노드 갯수 카운트
            public int iCttrCnt; //한 노드 당 가시 내는 카운트
            public int iWorkCnt; //완성 자재 카운트
            
        } ;

        public const int MAX_NODE_POS = 100;
        public struct TNodePos//xPos	yPos	zPos	사전공급	이동속도	이동중공급대기시간	이동중공급시간.	이동후히팅
        {

            public double dWrkSttPos; //작업할 가시 위치.
            public int    iDirection; //Left/Right Tool 선택
            public double dDegree   ; //실 돌려주는 위치 
            public int    iWrkCnt   ; //반복작업 갯수
            public double dWrkPitch ; //반복작업 피치.
            //public double Degree  ; //실 돌려주는 각도
        }
        
        public static string m_sCrntDev; //Current open device.
    
        public static CDevInfo DevInfo;
        public static CDevOptn DevOptn;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

        public static TNodePos [] NodePos ;

        public static void Init()
        {
            NodePos = new TNodePos[MAX_NODE_POS];
            //Load
            LoadLastInfo();
            LoadMstOptn();
            LoadEqpOptn();
            LoadCmnOptn();
            LoadEqpStat();
            LoadJobFile(m_sCrntDev);
        }
        public static void Close()
        {
            SaveEqpStat();
            SaveLastInfo();
        }

        public static string GetCrntDev() 
        {
            return m_sCrntDev ; 
        }

        public static void SetCrntDev(string _sName)
        {
            m_sCrntDev = _sName;
        }

        public static void LoadJobFile(string _sDevName) 
        {
            LoadDevInfo(_sDevName);
            LoadDevOptn(_sDevName);
            LoadNodePos(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);
        }
        public static void SaveJobFile(string _sDevName)
        {
            SaveDevInfo(_sDevName);
            SaveDevOptn(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);
        }

        public static void LoadDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";
            CAutoIniFile.LoadStruct<CDevInfo>(sDevInfoPath,"DevInfo",ref DevInfo);          
        }

        public static void SaveDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";
            CAutoIniFile.SaveStruct<CDevInfo>(sDevInfoPath,"DevInfo",ref DevInfo);   
        }

        public static void LoadDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";
            CAutoIniFile.LoadStruct<CDevOptn>(sDevOptnPath,"DevOptn",ref DevOptn);               
        }

        public static void SaveDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";
            CAutoIniFile.SaveStruct<CDevOptn>(sDevOptnPath,"DevOptn",ref DevOptn);
        }

        public static void LoadCmnOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";
            CAutoIniFile.LoadStruct<CCmnOptn>(sCmnOptnPath,"CmnOptn",ref CmnOptn);
        }
        public static void SaveCmnOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";
            CAutoIniFile.SaveStruct<CCmnOptn>(sCmnOptnPath,"CmnOptn",ref CmnOptn);
            
        }

        public static void LoadMstOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";
            CAutoIniFile.LoadStruct<CMstOptn>(sMstOptnPath,"MstOptn",ref MstOptn);
            
        }

        public static void SaveMstOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";
            CAutoIniFile.SaveStruct<CMstOptn>(sMstOptnPath,"MstOptn",ref MstOptn);
            
        }

        //여기부터
        public static void LoadEqpOptn()
        {
            //string sModelName = "HEC-720BT";
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";
            CAutoIniFile.LoadStruct<CEqpOptn>(sEqpOptnPath,"EqpOptn",ref EqpOptn);
        }
        public static void SaveEqpOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";
            CAutoIniFile.SaveStruct<CEqpOptn>(sEqpOptnPath,"EqpOptn",ref EqpOptn);
        }
        public static void LoadEqpStat()
        {
            //string sModelName = "PMK-100";
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.LoadStruct<CEqpStat>(sEqpOptnPath,"EqpStat",ref EqpStat);
        }
        public static void SaveEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.SaveStruct<CEqpStat>(sEqpOptnPath,"EqpStat",ref EqpStat);
        }

        public static void LoadLastInfo() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\LastInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);

            //Load
            IniLastInfo.Load("LAST WORK INFO", "Device",out m_sCrntDev);

            if (m_sCrntDev == "") m_sCrntDev = "NONE";
        }
        public static void SaveLastInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\LastInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);
            
            //Save
            IniLastInfo.Save("LAST WORK INFO", "Device", m_sCrntDev);
        }

        //작업 노드 로드 세이브.
        public static void SaveNodePos(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\NodePos.ini";

            CIniFile IniNodePos = new CIniFile(sDevOptnPath);

            for (int i = 0; i < OM.MAX_NODE_POS; i++)
            {
                IniNodePos.Save(i.ToString(), "dWrkSttPos", NodePos[i].dWrkSttPos);
                IniNodePos.Save(i.ToString(), "iDirection", NodePos[i].iDirection);
                IniNodePos.Save(i.ToString(), "dDegree   ", NodePos[i].dDegree);
                IniNodePos.Save(i.ToString(), "iWrkCnt   ", NodePos[i].iWrkCnt);
                IniNodePos.Save(i.ToString(), "iWrkPitch ", NodePos[i].dWrkPitch);
            }

        }

        public static void LoadNodePos(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\NodePos.ini";

            CIniFile IniNodePos = new CIniFile(sDevOptnPath);

            for (int i = 0; i < OM.MAX_NODE_POS; i++)
            {
                IniNodePos.Load(i.ToString(), "dWrkSttPos", out NodePos[i].dWrkSttPos);
                IniNodePos.Load(i.ToString(), "iDirection", out NodePos[i].iDirection);
                IniNodePos.Load(i.ToString(), "dDegree   ", out NodePos[i].dDegree);
                IniNodePos.Load(i.ToString(), "iWrkCnt   ", out NodePos[i].iWrkCnt);
                IniNodePos.Load(i.ToString(), "iWrkPitch ", out NodePos[i].dWrkPitch);
            }
        }

    
    
    };

    

}
