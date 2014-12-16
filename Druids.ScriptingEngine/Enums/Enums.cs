namespace Druids.ScriptingEngine.Enums
{
    public enum TargetObjectType
    {
        TempReferenceObject = -2, // Whatever is on the stack
        Self = -1, // The script self object
        ReferenceId = 0 // Reference id
    }

    public enum SupportedHostType
    {
        None = 0,
        Double = 1,
        String = 2,
        Int32 = 3,
        Int16 = 4,
        Int64 = 5,
        Byte = 6,
        Float = 7,
        Boolean = 8
    }

    public enum GossipType
    {
        None = 0,
        Number = 1, // Implemented as Double
        String = 2,
        ReferenceObject = 3,
        Action = 4,
        ReferenceModule = 5
    }

    public enum VariableScope
    {
        None = 0,
        Global = 1,
        Local = 2
    }

    public enum ScriptResult
    {
        Yield = 0,          // API needs something before it can continue
        Continue = 1,       // Go to the next node
        EndProcess = 2,     // Finished execution with errors
        Return = 3,         // ? Unsure (Possibly obsolete)
        Ok = 4,             // Finished Execution without errors
        ChangedNode = 5     // Jumped to a different execution node
    }

    public enum CommandNode
    {
        None = 0,
        ApiCall = 1,
        Conditional = 2,
        Wait = 3
    }

    public enum OperationType
    {
        None = 0,
        Operand = 1,
        Operator = 2,
        FunctionCall = 3,
        SystemCall = 4,
        Error = 5,
        PropertyCall = 6,
        MethodCall = 7,
        CreateAction = 8
    }

    public enum TokenDiscardPolicy
    {
        Keep = 0,
        Discard = 1
    }

    public enum OperatorAssociativity
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    public enum TokenType
    {
        None = 0,
        OpenCurlyBrace = 1,
        CloseCurlyBrace = 2,
        ParallelBlock = 3,
        If = 4,
        Else = 5,
        OpenBracket = 6,
        CloseBracket = 7,
        StringLiteral = 8,
        EndStatement = 9, // ;
        WaitStatement = 10,
        DecimalLiteral = 11,
        EvaluateExpression = 12,
        GlobalVariableAccess = 13,
        GlobalFlagAccess = 14,
        LocalVariableAccess = 15,
        LocalFlagAccess = 16,
        Decrement = 17,
        Increment = 18,
        IncrementAndAssign = 19,
        DecrementAndAssign = 20,
        //ObjectInstancePropertySetter = 21,
        //ObjectInstancePropertyGetter = 22,
        Comment = 23,
        Whitespace = 24,
        EndOfProgram = 25,
        HostFunctionCall = 26,
        Addition = 27,
        Subtract = 28,
        Multiply = 29,
        Divide = 30,
        Assignment = 31,
        Negation = 32,
        PowerOf = 33,
        UnaryMinus = 34, // Requires semantic analyser
        Modulo = 35,
        GreaterThan = 36,
        LessThan = 37,
        Equal = 38,
        NotEqual = 39,
        GreaterThanOrEqualTo = 40,
        LessThanOrEqualTo = 41,
        LogicalAnd = 42,
        LogicalOr = 43,
        FunctionArgumentSeperator = 44,
        TrueLiteral = 45,
        FalseLiteral = 46,
        GlobalStringAccess = 47,
        LocalStringAccess = 48,
        StringEquality = 49,
        StringInequality = 50,
        ReferenceObjectAccess = 51,
        StringStringConcatination = 52,
        StringDoubleConcatination = 53,
        DoubleStringConcatination = 54,
        Yeild = 55,
        Return = 56,
        Exit = 57,
        LocalVariableAccessEnum = 58,
        LocalFlagAccessEnum = 59,
        LocalStringAccessEnum = 60,
        GlobalVariableAccessEnum = 61,
        GlobalFlagAccessEnum = 62,
        GlobalStringAccessEnum = 63,
        ReferenceObjectAccessEnum = 64,
        ReferenceEquality = 65,
        ReferenceInEquality = 66,
        ReferencePropertyGet = 67,
        ReferencePropertySet = 68,
        Self = 69,
        MethodCall = 70,
        ReferenceAction = 71,
        ModuleAccess = 72,
        ModuleMethodCall = 73,
        ModuleAction = 74,
        ModulePropertyGet = 75,
        ModulePropertySet = 76,
        EventOnStart = 77,
        EventOnEnd = 78,
        EventOnRun = 79,
        EventOnInterrupt = 80,
        EventCustom = 81,
        EventDefault = 82,
        Event = 83,
        Enum = 84
    }
}