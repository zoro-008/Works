using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    class Constants
    {
        public const char START_ARG     = '(';
        public const char END_ARG       = ')';
        public const char END_LINE      = '\n';
        public const char NEXT_ARG      = ',';
        public const char QUOTE         = '"';
        public const char START_GROUP   = '{';
        public const char END_GROUP     = '}';
        public const char END_STATEMENT = ';';
        public const char VAR_PREFIX    = '$';
        
        public const string IF          = "if";
        public const string ELSE        = "else";
        public const string ELSE_IF     = "elif";
        public const string WHILE       = "while";
        public const string BREAK       = "break";
        public const string CONTINUE    = "continue";
        public const string COMMENT     = "//";
        
        public const string ABS         = "abs";
        public const string APPEND      = "append";
        public const string CD          = "cd";
        public const string CD__        = "cd..";
        public const string DIR         = "dir";
        public const string ENV         = "env";
        public const string EXP         = "exp";
        public const string FINDFILES   = "findfiles";
        public const string FINDSTR     = "findstr";
        public const string INDEX_OF    = "indexof";
        public const string KILL        = "kill";
        public const string PI          = "pi";
        public const string POW         = "pow";
        public const string PRINT       = "print";
        public const string PSINFO      = "psinfo";
        public const string PSTIME      = "pstime";
        public const string PWD         = "pwd";
        public const string RUN         = "run";
        public const string SETENV      = "setenv";
        public const string SET         = "set";
        public const string SIN         = "sin";
        public const string SIZE        = "size";
        public const string SQRT        = "sqrt";
        public const string SUBSTR      = "substr";
        public const string TOLOWER     = "tolower";
        public const string TOUPPER     = "toupper";
        
        public static char[] NEXT_ARG_ARRAY = NEXT_ARG.ToString().ToCharArray();
        public static char[] END_ARG_ARRAY  = END_ARG.ToString().ToCharArray();
        public static char[] END_LINE_ARRAY = END_LINE.ToString().ToCharArray();
        public static char[] QUOTE_ARRAY    = QUOTE.ToString().ToCharArray();
        
        public static char[] COMPARE_ARRAY     = "<>=)".ToCharArray();
        public static char[] IF_ARG_ARRAY      = "&|)".ToCharArray();
        public static char[] END_PARSE_ARRAY   = ";)}\n".ToCharArray();
        public static char[] NEXT_OR_END_ARRAY = { NEXT_ARG, END_ARG, END_STATEMENT };
        
        public static char[] TOKEN_SEPARATION  = ("<>=+-*/&^|!\n\t " + START_ARG + END_ARG +
                             START_GROUP + NEXT_ARG + END_STATEMENT).ToCharArray();

        //추가
        //grammar
        public const string TRUE                 = "true" ;
        public const string FALSE                = "false";

        //Motion
        public const string MOTOR_MOVE           = "motor_move"          ;
        public const string MOTOR_MOVEPOS        = "motor_movepos"       ;
        public const string MOTOR_CHECKSTOP      = "motor_checkstop"     ;
        public const string MOTOR_CHECKSTOPPOS   = "motor_checkstoppos"  ;
        public const string MOTOR_STOP           = "motor_stop"          ;
        public const string MOTOR_SERVO          = "motor_servo"         ;
        public const string MOTOR_HOME           = "motor_home"          ;
        public const string MOTOR_CHECKHOME      = "motor_checkhome"     ;

        //Cylinder
        public const string CYLINDER_MOVE        = "cylinder_checkstop"  ;
        public const string CYLINDER_CHECKSTOP   = "cylinder_checkstop"  ;
        
        //IO
        public const string IO_SETY              = "io_sety"             ;
        public const string IO_GETX              = "io_getx"             ;
                                                                         
        //Sequence                                                       
        //public const string CYCLE                = "cycle"               ;
        //public const string CYCLE_DIRECT         = "cycle_direct"        ;
        //public const string PART                 = "part"                ;
        //public const string PART_DIRECT          = "part_direct"         ;
        public const string RETURN               = "return"              ;
        public const string END                  = "end"                 ;
        public const string STOPWATCH            = "stopwatch"           ; public const int STOPWATCH_MAX = 20 ;
        //public const string USING                = "using"               ;
                                                 
        //Data                                   
        public const string DATA_SET             = "data_set"            ;
        public const string DATA_GET             = "data_get"            ;

        //Error                                   
        public const string ERROR_SET            = "error"               ;

        //Position
        public const string POSITION_GET         = "position_get"         ;
                                                                         
        //Option                                                         
        public const string OPTION_DEVICE        = "option_device"       ; public const int OPTION_DEVICE_MAX   = 20 ;
        public const string OPTION_COMMON        = "option_common"       ; public const int OPTION_COMMON_MAX   = 20 ;

        public static bool LOG   = true ;
        public static bool CHECK = false;
    }
}
