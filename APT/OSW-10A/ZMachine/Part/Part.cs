
namespace Machine
{
    public class Part : SM
    {
        virtual public void Reset     (){} //리셑 버튼 눌렀을때 타는 함수.

        //Running Functions.
        virtual public bool ToStopCon (){return false ;} //스탑을 하기 위한 조건을 보는 함수.
        virtual public bool ToStartCon(){return false ;} //스타트를 하기 위한 조건을 보는 함수.
        virtual public bool ToStart   (){return false ;} //스타트를 하기 위한 함수.
        virtual public bool ToStop    (){return false ;} //스탑을 하기 위한 함수.
        virtual public bool Autorun   (){return false ;} //오토런닝시에 계속 타는 함수.

        virtual public int    GetHomeStep    (){return 0;} virtual public int GetPreHomeStep   (){return 0;} virtual public void InitHomeStep (){}
        virtual public int    GetToStartStep (){return 0;} virtual public int GetPreToStartStep(){return 0;}
        virtual public int    GetSeqStep     (){return 0;} virtual public int GetPreSeqStep    (){return 0;}
        virtual public int    GetCycleStep   (){return 0;} virtual public int GetPreCycleStep  (){return 0;} virtual public void InitCycleStep(){}
        virtual public int    GetToStopStep  (){return 0;} virtual public int GetPreToStopStep (){return 0;}

        virtual public string GetCrntCycleName(         ){return "" ;}
        virtual public string GetCycleName    (int _iSeq){return "" ;}
        virtual public double GetCycleTime    (int _iSeq){return 0.0;}
        virtual public string GetPartName     (         ){return "" ;}
                                              
        virtual public int    GetCycleMaxCnt  (         ){return 0;} //해당파트의 싸이클 갯수 리턴.

        virtual public void   Update(){}                     
    }
}
