using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PRAM_lib.Code
{
    public class InstructionRegex
    {
        public Regex Comment { get; private set; } //Not a real instruction
        private static readonly Regex DefComment = new Regex(@"^#.*$");
        private static readonly int CommentGroupCount = 0;
        public Regex ReadInput { get; private set; }
        private static readonly Regex DefReadInput = new Regex(@"^[A-Z](\d+) := READ\((\d+|)\)\s*$");
        private static readonly int ReadInputGroupCount = 2;
        public Regex WriteOutput { get; private set; }
        private static readonly Regex DefWriteOutput = new Regex(@"^WRITE\((.*)\)\s*$");
        private static readonly int WriteOutputGroupCount = 1;
        public Regex SetMemoryToResult { get; private set; }
        private static readonly Regex DefSetMemoryToResult = new Regex(@"^([A-Z])(\d+) := (.*)\s*$");
        private static readonly int SetMemoryToResultGroupCount = 3;
        public Regex ResultSet_Cell { get; private set; }
        private static readonly Regex DefResultSet_Cell = new Regex(@"^([A-Z])(\d+)\s*$");
        private static readonly int ResultSet_CellGroupCount = 2;
        public Regex ResultSet_CellOpCell { get; private set; }
        private static readonly Regex DefResultSet_CellOpCell = new Regex(@"^([A-Z])(\d+) (\+|\-|\*|\/|%) ([A-Z])(\d+)\s*$");
        private static readonly int ResultSet_CellOpCellGroupCount = 5;
        public Regex ResultSet_CellOpConstant { get; private set; }
        private static readonly Regex DefResultSet_CellOpConstant = new Regex(@"^([A-Z])(\d+) (\+|\-|\*|\/|%) (\d+)\s*$");
        private static readonly int ResultSet_CellOpConstantGroupCount = 4;
        public Regex ResultSet_ConstantOpCell { get; private set; } //Supplemental regex for ResultSet_CellOpConstant alternative
        private static readonly Regex DefResultSet_ConstantOpCell = new Regex(@"^(\d+) (\+|\-|\*|\/|%) ([A-Z])(\d+)\s*$");
        private static readonly int ResultSet_ConstantOpCellGroupCount = 4;
        public Regex ResultSet_Pointer { get; private set; }
        private static readonly Regex DefResultSet_Pointer = new Regex(@"^\[([A-Z])(\d+)\]\s*$");
        private static readonly int ResultSet_PointerGroupCount = 2;
        public Regex ResultSet_Constant { get; private set; }
        private static readonly Regex DefResultSet_Constant = new Regex(@"^(\d+|-\d+)\s*$");
        private static readonly int ResultSet_ConstantGroupCount = 1;
        public Regex SetPointerToResult { get; private set; }
        private static readonly Regex DefSetPointerToResult = new Regex(@"^\[([A-Z])(\d+)\] := (.*)\s*$");
        private static readonly int SetPointerToResultGroupCount = 3;
        public Regex JumpToInstruction { get; private set; }
        private static readonly Regex DefJumpToInstruction = new Regex(@"^goto :([0-z]*)\s*$");
        private static readonly int JumpToInstructionGroupCount = 1;
        public Regex JumpToLabel { get; private set; } //Not a real instruction
        private static readonly Regex DefJumpToLabel = new Regex(@"^:([0-z]*)\s*$");
        private static readonly int JumpToLabelGroupCount = 1;
        public Regex IfJumpTo { get; private set; }
        private static readonly Regex DefIfJumpTo = new Regex(@"^if \(([A-Z]|)((?:-|)\d+) (==|!=|<|>|<=|>=) ([A-Z]|)((?:-|)\d+)\) goto :([0-z]*)\s*$");
        private static readonly int IfJumpToGroupCount = 6;
        public Regex ParallelStart { get; private set; }
        private static readonly Regex DefParallelStart = new Regex(@"^pardo (\d+)\s*$");
        private static readonly int ParallelStartGroupCount = 1;
        public readonly Regex ParallelEnd = new Regex(@"^parend\s*$"); //Cannot be changed by user
        public readonly string ParallelEndString = "parend"; //Cannot be changed by user
        public string ParallelCell { get; private set; } //Not a real instruction
        public static readonly string DefParallelCell = "S";
        private static readonly int ParallelCellGroupCount = 0;
        public Regex ResultSet_ParallelIndex { get; private set; }
        private static readonly Regex DefResultSet_ParallelIndex = new Regex(@"^{i}\s*$");
        private static readonly int ResultSet_ParallelIndexGroupCount = 1;
        public Regex IndirectMultiMemoryToResult { get; private set; }
        private static readonly Regex DefIndirectMultiMemoryToResult = new Regex(@"^([A-Z]){(.*)} := (.*)\s*$");
        private static readonly int IndirectMultiMemoryToResultGroupCount = 3;
        public Regex Halt { get; private set; }
        private static readonly Regex DefHalt = new Regex(@"^halt\s*$");
        private static readonly int HaltGroupCount = 0;
        public Regex NoOperation { get; private set; }
        private static readonly Regex DefNoOperation = new Regex(@"^nop\s*$");
        private static readonly int NoOperationGroupCount = 0;


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
            JumpToInstruction = DefJumpToInstruction;
            JumpToLabel = DefJumpToLabel;
            IfJumpTo = DefIfJumpTo;
            ParallelStart = DefParallelStart;
            ParallelCell = DefParallelCell;
            ResultSet_ParallelIndex = DefResultSet_ParallelIndex;
            IndirectMultiMemoryToResult = DefIndirectMultiMemoryToResult;
            Halt = DefHalt;
            NoOperation = DefNoOperation;
        }

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
            JumpToInstruction = DefJumpToInstruction;
            JumpToLabel = DefJumpToLabel;
            IfJumpTo = DefIfJumpTo;
            ParallelStart = DefParallelStart;
            ParallelCell = DefParallelCell;
            ResultSet_ParallelIndex = DefResultSet_ParallelIndex;
            IndirectMultiMemoryToResult = DefIndirectMultiMemoryToResult;
            Halt = DefHalt;
            NoOperation = DefNoOperation;
        }

        public string SaveToJson()
        {
            return "{" +
                "\"Comment\":\"" + Comment + "\"," +
                "\"ReadInput\":\"" + ReadInput + "\"," +
                "\"WriteOutput\":\"" + WriteOutput + "\"," +
                "\"SetMemoryToResult\":\"" + SetMemoryToResult + "\"," +
                "\"ResultSet_Cell\":\"" + ResultSet_Cell + "\"," +
                "\"ResultSet_CellOpCell\":\"" + ResultSet_CellOpCell + "\"," +
                "\"ResultSet_CellOpConstant\":\"" + ResultSet_CellOpConstant + "\"," +
                "\"ResultSet_ConstantOpCell\":\"" + ResultSet_ConstantOpCell + "\"," +
                "\"ResultSet_Pointer\":\"" + ResultSet_Pointer + "\"," +
                "\"ResultSet_Constant\":\"" + ResultSet_Constant + "\"," +
                "\"SetPointerToResult\":\"" + SetPointerToResult + "\"," +
                "\"JumpToInstruction\":\"" + JumpToInstruction + "\"," +
                "\"JumpToLabel\":\"" + JumpToLabel + "\"," +
                "\"IfJumpTo\":\"" + IfJumpTo + "\"," +
                "\"ParallelStart\":\"" + ParallelStart + "\"," +
                "\"ParallelCell\":\"" + ParallelCell + "\"," +
                "\"ResultSet_ParallelIndex\":\"" + ResultSet_ParallelIndex + "\"," +
                "\"IndirectMultiMemoryToResult\":\"" + IndirectMultiMemoryToResult + "\"," +
                "\"Halt\":\"" + Halt + "\"," +
                "\"NoOperation\":\"" + NoOperation + "\"" +
                "}";
        }

        public bool LoadFromJson(string json, out string errorMessage)
        {
            try
            {
                errorMessage = string.Empty;
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                JsonElement jsonObject;
                try
                {
                    jsonObject = JsonSerializer.Deserialize<JsonElement>(json, options);
                }
                catch (JsonException je)
                {
                    errorMessage = $"Failed to parse JSON: {je.Message}";
                    return false;
                }

                foreach (var property in jsonObject.EnumerateObject())
                {
                    var name = property.Name;
                    var regexPattern = property.Value.GetString();
                    if (regexPattern == null) continue;

                    //Find default regex field
                    var defFieldName = $"Def{name}";
                    var defField = this.GetType().GetField(defFieldName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                    if (defField != null)
                    {
                        var defRegex = (Regex)defField.GetValue(null);
                        var expectedGroupCount = defRegex.GetGroupNumbers().Length;

                        var regex = new Regex(regexPattern);
                        if (regex.GetGroupNumbers().Length != expectedGroupCount)
                        {
                            errorMessage = $"Invalid group count for {name}. Expected: {expectedGroupCount}, Found: {regex.GetGroupNumbers().Length}";
                            return false;
                        }

                        //Set property with reflection
                        var propertyInfo = this.GetType().GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                        if (propertyInfo != null && propertyInfo.PropertyType == typeof(Regex))
                        {
                            propertyInfo.SetValue(this, regex);
                        }
                    }
                    else if (name.Equals("ParallelCell", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ParallelCell = regexPattern;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                errorMessage = "Unknown exception occurred. Possible JSON format error.";
                return false;
            }
        }


        //Return a dictionary of all regexes, with their names as keys
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
                {"JumpToInstruction", JumpToInstruction},
                {"JumpToLabel", JumpToLabel},
                {"IfJumpTo", IfJumpTo},
                {"ParallelStart", ParallelStart},
                {"ParallelCell", new Regex(ParallelCell)},
                {"ResultSet_ParallelIndex", ResultSet_ParallelIndex},
                {"IndirectMultiMemoryToResult", IndirectMultiMemoryToResult},
                {"Halt", Halt},
                {"NoOperation", NoOperation}
            };
        }

        //Set a regex by name, and check if the group count is correct. Return true if the group count is correct, false otherwise
        public bool SetRegex(string name, string pattern)
        {
            try
            {
                string propertyName = Char.ToUpper(name[0]) + name.Substring(1);
                string fieldName = propertyName + "GroupCount";

                PropertyInfo? propertyInfo = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                FieldInfo? groupCountField = this.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);

                if (propertyInfo == null || groupCountField == null)
                {
                    return false;
                }

                object? groupCountValue = groupCountField.GetValue(null);
                if (groupCountValue is null)
                {
                    return false;
                }

                int expectedGroupCount = (int)groupCountValue;
                var regex = new Regex(pattern);

                //Does not account for the default group
                if (regex.GetGroupNumbers().Length - 1 != expectedGroupCount)
                {
                    return false;
                }

                if (propertyName == "ParallelCell")
                {
                    ParallelCell = pattern;
                }
                else if (propertyInfo.PropertyType == typeof(Regex))
                {
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
