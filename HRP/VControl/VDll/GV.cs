using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDll.Pakage;
//yttt
//sdfksdjf
namespace VDll
{

    public class CObject
    {
        protected string sName ;
        public string Name 
        {
            get{return sName ;}     
        }

        public CObject(string _sName)
        {
            sName = _sName ;
        }
    }

    public class CRegisteredObject : CObject
    {
        static Dictionary<string,CRegisteredObject> dtNameObjects = new Dictionary<string,CRegisteredObject>();
        static CRegisteredObject FindObject(string _sName)
        {
            return dtNameObjects[_sName];
        }
        public CRegisteredObject(string _sName) : base(_sName)
        {
            sName = _sName ;
            try
            {
                dtNameObjects.Add(sName , this);
            }
            catch(Exception _e)
            {
                MessageBox.Show(_e.ToString());
            }
        }
        ~CRegisteredObject()
        {
            dtNameObjects.Remove(sName);
        }
    }   

    public class CErrorObject : CObject
    {
        public CErrorObject(string _sName):base(_sName)
        {    
            
        }

        protected string sError ;
        public string Error 
        {
            get{string sRet = sError; sError = ""; return sRet ;}
        }
    }



    class GV
    {
        static public VL.TPara          Para;

        //전체 적용 되는 오토런.
        static public bool              AutoRun ;

        //현재 디바이스.
        static public string            DeviceName   ;//{get{ return sDeviceName;} set{sDeviceName = value;} }

        //전체 연관 있는 Value table.
        static public CVarTable         GlobalTables ;
        static public CVarTable      [] VarTables    ;

        //Pkg 타입 리스트
        static public CDynamic<IPkg>    DynamicPkgMaker;

        //하드웨어 카메라 갯수만큼 있음.
        static public Camera.ICamera [] Cameras    ;

        //하드웨어 조명 갯수만큼 있음.
        static public Light.ILight   [] Lights     ;

        //비전클래스 생성 갯수 만큼 있는 쎄트.
        static public Vision         [] Visions    ;
        //static public TVisnStat      [] VisnStats  ;
        
        static public FormVision     [] FrmVisions ;

        public delegate void FPaintCallback();
        static public FPaintCallback [] PaintCallbacks ;

        static public Calibration    [] calibration ;
        //화면 뿌릴때나 이미지메모리 접근때나.. 사용.
        //일관성 있게 구성 하고 싶으나 그것이 잘 안되어 쓸때 잘 써야함.
        static public object         [] ImageLock      ; //

        

    
        //FormRecipe.save_re
        //Run.CClear       += new CRun.Chart_Clear       (FormRecipe.Chart_Clear    );

        
        //static public List<string> lsErrors    ;
    }
}
