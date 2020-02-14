using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace VDll.Light
{
    class CDeakyum : CErrorObject , ILight
    {
        [Serializable ,TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUPara
        {   
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Ch1" )] public uint Ch1 {get;set;} 
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Ch2" )] public uint Ch2 {get;set;} 
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Ch3" )] public uint Ch3 {get;set;} 
        } 
        [Serializable ,TypeConverter(typeof(ExpandableObjectConverter))]
        public class CPara
        {   
        //    [CategoryAttribute("Para" ), DisplayNameAttribute("PhysicalAdd" )]public int PhysicalAdd{get{return PhysicalAdd;}set{if(value < 0)value=0 ;  }}  
        } 

        public CDeakyum(string _sName) : base(_sName)
        {

        }

        #region ILight
        /*
        bool   Init();
        bool   Close();        
        bool   LoadSavePara(bool _bLoad , string _sParaFilePath);
        bool   ApplyPara(object _UserPara);
        bool   ShowSettingDialog();
        string GetError();
        Type   GetParaType();
         */
        public bool Init()
        {
            return true ;
        }
        public bool Close()
        {
            return true ;
        }


        public bool LoadSavePara(bool _bLoad , string _sParaFilePath)
        {
            return false ;
        }
        public bool ApplyPara(object _UserPara)
        {
            return false ;
        }
        public bool ShowSettingDialog()
        {
            return false ;
        }
        public string GetError()
        {
            return "ssss";
        }

        public Type GetParaType()
        {
            return typeof(CUPara) ;
        }
        #endregion
    }
}
