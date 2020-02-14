using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using COMMON;

namespace VDll
{
    public struct TObj
    {
        public string  sCategory ;
        public string  sName     ;
        public Type    sType     ;
        public string  sVal      ;
        public string  sDesc     ;
    };

    public class CCustomProperty
    {
        //public object sValue ;
        public string Category { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public object DefaultValue { get; set; }
        //public object Value { get { return sValue; } set { sValue = value;} }
        public object Value { get ; set ; }
        Type type;
    
        public Type Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                if (value == typeof(string))
                    DefaultValue = "";
                else
                    DefaultValue = Activator.CreateInstance(value);
            }              
        }   
    }
    /*
     
     */
    class CMultiObjectSelector
    {
        static public void GetData<T>(ref T _oStruct, ref List<CCustomProperty> _sList, string _sNewCategory = "")
        {
            if (_oStruct == null) return;
            Type type = _oStruct.GetType();
            string sKey;
            int num1, num2;

            FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo[] prop_info_array = type.GetProperties();
            
            for (int i = 0; i < f.Length; i++)
            {
                CategoryAttribute[] attributes = (CategoryAttribute[])prop_info_array[i].GetCustomAttributes(typeof(CategoryAttribute), false);
                TObj sObj = new TObj();
                sObj.sCategory = _sNewCategory == "" ? attributes[0].Category : _sNewCategory;
                sObj.sType = f[i].FieldType;
                sKey = f[i].Name + ";";
                num1 = sKey.IndexOf("<");
                num2 = sKey.IndexOf(">");
                sObj.sName = sKey.Substring(num1 + 1, num2 - 1);
                sObj.sVal = "";

                dynamic dynamic ;
                if (f[i].GetValue(_oStruct) != null) dynamic = f[i].GetValue(_oStruct);
                else                                 dynamic = "";
                
                _sList.Add(new CCustomProperty { Category = sObj.sCategory, Name = sObj.sName, Type = sObj.sType, Desc = sObj.sDesc, Value = dynamic });
                
            }
        }

        private static object[] GetCustomAttributes(Type type, bool p)
        {
            throw new NotImplementedException();
        }
        
        static public void SetData<T>(ref T _oStruct, TObj[] _sObj)
        {
            Type type = _oStruct.GetType();
        
            FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < f.Length; i++)
            {
                     if(f[i].FieldType          == typeof(bool  ))f[i].SetValueDirect(__makeref(_oStruct), CIniFile.StrToBoolDef  (_sObj[i].sVal , false));
                else if(f[i].FieldType          == typeof(int   ))f[i].SetValueDirect(__makeref(_oStruct), CIniFile.StrToIntDef   (_sObj[i].sVal , 0    ));
                else if(f[i].FieldType          == typeof(uint  ))f[i].SetValueDirect(__makeref(_oStruct), CIniFile.StrToUintDef  (_sObj[i].sVal , 0    ));
                else if(f[i].FieldType          == typeof(double))f[i].SetValueDirect(__makeref(_oStruct), CIniFile.StrToDoubleDef(_sObj[i].sVal , 0.0  ));
                else if(f[i].FieldType          == typeof(string))f[i].SetValueDirect(__makeref(_oStruct), _sObj[i].sVal                                 );
                else if(f[i].FieldType.BaseType == typeof(Enum  ))f[i].SetValueDirect(__makeref(_oStruct), Enum.Parse(f[i].FieldType , _sObj[i].sVal.ToString()));
            }
        }

        //추가
        static public bool SetProperty<T>(ref T _oStruct, object _obj, string _sCategory )
        {

            Type type = _oStruct.GetType();
            FieldInfo[] f = _oStruct.GetType().GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);

            CustomObjectType CustumObjType = _obj as CustomObjectType;
            if(CustumObjType == null) return false ;
            PropertyInfo[] prop_info_array = type.GetProperties();                        
            for(int j=0; j<CustumObjType.props.Count; j++)
            {
                for (int i = 0; i < f.Length; i++)
                {
                    CategoryAttribute[] attributes = (CategoryAttribute[]) prop_info_array[i].GetCustomAttributes(typeof(CategoryAttribute), false);
                    if(_sCategory == CustumObjType.props[j].Category)
                    {
                        if (f[i].Name.Contains(CustumObjType.props[j].Name))
                        {
                            string sTemp=CustumObjType.props[j].Value.ToString();
                            //int    iTemp;
                            //int.TryParse(sTemp,out iTemp);

                                 if(f[i].FieldType == typeof(bool  ))                    f[i].SetValueDirect(__makeref(_oStruct), CConfig.StrToBoolDef    (sTemp,false));
                            else if(f[i].FieldType == typeof(int   ))                    f[i].SetValueDirect(__makeref(_oStruct), CConfig.StrToIntDef     (sTemp,0    ));
                            else if(f[i].FieldType == typeof(uint  ))                    f[i].SetValueDirect(__makeref(_oStruct), CConfig.StrToUintDef    (sTemp,0    ));
                            else if(f[i].FieldType == typeof(double))                    f[i].SetValueDirect(__makeref(_oStruct), CConfig.StrToDoubleDef  (sTemp,0.0  ));
                            else if(f[i].FieldType == typeof(string))                    f[i].SetValueDirect(__makeref(_oStruct),                          sTemp       );
                            else if(f[i].FieldType.BaseType == typeof(Enum))             f[i].SetValueDirect(__makeref(_oStruct), Enum.Parse(f[i].FieldType, sTemp    ));
                        }
                    }
                    
                }
            }

            return true;
        }

        [TypeConverter(typeof(CustomObjectType.CustomObjectConverter))]
        public class CustomObjectType : TypeConverter 
        {
              //https://stackoverflow.com/questions/3491556/how-to-display-a-dynamic-object-in-property-grid
              


            //[Category("Standard")]
            //public string Name { get; set; }
            public List<CCustomProperty> props = new List<CCustomProperty>();
            //[Browsable(true)]
            //public List<CustomProperty> Properties { get { return props; } }
        
            private Dictionary<string, object> values = new Dictionary<string, object>(); //values 요기에 아무값도 없다. 그게 이넘 안보이는 이유.
        
            public object this[string name]
            {
                get { object val; values.TryGetValue(name, out val); return val; }
                set { values.Remove(name); }
            }
        
            private class CustomObjectConverter : ExpandableObjectConverter
            {
                public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
                {
                    var stdProps = base.GetProperties(context, value, attributes);
                    CustomObjectType obj = value as CustomObjectType;
                    List<CCustomProperty> customProps = obj == null ? null : obj.props;
                    PropertyDescriptor[] props = new PropertyDescriptor[stdProps.Count + (customProps == null ? 0 : customProps.Count)];
                    stdProps.CopyTo(props, 0);
                    if (customProps != null)
                    {
                        int index = stdProps.Count;
                        foreach (CCustomProperty prop in customProps)
                        {
                            props[index++] = new CustomPropertyDescriptor(prop);
                        }
                    }
                    return new PropertyDescriptorCollection(props);
                }
            }
            private class CustomPropertyDescriptor : PropertyDescriptor
            {
                //private readonly CCustomProperty prop;
                private CCustomProperty prop;
                public CustomPropertyDescriptor(CCustomProperty prop) : base(prop.Name, null)
                {
                    this.prop = prop;
                }
                //public override string Category { get { return "Dynamic"; } }
                public override string Category { get { return prop.Category; }}
                public override string Description { get { return prop.Desc; } }
                public override string Name { get { return prop.Name; } }
                public override bool ShouldSerializeValue(object component) { return ((CustomObjectType)component)[prop.Name] != null; } //여기서 부터....보자
                public override void ResetValue(object component) { ((CustomObjectType)component)[prop.Name] = null; }
                public override bool IsReadOnly { get { return false; } }
                public override Type PropertyType { get { return prop.Type; } }
                public override bool CanResetValue(object component) { return true; }
                public override Type ComponentType { get { return typeof(CustomObjectType); } }
                public override void SetValue(object component, object value) { prop.Value = value;}
                    //((CustomProperty)component).DefaultValue = value; }
                public override object GetValue(object component) { return prop.Value;}
                //    return ((CustomProperty)component).DefaultValue ?? prop.DefaultValue; }
            }
        }
    }
}
