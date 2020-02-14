using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SML2;
using System.Runtime.CompilerServices;

namespace Machine
{
    public class SM
    {
        //여기는 함수 인자 형변환 안하고 할 수 있게 래핑.
        static public bool   IO_GetX    (xi _eX, bool _bDirect = false)               { return SML.IO.GetX    ((int)_eX, _bDirect);        }
        static public bool   IO_GetXDn  (xi _eX)                                      { return SML.IO.GetXDn  ((int)_eX);                  }
        static public bool   IO_GetXUp  (xi _eX)                                      { return SML.IO.GetXUp  ((int)_eX);                  }
        static public string IO_GetXName(xi _eX)                                      { return SML.IO.GetXName((int)_eX);                  }
        static public void   IO_SetY    (yi _eY, bool _bVal, bool _bDirect = false)   {        SML.IO.SetY    ((int)_eY, _bVal, _bDirect); }
        static public bool   IO_GetY    (yi _eY, bool _bDirect = false)               { return SML.IO.GetY    ((int)_eY, _bDirect);        }        
        static public bool   IO_GetYDn  (yi _eY)                                      { return SML.IO.GetYDn  ((int)_eY);                  }
        static public bool   IO_GetYUp  (yi _eY)                                      { return SML.IO.GetYUp  ((int)_eY);                  }
        static public string IO_GetYName(yi _eY)                                      { return SML.IO.GetYName((int)_eY);                  }


        static public bool            CL_Complete(ci _eCylNo, fb _eCmd)               { return SML.CL.Complete((int)_eCylNo, (EN_CYLINDER_POS)_eCmd); }
        static public bool            CL_Complete(ci _eCylNo)                         { return SML.CL.Complete((int)_eCylNo);                         }
        static public fb              CL_GetCmd  (ci _eCylNo)                         { return SML.CL.GetCmd  ((int)_eCylNo)==EN_CYLINDER_POS.Fwd ? fb.Fwd : fb.Bwd ;}
        static public fb              CL_GetAct  (ci _eCylNo)                         { return SML.CL.GetAct  ((int)_eCylNo)==EN_CYLINDER_POS.Bwd ? fb.Fwd : fb.Bwd ;}
        static public bool            CL_Err     (ci _eCylNo)                         { return SML.CL.Err     ((int)_eCylNo);                         }
        static public bool            CL_Move    (ci _eCylNo, fb _eCmd)               { return SML.CL.Move    ((int)_eCylNo, (EN_CYLINDER_POS)_eCmd); }
        static public string          CL_GetName (ci _eCylNo)                         { return SML.CL.GetName ((int)_eCylNo);                         }
       
        static public void         ER_SetErr         (ei _eErr, string _sMsg = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0){SML.ER.SetErr((int)_eErr, _sMsg, memberName, sourceLineNumber);}
        static public void         ER_Clear          (                                                                                                                   ){SML.ER.Clear ();}
        static public bool         ER_GetErrOn       (ei _eErr                                                                                                           ){return SML.ER.GetErrOn    ((int)_eErr);}
        static public string       ER_GetErrSubMsg   (ei _eErr                                                                                                           ){return SML.ER.GetErrSubMsg((int)_eErr);}
        static public bool         ER_IsErr          (                                                                                                                   ){return SML.ER.IsErr       (          );}
        static public void         ER_SetNeedShowErr (bool _bOn)                                                                                                          {SML.ER.SetNeedShowErr(_bOn);}
                                   
        static public void         TL_SetBuzzOff      (bool _bValue){SML.TL.SetBuzzOff(_bValue);}
        
        static public bool         MT_GetStop         (mi _eMotrNo)                                       {return SML.MT.GetStop         ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStopInpos    (mi _eMotrNo)                                       {return SML.MT.GetStopInpos    ((int)_eMotrNo)                         ;}
        static public void         MT_SetServo        (mi _eMotrNo, bool _bOn)                            {       SML.MT.SetServo        ((int)_eMotrNo, _bOn)                   ;}
        static public bool         MT_GetServo        (mi _eMotrNo)                                       {return SML.MT.GetServo        ((int)_eMotrNo)                         ;}
        static public void         MT_SetServoAll     (             bool _bOn)                            {       SML.MT.SetServoAll     (     _bOn    )                         ;}
        static public void         MT_SetReset        (mi _eMotrNo, bool _bOn)                            {       SML.MT.SetReset        ((int)_eMotrNo, _bOn)                   ;}
        static public void         MT_ResetAll        (                      )                            {       SML.MT.ResetAll        (                   )                   ;}
                                   
        static public double       MT_GetCmdPos       (mi _eMotrNo)                                       {return SML.MT.GetCmdPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetEncPos       (mi _eMotrNo)                                       {return SML.MT.GetEncPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetTrgPos       (mi _eMotrNo)                                       {return SML.MT.GetTrgPos       ((int)_eMotrNo)                         ;}
        static public void         MT_SetPos          (mi _eMotrNo, double _dPos)                         {       SML.MT.SetPos          ((int)_eMotrNo, _dPos)                  ;}
        static public bool         MT_CmprPos         (mi _eMotrNo, double _dPos, double _dRange = 0.0)   {return SML.MT.CmprPos         ((int)_eMotrNo, _dPos, _dRange = 0.0)   ;}
        static public bool         MT_GetHomeSnsr     (mi _eMotrNo)                                       {return SML.MT.GetHomeSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetNLimSnsr     (mi _eMotrNo)                                       {return SML.MT.GetNLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetPLimSnsr     (mi _eMotrNo)                                       {return SML.MT.GetPLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetZphaseSgnl   (mi _eMotrNo)                                       {return SML.MT.GetZphaseSgnl   ((int)_eMotrNo)                         ;}
        static public bool         MT_GetInPosSgnl    (mi _eMotrNo)                                       {return SML.MT.GetInPosSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetAlarmSgnl    (mi _eMotrNo)                                       {return SML.MT.GetAlarmSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GoHome          (mi _eMotrNo)                                       {return SML.MT.GoHome          ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeDone     (mi _eMotrNo)                                       {return SML.MT.GetHomeDone     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeDoneAll  (           )                                       {return SML.MT.GetHomeDoneAll  (             )                         ;}
        static public void         MT_SetHomeDone     (mi _eMotrNo, bool _bOn)                            {       SML.MT.SetHomeDone     ((int)_eMotrNo,_bOn )                   ;}
                                   
                                   
        static public string       MT_GetName         (mi _eMotrNo)                                       {return SML.MT.GetName         ((int)_eMotrNo)                         ;}
        static public void         MT_Stop            (mi _eMotrNo)                                       {       SML.MT.Stop            ((int)_eMotrNo)                         ;}
        static public void         MT_EmgStopAll      (           )                                       {       SML.MT.EmgStopAll      (             )                         ;}
        static public void         MT_EmgStop         (mi _eMotrNo)                                       {       SML.MT.EmgStop         ((int)_eMotrNo)                         ;}
        static public void         MT_JogP            (mi _eMotrNo)                                       {       SML.MT.JogP            ((int)_eMotrNo)                         ;}
        static public void         MT_JogN            (mi _eMotrNo)                                       {       SML.MT.JogN            ((int)_eMotrNo)                         ;}
        static public void         MT_GoAbs           (mi _eMotrNo, double _dPos, double _dVel, 
                                                                    double _dAcc, double _dDec)           {       SML.MT.GoAbs           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoAbsVel        (mi _eMotrNo, double _dPos, double _dVel)           {       SML.MT.GoAbsVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoAbsRun        (mi _eMotrNo, double _dPos)                         {       SML.MT.GoAbsRun        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoAbsMan        (mi _eMotrNo, double _dPos)                         {       SML.MT.GoAbsMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoAbsSlow       (mi _eMotrNo, double _dPos)                         {       SML.MT.GoAbsSlow       ((int)_eMotrNo, _dPos         )         ;}

        static public void         MT_GoInc           (mi _eMotrNo, double _dPos, double _dVel, 
                                                                    double _dAcc, double _dDec)           {       SML.MT.GoInc           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoIncVel        (mi _eMotrNo, double _dPos, double _dVel)           {       SML.MT.GoIncVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoIncRun        (mi _eMotrNo, double _dPos)                         {       SML.MT.GoIncRun        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncMan        (mi _eMotrNo, double _dPos)                         {       SML.MT.GoIncMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncSlow       (mi _eMotrNo, double _dPos)                         {       SML.MT.GoIncSlow       ((int)_eMotrNo, _dPos         )         ;}








          
        //포지션 관련 랩핑.                 
        static public void         MT_GoAbsRun        (mi _eMotrNo, pv _ePos)                             {       SML.MT.GoAbsRun        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsMan        (mi _eMotrNo, pv _ePos)                             {       SML.MT.GoAbsMan        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsSlow       (mi _eMotrNo, pv _ePos)                             {       SML.MT.GoAbsSlow       ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public MOTION_DIR   MT_GetDirType      (mi _eMotrNo          )                             {return SML.MT.GetDirType      ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStopPos      (mi _eMotrNo, pv _ePos ,double _dOfs=0.0)           {if(!MT_GetStop(_eMotrNo))return false ; return MT_CmprPos(_eMotrNo,(PM_GetValue(_eMotrNo , _ePos )+_dOfs)); }
        static public void         MT_SetPos          (mi _eMotrNo, pv _ePos)                             {       SML.MT.SetPos          ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public bool         MT_CmprPos         (mi _eMotrNo, pv _ePos, double _dRange = 0.0)       {return SML.MT.CmprPos         ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ), _dRange )   ;}

        //보간관련함수.
        public void MT_ContiSetAxisMap   (mi _eMotrNo , int _iCoord, uint _uiAxisCnt, int [] _iaAxisNo)   {       SML.MT.ContiSetAxisMap   ((int)_eMotrNo , _iCoord,  _uiAxisCnt   , _iaAxisNo);}
        public void MT_ContiSetAbsRelMode(mi _eMotrNo , int _iCoord, uint _uiAbsRelMode)                  {       SML.MT.ContiSetAbsRelMode((int)_eMotrNo , _iCoord,  _uiAbsRelMode)           ;}
        public void MT_ContiBeginNode    (mi _eMotrNo , int _iCoord)                                      {       SML.MT.ContiBeginNode    ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiEndNode      (mi _eMotrNo , int _iCoord)                                      {       SML.MT.ContiEndNode      ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiStart        (mi _eMotrNo , int _iCoord, uint _uiProfileset, int _iAngle)     {       SML.MT.ContiStart        ((int)_eMotrNo , _iCoord,  _uiProfileset, _iAngle)  ;}
        public int  MT_GetContCrntIdx    (mi _eMotrNo , int _iCoord)                                      {return SML.MT.GetContCrntIdx    ((int)_eMotrNo , _iCoord)                           ;}

        public void MT_LineMove          (mi _eMotrNo , int _iCoord, double []_daEndPos, double   _dVel        , double   _dAcc    , double _dDec)                                                                      {SML.MT.LineMove          ((int) _eMotrNo , _iCoord, _daEndPos, _dVel        ,_dAcc    , _dDec                                              );} 
        public void MT_CircleCenterMove  (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SML.MT.CircleCenterMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir                     );}
        public void MT_CirclePointMove   (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daMidPos    , double []_daEndPos, double _dVel, double _dAcc , double _dDec, int _iArcCircle)                        {SML.MT.CirclePointMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daMidPos    ,_daEndPos, _dVel, _dAcc , _dDec, _iArcCircle                  );}
        public void MT_CircleRadiusMove  (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double   _dRadius     , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  , uint _uiShortDistance) {SML.MT.CircleRadiusMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _dRadius     ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir   , _uiShortDistance);}
        public void MT_CircleAngleMove   (mi _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double   _dAngle  , double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SML.MT.CircleAngleMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_dAngle  , _dVel, _dAcc , _dDec, _uiCWDir                     );}


















        
        static public double       PM_GetValue        (mi _eMotrNo , pv _iPstnValue )                     {return PM.GetValue((uint)_eMotrNo , (uint)_iPstnValue); }
    }
}
