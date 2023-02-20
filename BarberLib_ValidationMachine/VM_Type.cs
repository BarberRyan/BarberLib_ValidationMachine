namespace ValidationMachine
{
    public enum VM_Type
    {
        //Is text only letters?
        Alpha,
        //Is text only numbers?
        Num,
        //Is text alphanumeric?
        Alpha_Num,
        //Is text equal to input?
        Equal,
        //Does text contain a specified string?
        Contains,
        //Does text contain special chars?
        Spec_Char,
        //Is text above the minimum length?
        Min_Length,
        //Is text below the maximum length?
        Max_Length,
        //Does text omit specified chars?
        Omit_Char,
        //Does text match Regex pattern?
        RegEx,
        //Does the provided function return true?
        Func
    }
}
