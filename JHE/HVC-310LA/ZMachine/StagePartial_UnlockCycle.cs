using COMMON;
using SMDll2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    //Stage가 너무 길어 언락사이클만 이리로 뺌.
    partial class Stage
    {
                //Unlock Cycle
        //Screw Type 전용
        public bool CycleUnlockVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;


            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:

                    return false;

                case 10:
                    if (OM.CmnOptn.bUseMultiHldr)
                    {
                        if ((DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipFrnt) || 
                            (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipRear))
                        {
                            Step.iCycle = 0;
                            return true;
                        }

                        if (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 1 || DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 1)
                        {
                            if (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 1)
                            {
                                Step.iCycle = 0;
                                return true;
                            }

                        }
                    }

                    if (FindChip(ref c, ref r, cs.Visn, ri.REAR) || FindChip(ref c, ref r, cs.Visn, ri.FRNT))
                    {
                        if ((DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipRear) ||
                        (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipFrnt) ||
                        (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Work)) ||
                        (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Work)))
                        {
                            Step.iCycle = 0;
                            return true;
                        }
                    }
                    

                    if (FindChip(ref c, ref r, cs.Unkwn, ri.REAR))//X는 오른쪽이홈 Y는 rear쪽이홈.)
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.REAR);//X는 오른쪽이홈 Y는 rear쪽이홈.
                        dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnRearStt) - c * OM.DevInfo.dRearColPitch;
                        dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnRearStt) - r * OM.DevInfo.dRearRowPitch;
                        iC = c;
                        iR = r;
                        iTray = ri.REAR;
                    }
                    else if (FindChip(ref c, ref r, cs.Unkwn, ri.FRNT))
                    {
                        dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnFrntStt) - c * OM.DevInfo.dFrntColPitch;
                        dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnFrntStt) - r * OM.DevInfo.dFrntRowPitch;
                        iC = c;
                        iR = r;
                        iTray = ri.FRNT;
                    }


                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    VC.SendVisnMsg(VC.sm.Ready);

                    Step.iCycle++;
                    return false;

                case 13: //일단은 여기서 레디 확인 하나 나중에 디버깅 끝나면 옮기자 렉타임 없게.
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Not Ready");
                        Step.iCycle = 0;
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 14:
                    MoveMotr(mi.PCK_X, dMoveX);
                    MoveMotr(mi.STG_Y, dMoveY);
                    //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    //나중엔 비젼 통신이 이리로 와야 함.
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;
                    //나중에 통신 확인 여기.
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    //Picker Left 비젼 결과값 초기화
                    if (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 0)
                    {
                        if (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 0)
                        {
                            RsltLensL.dX = 0.0;
                            RsltLensL.dY = 0.0;
                            RsltLensL.dT = 0.0;
                        }
                    }
                        

                    if (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 1)
                    {
                        if(DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 1)
                        {
                            RsltLensR.dX = 0.0;
                            RsltLensR.dY = 0.0;
                            RsltLensR.dT = 0.0;
                        }
                    }
                    
                    
                    VC.SendVisnMsg(VC.sm.Insp, "0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                        Step.iCycle = 0;
                        return true;
                    }

                    if (!OM.CmnOptn.bUseMultiHldr)
                    {
                        //Rear 첫번째 비전 이면.
                        if (!FindChip(ref c, ref r, cs.Visn, ri.PICK))//X는 오른쪽이홈 Y는 앞쪽이홈.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                                Step.iCycle = 0;
                                return true;
                            }

                            //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);
                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                            if (OM.CmnOptn.bUseMultiHldr)
                            {
                                if (DM.ARAY[(int)iTray].GetCntStat(cs.Visn) == 1)
                                {
                                    Step.iCycle = 10;
                                    return false;
                                }

                                //if (iC + OM.DevOptn.iPCKGapCnt < DM.ARAY[(int)ri.REAR].GetMaxCol())
                                //{
                                //if (DM.ARAY[(int)iTray].GetStat(iC + OM.DevOptn.iPCKGapCnt, iR) == cs.Unkwn)
                                //{
                                //    Step.iCycle = 10;
                                //    return false;
                                //}
                                //}
                            }

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensL.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensL.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensL.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensL.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                        }
                        else//Rear 두번째 비전이면.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                                Step.iCycle = 0;
                                return true;
                            }

                            //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);
                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensR.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensR.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensR.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensR.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            //DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                        }
                    }

                    if (OM.CmnOptn.bUseMultiHldr)
                    {
                        //Rear 첫번째 비전 이면.
                        //if (!FindChip(ref c, ref r, cs.Visn, iTray))//X는 오른쪽이홈 Y는 앞쪽이홈.
                        if (!FindChip(ref c, ref r, cs.Visn, iTray) && (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) < 1) && DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Empty)// && DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Empty)
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                                Step.iCycle = 0;
                                return true;
                            }

                            //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);
                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                            if (OM.CmnOptn.bUseMultiHldr)
                            {
                                if (GetPckStat(cs.Empty) == 2 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) < 2 &&
                                    DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) != 1)
                                {
                                    Step.iCycle = 10;
                                    return false;
                                }

                                //if (iC + OM.DevOptn.iPCKGapCnt < DM.ARAY[(int)ri.REAR].GetMaxCol())
                                //{
                                //if (DM.ARAY[(int)iTray].GetStat(iC + OM.DevOptn.iPCKGapCnt, iR) == cs.Unkwn)
                                //{
                                //    Step.iCycle = 10;
                                //    return false;
                                //}
                                //}
                            }

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensL.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensL.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensL.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensL.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                        }
                        else if (DM.ARAY[(int)ri.PICK].GetStat(1, 0) == cs.Empty)//Rear 두번째 비전이면.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                                Step.iCycle = 0;
                                return true;
                            }

                            //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);
                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensR.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensR.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensR.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensR.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            //DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                        }
                    }
                    Step.iCycle++;
                    return false;

                case 18:
                    Step.iCycle = 0;
                    return true;

            }
        }

        //Screw Type 전용
        public bool CycleUnlock()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            double dPckXPos = 0;
            double dPckYPos = 0;
            double dPckTPos = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10: //소팅정보 구하고 Z축 대기위치.
                    SortInfo.bPick = false;
                    SortInfo.eTool = ri.PICK;
                    if (FindChip(ref c, ref r, cs.Visn, ri.REAR)) SortInfo.eTray = ri.REAR;
                    else                                          SortInfo.eTray = ri.FRNT;
                    SortInfo.ePickChip = cs.Empty;
                    SortInfo.eTrayChip = cs.Visn;
                    if (!GetSortInfo(OM.CmnOptn.bUseMultiHldr, ref SortInfo))
                    {
                        Step.iCycle = 0;
                        return true;
                    }

                    if (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0)
                    {
                        DM.ARAY[(int)ri.PICK].ChangeStat(cs.Visn, cs.Align);
                    }


                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    //MoveMotr(mi.PCK_TL , pv.PCK_TLWait);
                    //MoveMotr(mi.PCK_TR , pv.PCK_TRWait);
                    //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;

                    Step.iCycle++;
                    return false;

                case 12://이동할곳 계산.
                    if (SortInfo.eTray == ri.REAR)
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X, pv.PCK_XVisnRearStt);
                        dPckYPos = GetMotrPos(mi.STG_Y, pv.STG_YVisnRearStt);
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dRearColPitch;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dRearColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dRearRowPitch;
                        //dPckXPos -= c * OM.DevInfo.dRearColPitch;
                        //dPckYPos -= r * OM.DevInfo.dRearRowPitch;
                    }
                    else
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X, pv.PCK_XVisnFrntStt);
                        dPckYPos = GetMotrPos(mi.STG_Y, pv.STG_YVisnFrntStt);
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dFrntColPitch;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dFrntColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dFrntRowPitch;
                        //dPckXPos -= c * OM.DevInfo.dFrntColPitch;
                        //dPckYPos -= r * OM.DevInfo.dFrntRowPitch;
                    }

                    //dPckXPos -= RsltLens.dX;
                    //dPckYPos -= RsltLens.dY;

                    FindChip(ref c, ref r, cs.Empty, ri.PICK);

                    if (c == 0)
                    {//픽커 1번 찝을때
                        dPckXPos -= RsltLensL.dX;
                        dPckYPos -= RsltLensL.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X , pv.PCK_XVisnPck1Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y , pv.STG_YVisnPck1Ofs);
                        dPckTPos  = GetMotrPos(mi.PCK_TL, pv.PCK_TLVisnZero  ) + RsltLensL.dT;

                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableLeftX(dPckTPos);
                            dPckYPos += LookUpTable.GetLookUpTableLeftY(dPckTPos);
                            dPckTPos += LookUpTable.GetLookUpTableLeftT(dPckTPos);
                        }
                        MoveMotr(mi.PCK_TL, dPckTPos);
                    }
                    else
                    {//픽커 2번 찝을때.
                        dPckXPos -= RsltLensR.dX;
                        dPckYPos -= RsltLensR.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X , pv.PCK_XVisnPck2Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y , pv.STG_YVisnPck2Ofs);
                        dPckTPos  = GetMotrPos(mi.PCK_TR, pv.PCK_TRVisnZero  ) + RsltLensR.dT;
                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableRightX(dPckTPos);
                            dPckYPos += LookUpTable.GetLookUpTableRightY(dPckTPos);
                            dPckTPos += LookUpTable.GetLookUpTableRightT(dPckTPos);
                        }
                        MoveMotr(mi.PCK_TR, dPckTPos);
                    }

                    MoveMotr(mi.PCK_X, dPckXPos);
                    MoveMotr(mi.STG_Y, dPckYPos);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    if (!GetStop(mi.PCK_TL, true)) return false;
                    if (!GetStop(mi.PCK_TR, true)) return false;

                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0) MoveMotr(mi.PCK_ZL, GetMotrPos(mi.PCK_ZL, pv.PCK_ZLUnlock));
                    else        MoveMotr(mi.PCK_ZR, GetMotrPos(mi.PCK_ZR, pv.PCK_ZRUnlock));

                    Step.iCycle++;
                    return false;

                case 14:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 16);
                        SetY(yi.PCK_VacLtOn, true);
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 16);
                        SetY(yi.PCK_VacRtOn, true);
                        SetY(yi.PCK_EjtRtOn, false);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    if(c==0){//픽커 1번 찝을때
                        if (GetX(xi.PCK_VacLt))
                        {
                            Step.iCycle++;
                            return false ;
                        }
                    }
                    else {//픽커 2번 찝을때.
                        if (GetX(xi.PCK_VacRt))
                        {
                            Step.iCycle++;
                            return false ;
                        }
                    }
                    
                    if(!m_tmDelay.OnDelay(1000))return false ;
                    
                    Step.iCycle = 50 ;
                    return false ;

                case 16:
                    //Device Option.
                    double dTWork = PM.GetValue(mi.PCK_TL, pv.PCK_TLUnlockWork); //T1440 Z0.4
                    double dHldrPitch = OM.DevOptn.dHldrPitch;
                    double dThetaWorkSpeed = OM.DevOptn.dThetaWorkSpeed; //초당 2바퀴.
                    double dThetaWorkAcc = OM.DevOptn.dThetaWorkAcc; //

                    //하드웨어 픽스된 변수들 하드웨어 변경시에 바꿔줘야함.
                    const double dTUnitPerRot = 360;//Degree
                    const double dZUnitPerRot = 2;//mm
                    //const double dZ_TRotRatio = dZUnitPerRot / dTUnitPerRot;
                    double dHldrPitch_ZRotRatio = dHldrPitch;


                    //double dZMovePos = dZ_TRotRatio * dTWork * dHldrPitch * dHldrPitch_ZRotRatio;
                    //double dZSpeed = dZ_TRotRatio * dThetaWorkSpeed * dHldrPitch_ZRotRatio;
                    //double dZAcc = dZ_TRotRatio * dThetaWorkAcc * dHldrPitch_ZRotRatio;



                    double dZMovePos = (dTWork / dTUnitPerRot) * dHldrPitch_ZRotRatio;

                    double dZSpeed = (dZMovePos / dTWork) * dThetaWorkSpeed;
                    double dZAcc = (dZMovePos / dTWork) * dThetaWorkAcc;


                    double dTemp;
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c==0)
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TL);
                        SM.MT.GoInc((int)mi.PCK_TL, -dTWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZL, -dZMovePos, dZSpeed, dZAcc, dZAcc);
                    }
                    else
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TR);
                        SM.MT.GoInc((int)mi.PCK_TR, -dTWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZR, -dZMovePos, dZSpeed, dZAcc, dZAcc);
                    }
                   
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c==0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        

                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!m_tmDelay.OnDelay(100))return false;
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c==0)
                    {
                        //if (!GetStop(mi.PCK_TL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 0);
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    else
                    {
                        //if (!GetStop(mi.PCK_TR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 0);
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "EncoderOffset", 4001);
                        //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    }
                    else
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "EncoderOffset", 4001);
                        //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    }
                    Step.iCycle++;
                    return false;

                case 20:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Visn);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Visn);
                    }

                    if (SortInfo.eTray == ri.REAR)
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.Work);
                    }
                    else
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.Work);
                    }

                    if(OM.CmnOptn.bIgnrLeftPck||OM.CmnOptn.bIgnrRightPck)
                    {
                    //if ((GetPckStat(cs.Visn) == 1 && DM.ARAY[(int)iTray].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)iTray].GetCntStat(cs.Visn) == 0) ||
                    //    (GetPckStat(cs.Visn) == 1 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) == 1))
                    //{
                        //if (DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) == 1 || OM.CmnOptn.bIgnrLeftPck || OM.CmnOptn.bIgnrRightPck)
                        //{
                        //FindChip(ref c, ref r, cs.Visn, ri.PICK);
                        DM.ARAY[(int)ri.PICK].ChangeStat(cs.Visn, cs.Align);
                        
                    }
                    
                    Step.iCycle = 0;
                    return true;

                //Up Write
                case 50:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {//픽커 1번 찝을때
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                    }
                    else
                    {//픽커 2번 찝을때.
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 51:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 52:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    SetY(yi.PCK_EjtLtOn, false);
                    SetY(yi.PCK_EjtRtOn, false);
                    FindChip(ref c, ref r, cs.Visn, ri.LENS);
                    if (c == 0)
                    {//픽커 1번 찝을때
                        SetErr(ei.PRT_VacErr, "Left Picker Pickup Vaccum Error");
                        DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                        //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);

                    }
                    else
                    {//픽커 2번 찝을때.
                        SetErr(ei.PRT_VacErr, "Right Picker Pickup Vaccum Error");
                        DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                        //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    }
                    Step.iCycle = 0;
                    return true;

            }
        }

        //Locking Type 전용
        public bool CycleUnlockVisn2()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;


            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:

                    return false;

                case 10:
                    if (OM.CmnOptn.bUseMultiHldr)
                    {
                        if ((DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipFrnt) ||
                            (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipRear))
                        {
                            Step.iCycle = 0;
                            return true;
                        }

                        if (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 1 || DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 1)
                        {
                            if (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 1)
                            {
                                Step.iCycle = 0;
                                return true;
                            }

                        }
                    }

                    if (FindChip(ref c, ref r, cs.Visn, ri.REAR) || FindChip(ref c, ref r, cs.Visn, ri.FRNT))
                    {
                        if ((DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipRear) ||
                            (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipFrnt) ||
                            (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Work)) ||
                            (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Work)))
                        {
                            Step.iCycle = 0;
                            return true;
                        }
                    }


                    if (FindChip(ref c, ref r, cs.Unkwn, ri.REAR))//X는 오른쪽이홈 Y는 rear쪽이홈.)
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.REAR);//X는 오른쪽이홈 Y는 rear쪽이홈.
                        dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnRearStt) - c * OM.DevInfo.dRearColPitch;
                        dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnRearStt) - r * OM.DevInfo.dRearRowPitch;
                        iC = c;
                        iR = r;
                        iTray = ri.REAR;
                    }
                    else if (FindChip(ref c, ref r, cs.Unkwn, ri.FRNT))
                    {
                        dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnFrntStt) - c * OM.DevInfo.dFrntColPitch;
                        dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnFrntStt) - r * OM.DevInfo.dFrntRowPitch;
                        iC = c;
                        iR = r;
                        iTray = ri.FRNT;
                    }


                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;

                    Step.iCycle++;
                    return false;

                case 12:
                    MoveMotr(mi.PCK_X, dMoveX);
                    MoveMotr(mi.STG_Y, dMoveY);
                    VC.SendVisnMsg(VC.sm.Ready);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;
                    //나중에 통신 확인 여기.

                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Not Ready");
                        Step.iCycle = 0;
                        return true;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;

                    //Picker Left 비젼 결과값 초기화
                    if (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 0)
                    {
                        if (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 0)
                        {
                            RsltLensL.dX = 0.0;
                            RsltLensL.dY = 0.0;
                            RsltLensL.dT = 0.0;
                        }
                    }


                    if (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 1)
                    {
                        if (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 1)
                        {
                            RsltLensR.dX = 0.0;
                            RsltLensR.dY = 0.0;
                            RsltLensR.dT = 0.0;
                        }
                    }
                    
                    Step.iCycle++;
                    return false;

                case 15:
                    if (OM.CmnOptn.bUseMultiHldr) //비젼 2방 찍고 둘다 픽
                    {
                        Step.iCycle = 30;
                        return false;
                    }

                    //비젼 찍고 픽
                    Step.iCycle = 20;
                    return false;

                //OM.CmnOptn.bUseMultiHldr 사용 안할때
                //렌즈 검사
                case 20:
                    VC.SendVisnMsg(VC.sm.Insp, "0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)

                    Step.iCycle++;
                    return false;

                case 21:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                        Step.iCycle = 0;
                        return true;
                    }

                    Step.iCycle++;
                    return false;
                
                case 22:
                    //Rear 첫번째 비전 이면.
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)//X는 오른쪽이홈 Y는 앞쪽이홈.
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);
                        //DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                        if (OM.DevOptn.dLensVisnXTol != 0)
                        {
                            if (RsltLensL.dX < -OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltLensL.dX > OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dLensVisnYTol != 0)
                        {
                            if (RsltLensL.dY < -OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltLensL.dY > OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                    }
                    else//Rear 두번째 비전이면.
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);
                        //DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                        if (OM.DevOptn.dLensVisnXTol != 0)
                        {
                            if (RsltLensR.dX < -OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltLensR.dX > OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dLensVisnYTol != 0)
                        {
                            if (RsltLensR.dY < -OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltLensR.dY > OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }
                    }

                    Step.iCycle++;
                    return false;

                //홀더 검사
                case 23:
                    VC.SendVisnMsg(VC.sm.Insp, "1");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)

                    Step.iCycle++;
                    return false;

                case 24:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                        Step.iCycle = 0;
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 25:
                    //Rear 첫번째 비전 이면.
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    //if (!FindChip(ref c, ref r, cs.Visn, ri.PICK))//X는 오른쪽이홈 Y는 앞쪽이홈.
                    if(c == 0)
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrL.dX, ref RsltHldrL.dY, ref RsltHldrL.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);

                        if (OM.DevOptn.dHldrVisnXTol != 0)
                        {
                            if (RsltHldrL.dX < -OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltHldrL.dX > OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dHldrVisnYTol != 0)
                        {
                            if (RsltHldrL.dY < -OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltHldrL.dY > OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                    }
                    else//Rear 두번째 비전이면.
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrR.dX, ref RsltHldrR.dY, ref RsltHldrR.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        //FindChip(ref c, ref r, cs.Unkwn, ri.REAR);

                        if (OM.DevOptn.dHldrVisnXTol != 0)
                        {
                            if (RsltHldrR.dX < -OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltHldrR.dX > OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dHldrVisnYTol != 0)
                        {
                            if (RsltHldrR.dY < -OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltHldrR.dY > OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                    }

                    Step.iCycle = 50;
                    return false;

                //OM.CmnOptn.bUseMultiHldr 사용할때
                case 30:
                    VC.SendVisnMsg(VC.sm.Insp, "0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)

                    Step.iCycle++;
                    return false;

                case 31:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                        Step.iCycle = 0;
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 32:
                    //Rear 첫번째 비전 이면.
                    //if (!FindChip(ref c, ref r, cs.Visn, iTray))//X는 오른쪽이홈 Y는 앞쪽이홈.
                    if (!FindChip(ref c, ref r, cs.Visn, iTray) && (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) < 1) && DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Empty)// && DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Empty)
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        

                        if (OM.DevOptn.dLensVisnXTol != 0)
                        {
                            if (RsltLensL.dX < -OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltLensL.dX > OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dLensVisnYTol != 0)
                        {
                            if (RsltLensL.dY < -OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltLensL.dY > OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                    }
                    else if (DM.ARAY[(int)ri.PICK].GetStat(1, 0) == cs.Empty)//Rear 두번째 비전이면.
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        if (OM.DevOptn.dLensVisnXTol != 0)
                        {
                            if (RsltLensR.dX < -OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltLensR.dX > OM.DevOptn.dLensVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dLensVisnYTol != 0)
                        {
                            if (RsltLensR.dY < -OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltLensR.dY > OM.DevOptn.dLensVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }
                    }
                    
                    Step.iCycle++;
                    return false;

                //홀더 검사
                case 33:
                    VC.SendVisnMsg(VC.sm.Insp, "1");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)

                    Step.iCycle++;
                    return false;

                case 34:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                        Step.iCycle = 0;
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 35:
                    //Rear 첫번째 비전 이면.
                    if (!FindChip(ref c, ref r, cs.Visn, iTray) && (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) < 1) && DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Empty)// && DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Empty)
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrL.dX, ref RsltHldrL.dY, ref RsltHldrL.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        if (OM.DevOptn.dHldrVisnXTol != 0)
                        {
                            if (RsltHldrL.dX < -OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltHldrL.dX > OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dHldrVisnYTol != 0)
                        {
                            if (RsltHldrL.dY < -OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltHldrL.dY > OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);

                        if (GetPckStat(cs.Empty) == 2 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) < 2 &&
                            DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) != 1)
                        {
                            Step.iCycle = 10;
                            return false;
                        }
                    }
                    else if (DM.ARAY[(int)ri.PICK].GetStat(1, 0) == cs.Empty)//Rear 두번째 비전이면.
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrR.dX, ref RsltHldrR.dY, ref RsltHldrR.dT))
                        {
                            DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        if (OM.DevOptn.dHldrVisnXTol != 0)
                        {
                            if (RsltHldrR.dX < -OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                            if (RsltHldrR.dX > OM.DevOptn.dHldrVisnXTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.DevOptn.dHldrVisnYTol != 0)
                        {
                            if (RsltHldrR.dY < -OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }

                            if (RsltHldrR.dY > OM.DevOptn.dHldrVisnYTol)
                            {
                                DM.ARAY[(int)iTray].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                    }
                    //비젼 결과 다 얻었으면 50번
                    Step.iCycle = 50;
                    return false;

                case 50:

                    Step.iCycle = 0;
                    return true;

            }
        }

        //Locking Type 전용
        public bool CycleUnlock2()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            double dPckXPos = 0;
            double dPckYPos = 0;
            double dPckTPos = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10: //소팅정보 구하고 Z축 대기위치.
                    SortInfo.bPick = false;
                    SortInfo.eTool = ri.PICK;
                    if (FindChip(ref c, ref r, cs.Visn, ri.REAR)) SortInfo.eTray = ri.REAR;
                    else SortInfo.eTray = ri.FRNT;
                    SortInfo.ePickChip = cs.Empty;
                    SortInfo.eTrayChip = cs.Visn;
                    if (!GetSortInfo(OM.CmnOptn.bUseMultiHldr, ref SortInfo))
                    {
                        Step.iCycle = 0;
                        return true;
                    }

                    if (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0)
                    {
                        DM.ARAY[(int)ri.PICK].ChangeStat(cs.Visn, cs.Align);
                    }


                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    //MoveMotr(mi.PCK_TL , pv.PCK_TLWait);
                    //MoveMotr(mi.PCK_TR , pv.PCK_TRWait);
                    //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;

                    Step.iCycle++;
                    return false;

                case 12://이동할곳 계산.
                    if (SortInfo.eTray == ri.REAR)
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X, pv.PCK_XVisnRearStt);
                        dPckYPos = GetMotrPos(mi.STG_Y, pv.STG_YVisnRearStt);
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dRearColPitch;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dRearColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dRearRowPitch;
                        //dPckXPos -= c * OM.DevInfo.dRearColPitch;
                        //dPckYPos -= r * OM.DevInfo.dRearRowPitch;
                    }
                    else
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X, pv.PCK_XVisnFrntStt);
                        dPckYPos = GetMotrPos(mi.STG_Y, pv.STG_YVisnFrntStt);
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dFrntColPitch;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dFrntColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dFrntRowPitch;
                        //dPckXPos -= c * OM.DevInfo.dFrntColPitch;
                        //dPckYPos -= r * OM.DevInfo.dFrntRowPitch;
                    }

                    //dPckXPos -= RsltLens.dX;
                    //dPckYPos -= RsltLens.dY;

                    FindChip(ref c, ref r, cs.Empty, ri.PICK);

                    if (c == 0)
                    {//픽커 1번 찝을때
                        dPckXPos -= RsltLensL.dX;
                        dPckYPos -= RsltLensL.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs);
                        dPckTPos = GetMotrPos(mi.PCK_TL, pv.PCK_TLVisnZero) + RsltLensL.dT;

                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableLeftX(RsltLensL.dT);
                            dPckYPos += LookUpTable.GetLookUpTableLeftY(RsltLensL.dT);
                            dPckTPos += LookUpTable.GetLookUpTableLeftT(RsltLensL.dT);
                        }
                        MoveMotr(mi.PCK_TL, dPckTPos);
                    }
                    else if(c == 1)
                    {//픽커 2번 찝을때.
                        dPckXPos -= RsltLensR.dX;
                        dPckYPos -= RsltLensR.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs);
                        dPckTPos = GetMotrPos(mi.PCK_TR, pv.PCK_TRVisnZero) + RsltLensR.dT;
                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableRightX(RsltLensR.dT);
                            dPckYPos += LookUpTable.GetLookUpTableRightY(RsltLensR.dT);
                            dPckTPos += LookUpTable.GetLookUpTableRightT(RsltLensR.dT);
                        }
                        MoveMotr(mi.PCK_TR, dPckTPos);
                    }

                    MoveMotr(mi.PCK_X, dPckXPos);
                    MoveMotr(mi.STG_Y, dPckYPos);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    if (!GetStop(mi.PCK_TL, true)) return false;
                    if (!GetStop(mi.PCK_TR, true)) return false;

                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0) MoveMotr(mi.PCK_ZL, GetMotrPos(mi.PCK_ZL, pv.PCK_ZLUnlock));
                    else MoveMotr(mi.PCK_ZR, GetMotrPos(mi.PCK_ZR, pv.PCK_ZRUnlock));

                    Step.iCycle++;
                    return false;

                case 14:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 16);
                        SetY(yi.PCK_VacLtOn, true);
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 16);
                        SetY(yi.PCK_VacRtOn, true);
                        SetY(yi.PCK_EjtRtOn, false);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {//픽커 1번 찝을때
                        if (GetX(xi.PCK_VacLt))
                        {
                            Step.iCycle++;
                            return false;
                        }
                    }
                    else
                    {//픽커 2번 찝을때.
                        if (GetX(xi.PCK_VacRt))
                        {
                            Step.iCycle++;
                            return false;
                        }
                    }

                    if (!m_tmDelay.OnDelay(1000)) return false;

                    Step.iCycle = 50;
                    return false;

                case 16:
                    //Device Option.
                    double dTLWork = (RsltLensL.dT - RsltHldrL.dT) - PM.GetValue(mi.PCK_TL, pv.PCK_TLHolderPutOfs); //T1440 Z0.4
                    double dTRWork = (RsltLensR.dT - RsltHldrR.dT) - PM.GetValue(mi.PCK_TR, pv.PCK_TRHolderPutOfs); //T1440 Z0.4
                    Log.Trace("dTLUnlockWork", dTLWork.ToString());
                    Log.Trace("dTRUnlockWork", dTRWork.ToString());
                    double dHldrPitch = OM.DevOptn.dHldrPitch;
                    double dThetaWorkSpeed = OM.DevOptn.dThetaWorkSpeed; //초당 2바퀴.
                    double dThetaWorkAcc = OM.DevOptn.dThetaWorkAcc; //

                    //하드웨어 픽스된 변수들 하드웨어 변경시에 바꿔줘야함.
                    const double dTUnitPerRot = 360;//Degree
                    const double dZUnitPerRot = 2;//mm
                    //const double dZ_TRotRatio = dZUnitPerRot / dTUnitPerRot;
                    double dHldrPitch_ZRotRatio = dHldrPitch;


                    //double dZMovePos = dZ_TRotRatio * dTWork * dHldrPitch * dHldrPitch_ZRotRatio;
                    //double dZSpeed = dZ_TRotRatio * dThetaWorkSpeed * dHldrPitch_ZRotRatio;
                    //double dZAcc = dZ_TRotRatio * dThetaWorkAcc * dHldrPitch_ZRotRatio;



                    double dZLMovePos = (dTLWork / dTUnitPerRot) * dHldrPitch_ZRotRatio;
                    double dZRMovePos = (dTRWork / dTUnitPerRot) * dHldrPitch_ZRotRatio;

                    double dZLSpeed = (dZLMovePos / dTLWork) * dThetaWorkSpeed;
                    double dZRSpeed = (dZRMovePos / dTRWork) * dThetaWorkSpeed;

                    double dZLAcc = (dZLMovePos / dTLWork) * dThetaWorkAcc;
                    double dZRAcc = (dZRMovePos / dTRWork) * dThetaWorkAcc;


                    double dTemp;
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TL);
                        SM.MT.GoInc((int)mi.PCK_TL, -dTLWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZL, -dZLMovePos, dZLSpeed, dZLAcc, dZLAcc);
                    }
                    else
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TR);
                        SM.MT.GoInc((int)mi.PCK_TR, -dTRWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZR, -dZRMovePos, dZRSpeed, dZRAcc, dZRAcc);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        if (!GetStop(mi.PCK_TL, true)) return false;


                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        if (!GetStop(mi.PCK_TR, true)) return false;

                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!m_tmDelay.OnDelay(100)) return false;

                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)//need back move when unlock.
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TL);
                        SM.MT.GoInc((int)mi.PCK_TL, OM.DevOptn.dThetaBackPos, OM.DevOptn.dThetaWorkSpeed, OM.DevOptn.dThetaWorkAcc, OM.DevOptn.dThetaWorkAcc);
                    }
                    else
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TR);
                        SM.MT.GoInc((int)mi.PCK_TR, OM.DevOptn.dThetaBackPos, OM.DevOptn.dThetaWorkSpeed, OM.DevOptn.dThetaWorkAcc, OM.DevOptn.dThetaWorkAcc);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        //if (!GetStop(mi.PCK_TL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 0);
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    else
                    {
                        //if (!GetStop(mi.PCK_TR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 0);
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "EncoderOffset", 4001);
                        //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    }
                    else
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "EncoderOffset", 4001);
                        //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    }
                    Step.iCycle++;
                    return false;

                case 21:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Visn);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Visn);
                    }

                    if (SortInfo.eTray == ri.REAR)
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.Work);
                    }
                    else
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.Work);
                    }

                    if (OM.CmnOptn.bIgnrLeftPck || OM.CmnOptn.bIgnrRightPck)
                    {
                        //if ((GetPckStat(cs.Visn) == 1 && DM.ARAY[(int)iTray].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)iTray].GetCntStat(cs.Visn) == 0) ||
                        //    (GetPckStat(cs.Visn) == 1 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) == 1))
                        //{
                        //if (DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) == 1 || OM.CmnOptn.bIgnrLeftPck || OM.CmnOptn.bIgnrRightPck)
                        //{
                        //FindChip(ref c, ref r, cs.Visn, ri.PICK);
                        DM.ARAY[(int)ri.PICK].ChangeStat(cs.Visn, cs.Align);

                    }

                    Step.iCycle = 0;
                    return true;

                //Up Write
                case 50:
                    FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if (c == 0)
                    {//픽커 1번 찝을때
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                    }
                    else
                    {//픽커 2번 찝을때.
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 51:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 52:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    SetY(yi.PCK_EjtLtOn, false);
                    SetY(yi.PCK_EjtRtOn, false);
                    FindChip(ref c, ref r, cs.Visn, ri.LENS);
                    if (c == 0)
                    {//픽커 1번 찝을때
                        SetErr(ei.PRT_VacErr, "Left Picker Pickup Vaccum Error");
                        DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                        //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);

                    }
                    else
                    {//픽커 2번 찝을때.
                        SetErr(ei.PRT_VacErr, "Right Picker Pickup Vaccum Error");
                        DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                        //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    }
                    Step.iCycle = 0;
                    return true;

            }
        }

        //ScrewType, Locking Type 공용
        public bool CycleUnlockAlign()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    //FindChip(ref c , ref r , cs.Visn , ri.PICK);
                    DM.ARAY[(int)ri.PICK].ChangeStat(cs.Visn, cs.Align);

                    Step.iCycle = 0;
                    return true;
            }
        }

        //ScrewType, Locking Type 공용
        public bool CycleUnlockPlace()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            double dPckXPos = 0;
            double dPckYPos = 0;
            double dPckTPos = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    //if (!GetSortInfo(OM.DevOptn.bUseMultiHldr, ref SortInfo))
                    //{
                    //    Step.iCycle = 0;
                    //    return true;
                    //}


                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    //MoveMotr(mi.PCK_TL , pv.PCK_TLWait);
                    //MoveMotr(mi.PCK_TR , pv.PCK_TRWait);
                    //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    

                    Step.iCycle++;
                    return false;

                case 12:
                    FindChip(ref c, ref r, cs.Empty, ri.LENS);//X는 오른쪽이홈 Y는 rear쪽이홈.
                    dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnLensStt) - c * OM.DevInfo.dLensColPitch;
                    dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnLensStt) - r * OM.DevInfo.dLensRowPitch;

                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs);
                    }

                    MoveMotr(mi.PCK_X, dMoveX);
                    MoveMotr(mi.STG_Y, dMoveY);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLPick);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRPick);
                    }

                    Step.iCycle++;
                    return false;
                case 14:
                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                        
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                        
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    SetY(yi.PCK_EjtLtOn, false);
                    SetY(yi.PCK_EjtRtOn, false);

                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    Step.iCycle++;
                    return false;

                case 16:
                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "EncoderOffset", 4001);
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Unkwn);
                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "EncoderOffset", 4001);
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Unkwn);
                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    
                    

                    Step.iCycle = 0;
                    return true;
            }
















            /*
            public bool CycleHoldrCalib()
            {
                String sTemp;
                if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
                {
                    sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                    sTemp = m_sPartName + sTemp;
                    SetErr(ei.PRT_CycleTO, sTemp);
                    Log.Trace(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;
                }

                if (Step.iCycle != PreStep.iCycle)
                {
                    sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                    Log.Trace(m_sPartName, sTemp);
                }

                PreStep.iCycle = Step.iCycle;

                if (Stat.bReqStop)
                {
                    //Step.iHome = 0;
                    //return true ;
                }

                int r = 0, c = 0;

                switch (Step.iCycle)
                {
                    default:
                        sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                        Log.ShowMessage(m_sPartName, sTemp);
                        Step.iCycle = 0;
                        return true;

                    case 0:
                        return false;

                    case 10:
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                        Step.iCycle++;
                        return false;

                    case 11:
                        if (!GetStop(mi.PCK_ZL)) return false;
                        if (!GetStop(mi.PCK_ZR)) return false;
                        Step.iCycle++;
                        return false;

                    case 12:
                        Step.iCycle++;
                        return false;

                    case 13: //일단은 여기서 레디 확인 하나 나중에 디버깅 끝나면 옮기자 렉타임 없게.                    
                        Step.iCycle++;
                        return false;

                    case 14:
                        MoveMotr(mi.PCK_X, pv.PCK_XVisnRearStt);
                        MoveMotr(mi.STG_Y, pv.STG_YVisnRearStt);
                        VC.SendVisnMsg(VC.sm.Ready);
                        Step.iCycle++;
                        return false;

                    case 15:
                        if (!GetStop(mi.PCK_X)) return false;
                        if (!GetStop(mi.STG_Y)) return false;
                        //나중에 통신 확인 여기.

                        if (!VC.IsEndSendMsg()) return false;
                        if (VC.GetVisnRecvErrMsg() != "")
                        {
                            SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                            Step.iCycle = 0;
                            return true;
                        }
                        if (VC.GetVisnSendMsg() != "OK")
                        {
                            SetErr(ei.VSN_ComErr, "Vision Not Ready");
                            Step.iCycle = 0;
                            return true;
                        }
                        m_tmDelay.Clear();
                        Step.iCycle++;
                        return false;

                    case 16:
                        if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                        VC.SendVisnMsg(VC.sm.Insp, "0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                        Step.iCycle++;
                        return false;

                    case 17: //렌즈먼저 검사. 
                        if (!VC.IsEndSendMsg()) return false;
                        if (VC.GetVisnRecvErrMsg() != "")
                        {
                            SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                            Step.iCycle = 0;
                            return true;
                        }

                        if (VC.GetVisnSendMsg() == "NG")
                        {
                            SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                            Step.iCycle = 0;
                            return true;
                        }

                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLens.dX, ref RsltLens.dY, ref RsltLens.dT))
                        {
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }
                        Step.iCycle++;
                        return false;

                    case 18:
                        VC.SendVisnMsg(VC.sm.Insp, "1");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                        Step.iCycle++;
                        return false;

                    case 19:  //홀더 검사.
                        if (!VC.IsEndSendMsg()) return false;
                        if (VC.GetVisnRecvErrMsg() != "")
                        {
                            SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                            Step.iCycle = 0;
                            return true;
                        }

                        if (VC.GetVisnSendMsg() == "NG")
                        {
                            SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                            Step.iCycle = 0;
                            return true;
                        }

                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldr.dX, ref RsltHldr.dY, ref RsltHldr.dT))
                        {
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        //렌즈와 홀더간의 오프셑 계산하여 
                        //홀더에서 T값을 얻게되면 
                        PM.SetValue(mi.PCK_TL, pv.PCK_TLHolderPutOfs, RsltLens.dT - RsltHldr.dT);
                        PM.SetValue(mi.PCK_TR, pv.PCK_TLHolderPutOfs, RsltLens.dT - RsltHldr.dT);

                        MoveMotr(mi.PCK_X, pv.PCK_XWait);
                        MoveMotr(mi.STG_Y, pv.STG_YWait);
                        Step.iCycle++;
                        return false;

                    case 20:
                        if (!GetStop(mi.PCK_X)) return false;
                        if (!GetStop(mi.STG_Y)) return false;
                        PM.Save(OM.GetCrntDev());

                        Step.iCycle = 0;
                        return true;

                }
            }
        */



        }


    }
}
