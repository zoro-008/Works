﻿namespace SML2
{

    public enum EN_LEVEL : uint
    {
        Operator = 0,
        Engineer    ,
        Master      ,
        Contol      ,

        MAX_LEVEL
    }

    public enum EN_SEQ_STAT : uint
    {
        Init = 0,
        Warning ,
        Error   ,
        Running ,
        Stop    ,
        Maint   ,
        RunWarn ,
        WorkEnd ,

        MAX_SEQ_STAT
    }

    public enum EN_LAN_SEL : uint
    {
        Korean  = 0,
        English    ,
        Chinese    ,

        MAX_LAN_SEL
    };

    public enum EN_MOTR_SEL : uint
    {
        AXL  = 0,
        NMC2 = 1,
        EMCL = 2,
       

        MAX_MOTR_SEL
    };

    public enum EN_DIO_SEL : uint
    {
        AXL  = 0 ,
        NMC2 = 1 ,

        MAX_DIO_SEL
    };

    public enum EN_CYLINDER_POS : uint
    {
        Bwd = 0 ,
        Fwd = 1 
    };

    //Cylinder , Motr (Negative , Bwd) ,(Positive,Fwd); 
    public enum EN_MOVE_DIRECTION : uint
    {
        LR    = 0 , //정면에서   봤을때 Left 가 - Bwd Right가 + 
        RL        , //정면에서   봤을때 Right가 - Left 가 +
        BF        , //정면에서   봤을때 Bwd  가 - Fwd  가 +
        FB        , //정면에서   봤을때 Fwd  가 - Bwd  가 +
        UD        , //정면에서   봤을때 Up   가 - Down 가 +
        DU        , //정면에서   봤을때 Down 가 - Up   가 +
        CA        , //회전축에서 봤을때 Clock가 - AntiC가 +
        AC        , //회전축에서 봤을때 AntiC가 - Clock가 +
        MAX_MOVE_DIRECTION
    };

    public enum EClickPos : uint
    {
        None,
        LeftTop,
        Top,
        RightTop,
        Right,
        RightBottom,
        Bottom,
        LeftBottom,
        Left,
        Move,
    };

    public enum EN_SEQ_STEP : uint
    {
        Idle       = 0,
        ToStartCon = 10,
        ToStart    = 11,
        Run        = 12,
        ToStopCon  = 13,
        ToStop     = 14,

        MAX_SEQ_CYCLE
    };







   














    
    
}