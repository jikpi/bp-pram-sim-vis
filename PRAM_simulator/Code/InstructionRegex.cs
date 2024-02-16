using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PRAM_lib.Code
{
    /// <summary>
    /// A class that holds all the regexes for the instructions of the PRAM language
    /// </summary>
    public class InstructionRegex
    {
        public Regex Comment { get; private set; } //Not a real instruction
        private static readonly Regex DefComment = new Regex(@"^#.*$");
        public Regex ReadInput { get; private set; }
        private static readonly Regex DefReadInput = new Regex(@"^[A-Z](\d+) := READ\((\d+|)\)\s*$");
        public Regex WriteOutput { get; private set; }
        private static readonly Regex DefWriteOutput = new Regex(@"^WRITE\((.*)\)\s*$");
        public Regex SetMemoryToResult { get; private set; }
        private static readonly Regex DefSetMemoryToResult = new Regex(@"^([A-Z])(\d+) := (.*)\s*$");
        public Regex ResultSet_Cell { get; private set; }
        private static readonly Regex DefResultSet_Cell = new Regex(@"^([A-Z])(\d+)\s*$");
        public Regex ResultSet_CellOpCell { get; private set; }
        private static readonly Regex DefResultSet_CellOpCell = new Regex(@"^([A-Z])(\d+) (\+|\-|\*|\/|%) ([A-Z])(\d+)\s*$");
        public Regex ResultSet_CellOpConstant { get; private set; }
        private static readonly Regex DefResultSet_CellOpConstant = new Regex(@"^([A-Z])(\d+) (\+|\-|\*|\/|%) (\d+)\s*$");
        public Regex ResultSet_ConstantOpCell { get; private set; } //Supplemental regex for ResultSet_CellOpConstant alternative
        private static readonly Regex DefResultSet_ConstantOpCell = new Regex(@"^(\d+) (\+|\-|\*|\/|%) ([A-Z])(\d+)\s*$");
        public Regex ResultSet_Pointer { get; private set; }
        private static readonly Regex DefResultSet_Pointer = new Regex(@"^\[([A-Z])(\d+)\]\s*$");
        public Regex ResultSet_Constant { get; private set; }
        private static readonly Regex DefResultSet_Constant = new Regex(@"^(\d+|-\d+)\s*$");
        public Regex SetPointerToResult { get; private set; }
        private static readonly Regex DefSetPointerToResult = new Regex(@"^\[([A-Z])(\d+)\] := (.*)\s*$");
        public Regex JumpToLabel { get; private set; }
        private static readonly Regex DefJumpToInstruction = new Regex(@"^goto :([0-z]*)\s*$");
        public Regex JumpLabel { get; private set; } //Not a real instruction
        private static readonly Regex DefJumpToLabel = new Regex(@"^:([0-z]*)\s*$");
        public Regex IfJumpToLabel { get; private set; }
        private static readonly Regex DefIfJumpTo = new Regex(@"^if \(([A-Z]|)((?:-|)\d+) (==|!=|<|>|<=|>=) ([A-Z]|)((?:-|)\d+)\) goto :([0-z]*)\s*$");
        public Regex ParallelStart { get; private set; }
        private static readonly Regex DefParallelStart = new Regex(@"^pardo (.*)\s*$");
        public readonly Regex ParallelEnd = new Regex(@"^parend\s*$"); //Cannot be changed by user
        public readonly string ParallelEndString = "parend"; //Cannot be changed by user
        public string ParallelCell { get; private set; } //Not a real instruction
        private static readonly string DefParallelCell = new string("S");
        public Regex ResultSet_ParallelIndex { get; private set; }
        private static readonly Regex DefResultSet_ParallelIndex = new Regex(@"^{i}\s*$");
        public Regex IndirectMultiMemoryToResult { get; private set; }
        private static readonly Regex DefIndirectMultiMemoryToResult = new Regex(@"^([A-Z]){(.*)} := (.*)\s*$");
        public Regex Halt { get; private set; }
        private static readonly Regex DefHalt = new Regex(@"^halt\s*$");
        public Regex NoOperation { get; private set; }
        private static readonly Regex DefNoOperation = new Regex(@"^nop\s*$");

        public Regex ResultSet_IndirectPointer { get; private set; }
        private static readonly Regex DefResultSet_IndirectPointer = new Regex(@"^([A-Z]){(.*)}\s*$");

        public InstructionRegex()
        {
            Comment = DefComment;
            ReadInput = DefReadInput;
            WriteOutput = DefWriteOutput;
            SetMemoryToResult = DefSetMemoryToResult;
            SetPointerToResult = DefSetPointerToResult;
            ResultSet_Cell = DefResultSet_Cell;
            ResultSet_CellOpCell = DefResultSet_CellOpCell;
            ResultSet_CellOpConstant = DefResultSet_CellOpConstant;
            ResultSet_ConstantOpCell = DefResultSet_ConstantOpCell;
            ResultSet_Pointer = DefResultSet_Pointer;
            ResultSet_Constant = DefResultSet_Constant;
            JumpToLabel = DefJumpToInstruction;
            JumpLabel = DefJumpToLabel;
            IfJumpToLabel = DefIfJumpTo;
            ParallelStart = DefParallelStart;
            ParallelCell = DefParallelCell;
            ResultSet_ParallelIndex = DefResultSet_ParallelIndex;
            IndirectMultiMemoryToResult = DefIndirectMultiMemoryToResult;
            Halt = DefHalt;
            NoOperation = DefNoOperation;
            ResultSet_IndirectPointer = DefResultSet_IndirectPointer;
        }
        /// <summary>
        /// Reset all regexes to their default values
        /// </summary>
        public void ResetToDefault()
        {
            Comment = DefComment;
            ReadInput = DefReadInput;
            WriteOutput = DefWriteOutput;
            SetMemoryToResult = DefSetMemoryToResult;
            SetPointerToResult = DefSetPointerToResult;
            ResultSet_Cell = DefResultSet_Cell;
            ResultSet_CellOpCell = DefResultSet_CellOpCell;
            ResultSet_CellOpConstant = DefResultSet_CellOpConstant;
            ResultSet_ConstantOpCell = DefResultSet_ConstantOpCell;
            ResultSet_Pointer = DefResultSet_Pointer;
            ResultSet_Constant = DefResultSet_Constant;
            JumpToLabel = DefJumpToInstruction;
            JumpLabel = DefJumpToLabel;
            IfJumpToLabel = DefIfJumpTo;
            ParallelStart = DefParallelStart;
            ParallelCell = DefParallelCell;
            ResultSet_ParallelIndex = DefResultSet_ParallelIndex;
            IndirectMultiMemoryToResult = DefIndirectMultiMemoryToResult;
            Halt = DefHalt;
            NoOperation = DefNoOperation;
            ResultSet_IndirectPointer = DefResultSet_IndirectPointer;
        }

        /// <summary>
        /// Return a string with all the regexes, with property names on first line, and regex patterns on second line, in pairs.
        /// </summary>
        /// <returns></returns>
        public string SaveToText()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("Comment");
            sb.AppendLine(Comment.ToString());
            sb.AppendLine("ReadInput");
            sb.AppendLine(ReadInput.ToString());
            sb.AppendLine("WriteOutput");
            sb.AppendLine(WriteOutput.ToString());
            sb.AppendLine("SetMemoryToResult");
            sb.AppendLine(SetMemoryToResult.ToString());
            sb.AppendLine("ResultSet_Cell");
            sb.AppendLine(ResultSet_Cell.ToString());
            sb.AppendLine("ResultSet_CellOpCell");
            sb.AppendLine(ResultSet_CellOpCell.ToString());
            sb.AppendLine("ResultSet_CellOpConstant");
            sb.AppendLine(ResultSet_CellOpConstant.ToString());
            sb.AppendLine("ResultSet_ConstantOpCell");
            sb.AppendLine(ResultSet_ConstantOpCell.ToString());
            sb.AppendLine("ResultSet_Pointer");
            sb.AppendLine(ResultSet_Pointer.ToString());
            sb.AppendLine("ResultSet_Constant");
            sb.AppendLine(ResultSet_Constant.ToString());
            sb.AppendLine("SetPointerToResult");
            sb.AppendLine(SetPointerToResult.ToString());
            sb.AppendLine("JumpToLabel");
            sb.AppendLine(JumpToLabel.ToString());
            sb.AppendLine("JumpLabel");
            sb.AppendLine(JumpLabel.ToString());
            sb.AppendLine("IfJumpToLabel");
            sb.AppendLine(IfJumpToLabel.ToString());
            sb.AppendLine("ParallelStart");
            sb.AppendLine(ParallelStart.ToString());
            sb.AppendLine("ParallelCell");
            sb.AppendLine(ParallelCell.ToString());
            sb.AppendLine("ResultSet_ParallelIndex");
            sb.AppendLine(ResultSet_ParallelIndex.ToString());
            sb.AppendLine("IndirectMultiMemoryToResult");
            sb.AppendLine(IndirectMultiMemoryToResult.ToString());
            sb.AppendLine("Halt");
            sb.AppendLine(Halt.ToString());
            sb.AppendLine("NoOperation");
            sb.AppendLine(NoOperation.ToString());
            sb.AppendLine("ResultSet_IndirectPointer");
            sb.AppendLine(ResultSet_IndirectPointer.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Load pairs of property name and regex pattern from a string, and set the properties to the regexes using reflection.
        /// Should anything go wrong, return false and set errorMessage.
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool LoadFromText(string plaintext, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (plaintext.Length > 10000)
            {
                errorMessage = "Input too long, no changes were made.";
                return false;
            }

            string[] lines = plaintext.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length % 2 != 0)
            {
                errorMessage = "Input parse error, no changes were made.";
                return false;
            }

            for (int i = 0; i < lines.Length; i += 2)
            {
                string name = lines[i];
                string value = lines[i + 1];

                PropertyInfo? propertyInfo = GetType().GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    errorMessage = $"Unknown property at: {name}, stopping now.";
                    return false;
                }

                if (propertyInfo.PropertyType == typeof(Regex))
                {
                    Regex regex;
                    try
                    {
                        regex = new Regex(value);
                    }
                    catch (ArgumentException ex)
                    {
                        errorMessage = $"Invalid regex pattern for {name}: {ex.Message}, stopping now.";
                        return false;
                    }

                    if (!value.StartsWith("^") || !value.EndsWith("*$"))
                    {
                        errorMessage = $"Invalid regex pattern for {name}: Must start with '^' and end with '*$', stopping now.";
                        return false;
                    }

                    propertyInfo.SetValue(this, regex);
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(this, value);
                }
                else
                {
                    errorMessage = $"Property {name} has an unsupported type: {propertyInfo.PropertyType}, stopping now.";
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Return a dictionary of all regexes, with their names as keys
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.Dictionary<string, Regex> GetRegexes()
        {
            return new System.Collections.Generic.Dictionary<string, Regex>
            {
                {"Comment", Comment},
                {"ReadInput", ReadInput},
                {"WriteOutput", WriteOutput},
                {"SetMemoryToResult", SetMemoryToResult},
                {"ResultSet_Cell", ResultSet_Cell},
                {"ResultSet_CellOpCell", ResultSet_CellOpCell},
                {"ResultSet_CellOpConstant", ResultSet_CellOpConstant},
                {"ResultSet_ConstantOpCell", ResultSet_ConstantOpCell},
                {"ResultSet_Pointer", ResultSet_Pointer},
                {"ResultSet_Constant", ResultSet_Constant},
                {"SetPointerToResult", SetPointerToResult},
                {"JumpToLabel", JumpToLabel},
                {"JumpLabel", JumpLabel},
                {"IfJumpToLabel", IfJumpToLabel},
                {"ParallelStart", ParallelStart},
                {"ParallelCell", new Regex(ParallelCell)},
                {"ResultSet_ParallelIndex", ResultSet_ParallelIndex},
                {"IndirectMultiMemoryToResult", IndirectMultiMemoryToResult},
                {"Halt", Halt},
                {"NoOperation", NoOperation},
                {"ResultSet_IndirectPointer", ResultSet_IndirectPointer}
            };
        }

        /// <summary>
        /// Set a regex by name, and check if the group count is correct. Return true if the group count is correct, false otherwise
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool SetRegex(string name, string pattern)
        {
            try
            {
                string propertyName = Char.ToUpper(name[0]) + name.Substring(1);

                PropertyInfo? propertyInfo = GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return false;
                }

                if (propertyName == "ParallelCell")
                {
                    ParallelCell = pattern;
                }
                else if (propertyInfo.PropertyType == typeof(Regex))
                {
                    Regex regex = new Regex(pattern);

                    int expectedGroupCount = regex.GetGroupNumbers().Length;
                    if (regex.GetGroupNumbers().Length != expectedGroupCount)
                    {
                        return false;
                    }

                    if (!pattern.StartsWith("^") || !pattern.EndsWith("*$"))
                    {
                        return false;
                    }

                    propertyInfo.SetValue(this, regex);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
