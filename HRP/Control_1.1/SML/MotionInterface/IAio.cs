
namespace MotionInterface
{
    public interface IAio
    {
        //Display용 모듈 순서
        /// <summary>
        /// 초기화함수
        /// </summary>
        /// <returns>성공했는지 실패했는지 여부</returns>
        bool Init();

        /// <summary>
        /// 닫기 함수.
        /// </summary>
        /// <returns>성공 했는지 실패했는지 여부</returns>
        bool Close();

        /// <summary>
        /// AIO번호에 따른 해당 모듈의 정보 리턴.
        /// </summary>
        /// <param name="_iNo">정보를 가져올 AIO번호</param>
        /// <param name="_iModuleNo">AIO번호에 따른 AIO모듈넘버</param>
        /// <param name="_iModuleNoDp">AIO번호에 따른 Display용 모듈넘버</param>
        /// <param name="_iOffset">해당 모듈에서의 AIO번호.</param>
        /// <returns>성공 여부.</returns>
        bool GetInfoInput (int _iNo , out int _iModuleNo ,out int _iModuleNoDp ,out int _iOffset);
        /// <summary>
        /// AIO번호에 따른 해당 모듈의 정보 리턴.
        /// </summary>
        /// <param name="_iNo">정보를 가져올 AIO번호</param>
        /// <param name="_iModuleNo">AIO번호에 따른 AIO모듈넘버</param>
        /// <param name="_iModuleNoDp">AIO번호에 따른 Display용 모듈넘버</param>
        /// <param name="_iOffset">해당 모듈에서의 AIO번호.</param>
        /// <returns>성공 여부.</returns>
        bool GetInfoOutput(int _iNo , out int _iModuleNo ,out int _iModuleNoDp ,out int _iOffset);

        /// <summary>
        /// AIO출력(지정한 출력 채널에 입력한값을 전압으로 출력 합니다)
        /// </summary>
        /// <param name="_iNo">AIO번호</param>
        /// <param name="_bOn">true=ON , false=OFF</param>
        /// <returns>성공여부</returns>
        bool SetOut (int _iNo , double _dVal) ;
        /// <summary>
        /// AIO 출력 상태 가져오기(지정한 출력 채널에 출력하고 있는 전압 값을 확인합니다)
        /// </summary>
        /// <param name="_iNo">AIO번호</param>
        /// <returns>AIO상태</returns>
        double GetOut (int _iNo             ) ;
        /// <summary>
        /// AIO입력 상태 가져오기.
        /// </summary>
        /// <param name="_iNo">AIO번호</param>
        /// <param name="_bDigit">아날로그값으로 받을지 디지털값으로 받을지</param>
        /// <returns>AIO상태</returns>
        double GetIn  (int _iNo , bool _bDigit = false) ;

    }
}
