using PRAM_lib.Code.CustomExceptions;
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
                    throw new LocalException($"Error: Operation \"{operation}\" is not recognized.");
            }
        }

        private IInstructionResult AssignResultResolver(InstructionRegex regex, string inputText)
        {
            Match match;

            //ResultIs_Cell
            if (regex.ResultIs_Cell.IsMatch(inputText))
            {
                match = regex.ResultIs_Cell.Match(inputText);

                int cellIndex = int.Parse(match.Groups[1].Value);

                return new ResultIs_Cell(cellIndex);
            }

            //ResultIs_Cell2Cell
            if (regex.ResultIs_Cell2Cell.IsMatch(inputText))
            {
                match = regex.ResultIs_Cell2Cell.Match(inputText);

                int leftCellIndex = int.Parse(match.Groups[1].Value);
                string operation = match.Groups[2].Value;
                int rightCellIndex = int.Parse(match.Groups[3].Value);

                return new ResultIs_Cell2Cell(leftCellIndex, rightCellIndex, DetermineOperation(operation));
            }

            //ResultIs_Cell2Constant
            if (regex.ResultIs_Cell2Constant.IsMatch(inputText))
            {
                match = regex.ResultIs_Cell2Constant.Match(inputText);

                int cellIndex = int.Parse(match.Groups[1].Value);
                string operation = match.Groups[2].Value;
                int constantValue = int.Parse(match.Groups[3].Value);

                return new ResultIs_Cell2Constant(cellIndex, constantValue, DetermineOperation(operation));
            }

            //ResultIs_CellPointer
            if (regex.ResultIs_CellPointer.IsMatch(inputText))
            {
                match = regex.ResultIs_CellPointer.Match(inputText);

                int cellIndex = int.Parse(match.Groups[1].Value);

                return new ResultIs_CellPointer(cellIndex);
            }

            //ResultIs_Constant
            if (regex.ResultIs_Constant.IsMatch(inputText))
            {
                match = regex.ResultIs_Constant.Match(inputText);

                int constantValue = int.Parse(match.Groups[1].Value);

                return new ResultIs_Constant(constantValue);
            }

            throw new LocalException($"Error: Cannot assign to cell, as result is not recognized. \"{inputText}\" is not recognized.");
        }

        //Will return null if compilation fails, and will return the error message and the line index of the error
        public CodeMemory.CodeMemory? Compile(string code, InstructionRegex regex, out string ErrorMessage, out int ErrorLineIndex)
        {
            CodeMemory.CodeMemory newCodeMemory = new CodeMemory.CodeMemory();

            List<string> strings = code.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            StringBuilder potentialErrorMessage = new StringBuilder();
            int lineIndex = -1;
            //An index for instructions, for example flags don't count as instructions
            int virtulLineIndex = 0;

            Match? match;
            foreach (string s in strings)
            {
                lineIndex++;

                //ReadInput
                if (regex.ReadInput.IsMatch(s))
                {
                    match = regex.ReadInput.Match(s);

                    string sharedMemoryAddress = match.Groups[1].Value;
                    string selectedReadMemoryAddress = match.Groups[2].Value;

                    if (selectedReadMemoryAddress == "")
                    {
                        newCodeMemory.Instructions.Add(new ReadInput(int.Parse(sharedMemoryAddress), virtulLineIndex++, lineIndex));
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

                    string sharedMemoryAddress = match.Groups[1].Value;

                    newCodeMemory.Instructions.Add(new WriteOutput(int.Parse(sharedMemoryAddress), virtulLineIndex++, lineIndex));

                    continue;
                }

                //AssignResult
                if (regex.AssignResult.IsMatch(s))
                {
                    match = regex.AssignResult.Match(s);

                    string sharedMemoryResultAddress = match.Groups[1].Value;
                    string resultIs_any = match.Groups[2].Value;

                    try
                    {
                        newCodeMemory.Instructions.Add(new AssignResult(int.Parse(sharedMemoryResultAddress), AssignResultResolver(regex, resultIs_any), virtulLineIndex++, lineIndex));
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

                //WritePointer
                if (regex.WritePointer.IsMatch(s))
                {
                    match = regex.WritePointer.Match(s);

                    string leftPointingIndex = match.Groups[1].Value;
                    string rightValueIndex = match.Groups[2].Value;

                    newCodeMemory.Instructions.Add(new WritePointer(int.Parse(leftPointingIndex), int.Parse(rightValueIndex), virtulLineIndex++, lineIndex));

                    continue;
                }


                //Error
                ErrorMessage = $"Error: Instruction \"{s}\" is not recognized.";
                ErrorLineIndex = lineIndex;
                return null;
            }

            ErrorMessage = string.Empty;
            ErrorLineIndex = -1;
            return newCodeMemory;
        }
    }
}
