using SML;
using System.Runtime.CompilerServices;

namespace Machine
{
    public class ML
    {
        //D IO
        static public bool         IO_GetX    (xi   _eX, bool _bDirect = false)                        { return SM.DIO.GetX    ((int)_eX, _bDirect);        }
        static public bool         IO_GetX    (int  _eX, bool _bDirect = false)                        { return SM.DIO.GetX    ((int)_eX, _bDirect);        }
        static public bool         IO_GetX    (uint _eX, bool _bDirect = false)                        { return SM.DIO.GetX    ((int)_eX, _bDirect);        }
        //20180814 DryRunning 시 일일이 예외처리하기 힘들어서 만듬. _bDryRun에 드라이런닝 옵션 넣어서 쓴다.
        static public bool         IO_GetXDryRun(xi _eX, bool _bDryRun, bool _bDirect = false) { bool bRet = false; if (!_bDryRun) bRet = SM.DIO.GetX((int)_eX, _bDirect); else bRet = true; return bRet; }


        static public bool         IO_GetXDn  (xi   _eX)                                               { return SM.DIO.GetXDn  ((int)_eX);                  }
        static public bool         IO_GetXDn  (int  _eX)                                               { return SM.DIO.GetXDn  ((int)_eX);                  }
        static public bool         IO_GetXDn  (uint _eX)                                               { return SM.DIO.GetXDn  ((int)_eX);                  }
                                                                                                       
        static public bool         IO_GetXUp  (xi   _eX)                                               { return SM.DIO.GetXUp  ((int)_eX);                  }
        static public bool         IO_GetXUp  (int  _eX)                                               { return SM.DIO.GetXUp  ((int)_eX);                  }
        static public bool         IO_GetXUp  (uint _eX)                                               { return SM.DIO.GetXUp  ((int)_eX);                  }
                                                                                                       
        static public string       IO_GetXName(xi   _eX)                                               { return SM.DIO.GetXName((int)_eX);                  }
        static public string       IO_GetXName(int  _eX)                                               { return SM.DIO.GetXName((int)_eX);                  }
        static public string       IO_GetXName(uint _eX)                                               { return SM.DIO.GetXName((int)_eX);                  }
                                                                                                       
        static public void         IO_SetY    (yi   _eY, bool _bVal, bool _bDirect = false)            {        SM.DIO.SetY    ((int)_eY, _bVal, _bDirect); }
        static public void         IO_SetY    (int  _eY, bool _bVal, bool _bDirect = false)            {        SM.DIO.SetY    ((int)_eY, _bVal, _bDirect); }
        static public void         IO_SetY    (uint _eY, bool _bVal, bool _bDirect = false)            {        SM.DIO.SetY    ((int)_eY, _bVal, _bDirect); }
                                                                                                       
        static public bool         IO_GetY    (yi   _eY, bool _bDirect = false)                        { return SM.DIO.GetY    ((int)_eY, _bDirect);        }        
        static public bool         IO_GetY    (int  _eY, bool _bDirect = false)                        { return SM.DIO.GetY    ((int)_eY, _bDirect);        }        
        static public bool         IO_GetY    (uint _eY, bool _bDirect = false)                        { return SM.DIO.GetY    ((int)_eY, _bDirect);        }        
                                                    
        static public bool         IO_GetYDn  (yi   _eY)                                               { return SM.DIO.GetYDn  ((int)_eY);                  }
        static public bool         IO_GetYDn  (int  _eY)                                               { return SM.DIO.GetYDn  ((int)_eY);                  }
        static public bool         IO_GetYDn  (uint _eY)                                               { return SM.DIO.GetYDn  ((int)_eY);                  }
                                                                                                       
        static public bool         IO_GetYUp  (yi   _eY)                                               { return SM.DIO.GetYUp  ((int)_eY);                  }
        static public bool         IO_GetYUp  (int  _eY)                                               { return SM.DIO.GetYUp  ((int)_eY);                  }
        static public bool         IO_GetYUp  (uint _eY)                                               { return SM.DIO.GetYUp  ((int)_eY);                  }
                                                                                                       
        static public string       IO_GetYName(yi   _eY)                                               { return SM.DIO.GetYName((int)_eY);                  }
        static public string       IO_GetYName(int  _eY)                                               { return SM.DIO.GetYName((int)_eY);                  }
        static public string       IO_GetYName(uint _eY)                                               { return SM.DIO.GetYName((int)_eY);                  }

        //A IO
        static public double       AIO_GetX   (ax   _eX,  bool   _bDigit = false)                      { return SM.AIO.GetX    ((int)_eX,_bDigit);       }
        static public double       AIO_GetX   (int  _eX,  bool   _bDigit = false)                      { return SM.AIO.GetX    ((int)_eX,_bDigit);       }
        static public double       AIO_GetX   (uint _eX,  bool   _bDigit = false)                      { return SM.AIO.GetX    ((int)_eX,_bDigit);       }
                                                                                                       
        static public void         AIO_SetY   (ay    _eY,  double _dVal         )                      {        SM.AIO.SetY    ((int)_eY,_dVal  );       }
        static public void         AIO_SetY   (int   _eY,  double _dVal         )                      {        SM.AIO.SetY    ((int)_eY,_dVal  );       }
        static public void         AIO_SetY   (uint  _eY,  double _dVal         )                      {        SM.AIO.SetY    ((int)_eY,_dVal  );       }

        //Cylinder
        static public bool              CL_Complete  (ci   _eCylNo, fb _eCmd)                { return SM.CYL.Complete((int)_eCylNo, (EN_CYL_POS)_eCmd);      }
        static public bool              CL_Complete  (int  _eCylNo, fb _eCmd)                { return SM.CYL.Complete((int)_eCylNo, (EN_CYL_POS)_eCmd);      }
        static public bool              CL_Complete  (uint _eCylNo, fb _eCmd)                { return SM.CYL.Complete((int)_eCylNo, (EN_CYL_POS)_eCmd);      }

        static public bool              CL_Complete  (ci   _eCylNo)                          { return SM.CYL.Complete((int)_eCylNo);                         }
        static public bool              CL_Complete  (int  _eCylNo)                          { return SM.CYL.Complete((int)_eCylNo);                         }
        static public bool              CL_Complete  (uint _eCylNo)                          { return SM.CYL.Complete((int)_eCylNo);                         }
                                                           
        static public fb                CL_GetCmd    (ci   _eCylNo)                          { return SM.CYL.GetCmd  ((int)_eCylNo)==EN_CYL_POS.Fwd ? fb.Fwd : fb.Bwd ;}
        static public fb                CL_GetCmd    (int  _eCylNo)                          { return SM.CYL.GetCmd  ((int)_eCylNo)==EN_CYL_POS.Fwd ? fb.Fwd : fb.Bwd ;}
        static public fb                CL_GetCmd    (uint _eCylNo)                          { return SM.CYL.GetCmd  ((int)_eCylNo)==EN_CYL_POS.Fwd ? fb.Fwd : fb.Bwd ;}
                                                           
        static public fb                CL_GetAct    (ci   _eCylNo)                          { return SM.CYL.GetAct  ((int)_eCylNo)==EN_CYL_POS.Bwd ? fb.Fwd : fb.Bwd ;}
        static public fb                CL_GetAct    (int  _eCylNo)                          { return SM.CYL.GetAct  ((int)_eCylNo)==EN_CYL_POS.Bwd ? fb.Fwd : fb.Bwd ;}
        static public fb                CL_GetAct    (uint _eCylNo)                          { return SM.CYL.GetAct  ((int)_eCylNo)==EN_CYL_POS.Bwd ? fb.Fwd : fb.Bwd ;}
                                                           
        static public bool              CL_Err       (ci   _eCylNo)                          { return SM.CYL.Err     ((int)_eCylNo);                         }
        static public bool              CL_Err       (int  _eCylNo)                          { return SM.CYL.Err     ((int)_eCylNo);                         }
        static public bool              CL_Err       (uint _eCylNo)                          { return SM.CYL.Err     ((int)_eCylNo);                         }
                                                           
        static public bool              CL_Move      (ci   _eCylNo, fb _eCmd)                { return SM.CYL.Move    ((int)_eCylNo, (EN_CYL_POS)_eCmd); }
        static public bool              CL_Move      (int  _eCylNo, fb _eCmd)                { return SM.CYL.Move    ((int)_eCylNo, (EN_CYL_POS)_eCmd); }
        static public bool              CL_Move      (uint _eCylNo, fb _eCmd)                { return SM.CYL.Move    ((int)_eCylNo, (EN_CYL_POS)_eCmd); }
                                                           
        static public string            CL_GetName   (ci   _eCylNo)                          { return SM.CYL.GetName ((int)_eCylNo);                         }
        static public string            CL_GetName   (int  _eCylNo)                          { return SM.CYL.GetName ((int)_eCylNo);                         }
        static public string            CL_GetName   (uint _eCylNo)                          { return SM.CYL.GetName ((int)_eCylNo);                         }

        static public void              CL_Reset     (            )                          {      SM.CYL.Reset   (            );                         }

        static public void              CL_GoRpt     (int _iDly, int _iA1, int _iA2 = -1  ){        SM.CYL.GoRpt   (_iDly, (int)_iA1, (int)_iA2);          }
        static public void              CL_StopRpt   ()                                    {        SM.CYL.StopRpt ();                                     }
        static public EN_MOVE_DIRECTION CL_GetDirType(dynamic _eCylNo                     ){ return SM.CYL.GetDirType((int)_eCylNo);                       }

        //Error
        static public void         ER_SetErr      (ei  _eErr, string _sMsg = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = ""){SM.ERR.SetErr((int)_eErr, _sMsg, memberName, sourceLineNumber, sourceFilePath);}
        static public void         ER_SetErr      (int _eErr, string _sMsg = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = ""){SM.ERR.SetErr((int)_eErr, _sMsg, memberName, sourceLineNumber, sourceFilePath);}

        static public void         ER_Clear       (          ){SM.ERR.Clear ();}

        static public bool         ER_GetErrOn    (ei   _eErr ){return SM.ERR.GetErrOn    ((int)_eErr);}
        static public bool         ER_GetErrOn    (int  _eErr ){return SM.ERR.GetErrOn    ((int)_eErr);}
        static public bool         ER_GetErrOn    (uint _eErr ){return SM.ERR.GetErrOn    ((int)_eErr);}

        static public string       ER_GetErrSubMsg(ei   _eErr ){return SM.ERR.GetErrSubMsg((int)_eErr);}
        static public string       ER_GetErrSubMsg(int  _eErr ){return SM.ERR.GetErrSubMsg((int)_eErr);}
        static public string       ER_GetErrSubMsg(uint _eErr ){return SM.ERR.GetErrSubMsg((int)_eErr);}

        static public bool         ER_IsErr       (          ){return SM.ERR.IsErr       (          );}
        
        static public EN_ERR_LEVEL ER_GetErrLevel (ei   _eErr ){return SM.ERR.GetErrLevel((int)_eErr); }
        static public EN_ERR_LEVEL ER_GetErrLevel (int  _eErr ){return SM.ERR.GetErrLevel((int)_eErr); }
        static public EN_ERR_LEVEL ER_GetErrLevel (uint _eErr ){return SM.ERR.GetErrLevel((int)_eErr); }

        static public int          ER_MaxCount    (          ){return SM.ERR._iMaxErrCnt;             }
        static public void         ER_SetDisp     (bool _bOn ){       SM.ERR.SetNeedShowErr(_bOn);    }

        static public bool         ER_GetErr      (ei   _eErr ){return SM.ERR.GetErr((int)_eErr);      }
        static public bool         ER_GetErr      (int  _eErr ){return SM.ERR.GetErr((int)_eErr);      }
        static public bool         ER_GetErr      (uint _eErr ){return SM.ERR.GetErr((int)_eErr);      }

        //Tower Lamp
        static public void         TL_SetBuzzOff      (bool _bValue){SM.TWL.SetBuzzOff(_bValue);}
        
        //Motor
        static public bool         MT_GetStop         (mi   _eMotrNo)                                       {return SM.MTR.GetStop         ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStop         (int  _eMotrNo)                                       {return SM.MTR.GetStop         ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStop         (uint _eMotrNo)                                       {return SM.MTR.GetStop         ((int)_eMotrNo)                         ;}

        static public bool         MT_GetStopInpos    (mi   _eMotrNo)                                       {return SM.MTR.GetStopInpos    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStopInpos    (int  _eMotrNo)                                       {return SM.MTR.GetStopInpos    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetStopInpos    (uint _eMotrNo)                                       {return SM.MTR.GetStopInpos    ((int)_eMotrNo)                         ;}

        static public void         MT_SetServo        (mi   _eMotrNo, bool _bOn)                            {       SM.MTR.SetServo        ((int)_eMotrNo, _bOn)                   ;}
        static public void         MT_SetServo        (int  _eMotrNo, bool _bOn)                            {       SM.MTR.SetServo        ((int)_eMotrNo, _bOn)                   ;}
        static public void         MT_SetServo        (uint _eMotrNo, bool _bOn)                            {       SM.MTR.SetServo        ((int)_eMotrNo, _bOn)                   ;}

        static public bool         MT_GetServo        (mi   _eMotrNo)                                       {return SM.MTR.GetServo        ((int)_eMotrNo)                         ;}
        static public bool         MT_GetServo        (int  _eMotrNo)                                       {return SM.MTR.GetServo        ((int)_eMotrNo)                         ;}
        static public bool         MT_GetServo        (uint _eMotrNo)                                       {return SM.MTR.GetServo        ((int)_eMotrNo)                         ;}

        static public void         MT_SetServoAll     (               bool _bOn)                            {       SM.MTR.SetServoAll     (     _bOn    )                         ;}

        static public void         MT_SetReset        (mi   _eMotrNo, bool _bOn)                            {       SM.MTR.SetReset        ((int)_eMotrNo, _bOn)                   ;}
        static public void         MT_SetReset        (int  _eMotrNo, bool _bOn)                            {       SM.MTR.SetReset        ((int)_eMotrNo, _bOn)                   ;}
        static public void         MT_SetReset        (uint _eMotrNo, bool _bOn)                            {       SM.MTR.SetReset        ((int)_eMotrNo, _bOn)                   ;}

        static public void         MT_ResetAll        (                        )                            {       SM.MTR.ResetAll        (                   )                   ;}
                                                          
        static public double       MT_GetCmdPos       (mi   _eMotrNo)                                       {return SM.MTR.GetCmdPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetCmdPos       (int  _eMotrNo)                                       {return SM.MTR.GetCmdPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetCmdPos       (uint _eMotrNo)                                       {return SM.MTR.GetCmdPos       ((int)_eMotrNo)                         ;}

        static public double       MT_GetEncPos       (mi   _eMotrNo)                                       {return SM.MTR.GetEncPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetEncPos       (int  _eMotrNo)                                       {return SM.MTR.GetEncPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetEncPos       (uint _eMotrNo)                                       {return SM.MTR.GetEncPos       ((int)_eMotrNo)                         ;}

        static public double       MT_GetTrgPos       (mi   _eMotrNo)                                       {return SM.MTR.GetTrgPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetTrgPos       (int  _eMotrNo)                                       {return SM.MTR.GetTrgPos       ((int)_eMotrNo)                         ;}
        static public double       MT_GetTrgPos       (uint _eMotrNo)                                       {return SM.MTR.GetTrgPos       ((int)_eMotrNo)                         ;}

        static public void         MT_SetPos          (mi   _eMotrNo, double _dPos)                         {       SM.MTR.SetPos          ((int)_eMotrNo, _dPos)                  ;}
        static public void         MT_SetPos          (int  _eMotrNo, double _dPos)                         {       SM.MTR.SetPos          ((int)_eMotrNo, _dPos)                  ;}
        static public void         MT_SetPos          (uint _eMotrNo, double _dPos)                         {       SM.MTR.SetPos          ((int)_eMotrNo, _dPos)                  ;}

        static public bool         MT_CmprPos         (mi   _eMotrNo, double _dPos, double _dRange = 0.0)   {return SM.MTR.CmprPos         ((int)_eMotrNo, _dPos, _dRange = 0.0)   ;}
        static public bool         MT_CmprPos         (int  _eMotrNo, double _dPos, double _dRange = 0.0)   {return SM.MTR.CmprPos         ((int)_eMotrNo, _dPos, _dRange = 0.0)   ;}
        static public bool         MT_CmprPos         (uint _eMotrNo, double _dPos, double _dRange = 0.0)   {return SM.MTR.CmprPos         ((int)_eMotrNo, _dPos, _dRange = 0.0)   ;}

        static public bool         MT_GetHomeSnsr     (mi   _eMotrNo)                                       {return SM.MTR.GetHomeSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeSnsr     (int  _eMotrNo)                                       {return SM.MTR.GetHomeSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeSnsr     (uint _eMotrNo)                                       {return SM.MTR.GetHomeSnsr     ((int)_eMotrNo)                         ;}

        static public bool         MT_GetNLimSnsr     (mi   _eMotrNo)                                       {return SM.MTR.GetNLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetNLimSnsr     (int  _eMotrNo)                                       {return SM.MTR.GetNLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetNLimSnsr     (uint _eMotrNo)                                       {return SM.MTR.GetNLimSnsr     ((int)_eMotrNo)                         ;}

        static public bool         MT_GetPLimSnsr     (mi   _eMotrNo)                                       {return SM.MTR.GetPLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetPLimSnsr     (int  _eMotrNo)                                       {return SM.MTR.GetPLimSnsr     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetPLimSnsr     (uint _eMotrNo)                                       {return SM.MTR.GetPLimSnsr     ((int)_eMotrNo)                         ;}

        static public bool         MT_GetZphaseSgnl   (mi   _eMotrNo)                                       {return SM.MTR.GetZphaseSgnl   ((int)_eMotrNo)                         ;}
        static public bool         MT_GetZphaseSgnl   (int  _eMotrNo)                                       {return SM.MTR.GetZphaseSgnl   ((int)_eMotrNo)                         ;}
        static public bool         MT_GetZphaseSgnl   (uint _eMotrNo)                                       {return SM.MTR.GetZphaseSgnl   ((int)_eMotrNo)                         ;}

        static public bool         MT_GetInPosSgnl    (mi   _eMotrNo)                                       {return SM.MTR.GetInPosSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetInPosSgnl    (int  _eMotrNo)                                       {return SM.MTR.GetInPosSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetInPosSgnl    (uint _eMotrNo)                                       {return SM.MTR.GetInPosSgnl    ((int)_eMotrNo)                         ;}

        static public bool         MT_GetAlarmSgnl    (mi   _eMotrNo)                                       {return SM.MTR.GetAlarmSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetAlarmSgnl    (int  _eMotrNo)                                       {return SM.MTR.GetAlarmSgnl    ((int)_eMotrNo)                         ;}
        static public bool         MT_GetAlarmSgnl    (uint _eMotrNo)                                       {return SM.MTR.GetAlarmSgnl    ((int)_eMotrNo)                         ;}

        static public bool         MT_GoHome          (mi   _eMotrNo)                                       {return SM.MTR.GoHome          ((int)_eMotrNo)                         ;}
        static public bool         MT_GoHome          (int  _eMotrNo)                                       {return SM.MTR.GoHome          ((int)_eMotrNo)                         ;}
        static public bool         MT_GoHome          (uint _eMotrNo)                                       {return SM.MTR.GoHome          ((int)_eMotrNo)                         ;}

        static public bool         MT_GetHomeDone     (mi   _eMotrNo)                                       {return SM.MTR.GetHomeDone     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeDone     (int  _eMotrNo)                                       {return SM.MTR.GetHomeDone     ((int)_eMotrNo)                         ;}
        static public bool         MT_GetHomeDone     (uint _eMotrNo)                                       {return SM.MTR.GetHomeDone     ((int)_eMotrNo)                         ;}

        static public bool         MT_GetHomeDoneAll  (             )                                       {return SM.MTR.GetHomeDoneAll  (             )                         ;}

        static public void         MT_SetHomeDone     (mi   _eMotrNo, bool _bOn)                            {       SM.MTR.SetHomeDone     ((int)_eMotrNo,_bOn )                   ;}
        static public void         MT_SetHomeDone     (int  _eMotrNo, bool _bOn)                            {       SM.MTR.SetHomeDone     ((int)_eMotrNo,_bOn )                   ;}
        static public void         MT_SetHomeDone     (uint _eMotrNo, bool _bOn)                            {       SM.MTR.SetHomeDone     ((int)_eMotrNo,_bOn )                   ;}
                                                          
        static public string       MT_GetName         (mi   _eMotrNo)                                       {return SM.MTR.GetName         ((int)_eMotrNo)                         ;}
        static public string       MT_GetName         (int  _eMotrNo)                                       {return SM.MTR.GetName         ((int)_eMotrNo)                         ;}
        static public string       MT_GetName         (uint _eMotrNo)                                       {return SM.MTR.GetName         ((int)_eMotrNo)                         ;}

        static public void         MT_Stop            (mi   _eMotrNo)                                       {       SM.MTR.Stop            ((int)_eMotrNo)                         ;}
        static public void         MT_Stop            (int  _eMotrNo)                                       {       SM.MTR.Stop            ((int)_eMotrNo)                         ;}
        static public void         MT_Stop            (uint _eMotrNo)                                       {       SM.MTR.Stop            ((int)_eMotrNo)                         ;}

        static public void         MT_EmgStopAll      (            )                                       {       SM.MTR.EmgStopAll      (             )                         ;}

        static public void         MT_EmgStop         (mi   _eMotrNo)                                       {       SM.MTR.EmgStop         ((int)_eMotrNo)                         ;}
        static public void         MT_EmgStop         (int  _eMotrNo)                                       {       SM.MTR.EmgStop         ((int)_eMotrNo)                         ;}
        static public void         MT_EmgStop         (uint _eMotrNo)                                       {       SM.MTR.EmgStop         ((int)_eMotrNo)                         ;}

        static public void         MT_JogP            (mi   _eMotrNo)                                       {       SM.MTR.JogP            ((int)_eMotrNo)                         ;}
        static public void         MT_JogP            (int  _eMotrNo)                                       {       SM.MTR.JogP            ((int)_eMotrNo)                         ;}
        static public void         MT_JogP            (uint _eMotrNo)                                       {       SM.MTR.JogP            ((int)_eMotrNo)                         ;}

        static public void         MT_JogN            (mi   _eMotrNo)                                       {       SM.MTR.JogN            ((int)_eMotrNo)                         ;}
        static public void         MT_JogN            (int  _eMotrNo)                                       {       SM.MTR.JogN            ((int)_eMotrNo)                         ;}
        static public void         MT_JogN            (uint _eMotrNo)                                       {       SM.MTR.JogN            ((int)_eMotrNo)                         ;}

        static public void         MT_JogVel          (mi   _eMotrNo, double _dVel)                         {       SM.MTR.JogVel          ((int)_eMotrNo, _dVel)                  ;}
        static public void         MT_JogVel          (int  _eMotrNo, double _dVel)                         {       SM.MTR.JogVel          ((int)_eMotrNo, _dVel)                  ;}
        static public void         MT_JogVel          (uint _eMotrNo, double _dVel)                         {       SM.MTR.JogVel          ((int)_eMotrNo, _dVel)                  ;}

        static public void         MT_JogAbs          (mi _eMotrNo  , double _dVel,
                                                                      double _dAcc, double _dDec) { SM.MTR.JogAbs((int)_eMotrNo, _dVel, _dAcc, _dDec); }
        static public void         MT_JogAbs          (int _eMotrNo , double _dVel,
                                                                      double _dAcc, double _dDec) { SM.MTR.JogAbs((int)_eMotrNo, _dVel, _dAcc, _dDec); }
        static public void         MT_JogAbs          (uint _eMotrNo, double _dVel,
                                                                      double _dAcc, double _dDec) { SM.MTR.JogAbs((int)_eMotrNo, _dVel, _dAcc, _dDec); }

        static public void         MT_SetOverrideMaxVel(mi   _eMotrNo, double _dVel)                        {       SM.MTR.SetOverrideMaxVel((int)_eMotrNo, _dVel)                  ;}
        static public void         MT_SetOverrideMaxVel(int  _eMotrNo, double _dVel)                        {       SM.MTR.SetOverrideMaxVel((int)_eMotrNo, _dVel)                  ;}
        static public void         MT_SetOverrideMaxVel(uint _eMotrNo, double _dVel)                        {       SM.MTR.SetOverrideMaxVel((int)_eMotrNo, _dVel)                  ;}

        static public void         MT_OverrideVel     (mi   _eMotrNo, double _dVel)                         {       SM.MTR.OverrideVel     ((int)_eMotrNo, _dVel)                  ;}
        static public void         MT_OverrideVel     (int  _eMotrNo, double _dVel)                         {       SM.MTR.OverrideVel     ((int)_eMotrNo, _dVel)                  ;}
        static public void         MT_OverrideVel     (uint _eMotrNo, double _dVel)                         {       SM.MTR.OverrideVel     ((int)_eMotrNo, _dVel)                  ;}

        static public void         MT_GoAbs           (mi   _eMotrNo, double _dPos, double _dVel, 
                                                                      double _dAcc, double _dDec)           {       SM.MTR.GoAbs           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoAbs           (int  _eMotrNo, double _dPos, double _dVel, 
                                                                      double _dAcc, double _dDec)           {       SM.MTR.GoAbs           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoAbs           (uint _eMotrNo, double _dPos, double _dVel, 
                                                                      double _dAcc, double _dDec)           {       SM.MTR.GoAbs           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}

        static public void         MT_GoAbsVel        (mi   _eMotrNo, double _dPos, double _dVel)           {       SM.MTR.GoAbsVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoAbsVel        (int  _eMotrNo, double _dPos, double _dVel)           {       SM.MTR.GoAbsVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoAbsVel        (uint _eMotrNo, double _dPos, double _dVel)           {       SM.MTR.GoAbsVel        ((int)_eMotrNo, _dPos , _dVel )         ;}

        static public void         MT_GoAbsRun        (mi   _eMotrNo, double _dPos, int    _iPer = 0)       {       SM.MTR.GoAbsRun        ((int)_eMotrNo, _dPos , _iPer )         ;}
        static public void         MT_GoAbsRun        (int  _eMotrNo, double _dPos, int    _iPer = 0)       {       SM.MTR.GoAbsRun        ((int)_eMotrNo, _dPos , _iPer )         ;}
        static public void         MT_GoAbsRun        (uint _eMotrNo, double _dPos, int    _iPer = 0)       {       SM.MTR.GoAbsRun        ((int)_eMotrNo, _dPos , _iPer )         ;}

        static public void         MT_GoAbsMan        (mi   _eMotrNo, double _dPos)                         {       SM.MTR.GoAbsMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoAbsMan        (int  _eMotrNo, double _dPos)                         {       SM.MTR.GoAbsMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoAbsMan        (uint _eMotrNo, double _dPos)                         {       SM.MTR.GoAbsMan        ((int)_eMotrNo, _dPos         )         ;}

        static public void         MT_GoAbsSlow       (mi   _eMotrNo, double _dPos)                         {       SM.MTR.GoAbsSlow       ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoAbsSlow       (int  _eMotrNo, double _dPos)                         {       SM.MTR.GoAbsSlow       ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoAbsSlow       (uint _eMotrNo, double _dPos)                         {       SM.MTR.GoAbsSlow       ((int)_eMotrNo, _dPos         )         ;}
                                                          
        static public void         MT_GoInc           (mi   _eMotrNo, double _dPos, double _dVel, 
                                                                      double _dAcc, double _dDec)           {       SM.MTR.GoInc           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoInc           (int  _eMotrNo, double _dPos, double _dVel, 
                                                                      double _dAcc, double _dDec)           {       SM.MTR.GoInc           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}
        static public void         MT_GoInc           (uint _eMotrNo, double _dPos, double _dVel, 
                                                                      double _dAcc, double _dDec)           {       SM.MTR.GoInc           ((int)_eMotrNo, _dPos , _dVel , _dAcc , _dDec);}

        static public void         MT_GoIncVel        (mi   _eMotrNo, double _dPos, double _dVel)           {       SM.MTR.GoIncVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoIncVel        (int  _eMotrNo, double _dPos, double _dVel)           {       SM.MTR.GoIncVel        ((int)_eMotrNo, _dPos , _dVel )         ;}
        static public void         MT_GoIncVel        (uint _eMotrNo, double _dPos, double _dVel)           {       SM.MTR.GoIncVel        ((int)_eMotrNo, _dPos , _dVel )         ;}

        static public void         MT_GoIncRun        (mi   _eMotrNo, double _dPos)                         {       SM.MTR.GoIncRun        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncRun        (int  _eMotrNo, double _dPos)                         {       SM.MTR.GoIncRun        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncRun        (uint _eMotrNo, double _dPos)                         {       SM.MTR.GoIncRun        ((int)_eMotrNo, _dPos         )         ;}

        static public void         MT_GoIncMan        (mi   _eMotrNo, double _dPos)                         {       SM.MTR.GoIncMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncMan        (int  _eMotrNo, double _dPos)                         {       SM.MTR.GoIncMan        ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncMan        (uint _eMotrNo, double _dPos)                         {       SM.MTR.GoIncMan        ((int)_eMotrNo, _dPos         )         ;}

        static public void         MT_GoIncSlow       (mi   _eMotrNo, double _dPos)                         {       SM.MTR.GoIncSlow       ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncSlow       (int  _eMotrNo, double _dPos)                         {       SM.MTR.GoIncSlow       ((int)_eMotrNo, _dPos         )         ;}
        static public void         MT_GoIncSlow       (uint _eMotrNo, double _dPos)                         {       SM.MTR.GoIncSlow       ((int)_eMotrNo, _dPos         )         ;}
                                                          
        static public double       MT_GetMinPosition  (mi   _eMotrNo)                                       { return SM.MTR.GetMinPosition ((int)_eMotrNo); }
        static public double       MT_GetMinPosition  (int  _eMotrNo)                                       { return SM.MTR.GetMinPosition ((int)_eMotrNo); }
        static public double       MT_GetMinPosition  (uint _eMotrNo)                                       { return SM.MTR.GetMinPosition ((int)_eMotrNo); }

        static public double       MT_GetMaxPosition  (mi   _eMotrNo)                                       { return SM.MTR.GetMaxPosition ((int)_eMotrNo); }
        static public double       MT_GetMaxPosition  (int  _eMotrNo)                                       { return SM.MTR.GetMaxPosition ((int)_eMotrNo); }
        static public double       MT_GetMaxPosition  (uint _eMotrNo)                                       { return SM.MTR.GetMaxPosition ((int)_eMotrNo); }
                                                          
        //포지션 관련 랩핑.                               
        static public void         MT_GoAbsRun        (mi   _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsRun        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsRun        (int  _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsRun        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsRun        (uint _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsRun        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}

        static public void         MT_GoAbsMan        (mi   _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsMan        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsMan        (int  _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsMan        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsMan        (uint _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsMan        ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}

        static public void         MT_GoAbsSlow       (mi   _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsSlow       ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsSlow       (int  _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsSlow       ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_GoAbsSlow       (uint _eMotrNo, pv _ePos)                             {       SM.MTR.GoAbsSlow       ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}

        static public MOTION_DIR   MT_GetDirType      (mi   _eMotrNo          )                             {return SM.MTR.GetDirType      ((int)_eMotrNo)                         ;}
        static public MOTION_DIR   MT_GetDirType      (int  _eMotrNo          )                             {return SM.MTR.GetDirType      ((int)_eMotrNo)                         ;}
        static public MOTION_DIR   MT_GetDirType      (uint _eMotrNo          )                             {return SM.MTR.GetDirType      ((int)_eMotrNo)                         ;}

        static public bool         MT_GetStopPos      (mi   _eMotrNo, pv _ePos ,double _dOfs=0.0)           {if(!MT_GetStop(_eMotrNo))return false ; return MT_CmprPos(_eMotrNo,(PM_GetValue(_eMotrNo , _ePos )+_dOfs)); }
        static public bool         MT_GetStopPos      (int  _eMotrNo, pv _ePos ,double _dOfs=0.0)           {if(!MT_GetStop(_eMotrNo))return false ; return MT_CmprPos(_eMotrNo,(PM_GetValue(_eMotrNo , _ePos )+_dOfs)); }
        static public bool         MT_GetStopPos      (uint _eMotrNo, pv _ePos ,double _dOfs=0.0)           {if(!MT_GetStop(_eMotrNo))return false ; return MT_CmprPos(_eMotrNo,(PM_GetValue(_eMotrNo , _ePos )+_dOfs)); }

        static public void         MT_SetPos          (mi   _eMotrNo, pv _ePos)                             {       SM.MTR.SetPos          ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_SetPos          (int  _eMotrNo, pv _ePos)                             {       SM.MTR.SetPos          ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}
        static public void         MT_SetPos          (uint _eMotrNo, pv _ePos)                             {       SM.MTR.SetPos          ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ));}

        static public bool         MT_CmprPos         (mi   _eMotrNo, pv _ePos, double _dRange = 0.0)       {return SM.MTR.CmprPos         ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ), _dRange )   ;}
        static public bool         MT_CmprPos         (int  _eMotrNo, pv _ePos, double _dRange = 0.0)       {return SM.MTR.CmprPos         ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ), _dRange )   ;}
        static public bool         MT_CmprPos         (uint _eMotrNo, pv _ePos, double _dRange = 0.0)       {return SM.MTR.CmprPos         ((int)_eMotrNo, PM.GetValue((uint)_eMotrNo , (uint)_ePos ), _dRange )   ;}

        //보간관련함수.
        public void MT_ContiSetAxisMap   (mi   _eMotrNo , int _iCoord, uint _uiAxisCnt, int [] _iaAxisNo)   {       SM.MTR.ContiSetAxisMap   ((int)_eMotrNo , _iCoord,  _uiAxisCnt   , _iaAxisNo);}
        public void MT_ContiSetAxisMap   (int  _eMotrNo , int _iCoord, uint _uiAxisCnt, int [] _iaAxisNo)   {       SM.MTR.ContiSetAxisMap   ((int)_eMotrNo , _iCoord,  _uiAxisCnt   , _iaAxisNo);}
        public void MT_ContiSetAxisMap   (uint _eMotrNo , int _iCoord, uint _uiAxisCnt, int [] _iaAxisNo)   {       SM.MTR.ContiSetAxisMap   ((int)_eMotrNo , _iCoord,  _uiAxisCnt   , _iaAxisNo);}

        public void MT_ContiSetAbsRelMode(mi   _eMotrNo , int _iCoord, uint _uiAbsRelMode)                  {       SM.MTR.ContiSetAbsRelMode((int)_eMotrNo , _iCoord,  _uiAbsRelMode)           ;}
        public void MT_ContiSetAbsRelMode(int  _eMotrNo , int _iCoord, uint _uiAbsRelMode)                  {       SM.MTR.ContiSetAbsRelMode((int)_eMotrNo , _iCoord,  _uiAbsRelMode)           ;}
        public void MT_ContiSetAbsRelMode(uint _eMotrNo , int _iCoord, uint _uiAbsRelMode)                  {       SM.MTR.ContiSetAbsRelMode((int)_eMotrNo , _iCoord,  _uiAbsRelMode)           ;}

        public void MT_ContiBeginNode    (mi   _eMotrNo , int _iCoord)                                      {       SM.MTR.ContiBeginNode    ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiBeginNode    (int  _eMotrNo , int _iCoord)                                      {       SM.MTR.ContiBeginNode    ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiBeginNode    (uint _eMotrNo , int _iCoord)                                      {       SM.MTR.ContiBeginNode    ((int)_eMotrNo , _iCoord)                           ;}

        public void MT_ContiEndNode      (mi   _eMotrNo , int _iCoord)                                      {       SM.MTR.ContiEndNode      ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiEndNode      (int  _eMotrNo , int _iCoord)                                      {       SM.MTR.ContiEndNode      ((int)_eMotrNo , _iCoord)                           ;}
        public void MT_ContiEndNode      (uint _eMotrNo , int _iCoord)                                      {       SM.MTR.ContiEndNode      ((int)_eMotrNo , _iCoord)                           ;}

        public void MT_ContiStart        (mi   _eMotrNo , int _iCoord, uint _uiProfileset, int _iAngle)     {       SM.MTR.ContiStart        ((int)_eMotrNo , _iCoord,  _uiProfileset, _iAngle)  ;}
        public void MT_ContiStart        (int  _eMotrNo , int _iCoord, uint _uiProfileset, int _iAngle)     {       SM.MTR.ContiStart        ((int)_eMotrNo , _iCoord,  _uiProfileset, _iAngle)  ;}
        public void MT_ContiStart        (uint _eMotrNo , int _iCoord, uint _uiProfileset, int _iAngle)     {       SM.MTR.ContiStart        ((int)_eMotrNo , _iCoord,  _uiProfileset, _iAngle)  ;}

        public int  MT_GetContCrntIdx    (mi   _eMotrNo , int _iCoord)                                      {return SM.MTR.GetContCrntIdx    ((int)_eMotrNo , _iCoord)                           ;}
        public int  MT_GetContCrntIdx    (int  _eMotrNo , int _iCoord)                                      {return SM.MTR.GetContCrntIdx    ((int)_eMotrNo , _iCoord)                           ;}
        public int  MT_GetContCrntIdx    (uint _eMotrNo , int _iCoord)                                      {return SM.MTR.GetContCrntIdx    ((int)_eMotrNo , _iCoord)                           ;}
                                              
        public void MT_LineMove          (mi   _eMotrNo , int _iCoord, double []_daEndPos, double   _dVel        , double   _dAcc    , double _dDec)                                                                      {SM.MTR.LineMove          ((int) _eMotrNo , _iCoord, _daEndPos, _dVel        ,_dAcc    , _dDec                                              );} 
        public void MT_LineMove          (int  _eMotrNo , int _iCoord, double []_daEndPos, double   _dVel        , double   _dAcc    , double _dDec)                                                                      {SM.MTR.LineMove          ((int) _eMotrNo , _iCoord, _daEndPos, _dVel        ,_dAcc    , _dDec                                              );}
        public void MT_LineMove          (uint _eMotrNo , int _iCoord, double []_daEndPos, double   _dVel        , double   _dAcc    , double _dDec)                                                                      {SM.MTR.LineMove          ((int) _eMotrNo , _iCoord, _daEndPos, _dVel        ,_dAcc    , _dDec                                              );}

        public void MT_CircleCenterMove  (mi   _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MTR.CircleCenterMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir                     );}
        public void MT_CircleCenterMove  (int  _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MTR.CircleCenterMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir                     );}
        public void MT_CircleCenterMove  (uint _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MTR.CircleCenterMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir                     );}

        public void MT_CirclePointMove   (mi   _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daMidPos    , double []_daEndPos, double _dVel, double _dAcc , double _dDec, int _iArcCircle)                        {SM.MTR.CirclePointMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daMidPos    ,_daEndPos, _dVel, _dAcc , _dDec, _iArcCircle                  );}
        public void MT_CirclePointMove   (int  _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daMidPos    , double []_daEndPos, double _dVel, double _dAcc , double _dDec, int _iArcCircle)                        {SM.MTR.CirclePointMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daMidPos    ,_daEndPos, _dVel, _dAcc , _dDec, _iArcCircle                  );}
        public void MT_CirclePointMove   (uint _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daMidPos    , double []_daEndPos, double _dVel, double _dAcc , double _dDec, int _iArcCircle)                        {SM.MTR.CirclePointMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daMidPos    ,_daEndPos, _dVel, _dAcc , _dDec, _iArcCircle                  );}

        public void MT_CircleRadiusMove  (mi   _eMotrNo , int _iCoord, int    []_iaAxisNo, double   _dRadius     , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  , uint _uiShortDistance) {SM.MTR.CircleRadiusMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _dRadius     ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir   , _uiShortDistance);}
        public void MT_CircleRadiusMove  (int  _eMotrNo , int _iCoord, int    []_iaAxisNo, double   _dRadius     , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  , uint _uiShortDistance) {SM.MTR.CircleRadiusMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _dRadius     ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir   , _uiShortDistance);}
        public void MT_CircleRadiusMove  (uint _eMotrNo , int _iCoord, int    []_iaAxisNo, double   _dRadius     , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  , uint _uiShortDistance) {SM.MTR.CircleRadiusMove  ((int) _eMotrNo , _iCoord, _iaAxisNo, _dRadius     ,_daEndPos, _dVel, _dAcc , _dDec, _uiCWDir   , _uiShortDistance);}

        public void MT_CircleAngleMove   (mi   _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double   _dAngle  , double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MTR.CircleAngleMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_dAngle  , _dVel, _dAcc , _dDec, _uiCWDir                     );}
        public void MT_CircleAngleMove   (int  _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double   _dAngle  , double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MTR.CircleAngleMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_dAngle  , _dVel, _dAcc , _dDec, _uiCWDir                     );}
        public void MT_CircleAngleMove   (uint _eMotrNo , int _iCoord, int    []_iaAxisNo, double []_daCenterPos , double   _dAngle  , double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )                        {SM.MTR.CircleAngleMove   ((int) _eMotrNo , _iCoord, _iaAxisNo, _daCenterPos ,_dAngle  , _dVel, _dAcc , _dDec, _uiCWDir                     );}

        static public void MT_SetY              (mi   _eMotrNo , int _iNo , bool _bOn  )                                                                                                                                         {SM.MTR.SetY              ((int) _eMotrNo , _iNo , _bOn);  }
        static public void MT_SetY              (int  _eMotrNo , int _iNo , bool _bOn  )                                                                                                                                         {SM.MTR.SetY              ((int) _eMotrNo , _iNo , _bOn);  }
        static public void MT_SetY              (uint _eMotrNo , int _iNo , bool _bOn  )                                                                                                                                         {SM.MTR.SetY              ((int) _eMotrNo , _iNo , _bOn);  }
         
        static public bool MT_GetY              (mi   _eMotrNo , int _iNo              )                                                                                                                                         {return SM.MTR.GetY       ((int) _eMotrNo , _iNo );        }
        static public bool MT_GetY              (int  _eMotrNo , int _iNo              )                                                                                                                                         {return SM.MTR.GetY       ((int) _eMotrNo , _iNo );        }
        static public bool MT_GetY              (uint _eMotrNo , int _iNo              )                                                                                                                                         {return SM.MTR.GetY       ((int) _eMotrNo , _iNo );        }
         
        static public bool MT_GetX              (mi   _eMotrNo , int _iNo              )                                                                                                                                         {return SM.MTR.GetX       ((int) _eMotrNo , _iNo );        }
        static public bool MT_GetX              (int  _eMotrNo , int _iNo              )                                                                                                                                         {return SM.MTR.GetX       ((int) _eMotrNo , _iNo );        }
        static public bool MT_GetX              (uint _eMotrNo , int _iNo              )                                                                                                                                         {return SM.MTR.GetX       ((int) _eMotrNo , _iNo );        }


        static public double       PM_GetValue        (mi   _eMotrNo , pv _iPstnValue )                     {return PM.GetValue      ((uint)_eMotrNo    , (uint)_iPstnValue); }
        static public double       PM_GetValue        (int  _eMotrNo , pv _iPstnValue )                     {return PM.GetValue      ((uint)_eMotrNo    , (uint)_iPstnValue); }
        static public double       PM_GetValue        (uint _eMotrNo , pv _iPstnValue )                     {return PM.GetValue      ((uint)_eMotrNo    , (uint)_iPstnValue); }

        static public int          PM_GetValueSpdPer  (mi   _eMotrNo , pv _iPstnValue )                     {return PM.GetValueSpdPer((uint)_eMotrNo    , (uint)_iPstnValue); }
        static public int          PM_GetValueSpdPer  (int  _eMotrNo , pv _iPstnValue )                     {return PM.GetValueSpdPer((uint)_eMotrNo    , (uint)_iPstnValue); }
        static public int          PM_GetValueSpdPer  (uint _eMotrNo , pv _iPstnValue )                     {return PM.GetValueSpdPer((uint)_eMotrNo    , (uint)_iPstnValue); }
                                                           
        static public void         MT_ResetTrgPos     (mi   _eMotrNo                                                                 )           {       SM.MTR.ResetTrgPos((int)_eMotrNo);}
        static public void         MT_ResetTrgPos     (int  _eMotrNo                                                                 )           {       SM.MTR.ResetTrgPos((int)_eMotrNo);}
        static public void         MT_ResetTrgPos     (uint _eMotrNo                                                                 )           {       SM.MTR.ResetTrgPos((int)_eMotrNo);}

        static public void         MT_SetTrgPos       (mi   _eMotrNo, double[] _dPos, double _dTrgTime, bool _bActual, bool _bOnLevel)           {       SM.MTR.SetTrgPos((int)_eMotrNo, _dPos, _dTrgTime, _bActual, _bOnLevel);}
        static public void         MT_SetTrgPos       (int  _eMotrNo, double[] _dPos, double _dTrgTime, bool _bActual, bool _bOnLevel)           {       SM.MTR.SetTrgPos((int)_eMotrNo, _dPos, _dTrgTime, _bActual, _bOnLevel);}
        static public void         MT_SetTrgPos       (uint _eMotrNo, double[] _dPos, double _dTrgTime, bool _bActual, bool _bOnLevel)           {       SM.MTR.SetTrgPos((int)_eMotrNo, _dPos, _dTrgTime, _bActual, _bOnLevel);}

        static public void         MT_OneShotTrg      (mi   _eMotrNo, bool _bOnLevel, int _iTime                                     )           {       SM.MTR.OneShotTrg((int)_eMotrNo, _bOnLevel, _iTime);}
        static public void         MT_OneShotTrg      (int  _eMotrNo, bool _bOnLevel, int _iTime                                     )           {       SM.MTR.OneShotTrg((int)_eMotrNo, _bOnLevel, _iTime);}
        static public void         MT_OneShotTrg      (uint _eMotrNo, bool _bOnLevel, int _iTime                                     )           {       SM.MTR.OneShotTrg((int)_eMotrNo, _bOnLevel, _iTime);}
        
        static public void         MT_SetTrgBlock     (mi   _eMotrNo, double _dStt, double _dEnd, double _dPeriod, double _dTrgTime, bool _bActual, bool _bOnLevel) {  SM.MTR.SetTrgBlock((int)_eMotrNo, _dStt, _dEnd, _dPeriod, _dTrgTime, _bActual, _bOnLevel);}
        static public void         MT_SetTrgBlock     (int  _eMotrNo, double _dStt, double _dEnd, double _dPeriod, double _dTrgTime, bool _bActual, bool _bOnLevel) {  SM.MTR.SetTrgBlock((int)_eMotrNo, _dStt, _dEnd, _dPeriod, _dTrgTime, _bActual, _bOnLevel);}
        static public void         MT_SetTrgBlock     (uint _eMotrNo, double _dStt, double _dEnd, double _dPeriod, double _dTrgTime, bool _bActual, bool _bOnLevel) {  SM.MTR.SetTrgBlock((int)_eMotrNo, _dStt, _dEnd, _dPeriod, _dTrgTime, _bActual, _bOnLevel);}

        //20180723 동기구동 추가 진섭
        static public bool         MT_SetLinkEnable   (mi   _eMstMotrNo, mi _eSlvMotrNo)                                                         { return SM.MTR.SetLinkEnable ((int)_eMstMotrNo, (int)_eSlvMotrNo); }
        static public bool         MT_GetLinkMode     (mi   _eMstMotrNo                )                                                         { return SM.MTR.GetLinkMode   ((int)_eMstMotrNo                  ); }
        static public bool         MT_SetLinkDisable  (mi   _eMstMotrNo, mi _eSlvMotrNo)                                                         { return SM.MTR.SetLinkDisable((int)_eMstMotrNo, (int)_eSlvMotrNo); }
                                                                                                      
        static public void         MT_LinkJogP        (int  _iMstMotrNo, int _iSlvMotrNo                )                                          { SM.MTR.LinkJogP(_iMstMotrNo, _iSlvMotrNo); }
        static public void         MT_LinkJogN        (int  _iMstMotrNo, int _iSlvMotrNo                )                                          { SM.MTR.LinkJogN(_iMstMotrNo, _iSlvMotrNo); }
        static public void         MT_GoIncManLink    (mi   _eMstMotrNo, mi  _eSlvMotrNo, pv _iPstnValue)                                          { SM.MTR.GoIncManLink((int)_eMstMotrNo, (int)_eSlvMotrNo, (uint)_iPstnValue); }
        static public void         MT_GoIncManLink    (int  _iMstMotrNo, int _iSlvMotrNo, double _dPos  )                                          { SM.MTR.GoIncManLink(_iMstMotrNo, _iSlvMotrNo, _dPos); }

        static public int          ER_GetLastErr      (                                                                              )           {return SM.ERR.GetLastErr()    ;}
        static public string       ER_GetErrName      (int _iNo                                                                      )                                          {return SM.ERR.GetErrName(_iNo);}
         
        //20190326 포지션 지정 속도 오버라이딩 추가 진섭
        static public bool         MT_OverrideVelAtMultiPos(mi   _eMotrNo, double _dPos, double[] _daOverridePos, double[] _daOverrideVel) { return SM.MTR.OverrideVelAtMultiPos((int)_eMotrNo, _dPos, _daOverridePos, _daOverrideVel); }
        static public bool         MT_OverrideVelAtMultiPos(int  _eMotrNo, double _dPos, double[] _daOverridePos, double[] _daOverrideVel) { return SM.MTR.OverrideVelAtMultiPos((int)_eMotrNo, _dPos, _daOverridePos, _daOverrideVel); }
        static public bool         MT_OverrideVelAtMultiPos(uint _eMotrNo, double _dPos, double[] _daOverridePos, double[] _daOverrideVel) { return SM.MTR.OverrideVelAtMultiPos((int)_eMotrNo, _dPos, _daOverridePos, _daOverrideVel); }
    
        //20190326 포지션 지정 속도 오버라이딩 추가 진섭
        static public bool         MT_OverrideVelAtPos(mi   _eMotrNo, double _dPos, double _dOverridePos, double _dOverrideVel) { return SM.MTR.OverrideVelAtPos((int)_eMotrNo, _dPos, _dOverridePos, _dOverrideVel); }
        static public bool         MT_OverrideVelAtPos(int  _eMotrNo, double _dPos, double _dOverridePos, double _dOverrideVel) { return SM.MTR.OverrideVelAtPos((int)_eMotrNo, _dPos, _dOverridePos, _dOverrideVel); }
        static public bool         MT_OverrideVelAtPos(uint _eMotrNo, double _dPos, double _dOverridePos, double _dOverrideVel) { return SM.MTR.OverrideVelAtPos((int)_eMotrNo, _dPos, _dOverridePos, _dOverrideVel); }

        static public double       MT_GetRunVel       (mi   _eMotrNo ) {return SM.MTR.GetRunVel  ((int)_eMotrNo); }
        static public double       MT_GetSlowVel      (int  _eMotrNo ) {return SM.MTR.GetSlowVel ((int)_eMotrNo); }

    }
}
