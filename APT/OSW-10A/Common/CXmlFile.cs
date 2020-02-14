using System;
using System.IO;
using System.Xml.Serialization;


namespace COMMON
{
    static class CFolderMaker
    {
        /// <summary>
        /// 해당 _sFileName으로 입력 받은 루트의 폴더생성.
        /// 다중루트 이면 모두 생성한다. 폴더만.
        /// </summary>
        /// <param name="_sFileName">생성하고자 하는 풀루트</param>
        /// <returns>생성 성공여부</returns>
        public static bool MakeFilePathFolder(string _sFileName)
        {
            string sFileFolderPath = Path.GetDirectoryName(_sFileName);

            DirectoryInfo _diPath = new DirectoryInfo(sFileFolderPath);

            // 해당 경로에 해당 하는 폴더가 없으면 만들어 줌 
            if(!_diPath.Exists)
            {
                try
                {
                    _diPath.Create();
                }
                catch(Exception e)
                {

                    Log.ShowMessage("Exception",e.Message);
                    throw new FileNotFoundException(e.Message);

                }
                return true;
            }

            return true;
        }


    }
 

    /*
    Static 메서드는 인스턴스 메서드와는 달리 클래스로부터 객체를 생성하지 않고 직접 [클래스명.메서드명] 형식으로 호출하는 메서드이다. \
    이 메서드는 메서드 앞에 static 이라는 C# 키워드를 적어주며, 메서드 내부에서 클래스 인스턴스 객체 멤버를 참조해서는 안된다. 
    이 static 메서드는 인스턴스 객체로부터 호출될 수 없으며, 반드시 클래스명과 함께 사용된다.   
    */
    public static class CXml
    {
        /// <summary>
        /// 클래스 객체를 XML파일로 저장한다.
        /// </summary>
        /// <typeparam name="T">클래스 형태의 객체만 올수 있다.</typeparam>
        /// <param name="_sFileName">저장할 파일의 패스및 화일명</param>
        /// <param name="_Obj">저장할 클래스 객체</param>
        /// <returns>루트를 모두 만들지 못하면 false</returns>
        public static  bool SaveXml<T>(string _sFileName,ref T _Obj) where T:class
        {
            if(!CFolderMaker.MakeFilePathFolder(_sFileName)) return false;

            using(StreamWriter Writer = new StreamWriter(_sFileName))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                Serializer.Serialize(Writer,_Obj);
                Writer.Close();
            }
            return true ;
        }

        /// <summary>
        /// 클래스 객체를 XML에서 로딩한다.
        /// </summary>
        /// <typeparam name="T">클래스여야만 된다.</typeparam>
        /// <param name="_sFileName">파일 풀패스</param>
        /// <param name="_Obj">로딩할 객체</param>
        /// <returns>파일이 없으면 false</returns>
        public static bool LoadXml<T>(string _sFileName,ref T _Obj) where T:class
        {

            if (!CFolderMaker.MakeFilePathFolder(_sFileName)) return false;
            if (!File.Exists(_sFileName))
            {
                SaveXml<T>(_sFileName, ref _Obj);
                return false;
            }
            if (!File.Exists(_sFileName))
            {
                SaveXml<T>(_sFileName, ref _Obj);
                return false;
            }

            using(StreamReader Reader = new StreamReader(_sFileName))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                _Obj = (T)Serializer.Deserialize(Reader);
                //Serializer.Serialize(Reader,_Obj);
                Reader.Close();
            }
            return true;
        }


        /// <summary>
        /// xml파일에서 로딩함.
        /// </summary>
        /// <param name="_sFileName">파일 풀루트</param>
        /// <returns>로딩한 객체를 반환.</returns>
        public static bool LoadXml(string _sFileName, ref object _Obj)
        {    
            if (!CFolderMaker.MakeFilePathFolder(_sFileName)) return false;
            if (!File.Exists(_sFileName))
            {
                SaveXml(_sFileName, ref _Obj);
                return false;
            }
            try
            {
                //using : 반드시 Dispose()를 호출해서 소멸해야 하는 객체에서 몇 가지 값을 읽은 다음 자동 페기.
                using (StreamReader _srReader = new System.IO.StreamReader(_sFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(_Obj.GetType());
                    _Obj = serializer.Deserialize(_srReader);//박싱이라 크면 안좋을듯 참조 대비 20배느림.
                    _srReader.Close();
                    return true;//나중에 안되면 return serializer.Deserialize(_srReader);
                }
            }
            catch (Exception e)
            {

                Log.ShowMessage("Exception", e.Message);
                throw new FileNotFoundException(e.Message);
            }

            return true;
        }

        /// <summary>
        /// 객체를 세이브함.
        /// </summary>
        /// <param name="_sFileName">파일 풀패스</param>
        /// <returns>저장 성공여부</returns>
        public static bool SaveXml(string _sFileName, ref object _Obj)
        {
            if (!CFolderMaker.MakeFilePathFolder(_sFileName)) return false;

            try
            {
                using (StreamWriter _swWriter = new System.IO.StreamWriter(_sFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(_Obj.GetType());
                    serializer.Serialize(_swWriter, _Obj);
                    _swWriter.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.ShowMessage("Exception", e.Message);

                //return false;
                throw new FileNotFoundException(e.Message);
            }

        }
    }



    /// <summary>
    /// 상속해서 사용하는 버젼.
    /// </summary>
    public abstract class CXmlObject
    {
        public CXmlObject()
        {

        }
        /// <summary>
        /// xml파일에서 로딩함.
        /// </summary>
        /// <param name="_sFileName">파일 풀루트</param>
        /// <returns>로딩한 객체를 반환.</returns>
        public bool LoadXml(string _sFileName)
        {
            if (!CFolderMaker.MakeFilePathFolder(_sFileName)) return false;
            if (!File.Exists(_sFileName))
            {
                SaveXml(_sFileName);
                return false;
            }

            try
            {
                //using : 반드시 Dispose()를 호출해서 소멸해야 하는 객체에서 몇 가지 값을 읽은 다음 자동 페기.
                using (StreamReader _srReader = new System.IO.StreamReader(_sFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(this.GetType());

                    object oTemp = this ;
                    oTemp = serializer.Deserialize(_srReader);//박싱이라 크면 안좋을듯 참조 대비 20배느림.
                    _srReader.Close();
                    return true;//나중에 안되면 return serializer.Deserialize(_srReader);
                }
            }
            catch (Exception e)
            {
               
                Log.ShowMessage("Exception", e.Message);
                throw new FileNotFoundException(e.Message);
            }
        }

        /// <summary>
        /// 객체를 세이브함.
        /// </summary>
        /// <param name="_sFileName">파일 풀패스</param>
        /// <returns>저장 성공여부</returns>
        public bool SaveXml(string _sFileName)
        {
            if (!CFolderMaker.MakeFilePathFolder(_sFileName)) return false;

            try
            {
                using (StreamWriter _swWriter = new System.IO.StreamWriter(_sFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(_swWriter, this);
                    _swWriter.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.ShowMessage("Exception", e.Message);

                //return false;
                throw new FileNotFoundException(e.Message);
            }

        }
    }


}
