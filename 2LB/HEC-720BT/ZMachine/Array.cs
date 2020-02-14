using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using COMMON;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Machine
{
    public struct TChip
    {
        public cs Stat;
        //public string sData ;
        public void Clear()
        {
            Stat  = cs.None;
            //sData = "" ;
        }
    }

    public class CArrayData
    {
        public CArrayData()
        {
            
        }
        
        private TChip[,]    m_tChips   = new TChip[1, 1];
        private String      m_sName    = ""   ; //어레이의 이름 ex)PreBufferZone
        private String      m_sLotNo   = ""   ; //장비에 따라 랏 넘버를 붙여 줄수 있다
        private String      m_sID      = ""   ;//스트립의 메가진 No 슬롯 No ;  m_sID/100 = 메가진 카운트 , m_sID%100 = 슬롯 카운터.
        private int         m_iStep    = 0    ; //어레이에서 어떤 작업을 함에 있어서 스텝. 데이터를 건드리지 않고 작업의 단계를 나타낼때 쓴다.
        private int         m_iSubStep = 0    ; //어레이에서 어떤 작업을 함에 있어서 스텝. 데이터를 건드리지 않고 작업의 단계를 나타낼때 쓴다.(Sub Step)
        private int         m_iMaxCol  = 1    ;
        private int         m_iMaxRow  = 1    ;

        public TChip[,]    Chip   { get { return m_tChips;  } set { m_tChips   = value; } }
        public String      Name   { get { return m_sName;   } set { m_sName    = value; } }
        public String      LotNo  { get { return m_sLotNo;  } set { m_sLotNo   = value; } }
        public String      ID     { get { return m_sID;     } set { m_sID      = value; } }
        public int         Step   { get { return m_iStep;   } set { m_iStep    = value; } }
        public int         SubStep{ get { return m_iSubStep;} set { m_iSubStep = value; } }
        public int         MaxCol { get { return m_iMaxCol; } set { SetMaxColRow(value, m_iMaxRow); } }
        public int         MaxRow { get { return m_iMaxRow; } set { SetMaxColRow(m_iMaxCol, value); } }

        virtual protected void OnChange()
        {

        }

        public void ClearMap()
        {
            for(int r = 0; r < m_iMaxRow; r++){
                for(int c = 0; c < m_iMaxCol; c++){
                    m_tChips[c, r].Clear();
                }
            }

            m_sLotNo   = "";
            m_sID      = "";
            m_iStep    = 0 ;
            m_iSubStep = 0 ;

            OnChange();
        }
       
        public void FlipX()
        {
            TChip[,] chps = new TChip[m_iMaxCol , m_iMaxRow];
            
            for (int i = 0; i < m_iMaxRow; i++)
            {
                for (int j = 0; j < m_iMaxCol; j++)
                {
                    chps[j, i] = m_tChips[ m_iMaxCol - 1 - j , i];
                }
            }
            Array.Copy(chps, 0, m_tChips, 0, m_tChips.Length);
            
            OnChange();
         
        }
        public void FlipY()
        {
            TChip[,] chps = new TChip[m_iMaxCol, m_iMaxRow];

            for (int i = 0; i < m_iMaxRow; i++)
            {
                for (int j = 0; j < m_iMaxCol; j++)
                {
                    chps[j, i] = m_tChips[j,m_iMaxRow - 1 - i];
                }
            }
            Array.Copy(chps, 0, m_tChips, 0, m_tChips.Length);
         
            OnChange();
         
        }
        public void FlipXY()
        {

            TChip[,] chps = new TChip[m_iMaxCol, m_iMaxRow];

            for (int i = 0; i < m_iMaxRow; i++)
            {
                for (int j = 0; j < m_iMaxCol; j++)
                {
                    chps[j, i] = m_tChips[m_iMaxCol - 1 - j, m_iMaxRow - 1 - i];
                }
            }
            Array.Copy(chps, 0, m_tChips, 0, m_tChips.Length);

            OnChange();
            
        }
        public void TurnCw90()  //시계 방향.
        {
            //버퍼 생성.
            TChip[,] chps = new TChip[m_iMaxRow, m_iMaxCol];

            //데이터 버퍼로 옮김.
            for (int i = 0; i < m_iMaxCol; i++)
            {
                for (int j = 0; j < m_iMaxRow; j++)
                {
                    chps[j,i] = m_tChips[i, m_iMaxRow - 1 - j];
                }
            }



            //맥스 세팅.
            SetMaxColRow(m_iMaxRow, m_iMaxCol);

            Array.Copy(chps, 0, m_tChips, 0, m_tChips.Length);

            OnChange();
            
        }
        public void TurnCw180()
        {
            TChip[,] chps = new TChip[m_iMaxCol, m_iMaxRow];

            for (int i = 0 ; i < m_iMaxRow ; i++) {
                for (int j = 0 ; j < m_iMaxCol ; j++) {
                    chps[j, i] = m_tChips[m_iMaxCol - 1 - j, m_iMaxRow - 1 - i];
                }
            }

            Array.Copy(chps, 0, m_tChips, 0, m_tChips.Length);
                        
            OnChange();

        }
        public void TurnCw270()
        {
            TChip[,] chps = new TChip[m_iMaxCol,m_iMaxRow];

            for (int i = 0 ; i < m_iMaxCol ; i++) {
                for (int j = 0 ; j < m_iMaxRow ; j++) {
                                   //r,c
                    chps[j,i] = m_tChips[m_iMaxCol - 1 - i, j];
                }
            }
            
            SetMaxColRow( m_iMaxCol,m_iMaxRow);
            Array.Copy(chps, 0, m_tChips, 0, m_tChips.Length);
                       
            OnChange();
        }
        public void SetMaxColRow (int _iC,int _iR)
        {
            if(m_iMaxCol == _iC && m_iMaxRow == _iR) return ;

            m_iMaxCol = _iC ;
            m_iMaxRow = _iR;
            TChip[,] chps = new TChip[_iC, _iR];
            int iCol = Math.Min(_iC, m_tChips.GetLength(0));
            int iRow = Math.Min(_iR, m_tChips.GetLength(1));
            
            for(int r = 0; r < _iR; r++){
                for(int c = 0; c < _iC; c++){
                    chps[c,r] = new TChip();
                }
            }
              

            for (int r = 0; r < iRow; r++)
                for (int c = 0; c < iCol; c++)
                    chps[c, r] = m_tChips[c, r];

            m_tChips = chps;            
            OnChange();
        }

        //Search Chip.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool FindChip(cs _eStat, int _iC, int _iR)
        {
            bool bRet = CheckStat(_iC , _iR, _eStat);
            return bRet ;
        }

        public int FindFrstRow(cs _eStat)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstRowCol(_eStat, ref iColNum, ref iRowNum);
            return iRowNum;
        }
        public int FindFrstCol(cs _eStat)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstColRow(_eStat, ref iColNum, ref iRowNum);
            return iColNum;

        }
        public int FindLastRow(cs _eStat)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastRowCol(_eStat, ref iColNum, ref iRowNum);

            return iRowNum;
        }
        public int FindLastCol(cs _eStat)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastColRow(_eStat, ref iColNum, ref iRowNum);
            
            //return Find Row No.
            return iColNum;

        }
        public bool FindFrstRowCol(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = 0 ; c < m_iMaxCol ; c++) {
                    if (FindChip(_eStat , c , r)) {
                        _iC = c;
                        _iR = r;
                        return true;
                    }
                }
            }
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;

        }
        public bool FindFrstColRow(cs _eStat, ref int _iC, ref int _iR)
        {
            
            //Local Var.
            for (int c = 0 ; c < m_iMaxCol ; c++) {
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                        
                    }
                }
            }
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        
        }
        public bool FindLastRowCol(cs _eStat, ref int _iC, ref int _iR)
        {
            for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                for (int c = m_iMaxCol - 1 ; c >= 0 ; c--) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstRowLastCol(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = m_iMaxCol - 1 ; c >= 0 ; c--) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindLastRowFrstCol(cs _eStat, ref int _iC, ref int _iR)
        {
            for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                for (int c = 0 ; c < m_iMaxCol ; c++) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindLastColFrstRow(cs _eStat, ref int _iC, ref int _iR)
        {
            for (int c = m_iMaxCol - 1 ; c >= 0 ; c--) {
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    if (FindChip(_eStat , c , r)) {
                        _iC = c;
                        _iR = r;
                        return true;
                    }
                }
            }
            
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstColLastRow(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int c = 0 ; c < m_iMaxCol ; c++) {
                for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindLastColRow(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int c = m_iMaxCol-1 ; c >= 0 ; c--) {
                for (int r = m_iMaxRow-1 ; r >= 0 ; r--) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;

        }

        public bool FindFrstRowCol_Indx(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0        ) { _iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) { _iR = -1; _iC = -1; return false ;}
            
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = iStrCol ; c <= iEndCol ; c++) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstRowLastCol_Indx(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = iEndCol ; c >= iStrCol ; c--) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }

        public bool FindFrstColRow_Indx(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            
            for (int c = iStrCol ; c <= iEndCol ; c++) {
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    if (FindChip(_eStat , c , r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstColLastRow_Indx(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            
            //Local Var.
            for (int c = iStrCol ; c <= iEndCol ; c++) {
                for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                    if (FindChip(_eStat , c ,r)) {
                        _iR = r;
                        _iC = c;
                        return true;
                    }
                }
            }
            
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }

        //Loading Para.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Load(CConfig Config , bool IsLoad)
        {

            String sArayName;
            int iStat;
            
            sArayName = string.Format("ARRAY_" + m_sName);

            //Load
            String sRowData;
            String sIndex;
            String sTemp;
            int iMaxCol , iMaxRow ;
            if(m_sName == "") return;
            if(IsLoad) {
                
                
                Config.GetValue(sArayName,"m_iMaxRow" ,  out iMaxRow );
                Config.GetValue(sArayName,"m_iMaxCol" ,  out iMaxCol );
                Config.GetValue(sArayName,"m_sID"     ,  out m_sID   );
                Config.GetValue(sArayName,"m_sLotNo"  ,  out m_sLotNo);
                Config.GetValue(sArayName,"m_iStep"   ,  out m_iStep );
                Config.GetValue(sArayName,"m_iSubStep",  out m_iSubStep);
                if(iMaxRow < 1) iMaxRow = 1 ;
                if(iMaxCol < 1) iMaxCol = 1 ;
            
                //메모리 재할당.
                SetMaxColRow(iMaxCol , iMaxRow);
            
                try
                {
                    for (int r = 0; r < m_iMaxRow; r++)
                    {
                        sIndex = string.Format("Row{0:000}", r);
                        Config.GetValue(sArayName , sIndex, out sRowData);
                        for (int c = 0; c < m_iMaxCol; c++)
                        {
                            sTemp = sRowData.Substring(c * 3, 3);
                            iStat = int.Parse(sTemp);
                            m_tChips[c, r].Stat = (cs)iStat;
                        }
                    }
                }
                catch(Exception e)
                {

                    Log.ShowMessage("Exception",e.Message);
                    //Load(false);

                }


                
            
                OnChange();
            }
            else {
                Config.SetValue(sArayName,"m_iMaxRow" ,  m_iMaxRow );
                Config.SetValue(sArayName,"m_iMaxCol" ,  m_iMaxCol );
                Config.SetValue(sArayName,"m_sID"     ,  m_sID     );
                Config.SetValue(sArayName,"m_sLotNo"  ,  m_sLotNo  );
                Config.SetValue(sArayName,"m_iStep"   ,  m_iStep   );
                Config.SetValue(sArayName,"m_iSubStep",  m_iSubStep); //원래 주석이였다.
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    sRowData = "" ;
                    sIndex = string.Format("Row{0:000}", r);
                    for (int c = 0 ; c < m_iMaxCol ; c++) {
                        sTemp = string.Format("{0:000}", (int)m_tChips[c, r].Stat);
                        sRowData += sTemp ;
                    }
                    Config.SetValue(sArayName , sIndex ,sRowData );
                }

            } 
        }

        //inline func
        //======================================================================
        public bool SetStat( int _iC , int _iR , cs _eStat) {
            if (_iR < 0 || _iC < 0 || _iR > m_iMaxRow || _iC > m_iMaxCol) return false;

            m_tChips[_iC, _iR].Stat = _eStat;
            OnChange();
            return true;
        }
        public void SetStat(cs _eStat)
        {

            String sHead = "SetStatAll " + m_sName;
            //Trace( sHead.c_str() , String(_eStat).c_str());
            for (int r = 0; r < m_iMaxRow; r++)
            {
                for (int c = 0; c < m_iMaxCol; c++)
                {
                    m_tChips[c, r].Stat = _eStat;
                }
            }

            if (_eStat == cs.None)
            {
                m_iStep = 0;
                m_iSubStep = 0;
            }


            OnChange();

        }

        public bool RangeSetStat(int _iSc, int _iSr, int _iEc, int _iEr, cs _eStat)
        {
            if (_iSr < 0 || _iSc < 0 || _iSr >= m_iMaxRow || _iSc >= m_iMaxCol) return false;
            if (_iEr < 0 || _iEc < 0 || _iEr >= m_iMaxRow || _iEc >= m_iMaxCol) return false;
            if (_iSr > _iEr || _iSc > _iEc) return false;
            for (int r = _iSr; r <= _iEr; r++)
            {
                for (int c = _iSc; c <= _iEc; c++) 
                {
                    m_tChips[c,r].Stat = _eStat;
                }
            }
            OnChange();
            return true ;
        }

        public void ChangeStat(cs _eFrom , cs _eTo) 
        {
            for (int r = 0; r < m_iMaxRow; r++){
                for (int c = 0; c < m_iMaxCol; c++)
                {
                    if (m_tChips[c,r].Stat == _eFrom) m_tChips[c,r].Stat = _eTo;
                }
            }
            OnChange();
            
        }



        public int  GetMaxRow () { return  m_iMaxRow ;}
        public int  GetMaxCol () { return  m_iMaxCol ;}

        public cs GetStat( int c , int r) 
        {
            if (r < 0 || c < 0 || r > m_iMaxRow || c > m_iMaxCol)
            {
                return cs.None;
            }
            return m_tChips[c,r].Stat;
        }
        public bool CheckStat(int c, int r, cs Stat) 
        {
            if (r < 0 || c < 0 || r > m_iMaxRow || c > m_iMaxCol)
            {
                return false;
            }
            return GetStat(c,r) == Stat ; 
        }

        
        public bool CheckAllStat (cs Stat) 
        {
            for (int r = 0; r < m_iMaxRow; r++)
            {
                for (int c = 0; c < m_iMaxCol; c++)
                {
                    if (!CheckStat(c,r, Stat))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        //만들다가 로드세이브 구현 귀찮아서 않함.
        //public bool SetChipData(int c , int r , string Data)
        //{
        //    if (r < 0 || c < 0 || r > m_iMaxRow || c > m_iMaxCol)
        //    {
        //        return false;
        //    }
        //    m_tChips[c,r].sData = Data;
        //    return true ;
        //}

        //public string GetChipData(int c , int r)
        //{
        //    if (r < 0 || c < 0 || r > m_iMaxRow || c > m_iMaxCol)
        //    {
        //        return "";
        //    }
        //    return m_tChips[c,r].sData ;
        //}

        //Get Row Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int  GetCntRowStat  (int r, cs Stat) { int iCnt = 0; for (int c = 0 ; c < m_iMaxCol ; c++) if (CheckStat (c,r,Stat)) iCnt++; return iCnt; }

        //Get Col Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int  GetCntColStat  (int c, cs Stat) { int iCnt = 0; for (int r = 0 ; r < m_iMaxRow ; r++) if (CheckStat (c,r,Stat)) iCnt++; return iCnt; }

        //Get All Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int  GetCntStat (cs Stat) { int iCnt = 0; for (int r = 0 ; r < m_iMaxRow ; r++) iCnt += GetCntRowStat (r,Stat); return iCnt; }

        //Sub Stat
        //public void SetSubStat(int r , int c , int _iIdx , EN_CHIP_STAT _iSubStat) { m_tChips[r,c].SetSubStat(_iIdx , _iSubStat); }
        //public EN_CHIP_STAT GetSubStat(int r , int c , int _iIdx ) { return m_tChips[r,c].GetSubStat(_iIdx); }
        //public void SetSubStat(int _iIdx, EN_CHIP_STAT _iSubStat) {
        //    for(int r = 0 ; r < m_iMaxRow ; r++)
        //        for(int c = 0 ; c < m_iMaxCol ; c++)
        //            m_tChips[r,c].SetSubStat(_iIdx , _iSubStat);
        //}

        public void CopyData(ref CArrayData _cArray){
            SetMaxColRow(_cArray.m_iMaxCol , _cArray.m_iMaxRow) ;

            m_sID      = _cArray.m_sID     ;
            m_sLotNo   = _cArray.m_sLotNo  ;
            m_iStep    = _cArray.m_iStep   ;
            m_iSubStep = _cArray.m_iSubStep;

            for(int r = 0 ; r < m_iMaxRow ; r++){
                //memcpy(CHPS[r],_cArray.CHPS[r],sizeof(CChip)*m_iMaxCol);
                for (int c = 0; c < m_iMaxCol; c++) m_tChips[r, c] = _cArray.m_tChips[r, c];

            }

            OnChange();
        }

    }

    public class CArray : CArrayData
    {   
        public enum EN_STATUS_POPUP_ID : uint
        {
            spOne  = 0 ,
            spCol      ,
            spRow      ,
            spAll      ,
            spDrag     ,

            MAX_STATUS_POPUP_ID
        }

        private struct TClickStat
        {
            public bool bMouseDown;
            public int iSelX1, iSelY1, iSelX2, iSelY2;
            public int iSelR1, iSelR2, iSelC1, iSelC2;
        }
        TClickStat ClickStat;

        private bool m_bSameCellSize; //셀사이즈 관련.
        private PictureBox pbArray = new PictureBox();
        //pbArray = new DoubleBufferP();

        //One,Col,Row,All,Drag 총4개.
        private ContextMenuStrip [] cmPopupMenus = new ContextMenuStrip[(int)EN_STATUS_POPUP_ID.MAX_STATUS_POPUP_ID];

        struct TStatProp{
            public bool   Visible   ;
            public Color  ChipColor ;
            public string Caption   ;
            
        }

        private TStatProp[] StatProp = new TStatProp[(int)cs.MAX_CHIP_STAT];
        public Point lpPoint;

        public CArray()
        {
         
            for (int i = 0; i < (int)EN_STATUS_POPUP_ID.MAX_STATUS_POPUP_ID; i++)
            {
                cmPopupMenus[i] = new ContextMenuStrip();
                cmPopupMenus[i].Closing += Popup_Closing;


            }

            for (int i = 0; i < (int)cs.MAX_CHIP_STAT; i++)
            {
                StatProp[i].Visible   = false ;
                StatProp[i].ChipColor = Color.Black ;
                StatProp[i].Caption   = "NoCaption";
            }

            

            //PictureBox Mouse Event.
            pbArray.MouseMove += new MouseEventHandler (pbArray_MouseMove);
            pbArray.MouseDown += new MouseEventHandler (pbArray_MouseDown);
            pbArray.MouseUp   += new MouseEventHandler (pbArray_MouseUp  );
            pbArray.Paint     += new PaintEventHandler (pbArray_Paint    );
        }

        

        private void SetPopupMenuStat()
        {
            ToolStripMenuItem miStatusOne ;
            ToolStripMenuItem miStatusCol ;
            ToolStripMenuItem miStatusRow ;
            ToolStripMenuItem miStatusAll ;
            ToolStripMenuItem miStatusDrag;


            cmPopupMenus[(int)EN_STATUS_POPUP_ID.spOne ].Items.Clear();
            cmPopupMenus[(int)EN_STATUS_POPUP_ID.spCol ].Items.Clear();
            cmPopupMenus[(int)EN_STATUS_POPUP_ID.spRow ].Items.Clear();
            cmPopupMenus[(int)EN_STATUS_POPUP_ID.spAll ].Items.Clear();
            cmPopupMenus[(int)EN_STATUS_POPUP_ID.spDrag].Items.Clear();

            for (int s = 0; s < (int)cs.MAX_CHIP_STAT; s++)
            {
                miStatusOne = new ToolStripMenuItem();
                miStatusOne.Visible = StatProp[s].Visible;
                miStatusOne.Text = "ONE_" + StatProp[s].Caption;
                miStatusOne.Tag = s;
                miStatusOne.Click += new EventHandler(this.OneClick);
                miStatusOne.Paint += new PaintEventHandler(this.ItemPaint);
                cmPopupMenus[(int)EN_STATUS_POPUP_ID.spOne].Items.Add(miStatusOne);

                miStatusCol = new ToolStripMenuItem();
                miStatusCol.Visible = StatProp[s].Visible;
                miStatusCol.Text = "COL_" + StatProp[s].Caption;
                miStatusCol.Tag = s;
                miStatusCol.Click += new EventHandler(this.ColClick);
                miStatusCol.Paint += new PaintEventHandler(this.ItemPaint);
                cmPopupMenus[(int)EN_STATUS_POPUP_ID.spCol].Items.Add(miStatusCol);

                miStatusRow = new ToolStripMenuItem();
                miStatusRow.Visible = StatProp[s].Visible;
                miStatusRow.Text = "ROW_" + StatProp[s].Caption;
                miStatusRow.Tag = s;
                miStatusRow.Click += new EventHandler(this.RowClick);
                miStatusRow.Paint += new PaintEventHandler(this.ItemPaint);
                cmPopupMenus[(int)EN_STATUS_POPUP_ID.spRow].Items.Add(miStatusRow);

                miStatusAll = new ToolStripMenuItem();
                miStatusAll.Visible = StatProp[s].Visible;
                miStatusAll.Text = "ALL_" + StatProp[s].Caption;
                miStatusAll.Tag = s;
                miStatusAll.Click += new EventHandler(this.AllClick);
                miStatusAll.Paint += new PaintEventHandler(this.ItemPaint);
                cmPopupMenus[(int)EN_STATUS_POPUP_ID.spAll].Items.Add(miStatusAll);

                miStatusDrag = new ToolStripMenuItem();
                miStatusDrag.Visible = StatProp[s].Visible;
                miStatusDrag.Text = "DRG_" + StatProp[s].Caption;
                miStatusDrag.Tag = s;
                miStatusDrag.Click += new EventHandler(this.DragClick);
                miStatusDrag.Paint += new PaintEventHandler(this.ItemPaint);
                cmPopupMenus[(int)EN_STATUS_POPUP_ID.spDrag].Items.Add(miStatusDrag);
                
            }

        }

        public void SetParent(Panel _pnBase)
        {
            //pbArray.Parent = _pnBase;

            pbArray.Width  = _pnBase.Width   ;
            pbArray.Height = _pnBase.Height  ;

            pbArray.Image = new Bitmap(pbArray.Width, pbArray.Height);

            _pnBase.Controls.Add(pbArray);

            m_bSameCellSize = false;
            OnChange();

        }

        public void SetDispColor(cs _eStat, Color _cColor)
        {
            StatProp[(int)_eStat].ChipColor = _cColor;
            SetPopupMenuStat();
            OnChange();

        }

        public void SetDispName(cs _eStat, String _sName)
        {
            StatProp[(int)_eStat].Caption = _sName;
            SetPopupMenuStat();
            OnChange();

        }
        public void SetVisible(cs _eStat, bool _bVisible)
        {
            StatProp[(int)_eStat].Visible = _bVisible;
            SetPopupMenuStat();
            OnChange();
        }

       

        //PopupMenu
        public void SetPopupMenuEnable(bool _bState)
        {
            for (int i = 0; i < (int)EN_STATUS_POPUP_ID.MAX_STATUS_POPUP_ID; i++)
            {
                cmPopupMenus[i].Enabled = _bState;
            }
            SetPopupMenuStat();
        }

        private void OneClick(object sender, System.EventArgs e)
        {
            //Local Var.
            int Tag = (int)((ToolStripMenuItem)sender).Tag;

            String sTemp;
            sTemp = "One(C=" + ClickStat.iSelR2.ToString() + ",R=" + ClickStat.iSelC2.ToString() + ") Stat Change " + (cs)GetStat(ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "One StatChange " + Name;
            Log.Trace(sHead, sTemp);


            ClickStat.iSelX1 = ClickStat.iSelX2 = ClickStat.iSelY1 = ClickStat.iSelY2 = 0;

            //Local Var.
            SetStat(ClickStat.iSelC2, ClickStat.iSelR2, (cs)Tag);

        }
        private void AllClick(object sender, System.EventArgs e)
        {
            //Local Var.
            int Tag = (int)((ToolStripMenuItem)sender).Tag;

            String sTemp;
            sTemp = "All Stat Change To " + (cs)Tag;
            String sHead = "All StatChange " + Name;
            Log.Trace(sHead, sTemp);

            ClickStat.iSelX1 = ClickStat.iSelX2 = ClickStat.iSelY1 = ClickStat.iSelY2 = 0;

            //Get RC.
            SetStat((cs)Tag);
        }
        private void RowClick(object sender, System.EventArgs e)
        {
            //Local Var.
            int Tag = (int)((ToolStripMenuItem)sender).Tag;

            String sTemp;
            sTemp = "Row(R=" + ClickStat.iSelC2.ToString() + ") Stat Change " + (cs)GetStat(ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "Row StatChange " + Name;
            Log.Trace(sHead, sTemp);

            ClickStat.iSelX1 = ClickStat.iSelX2 = ClickStat.iSelY1 = ClickStat.iSelY2 = 0;

            //Get RC.
            for (int c = 0; c < GetMaxCol(); c++)
            {
                SetStat(c, ClickStat.iSelR2, (cs)Tag);
            }

        }
        private void ColClick(object sender, System.EventArgs e)
        {
            //Local Var.
            int Tag = (int)((ToolStripMenuItem)sender).Tag;

            String sTemp;
            sTemp = "Col(C=" + ClickStat.iSelC2.ToString() + ") Stat Change " + (cs)GetStat(ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "Col StatChange " + Name;
            Log.Trace(sHead, sTemp);

            ClickStat.iSelX1 = ClickStat.iSelX2 = ClickStat.iSelY1 = ClickStat.iSelY2 = 0;

            for (int r = 0; r < GetMaxRow(); r++)
            {
                SetStat(ClickStat.iSelC2, r, (cs)Tag);
            }
        
        }
        private void DragClick(object sender, System.EventArgs e)
        {
            //Local Var.
            int Tag = (int)((ToolStripMenuItem)sender).Tag;

            String sTemp;
            sTemp = "Drag(C1=" + ClickStat.iSelC1.ToString() + ",R1=" + ClickStat.iSelR1.ToString() + " C2=" + ClickStat.iSelC2.ToString() + ",R2=" + ClickStat.iSelR2.ToString() + "Stat Change " + (cs)GetStat(ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "Drag StatChange " + Name;
            Log.Trace(sHead, sTemp);

            int r1, r2, c1, c2;
            if (ClickStat.iSelR1 < ClickStat.iSelR2) { r1 = ClickStat.iSelR1; r2 = ClickStat.iSelR2; }
            else                                     { r1 = ClickStat.iSelR2; r2 = ClickStat.iSelR1; }
            if (ClickStat.iSelC1 < ClickStat.iSelC2) { c1 = ClickStat.iSelC1; c2 = ClickStat.iSelC2; }
            else                                     { c1 = ClickStat.iSelC2; c2 = ClickStat.iSelC1; }

            ClickStat.iSelX1 = ClickStat.iSelX2 = ClickStat.iSelY1 = ClickStat.iSelY2 = 0;

            RangeSetStat(c1, r1, c2, r2, (cs)Tag);
        }

        private void Popup_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ClickStat.iSelX1 = ClickStat.iSelX2 = ClickStat.iSelY1 = ClickStat.iSelY2 = 0;
            OnChange();
        }

        public void pbArray_MouseDown(object sender, MouseEventArgs e)//TObject *Sender,
        {
            //To remove error.
            if (e.Button == MouseButtons.Left)
            {                
                ClickStat.bMouseDown = true;

                ClickStat.iSelX1 = e.X;
                ClickStat.iSelY1 = e.Y;
                ClickStat.iSelX2 = e.X;
                ClickStat.iSelY2 = e.Y;

                OnChange();

                int C=0 , R=0 ;
                if (GetChipCR(ClickStat.iSelX1, ClickStat.iSelY1, ref C, ref R))
                {
                    ClickStat.iSelC1 = C;
                    ClickStat.iSelR1 = R;

                    ClickStat.iSelC2 = C;
                    ClickStat.iSelR2 = R;
                }               
            }
        }
        public void pbArray_MouseMove(object sender, MouseEventArgs e) //TObject *Sender,
        {
            if (!ClickStat.bMouseDown) return;
            ClickStat.iSelX2 = e.X;
            ClickStat.iSelY2 = e.Y;

            int C=0, R=0;
            if (GetChipCR(ClickStat.iSelX2, ClickStat.iSelY2, ref C, ref R))
            {
                ClickStat.iSelC2 = C;
                ClickStat.iSelR2 = R;
            }

            OnChange();

        }
        
        public void pbArray_MouseUp(object sender, MouseEventArgs e) //TObject *Sender,
        {
            if (!ClickStat.bMouseDown) return;
            ClickStat.bMouseDown = false;

            lpPoint.X = e.X;
            lpPoint.Y = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (ClickStat.iSelX1 == ClickStat.iSelX2 && ClickStat.iSelY1 == ClickStat.iSelY2)
                {
                  

                    if (Control.ModifierKeys == (Keys.Shift | Keys.Control))
                    {
                        pbArray.ContextMenuStrip = cmPopupMenus[(int)EN_STATUS_POPUP_ID.spAll];
                        cmPopupMenus[(int)EN_STATUS_POPUP_ID.spAll].Show(pbArray, e.X, e.Y);
                    }

                    else if (Control.ModifierKeys == Keys.Control)
                    {
                        pbArray.ContextMenuStrip = cmPopupMenus[(int)EN_STATUS_POPUP_ID.spCol];
                        cmPopupMenus[(int)EN_STATUS_POPUP_ID.spCol].Show(pbArray, e.X, e.Y);
                    }

                    else if (Control.ModifierKeys == Keys.Shift)
                    {
                        pbArray.ContextMenuStrip = cmPopupMenus[(int)EN_STATUS_POPUP_ID.spRow];
                        cmPopupMenus[(int)EN_STATUS_POPUP_ID.spRow].Show(pbArray, e.X, e.Y);
                    }
                    else
                    {
                        pbArray.ContextMenuStrip = cmPopupMenus[(int)EN_STATUS_POPUP_ID.spOne];
                        cmPopupMenus[(int)EN_STATUS_POPUP_ID.spOne].Show(pbArray, e.X, e.Y);
                    }
                }

                else
                {
                    pbArray.ContextMenuStrip = cmPopupMenus[(int)EN_STATUS_POPUP_ID.spDrag];
                    cmPopupMenus[(int)EN_STATUS_POPUP_ID.spDrag].Show(pbArray, e.X, e.Y);
                }
                
            
            }
            else if (e.Button == MouseButtons.Right && Control.ModifierKeys == Keys.Control)
            {

            }

        }
      
        private void GetChipCoord(int _iCol , int _iRow , ref int _iX , ref int _iY , ref int _iW , ref int _iH)
        {
            //셀간 간격 
            const int iCellMargin = 1;

            //그림그릴때 1픽셀 오른쪽으로 그리는 것 때문에 
            const int iPaintSttOft = 0;
            int iRow = GetMaxRow();
            int iCol = GetMaxCol();
                        
            //똑같은 크기로 셀을 그릴때 그릴수 있는 총넓이.
            //셀들이 작아지면 똑같이 안그리면 삐뚤빼뚤 하다.
            double dDrawWidth  = pbArray.Width  / iCol * iCol;
            double dDrawHeight = pbArray.Height / iRow * iRow;

            //셀들의 넓이 높이.
            double dCellWidth  = dDrawWidth  / (double)iCol;
            double dCellHeight = dDrawHeight / (double)iRow;

            //전체 그림을 그릴때 시작 오프셑.
            int iXSttOfs = (int)((pbArray.Width  - dDrawWidth) / 2.0);
            int iYSttOfs = (int)((pbArray.Height - dDrawHeight) / 2.0);

            _iX = iXSttOfs + iPaintSttOft + (int)(dCellWidth * _iCol) + iCellMargin;
            _iY = iYSttOfs + iPaintSttOft + (int)(dCellHeight * _iRow) + iCellMargin;

            _iW = (int)dCellWidth - iCellMargin*2 + iPaintSttOft;
            _iH = (int)dCellHeight - iCellMargin*2 + iPaintSttOft;
            
        }

        private bool GetChipCR(int _iX, int _iY, ref int _iC, ref int _iR)
        {
            //셀간 간격 
            const int iCellMargin = 1;

            //그림그릴때 1픽셀 오른쪽으로 그리는 것 때문에 
            const int iPaintSttOft = 0;
            int iRow = GetMaxRow();
            int iCol = GetMaxCol();

            //똑같은 크기로 셀을 그릴때 그릴수 있는 총넓이.
            //셀들이 작아지면 똑같이 안그리면 삐뚤빼뚤 하다.
            double dDrawWidth = pbArray.Width / iCol * iCol;
            double dDrawHeight = pbArray.Height / iRow * iRow;

            //셀들의 넓이 높이.
            double dCellWidth = dDrawWidth / (double)iCol;
            double dCellHeight = dDrawHeight / (double)iRow;

            //전체 그림을 그릴때 시작 오프셑.
            int iXSttOfs = (int)((pbArray.Width - dDrawWidth) / 2.0);
            int iYSttOfs = (int)((pbArray.Height - dDrawHeight) / 2.0);
            
            _iC = (int)((_iX - iXSttOfs - iPaintSttOft) / dCellWidth ) ;
            _iR = (int)((_iY - iYSttOfs - iPaintSttOft) / dCellHeight);

            if (_iR < 0) return false;
            if (_iC < 0) return false;

            if (_iR >= iRow) return false;
            if (_iC >= iCol) return false;

            return true;

        }


        public void PaintAray() 
        {
            //Local Var.
            //Color sColor = new Color();
            int    iRow , iCol;

            SolidBrush Brush = new SolidBrush(Color.Black);
            Pen        Pen   = new Pen(Color.Black);

            Graphics gArray = Graphics.FromImage(pbArray.Image); //pbArray.CreateGraphics();

            //Set Disp Dir.
            iRow = GetMaxRow();
            iCol = GetMaxCol();
            
            if(iRow == 0) iRow = 1 ;
            if(iCol == 0) iCol = 1 ;

            int iX = 0 ;
            int iY = 0 ;
            int iW = 0 ;
            int iH = 0 ;

           
            
            using(BufferedGraphics Buffer = BufferedGraphicsManager.Current.Allocate(gArray , pbArray.ClientRectangle))
            {
                if (CheckAllStat(cs.None))
                {                                        
                    Buffer.Graphics.Clear(StatProp[(int)cs.None].ChipColor);
                    //Buffer.Graphics.FillRectangle(bBrush, 0, 0, pbArray.Width, pbArray.Height);

                    Pen.Color = Color.Black;
                    Buffer.Graphics.DrawRectangle(Pen, 0, 0, pbArray.Width - 1, pbArray.Height - 1);
                    //View Chip Info.
                }
                else
                {
                    Buffer.Graphics.Clear(Color.White);

                    Pen.Color = Color.Black;
                    Buffer.Graphics.DrawRectangle(Pen, 0, 0, pbArray.Width - 1, pbArray.Height - 1);

                    cs eStat;

                    for (int r = 0; r < iRow; r++)
                    {
                        for (int c = 0; c < iCol; c++)
                        {
                            GetChipCoord(c, r, ref iX, ref iY, ref iH, ref iW);
                            eStat = GetStat(c, r);
                            Brush.Color = StatProp[(int)eStat].ChipColor;
                            Buffer.Graphics.FillRectangle(Brush, iX, iY, iH, iW);
                            Buffer.Graphics.DrawRectangle(Pen, iX, iY, iH, iW);
                        }
                    }
                }

                if (ClickStat.iSelX1 != 0 || ClickStat.iSelX2 != 0 || ClickStat.iSelY1 != 0 || ClickStat.iSelY2 != 0)
                {
                    Pen.Color = Color.Black;
                    Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                    int iLeft   = ClickStat.iSelX2 < ClickStat.iSelX1 ? ClickStat.iSelX2 : ClickStat.iSelX1;
                    int iTop    = ClickStat.iSelY2 < ClickStat.iSelY1 ? ClickStat.iSelY2 : ClickStat.iSelY1;
                    int iRight  = ClickStat.iSelX2 < ClickStat.iSelX1 ? ClickStat.iSelX1 : ClickStat.iSelX2;
                    int iBottom = ClickStat.iSelY2 < ClickStat.iSelY1 ? ClickStat.iSelY1 : ClickStat.iSelY2;

                    int iWidth = iRight - iLeft;
                    int iHeight = iBottom - iTop;
                    Buffer.Graphics.DrawRectangle(Pen, iLeft, iTop, iWidth, iHeight);
                }
                Buffer.Render(gArray);
            }

            Brush.Dispose();
            Pen.Dispose();
            gArray.Dispose();
            
        }

        private void ItemPaint(object sender, PaintEventArgs e)
        {
            int iStat = (int)((ToolStripMenuItem)sender).Tag ;
            SolidBrush Brush = new SolidBrush(StatProp[iStat].ChipColor) ;
            Pen        pen   = new Pen       (Color.Black);
            
            e.Graphics.FillRectangle(Brush , 5,2,18,18);
            e.Graphics.DrawRectangle(pen   , 5,2,18,18);

            Brush.Dispose();
            pen  .Dispose();

        }

        delegate void PaintArayCallback();
        public void UpdateAray() //일단 외부 억세스 막고.  이벤트 방식에서 다시 바꿈.
        {
            if (pbArray.InvokeRequired) // Invoke가 필요하면
            {
                pbArray.Invoke(new PaintArayCallback(PaintAray), new object[] { }); // 대리자를 호출
            }
            else
            {
                PaintAray();
            }
        }

        private void pbArray_Paint(object sender, PaintEventArgs e)
        {
            

        }

        protected override void OnChange(){
            UpdateAray();

            //cross Thread
            //PaintAray();

            //Sun.
            //pbArray.Refresh();
        }
    
    }   
}

