using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MotionLink;
using System.Windows.Forms;

namespace Machine
{
    public class CCode
    {
        //실제적인 모션함수들 모음.
        private List<CFunction> Functions       = new List<CFunction>();

        //컴파일시에 에러이고 여러에러를 한번에 보여주기 위해 리스트.
        private List<string>    lsCompileErrs   = new List<string>();
        public  List<string>    CompileErrs     {get{return lsCompileErrs;}}

        //러닝시에 에러.
        private string          sRunError ;
        public  string          RunError{get{return sRunError; }}

        //러닝중인 라인.
        private int             iRunLine ;
        public  int             RunLine {get{return iRunLine ; }}

        //현재 반복 횟수.
        //static private int      iRepeatCount ;
        //public  int             RepeatCount {get{return iRepeatCount ; }}

        //속도 오버라이딩 최고 속도.
        private double          dMaxOverrideVel ;
        
        string[] sLines = null ; //= new string[] ;


        //공백과 커멘트 트림.
        static private String DeleteSpaceComment(string _sLine)
        {
            String sLine = _sLine;
            while(sLine.Contains(" "))
            {
                sLine = sLine.Replace(" ","");
            }

            if(sLine.Contains("//"))
            {
                sLine = sLine.Substring(0,sLine.IndexOf("//"));
            }
            return sLine ;
        }

        //1라인에서 함수이름을 따옴.
        static private String GetFuncName(string _sLine)
        {
            String sRet = "";
            if(_sLine == "") return "VOID" ;
                        
            int iIdx = _sLine.IndexOf("(") ;
            if(iIdx > 0) sRet = _sLine.Substring(0 , _sLine.IndexOf("("));  //sun(1,2,3);

            return sRet ;
        }

        //1라인에서 파라미터들을 분리함.
        static private String[] GetParameters(string _sLine)
        {
            if(_sLine == "") return null ;
            if(!_sLine.Contains("(")) return null ;
            if(!_sLine.Contains(")")) return null ;

            int iStartIdx = _sLine.IndexOf("(")+1 ;
            int iEndIdx   = _sLine.IndexOf(")")   ; 
            int iLength   =  iEndIdx - iStartIdx ;

            String sPara = _sLine.Substring(iStartIdx,iLength );
            if(sPara =="")return null;

            String [] sRet = sPara.Split(',');

            return sRet ;

        }

        public void Reset()
        {
            lsCompileErrs.Clear();
            sRunError = "" ;
        }

        //ToStartCon에서 수행.
        //문법을 확인하고 Codes 리스트에 모션함수클래스를 넣음.
        
        public void SetCode(string [] _sLines)
        {
            sLines = _sLines ;
        }
        private string Compile()
        {
            lsCompileErrs.Clear();
            Functions.Clear();

            string    sReturnMsg = "" ;
            string    sLine           ;
            string    sFuncName       ;
            string[]  sParas          ;
            CFunction Function        ;

            double  dOverrideVel = 0 ;
            dMaxOverrideVel = 0 ;
            for(int i = 0 ; i < sLines.Length ; i++)
            {
                sLine     = DeleteSpaceComment(sLines[i]);
                sFuncName = GetFuncName(sLine);
                sParas    = GetParameters(sLine);
                try
                {
                    Function = CFunction.MakeFunction(sFuncName , sParas);

                    //속도 오버라이딩 함수의 가장 높은 속도를 구해서 속도 분해능을 설정해야 한다.
                    if(Function is MOTOR_RPM)
                    {
                        if(!double.TryParse(sParas[0] , out dOverrideVel))
                        {
                            lsCompileErrs.Add("Line:"+i.ToString() + "-" + "TryParse("+sParas[0].ToString()+") OverrideVel failed");
                            dOverrideVel = 0.0 ;
                            
                        }
                        if (dMaxOverrideVel < dOverrideVel)
                        {
                            dMaxOverrideVel = dOverrideVel;
                        }
                    }
                    if(Function is JUMP)
                    {
                        if(int.TryParse(sParas[0] , out int iRow))
                        {
                            if(iRow >= sLines.Length - 1) {
                                lsCompileErrs.Add("Line:"+i.ToString() + "-[0] It must be less than the total number of lines");
                            }
                        }
                        
                        
                    }
                    Functions.Add(Function);
                }
                catch(Exception _e)
                {
                    lsCompileErrs.Add("Line:"+i.ToString() + "-" + _e.Message);
                }                
            }

            if (Functions.Count     == 0) sReturnMsg = "수행할 코드가 없거나 컴파일이 완료 되지 않았습니다."; 
            if (lsCompileErrs.Count != 0) {
                for(int i=0; i<lsCompileErrs.Count; i++)
                {
                    Eqp.AddMsg(lsCompileErrs[i]);
                }
                
                sReturnMsg = "에러가 존재하여 컴파일이 완료되지 않았습니다."      ;
            }

            return sReturnMsg ;
        }

        //ToStartCon에서 수행.
        public bool RunInit()
        {
            sRunError = Compile();
            if(sRunError != "") return false;

            iRunLine = 0;
            iPreLine = -1;
            sRunError = "";
            
            for (int i = 0; i < Functions.Count; i++)
            {
                //에러와 스텝 초기화.
                Functions[i].InitRun();
            }
            return true ;
        }

        //코드에 있는 모션을 실행.
        //기존 장비들과 다르게 수행 못하면 계속 false이고 에러로 더 진행 안될때도 false로 리턴이지만 RunError를 확인 하면 된다.
        int iPreLine = -1 ;
        public bool Run()
        {
            //런라인 오버플로우.
            if(iRunLine >= Functions.Count)
            {
                sRunError = "Line:"+iRunLine.ToString() + "-" + "수행 라인이 전체코드 라인를 초과했습니다." ;
                return false ;
            }

            //펑션
            bool bRet ;
            if(iRunLine != iPreLine)
            {
                Eqp.AddMsg("Line:" + iRunLine.ToString() + " "+ Functions[iRunLine].GetType().Name + "을 수행합니다.");
            }
            iPreLine = iRunLine ;
            

            if(Functions[iRunLine] is MOTOR_RPM)
            {
                if(ML.MT_GetStop(mi.RotorT))
                {
                    double dSpdRotationPerSec = dMaxOverrideVel / 60.0; //초당 회전수.dSpdRotationPersec = 100 ;
                    double dSpdDegreePerSec = dSpdRotationPerSec * ML.MT_GetUnitPerRev(mi.RotorT); //초당 각도.dSpdDegreePerSec   = 36000 ;
                    ML.MT_SetOverrideMaxVel(mi.RotorT , dSpdDegreePerSec);
                }
            }

            bRet = Functions[iRunLine].Run() ;                

            bool bJump = Functions[iRunLine] is JUMP ;
            if (!bRet)
            {
                if (Functions[iRunLine].Error != "")
                {
                    sRunError = "Line:" + iRunLine.ToString() + "-" + Functions[iRunLine].Error;                    
                }
                if(!bJump) return false;
            }

            if(bJump)
            {
                JUMP jump = (JUMP)Functions[iRunLine];
                //iRepeatCount = jump.iCount;

                if (!bRet) {
                    for (int i = 0; i < Functions.Count; i++)
                    {
                        //에러와 스텝 초기화.
                        Functions[i].InitRun();
                    }
                    iRunLine     = jump.iRow  ;
                    Eqp.AddMsg("Jump:" + iRunLine.ToString() + " ("+ jump.iCount.ToString() + " / " + jump.iRepeat + ")");
                    return false;
                }
            }

            iRunLine++;
            return iRunLine == Functions.Count ;
        }
    }





}