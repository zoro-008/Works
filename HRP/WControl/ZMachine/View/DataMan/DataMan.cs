using COMMON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Machine.View.DataMan
{
    //--------------------------------------------------------------------------------
    //DataMan
    public struct TChip
    {
        public cs Stat;
        public void Clear()
        {
            Stat = cs.None;
        }
    }

    public static class DM
    {
        public static List<ArrayData> ARAY;

        private static int    iMaxRow  = 1 ;
        private static int    iMaxCol  = 1 ;
        private static string sID      = "";
        private static string sLotNo   = "";
        private static int    iStep    = 0 ;
        private static int    iSubStep = 0 ;

        public static void Init()
        {
            ARAY   = new List<ArrayData>();
          
            for(int i = 0; i < (int)ri.MAX_ARAY; i++)
            {
                ARAY.Add(new ArrayData() {
                    MaxRow  = iMaxRow  ,
                    MaxCol  = iMaxCol  ,
                    ID      = sID      ,
                    LotNo   = sLotNo   ,
                    Step    = iStep    ,
                    SubStep = iSubStep
                }); 
            }
        }

        public static void ShiftData(int _iFrom, int _iTo)
        {
            SetMaxColRow(_iTo, ARAY[_iFrom].MaxCol, ARAY[_iFrom].MaxRow);

            ARAY[_iTo].ID      = ARAY[_iFrom].ID;
            ARAY[_iTo].LotNo   = ARAY[_iFrom].LotNo;
            ARAY[_iTo].Step    = ARAY[_iFrom].Step;
            ARAY[_iTo].SubStep = ARAY[_iFrom].SubStep;

            for (int r = 0; r < ARAY[_iFrom].MaxRow; r++)
            {
                for (int c = 0; c < ARAY[_iFrom].MaxCol; c++)
                {
                    SetStat(_iTo, c, r, GetStat(_iFrom, c, r));
                }
            }
            ClearMap(_iFrom);
        }
        public static void CopyData(int _iFrom, int _iTo)
        {
            SetMaxColRow(_iTo, ARAY[_iFrom].MaxCol, ARAY[_iFrom].MaxRow);

            ARAY[_iTo].ID      = ARAY[_iFrom].ID     ;
            ARAY[_iTo].LotNo   = ARAY[_iFrom].LotNo  ;
            ARAY[_iTo].Step    = ARAY[_iFrom].Step   ;
            ARAY[_iTo].SubStep = ARAY[_iFrom].SubStep;

            for (int r = 0; r < ARAY[_iFrom].MaxRow; r++)
            {
                for (int c = 0; c < ARAY[_iFrom].MaxCol; c++)
                {
                    SetStat(_iTo, c, r, GetStat(_iFrom, c, r));
                }
            }
        }

        public static void SetMask(int _iId, ArrayData _tMask)
        {
            if(ARAY[_iId].MaxCol != _tMask.MaxCol) return ;
            if(ARAY[_iId].MaxRow != _tMask.MaxRow) return ;

            for(int r = 0 ; r < ARAY[_iId].MaxRow ; r++){
                for(int c = 0 ; c < ARAY[_iId].MaxCol ; c++) {
                    if(_tMask.Chip[c,r].Stat == cs.None) {
                        ARAY[_iId].NotUse[c,r]=true ;
                        ARAY[_iId].Chip[c,r].Stat = cs.None ;
                    }
                    else {
                        ARAY[_iId].NotUse[c,r]=false ;
                    }
                }
            }
        }

        public static void ClearMask(int _iId)
        {
            for(int r = 0 ; r < ARAY[_iId].MaxRow ; r++){
                for(int c = 0 ; c < ARAY[_iId].MaxCol ; c++) {
                    ARAY[_iId].NotUse[c,r]=false ;
                }
            }
        }
        public static void SetWorkChip(int _iId, int c, int r)
        {
            ClearWorkChip(_iId);

            ARAY[_iId].WorkChip[c,r] = true;
        }

        public static void ClearWorkChip(int _iId)
        {
            for (int Row = 0; Row < ARAY[_iId].MaxRow; Row++)
            {
                for (int Col = 0; Col < ARAY[_iId].MaxCol; Col++)
                {
                    ARAY[_iId].WorkChip[Col, Row] = false;
                }
            }
        }

        public static void ClearMap(int _iId)
        {
            for(int r = 0; r < ARAY[_iId].MaxRow; r++){
                for(int c = 0; c < ARAY[_iId].MaxCol; c++){
                    ARAY[_iId].Chip[c, r].Clear();
                    ARAY[_iId].WorkChip[c,r] = false;
                }
            }

            ARAY[_iId].LotNo   = "";
            ARAY[_iId].ID      = "";
            ARAY[_iId].Step    = 0 ;
            ARAY[_iId].SubStep = 0 ;

            ARAY[_iId].Data = new ArrayData.TData();
        }
       
        public static void FlipX(int _iId)
        {
            TChip[,] chps = new TChip[ARAY[_iId].MaxCol , ARAY[_iId].MaxRow];
            
            for (int i = 0; i < ARAY[_iId].MaxRow; i++)
            {
                for (int j = 0; j < ARAY[_iId].MaxCol; j++)
                {
                    chps[j, i] = ARAY[_iId].Chip[ARAY[_iId].MaxCol - 1 - j , i];
                }
            }
            Array.Copy(chps, 0, ARAY[_iId].Chip, 0, ARAY[_iId].Chip.Length);
        }
        public static void FlipY(int _iId)
        {
            TChip[,] chps = new TChip[ARAY[_iId].MaxCol, ARAY[_iId].MaxRow];

            for (int i = 0; i < ARAY[_iId].MaxRow; i++)
            {
                for (int j = 0; j < ARAY[_iId].MaxCol; j++)
                {
                    chps[j, i] = ARAY[_iId].Chip[j, ARAY[_iId].MaxRow - 1 - i];
                }
            }
            Array.Copy(chps, 0, ARAY[_iId].Chip, 0, ARAY[_iId].Chip.Length);
        }
        public static void FlipXY(int _iId)
        {

            TChip[,] chps = new TChip[ARAY[_iId].MaxCol, ARAY[_iId].MaxRow];

            for (int i = 0; i < ARAY[_iId].MaxRow; i++)
            {
                for (int j = 0; j < ARAY[_iId].MaxCol; j++)
                {
                    chps[j, i] = ARAY[_iId].Chip[ARAY[_iId].MaxCol - 1 - j, ARAY[_iId].MaxRow - 1 - i];
                }
            }
            Array.Copy(chps, 0, ARAY[_iId].Chip, 0, ARAY[_iId].Chip.Length);
        }
        public static void TurnCw90(int _iId)  //시계 방향.
        {
            //버퍼 생성.
            TChip[,] chps = new TChip[ARAY[_iId].MaxRow, ARAY[_iId].MaxCol];

            //데이터 버퍼로 옮김.
            for (int i = 0; i < ARAY[_iId].MaxCol; i++)
            {
                for (int j = 0; j < ARAY[_iId].MaxRow; j++)
                {
                    chps[j,i] = ARAY[_iId].Chip[i, ARAY[_iId].MaxRow - 1 - j];
                }
            }

            //맥스 세팅.
            SetMaxColRow(_iId, ARAY[_iId].MaxRow, ARAY[_iId].MaxCol);

            Array.Copy(chps, 0, ARAY[_iId].Chip, 0, ARAY[_iId].Chip.Length);
        }
        public static void TurnCw180(int _iId)
        {
            TChip[,] chps = new TChip[ARAY[_iId].MaxCol, ARAY[_iId].MaxRow];

            for (int i = 0 ; i < ARAY[_iId].MaxRow ; i++) {
                for (int j = 0 ; j < ARAY[_iId].MaxCol ; j++) {
                    chps[j, i] = ARAY[_iId].Chip[ARAY[_iId].MaxCol - 1 - j, ARAY[_iId].MaxRow - 1 - i];
                }
            }

            Array.Copy(chps, 0, ARAY[_iId].Chip, 0, ARAY[_iId].Chip.Length);
        }
        public static void TurnCw270(int _iId)
        {
            TChip[,] chps = new TChip[ARAY[_iId].MaxCol, ARAY[_iId].MaxRow];

            for (int i = 0 ; i < ARAY[_iId].MaxCol ; i++) {
                for (int j = 0 ; j < ARAY[_iId].MaxRow ; j++) {
                                   //r,c
                    chps[j,i] = ARAY[_iId].Chip[ARAY[_iId].MaxCol - 1 - i, j];
                }
            }
            
            SetMaxColRow(_iId, ARAY[_iId].MaxCol, ARAY[_iId].MaxRow);
            Array.Copy(chps, 0, ARAY[_iId].Chip, 0, ARAY[_iId].Chip.Length);
        }
        public static void SetMaxColRow (int _iId, int _iC,int _iR)
        {
            if(ARAY[_iId].MaxCol == _iC && ARAY[_iId].MaxRow == _iR) return ;

            ARAY[_iId].MaxCol = _iC ;
            ARAY[_iId].MaxRow = _iR;
            TChip[,] chps = new TChip[_iC, _iR];
            bool [,] mask = new bool [_iC, _iR];
            bool [,] Work = new bool [_iC, _iR];
            int iCol = Math.Min(_iC, ARAY[_iId].Chip.GetLength(0));
            int iRow = Math.Min(_iR, ARAY[_iId].Chip.GetLength(1));
            
            for(int r = 0; r < _iR; r++){
                for(int c = 0; c < _iC; c++){
                    chps[c,r] = new TChip();
                }
            }

            for (int r = 0; r < iRow; r++)
                for (int c = 0; c < iCol; c++){
                    chps[c, r] = ARAY[_iId].Chip  [c, r];
                    mask[c, r] = ARAY[_iId].NotUse[c, r];
                    Work[c, r] = ARAY[_iId].WorkChip[c,r];
                    
                }

            ARAY[_iId].Chip     = chps ; 
            ARAY[_iId].NotUse   = mask ;
            ARAY[_iId].WorkChip = Work ;

            ARAY[_iId].Resize(); //그리는 쪽에서 그리드 구성 새로 함
        }

        //Search Chip.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public bool FindChip(cs _eStat, int _iC, int _iR)
        //{
        //    bool bRet = CheckStat(_iC , _iR, _eStat);
        //    return bRet ;
        //}

        #region FindOri
        public static int FindFrstRowOri(int _iId, cs _eStat)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstRowColOri(_iId, _eStat, ref iColNum, ref iRowNum);
            return iRowNum;
        }

        public static int FindFrstColOri(int _iId, cs _eStat)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstColRowOri(_iId, _eStat, ref iColNum, ref iRowNum);
            return iColNum;
        }
        public static int FindLastRowOri(int _iId, cs _eStat)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastRowColOri(_iId, _eStat, ref iColNum, ref iRowNum);
            return iRowNum;
        }
        public static int FindLastColOri(int _iId, cs _eStat)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastColRowOri(_iId, _eStat, ref iColNum, ref iRowNum);            
            //return Find Row No.
            return iColNum;

        }
        public static bool FindFrstRowColOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) {
                for (int c = 0 ; c < ARAY[_iId].MaxCol ; c++) {
                    if (CheckStat(_iId, c , r, _eStat)) {//
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
        public static bool FindFrstColRowOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int c = 0 ; c < ARAY[_iId].MaxCol ; c++) {
                for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindLastRowColOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            for (int r = ARAY[_iId].MaxRow - 1 ; r >= 0 ; r--) {
                for (int c = ARAY[_iId].MaxCol - 1 ; c >= 0 ; c--) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindFrstRowLastColOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) {
                for (int c = ARAY[_iId].MaxCol - 1 ; c >= 0 ; c--) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindLastRowFrstColOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            for (int r = ARAY[_iId].MaxRow - 1 ; r >= 0 ; r--) {
                for (int c = 0 ; c < ARAY[_iId].MaxCol ; c++) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindLastColFrstRowOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            for (int c = ARAY[_iId].MaxCol - 1 ; c >= 0 ; c--) {
                for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindFrstColLastRowOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int c = 0 ; c < ARAY[_iId].MaxCol ; c++) {
                for (int r = ARAY[_iId].MaxRow - 1 ; r >= 0 ; r--) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindLastColRowOri(int _iId, cs _eStat, ref int _iC, ref int _iR)
        {
            //Local Var.
            for (int c = ARAY[_iId].MaxCol -1 ; c >= 0 ; c--) {
                for (int r = ARAY[_iId].MaxRow -1 ; r >= 0 ; r--) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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

        public static bool FindFrstRowCol_IndxOri(int _iId, cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false ;}
            
            for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) {
                for (int c = iStrCol ; c <= iEndCol ; c++) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindFrstRowLastCol_IndxOri(int _iId, cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= ARAY[_iId].MaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0                ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0                ) {_iR = -1; _iC = -1; return false ;}
            
            for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) {
                for (int c = iEndCol ; c >= iStrCol ; c--) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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

        public static bool FindFrstColRow_IndxOri(int _iId, cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= ARAY[_iId].MaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0                ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0                ) {_iR = -1; _iC = -1; return false ;}
            
            for (int c = iStrCol ; c <= iEndCol ; c++) {
                for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static bool FindFrstColLastRow_IndxOri(int _iId, cs _eStat, int iStrCol, int iEndCol, ref int _iC, ref int _iR)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iStrCol >= ARAY[_iId].MaxCol) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0                ) {_iR = -1; _iC = -1; return false ;}
            if (iEndCol <  0                ) {_iR = -1; _iC = -1; return false ;}
            
            //Local Var.
            for (int c = iStrCol ; c <= iEndCol ; c++) {
                for (int r = ARAY[_iId].MaxRow - 1 ; r >= 0 ; r--) {
                    if (CheckStat(_iId, c , r, _eStat)) {
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
        public static int FindFrstRow(int _iId, params cs[] _csStats)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstRowCol(_iId, ref iColNum, ref iRowNum, _csStats);
            return iRowNum;
        }

        public static int FindFrstCol(int _iId, params cs[] _csStats)
        {
            int iRowNum = -1;
            int iColNum = -1;
            FindFrstColRow(_iId, ref iColNum, ref iRowNum, _csStats);
            return iColNum;
        }
        public static int FindLastRow(int _iId, params cs[] _csStats)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastRowCol(_iId, ref iColNum, ref iRowNum, _csStats);
            return iRowNum;
        }
        public static int FindLastCol(int _iId, params cs[] _csStats)
        {
            //Local Var.
            int iRowNum = -1;
            int iColNum = -1;
            FindLastColRow(_iId, ref iColNum, ref iRowNum, _csStats);
            //return Find Row No.
            return iColNum;
        }
        public static bool FindFrstRowCol(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.
            for (int r = 0; r < ARAY[_iId].MaxRow; r++)
            {
                for (int c = 0; c < ARAY[_iId].MaxCol; c++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindFrstColRow(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.
            for (int c = 0; c < ARAY[_iId].MaxCol; c++)
            {
                for (int r = 0; r < ARAY[_iId].MaxRow; r++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindLastRowCol(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            for (int r = ARAY[_iId].MaxRow - 1; r >= 0; r--)
            {
                for (int c = ARAY[_iId].MaxCol - 1; c >= 0; c--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindFrstRowLastCol(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {   
            for (int r = 0; r < ARAY[_iId].MaxRow; r++)
            {
                for (int c = ARAY[_iId].MaxCol - 1; c >= 0; c--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindLastRowFrstCol(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            for (int r = ARAY[_iId].MaxRow - 1; r >= 0; r--)
            {
                for (int c = 0; c < ARAY[_iId].MaxCol; c++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindLastColFrstRow(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            for (int c = ARAY[_iId].MaxCol - 1; c >= 0; c--)
            {
                for (int r = 0; r < ARAY[_iId].MaxRow; r++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindFrstColLastRow(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.
            for (int c = 0; c < ARAY[_iId].MaxCol; c++)
            {
                for (int r = ARAY[_iId].MaxRow - 1; r >= 0; r--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindLastColRow(int _iId, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            //Local Var.
            for (int c = ARAY[_iId].MaxCol - 1; c >= 0; c--)
            {
                for (int r = ARAY[_iId].MaxRow - 1; r >= 0; r--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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

        public static bool FindFrstRowCol_Indx(int _iId, int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }

            for (int r = 0; r < ARAY[_iId].MaxRow; r++)
            {
                for (int c = iStrCol; c <= iEndCol; c++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindFrstRowLastCol_Indx(int _iId, int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }

            for (int r = 0; r < ARAY[_iId].MaxRow; r++)
            {
                for (int c = iEndCol; c >= iStrCol; c--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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

        public static bool FindFrstColRow_Indx(int _iId, int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }

            for (int c = iStrCol; c <= iEndCol; c++)
            {
                for (int r = 0; r < ARAY[_iId].MaxRow; r++)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static bool FindFrstColLastRow_Indx(int _iId, int iStrCol, int iEndCol, ref int _iC, ref int _iR, params cs[] _csStats)
        {
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iStrCol >= ARAY[_iId].MaxCol) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }
            if (iEndCol <  0                ) { _iR = -1; _iC = -1; return false; }

            //Local Var.
            for (int c = iStrCol; c <= iEndCol; c++)
            {
                for (int r = ARAY[_iId].MaxRow - 1; r >= 0; r--)
                {
                    for (int s = 0; s < _csStats.Length; s++)
                    {
                        if (CheckStat(_iId, c, r, _csStats[s]))
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
        public static void LoadData()
        {
            string sPath = Directory.GetCurrentDirectory() + "\\SeqData\\ArrayData.INI";

            for (int i = 0; i < (int)ri.MAX_ARAY; i++)
            {
                if (ARAY[i].Name == "") return;
                string sSection = "ARRAY_" + ARAY[i].Name;
                ARAY[i].Data = new ArrayData.TData();
                CAutoIniFile.LoadStruct(sPath, sSection, ref ARAY[i].Data);
            }
        }

        public static void SaveData()
        {
            string sPath = Directory.GetCurrentDirectory() + "\\SeqData\\ArrayData.INI";

            for (int i = 0; i < (int)ri.MAX_ARAY; i++)
            {
                if (ARAY[i].Name == "") return;
                string sSection = "ARRAY_" + ARAY[i].Name;
                CAutoIniFile.SaveStruct(sPath, sSection, ref ARAY[i].Data);
            }
        }

        public static void LoadMap()
        {

            String sArayName;
            int iStat;
            CConfig Config = new CConfig();
            String sPath;
            sPath = Directory.GetCurrentDirectory();
            sPath = sPath + "\\SeqData\\ArrayData.INI";
            
            Config.Load(sPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);

            //Load
            String sRowData;
            String sIndex;
            String sTemp;
            
            for (int i = 0; i < (int)ri.MAX_ARAY; i++)
            {
                sArayName = string.Format("ARRAY_" + ARAY[i].Name);
                if (ARAY[i].Name == "") return;

                Config.GetValue(sArayName,"m_iMaxRow" ,  out iMaxRow ); //ARAY[i].MaxRow  = iMaxRow ;
                Config.GetValue(sArayName,"m_iMaxCol" ,  out iMaxCol ); //ARAY[i].MaxCol  = iMaxCol ; 
                Config.GetValue(sArayName,"m_sID"     ,  out sID     ); ARAY[i].ID      = sID     ; 
                Config.GetValue(sArayName,"m_sLotNo"  ,  out sLotNo  ); ARAY[i].LotNo   = sLotNo  ; 
                Config.GetValue(sArayName,"m_iStep"   ,  out iStep   ); ARAY[i].Step    = iStep   ; 
                Config.GetValue(sArayName,"m_iSubStep",  out iSubStep); ARAY[i].SubStep = iSubStep;
                if(iMaxRow < 1) iMaxRow = 1 ;
                if(iMaxCol < 1) iMaxCol = 1 ;
                
                //메모리 재할당.
                SetMaxColRow(i, iMaxCol , iMaxRow);
                
                try
                {
                    for (int r = 0; r < ARAY[i].MaxRow; r++)
                    {
                        sIndex = string.Format("Row{0:000}", r);
                        Config.GetValue(sArayName, sIndex, out sRowData);
                        for (int c = 0; c < ARAY[i].MaxCol; c++)
                        {
                            sTemp = sRowData.Substring(c * 3, 3);
                            iStat = int.Parse(sTemp);
                            ARAY[i].Chip[c, r].Stat = (cs)iStat;
                        }
                    }
                    for (int r = 0; r < ARAY[i].MaxRow; r++)
                    {
                        sIndex = string.Format("Msk{0:000}", r);
                        Config.GetValue(sArayName, sIndex, out sRowData);
                        for (int c = 0; c < ARAY[i].MaxCol; c++)
                        {
                            sTemp = sRowData.Substring(c * 3, 3);
                            iStat = int.Parse(sTemp);
                            ARAY[i].NotUse[c, r] = iStat==1;
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.ShowMessage("Exception",e.Message);
                }
            }
        }

        public static void SaveMap()
        {

            String sArayName;
            int iStat;

            CConfig Config = new CConfig();
            String sPath;
            
            sPath = Directory.GetCurrentDirectory();
            sPath = sPath + "\\SeqData\\ArrayData.INI"; 

            //Load
            String sRowData;
            String sIndex;
            String sTemp;

            for (int i = 0; i < (int)ri.MAX_ARAY; i++)
            {
                sArayName = string.Format("ARRAY_" + ARAY[i].Name);
                if (ARAY[i].Name == "") return;

                Config.SetValue(sArayName,"m_iMaxRow" ,  ARAY[i].MaxRow );
                Config.SetValue(sArayName,"m_iMaxCol" ,  ARAY[i].MaxCol );
                Config.SetValue(sArayName,"m_sID"     ,  ARAY[i].ID     );
                Config.SetValue(sArayName,"m_sLotNo"  ,  ARAY[i].LotNo  );
                Config.SetValue(sArayName,"m_iStep"   ,  ARAY[i].Step   );
                Config.SetValue(sArayName,"m_iSubStep",  ARAY[i].SubStep); //원래 주석이였다.
                for (int r = 0 ; r < ARAY[i].MaxRow ; r++) {
                    sRowData = "" ;
                    sIndex = string.Format("Row{0:000}", r);
                    for (int c = 0 ; c < ARAY[i].MaxCol ; c++) {
                        sTemp = string.Format("{0:000}", (int)ARAY[i].Chip[c, r].Stat);
                        sRowData += sTemp ;
                    }
                    Config.SetValue(sArayName , sIndex ,sRowData );
                }
                for (int r = 0 ; r < ARAY[i].MaxRow ; r++) {
                    sRowData = "" ;
                    sIndex = string.Format("Msk{0:000}", r);
                    for (int c = 0 ; c < ARAY[i].MaxCol ; c++) {
                        sTemp = string.Format("{0:000}", ARAY[i].NotUse[c, r] ? 1 : 0);
                        sRowData += sTemp ;
                    }
                    Config.SetValue(sArayName , sIndex ,sRowData );
                }
            }
            Config.Save(sPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        }
        
        public static void Trace(int _iId, string sTag = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sMsg;
            for (int i = 0 ; i < ARAY[_iId].MaxRow ; i++)
            {
                sMsg = string.Format("R{0:000}", i);
                for (int j = 0; j < ARAY[_iId].MaxCol; j++)
                {
                    sMsg += string.Format("_{0:00}", (int)ARAY[_iId].Chip[j, i].Stat);
                }
                Log.Trace(sMsg, sTag);
            }
            sMsg = "Name : " + ARAY[_iId].Name + " ID : " + ARAY[_iId].ID + " LotNo : " + ARAY[_iId].LotNo + " Step : " + ARAY[_iId].Step + " SubStep : " + ARAY[_iId].SubStep;
            Log.Trace(sMsg, sTag);
        }
        
        //inline func
        //======================================================================
        public static bool SetStat(int _iId, int _iC , int _iR , cs _eStat) {
            if (_iR < 0 || _iC < 0 || _iR >= ARAY[_iId].MaxRow || _iC >= ARAY[_iId].MaxCol) return false;

            ARAY[_iId].Chip[_iC, _iR].Stat = _eStat;
            //OnChange();
            return true;
        }
        public static void SetStat(int _iId, cs _eStat)
        {

            String sHead = "SetStatAll " + ARAY[_iId].Name;
            //Trace( sHead.c_str() , String(_eStat).c_str());
            for (int r = 0; r < ARAY[_iId].MaxRow; r++)
            {
                for (int c = 0; c < ARAY[_iId].MaxCol; c++)
                {
                    ARAY[_iId].Chip[c, r].Stat = _eStat;
                }
            }

            if (_eStat == cs.None)
            {
                ARAY[_iId].Step    = 0;
                ARAY[_iId].SubStep = 0;
            }
        }

        public static bool RangeSetStat(int _iId, int _iSc, int _iSr, int _iEc, int _iEr, cs _eStat)
        {
            if (_iSr < 0 || _iSc < 0 || _iSr >= ARAY[_iId].MaxRow || _iSc >= ARAY[_iId].MaxCol) return false;
            if (_iEr < 0 || _iEc < 0 || _iEr >= ARAY[_iId].MaxRow || _iEc >= ARAY[_iId].MaxCol) return false;
            if (_iSr > _iEr || _iSc > _iEc) return false;
            for (int r = _iSr; r <= _iEr; r++)
            {
                for (int c = _iSc; c <= _iEc; c++) 
                {
                    ARAY[_iId].Chip[c,r].Stat = _eStat;
                }
            }
            return true ;
        }

        public static void ChangeStat(int _iId, cs _eFrom , cs _eTo) 
        {
            for (int r = 0; r < ARAY[_iId].MaxRow; r++){
                for (int c = 0; c < ARAY[_iId].MaxCol; c++)
                {
                    if (ARAY[_iId].Chip[c,r].Stat == _eFrom) 
                    {
                        ARAY[_iId].Chip[c,r].Stat = _eTo;
                    }
                }
            }
        }

        public static cs GetStat(int _iId, int c , int r) 
        {
            if (r < 0 || c < 0 || r >= ARAY[_iId].MaxRow || c >= ARAY[_iId].MaxCol)
            {
                return cs.None;
            }
            if(ARAY[_iId].NotUse[c,r]) return cs.None;
            return ARAY[_iId].Chip[c,r].Stat;
        }

        public static bool CheckStat(int _iId, int c, int r, params cs[] _csStats) 
        {
            if (r < 0 || c < 0 || r >= ARAY[_iId].MaxRow || c >= ARAY[_iId].MaxCol)
            {
                return false;
            }

            if(ARAY[_iId].NotUse[c,r]) return false ;

            for (int s = 0; s < _csStats.Length; s++)
            {
                if (GetStat(_iId, c,r) == _csStats[s])
                {
                    return true;
                }
            }
            return false  ; 
        }

        
        public static bool CheckAllStat (int _iId, params cs[] _csStats) 
        {
            for (int r = 0; r < ARAY[_iId].MaxRow; r++)
            {
                for (int c = 0; c < ARAY[_iId].MaxCol; c++)
                {
                    if (!ARAY[_iId].NotUse[c,r] && !CheckStat(_iId, c,r, _csStats))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CheckAllStatCol (int _iId, int c , params cs[] _csStats) 
        {
            for (int r = 0; r < ARAY[_iId].MaxRow; r++)
            {
                if (!ARAY[_iId].NotUse[c,r] && !CheckStat(_iId, c,r, _csStats))
                {
                    return false;
                }
                
            }
            return true;
        }

        public static bool CheckAllStatRow (int _iId, int r , params cs[] _csStats) 
        {
            for (int c = 0; c < ARAY[_iId].MaxCol; c++)
            {
                if (!ARAY[_iId].NotUse[c,r] && !CheckStat(_iId, c,r, _csStats))
                {
                    return false;
                }
            }            
            return true;
        }

        //Get Row Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static int GetCntRowStat  (int _iId, int r, params cs[] _csStats) { int iCnt = 0; for (int c = 0 ; c < ARAY[_iId].MaxCol ; c++) if (CheckStat (_iId, c,r,_csStats)) iCnt++; return iCnt; }

        //Get Col Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static int GetCntColStat  (int _iId, int c, params cs[] _csStats) { int iCnt = 0; for (int r = 0 ; r < ARAY[_iId].MaxRow ; r++) if (CheckStat (_iId, c,r,_csStats)) iCnt++; return iCnt; }

        //Get All Count by m_tChipstatus.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public int  GetCntStat (cs Stat) { int iCnt = 0; for (int r = 0 ; r < m_iMaxRow ; r++) iCnt += GetCntRowStat (r,Stat); return iCnt; }
        public static int GetCntStat(int _iId, params cs[] _csStats)
        {
            int iCnt = 0;
            for (int i = 0; i < _csStats.Length; i++)
            {
                for (int r = 0; r < ARAY[_iId].MaxRow; r++) iCnt += GetCntRowStat(_iId, r, _csStats[i]);
            }
            return iCnt;
        }

        //스텟 배열에 있는 것 뺀 상태만 카운팅.
        public static int GetCntStatEx(int _iId, params cs[] _csStats)
        {
            int iCnt = 0;
            iCnt = GetCntStat(_iId, _csStats);
            return (ARAY[_iId].MaxCol * ARAY[_iId].MaxRow - iCnt);
        }

        public static bool IsExist(int _iId, params cs[] _csStats) { 
            for (int r = 0; r < ARAY[_iId].MaxRow; r++) { 
                if (GetCntRowStat(_iId, r, _csStats) != 0) 
                    return true; 
            } 
            return false; 
        }
    }
    //--------------------------------------------------------------------------------
    
    //--------------------------------------------------------------------------------
    //ArrayData
    public class ArrayData
    {
        public delegate void dgResize();
        public dgResize Resize;

        private TChip[,]    m_tChips   = new TChip[1, 1];
        private bool[,]     m_bNotUse  = new bool [1, 1]; //안쓰는 셀들이 있다. 트레이가 이렇게 생긴것들이 있음.. 오스람.
        private bool[,]     m_bWorkChip= new bool [1, 1];
        private string      m_sName    = ""   ; //어레이의 이름 ex)PreBufferZone
        private String      m_sLotNo   = ""   ; //장비에 따라 랏 넘버를 붙여 줄수 있다
        private String      m_sID      = ""   ;//스트립의 메가진 No 슬롯 No ;  m_sID/100 = 메가진 카운트 , m_sID%100 = 슬롯 카운터.
        private int         m_iStep    = 0    ; //어레이에서 어떤 작업을 함에 있어서 스텝. 데이터를 건드리지 않고 작업의 단계를 나타낼때 쓴다.
        private int         m_iSubStep = 0    ; //어레이에서 어떤 작업을 함에 있어서 스텝. 데이터를 건드리지 않고 작업의 단계를 나타낼때 쓴다.(Sub Step)
        private int         m_iMaxCol  = 1    ;
        private int         m_iMaxRow  = 1    ;
        
    
        public TChip[,]    Chip       { get { return m_tChips;     } set { m_tChips      = value; } }
        public bool[,]     NotUse     { get { return m_bNotUse;    } set { m_bNotUse     = value; } }
        public bool[,]     WorkChip   { get { return m_bWorkChip;  } set { m_bWorkChip   = value; } }
        public string      Name       { get { return m_sName;      } set { m_sName       = value; } } 
        public String      LotNo      { get { return m_sLotNo;     } set { m_sLotNo      = value; } }
        public String      ID         { get { return m_sID;        } set { m_sID         = value; } }
        public int         Step       { get { return m_iStep;      } set { m_iStep       = value; } }
        public int         SubStep    { get { return m_iSubStep;   } set { m_iSubStep    = value; } }
        public int         MaxCol     { get { return m_iMaxCol;    } set { m_iMaxCol     = value; } }
        public int         MaxRow     { get { return m_iMaxRow;    } set { m_iMaxRow     = value; } }

        public struct TData
        {
            public double dData1;
            public double dData2;
            public double dData3;
            public double dData4;
        }

        public TData Data;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    //ArrayPos
    public class ArrayPos
    {
        public struct TPara
        {
            //public WorkStartCorner eWorkStartConner ; //어디부터 작업 시작 하는지.
            //public double dWorkStartX ; //eWorkStartConner와 iWorkXCnt iWorkYCnt 가 감안된 워크 스타트포지션 
            //public double dWorkStartY ; //비전으로 생각하면 한번에 칩을 X3,Y2개 검사할때 X1.5 , Y1의 위치. 즉 6개의 칩의 가운데.
            
            

            //public int    iWorkXCnt   ; //한번 작업시에 커버하는 X카운트 작업을 1개씩 하면 1이라고 하면 됨.
            //public int    iWorkYCnt   ; //한번 작업시에 커버하는 Y카운트

            public int    iColCnt     ; //전체 X갯수.
            public int    iRowCnt     ; //전체 Y갯수.
            public double dColPitch   ; //기본 X피치
            public double dRowPitch   ; //기본 Y피치
            public int    iColGrCnt   ; //스트립에서 X 그룹의 갯수. 없을시는 0
            public int    iRowGrCnt   ; //스트립에서 Y 그풉의 갯수.
            public double dColGrGap   ; //1그룹마지막 자제 와 2그룹첫 자제 간의 피치.
            public double dRowGrGap   ; //1그룹마지막 자제 와 2그룹첫 자제 간의 피치.
            

            //딱히 필요는 없음...
            //public double dWidth      ; //한칩의 넓이.
            //public double dHeight     ; //한칩의 높이.

            //1그룹안에서 또 그룹이 나눠져 있을때 사용.===================================================
            public int    iColSbGrCnt ; //서브그룹이 있을때 1그룹안에 서부그룹이 총몇개 있는지..사용 안할시에는 iRowSubGroupCount==0
            public int    iRowSbGrCnt ; 

            public double dRowSbGrGap ; //서브그룹이 있을때 1번써브그룹 마지막 자제 시작지점과 2번써브그룹 첫번째 자제 시작지점간의 거리.
            public double dColSbGrGap ; 
        }
        TPara Para ;

        string sError ; public string Error {get{return sError; } }
        public ArrayPos()
        {
            
        }
        public bool SetPara(TPara _tPara)
        {
            
            //if(_tPara.iWorkXCnt < 1)
            //{
            //    sError = "iWorkXCnt have to be bigger than 0" ;
            //    return false ;
            //}
            //if(_tPara.iWorkYCnt < 1)
            //{
            //    sError = "iWorkYCnt have to be bigger than 0" ;
            //    return false ;
            //}
            if(_tPara.iColCnt < 1)
            {
                sError = "iColCnt have to be bigger than 0" ;
                return false ;
            }
            if(_tPara.iRowCnt < 1)
            {
                sError = "iRowCnt have to be bigger than 0" ;
                return false ;
            }
            if(_tPara.iColGrCnt >= _tPara.iColCnt)
            {
                sError = "iColGrCnt have to be smaller than iColCnt" ;
                return false ;
            }
            if(_tPara.iRowGrCnt >= _tPara.iRowCnt)
            {
                sError = "iRowGrCnt have to be smaller than iRowCnt" ;
                return false ;
            }
            if(_tPara.iColSbGrCnt!=0 && _tPara.iColGrCnt==0) //서브그룹카운트 설정전에 그룹카운트를 설정해야 함.
            {
                sError = "Set iColGrCnt first" ;
                return false ;
            }
            if(_tPara.iRowSbGrCnt!=0 && _tPara.iRowGrCnt==0)
            {
                sError = "Set iRowGrCnt first" ;
                return false ;
            }

            //그룹카운트 체크.
            int iColCntInGroup = _tPara.iColCnt ;
            if(_tPara.iColGrCnt!=0)
            { 
                if(iColCntInGroup % _tPara.iColGrCnt != 0)
                {
                    sError = "iColCnt divided by iColGrCnt have to be integer" ; //그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
                iColCntInGroup =  iColCntInGroup / _tPara.iColGrCnt ;
            }
            if(_tPara.iColSbGrCnt!=0)
            {
                if(iColCntInGroup % _tPara.iColSbGrCnt != 0)
                {
                    sError = "iColCnt divided by iColGrCnt divided by iColCntInGroup have to be integer" ; //서브그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
            }

            //그룹카운트 체크.
            int iRowCntInGroup = _tPara.iRowCnt ;
            if(_tPara.iRowGrCnt!=0)
            { 
                if(iRowCntInGroup % _tPara.iRowGrCnt != 0)
                {
                    sError = "iRowCnt divided by iRowGrCnt have to be integer" ; //그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
                iRowCntInGroup =  iRowCntInGroup / _tPara.iRowGrCnt ;
            }
            if(_tPara.iRowSbGrCnt!=0)
            {
                if(iRowCntInGroup % _tPara.iRowSbGrCnt != 0)
                {
                    sError = "iRowCnt divided by iRowGrCnt divided by iRowCntInGroup have to be integer" ; //서브그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
            }

            Para = _tPara ;
            return true ;
        }

        private double GetPos(int _iIdx , int _iMaxIdx , double _dPitch , int _iGrCnt, double _dGrPitch , int _iSbGrCnt , double _dSbGrPitch)
        {
            //그룹과 서브그룹의 칩갯수.
            int iGroupChipCnt    = _iGrCnt  !=0 ? (_iMaxIdx     / _iGrCnt  ) : 0 ;
            int iSubGroupChipCnt = _iSbGrCnt!=0 ? (iGroupChipCnt / _iSbGrCnt) : 0 ;

            //그룹간의 거리에서 피치를 뺀 나머지 거리를 계산.
            //double dColGroupOfs    = Para.dColGrGap   - Para.dColPitch ; //무조건 0보다 켜야함.
            //double dColSubGroupOfs = Para.dColSbGrGap - Para.dColPitch ; //무조건 0보다 켜야함.

            //dX = _iC * Para.dColPitch ;

            double dPos = 0 ;
            // 칩 , 칩그룹 , 칩서브그룹 피치개념이여서 1부터 시작.
            for(int c = 1 , cg = 1 , csg = 1 ; c<=_iIdx ; c++ , cg++ , csg++) 
            {
                if(iGroupChipCnt!=0 && cg==iGroupChipCnt)//그룹의 칩카운트와 같다면 그룹갭을 더함.
                {
                    dPos += _dGrPitch ;
                    cg  = 0 ;//그룹 카운트 리셑.
                    csg = 0 ;//그룹 카운트가 상위여서 서브그룹카운트도 리셑.
                }
                else if(iSubGroupChipCnt != 0 && cg==iSubGroupChipCnt)//서브그룹의 칩카운트와 같다면.
                {
                    dPos += _dSbGrPitch ;
                    csg = 0 ; //서브그룹카운트 리셑.
                }
                else 
                {
                    dPos += _dPitch ;
                }
            }

            return dPos ;
        }

        /// <summary>
        ///하기내용 일단 너무복잡해 고려하지 않음===========
        ///성용이 말로는 중간에 서브그룹의 칩갯수가 3개일때 2개씩검사해서 1번은 2개검사 2번은 1개검사 3번은 2개검사 4번은 1개검사 이렇게도 된다고함.
        ///그래서 포지션은 검사세트의 센터로 하면 안되고 첫자재의 포지션이 세트첫번째 포지션과 일치하게 검사해야함. 
        /// </summary>
        /// <param name="_iC">Col</param>
        /// <param name="_iR">Row</param>
        /// <param name="_iQuadrant">하부가 고정된 스테이지고 상부가 움직일때 4사분면중 사용사분면 설정</param>
        /// <param name="_dX">결과값 X</param>
        /// <param name="_dY">결과값 Y</param>
        /// <returns></returns>
        public bool GetPos(int _iC , int _iR , int _iQuadrant ,out double _dX , out double _dY) //Array와 같이 왼쪽 위가 기준 포지션.
        {
            _dX = 0 ;
            _dY = 0 ;
            if(_iQuadrant < 1 || _iQuadrant > 4)
            {
                sError = "Quadrant have to be between 1 and 4" ;
                return false ;
            }
            if(_iC >= Para.iColCnt)
            {
                sError = "_iC have to be smaller than iColCnt" ;
                return false ;
            }
            if(_iR >= Para.iRowCnt)
            {
                sError = "_iR have to be smaller than iRowCnt" ;
                return false ;
            }
            if(_iC < 0)
            {
                sError = "_iC have to be bigger than 0" ;
                return false ;
            }
            if(_iR < 0)
            {
                sError = "_iR have to be bigger than 0" ;
                return false ;
            }

            //double dMaxX = GetPos(Para.iColCnt-1, Para.iColCnt, Para.dColPitch, Para.iColGrCnt , Para.dColGrGap , Para.iColSbGrCnt , Para.dColSbGrGap);
            //double dMaxY = GetPos(Para.iRowCnt-1, Para.iRowCnt, Para.dRowPitch, Para.iRowGrCnt , Para.dRowGrGap , Para.iRowSbGrCnt , Para.dRowSbGrGap);

            double dRetX = GetPos(_iC, Para.iColCnt, Para.dColPitch, Para.iColGrCnt , Para.dColGrGap , Para.iColSbGrCnt , Para.dColSbGrGap);
            double dRetY = GetPos(_iR, Para.iRowCnt, Para.dRowPitch, Para.iRowGrCnt , Para.dRowGrGap , Para.iRowSbGrCnt , Para.dRowSbGrGap);

            if(_iQuadrant == 1)//1사분면기준인 상태의 값.
            {
                _dX =  dRetX ;
                _dY = -dRetY ;                
            }
            else if(_iQuadrant == 2)//2사분면기준인 상태의 값.
            {
                _dX = -dRetX ;
                _dY = -dRetY ;   
            }
            else if(_iQuadrant == 3)//3사분면기준인 상태의 값.
            {
                _dX = -dRetX ;
                _dY =  dRetY ;
            }
            else //4사분면기준인 상태의 값.
            {
                _dX = dRetX  ;
                _dY = dRetY  ;
            }

            return true ;
        }
    }
    //--------------------------------------------------------------------------------
}
