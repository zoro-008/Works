using COMMON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public class CSpcSubUnit
    {
        public struct TLotData
        {
            public int    WorkNo        ;
            public string Result        ;
            public string ResultNg      ; //NG 이유
            public double TargetToque   ; //체결 목표 토크
            public double MaxToque      ; //맥스토크
            public double TargetToque1  ; //체결 목표 토크
            public double MaxToque1     ; //맥스토크
            public double TargetToque2  ; //체결 목표 토크
            public double MaxToque2     ; //맥스토크
            public double TargetToque3  ; //체결 목표 토크
            public double MaxToque3     ; //맥스토크
            public double HeightData1   ; //좌측 부터 1번
            public double HeightData2   ;
            public double HeightData3   ;
            public double HeightData4   ;

        }

        //-------------------------------------------------------------------------------------------------------
        //랏에 MGZ넘버가 없는 그냥 랏별 데이터 보여주기용
        //-------------------------------------------------------------------------------------------------------
        public void DispLotData(string sPath, ListView _lvTable)
        {
            if (_lvTable == null) return;

            _lvTable.BeginUpdate();
            
            int iCnt = 0;
            CIniFile IniCnt = new CIniFile(sPath);
            IniCnt.Load("ETC", "DataCnt", ref iCnt);

            List<TLotData> Datas = new List<TLotData>();
            for (int c = 0; c < iCnt; c++)
            {
                TLotData LotData = new TLotData();
                CAutoIniFile.LoadStruct(sPath, "LotData" + c.ToString(), ref LotData);
                Datas.Add(LotData);
            }

            _lvTable.Clear();
            _lvTable.View               = View.Details;
            _lvTable.LabelEdit          = true;
            _lvTable.AllowColumnReorder = true;
            _lvTable.FullRowSelect      = true;
            _lvTable.GridLines          = true;
            //_lvTable.Sorting          = SortOrder.Descending;
            _lvTable.Scrollable         = true;

            //Type type = typeof(TLotData);
            //int iCntOfItem = type.GetProperties().Length;
            //FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //
            ////컬럼추가 하고 이름을 넣는다.
            //_lvTable.Columns.Add("No", 100, HorizontalAlignment.Left);
            //for (int c = 0; c < f.Length; c++)
            //{
            //    _lvTable.Columns.Add(f[c].Name, 100, HorizontalAlignment.Left);
            //}

            Type type = typeof(TLotData);
            int iCntOfItem = type.GetProperties().Length;
            FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            //컬럼추가 하고 이름을 넣는다.
            _lvTable.Columns.Add("No", 100, HorizontalAlignment.Left);
            
            _lvTable.Columns.Add("작업번호"          , 100, HorizontalAlignment.Left);        
            _lvTable.Columns.Add("결과"              , 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("NG결과"            , 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 체결토크"    , 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 최대토크"    , 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 후 체결토크1", 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 후 최대토크1", 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 후 체결토크2", 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 후 최대토크2", 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 후 체결토크3", 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("본체결 후 최대토크3", 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("높이측정 데이터1"   , 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("높이측정 데이터2"   , 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("높이측정 데이터3"   , 100, HorizontalAlignment.Left);
            _lvTable.Columns.Add("높이측정 데이터4"   , 100, HorizontalAlignment.Left);

            _lvTable.Items.Clear();
            string sValue = "";
            string sName = "";
            ListViewItem[] liitem = new ListViewItem[Datas.Count];
            for (int r = 0; r < Datas.Count; r++)
            {
                liitem[r] = new ListViewItem(string.Format("{0}", r+1));
                for (int c = 0; c < f.Length; c++)
                {
                    sName = f[c].Name;
                    sValue = f[c].GetValue(Datas[r]).ToString();
                    liitem[r].SubItems.Add(sValue);
                }
                liitem[r].UseItemStyleForSubItems = false;
                _lvTable.Items.Add(liitem[r]);
            }
            //_lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //if(iCnt == 0) _lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize   );
            //else          _lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            _lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize   );

            _lvTable.EndUpdate();
        }

        public void SaveData(double _dStartTime  , string _sLotNo , TLotData _LotData)
        {
            try
            {
                string sSpcFd = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString();
                string sSttDt = DateTime.FromOADate(_dStartTime).ToString("yyyyMMdd");//DateTime.Now.ToString("yyyyMMdd");

                string sPath  = SPC.LOT.Folder + sSttDt + "\\" + _sLotNo + ".ini";
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sPath));
                //DirectoryInfo di = new DirectoryInfo(sPath);
                if (!di.Exists) di.Create();

                //몬가 마음에 안드는데 ... 시간상 패스 대충 쓸라고 만든거임
                //카운트 저장.            
                int iCnt = 0;
                CIniFile IniCnt = new CIniFile(sPath);
                IniCnt.Load("ETC", "DataCnt", ref iCnt);//이거 여기서 저장하는데 Datamap에서도 가져다가 쓴다.
                IniCnt.Save("ETC", "DataCnt", iCnt + 1);

                //데이터 저장.
                CAutoIniFile.SaveStruct(sPath, "LotData"+(iCnt).ToString(), ref _LotData);
            }
            catch (Exception _e)
            {
                Log.Trace("Data Save Fail" + _e.Message);
            }
        }

    }
}
