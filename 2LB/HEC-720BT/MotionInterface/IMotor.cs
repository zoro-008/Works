﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MotionInterface
{
    public enum EN_FIND_EDGE_SIGNAL:uint
    {
        PosEndLimit                            = 0,            // +Elm(End limit) +방향 리미트 센서 신호
        NegEndLimit                            = 1,            // -Elm(End limit) -방향 리미트 센서 신호
        PosSloLimit                            = 2,            // +Slm(Slow Down limit) 신호 - 사용하지 않음
        NegSloLimit                            = 3,            // -Slm(Slow Down limit) 신호 - 사용하지 않음
        HomeSensor                             = 4,            // IN0(ORG)  원점 센서 신호
        EncodZPhase                            = 5,            // IN1(Z상)  Encoder Z상 신호
    };

    public enum EN_HOME_SIGNAL : uint
    { 
        PlusLimit  = 0 , 
        MinasLimit = 1 , 
        Home       = 4
    }
    public enum EN_ZSIGNAL_MATHODE: uint 
    { 
        NotUse         = 0 ,
        PlusDirection  = 1 ,
        MinasDirection = 2
    }

    //인터페이스는 접근 제한자가 없는 멤버 함수만 있다.
    //인터페이스 상속시에는 인터페이스에 기술되어 있는 함수들이 꼭 퍼블릭으로 존재해야 한다.
    //인터페이스는 다중상속이 가능하고 인터페이스 다중 상속시에는 먼저 클래스를 상속하고 인터페이스를 상속해야 한다.
    public interface IMotor
    {

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        bool Init();
        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        bool Close();
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지여부</returns>
        bool GetStop();

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        void SetServo(bool _bOn);
        /// <summary>
        /// 서보상태를 받아옴.
        /// </summary>
        /// <returns>온오프</returns>
        bool GetServo();
        /// <summary>
        /// 리셑 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        void SetReset(bool _bOn);

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        double GetCmdPos();
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        double GetEncPos();
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        void SetPos(double _dPos);

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        bool GetHomeSnsr();
        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        bool GetNLimSnsr();
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        bool GetPLimSnsr();
        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        bool GetZphaseSgnl();
        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        bool GetInPosSgnl(); //Servo Pack InPosition Signal.
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        bool GetAlarmSgnl();

        //Home씨퀜스.

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        bool GoHome(double _dHomeVelFirst, double _dHomeVelLast, double _dHomeAccFirst);
        /// <summary>
        /// 홈동작 완료 확인.
        /// </summary>
        /// <returns>홈동작 완료여부</returns>
        bool GetHomeDone();
        /// <summary>
        /// 강제로 홈돈 시그널 세팅.
        /// </summary>
        /// <param name="_bOn">세팅 값.</param>
        void SetHomeDone(bool _bOn);
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        void StopHome();
        /// <summary>
        /// 홈이 진행중인지 확인.
        /// </summary>
        bool GetHoming();


        //Motion Functions.
        /// <summary>
        /// 모터 정지 명령.
        /// </summary>
        void Stop();
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        void EmgStop(); //Stop Without Deceleration.

        //내부 설정에 따라 Acc Dcc는 가감속율 즉 초당 증가pulse단위 
        /// <summary>
        /// +방향 조그 이동.
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스.</param>
        /// <param name="_dAcc">구동 가속율 펄스 </param>
        /// <param name="_dDec">감속율 펄스</param>
        void JogP(double _dVel, double _dAcc, double _dDec); //Jogging to CW.
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        void JogN(double _dVel, double _dAcc, double _dDec); //Jogging to CCW.
        /// <summary>
        /// 홈센서 기준 절대 위치로 이동.
        /// </summary>
        /// <param name="_dPos">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        void GoAbs(double _dPos, double _dVel, double _dAcc, double _dDec);  //abs move


        /// <summary>
        /// 홈센서 기준 절대 위치로 다축 이동.
        /// </summary>
        /// <param name="_iMotrPhyAdds">이동할축들 0번이 마스터</param>
        /// <param name="_dPoses">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        void GoMultiAbs(int[] _iMotrPhyAdds, double[] _dPoses, double _dVel, double _dAcc, double _dDec);

        /// <summary>
        /// 시그널 찾아서 감속 정지한다..
        /// </summary>
        /// <param name="_dVel">구동 속도 펄스단위</param>
        /// <param name="_dAcc">구동 가속 펄스단위</param>
        /// <param name="_eSignal">사용 시그널</param>
        /// <param name="_bEdgeUp">업엣지인지 다운엣지인지.</param>
        /// <returns></returns>
        void FindEdgeStop(double _dVel, double _dAcc, EN_FIND_EDGE_SIGNAL _eSignal, bool _bEdgeUp, bool _bEmgStop);


        /// <summary>
        /// 현재 파라미터들을 아진 함수를 이용하여 세팅함.
        /// </summary>
        void ApplyPara(double _dPulsePerUnit);


        //특수기능
        /// <summary>
        /// 하드웨어 트리거를 발생시킨다.
        /// </summary>
        /// <param name="_daPos">발생시킬 트리거 위치들</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커멘드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        void SetTrgPos(double[] _daPos, double _dTrgTime, bool _bActual, bool _bOnLevel);    //Target Actual Position or Command Position.
        /// <summary>
        /// SetTrgPos로 세팅된 포지션 리셑
        /// </summary>
        void ResetTrgPos();
        /// <summary>
        /// 테스트로 한번의 트리거를 출력.
        /// </summary>
        /// <param name="_bOnLevel">출력 레벨</param>
        /// <param name="_iTime">시간us</param>
        void OneShotTrg(bool _bOnLevel, int _iTime);


        // 다축제어 라서 static이여야함.
        // 보간 구동을 위해 추가
        void ContiSetAxisMap(int _iCoord, uint _uiAxisCnt, int[] _iaAxisNo);
        void ContiSetAbsRelMode(int _iCoord, uint _uiAbsRelMode);
        void ContiBeginNode(int _iCoord);
        void ContiEndNode(int _iCoord);
        void ContiStart(int _iCoord, uint _uiProfileset, int _iAngle);
        int  GetContCrntIdx(int _iCoord);
        void LineMove(int _iCoord, double[] _daEndPos, double _dVel, double _dAcc, double _dDec);

        void CircleCenterMove(int _iCoord, int[] _iaAxisNo, double[] _daCenterPos, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, uint _uiCWDir);
        void CirclePointMove(int _iCoord, int[] _iaAxisNo, double[] _daMidPos, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, int _iArcCircle);
        void CircleRadiusMove(int _iCoord, int[] _iaAxisNo, double _dRadius, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, uint _uiCWDir, uint _uiShortDistance);
        void CircleAngleMove(int _iCoord, int[] _iaAxisNo, double[] _daCenterPos, double _dAngle, double _dVel, double _dAcc, double _dDec, uint _uiCWDir);


        //2축 제어 겐트리.
        //void SetGantryEnable(double _dPulsePerUnit);
        //void SetGantryDisable();


        /// <summary>
        /// 네트워크 타입들은 시그널을 받아 놓는다.
        /// </summary>
        void Update();

        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo);

        /// <summary>
        /// Para Return
        /// </summary>
        /// <returns></returns>
        object GetPara();
    }
}
