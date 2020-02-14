using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using SML2;

namespace Machine
{
    public partial class FormLotOpen : Form
    {
        public static FormLotOpen FrmLotOpen;

        public FormLotOpen()
        {
            InitializeComponent();

            ////Control Scale 변경
            //float widthRatio = 1024f / 1280f;
            //float heightRatio = 607f / 863f;
            //SizeF scale = new SizeF(widthRatio, heightRatio);
            //this.Scale(scale);
            
            //foreach (Control control in this.Controls)
            //{
            //    control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            //}
            
            tbSelDevice.Text = OM.GetCrntDev();
            tbLotId.Text = "";

            tbLotId.Focus();
            
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            if (tbLotId.Text == "") return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");

            string LotNo  = tbLotId.Text.Trim();
            string Device = tbSelDevice.Text.Trim();

            DM.ARAY[ri.WLDB].SetStat(cs.Unknown);
            DM.ARAY[ri.SLDB].SetStat(cs.Unknown);

            if(cbUseTopLd.Checked) {
                 DM.ARAY[ri.WLDT].SetStat(cs.Unknown);
                 DM.ARAY[ri.SLDT].SetStat(cs.Unknown);

            }


            DM.ARAY[ri.WSTG].SetStat(cs.None);
            DM.ARAY[ri.SSTG].SetStat(cs.None);

            DM.ARAY[ri.PCKR].SetStat(cs.None);

            /*
             * if(SML.IO.GetX((int)xi.WLDR_MgzCheck)) {
                DM.ARAY[ri.WLDT].SetStat(cs.Unknown);
                DM.ARAY[ri.WLDB].SetStat(cs.Unknown);
            } 
            else {
                DM.ARAY[ri.WLDB].SetStat(cs.Unknown);
            }

            if(SML.IO.GetX((int)xi.SLDR_MgzCheck)) {
                DM.ARAY[ri.SLDT].SetStat(cs.Unknown);
                DM.ARAY[ri.SLDB].SetStat(cs.Unknown);
            } 
            else {
                DM.ARAY[ri.SLDB].SetStat(cs.Unknown);
            }*/


            //string Operator = edOperator  -> Text.Trim() ;

            //FM_MsgOk("Check","로더 언로더 외의 ");

            //    PRI_F.m_iWorkPkgCnt = 0 ;
            //    PRI_R.m_iWorkPkgCnt = 0 ;




            //스테이지 미드 갯수.
            //int iMidColCnt = OM.DevInfo.iMidColCnt;
            //int iMidRowCnt = OM.DevInfo.iMidRowCnt;
            //int iMidMaxCnt = iMidColCnt * iMidRowCnt;

            //DM.ClearMap();
            //DM.ARAY[riLDR_F].SetStat(csEmpty);
            //DM.ARAY[riLDR_R].SetStat(csEmpty);
            //DM.ARAY[riULD].SetStat(csEmpty);

            //체크 모드 Stat 변경
            ////if(OM.CmnOptn.iWorkMode == 1) { //측정모드일때.
            //    DM.ARAY[riLDR_F].SetStat(csDetect);
            //    LOT.LotOpen(LotNo , Device) ;
            //    Trace("LotOpen" , "Try");
            //    Close();
            //    return ;
            //}

            //프론트 로더 Stat 변경

            //엑셀 미들블럭 데이터 개수.
            //int iMidDataCnt = g_LotData.GetMaxRow() - 1;
            //
            //if (OM.CmnOptn.iWorkMode == 1)
            //{ //측정모드일때.
            //    DM.ARAY[riLDR_F].SetStat(csEmpty);
            //}
            //else if (OM.CmnOptn.iWorkMode == 0 || OM.CmnOptn.iWorkMode == 2)
            //{
            //    int iCrntMidDataCnt = 0;
            //    String sCrntMidId = "";
            //    for (int r = 0; r < OM.DevInfo.iLDRFMgzSlotCnt; r++)
            //    {
            //        for (int c = OM.DevInfo.iLDRFMgzBayCnt - 1; c >= 0; c--)
            //        { //로더 슬롯은 장비전면에서 오른쪽것이 처음이다.
            //            if (iCrntMidDataCnt >= iMidDataCnt) continue;
            //            iCrntMidDataCnt++;
            //            //sCrntMidId=g_LotData.GetCellByColTitle("MiddleBlockID",iCrntMidDataCnt);
            //            //if(sCrntMidId != ""){
            //            //DM.ARAY[riLDR_F].CHPS[r][c].SetID(sCrntMidId);
            //            DM.ARAY[riLDR_F].SetStat(r, c, csUnkwn);
            //            //}
            //        }
            //    }
            //}

            //리어 로더 Stat 변경
            //if (OM.CmnOptn.iWorkMode == 2)
            //{
            //    DM.ARAY[riLDR_R].SetStat(csEmpty);
            //}
            //else if (OM.CmnOptn.iWorkMode == 0 || OM.CmnOptn.iWorkMode == 1)
            //{
            //    int iCrntModDataCnt = 0;
            //    const int iStgModCnt = OM.DevInfo.iMidColCnt * OM.DevInfo.iMidRowCnt;
            //    int iCrntStgDataIdx = 0; //iStgModCnt
            //    //엑셀 모듈 데이터 갯수
            //    int iModDataCnt = iMidDataCnt * OM.DevInfo.iMidColCnt * OM.DevInfo.iMidRowCnt;
            //    String sCrntModId = "";
            //    String sCrntColTilte = "";
            //    for (int r = 0; r < OM.DevInfo.iLDRRMgzSlotCnt; r++)
            //    {
            //        for (int c = OM.DevInfo.iLDRRMgzBayCnt - 1; c >= 0; c--)
            //        { //로더 슬롯은 장비전면에서 오른쪽것이 처음이다.
            //            if (iCrntModDataCnt >= iModDataCnt) continue;
            //            iCrntStgDataIdx = iCrntModDataCnt % iStgModCnt; //미들블럭 개별 카운트 인덱스.
            //            sCrntColTilte = String("ModuleID") + (iCrntStgDataIdx % OM.DevInfo.iMidColCnt) + (iCrntStgDataIdx / OM.DevInfo.iMidColCnt);
            //            //sCrntModId=g_LotData.GetCellByColTitle(sCrntColTilte,iCrntModDataCnt/OM.DevInfo.iMidColCnt+1);
            //            sCrntModId = g_LotData.GetCellByColTitle(sCrntColTilte, iCrntModDataCnt / iStgModCnt + 1);
            //            iCrntModDataCnt++;
            //            if (sCrntModId != "")
            //            {
            //                DM.ARAY[riLDR_R].CHPS[r][c].SetID(sCrntModId);
            //                DM.ARAY[riLDR_R].SetStat(r, c, csUnkwn);
            //            }
            //        }
            //    }
            //}

            
            LOT.LotOpen(LotNo, Device);

            SPC.WRK.DataClear();

            Log.Trace("LotOpen", "Try");
            Close();
            
    
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();   
        }

        private void FormLotOpen_Shown(object sender, EventArgs e)
        {
            tbLotId.Focus();
        }


    }
}
