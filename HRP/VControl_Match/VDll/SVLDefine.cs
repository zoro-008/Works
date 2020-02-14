namespace VDll
{
    public enum ECameraType : uint
    {
        Neptune  = 0,
        MAX_CAM_SEL
    };

    public enum ELightType : uint
    {
        Deakyum  = 0 ,
        MAX_AIO_SEL
    };

    public enum ELevel : uint
    {
        LogOff   = 0,
        Operator    ,
        Engineer    ,
        Master      ,
        Control     ,

        MAX_LEVEL
    }

    public enum ESeqStat : uint
    {
        Init = 0,
        Warning ,
        Error   ,
        Running ,
        Stop    ,
        RunWarn ,
        WorkEnd ,
        Manual  , //AP텍 장비에서 추가
        MAX_SEQ_STAT
    }
}