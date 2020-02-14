using System;
using System.Windows.Forms;
using COMMON;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Machine
{
    public struct TChip
    {
        public cs Stat;
        public void Clear()
        {
            Stat = cs.None;
        }
    }

    public class CArrayData
    {
        public CArrayData()
        {
            
        }
        
        private TChip[,]    m_tChips   = new TChip[1, 1];
        private bool[,]     m_bNotUse  = new bool [1, 1]; //안쓰는 셀들이 있다. 트레이가 이렇게 생긴것들이 있음.. 오스람.
        private bool[,]     m_bWorkChip= new bool [1, 1];
        private String      m_sName    = ""   ; //어레이의 이름 ex)PreBufferZone
        private String      m_sLotNo   = ""   ; //장비에 따라 랏 넘버를 붙여 줄수 있다
        private String      m_sID      = ""   ;//스트립의 메가진 No 슬롯 No ;  m_sID/100 = 메가진 카운트 , m_sID%100 = 슬롯 카운터.
        private int         m_iStep    = 0    ; //어레이에서 어떤 작업을 함에 있어서 스텝. 데이터를 건드리지 않고 작업의 단계를 나타낼때 쓴다.
        private int         m_iSubStep = 0    ; //어레이에서 어떤 작업을 함에 있어서 스텝. 데이터를 건드리지 않고 작업의 단계를 나타낼때 쓴다.(Sub Step)
        private int         m_iMaxCol  = 1    ;
        private int         m_iMaxRow  = 1    ;
        

        public TChip[,]    Chip       { get { return m_tChips;     } set { m_tChips      = value; } }
        public bool[,]     NotUse     { get { return m_bNotUse;    } set { m_bNotUse     = value; } }
        public bool[,]     WorkChip   { get { return m_bWorkChip;  } set { m_bWorkChip   = value; } }
        public String      Name       { get { return m_sName;      } set { m_sName       = value; } }
        public String      LotNo      { get { return m_sLotNo;     } set { m_sLotNo      = value; } }
        public String      ID         { get { return m_sID;        } set { m_sID         = value; } }
        public int         Step       { get { return m_iStep;      } set { m_iStep       = value; } }
        public int         SubStep    { get { return m_iSubStep;   } set { m_iSubStep    = value; } }
        public int         MaxCol     { get { return m_iMaxCol;    } set { SetMaxColRow(value, m_iMaxRow); } }
        public int         MaxRow     { get { return m_iMaxRow;    } set { SetMaxColRow(m_iMaxCol, value); } }
        

        virtual protected void OnChange()
        {

        }

        public void SetMask(CArrayData _tMask)
        {
            if(m_iMaxCol != _tMask.m_iMaxCol) return ;
            if(m_iMaxRow != _tMask.m_iMaxRow) return ;

            for(int r = 0 ; r < m_iMaxRow ; r++){
                for(int c = 0 ; c < m_iMaxCol ; c++) {
                    if(_tMask.Chip[c,r].Stat == cs.None) {
                        m_bNotUse[c,r]=true ;
                        Chip[c,r].Stat = cs.None ;
                    }
                    else {
                        m_bNotUse[c,r]=false ;
                    }
                }
            }
            OnChange();
        }

        public void ClearMask()
        {
            for(int r = 0 ; r < m_iMaxRow ; r++){
                for(int c = 0 ; c < m_iMaxCol ; c++) {
                    m_bNotUse[c,r]=false ;
                }
            }
            OnChange();
        }
        public void SetWorkChip(int c, int r)
        {
            ClearWorkChip();

            m_bWorkChip[c,r] = true;
            OnChange();
        }

        public void ClearWorkChip()
        {
            for (int Row = 0; Row < m_iMaxRow; Row++)
            {
                for (int Col = 0; Col < m_iMaxCol; Col++)
                {
                    m_bWorkChip[Col, Row] = false;
                }
            }
            OnChange();
        }


        public void ClearMap()
        {
            for(int r = 0; r < m_iMaxRow; r++){
                for(int c = 0; c < m_iMaxCol; c++){
                    m_tChips[c, r].Clear();
                    m_bWorkChip[c,r] = false;
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
            bool [,] mask = new bool [_iC, _iR];
            bool [,] Work = new bool [_iC, _iR];
            int iCol = Math.Min(_iC, m_tChips.GetLength(0));
            int iRow = Math.Min(_iR, m_tChips.GetLength(1));
            
            for(int r = 0; r < _iR; r++){
                for(int c = 0; c < _iC; c++){
                    chps[c,r] = new TChip();
                }
            }
              

            for (int r = 0; r < iRow; r++)
                for (int c = 0; c < iCol; c++){
                    chps[c, r] = m_tChips [c, r];
                    mask[c, r] = m_bNotUse[c, r];
                    Work[c, r] = m_bWorkChip[c,r];
                    
                }

            m_tChips  = chps ; 
            m_bNotUse = mask ;
            m_bWorkChip = Work;
            OnChange();
        }

        //Search Chip.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public bool FindChip(cs _eStat, int _iC, int _iR)
        //{
        //    bool bRet = CheckStat(_iC , _iR, _eStat);
        //    return bRet ;
        //}

        #region FindOri
        public int FindFrstRowOri(cs _eStat)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstRowColOri(_eStat, ref iColNum, ref iRowNum);
            return iRowNum;
        }

        public int FindFrstColOri(cs _eStat)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstColRowOri(_eStat, ref iColNum, ref iRowNum);
            return iColNum;
        }
        public int FindLastRowOri(cs _eStat)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastRowColOri(_eStat, ref iColNum, ref iRowNum);
            return iRowNum;
        }
        public int FindLastColOri(cs _eStat)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastColRowOri(_eStat, ref iColNum, ref iRowNum);            
            //return Find Row No.
            return iColNum;

        }
        public bool FindFrstRowColOri(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = 0 ; c < m_iMaxCol ; c++) {
                    if (CheckStat(c , r, _eStat)) {//
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
        public bool FindFrstColRowOri(cs _eStat, ref int _iC, ref int _iR)
        {
            
            //Local Var.
            for (int c = 0 ; c < m_iMaxCol ; c++) {
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindLastRowColOri(cs _eStat, ref int _iC, ref int _iR)
        {
            for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                for (int c = m_iMaxCol - 1 ; c >= 0 ; c--) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindFrstRowLastColOri(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = m_iMaxCol - 1 ; c >= 0 ; c--) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindLastRowFrstColOri(cs _eStat, ref int _iC, ref int _iR)
        {
            for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                for (int c = 0 ; c < m_iMaxCol ; c++) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindLastColFrstRowOri(cs _eStat, ref int _iC, ref int _iR)
        {
            for (int c = m_iMaxCol - 1 ; c >= 0 ; c--) {
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindFrstColLastRowOri(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int c = 0 ; c < m_iMaxCol ; c++) {
                for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindLastColRowOri(cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int c = m_iMaxCol-1 ; c >= 0 ; c--) {
                for (int r = m_iMaxRow-1 ; r >= 0 ; r--) {
                    if (CheckStat(c , r, _eStat)) {
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

        public bool FindFrstRowCol_IndxOri(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0        ) { _iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) { _iR = -1; _iC = -1; return false ;}
            
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = iStrCol ; c <= iEndCol ; c++) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindFrstRowLastCol_IndxOri(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            
            for (int r = 0 ; r < m_iMaxRow ; r++) {
                for (int c = iEndCol ; c >= iStrCol ; c--) {
                    if (CheckStat(c , r, _eStat)) {
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

        public bool FindFrstColRow_IndxOri(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            
            for (int c = iStrCol ; c <= iEndCol ; c++) {
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    if (CheckStat(c , r, _eStat)) {
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
        public bool FindFrstColLastRow_IndxOri(cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= m_iMaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0        ) {_iR = -1; _iC = -1; return false ;}
            
            //Local Var.
            for (int c = iStrCol ; c <= iEndCol ; c++) {
                for (int r = m_iMaxRow - 1 ; r >= 0 ; r--) {
                    if (CheckStat(c , r, _eStat)) {
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
        #endregion

        #region FindNew
        public int FindFrstRow(params cs[] _csStats)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstRowCol(ref iColNum, ref iRowNum, _csStats);
            return iRowNum;
        }

        public int FindFrstCol(params cs[] _csStats)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstColRow(ref iColNum, ref iRowNum, _csStats);
            return iColNum;
        }
        public int FindLastRow(params cs[] _csStats)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastRowCol(ref iColNum, ref iRowNum, _csStats);
            return iRowNum;
        }
        public int FindLastCol(params cs[] _csStats)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastColRow(ref iColNum, ref iRowNum, _csStats);
            //return Find Row No.
            return iColNum;

        }
        public bool FindFrstRowCol(ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.
            for (int r = 0; r < m_iMaxRow; r++)
            {
                for (int c = 0; c < m_iMaxCol; c++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iC = c;
                            _iR = r;
                            return true;
                        }
                    }
                }
            }
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstColRow(ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.
            for (int c = 0; c < m_iMaxCol; c++)
            {
                for (int r = 0; r < m_iMaxRow; r++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;

                        }
                    }
                }
            }
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;

        }
        public bool FindLastRowCol(ref int _iC, ref int _iR, params cs[] _csStats)
        {
            for (int r = m_iMaxRow - 1; r >= 0; r--)
            {
                for (int c = m_iMaxCol - 1; c >= 0; c--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }
            }

            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstRowLastCol(ref int _iC, ref int _iR, params cs[] _csStats)
        {   
            for (int r = 0; r < m_iMaxRow; r++)
            {
                for (int c = m_iMaxCol - 1; c >= 0; c--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }
            }

            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindLastRowFrstCol(ref int _iC, ref int _iR, params cs[] _csStats)
        {
            for (int r = m_iMaxRow - 1; r >= 0; r--)
            {
                for (int c = 0; c < m_iMaxCol; c++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }

            }


            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindLastColFrstRow(ref int _iC, ref int _iR, params cs[] _csStats)
        {
            for (int c = m_iMaxCol - 1; c >= 0; c--)
            {
                for (int r = 0; r < m_iMaxRow; r++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iC = c;
                            _iR = r;
                            return true;
                        }
                    }
                }

            }


            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstColLastRow(ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.


            for (int c = 0; c < m_iMaxCol; c++)
            {
                for (int r = m_iMaxRow - 1; r >= 0; r--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }

            }

            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindLastColRow(ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.


            for (int c = m_iMaxCol - 1; c >= 0; c--)
            {
                for (int r = m_iMaxRow - 1; r >= 0; r--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }

            }

            //No Find.
            _iR = -1;
            _iC = -1;
            return false;

        }

        public bool FindFrstRowCol_Indx(int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }


            for (int r = 0; r < m_iMaxRow; r++)
            {
                for (int c = iStrCol; c <= iEndCol; c++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }
            }
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstRowLastCol_Indx(int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }


            for (int r = 0; r < m_iMaxRow; r++)
            {
                for (int c = iEndCol; c >= iStrCol; c--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }

            }
            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }

        public bool FindFrstColRow_Indx(int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }

            for (int c = iStrCol; c <= iEndCol; c++)
            {
                for (int r = 0; r < m_iMaxRow; r++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }
            }

            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        public bool FindFrstColLastRow_Indx(int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= m_iMaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }
            if (iEndCol < 0) { _iR = -1; _iC = -1; return false; }

            //Local Var.
            for (int c = iStrCol; c <= iEndCol; c++)
            {
                for (int r = m_iMaxRow - 1; r >= 0; r--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(c, r, _csStats[s]))
                        {
                            _iR = r;
                            _iC = c;
                            return true;
                        }
                    }
                }

            }

            //No Find.
            _iR = -1;
            _iC = -1;
            return false;
        }
        #endregion
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
                        Config.GetValue(sArayName, sIndex, out sRowData);
                        for (int c = 0; c < m_iMaxCol; c++)
                        {
                            sTemp = sRowData.Substring(c * 3, 3);
                            iStat = int.Parse(sTemp);
                            m_tChips[c, r].Stat = (cs)iStat;
                        }
                    }
                    for (int r = 0; r < m_iMaxRow; r++)
                    {
                        sIndex = string.Format("Msk{0:000}", r);
                        Config.GetValue(sArayName, sIndex, out sRowData);
                        for (int c = 0; c < m_iMaxCol; c++)
                        {
                            sTemp = sRowData.Substring(c * 3, 3);
                            iStat = int.Parse(sTemp);
                            m_bNotUse[c, r] = iStat==1;
                        }
                    }
                }
                catch(Exception e)
                {

                    Log.ShowMessage("Exception",e.Message);

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
                for (int r = 0 ; r < m_iMaxRow ; r++) {
                    sRowData = "" ;
                    sIndex = string.Format("Msk{0:000}", r);
                    for (int c = 0 ; c < m_iMaxCol ; c++) {
                        sTemp = string.Format("{0:000}", m_bNotUse[c, r] ? 1 : 0);
                        sRowData += sTemp ;
                    }
                    Config.SetValue(sArayName , sIndex ,sRowData );
                }
                
            } 
        }

        public void Trace(int iTag = 0, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sMsg;
            for (int i = 0 ; i < m_iMaxRow ; i++)
            {
                sMsg = string.Format("R{0:000}", i);
                for (int j = 0; j < m_iMaxCol; j++)
                {
                    sMsg += string.Format("_{0:00}", (int)m_tChips[j, i].Stat);
                }
                Log.Trace(sMsg, iTag);
            }
            sMsg = "Name : " + m_sName + " ID : " + m_sID + " LotNo : " + m_sLotNo + " Step : " + m_iStep + " SubStep : " + m_iSubStep;
            Log.Trace(sMsg, iTag);
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
                    if (m_tChips[c,r].Stat == _eFrom) 
                    {
                        m_tChips[c,r].Stat = _eTo;
                    }
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
            if(m_bNotUse[c,r]) return cs.None;
            return m_tChips[c,r].Stat;
        }

        public bool CheckStat(int c, int r, params cs[] _csStats) 
        {
            if (r < 0 || c < 0 || r > m_iMaxRow || c > m_iMaxCol)
            {
                return false;
            }

            if(m_bNotUse[c,r]) return false ;

            for (int s = 0; s < _csStats.Length; s++)
            {
                if (GetStat(c,r) == _csStats[s])
                {
                    return true;
                }
            }
            return false  ; 
        }

        
        public bool CheckAllStat (params cs[] _csStats) 
        {
            for (int r = 0; r < m_iMaxRow; r++)
            {
                for (int c = 0; c < m_iMaxCol; c++)
                {
                    if (!m_bNotUse[c,r] && !CheckStat(c,r, _csStats))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckAllStatCol (int c , params cs[] _csStats) 
        {
            for (int r = 0; r < m_iMaxRow; r++)
            {
                if (!m_bNotUse[c,r] && !CheckStat(c,r, _csStats))
                {
                    return false;
                }
                
            }
            return true;
        }

        public bool CheckAllStatRow (int r , params cs[] _csStats) 
        {
            for (int c = 0; c < m_iMaxCol; c++)
            {
                if (!m_bNotUse[c,r] && !CheckStat(c,r, _csStats))
                {
                    return false;
                }
            }            
            return true;
        }

        //Get Row Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int  GetCntRowStat  (int r, params cs[] _csStats) { int iCnt = 0; for (int c = 0 ; c < m_iMaxCol ; c++) if (CheckStat (c,r,_csStats)) iCnt++; return iCnt; }

        //Get Col Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int  GetCntColStat  (int c, params cs[] _csStats) { int iCnt = 0; for (int r = 0 ; r < m_iMaxRow ; r++) if (CheckStat (c,r,_csStats)) iCnt++; return iCnt; }

        //Get All Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public int  GetCntStat (cs Stat) { int iCnt = 0; for (int r = 0 ; r < m_iMaxRow ; r++) iCnt += GetCntRowStat (r,Stat); return iCnt; }
        public int GetCntStat(params cs[] _csStats)
        {
            int iCnt = 0;
            for (int i = 0; i < _csStats.Length; i++)
            {
                for (int r = 0; r < m_iMaxRow; r++) iCnt += GetCntRowStat(r, _csStats[i]);
            }
            return iCnt;
        }

        //스텟 배열에 있는 것 뺀 상태만 카운팅.
        public int GetCntStatEx(params cs[] _csStats)
        {
            int iCnt = 0;
            iCnt = GetCntStat(_csStats);
            return (m_iMaxCol * m_iMaxRow - iCnt);
        }

        public bool IsExist(params cs[] _csStats) { 
            for (int r = 0; r < m_iMaxRow; r++) { 
                if (GetCntRowStat(r, _csStats) != 0) 
                    return true; 
            } 
            return false; 
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

        private int         m_iCellMargin = 0 ; //디스플레이 및 클릭시에 마진.
        private bool        m_bSameCellSize; //셀사이즈 관련.

        public int          CellMargin   { get { return m_iCellMargin  ;} set { m_iCellMargin   = value; } }
        public bool         SameCellSize { get { return m_bSameCellSize;} set { m_bSameCellSize = value; } } //셀사이즈 관련.

        
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
            pbArray.Parent = _pnBase;
            pbArray.Width  = _pnBase.Width  -1 ;
            pbArray.Height = _pnBase.Height -1 ;
            //_pnBase.Controls.Add(pbArray);
            pbArray.Image  = new Bitmap(pbArray.Width,pbArray.Height);

            m_bSameCellSize = true;
            OnChange();
        }

        public void SetDisp(cs _eStat , String _sName , Color _cColor)
        {
            StatProp[(int)_eStat].Caption   = _sName   ;
            StatProp[(int)_eStat].Visible   = true     ;
            StatProp[(int)_eStat].ChipColor = _cColor  ;
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

        public void SetDispColor(cs _eStat, Color _cColor)
        {
            StatProp[(int)_eStat].ChipColor = _cColor;
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
                        int C=0 , R=0 ;
                        if (GetChipCR(ClickStat.iSelX1, ClickStat.iSelY1, ref C, ref R))
                        {
                            for (int s = 0; s < (int)cs.MAX_CHIP_STAT; s++)
                            {
                                cmPopupMenus[(int)EN_STATUS_POPUP_ID.spOne].Items[s].Text = C.ToString() + "_" + R.ToString() + "_" + StatProp[s].Caption;
                            }
                        }

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
            
            /*
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

            _iX = iXSttOfs + iPaintSttOft + (int)(dCellWidth  * _iCol) + m_iCellMargin;
            _iY = iYSttOfs + iPaintSttOft + (int)(dCellHeight * _iRow) + m_iCellMargin;

            _iW = (int)dCellWidth  - m_iCellMargin*2 + iPaintSttOft;
            _iH = (int)dCellHeight - m_iCellMargin*2 + iPaintSttOft;
            */
            //그림그릴때 1픽셀 오른쪽으로 그리는 것 때문에 
            const int iPaintSttOft = 0;
            int iRow = GetMaxRow();
            int iCol = GetMaxCol();
                        
            //똑같은 크기로 셀을 그릴때 그릴수 있는 총넓이.
            //셀들이 작아지면 똑같이 안그리면 삐뚤빼뚤 하다.
            int    DrawWidth  = pbArray.Width  / iCol * iCol ;
            int    DrawHeight = pbArray.Height / iRow * iRow ;

            //셀들의 넓이 높이.
            double dCellWidth  = (double)DrawWidth  / (double)iCol;
            double dCellHeight = (double)DrawHeight / (double)iRow;

            //전체 그림을 그릴때 시작 오프셑.
            int iXSttOfs = (pbArray.Width  - DrawWidth ) / 2;
            int iYSttOfs = (pbArray.Height - DrawHeight) / 2;

            _iX = (int)iXSttOfs + iPaintSttOft + (int)(dCellWidth  * _iCol) + m_iCellMargin;
            _iY = (int)iYSttOfs + iPaintSttOft + (int)(dCellHeight * _iRow) + m_iCellMargin;

            _iW = (int)dCellWidth  - m_iCellMargin*2 + iPaintSttOft;
            _iH = (int)dCellHeight - m_iCellMargin*2 + iPaintSttOft;
            
        }

        private bool GetChipCR(int _iX, int _iY, ref int _iC, ref int _iR)
        {
            //그림그릴때 1픽셀 오른쪽으로 그리는 것 때문에 
            const int iPaintSttOft = 0;
            int iRow = GetMaxRow();
            int iCol = GetMaxCol();

            //똑같은 크기로 셀을 그릴때 그릴수 있는 총넓이.
            //셀들이 작아지면 똑같이 안그리면 삐뚤빼뚤 하다.
            //double dDrawWidth  = pbArray.Width / iCol * iCol;
            //double dDrawHeight = pbArray.Height / iRow * iRow;
            int    DrawWidth  = pbArray.Width  / iCol * iCol;
            int    DrawHeight = pbArray.Height / iRow * iRow;

            //셀들의 넓이 높이.
            double dCellWidth  = (double)DrawWidth  / (double)iCol;
            double dCellHeight = (double)DrawHeight / (double)iRow;

            //전체 그림을 그릴때 시작 오프셑.
            int iXSttOfs = (pbArray.Width  - DrawWidth ) / 2;
            int iYSttOfs = (pbArray.Height - DrawHeight) / 2;
            
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
            if(pbArray == null)return  ;
            if(pbArray.Image == null)return  ; //Need to SetParent , SetParent할때 이미지 생성함
            

            int    iRow , iCol;

            SolidBrush Brush = new SolidBrush(Color.Black);
            Pen        Pen   = new Pen(Color.Black);

            SolidBrush WorkBrush = new SolidBrush(Color.Red);

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
                            GetChipCoord(c, r, ref iX, ref iY, ref iH, ref iW);//H W가 바뀐듯 나중에 시간 날때 확인 하자.
                            eStat = GetStat(c, r);
                            Brush.Color = StatProp[(int)eStat].ChipColor;
                            Buffer.Graphics.FillRectangle(Brush, iX, iY, iH, iW);
                            Buffer.Graphics.DrawRectangle(Pen, iX, iY, iH, iW);
                            if(NotUse[c,r]){
                                Buffer.Graphics.DrawLine(Pen , iX , iY , iX + iH , iY + iW); 
                                Buffer.Graphics.DrawLine(Pen , iX + iH , iY , iX , iY + iW); 
                            }

                            //int iRad = iH < iW ? iH/2 : iW/2 ;
                            //int iCntX = iX + iH/2.0 ;
                            //int iCntY = iY + iW/2.0 ;
                            if (WorkChip[c, r]){
                                Buffer.Graphics.FillEllipse(WorkBrush, iX, iY, iH, iW);
                                //Buffer.Graphics.FillEllipse((WorkBrush, iX, iY, iH, iW);
                            }
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
            Pen  .Dispose();
            WorkBrush.Dispose();

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
        public string sTemp ;
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
            
            //Too Much time takes in SetStat 
            //
            //if(pbArray.Parent != null) UpdateAray();

            //cross Thread
            //PaintAray();

            //Sun.
            //pbArray.Refresh();
        }
    
    }   
}

