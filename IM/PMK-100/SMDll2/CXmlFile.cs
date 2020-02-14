using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;


namespace SMDll2
{
    /// <summary>
    /// 상속해서 사용하는 버젼.
    /// </summary>
    public abstract class CXmlFile
    {
        public CXmlFile()
        {

        }

        /// <summary>
        /// 상속시 필히 구현
        /// </summary>
        /// <returns>각클래스별 저장루트및 파일명 리턴.</returns>
        public abstract string GetXmlFilePath();

        /// <summary>
        /// 해당 _sFileName으로 입력 받은 루트의 폴더생성.
        /// 다중루트 이면 모두 생성한다. 폴더만.
        /// </summary>
        /// <param name="_sFileName">생성하고자 하는 풀루트</param>
        /// <returns>생성 성공여부</returns>
        protected bool MakeFilePathFolder(string _sFileName)
        {
            string sFileFolderPath = Path.GetDirectoryName(_sFileName);

            DirectoryInfo _diPath = new DirectoryInfo(sFileFolderPath);

            // 해당 경로에 해당 하는 폴더가 없으면 만들어 줌 
            if (!_diPath.Exists)
            {
                try
                {
                    //System.IO.Directory.exist
                    //System.IO.Directory.CreateDirectory(_sFileName); 
                    _diPath.Create();
                }
                catch(Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    return false ;
                }
                return true ;
            }

            return true;
        }

        /// <summary>
        /// xml파일에서 로딩함.
        /// </summary>
        /// <param name="_sFileName">파일 풀루트</param>
        /// <returns>로딩한 객체를 반환.</returns>
        public object LoadXml(string _sFileName)
        {
            if (!MakeFilePathFolder(_sFileName)) return false;
            try
            {
                //using : 반드시 Dispose()를 호출해서 소멸해야 하는 객체에서 몇 가지 값을 읽은 다음 자동 페기.
                using (StreamReader _srReader = new System.IO.StreamReader(_sFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(this.GetType());
                    object oTemp = serializer.Deserialize(_srReader);
                    _srReader.Close();
                    return oTemp;//나중에 안되면 return serializer.Deserialize(_srReader);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 객체를 세이브함.
        /// </summary>
        /// <param name="_sFileName">파일 풀패스</param>
        /// <returns>저장 성공여부</returns>
        public bool SaveXml(string _sFileName)
        {
            if (!MakeFilePathFolder(_sFileName)) return false;

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
                System.Windows.Forms.MessageBox.Show(e.Message);

                return false;
            }

        }
    }

    /// <summary>
    /// 상속해서 사용하는 버젼.
    /// </summary>
    public class CXmlFile2
    {
        public CXmlFile2()
        {

        }




        /// <summary>
        /// 해당 _sFileName으로 입력 받은 루트의 폴더생성.
        /// 다중루트 이면 모두 생성한다. 폴더만.
        /// </summary>
        /// <param name="_sFileName">생성하고자 하는 풀루트</param>
        /// <returns>생성 성공여부</returns>
        protected bool MakeFilePathFolder(string _sFileName)
        {
            string sFileFolderPath = Path.GetDirectoryName(_sFileName);

            DirectoryInfo _diPath = new DirectoryInfo(sFileFolderPath);

            // 해당 경로에 해당 하는 폴더가 없으면 만들어 줌 
            if (!_diPath.Exists)
            {
                try
                {
                    //System.IO.Directory.exist
                    //System.IO.Directory.CreateDirectory(_sFileName); 
                    _diPath.Create();
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    return false;
                }
                return true;
            }

            return true;
        }

        /// <summary>
        /// xml파일에서 로딩함.
        /// </summary>
        /// <param name="_sFileName">파일 풀루트</param>
        /// <returns>로딩한 객체를 반환.</returns>
        public bool LoadXml(string _sFileName , ref object _oTrg)
        {
            if (!MakeFilePathFolder(_sFileName)) return false;
            try
            {
                //using : 반드시 Dispose()를 호출해서 소멸해야 하는 객체에서 몇 가지 값을 읽은 다음 자동 페기.
                using (StreamReader _srReader = new System.IO.StreamReader(_sFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(_oTrg.GetType());
                    _oTrg = serializer.Deserialize(_srReader);
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 객체를 세이브함.
        /// </summary>
        /// <param name="_sFileName">파일 풀패스</param>
        /// <returns>저장 성공여부</returns>
        public bool SaveXml(string _sFileName , ref object _oTrg)
        {
            if (!MakeFilePathFolder(_sFileName)) return false;

            try
            {
                using (StreamWriter _swWriter = new System.IO.StreamWriter(_sFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(_oTrg.GetType());
                    serializer.Serialize(_swWriter, _oTrg);
                    _swWriter.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);

                return false;
            }

        }
    }
}
