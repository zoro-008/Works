

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
        /// OverrideVel을 하기전에 구동속도중에 가장 높은 놈을 세팅을 한다.
        /// </summary>
        /// <param name="_dMaxVel">가장높은 속도값</param>
        /// <returns></returns>
        bool SetOverrideMaxVel(double _dMaxVel);
        
        /// <summary>
        /// 구동중 속도를 오버라이딩한다.
        /// 정속구간에서만 먹는 함수 이고. SetOverrideMaxVel 구동 속도중 가장 빠른 속도를 세팅 하고 수행해야함.
        /// </summary>
        /// <param name="_dVel">오버라이딩 할 속도</param>
        /// <returns></returns>
        bool OverrideVel(double _dVel);

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
        /// 하드웨어 트리거를 시작 위치 부터 끝위치 까지 일정 간격으로 트리거를 발생 시킨다.
        /// </summary>
        /// <param name="_dStt">시작 위치</param>
        /// <param name="_dEnd">종료 위치</param>
        /// <param name="_dDist">주기</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커맨드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        void SetTrgBlock(double _dStt, double _dEnd, double _dPeriod, double _dTrgTime, bool _bActual, bool _bOnLevel);
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

        /// <summary>
        /// 해당 모터의 입력 비트를 확인
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <returns>true=ON, false=OFF</returns>
        bool GetX(int _iNo);

        /// <summary>
        /// 해당 모터의 출력 비트를 확인
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <returns>true=ON, false=OFF</returns>
        bool GetY(int _iNo);

        /// <summary>
        /// 해당 모터의 출력 비트를 제어
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <param name="_bOn">true=ON, false=OFF</param>
        void SetY(int _iNo, bool _bOn);

        /// <summary>
        /// Master/Slave모터간 동기구동
        /// 이 함수는 밖에서 자유롭게 Link Enable 할수 있다.
        /// </summary>
        bool SetLinkEnable(int _iSlvMotrNo);

        /// <summary>
        /// 현재 동기구동 상태이면 return true;
        /// </summary>
        /// <returns></returns>
        bool GetLinkMode();

        /// <summary>
        /// 동기구동 해제
        /// </summary>
        bool SetLinkDisable(int _iSlvMotrNo);

        /// <summary>
        /// 상대위치, 절대위치 변경
        /// </summary>
        /// <param name="_iAxisNo"> 축 넘버 </param>
        /// <param name="_uiAbsRelMode">0 : 절대모드, 1:상대모드</param>
        void SetAbsRelMode(uint _uiAbsRelMode);

        /// <summary>
        /// Master축이 +로 이동할때 Slave 축이 -로 이동하도록 하는 조그 기능
        /// 실제 동기구동은 아님
        /// </summary>
        /// <param name="iSlvMotrNo"> Slave 축 넘버 </param>
        /// <param name="_dVel"> 마스터축 속도 </param>
        /// <param name="_dAcc"> 마스터축 가속 </param>
        /// <param name="_dDec"> 마스터축 감속 </param>
        void LinkJogP(int iSlvMotrNo, double _dVel, double _dAcc, double _dDec);

        /// <summary>
        /// Master축이 -로 이동할때 Slave 축이 +로 이동하도록 하는 조그 기능
        /// 실제 동기구동은 아님
        /// </summary>
        /// <param name="iSlvMotrNo"> Slave 축 넘버 </param>
        /// <param name="_dVel"> 마스터축 속도 </param>
        /// <param name="_dAcc"> 마스터축 가속 </param>
        /// <param name="_dDec"> 마스터축 감속 </param>
        void LinkJogN(int iSlvMotrNo, double _dVel, double _dAcc, double _dDec);

        /// <summary>
        /// 설정 축의 모션 동작 중 사용자가 지정한 다수의 위치에서 구동 속도를 변경하는 함수
        /// </summary>
        /// <param name="_dPos"> 이동할 총 거리 </param>
        /// <param name="_dVel">구동 속도</param>
        /// <param name="_dAcc">가속도</param>
        /// <param name="_dDec">감속도</param>
        /// <param name="_iAraySize">위치 배열의 개수</param>
        /// <param name="_daOverridePos">속도를 변경할 위치 배열</param>
        /// <param name="_daOverrideVel">변경할 구동 속도 배열</param>
        /// <param name="_iTarget">속도를 변경할 위치 소스 선택(Cmd/Enc)</param>
        /// <param name="_uiOverrideMode">오버라이드 시작방법 지정</param>
        bool OverrideVelAtMultiPos(double _dPos, double _dVel, double _dAcc, double _dDec, int _iAraySize, double[] _daOverridePos, double[] _daOverrideVel, int _iTarget, uint _uiOverrideMode);

        bool OverrideVelAtPos(double _dPos, double _dVel, double _dAcc, double _dDec, double _dOverridePos, double _dOverrideVel, int _iTarget);
    
    }
}
