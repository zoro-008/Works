using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VDll.Pakage
{
    public class CDynamic<T> where T : class
    {
        private static Dictionary<string,Type> GetClassType()
        {
            Type ParentType = typeof(T);
            Dictionary<string,Type> dtPkgTypes = new Dictionary<string,Type>();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {

                //IPkg는 빼고 상속받은놈들 넣는다.
                if(t!=ParentType && ParentType.IsAssignableFrom(t)) 
                {
                    dtPkgTypes.Add(t.Name , t);
                }
            }
            return dtPkgTypes;
        }

        Dictionary<string,Type> ClassTypes ;
        public CDynamic()
        {
            ClassTypes = GetClassType();
        }

        public Dictionary<string,Type> Types {get{return ClassTypes;}}

        public T New(string _sClassName , object [] _Paras )
        {
            Type t ;
            T Ret = null;
            try{
                ClassTypes.TryGetValue(_sClassName , out t);
                Ret = Activator.CreateInstance(t , _Paras) as T;
            }
            catch(Exception _e)
            {
                //throw _e ;
            }
            finally
            {
                
            }
            return Ret ;
        }
    }    
}
