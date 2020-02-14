
namespace MotionInterface
{
    public interface IDio
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
        /// IO번호에 따른 해당 모듈의 정보 리턴.
        /// </summary>
        /// <param name="_iNo">정보를 가져올 IO번호</param>
        /// <param name="_iModuleNo">IO번호에 따른 IO모듈넘버</param>
        /// <param name="_iModuleNoDp">IO번호에 따른 Display용 모듈넘버</param>
        /// <param name="_iOffset">해당 모듈에서의 IO번호.</param>
        /// <returns>성공 여부.</returns>
        bool GetInfoInput (int _iNo , out int _iModuleNo ,out int _iModuleNoDp ,out int _iOffset);
        /// <summary>
        /// IO번호에 따른 해당 모듈의 정보 리턴.
        /// </summary>
        /// <param name="_iNo">정보를 가져올 IO번호</param>
        /// <param name="_iModuleNo">IO번호에 따른 IO모듈넘버</param>
        /// <param name="_iModuleNoDp">IO번호에 따른 Display용 모듈넘버</param>
        /// <param name="_iOffset">해당 모듈에서의 IO번호.</param>
        /// <returns>성공 여부.</returns>
        bool GetInfoOutput(int _iNo , out int _iModuleNo ,out int _iModuleNoDp ,out int _iOffset);

        /// <summary>
        /// IO출력
        /// </summary>
        /// <param name="_iNo">IO번호</param>
        /// <param name="_bOn">true=ON , false=OFF</param>
        /// <returns>성공여부</returns>
        bool SetOut (int _iNo , bool  _bOn) ;
        /// <summary>
        /// IO 출력 상태 가져오기.
        /// </summary>
        /// <param name="_iNo">IO번호</param>
        /// <returns>IO상태</returns>
        bool GetOut (int _iNo             ) ;
        /// <summary>
        /// IO입력 상태 가져오기.
        /// </summary>
        /// <param name="_iNo">IO번호</param>
        /// <returns>IO상태</returns>
        bool GetIn  (int _iNo             ) ;
    }
}
