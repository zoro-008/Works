namespace SMDll2
{

    public enum EN_LEVEL : uint
    {
        lvOperator = 0,
        lvEngineer    ,
        lvMaster      ,
        lvContol      ,

        MAX_LEVEL
    }

    public enum EN_SEQ_STAT : uint
    {
        ssInit = 0,
        ssWarning ,
        ssError   ,
        ssRunning ,
        ssStop    ,
        ssMaint   ,
        ssRunWarn ,
        ssWorkEnd ,

        MAX_SEQ_STAT
    }

    public enum EN_LAN_SEL : uint
    {
        lsKorean  = 0,
        lsEnglish    ,
        lsChinese    ,

        MAX_LAN_SEL
    };

    public enum EN_MOTR_SEL : uint
    {
        msAXL  = 0,
        msNMC2 = 1,
        msEMCL = 2,
       

        MAX_MOTR_SEL
    };

    public enum EN_DIO_SEL : uint
    {
        dsAXL  = 0 ,
        dsNMC2 = 1 ,

        MAX_DIO_SEL
    };

    public enum EN_CYLINDER_POS : uint
    {
        cpBwd = 0 ,
        cpFwd = 1 
    };

    //Cylinder , Motr (Negative , Bwd) ,(Positive,Fwd); 
    public enum EN_MOVE_DIRECTION : uint
    {
        mdLR    = 0 , //정면에서   봤을때 Left 가 - Bwd Right가 + 
        mdRL        , //정면에서   봤을때 Right가 - Left 가 +
        mdBF        , //정면에서   봤을때 Bwd  가 - Fwd  가 +
        mdFB        , //정면에서   봤을때 Fwd  가 - Bwd  가 +
        mdUD        , //정면에서   봤을때 Up   가 - Down 가 +
        mdDU        , //정면에서   봤을때 Down 가 - Up   가 +
        mdCA        , //회전축에서 봤을때 Clock가 - AntiC가 +
        mdAC        , //회전축에서 봤을때 AntiC가 - Clock가 +
        MAX_MOVE_DIRECTION
    };

    public enum EClickPos : uint
    {
        cpNone,
        cpLeftTop,
        cpTop,
        cpRightTop,
        cpRight,
        cpRightBottom,
        cpBottom,
        cpLeftBottom,
        cpLeft,
        cpMove,
    };

    public enum EN_SEQ_STEP : uint
    {
        scIdle = 0,
        scToStartCon = 10,
        scToStart = 11,
        scRun = 12,
        scToStopCon = 13,
        scToStop = 14,

        MAX_SEQ_CYCLE
    };







   














    
    
}