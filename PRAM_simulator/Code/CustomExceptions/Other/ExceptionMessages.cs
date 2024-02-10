namespace PRAM_lib.Code.CustomExceptions.Other
{
    internal static class ExceptionMessages
    {
        internal static string NotCompiled()
        {
            return "Code is not compiled.";
        }

        internal static string BugCompilationError()
        {
            return "Unknown compilation error.";
        }

        internal static string HasHalted()
        {
            return "Machine has halted.";
        }

        internal static string HasCrashed()
        {
            return "Machine has crashed.";
        }

        internal static string InvalidInstruction()
        {
            return "Invalid instruction.";
        }

        internal static string AddressIsNegative(int memoryAddress)
        {
            return $"Address cannot be negative. Tried to access address at {memoryAddress.ToString()}.";
        }

        internal static string AddressIsTooBig(int memoryAddress, int MaxCellSize)
        {
            return $"Address is too big. Tried to access {memoryAddress}, but max size is {MaxCellSize}.";
        }

        internal static string IOInputIsEmpty()
        {
            return "Cannot read from an empty IO memory.";
        }

        internal static string DivisionByZero()
        {
            return "Division by zero.";
        }

        internal static string JumpNotSet(string jumpName)
        {
            return $"Jump \"{jumpName}\" is not set.";
        }

        internal static string JumpNotDefined(string jumpName)
        {
            return $"Jump \"{jumpName}\" is not defined.";
        }

        internal static string CompilationInstructionNotRecognized(string instruction)
        {
            return $"Instruction \"{instruction}\" is not recognized.";
        }

        internal static string CompilationComparisonNotRecognized(string comparison)
        {
            return $"Comparison method \"{comparison}\" is not recognized.";
        }

        internal static string CompilerResultSetNotRecognized(string resultSet)
        {
            return $"Cannot set a ResultSet, as it is not recognized: \"{resultSet}\"";
        }

        internal static string CompilerOperationNotRecognized(string operation)
        {
            return $"Operation \"{operation}\" is not recognized.";
        }

        internal static string CompilerParallelNotEnded()
        {
            return "Parallel block is not ended.";
        }

        internal static string IllegalMemoryRead()
        {
            return "Illegal memory read.";
        }

        internal static string IllegalMemoryWrite()
        {
            return "Illegal memory write.";
        }

        internal static string IllegalMemoryAccess()
        {
            return "Illegal memory access.";
        }

    }
}
