using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Instruction.Master_Instructions;
using PRAM_lib.Instruction.Other;
using PRAM_lib.Instruction.Other.InstructionResult;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using System.Text;
using System.Text.RegularExpressions;

namespace PRAM_lib.Code.Compiler
{
    //A class that compiles code into CodeMemory
    internal class CodeCompiler
    {
        public CodeCompiler()
        {
            //Instructions = new List<Type>() {
            //    typeof(ReadInput),

            //};
        }

        //private List<Type> Instructions { get; set; }

        private Operation DetermineOperation(string operation)
        {
            switch (operation)
            {
                case "+":
                    return Operation.Add;
                case "-":
                    return Operation.Sub;
                case "*":
                    return Operation.Mul;
                case "/":
                    return Operation.Div;
                case "%":
                    return Operation.Mod;
                default:
                    throw new LocalException(ExceptionMessages.CompilerOperationNotRecognized(operation));
            }
        }

        private IResultSet ResultSetResolver(InstructionRegex regex, string inputText)
        {
            Match match;

            //ResultSet_Cell
            if (regex.ResultSet_Cell.IsMatch(inputText))
            {
                match = regex.ResultSet_Cell.Match(inputText);

                int cellIndex = int.Parse(match.Groups[1].Value);

                return new ResultSet_Cell(cellIndex);
            }

            //ResultSet_CellOpCell
            if (regex.ResultSet_CellOpCell.IsMatch(inputText))
            {
                match = regex.ResultSet_CellOpCell.Match(inputText);

                int leftCellIndex = int.Parse(match.Groups[1].Value);
                string operation = match.Groups[2].Value;
                int rightCellIndex = int.Parse(match.Groups[3].Value);

                return new ResultSet_CellOpCell(leftCellIndex, rightCellIndex, DetermineOperation(operation));
            }

            //ResultSet_CellOpConstant
            if (regex.ResultSet_CellOpConstant.IsMatch(inputText))
            {
                match = regex.ResultSet_CellOpConstant.Match(inputText);

                int cellIndex = int.Parse(match.Groups[1].Value);
                string operation = match.Groups[2].Value;
                int constantValue = int.Parse(match.Groups[3].Value);

                return new ResultSet_CellOpConstant(cellIndex, constantValue, DetermineOperation(operation));
            }

            if (regex.ResultSet_ConstantOpCell.IsMatch(inputText))
            {
                match = regex.ResultSet_ConstantOpCell.Match(inputText);

                int constantValue = int.Parse(match.Groups[1].Value);
                string operation = match.Groups[2].Value;
                int cellIndex = int.Parse(match.Groups[3].Value);

                return new ResultSet_CellOpConstant(cellIndex, constantValue, DetermineOperation(operation), false);
            }

            //ResultSet_Pointer
            if (regex.ResultSet_Pointer.IsMatch(inputText))
            {
                match = regex.ResultSet_Pointer.Match(inputText);

                int cellIndex = int.Parse(match.Groups[1].Value);

                return new ResultSet_Pointer(cellIndex);
            }

            //ResultSet_Constant
            if (regex.ResultSet_Constant.IsMatch(inputText))
            {
                match = regex.ResultSet_Constant.Match(inputText);

                int constantValue = int.Parse(match.Groups[1].Value);

                return new ResultSet_Constant(constantValue);
            }

            throw new LocalException(ExceptionMessages.CompilerResultSetNotRecognized(inputText));
        }

        //Parses 
        private ComparisonSet DetermineComparisonSet(string[] groups)
        {
            ComparisonSet set = new ComparisonSet();

            if (string.IsNullOrEmpty(groups[0]))
            {
                set.LeftValue = int.Parse(groups[1]);
            }
            else
            {
                set.LeftCell = int.Parse(groups[1]);
            }

            if (string.IsNullOrEmpty(groups[3]))
            {
                set.RightValue = int.Parse(groups[4]);
            }
            else
            {
                set.RightCell = int.Parse(groups[4]);
            }

            switch (groups[2])
            {
                case "==":
                    set.ComparisonMethod = ComparisonMethod.Equal;
                    break;
                case "!=":
                    set.ComparisonMethod = ComparisonMethod.NotEqual;
                    break;
                case "<":
                    set.ComparisonMethod = ComparisonMethod.Less;
                    break;
                case "<=":
                    set.ComparisonMethod = ComparisonMethod.LessOrEqual;
                    break;
                case ">":
                    set.ComparisonMethod = ComparisonMethod.Greater;
                    break;
                case ">=":
                    set.ComparisonMethod = ComparisonMethod.GreaterOrEqual;
                    break;
                default:
                    throw new LocalException(ExceptionMessages.CompilationComparisonNotRecognized(groups[3]));
            }

            return set;
        }

        //Will return the compiled code and the jump memory. Otherwise will return null if compilation fails, and will return the error message and the line index of the error.
        public CodeMemory.CodeMemory? Compile(string code, InstructionRegex regex, out Jumps.JumpMemory jumpMemory, out string ErrorMessage, out int ErrorLineIndex)
        {
            CodeMemory.CodeMemory newCodeMemory = new CodeMemory.CodeMemory();
            jumpMemory = new Jumps.JumpMemory();

            List<string> strings = code.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            StringBuilder potentialErrorMessage = new StringBuilder();
            
            // An index for lines, where even comments count as lines
            int lineIndex = -1;
            //An index for instructions, for example comments or jump labels do not count as instructions.
            //Used by compiler to set the values in instructions, and to set the jump memory.
            int instructionPointerIndex = 0;

            Match? match;
            foreach (string s in strings)
            {
                lineIndex++;

                //Empty line
                if(string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                //Comment
                if (regex.Comment.IsMatch(s))
                {
                    continue;
                }

                //ReadInput
                if (regex.ReadInput.IsMatch(s))
                {
                    match = regex.ReadInput.Match(s);

                    string sharedMemoryAddress = match.Groups[1].Value;
                    string selectedReadMemoryAddress = match.Groups[2].Value;

                    if (selectedReadMemoryAddress == "")
                    {
                        newCodeMemory.Instructions.Add(new ReadInput(int.Parse(sharedMemoryAddress), instructionPointerIndex++, lineIndex));
                    }
                    else
                    {
                        newCodeMemory.Instructions.Add(new ReadInput(int.Parse(sharedMemoryAddress), int.Parse(selectedReadMemoryAddress), lineIndex));
                    }
                    continue;
                }

                //WriteOutput
                if (regex.WriteOutput.IsMatch(s))
                {
                    match = regex.WriteOutput.Match(s);

                    string resultIs_any = match.Groups[1].Value;

                    try
                    {
                        newCodeMemory.Instructions.Add(new WriteOutput(ResultSetResolver(regex, resultIs_any), instructionPointerIndex++, lineIndex));
                    }
                    catch (LocalException e)
                    {
                        ErrorMessage = e.Message;
                        ErrorLineIndex = lineIndex;
                        return null;
                    }
                    continue;
                }

                //SetMemoryToResult
                if (regex.SetMemoryToResult.IsMatch(s))
                {
                    match = regex.SetMemoryToResult.Match(s);

                    string sharedMemoryResultAddress = match.Groups[1].Value;
                    string resultIs_any = match.Groups[2].Value;

                    try
                    {
                        newCodeMemory.Instructions.Add(new SetMemoryToResult(int.Parse(sharedMemoryResultAddress), ResultSetResolver(regex, resultIs_any), instructionPointerIndex++, lineIndex));
                        //NOTE: No checks for infinity loops right now
                    }
                    catch (LocalException e)
                    {
                        ErrorMessage = e.Message;
                        ErrorLineIndex = lineIndex;
                        return null;
                    }

                    continue;
                }

                //SetPointerToResult
                if (regex.SetPointerToResult.IsMatch(s))
                {
                    match = regex.SetPointerToResult.Match(s);

                    string leftPointingIndex = match.Groups[1].Value;
                    string resultIs_any = match.Groups[2].Value;

                    newCodeMemory.Instructions.Add(new SetPointerToResult(int.Parse(leftPointingIndex), ResultSetResolver(regex, resultIs_any), instructionPointerIndex++, lineIndex));

                    continue;
                }

                //JumpToInstruction
                if (regex.JumpToInstruction.IsMatch(s))
                {
                    match = regex.JumpToInstruction.Match(s);

                    string jumpName = match.Groups[1].Value;

                    jumpMemory.AddJumpLabel(jumpName);

                    newCodeMemory.Instructions.Add(new JumpTo(jumpName, instructionPointerIndex++, lineIndex));

                    continue;
                }

                //JumpToLabel
                if (regex.JumpToLabel.IsMatch(s))
                {
                    match = regex.JumpToLabel.Match(s);

                    string jumpName = match.Groups[1].Value;

                    jumpMemory.SetJump(jumpName, instructionPointerIndex); //Does not increment the virtual line index, because not an instruction

                    continue;
                }

                //IfJumpTo
                if (regex.IfJumpTo.IsMatch(s))
                {
                    match = regex.IfJumpTo.Match(s);

                    string potentialLeftCell = match.Groups[1].Value; //A cell identifier or empty string, if it's a constant on the left
                    string leftValue = match.Groups[2].Value; //Cell address or constant value
                    string comparisonOperator = match.Groups[3].Value; //Comparison operator
                    string potentialRightCell = match.Groups[4].Value; //A cell identifier or empty string, if it's a constant on the right
                    string rightValue = match.Groups[5].Value; //Cell address or constant value

                    ComparisonSet set = DetermineComparisonSet(new string[] { potentialLeftCell,
                        leftValue,
                        comparisonOperator,
                        potentialRightCell,
                        rightValue });

                    try
                    {
                        newCodeMemory.Instructions.Add(new IfJumpTo(match.Groups[6].Value, instructionPointerIndex++, lineIndex, set));
                    }
                    catch (LocalException e)
                    {
                        ErrorMessage = e.Message;
                        ErrorLineIndex = lineIndex;
                        return null;
                    }

                    continue;
                }



                //Instruction not recognized
                ErrorMessage = ExceptionMessages.CompilationInstructionNotRecognized(s);
                ErrorLineIndex = lineIndex;
                return null;
            }

            ErrorMessage = string.Empty;
            ErrorLineIndex = -1;
            return newCodeMemory;
        }
    }
}
