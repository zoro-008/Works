using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public interface PartInterface
    {
        void Reset     (); //리셑 버튼 눌렀을때 타는 함수.

        //Running Functions.
        bool ToStopCon (); //스탑을 하기 위한 조건을 보는 함수.
        bool ToStartCon(); //스타트를 하기 위한 조건을 보는 함수.
        bool ToStart   (); //스타트를 하기 위한 함수.
        bool ToStop    (); //스탑을 하기 위한 함수.
        bool Autorun   (); //오토런닝시에 계속 타는 함수.

        int          GetHomeStep    (); int GetPreHomeStep   (); void InitHomeStep ();
        int          GetToStartStep (); int GetPreToStartStep();
        int          GetSeqStep     (); int GetPreSeqStep    ();
        int          GetCycleStep   (); int GetPreCycleStep  (); void InitCycleStep();
        int          GetToStopStep  (); int GetPreToStopStep ();

        string       GetCycleName   (int _iSeq);
        double       GetCycleTime   (int _iSeq);
        string       GetPartName    (         );

        int          GetCycleMaxCnt (         ); //해당파트의 싸이클 갯수 리턴.

        void  Update();

    }
}
