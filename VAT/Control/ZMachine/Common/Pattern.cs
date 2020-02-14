using System;
using COMMON;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace Machine
{
    public class DispensePattern
    {
        //const
        public const int MAX_DSP_CMD = 50 ;
        
        public const int PANEL_DRAW_POS  = 500; //Px
        public const int REAL_DRAW_POS   = 50 ; //mm 

        //private
        //private double[] m_dDispXPos = new double[MAX_PATTERN_POS]; // 실제 포지션 값이 저장되어 있는 배열
        //private double[] m_dDispYPos = new double[MAX_PATTERN_POS];
        //private double[] m_dDispZPos = new double[MAX_PATTERN_POS];

        public struct TDspPos 
        {
            public double dPosX   ;
            public double dPosY   ;
            public double dPosZ   ;
            public int    iDelay  ;
            public double dSpeed  ;
            public double dUVSpeed;
            public bool   bDispOn ;
        }

        public TDspPos[] DspPos = new TDspPos[MAX_DSP_CMD];

        //private bool  [] m_bDispOn   = new bool  [MAX_DSP_CMD]; // Dispensor 켜기 끄기.

        private double m_dXScale     ; // 실제 스케일 값이 저장되어 있는 값
        private double m_dYScale     ;

        private double m_dXSttOffset ; // Node 1번 처음 시작 위치 X Offset mm
        private double m_dYSttOffset ; //                                  mm

        private double m_dSpeed      ; // Node 간 속도 제어를 하기 위해 설정

        private int    m_iDispPosCnt ; // 디스펜서 움직일 노드 갯수.

        private string m_sCrntDev;

        private bool m_bPreInRange ;
        private bool m_bInRange    ;//라인 선택 범위 안에 있음.

        private double m_dAcc        ; // Dispensor Acc 속도
        private double m_dDec        ; // Dispensor Dec 속도  
        //public int m_iSubRgb      ;
        //public int m_iDieRgb      ;
        public int m_iDisprLineRgb;
        public int m_iDisprSlctRgb;
        //public int m_iVsnRgb      ;
        

        //public
        public void Init()
        {
            LoadLastInfo();
            Load(m_sCrntDev);
            LoadPttColor(m_sCrntDev);

            m_bPreInRange = false;
            m_bInRange    = false;
        }

        // 포지션 값 관련
        //Get
        public double GetDispPosX(int _iNo) // 순수한 X Pos 값을 가져 온다 (Form이 새로 열릴 때, Get으로 이 값을 가지고 와야함)
        {
            if (_iNo < 0                ) return 0.0;
            if (_iNo > m_iDispPosCnt - 1) return 0.0;

            return DspPos[_iNo].dPosX;
        }

        public double GetDispPosY(int _iNo)
        {
            if (_iNo < 0                ) return 0.0;
            if (_iNo > m_iDispPosCnt - 1) return 0.0;

            return DspPos[_iNo].dPosY;
        }

        public double GetDispPosZ(int _iNo)
        {
            if (_iNo < 0                ) return 0.0;
            if (_iNo > m_iDispPosCnt - 1) return 0.0;

            return DspPos[_iNo].dPosZ;
        }

        public double GetScaleDispPosX(int _iNo) // X Scale Pos 을 가져 온다 GetPosX * Scale + Sttofset
        {
            if (_iNo < 0                ) return 0.0;
            if (_iNo > m_iDispPosCnt - 1) return 0.0;

            //return m_dXScalePos[_iNo];
            return DspPos[_iNo].dPosX * m_dXScale + m_dXSttOffset;
        }

        public double GetScaleDispPosY(int _iNo)
        {
            if (_iNo < 0                ) return 0.0;
            if (_iNo > m_iDispPosCnt - 1) return 0.0;

            //return m_dYScalePos[_iNo];
            return DspPos[_iNo].dPosY * m_dYScale + m_dYSttOffset;
        }

        public double GetSpeed()  // Speed 을 가져 온다
        {
            return m_dSpeed;
        }

        public double GetSttOffsetX()  // 스타트 위치 값 가져오기 위해
        {
            return m_dXSttOffset;
        }

        public double GetSttOffsetY() 
        {
            return m_dYSttOffset;
        }

        public double GetScaleX()  // X Scale 을 가져 온다
        {
            return m_dXScale;
        }

        public double GetScaleY() 
        {
            return m_dYScale;
        }

        public double GetAcc()  // Acc 을 가져 온다
        {
            return m_dAcc;
        }

        public double GetDec()  // Dec 을 가져 온다
        {
            return m_dDec;
        }

        public bool GetDispOn(int _iNo)  // Disp 켜기 끄기.. 가져 온다.
        {
            if (_iNo < 0                ) return false;
            if (_iNo > m_iDispPosCnt - 1) return false;

            return DspPos[_iNo].bDispOn;
        }

        //Set
        public void SetScale(double _dX, double _dY)  // X, Y의 Scale 값을 설정
        {
            //double dCvsData = 0.01 ;

            if (_dX > 1) _dX = 1;
            if (_dY > 1) _dY = 1;
            m_dXScale = _dX;
            m_dYScale = _dY;
        }

        public void SetSttOffset(double _dX, double _dY)  // Node 1번 처음 시작 위치 X Offset
        {
            m_dXSttOffset = _dX;
            m_dYSttOffset = _dY;
        }

        public void SetSpeed(double _dVal)  // Speed 값을 설정
        {
            m_dSpeed = _dVal;
        }

        public void SetDispPosX(int _iNo, double _dPos)  // X Pos 값을 설정
        {
            if (_iNo < 0                ) return;
            if (_iNo > m_iDispPosCnt - 1) return;

            DspPos[_iNo].dPosX = _dPos;
        }

        public void SetDispPosY(int _iNo, double _dPos) 
        {
            if (_iNo < 0                ) return;
            if (_iNo > m_iDispPosCnt - 1) return;

            DspPos[_iNo].dPosY = _dPos;
        }

        public void SetDispPosZ(int _iNo, double _dPos) 
        {
            if (_iNo < 0                ) return;
            if (_iNo > m_iDispPosCnt - 1) return;

            DspPos[_iNo].dPosZ = _dPos;
        }

        public void SetDispPosCnt(int _iCnt) 
        {
            m_iDispPosCnt = _iCnt;
        }

        public void SetAccDec(double _dAcc , double _dDec)
        {
            m_dAcc = _dAcc;
            m_dDec = _dDec;
        }

        public void SetDispOn(int _iNo , bool _bOn)
        {
            if (_iNo < 0                ) return;
            if (_iNo > m_iDispPosCnt - 1) return;

            DspPos[_iNo].bDispOn = _bOn;
        }

        public void SetScalePosX(int _iNo)  // 해당 Node 넘버에 Scale 값을 설정
        {

        }

        public void SetScalePosY(int _iNo) 
        {

        }

        public void Load(string _sDevName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath;

            sPath = sExeFolder + "JobFile\\" + _sDevName + "\\DispensePattern.pat";
            
            LoadPat(sPath);
        }

        public void Save(string _sDevName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath;

            sPath = sExeFolder + "JobFile\\" + _sDevName + "\\DispensePattern.pat";

            SavePat(sPath);
        }

        //패턴 파일 단일 로딩 할때...
        public void LoadPat(string _sFileName) 
        {
            string sPath, sNum;
            int iNum;

            sPath = _sFileName;

            CIniFile IniPattern = new CIniFile(sPath);

            //dPosX   
            //dPosY   
            //dPosZ   
            //iDelay  
            //dSpeed  
            //dUVSpeed
            //bDispOn 


            for (iNum = 0; iNum < MAX_DSP_CMD; iNum++)
            {
                sNum = iNum.ToString();
                IniPattern.Load("PatternPara", "dPosX"         + sNum, out DspPos[iNum].dPosX  );
                IniPattern.Load("PatternPara", "dPosY"         + sNum, out DspPos[iNum].dPosY  );
                IniPattern.Load("PatternPara", "dPosZ"         + sNum, out DspPos[iNum].dPosZ  );
                IniPattern.Load("PatternPara", "bDispOn"       + sNum, out DspPos[iNum].bDispOn);
            }

            IniPattern.Load("PatternPara" , "m_dXScale"     , out m_dXScale    );
            IniPattern.Load("PatternPara" , "m_dYScale"     , out m_dYScale    );
            
            IniPattern.Load("PatternPara" , "m_dXSttOffset" , out m_dXSttOffset);
            IniPattern.Load("PatternPara" , "m_dYSttOffset" , out m_dYSttOffset);
            
            IniPattern.Load("PatternPara" , "m_dSpeed"      , out m_dSpeed     );
            IniPattern.Load("PatternPara" , "m_iDispPosCnt" , out m_iDispPosCnt);
            
            IniPattern.Load("PatternPara" , "m_dAcc"        , out m_dAcc       );
            IniPattern.Load("PatternPara" , "m_dDec"        , out m_dDec       );

        }

        public void SavePat(string _sFileName) 
        {
            string sPath, sNum;
            int iNum;

            sPath = _sFileName;

            CIniFile IniPattern = new CIniFile(sPath);

            for (iNum = 0; iNum < MAX_DSP_CMD; iNum++)
            {
                sNum = iNum.ToString();
                IniPattern.Save("PatternPara", "dPosX"   + sNum, DspPos[iNum].dPosX);
                IniPattern.Save("PatternPara", "dPosY"   + sNum, DspPos[iNum].dPosY);
                IniPattern.Save("PatternPara", "dPosZ"   + sNum, DspPos[iNum].dPosZ);
                IniPattern.Save("PatternPara", "bDispOn" + sNum, DspPos[iNum].bDispOn);
            }

            IniPattern.Save("PatternPara" , "m_dXScale"     , m_dXScale    );
            IniPattern.Save("PatternPara" , "m_dYScale"     , m_dYScale    );
            
            IniPattern.Save("PatternPara" , "m_dXSttOffset" , m_dXSttOffset);
            IniPattern.Save("PatternPara" , "m_dYSttOffset" , m_dYSttOffset);
            
            IniPattern.Save("PatternPara" , "m_dSpeed"      , m_dSpeed     );
            IniPattern.Save("PatternPara" , "m_iDispPosCnt" , m_iDispPosCnt);
            
            IniPattern.Save("PatternPara" , "m_dAcc"        , m_dAcc       );
            IniPattern.Save("PatternPara" , "m_dDec"        , m_dDec       );
        }

        //색깔 바꾸는거 저장/로드하는부분
        public void LoadPttColor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPttColorPath = sExeFolder + "JobFile\\" + _sJobName + "\\DispenseColor.ini";

            CIniFile IniPttColor = new CIniFile(sPttColorPath);

            IniPttColor.Load("PttColor", "m_iDisprLineRgb", out m_iDisprLineRgb);
            IniPttColor.Load("PttColor", "m_iDisprSlctRgb", out m_iDisprSlctRgb);
        }

        public void SavePttColor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPttColorPath = sExeFolder + "JobFile\\" + _sJobName + "\\DispenseColor.ini";

            CIniFile IniPttColor = new CIniFile(sPttColorPath);

            IniPttColor.Save("PttColor", "m_iDisprLineRgb", m_iDisprLineRgb);
            IniPttColor.Save("PttColor", "m_iDisprSlctRgb", m_iDisprSlctRgb);
        }

        public int GetDispPosCnt()
        {
            return m_iDispPosCnt;
        }

        public void LoadLastInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\LastInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);

            //Load
            IniLastInfo.Load("LAST WORK INFO", "Device", out m_sCrntDev);

            if (m_sCrntDev == "") m_sCrntDev = "NONE";
        }

        public void SetDispValue(uint _uPstnNo, double _dValue)
        {
            //PttValue[_uPstnNo].dValue = _dValue;
            
            //UI에 접근 하기 위한 인보크
            if (lvNodePos.Parent != null)
            {
                if (lvNodePos.InvokeRequired)
                {
                    lvNodePos.Invoke(new MethodInvoker(delegate()
                    {
                        // code for running in ui thread
                        for (int i=0; i < GetDispPosCnt(); i++)
                        {
                            if (lvNodePos.SelectedItems[0].Index == i) lvNodePos.Items[i].SubItems[(int)_uPstnNo].Text = _dValue.ToString();
                        }
                    }));
                }
                else
                {
                    for (int i = 0; i < GetDispPosCnt(); i++)
                    {
                        if (lvNodePos.SelectedItems[0].Index == i) lvNodePos.Items[i].SubItems[(int)_uPstnNo].Text = _dValue.ToString();
                    }
                }
            }
        }

        //디스펜싱 패턴 노드 리스트뷰 만드는부분
        public ListViewEx lvNodePos;
               TextBox    tbPttEdit;
        public ListViewItem[] liInput = new ListViewItem[MAX_DSP_CMD];//이거 노드카운트 수량만큼 만들어야함
        public void InitNodePosView(Panel _pnBase)
        {
            lvNodePos = new ListViewEx();
            //PttValue = new TPttValue[GetPosCnt()];

            lvNodePos.Parent = _pnBase;
            lvNodePos.Dock = DockStyle.Fill;
        
            lvNodePos.Clear();
            lvNodePos.View          = View.Details;
            lvNodePos.FullRowSelect = true        ;
            lvNodePos.MultiSelect   = true        ;
            lvNodePos.GridLines     = true        ;
        
            //셀높이 조절하기 편법.
            ImageList dummyImage     = new ImageList()               ;
            dummyImage.ImageSize     = new System.Drawing.Size(1, 25);
            lvNodePos.SmallImageList = dummyImage                    ;
        
            lvNodePos.Columns.Add(""             , 80        , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("X Pos"        , 70        , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("Y Pos"        , 70        , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("Z Pos"        , 70        , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("X Scale"      , 80        , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("Y Scale"      , 80        , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("Disp"         , 50        , HorizontalAlignment.Left);

            for (int i = 0; i < GetDispPosCnt(); i++)
            {
                liInput[i] = new ListViewItem("NODE" + i.ToString());

                for (int j = 0; j < lvNodePos.Columns.Count; j++)
                {
                    liInput[i].SubItems.Add("");
                }
        
                liInput[i].UseItemStyleForSubItems = false;
                lvNodePos.Items.Add(liInput[i]);
            }
        
            var PropInput = lvNodePos.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvNodePos, true, null);

            tbPttEdit = new TextBox();
              
            tbPttEdit.Visible = false;
            tbPttEdit.BorderStyle = BorderStyle.FixedSingle;
            tbPttEdit.Tag = 0;
            tbPttEdit.Leave += tbPttEdit_Leave;

            tbPttEdit.KeyDown += tbPttEdit_KeyDown;

            if (GetDispPosCnt() != 0)
            {
                lvNodePos.AddEmbeddedControl(tbPttEdit, 1, 0);
            }

            lvNodePos.Click += new EventHandler(lvNodePos_Click);
        }

        

        private void tbPttEdit_Leave(object sender, EventArgs e)
        {
            //TextBox Tb = sender as TextBox;
            TextBox Tb = tbPttEdit as TextBox;
            int iPosId = (int)Tb.Tag;

            Tb.Visible = false;

            double dVal;
            if (!double.TryParse(Tb.Text, out dVal))
            {
                return;
            }
            SetDispValue((uint)iPosId, dVal);
        }

        private void tbPttEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double dVal;
                int iPosId = (int)tbPttEdit.Tag;
                tbPttEdit.Visible = false;
                if (!double.TryParse(tbPttEdit.Text, out dVal))
                {
                    return;
                }
                SetDispValue((uint)iPosId, dVal);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                tbPttEdit.Text = "";
                tbPttEdit.Visible = false;
            }
        }

        private void lvNodePos_Click(object sender, EventArgs e)
        {
            ListView Lv = sender as ListView;

            Point mousePos = Lv.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = Lv.HitTest(mousePos);
            if (hitTest.Item == null) return;
            int row = hitTest.Item.Index;
            int col = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);

            if (col == 1)
            {
                lvNodePos.MoveEmbeddedControl(tbPttEdit, 1, row); //이게 텍스트박스 옮기는거

                tbPttEdit.Visible = true;
                tbPttEdit.Text = GetDispPosX(row).ToString();
                tbPttEdit.Tag = col;
                tbPttEdit.Select(); ;//Focus();

            }

            else if (col == 2)
            {
                lvNodePos.MoveEmbeddedControl(tbPttEdit, 2, row); //이게 텍스트박스 옮기는거

                tbPttEdit.Visible = true;
                tbPttEdit.Text = GetDispPosY(row).ToString();
                tbPttEdit.Tag = col;
                tbPttEdit.Select(); ;//Focus();

            }

            else if (col == 3)
            {
                lvNodePos.MoveEmbeddedControl(tbPttEdit, 3, row); //이게 텍스트박스 옮기는거

                tbPttEdit.Visible = true;
                tbPttEdit.Text = GetDispPosZ(row).ToString();
                tbPttEdit.Tag = col;
                tbPttEdit.Select(); ;//Focus();

            }

            else if (col == 6)
            {
                lvNodePos.MoveEmbeddedControl(tbPttEdit, 6, row); //이게 텍스트박스 옮기는거

                tbPttEdit.Visible = true;
                tbPttEdit.Text = GetDispOn(row) ? "1" : "0";
                tbPttEdit.Tag = col;
                tbPttEdit.Select(); ;//Focus();

            }

            //for (int i = 0; i < GetPosCnt(); i++)
            //{
            //    if (row == i)
            //    {
            //
            //    }
            //}
        }

        //NodeCount에 맞춰서 Row 넣었다 줄였다 하는부분
        public void SetListViewItem()
        {
            int iBfNodeCnt = lvNodePos.Items.Count;

            //NodeCount 늘어날때
            if (GetDispPosCnt() > iBfNodeCnt)
            {
                for (int i = iBfNodeCnt; i < GetDispPosCnt(); i++)
                {
                    liInput[i] = new ListViewItem("NODE" + i.ToString());

                    for (int j = 0; j < lvNodePos.Columns.Count; j++)
                    {
                        liInput[i].SubItems.Add("");
                    }

                    liInput[i].UseItemStyleForSubItems = false;
                    lvNodePos.Items.Add(liInput[i]);
                }
            }

            //NodeCount 줄어들때
            else
            {
                for (int i = iBfNodeCnt; i >= GetDispPosCnt(); i--)
                {
                    lvNodePos.Items.Remove(liInput[i]);
                }
            }
        }

        public void ListViewRefresh()
        {
            //lvNodePos.Refresh();
            lvNodePos.Invalidate();
        }

    }



















    public class HeightPattern
    {
        //const
        public const int MAX_HGHT_CMD = 50;

        public const int PANEL_DRAW_POS = 500; //Px
        public const int REAL_DRAW_POS = 50; //mm 

        public struct THghtPos
        {
            public double dPosX;
            public double dPosY;
            public double dPosZ;
            public int    iDelay;
            public double dSpeed;
        }

        public THghtPos[] HghtPos = new THghtPos[MAX_HGHT_CMD];

        private double m_dXScale; // 실제 스케일 값이 저장되어 있는 값
        private double m_dYScale;

        private double m_dXSttOffset; // Node 1번 처음 시작 위치 X Offset mm
        private double m_dYSttOffset; //                                  mm

        private double m_dSpeed; // Node 간 속도 제어를 하기 위해 설정

        private int m_iHghtPosCnt; // 높이측정기 움직일 노드 갯수.     

        private string m_sCrntDev;

        private bool m_bPreInRange;
        private bool m_bInRange;//라인 선택 범위 안에 있음.

        private double m_dAcc; // Dispensor Acc 속도
        private double m_dDec; // Dispensor Dec 속도  

        public int m_iHghtRgb;

        //public
        public void Init()
        {
            LoadLastInfo();
            Load(m_sCrntDev);
            LoadPttColor(m_sCrntDev);

            m_bPreInRange = false;
            m_bInRange = false;
        }

        // 포지션 값 관련
        //Get
        public double GetHghtPosX(int _iNo) // 순수한 X Pos 값을 가져 온다 (Form이 새로 열릴 때, Get으로 이 값을 가지고 와야함)
        {
            if (_iNo < 0) return 0.0;
            if (_iNo > m_iHghtPosCnt - 1) return 0.0;

            return HghtPos[_iNo].dPosX;
        }

        public double GetHghtPosY(int _iNo)
        {
            if (_iNo < 0) return 0.0;
            if (_iNo > m_iHghtPosCnt - 1) return 0.0;

            return HghtPos[_iNo].dPosY;
        }

        public double GetHghtPosZ(int _iNo)
        {
            if (_iNo < 0) return 0.0;
            if (_iNo > m_iHghtPosCnt - 1) return 0.0;

            return HghtPos[_iNo].dPosZ;
        }

        public double GetScaleHghtPosX(int _iNo)
        {
            if (_iNo < 0) return 0.0;
            if (_iNo > m_iHghtPosCnt - 1) return 0.0;

            //return m_dYScalePos[_iNo];
            return HghtPos[_iNo].dPosX * m_dXScale + m_dXSttOffset;
        }

        public double GetScaleHghtPosY(int _iNo)
        {
            if (_iNo < 0) return 0.0;
            if (_iNo > m_iHghtPosCnt - 1) return 0.0;

            //return m_dYScalePos[_iNo];
            return HghtPos[_iNo].dPosY * m_dYScale + m_dYSttOffset;
        }

        public double GetSpeed()  // Speed 을 가져 온다
        {
            return m_dSpeed;
        }

        public double GetSttOffsetX()  // 스타트 위치 값 가져오기 위해
        {
            return m_dXSttOffset;
        }

        public double GetSttOffsetY()
        {
            return m_dYSttOffset;
        }

        public double GetScaleX()  // X Scale 을 가져 온다
        {
            return m_dXScale;
        }

        public double GetScaleY()
        {
            return m_dYScale;
        }

        public double GetAcc()  // Acc 을 가져 온다
        {
            return m_dAcc;
        }

        public double GetDec()  // Dec 을 가져 온다
        {
            return m_dDec;
        }

        //Set
        public void SetScale(double _dX, double _dY)  // X, Y의 Scale 값을 설정
        {
            //double dCvsData = 0.01 ;

            if (_dX > 1) _dX = 1;
            if (_dY > 1) _dY = 1;
            m_dXScale = _dX;
            m_dYScale = _dY;
        }

        public void SetSttOffset(double _dX, double _dY)  // Node 1번 처음 시작 위치 X Offset
        {
            m_dXSttOffset = _dX;
            m_dYSttOffset = _dY;
        }

        public void SetSpeed(double _dVal)  // Speed 값을 설정
        {
            m_dSpeed = _dVal;
        }

        public void SetHghtPosX(int _iNo, double _dPos)  // X Pos 값을 설정
        {
            if (_iNo < 0) return;
            if (_iNo > m_iHghtPosCnt - 1) return;

            HghtPos[_iNo].dPosX = _dPos;
        }

        public void SetHghtPosY(int _iNo, double _dPos)
        {
            if (_iNo < 0) return;
            if (_iNo > m_iHghtPosCnt - 1) return;

            HghtPos[_iNo].dPosY = _dPos;
        }

        public void SetHghtPosZ(int _iNo, double _dPos)
        {
            if (_iNo < 0) return;
            if (_iNo > m_iHghtPosCnt - 1) return;

            HghtPos[_iNo].dPosZ = _dPos;
        }

        public void SetHghtPosCnt(int _iCnt)
        {
            m_iHghtPosCnt = _iCnt;
        }

        public void SetAccDec(double _dAcc, double _dDec)
        {
            m_dAcc = _dAcc;
            m_dDec = _dDec;
        }

        public void SetScalePosX(int _iNo)  // 해당 Node 넘버에 Scale 값을 설정
        {

        }

        public void SetScalePosY(int _iNo)
        {

        }

        public void Load(string _sDevName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath;

            sPath = sExeFolder + "JobFile\\" + _sDevName + "\\HeightPattern.pat";

            LoadPat(sPath);
        }

        public void Save(string _sDevName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath;

            sPath = sExeFolder + "JobFile\\" + _sDevName + "\\HeightPattern.pat";

            SavePat(sPath);
        }

        //패턴 파일 단일 로딩 할때...
        public void LoadPat(string _sFileName)
        {
            string sPath, sNum;
            int iNum;

            sPath = _sFileName;

            CIniFile IniPattern = new CIniFile(sPath);

            for (iNum = 0; iNum < MAX_HGHT_CMD; iNum++)
            {
                sNum = iNum.ToString();

                IniPattern.Load("PatternPara", "m_dHghtXPos" + sNum, out HghtPos[iNum].dPosX);
                IniPattern.Load("PatternPara", "m_dHghtYPos" + sNum, out HghtPos[iNum].dPosY);
                IniPattern.Load("PatternPara", "m_dHghtZPos" + sNum, out HghtPos[iNum].dPosZ);
            }

            IniPattern.Load("PatternPara", "m_dXScale", out m_dXScale);
            IniPattern.Load("PatternPara", "m_dYScale", out m_dYScale);

            IniPattern.Load("PatternPara", "m_dXSttOffset", out m_dXSttOffset);
            IniPattern.Load("PatternPara", "m_dYSttOffset", out m_dYSttOffset);

            IniPattern.Load("PatternPara", "m_dSpeed", out m_dSpeed);
            IniPattern.Load("PatternPara", "m_iHghtPosCnt", out m_iHghtPosCnt);

            IniPattern.Load("PatternPara", "m_dAcc", out m_dAcc);
            IniPattern.Load("PatternPara", "m_dDec", out m_dDec);

        }

        public void SavePat(string _sFileName)
        {
            string sPath, sNum;
            int iNum;

            sPath = _sFileName;

            CIniFile IniPattern = new CIniFile(sPath);

            for (iNum = 0; iNum < MAX_HGHT_CMD; iNum++)
            {
                sNum = iNum.ToString();

                IniPattern.Save("PatternPara", "m_dHghtXPos" + sNum, HghtPos[iNum].dPosX);
                IniPattern.Save("PatternPara", "m_dHghtYPos" + sNum, HghtPos[iNum].dPosY);
                IniPattern.Save("PatternPara", "m_dHghtZPos" + sNum, HghtPos[iNum].dPosZ);
            }

            IniPattern.Save("PatternPara", "m_dXScale", m_dXScale);
            IniPattern.Save("PatternPara", "m_dYScale", m_dYScale);

            IniPattern.Save("PatternPara", "m_dXSttOffset", m_dXSttOffset);
            IniPattern.Save("PatternPara", "m_dYSttOffset", m_dYSttOffset);

            IniPattern.Save("PatternPara", "m_dSpeed", m_dSpeed);
            IniPattern.Save("PatternPara", "m_iHghtPosCnt", m_iHghtPosCnt);


            IniPattern.Save("PatternPara", "m_dAcc", m_dAcc);
            IniPattern.Save("PatternPara", "m_dDec", m_dDec);
        }

        //색깔 바꾸는거 저장/로드하는부분
        public void LoadPttColor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPttColorPath = sExeFolder + "JobFile\\" + _sJobName + "\\HeightColor.ini";

            CIniFile IniPttColor = new CIniFile(sPttColorPath);

            IniPttColor.Load("PttColor", "m_iHghtRgb     ", out m_iHghtRgb);
        }

        public void SavePttColor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPttColorPath = sExeFolder + "JobFile\\" + _sJobName + "\\HeightColor.ini";

            CIniFile IniPttColor = new CIniFile(sPttColorPath);

            IniPttColor.Save("PttColor", "m_iHghtRgb     ", m_iHghtRgb);
        }

        public int GetHghtPosCnt()
        {
            return m_iHghtPosCnt;
        }

        public void LoadLastInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\LastInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);

            //Load
            IniLastInfo.Load("LAST WORK INFO", "Device", out m_sCrntDev);

            if (m_sCrntDev == "") m_sCrntDev = "NONE";
        }

        public void SetHghtValue(uint _uPstnNo, double _dValue)
        {
            //PttValue[_uPstnNo].dValue = _dValue;

            //UI에 접근 하기 위한 인보크
            if (lvHghtPos.Parent != null)
            {
                if (lvHghtPos.InvokeRequired)
                {
                    lvHghtPos.Invoke(new MethodInvoker(delegate()
                    {
                        // code for running in ui thread
                        for (int i = 0; i < GetHghtPosCnt(); i++)
                        {
                            if (lvHghtPos.SelectedItems[0].Index == i) lvHghtPos.Items[i].SubItems[(int)_uPstnNo].Text = _dValue.ToString();
                        }
                    }));
                }
                else
                {
                    for (int i = 0; i < GetHghtPosCnt(); i++)
                    {
                        if (lvHghtPos.SelectedItems[0].Index == i) lvHghtPos.Items[i].SubItems[(int)_uPstnNo].Text = _dValue.ToString();
                    }
                }
            }
        }


        //높이측정기 패턴 노드 리스트뷰 만드는부분
        public ListViewEx lvHghtPos;
        TextBox tbHghtEdit;
        public ListViewItem[] liHght = new ListViewItem[MAX_HGHT_CMD];//이거 노드카운트 수량만큼 만들어야함
        public void InitHghtPosView(Panel _pnBase)
        {
            lvHghtPos = new ListViewEx();
            //PttValue = new TPttValue[GetPosCnt()];

            lvHghtPos.Parent = _pnBase;
            lvHghtPos.Dock = DockStyle.Fill;

            lvHghtPos.Clear();
            lvHghtPos.View = View.Details;
            lvHghtPos.FullRowSelect = true;
            lvHghtPos.MultiSelect = true;
            lvHghtPos.GridLines = true;

            //셀높이 조절하기 편법.
            ImageList dummyImage = new ImageList();
            dummyImage.ImageSize = new System.Drawing.Size(1, 25);
            lvHghtPos.SmallImageList = dummyImage;

            lvHghtPos.Columns.Add("", 100, HorizontalAlignment.Left);
            lvHghtPos.Columns.Add("X Pos"  , 80, HorizontalAlignment.Left);
            lvHghtPos.Columns.Add("Y Pos"  , 80, HorizontalAlignment.Left);
            lvHghtPos.Columns.Add("Z Pos"  , 80, HorizontalAlignment.Left);
            lvHghtPos.Columns.Add("X Scale", 80, HorizontalAlignment.Left);
            lvHghtPos.Columns.Add("Y Scale", 80, HorizontalAlignment.Left);

            for (int i = 0; i < GetHghtPosCnt(); i++)
            {
                liHght[i] = new ListViewItem("NODE" + i.ToString());

                for (int j = 0; j < lvHghtPos.Columns.Count; j++)
                {
                    liHght[i].SubItems.Add("");
                }

                liHght[i].UseItemStyleForSubItems = false;
                lvHghtPos.Items.Add(liHght[i]);
            }

            var PropInput = lvHghtPos.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvHghtPos, true, null);

            tbHghtEdit = new TextBox();

            tbHghtEdit.Visible = false;
            tbHghtEdit.BorderStyle = BorderStyle.FixedSingle;
            tbHghtEdit.Tag = 0;
            tbHghtEdit.Leave += tbHghtEdit_Leave;

            tbHghtEdit.KeyDown += tbHghtEdit_KeyDown;

            if (GetHghtPosCnt() != 0)
            {
                lvHghtPos.AddEmbeddedControl(tbHghtEdit, 1, 0);
            }


            lvHghtPos.Click += new EventHandler(lvHghtPos_Click);
        }

        private void tbHghtEdit_Leave(object sender, EventArgs e)
        {
            //TextBox Tb = sender as TextBox;
            TextBox Tb = tbHghtEdit as TextBox;
            int iPosId = (int)Tb.Tag;

            Tb.Visible = false;

            double dVal;
            if (!double.TryParse(Tb.Text, out dVal))
            {
                return;
            }
            SetHghtValue((uint)iPosId, dVal);
        }

        private void tbHghtEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double dVal;
                int iPosId = (int)tbHghtEdit.Tag;
                tbHghtEdit.Visible = false;
                if (!double.TryParse(tbHghtEdit.Text, out dVal))
                {
                    return;
                }
                SetHghtValue((uint)iPosId, dVal);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                tbHghtEdit.Text = "";
                tbHghtEdit.Visible = false;
            }
        }

        private void lvHghtPos_Click(object sender, EventArgs e)
        {
            ListView Lv = sender as ListView;

            Point mousePos = Lv.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = Lv.HitTest(mousePos);
            if (hitTest.Item == null) return;
            int row = hitTest.Item.Index;
            int col = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);

            if (col == 1)
            {
                lvHghtPos.MoveEmbeddedControl(tbHghtEdit, 1, row); //이게 텍스트박스 옮기는거

                tbHghtEdit.Visible = true;
                tbHghtEdit.Text = GetHghtPosX(row).ToString();
                tbHghtEdit.Tag = col;
                tbHghtEdit.Select(); ;//Focus();

            }

            else if (col == 2)
            {
                lvHghtPos.MoveEmbeddedControl(tbHghtEdit, 2, row); //이게 텍스트박스 옮기는거

                tbHghtEdit.Visible = true;
                tbHghtEdit.Text = GetHghtPosY(row).ToString();
                tbHghtEdit.Tag = col;
                tbHghtEdit.Select(); ;//Focus();

            }

            else if (col == 3)
            {
                lvHghtPos.MoveEmbeddedControl(tbHghtEdit, 3, row); //이게 텍스트박스 옮기는거

                tbHghtEdit.Visible = true;
                tbHghtEdit.Text = GetHghtPosZ(row).ToString();
                tbHghtEdit.Tag = col;
                tbHghtEdit.Select(); ;//Focus();

            }

        }

        //NodeCount에 맞춰서 Row 넣었다 줄였다 하는부분
        public void SetHghtItem()
        {
            int iBfHghtCnt = lvHghtPos.Items.Count;

            //NodeCount 늘어날때
            if (GetHghtPosCnt() > iBfHghtCnt)
            {
                for (int i = iBfHghtCnt; i < GetHghtPosCnt(); i++)
                {
                    liHght[i] = new ListViewItem("NODE" + i.ToString());

                    for (int j = 0; j < lvHghtPos.Columns.Count; j++)
                    {
                        liHght[i].SubItems.Add("");
                    }

                    liHght[i].UseItemStyleForSubItems = false;
                    lvHghtPos.Items.Add(liHght[i]);
                }
            }

            //NodeCount 줄어들때
            else
            {
                for (int i = iBfHghtCnt; i >= GetHghtPosCnt(); i--)
                {
                    lvHghtPos.Items.Remove(liHght[i]);
                }
            }
        }

        public void ListViewRefresh()
        {
            //lvHghtPos.Refresh();
            lvHghtPos.Invalidate();
        }

    }
}
