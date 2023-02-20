namespace ValidationMachine
{
    public enum VM_Status
    {
        OK,
        Not_Alpha,
        Not_Num,
        Not_Alpha_Num,
        Not_Equal,
        Not_Contains,
        Bad_Equal_Args,
        Bad_Contains_Args,
        No_Spec_Char,
        Too_Short,
        Bad_Min_Args,
        Too_Long,
        Bad_Max_Args,
        Invalid_Char,
        Bad_Char_Args,
        RegEx_Fail,
        Bad_RegEx_Args,
        Func_Fail,
        Bad_Func_Args
    }
}
