using SML;
using System.Runtime.CompilerServices;

namespace Machine
{
    public class ML
    {
        //여기는 함수 인자 형변환 안하고 할 수 있게 래핑.
        static public bool         IO_GetX    (xi _eX, bool _bDirect = false)               { return SM.IO.GetX    ((int)_eX, _bDirect);        }
        static public bool         IO_GetXDn  (xi _eX)                                      { return SM.IO.GetXDn  ((int)_eX);                  }
        static public bool         IO_GetXUp  (xi _eX)                                      { return SM.IO.GetXUp  ((int)_eX);                  }
        static public string       IO_GetXName(xi _eX)                                      { return SM.IO.GetXName((int)_eX);                  }
        static public void         IO_SetY    (yi _eY, bool _bVal, bool _bDirect = false)   {        SM.IO.SetY    ((int)_eY, _bVal, _bDirect); }
        static public bool         IO_GetY    (yi _eY, bool _bDirect = false)               { return SM.IO.GetY    ((int)_eY, _bDirect);        }        
        static public bool         IO_GetYDn  (yi _eY)                                      { return SM.IO.GetYDn  ((int)_eY);                  }
        static public bool         IO_GetYUp  (yi _eY)                                      { return SM.IO.GetYUp  ((int)_eY);                  }
        static public string       IO_GetYName(yi _eY)                                      { return SM.IO.GetYName((int)_eY);                  }

        static public double       AIO_GetX   (ax _eX, bool   _bDigit = false)                { return SM.AIO.GetX    ((int)_eX,_bDigit);       }
        static public void         AIO_SetY   (ay _eY, double _dVal          )                {        SM.AIO.SetY    ((int)_eY,_dVal  );       }

        static public bool              CL_Complete  (ci _eCylNo, fb _eCmd)                { return SM.CL.Complete((int)_eCylNo, (EN_CYLINDER_POS)_eCmd); }
        static public bool              CL_Complete  (ci _eCylNo)                          { return SM.CL.Complete((int)_eCylNo);                         }
        static public fb                CL_GetCmd    (ci _eCylNo)                          { return SM.CL.GetCmd  ((int)_eCylNo)==EN_CYLINDER_POS.Fwd ? fb.Fwd : fb.Bwd ;}
        static public fb                CL_GetAct    (ci _eCylNo)                          { return SM.CL.GetAct  ((int)_eCylNo)==EN_CYLINDER_POS.Bwd ? fb.Fwd : fb.Bwd ;}
        static public bool              CL_Err       (ci _eCylNo)                          { return SM.CL.Err     ((int)_eCylNo);                         }
        static public bool              CL_Move      (ci _eCylNo, fb _eCmd)                { return SM.CL.Move    ((int)_eCylNo, (EN_CYLINDER_POS)_eCmd); }
        static public string            CL_GetName   (ci _eCylNo)                          { return SM.CL.GetName ((int)_eCylNo);                         }
        static public void              CL_GoRpt     (int _iDly, int _iA1, int _iA2 = -1  ){        SM.CL.GoRpt   (_iDly, (int)_iA1, (int)_iA2);          }
        static public void              CL_StopRpt   ()                                    {        SM.CL.StopRpt ();                                     }
        static public EN_MOVE_DIRECTION CL_GetDirType(ci _eCylNo                          ){ return SM.CL.GetDirType((int)_eCylNo);                       }

        static public void         ER_SetErr      (ei _eErr, string _sMsg = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0){SM.ER.SetErr((int)_eErr, _sMsg, memberName, sourceLineNumber);}
        static public void         ER_Clear       (                                                                                                                   ){SM.ER.Clear ();}
        static public bool         ER_GetErrOn    (ei _eErr                                                                                                           ){return SM.ER.GetErrOn    ((int)_eErr);}
        static public string       ER_GetErrSubMsg(ei _eErr                                                                                                           ){return SM.ER.GetErrSubMsg((int)_eErr);}
        static public bool         ER_IsErr       (                                                                                                                   ){return SM.ER.IsErr       (          );}
        static public EN_ERR_LEVEL ER_GetErrLevel (ei _eErr                                                                                                           ){return SM.ER.GetErrLevel((int)_eErr); }
        static public int          ER_MaxCount    (                                                                                                                   ){return SM.ER._iMaxErrCnt;             }
        static public void         ER_SetDisp     (bool _bOn                                                                                                          ){       SM.ER.SetNeedShowErr(_bOn);           }
        static public bool         ER_GetErr      (ei _eErr                                                                                                           ){return SM.ER.GetErr((int)_eErr);      }

        static public void         TL_SetBuzzOff      (bool _bValue){SM.TL.SetBuzzOff(_bValue);}
        
        static public bool         MT_GetStop         (mi _eMotrNo)                                       {return SM.MT.GetStop         ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStopInpos    (mi _eMotrNo)                                       {return SM.MT.GetStopInpos    ((int)_eMotrNo)                         ;}
        static public void         MT_SetServo        (mi _eMotrNo, bool _bOn)                            {       SM.MT.SetServo        ((int)_eMotrNo, _bOn)                   ;}
        static public bool         MT_GetServo        (mi _eMotrNo)                                       {return SM.MT.GetServo        ((int)_eMotrNo)                         ;}
        static public void         MT_SetServoAll     (             bool _bOn)                            {       SM.MT.SetServoAll     (     _bOn    )                         ;}
        static public void         MT_SetReset        (mi _eMotrNo, bool _bOn)                            {       SM.MT.SetReset        ((int)_eMotrNo, _bOn)                   ;}
        static public void         MT_ResetAll        (                      )                            {       SM.MT.ResetAll        (                   )                   ;}
                                   
        static public double       MT_GetCmdPos       (mi _eMotrNo)                                       {return SM.MT.GetCmdPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetEncPos       (mi _eMotrNo)                                       {return SM.MT.GetEncPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetTrgPos       (mi _eMotrNo)                                       {return SM.MT.GetTrgPos       ((int)_eMotrNo)                         ;}
        static public void         MT_SetPos          (mi _eMotrNo, double _dPos)                         {       SM.MT.SetPos          ((int)_eMotrNo, _dPos)                  ;}
        static public bool         MT_CmprPos         (mi _eMotrNo, double _dPos, double _dRange = 0.0)   {return SM.MT.CmprPos         ((int)_eMotrNo, _dPos, _dRange = 0.0)   ;}
        static public bool         MT_GetHomeSnsr     (mi _eMotrNo)                                       {return SM.MT.GetHomeSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetNLimSnsr     (mi _eMotrNo)                                       {return SM.MT.GetNLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetPLimSnsr     (mi _eMotrNo)                                       {return SM.MT.GetPLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetZphaseSgnl   (mi _eMotrNo)                                       {return SM.MT.GetZphaseSgnl   ((int)_eMotrNo)                         ;}
        static public bool         MT_GetInPosSgnl    (mi _eMotrNo)                                       {return SM.MT.GetInPosSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetAlarmSgnl    (mi _eMotrNo)                                       {return SM.MT.GetAlarmSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GoHome          (mi _eMotrNo)                                       {return SM.MT.GoHome          ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeDone     (mi _eMotrNo)                                       {return SM.MT.GetHomeDone     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeDoneAll  (           )                                       {return SM.MT.GetHomeDoneAll  (             )                         ;}
        static public void         MT_SetHomeDone     (mi _eMotrNo, bool _bOn)                            {       SM.MT.SetHomeDone     ((int)_eMotrNo,_bOn )                   ;}
                                   
                                   
        static public string       MT_GetName         (mi _eMotrNo)                                       {return SM.MT.GetName         ((int)_eMotrNo)                         ;}
        static public void         MT_Stop            (mi _eMotrNo)                                       {       SM.MT.Stop            ((int)_eMotrNo)                         ;}
        static public void         MT_EmgStopAll      (           )                                       {       SM.MT.EmgStopAll      (             )                         ;}
        static public void         MT_EmgStop         (mi _eMotrNo)                                       {       SM.MT.EmgStop         ((int)_eMotrNo)                         ;}
        static public void         MT_JogP            (mi _eMotrNo)                                       {       SM.MT.JogP            ((int)_eMotrNo)                         ;}
        static public void         MT_JogN            (mi _eMotrNo)                                       {       SM.MT.JogN            ((int)_eMotrNo)                         ;}
        static public void         MT_GoAbs           (mi _eMotrNo, double _dPos, double _dVel, 
                                                                    double _dAcc, double _dDec)           {       SM.MT.GoAbs           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoAbsVel        (mi _eMotrNo, double _dPos, double _dVel)           {       SM.MT.GoAbsVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoAbsRun        (mi _eMotrNo, double _dPos, int    _iPer = 0)       {       SM.MT.GoAbsRun        ((int)_eMotrNo, _dPos , _iPer )         ;}
        static public void         MT_GoAbsMan        (mi _eMotrNo, double _dPos)                         {       SM.MT.GoAbsMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoAbsSlow       (mi _eMotrNo, double _dPos)                         {       SM.MT.GoAbsSlow       ((int)_eMotrNo, _dPos         )         ;}

        static public void         MT_GoInc           (mi _eMotrNo, double _dPos, double _dVel, 
                                                                    double _dAcc, double _dDec)           {       SM.MT.GoInc           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoIncVel        (mi _eMotrNo, double _dPos, double _dVel)           {       SM.MT.GoIncVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoIncRun        (mi _eMotrNo, double _dPos)                         {       SM.MT.GoIncRun        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncMan        (mi _eMotrNo, double _dPos)                         {       SM.MT.GoIncMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncSlow       (mi _eMotrNo, double _dPos)                         {       SM.MT.GoIncSlow       ((int)_eMotrNo, _dPos         )         ;}

        static public double       MT_GetMinPosition  (mi _eMotrNo)                                       { return SM.MT.GetMinPosition ((int)_eMotrNo); }
        static public double       MT_GetMaxPosition  (mi _eMotrNo)                                       { return SM.MT.GetMaxPosition ((int)_eMotrNo); }




        //포지션 관련 랩핑.                 
        static public void         MT_GoAbsRun        (mi _eMotrNo, pv _ePos)                             {       SM.MT.GoAbsRun        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsMan        (mi _eMotrNo, pv _ePos)                             {       SM.MT.GoAbsMan        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsSlow       (mi _eMotrNo, pv _ePos)                             {       SM.MT.GoAbsSlow       ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public MOTION_DIR   MT_GetDirType      (mi _eMotrNo          )                             {return SM.MT.GetDirType      ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStopPos      (mi _eMotrNo, pv _ePos ,double _dOfs=0.0)           {if(!MT_GetStop(_eMotrNo))return false ; return MT_CmprPos(_eMotrNo,(PM_GetValue(_eMotrNo , _ePos )+_dOfs)); }
        static public void         MT_SetPos          (mi _eMotrNo, pv _ePos)                             {       SM.MT.SetPos          ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public bool         MT_CmprPos         (mi _eMotrNo, pv _ePos, double _dRange = 0.0)       {return SM.MT.CmprPos         ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ), _dRange )   ;}

        //보간관련함수.
        public void MT_ContiSetAxisMap   (mi _eMotrNo , int _iCoord, uint _uiAxisCnt, int [] _iaAxisNo)   {       SM.MT.ContiSetAxisMap   ((int)_eMotrNo , _iCoord,  _uiAxisCnt   , _iaAxisNo);}
        public void MT_ContiSetAbsRelMode(mi _eMotrNo , int _iCoord, uint _uiAbsRelMode)                  {       SM.MT.ContiSetAbsRelMode((int)_eMotrNo , _iCoord,  _uiAbsRelMode)           ;}
        public void MT_ContiBeginNode    (mi _eMotrNo , int _iCoord)                                      {       SM.MT.ContiBeginNode    ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiEndNode      (mi _eMotrNo , int _iCoord)                                      {       SM.MT.ContiEndNode      ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiStart        (mi _eMotrNo , int _iCoord, uint _uiProfileset, int _iAngle)     {       SM.MT.ContiStart        ((int)_eMotrNo , _iCoord,  _uiProfileset, _iAngle)  ;}
        public int  MT_GetContCrntIdx    (mi _eMotrNo , int _iCoord)                                      {return SM.MT.GetContCrntIdx    ((int)_eMotrNo , _iCoord)                           ;}

        public void MT_LineMove          (mi _eMotrNo , int _iCoord, double []_daEndPos, double   _dVel        , double   _dAcc    , double _dDec)                                                                      {SM.MT.LineMove          ((int) _eMotrNo , _iCoord, _daEndPos, _dVel        ,_dAcc    , _dDec                                              );} 
        public void MT_CircleCenterMove  (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MT.CircleCenterMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir                     );}
        public void MT_CirclePointMove   (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daMidPos    , double []_daEndPos, double _dVel, double _dAcc , double _dDec, int _iArcCircle)                        {SM.MT.CirclePointMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daMidPos    ,_daEndPos, _dVel, _dAcc , _dDec, _iArcCircle                  );}
        public void MT_CircleRadiusMove  (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double   _dRadius     , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  , uint _uiShortDistance) {SM.MT.CircleRadiusMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _dRadius     ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir   , _uiShortDistance);}
        public void MT_CircleAngleMove   (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double   _dAngle  , double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MT.CircleAngleMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_dAngle  , _dVel, _dAcc , _dDec, _uiCWDir                     );}

        //public void PW_  





        
        static public double       PM_GetValue        (mi _eMotrNo , pv _iPstnValue )                     {return PM.GetValue      ((uint)_eMotrNo    , (uint)_iPstnValue); }
        static public int          PM_GetValueSpdPer  (mi _eMotrNo , pv _iPstnValue )                     {return PM.GetValueSpdPer((uint)_eMotrNo    , (uint)_iPstnValue); }

        //static public void         PM_SetGetCmdPos    (mi _eMotrNo                                                                 )           {       PM.SetGetCmdPos((uint)_eMotrNo, SM.MT.GetCmdPos);}
        
        static public void         MT_ResetTrgPos     (mi _eMotrNo                                                                 )           {       SM.MT.ResetTrgPos((int)_eMotrNo);}
        static public void         MT_SetTrgPos       (mi _eMotrNo, double[] _dPos, double _dTrgTime, bool _bActual, bool _bOnLevel)           {       SM.MT.SetTrgPos((int)_eMotrNo, _dPos, _dTrgTime, _bActual, _bOnLevel);}
        static public void         MT_OneShotTrg      (mi _eMotrNo, bool _bOnLevel, int _iTime                                     )           {       SM.MT.OneShotTrg((int)_eMotrNo, _bOnLevel, _iTime);}
        
        static public int          ER_GetLastErr       (                                                                           )           {return SM.ER.GetLastErr()    ;}
        static public string       ER_GetErrName       (int _iNo                                                                   )                                          {return SM.ER.GetErrName(_iNo);}
        
        
    
    
    }
}
