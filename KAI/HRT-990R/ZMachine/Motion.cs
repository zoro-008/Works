using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Machine;
using COMMON;
using System.Runtime.InteropServices;
using System.Drawing;

namespace MotionLink
{
    public abstract class CFunction
    {
        [DllImport("user32")]
        public static extern Int32 GetCursorPos(out Point pt);
        [DllImport("user32")]
        public static extern Int32 SetCursorPos(Int32 x, Int32 y);
        public const uint LBUTTONDOWN = 0x00000002;
        public const uint LBUTTONUP   = 0x00000004;
        [DllImport("user32.dll")] // DllImport .
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        static public CFunction MakeFunction(String _sName , params object[] _obParas)
        {
            Assembly asm = Assembly.GetExecutingAssembly() ;
            Type type = Type.GetType("MotionLink." + _sName);
            object obj;
            try
            {
                obj = Activator.CreateInstance(type, _obParas);
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
            
            return obj as CFunction;
        }

        static public List<Type> GetFunctions()
        {
            //SpinnakerSDK_FULL_1.19.0.22_x64.exe
            //OmniDriver-2.46-win64-installer.exe
            //여기서 익셉션 발생시에는 카메라SDK()와 스펙트로미터SDK(2.46)를 버전에 맞게 설치해야함.
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> types = null;
            try
            {
                //List<Type> AsmTypes = assembly.GetTypes().ToList();
                //
                //foreach(Type t in AsmTypes)
                //{
                //    if(!typeof(CFunction).IsAssignableFrom(t)) continue;
                //    if(!t.IsClass                            ) continue;
                //    if(!t.IsAbstract                         ) continue;
                //
                //    types.Add(t);
                //}

                types = assembly.GetTypes().Where
                  (t => ((typeof(CFunction).IsAssignableFrom(t) 
                 && t.IsClass && !t.IsAbstract))).ToList();
                

            }
            catch(Exception _e)
            {
                Log.ShowMessage("Expection" , _e.Message);
            }
            
            return types ;
            
        }

        static public int GetParaCount(string _sName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            List<Type> types = assembly.GetTypes().Where
            (t => ((typeof(CFunction).IsAssignableFrom(t) 
                 && t.IsClass && !t.IsAbstract && t.Name == _sName))).ToList();

            if(types.Count == 0) return 0 ;

            //MemberInfo[] membersInfo = types[0].GetMembers(BindingFlags.DeclaredOnly|
            //                                               BindingFlags.Public      |
            //                                               BindingFlags.Instance    );    

            FieldInfo[] fieldInfos = types[0].GetFields(BindingFlags.DeclaredOnly|
                                                        BindingFlags.NonPublic   |
                                                        BindingFlags.Instance    );

            if(fieldInfos.Length == 0) return 0 ;

            Type paraType = fieldInfos[0].FieldType;

            FieldInfo[] paraFieldInfos = paraType.GetFields(BindingFlags.Public   |
                                                                           BindingFlags.Instance );

            return paraFieldInfos.Length ;

        }

        static public string GetComment(string _sName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
        
            List<Type> types = assembly.GetTypes().Where
            (t => ((typeof(CFunction).IsAssignableFrom(t) 
                 && t.IsClass && !t.IsAbstract && t.Name == _sName))).ToList();
        
            if(types.Count == 0) return "" ;
        
            //MemberInfo[] membersInfo = types[0].GetMembers(BindingFlags.DeclaredOnly|
            //                                               BindingFlags.Public      |
            //                                               BindingFlags.Instance    );    
        
            FieldInfo[] fieldInfos = types[0].GetFields(BindingFlags.DeclaredOnly|
                                                        BindingFlags.Public      |
                                                        BindingFlags.Instance    |
                                                        BindingFlags.Static      );
        
            if(fieldInfos.Length == 0) return "" ;
            string sComment = fieldInfos.First(f => f.Name == "Comment").GetValue(null).ToString();
            
            return sComment ;
        
            //for (int i = 0; i < _obParas.Length; i++)
            //{
            //    if (!SetData(__makeref(_oPara), f[i], _obParas[i]))
            //    {
            //        throw new Exception(i.ToString() + "번 Parameter(" + _obParas[i] + "가 " + memberInfo[i].ReflectedType.ToString() + "변환이 불가입니다.");
            //    }
            //}            
        
        }

        static public bool SetData(TypedReference _Type , FieldInfo f, object _Para)
        {
            bool   bTemp ;
            int    iTemp ;
            uint   uTemp ;
            double dTemp ;
            string sPara = _Para as string ;

                 if(f.FieldType == typeof(bool  )&&bool  .TryParse (sPara , out bTemp))f.SetValueDirect(_Type, bTemp );
            else if(f.FieldType == typeof(int   )&&int   .TryParse (sPara , out iTemp))f.SetValueDirect(_Type, iTemp );
            else if(f.FieldType == typeof(uint  )&&uint  .TryParse (sPara , out uTemp))f.SetValueDirect(_Type, uTemp );
            else if(f.FieldType == typeof(double)&&double.TryParse (sPara , out dTemp))f.SetValueDirect(_Type, dTemp );
            else if(f.FieldType == typeof(string)                                     )f.SetValueDirect(_Type, sPara );
            else                                                                        return false ;

            return true ;
            
        }

        protected int    iRunStep ;   
        protected int    iPreRunStep ;

        protected   string sError; //장비 Run시에 에러발생.
        public      string Error{get{return sError;}}    
        
        public CDelayTimer tmTimeOver = new CDelayTimer();

        //생성자에서 bool Init<T>(ref T _oPara , params object[] _obParas) 기능을 수행하고 싶은데 딱히 방법을 못찾음.
        //생성자 인자에 ref T _oPara , params object[] _obParas 를 가지고 base 로 호출하면 T _oPara 이부분이 안됌 왜 안되는지 모르겠음.
        public CFunction()
        {
        }

        //이렇게 호출 하고 싶은데...딱히 방법이 없네용~~~
        //public CFunction(ref object _oPara , params object[] _obParas)
        //{
        //    MemberInfo[] memberInfo = _oPara.GetType().GetMembers(BindingFlags.DeclaredOnly|
        //                                                          BindingFlags.Public |
        //                                                          BindingFlags.Instance);
        //
        //    if (_obParas.Length != memberInfo.Length)
        //    {
        //        throw new Exception("Parameter갯수가" + memberInfo.Length.ToString() + "개여야 합니다.");
        //    }
        //
        //    Type type = _oPara.GetType();
        //    FieldInfo[] f = type.GetFields(BindingFlags.DeclaredOnly |
        //                                   BindingFlags.Public |
        //                                   BindingFlags.Instance);
        //    for (int i = 0; i < _obParas.Length; i++)
        //    {
        //        if (!SetData(__makeref(_oPara), f[i], _obParas[i])) throw new Exception(i.ToString() + "번 Parameter(" + _obParas[i] + "가 " + memberInfo[i].ReflectedType.ToString() + "변환이 불가입니다.");
        //    }
        //}
        

        static protected void Init<T>(ref T _oPara , params object[] _obParas)
        {
            MemberInfo[] memberInfo = _oPara.GetType().GetMembers(BindingFlags.DeclaredOnly|
                                                                  BindingFlags.Public      |
                                                                  BindingFlags.Instance    );

            if (_obParas.Length != memberInfo.Length)
            {
                throw new Exception("Parameter갯수가" + memberInfo.Length.ToString() + "개여야 합니다.");
            }

            Type type = _oPara.GetType();
            FieldInfo[] f = type.GetFields(BindingFlags.DeclaredOnly |
                                           BindingFlags.Public |
                                           BindingFlags.Instance);
            for (int i = 0; i < _obParas.Length; i++)
            {
                if (!SetData(__makeref(_oPara), f[i], _obParas[i]))
                {
                    throw new Exception(i.ToString() + "번 Parameter(" + _obParas[i] + "가 " + memberInfo[i].ReflectedType.ToString() + "변환이 불가입니다.");
                }
            }            
        }

        

        public void InitRun()
        {
            iPreRunStep = 0 ;
            iRunStep = 10 ;
            sError   = "";

        }

        abstract public bool Run ();
    }

    public class VOID : CFunction
    {
        public const string Comment = "공백용 함수 아무것도 안함.";
        struct TPara
        {
        } 
        TPara Para ;
        public VOID(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }

        }
        override public bool Run()
        {
            
            return true ;
        }      
    }

    //OUTPUT
    public class IO_OUTPUT : CFunction 
    {
        public const string Comment = "출력을 내보냄(int iNo : 4, 8, 10 , bool bOn : true/false)";
        struct TPara
        {
            public int  iNo ;
            public bool bOn ;
        } 
        TPara Para ;

        public IO_OUTPUT(params object[] _obParas)
        {
            //string stu = VOID.sComment ;
            

            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
            if(Para.iNo != 4 && Para.iNo != 8 && Para.iNo != 10)
            {
                //throw new Exception("Output 번호가 4, 8, 10 이외의 번호가 입력되었습니다.");
                throw new Exception("Output 번호가 4, 8, 10 이외의 번호가 입력되었습니다.");
            }
        }
        override public bool Run()
        {
            ML.IO_SetY(Para.iNo,Para.bOn) ;
            return true;
        }           
    }

    //모터구동 종료 확인.
    public class MOTOR_CHECKSTOP : CFunction 
    {
        public const string Comment = "모터의 정지를 확인함(int iAxis : 0,1,2)";
        struct TPara
        {
            public int iAxis ;
        } 
        TPara Para ;

        public MOTOR_CHECKSTOP(params object[] _obParas)
        {
            //string stu = VOID.sComment ;
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            return ML.MT_GetStop(Para.iAxis) ;
        }           
    }

    //서보온 오프.
    public class MOTOR_SERVO : CFunction 
    {
        public const string Comment = "모터 서보를 제어함(int iAxis : 0,1,2 , bool bOn : true/false)";
        struct TPara
        {
            public int  iAxis ;
            public bool bOn   ;
        } 
        TPara Para ;

        public MOTOR_SERVO(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            ML.MT_SetServo(Para.iAxis , Para.bOn);
            return true ;
        }             
    }

    //온도세팅.
    public class HEATER_TEMP : CFunction 
    {
        public const string Comment = "히터 온도를 세팅함(int iTemp : 0~999)";
        struct TPara
        {
            public int  iTemp ;
        } 
        TPara Para ;

        public HEATER_TEMP(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            if(Para.iTemp < 0  )Para.iTemp = 0 ;
            //if(Para.iTemp > 150)Para.iTemp = 150 ;
            if(Para.iTemp > 999)Para.iTemp = 999 ;
            SEQ.Heater.SetTemp(0,Para.iTemp);
            return true ;
        }             
    }

    //카메라 라이브
    //public class CAMERA_LIVE : CFunction 
    //{
    //    struct TPara
    //    {
    //        public bool iOn ;
    //    } 
    //    TPara Para ;
    //
    //    public CAMERA_LIVE(params object[] _obParas)
    //    {
    //        try
    //        {
    //            Init(ref Para ,  _obParas);                           
    //        }
    //        catch(Exception _e)
    //        {
    //            throw _e.InnerException ;
    //        }
    //    }
    //    override public bool Run()
    //    {
    //        return true ;
    //    }             
    //}

    //딜레이.
    public class DELAY : CFunction 
    {
        public const string Comment = "ms 시간지연(int iMs : 0~)";
        struct TPara
        {
            public int iMs ;
        } 
        TPara Para ;

        public DELAY(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }

        CDelayTimer Timer = new CDelayTimer();
        override public bool Run()
        {
            string sTemp ;
            if (tmTimeOver.OnDelay(iRunStep == iPreRunStep , Para.iMs + 1000))
            {
                sTemp = string.Format("    Timeout Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
                sError = sTemp ;
                return false;
            }

            if (iRunStep != iPreRunStep)
            {
                sTemp = string.Format("    Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
            }
            iPreRunStep = iRunStep ;
            
            switch (iRunStep)
            {
                default :
                    sError = "    존재 하지 않는 스텝입니다." ;
                    Eqp.AddMsg(sError);
                    iRunStep = 0 ;
                    return false ;

                case 10 : 
                    if(Para.iMs < 0)Para.iMs = 0 ;
                    Timer.Clear();
                    iRunStep++;
                    return false ;

                case 11 :
                    if(!Timer.OnDelay(Para.iMs)) return false ;
                    return true ;
            }
        }             
    }

    //카메라 레코딩
    public class CAMERA_RECORD : CFunction 
    {
        public const string Comment = "카메라녹화 On/Off (bool bOn : true/false)";
        struct TPara
        {
            public bool bOn ;
        } 
        TPara Para ;

        public CAMERA_RECORD(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            //이건 카메라 구성하고 확인.
            Program.FrmMain.FrmOperation.FrmVision.Rec(Para.bOn);
            return true ;
        }             
    }

    //카메라 레코딩
    public class CAMERA_EXPOSURE : CFunction 
    {
        public const string Comment = "카메라 노출세팅 (int iValue : 4~5000)";
        struct TPara
        {
            public int iValue ;
        } 
        TPara Para ;

        public CAMERA_EXPOSURE(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {   
            //이건 카메라 구성하고 확인.
            if(Para.iValue < 4   ) Para.iValue = 4 ;
            if(Para.iValue > 5000) Para.iValue = 5000 ; //100프레임 한계
            SEQ.Cam.SetExposure((uint)Para.iValue);
            return true ;
        }             
    }
    
    //RPM 조그 구동 및 속도오버라이드 구동 겸용
    public class MOTOR_RPM : CFunction 
    {
        public const string Comment = "Spindle RPM구동 및 변속 (int iSpdRpm : 1~6000)";
        struct TPara
        {
            public int  iSpdRpm  ; 
        } 
        TPara Para ;

        public MOTOR_RPM(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {   
            if(Para.iSpdRpm > 6000) Para.iSpdRpm = 6000 ;
            //딱 완전히 멈춰있을때 와 정속 구간에서만 동작 함.
            //RPM = 6000
            double dSpdRotationPerSec = Para.iSpdRpm /60.0 ; //초당 회전수.dSpdRotationPersec = 100 ;
            double dSpdDegreePerSec   = dSpdRotationPerSec * ML.MT_GetUnitPerRev(mi.RotorT) ; //초당 각도.dSpdDegreePerSec   = 36000 ;

            //가감속 구간 진입시에는 먹통됨...
            if(ML.MT_GetStop(mi.RotorT)) ML.MT_JogVel     (mi.RotorT , dSpdDegreePerSec);
            else                         ML.MT_OverrideVel(mi.RotorT , dSpdDegreePerSec);
            return true ;
        }      
        
    }

    //RPM 조그 구동 및 속도오버라이드 구동 겸용
    public class MOTOR_RPM_ACC : CFunction
    {
        public const string Comment = "Spindle RPM구동 및 변속 (int iSpdRpm : 1~6000, double dAcc)";
        struct TPara
        {
            public int iSpdRpm;
            public double dAcc;

        }
        TPara Para;

        public MOTOR_RPM_ACC(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            if (Para.iSpdRpm > 6000) Para.iSpdRpm = 6000;
            //딱 완전히 멈춰있을때 와 정속 구간에서만 동작 함.
            //RPM = 6000
            double dSpdRotationPerSec = Para.iSpdRpm / 60.0; //초당 회전수.dSpdRotationPersec = 100 ;
            double dSpdDegreePerSec = dSpdRotationPerSec * ML.MT_GetUnitPerRev(mi.RotorT); //초당 각도.dSpdDegreePerSec   = 36000 ;

            //가감속 구간 진입시에는 먹통됨...
            if (ML.MT_GetStop(mi.RotorT)) ML.MT_JogAbs(mi.RotorT, dSpdDegreePerSec,Para.dAcc,Para.dAcc);
            else ML.MT_OverrideVel(mi.RotorT, dSpdDegreePerSec);
            return true;
        }

    }

    //RPM 조그 구동 및 속도오버라이드 구동 겸용
    public class MOTOR_RPM_CW : CFunction 
    {
        public const string Comment = "Spindle RPM구동 및 변속 (int iSpdRpm : 1~6000)";
        struct TPara
        {
            public int  iSpdRpm  ; 
        } 
        TPara Para ;

        public MOTOR_RPM_CW(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {   
            if(Para.iSpdRpm > 6000) Para.iSpdRpm = 6000 ;
            //딱 완전히 멈춰있을때 와 정속 구간에서만 동작 함.
            //RPM = 6000
            double dSpdRotationPerSec = Para.iSpdRpm /60.0 ; //초당 회전수.dSpdRotationPersec = 100 ;
            double dSpdDegreePerSec   = dSpdRotationPerSec * ML.MT_GetUnitPerRev(mi.RotorT) ; //초당 각도.dSpdDegreePerSec   = 36000 ;

            //가감속 구간 진입시에는 먹통됨...
            if(ML.MT_GetStop(mi.RotorT)) ML.MT_JogVel     (mi.RotorT , -dSpdDegreePerSec);
            else                         ML.MT_OverrideVel(mi.RotorT , -dSpdDegreePerSec);
            return true ;
        }      
        
    }

    //모터 스탑.
    public class MOTOR_STOPRPM : CFunction 
    {
        public const string Comment = "Spindle RPM구동 정지";
        struct TPara
        {
        } 
        TPara Para ;

        public MOTOR_STOPRPM(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            ML.MT_Stop(mi.RotorT);
            return true ;
        }             
    }

    //포지션 구동.(RunSpeed)
    public class MOTOR_MOVE : CFunction 
    {
        public const string Comment = "모터 구동 (int iAxis : 0~2 , double dPos : 0~해당축의 리밋까지)";
        struct TPara
        {
            public int    iAxis ;
            public double dPos  ;
        } 
        TPara Para ;

        public MOTOR_MOVE(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            double dPos = Para.dPos;
            if(Para.iAxis == 1)  dPos += OM.CmnOptn.dLsrSttPos ;
            ML.MT_GoAbsRun(Para.iAxis , dPos );
            return true ;
        }             
    }

    //포지션 구동.(SlowSpeed)
    public class MOTOR_MOVESLOW : CFunction 
    {
        public const string Comment = "모터 구동 (int iAxis : 0~2 , double dPos : 0~해당축의 리밋까지)";
        struct TPara
        {
            public int    iAxis ;
            public double dPos  ;
        } 
        TPara Para ;

        public MOTOR_MOVESLOW(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            double dPos = Para.dPos;
            if(Para.iAxis == 1)  dPos += OM.CmnOptn.dLsrSttPos ;
            ML.MT_GoAbsSlow(Para.iAxis , dPos );
            return true ;
        }             
    }
  
    //HOME
    public class MOTOR_HOME : CFunction 
    {
        public const string Comment = "홈동작 구동 (int iAxis : 0~2)";
        struct TPara
        {
            public int  iAxis ; 
        } 
        TPara Para ;

        public MOTOR_HOME(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            ML.MT_GoHome(Para.iAxis);
            return true ;
        }             
    }
  
    //HOME
    public class MOTOR_HOMEDONE : CFunction 
    {
        public const string Comment = "홈동작 완료확인 (int iAxis : 0~2)";
        struct TPara
        {
            public int  iAxis ; 
        } 
        TPara Para ;

        public MOTOR_HOMEDONE(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            return ML.MT_GetHomeDone(Para.iAxis); ;
        }             
    }
 

    public class MOTOR_REPEAT : CFunction 
    {   
        public const string Comment = "모터반복구동 (int iAxis : 0~2 , double dVel : 속도 , double dAcc : 감가속도 , double dPos : 반복구동2번째 위치 , int iRepeatCnt : 반복구동횟수)";
        struct TPara
        {
            public int    iAxis ; 
            public double dVel  ; 
            public double dAcc  ;
            public double dPos  ;
            public int    iRepeatCnt ;
        } 
        TPara Para ;

        public MOTOR_REPEAT(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }

        int iRepeatCnt = 0 ;
        double dStartPos = 0 ;
        override public bool Run()
        {
            string sTemp ;
            if (tmTimeOver.OnDelay(iRunStep == iPreRunStep && SEQ.CheckStop() ,1000))
            {
                sTemp = string.Format("    Timeout Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
                sError = sTemp ;
                return false;
            }

            if (iRunStep != iPreRunStep)
            {
                sTemp = string.Format("    Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
            }
            iPreRunStep = iRunStep ;

            switch(iRunStep)
            {
                default :
                    sError = "    존재 하지 않는 스텝입니다." ;
                    Eqp.AddMsg(sError);
                    iRunStep = 0 ;
                    return false ;

                case 10 : //초기화.
                    iRepeatCnt = 0 ;
                    dStartPos = ML.MT_GetCmdPos(Para.iAxis);
                    //if(Para.iRepeatCnt == 0)
                    //{
                    //    ML.MT_Repeat(Para.iAxis);
                    //    return true;
                    //}
                    iRunStep++;
                    return false ;

                //밑에서씀.
                case 11 :
                    ML.MT_GoAbs(Para.iAxis , Para.dPos , Para.dVel , Para.dAcc , Para.dAcc);
                    iRunStep++;
                    return false ;

                case 12 :
                    if(!ML.MT_GetStop(Para.iAxis)) return false ;
                    ML.MT_GoAbs(Para.iAxis , dStartPos , Para.dVel , Para.dAcc , Para.dAcc);
                    iRunStep++;
                    return false ;

                case 13:
                    if(!ML.MT_GetStop(Para.iAxis)) return false ;
                    iRepeatCnt++;
                    if(iRepeatCnt < Para.iRepeatCnt)
                    {
                        iRunStep = 11 ;
                        return false ;
                    }
                    return true ;
            }
        }                 
    }


    public class MOTOR_REPEAT_REL : CFunction 
    {   
        public const string Comment = "모터반복구동 (int iAxis : 0~2 , double dVel : 속도 , double dAcc : 감가속도 , double dPos : 반복구동 이동량 , int iRepeatCnt : 반복구동횟수)";
        struct TPara
        {
            public int    iAxis ; 
            public double dVel  ; 
            public double dAcc  ;
            public double dPos  ;
            public int    iRepeatCnt ;
        } 
        TPara Para ;

        public MOTOR_REPEAT_REL(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }

        int iRepeatCnt = 0 ;
        double dStartPos = 0 ;
        override public bool Run()
        {
            string sTemp ;
            if (tmTimeOver.OnDelay(iRunStep == iPreRunStep && SEQ.CheckStop() ,1000))
            {
                sTemp = string.Format("    Timeout Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
                sError = sTemp ;
                return false;
            }

            if (iRunStep != iPreRunStep)
            {
                sTemp = string.Format("    Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
            }
            iPreRunStep = iRunStep ;

            switch(iRunStep)
            {
                default :
                    sError = "    존재 하지 않는 스텝입니다." ;
                    Eqp.AddMsg(sError);
                    iRunStep = 0 ;
                    return false ;

                case 10 : //초기화.
                    iRepeatCnt = 0 ;
                    dStartPos = ML.MT_GetCmdPos(Para.iAxis);
                    //if(Para.iRepeatCnt == 0)
                    //{
                    //    ML.MT_Repeat(Para.iAxis);
                    //    return true;
                    //}
                    iRunStep++;
                    return false ;

                //밑에서씀.
                case 11 :
                    ML.MT_GoInc(Para.iAxis ,  Para.dPos , Para.dVel , Para.dAcc , Para.dAcc);
                    iRunStep++;
                    return false ;

                case 12 :
                    if(!ML.MT_GetStop(Para.iAxis)) return false ;
                    ML.MT_GoInc(Para.iAxis , -Para.dPos , Para.dVel , Para.dAcc , Para.dAcc);
                    iRunStep++;
                    return false ;

                case 13:
                    if(!ML.MT_GetStop(Para.iAxis)) return false ;
                    iRepeatCnt++;
                    if(iRepeatCnt < Para.iRepeatCnt)
                    {
                        iRunStep = 11 ;
                        return false ;
                    }
                    return true ;
            }
        }                 
    }

    public class CYLINDER_MOVE : CFunction 
    {   
        public const string Comment = "실린더구동 (int iCyl : 0~3 , bool bFwd : true,false)";
        struct TPara
        {
            public int    iCyl  ; 
            public bool   bFwd  ;
        } 
        TPara Para ;

        public CYLINDER_MOVE(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }

        override public bool Run()
        {
            string sTemp ;
            if (tmTimeOver.OnDelay(iRunStep == iPreRunStep && SEQ.CheckStop() ,1000))
            {
                sTemp = string.Format("    Timeout Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
                sError = sTemp ;
                return false;
            }

            if (iRunStep != iPreRunStep)
            {
                sTemp = string.Format("    Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
            }
            iPreRunStep = iRunStep ;

            switch(iRunStep)
            {
                default :
                    sError = "    존재 하지 않는 스텝입니다." ;
                    Eqp.AddMsg(sError);
                    iRunStep = 0 ;
                    return false ;

                case 10 : //초기화.
                    ML.CL_Move(Para.iCyl , Para.bFwd?fb.Fwd : fb.Bwd);
                    iRunStep++;
                    return false ;

                case 11 :
                    if(!ML.CL_Complete(Para.iCyl , Para.bFwd?fb.Fwd : fb.Bwd)) return false ;
                    return true ;
            }
        }                 
    }

    public class CAMERA_LIVE : CFunction 
    {
        public const string Comment = "CAMERA LIVE(bool bOn : true/false)";
        struct TPara
        {
            public bool bOn;
        } 
        TPara Para ;

        public CAMERA_LIVE(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            FormVision.bLive = Para.bOn;
            SEQ.Cam.SetModeHwTrigger(!Para.bOn);
            if (Para.bOn) SEQ.Cam.Grab();
            
            return true ;
        }             
    }

    public class CAMERA_HWTRIGGER: CFunction 
    {
        public const string Comment = "카메라 하드웨어 트리거 설정(bool bUse : true/false)";
        struct TPara
        {
            public bool bUse ;
        } 
        TPara Para ;

        public CAMERA_HWTRIGGER(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            SEQ.Cam.SetModeHwTrigger(Para.bUse);
            
            return true ;
        }             
    }

    //스펙트로미터 스타트,스탑
    public class SPECTRO_RUN : CFunction
    {
        public const string Comment = "스펙트로미터 동작 시작 또는 정지(bool bRun : true/false)";
        struct TPara
        {
            public bool bRun;
        }
        TPara Para;

        public SPECTRO_RUN(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            Eqp.SpectroRun(Para.bRun);

            return true;
        }
    }

    //스펙트로미터 데이터 저장
    public class SPECTRO_SAVE : CFunction
    {
        public const string Comment = "스펙트로미터 데이터 저장";
        struct TPara
        {

        }
        TPara Para;

        public SPECTRO_SAVE(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            Eqp.SpectroSave();

            return true;
        }
    }

    public class SPECTRO_CHECKSTOP: CFunction 
    {
        public const string Comment = "스펙트럼 정지확인";
        struct TPara
        {
        } 
        TPara Para ;

        public SPECTRO_CHECKSTOP(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }

        CDelayTimer Timer = new CDelayTimer();
        override public bool Run()
        {
            string sTemp;
            int iTimeOut = FormSpectroMain.frmSpectrometer.GetMaxScan() * FormSpectroMain.frmSpectrometer.GetIntegrationTime() + 5000;
            if(iTimeOut < 15000) iTimeOut = 15000;
            //bool   bStep = iPreScanCnt == FormSpectroMain.frmSpectrometer.GetScanCount();
            if (tmTimeOver.OnDelay(iRunStep == iPreRunStep && SEQ.CheckStop(), iTimeOut))
            {
                sTemp = string.Format("    Timeout Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
                sError = sTemp;
                return false;
            }

            if (iRunStep != iPreRunStep)
            {
                sTemp = string.Format("    Cycle Step.iCycle={0:00}", iRunStep);
                Eqp.AddMsg(sTemp);
            }
            iPreRunStep = iRunStep;

            //iPreScanCnt = FormSpectroMain.frmSpectrometer.GetScanCount();
            switch (iRunStep)
            {
                default:
                    sError = "    존재 하지 않는 스텝입니다.";
                    Eqp.AddMsg(sError);
                    iRunStep = 0;
                    return false;

                case 10: //초기화.
                    if(!FormSpectroMain.frmSpectrometer.CheckStop()) return false;
                    Timer.Clear();
                    iRunStep++;
                    return false;

                case 11:
                    if (!Timer.OnDelay(100)) return false;

                    return true;
            }
        }             
    }
    
    public class SPECTRO_SETINTERGRATIONTIME: CFunction 
    {
        public const string Comment = "스펙트럼 완성시간(int iVal : 1~10000)";
        struct TPara
        {
            public int iVal;
        } 
        TPara Para ;

        public SPECTRO_SETINTERGRATIONTIME(params object[] _obParas)
        {
            try
            {
                Init(ref Para ,  _obParas);                           
            }
            catch(Exception _e)
            {
                throw _e.InnerException ;
            }
        }
        override public bool Run()
        {
            FormSpectroMain.frmSpectrometer.SetIntegrationTime(Para.iVal);
            return true;
        }             
    }

    public class SPECTRO_SETMAXSCAN: CFunction
    {
        public const string Comment = "스펙트럼 스캔횟수(int iVal : 0~100)";
        struct TPara
        {
            public int iVal;
        }
        TPara Para;

        public SPECTRO_SETMAXSCAN(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            FormSpectroMain.frmSpectrometer.SetMaxScan(Para.iVal);
            return true;
        }
    }

    public class SPECTRO_SAVE_WAVE: CFunction
    {
        public const string Comment = "스펙트럼 특정파장저장(string sFilename : 파일이름, double dWavelengths : 파장)";
        struct TPara
        {
            public string sFilename   ;
            public double dWavelengths;
        }
        TPara Para;

        public SPECTRO_SAVE_WAVE(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            FormSpectroMain.frmSpectrometer.SpectroSaveWave(Para.sFilename,Para.dWavelengths);
            return true;
        }
    }

    public class SPECTRO_RESET: CFunction
    {
        public const string Comment = "스펙트럼 리셋";
        struct TPara
        {
        }
        TPara Para;

        public SPECTRO_RESET(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            FormSpectroMain.frmSpectrometer.Reset();
            return true;
        }
    }

    public class MOUSE_MOVE : CFunction
    {
        public const string Comment = "마우스 이동(int ix, int iy : 0~)";
        struct TPara
        {
            public int iX;
            public int iY;
        }
        TPara Para;

        public MOUSE_MOVE(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            if (Para.iX < 0) Para.iX = 0;
            if (Para.iY < 0) Para.iY = 0;

            SetCursorPos(Para.iX, Para.iY);
            return true;
        }
    }

    public class MOUSE_CLICK : CFunction
    {
        public const string Comment = "마우스 클릭";
        struct TPara
        {
        }
        TPara Para;

        public MOUSE_CLICK(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            mouse_event(LBUTTONDOWN, 0, 0, 0, 0); // 다운
            mouse_event(LBUTTONUP  , 0, 0, 0, 0); // 업
            return true;
        }
    }

    public class TRIGGER_RESET : CFunction
    {
        public const string Comment = "트리거 리셋";
        struct TPara
        {
        }
        TPara Para;

        public TRIGGER_RESET(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            ML.MT_ResetTrgPos(mi.RotorT);
            return true;
        }
    }

    public class TRIGGER_SET : CFunction
    {
        public const string Comment = "트리거 셋팅(트리거 시간(1~50000usec),포지션,bEncoder(true-엔코더,false-커맨드),bLevel(false-B접점,true-A접점))";
        struct TPara
        {
            public int    iusec    ;//트리거 시간usec
            public double dPos     ;//포지션
            public bool   bEncoder ;//엔코더,커맨드 포지션
            public bool   bLevel   ;//트리거 레벨 0-B접점 1-A접점
        }
        TPara Para;

        public TRIGGER_SET(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            ML.MT_SetTrgAbs(mi.RotorT,true,Para.dPos,Para.iusec,Para.bEncoder,Para.bLevel);
            return true;
        }
    }


    public class JUMP : CFunction
    {
        public const string Comment = "해당 줄로 해당 횟수만큼 점프(이동 줄수(0~현재줄),반복 횟수(0~1000))";

        public int    iCount ;//현재 반복 횟수
        public int    iRow   ;//이동 줄수
        public int    iRepeat;//반복 횟수

        struct TPara
        {
            public int    iRow     ;//이동 줄수(0~현재줄)
            public int    iRepeat  ;//반복 횟수(0~1000)
            
        }
        TPara Para;

        public JUMP(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
                iCount  = 0;
                iRow    = Para.iRow   ;
                iRepeat = Para.iRepeat;
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            if(Para.iRepeat >  1000) Para.iRepeat = 1000;
            if(Para.iRepeat <= 0   ) return true;

            if(iCount >= Para.iRepeat) {
                iCount = 0;
                return true ;
            }
            
            iCount++;
            return false;
        }
    }


    public class SAVE_IMAGE : CFunction
    {
        public const string Comment = "이미지 저장 (bool bUse : true-시작/false-정지, string sFileName : 파일이름 , int iCount 해당 카운트시에 이미지 저장 : 1~6000)";

        struct TPara
        {
            public bool   bUse     ;//이미지 저장 시작,정지
            public string sFileName;//파일이름
            public int    iCount   ;//해당 횟수 마다 이미지 저장
        }
        TPara Para;

        public SAVE_IMAGE(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            if(Para.iCount >  6000) Para.iCount = 6000;
            if(Para.iCount <     1) Para.iCount =    1;

            OM.Info.bImageSave      = Para.bUse      ;
            OM.Info.sImageFileName  = Para.sFileName ;
            OM.Info.iImageSaveCnt   = Para.iCount    ;

            return true;
        }
    }


    public class SAVE_TEMP : CFunction
    {
        public const string Comment = "온도 저장(bool bUse : (true-시작/false-정지), string sFileName : 파일이름, int iInterval : 간격(30~10000ms)), 저장위치(d:\\Heater\\)";

        struct TPara
        {
            public bool   bUse      ; //시작 , 정지
            public string sFileName ; 
            public int    iInterval ;
        }
        TPara Para;

        public SAVE_TEMP(params object[] _obParas)
        {
            try
            {
                Init(ref Para, _obParas);
            }
            catch (Exception _e)
            {
                throw _e.InnerException;
            }
        }
        override public bool Run()
        {
            if(Para.iInterval < 30   ) Para.iInterval = 30   ;
            if(Para.iInterval > 10000) Para.iInterval = 10000;

            OM.Info.bTempSave         = Para.bUse     ;
            OM.Info.sTempFileName     = Para.sFileName;
            OM.Info.iTempSaveInterval = Para.iInterval;
            return true;
        }
    }

}
