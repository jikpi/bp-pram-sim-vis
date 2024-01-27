using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Instruction.Independent_Instructions;
using PRAM_lib.Instruction.Master_Instructions;
using PRAM_lib.Instruction.Other.InstructionResult;
using PRAM_lib.Instruction.Other.InstructionResult.Interface;
using PRAM_lib.Instruction.Other.ResultSet;
using PRAM_lib.Instruction.Parallel_Instructions;
using PRAM_lib.Machine.Container;
using PRAM_lib.Processor;
using System.Text;
using System.Text.RegularExpressions;

namespace PRAM_lib.Code.Compiler
{
    //A class that compiles code into CodeMemory
    internal class CodeCompiler
    {
        public CodeCompiler()
        {

        }

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



        //Will return the compiled code and the jump memory. Otherwise will return null if compilation fails, and will return the error message and the line index of the error.
        internal CodeMemory.CodeMemory? Compile(string code, MasterGateway masterGateway, InstructionRegex regex, out Jumps.JumpMemory jumpMemory, out List<ParallelMachineContainer> parallelMachines, out string errorMessage, out int returnLineIndex, ParallelGateway? parallelGateway = null)
        {

            bool IsLocalMemoryAccess(string memoryAddressContext)
            {
                if (memoryAddressContext != regex.ParallelCell && parallelGateway != null)
                {
                    return true;
                }

                return false;
            }

            IResultSet ResultSetResolver(InstructionRegex regex, string inputText)
            {
                Match match;

                //ResultSet_Cell
                if (regex.ResultSet_Cell.IsMatch(inputText))
                {
                    match = regex.ResultSet_Cell.Match(inputText);

                    string memoryAddressContext = match.Groups[1].Value;
                    int cellIndex = int.Parse(match.Groups[2].Value);

                    IGateway selectedGateway = masterGateway;
                    if (IsLocalMemoryAccess(memoryAddressContext))
                    {
                        selectedGateway = parallelGateway!;
                    }

                    return new ResultSet_Cell(new GatewayIndexSet(selectedGateway, cellIndex));
                }

                //ResultSet_CellOpCell
                if (regex.ResultSet_CellOpCell.IsMatch(inputText))
                {
                    match = regex.ResultSet_CellOpCell.Match(inputText);

                    int leftCellIndex = int.Parse(match.Groups[2].Value);
                    string operation = match.Groups[3].Value;
                    int rightCellIndex = int.Parse(match.Groups[5].Value);
                    string leftMemoryAddressContext = match.Groups[1].Value;
                    string rightMemoryAddressContext = match.Groups[4].Value;

                    IGateway leftGateway = masterGateway;
                    IGateway rightGateway = masterGateway;
                    if (IsLocalMemoryAccess(leftMemoryAddressContext))
                    {
                        leftGateway = parallelGateway!;
                    }
                    if (IsLocalMemoryAccess(rightMemoryAddressContext))
                    {
                        rightGateway = parallelGateway!;
                    }

                    return new ResultSet_CellOpCell(new GatewayIndexSet(leftGateway, leftCellIndex), new GatewayIndexSet(rightGateway, rightCellIndex), DetermineOperation(operation));
                }

                //ResultSet_CellOpConstant
                if (regex.ResultSet_CellOpConstant.IsMatch(inputText))
                {
                    match = regex.ResultSet_CellOpConstant.Match(inputText);

                    string memoryAddressContext = match.Groups[1].Value;
                    int cellIndex = int.Parse(match.Groups[2].Value);
                    string operation = match.Groups[3].Value;
                    int constantValue = int.Parse(match.Groups[4].Value);

                    IGateway selectedGateway = masterGateway;
                    if (IsLocalMemoryAccess(memoryAddressContext))
                    {
                        selectedGateway = parallelGateway!;
                    }

                    return new ResultSet_CellOpConstant(new GatewayIndexSet(selectedGateway, cellIndex), constantValue, DetermineOperation(operation));
                }

                if (regex.ResultSet_ConstantOpCell.IsMatch(inputText))
                {
                    match = regex.ResultSet_ConstantOpCell.Match(inputText);

                    int constantValue = int.Parse(match.Groups[1].Value);
                    string operation = match.Groups[2].Value;
                    int cellIndex = int.Parse(match.Groups[4].Value);
                    string memoryAddressContext = match.Groups[3].Value;

                    IGateway selectedGateway = masterGateway;
                    if (IsLocalMemoryAccess(memoryAddressContext))
                    {
                        selectedGateway = parallelGateway!;
                    }

                    return new ResultSet_CellOpConstant(new GatewayIndexSet(selectedGateway, cellIndex), constantValue, DetermineOperation(operation), false);
                }

                //ResultSet_Pointer
                if (regex.ResultSet_Pointer.IsMatch(inputText))
                {
                    match = regex.ResultSet_Pointer.Match(inputText);

                    string memoryAddressContext = match.Groups[1].Value;
                    int cellIndex = int.Parse(match.Groups[2].Value);

                    IGateway selectedGateway = masterGateway;
                    if (IsLocalMemoryAccess(memoryAddressContext))
                    {
                        selectedGateway = parallelGateway!;
                    }

                    return new ResultSet_Pointer(new GatewayIndexSet(selectedGateway, cellIndex));
                }

                //ResultSet_Constant
                if (regex.ResultSet_Constant.IsMatch(inputText))
                {
                    match = regex.ResultSet_Constant.Match(inputText);

                    int constantValue = int.Parse(match.Groups[1].Value);

                    return new ResultSet_Constant(constantValue);
                }

                //ResultSet_ParallelIndex
                if (regex.ResultSet_ParallelIndex.IsMatch(inputText))
                {
                    match = regex.ResultSet_ParallelIndex.Match(inputText);

                    IGateway selectedGateway = masterGateway;
                    if (parallelGateway != null)
                    {
                        selectedGateway = parallelGateway;
                    }

                    return new ResultSet_ParallelIndex(new GatewayIndexSet(selectedGateway, -1));
                }

                throw new LocalException(ExceptionMessages.CompilerResultSetNotRecognized(inputText));
            }

            ComparisonSet DetermineComparisonSet(string[] groups)
            {
                IGateway leftSelectedGateway = masterGateway;
                IGateway rightSelectedGateway = masterGateway;

                string leftMemoryAddressContext = groups[0];
                string rightMemoryAddressContext = groups[4];

                if (IsLocalMemoryAccess(leftMemoryAddressContext))
                {
                    leftSelectedGateway = parallelGateway!;
                }

                if (IsLocalMemoryAccess(rightMemoryAddressContext))
                {
                    rightSelectedGateway = parallelGateway!;
                }

                int? leftValue = null;
                int? rightValue = null;
                GatewayIndexSet? leftGateway = null;
                GatewayIndexSet? rightGateway = null;
                ComparisonMethod? comparisonMethod = null;

                if (string.IsNullOrEmpty(groups[0]))
                {
                    leftValue = int.Parse(groups[1]);
                }
                else
                {
                    leftGateway = new GatewayIndexSet(leftSelectedGateway, int.Parse(groups[1]));
                }

                if (string.IsNullOrEmpty(groups[3]))
                {
                    rightValue = int.Parse(groups[4]);
                }
                else
                {
                    rightGateway = new GatewayIndexSet(rightSelectedGateway, int.Parse(groups[4]));
                }

                switch (groups[2])
                {
                    case "==":
                        comparisonMethod = ComparisonMethod.Equal;
                        break;
                    case "!=":
                        comparisonMethod = ComparisonMethod.NotEqual;
                        break;
                    case "<":
                        comparisonMethod = ComparisonMethod.Less;
                        break;
                    case "<=":
                        comparisonMethod = ComparisonMethod.LessOrEqual;
                        break;
                    case ">":
                        comparisonMethod = ComparisonMethod.Greater;
                        break;
                    case ">=":
                        comparisonMethod = ComparisonMethod.GreaterOrEqual;
                        break;
                    default:
                        throw new LocalException(ExceptionMessages.CompilationComparisonNotRecognized(groups[3]));
                }

                return new ComparisonSet(leftGateway, rightGateway, comparisonMethod, leftValue, rightValue);
            }



            CodeMemory.CodeMemory newCodeMemory = new CodeMemory.CodeMemory();
            jumpMemory = new Jumps.JumpMemory();
            parallelMachines = new List<ParallelMachineContainer>();

            List<string> strings = code.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            StringBuilder potentialErrorMessage = new StringBuilder();

            // An index for lines, where even comments count as lines
            int lineIndex = -1;
            //An index for instructions, for example comments or jump labels do not count as instructions.
            //Used by compiler to set the values in instructions, and to set the jump memory.
            int instructionPointerIndex = 0;

            //Set the gateways
            IGateway foreignGateway = masterGateway;
            IGateway localGateway = masterGateway;
            if (parallelGateway != null)
            {
                localGateway = parallelGateway;
            }

            Match? match;
            for (int i = 0; i < strings.Count; i++)
            {
                lineIndex++;

                //Parallel instructions beginning
                if (regex.ParallelStart.IsMatch(strings[i]))
                {
                    match = regex.ParallelStart.Match(strings[i]);

                    if (parallelGateway != null)
                    {
                        errorMessage = "Cannot start parallel processors inside a parallel processor";
                        returnLineIndex = lineIndex;
                        return null;
                    }

                    List<string> pardoStrings = strings.GetRange(i + 1, strings.Count - i - 1);
                    string pardoCode = string.Join("\r\n", pardoStrings);
                    int numberofprocessors = int.Parse(match.Groups[1].Value);

                    if(numberofprocessors < 1)
                    {
                        errorMessage = "Number of processors must be at least 1";
                        returnLineIndex = lineIndex;
                        return null;
                    }

                    List<InParallelMachine> inParallelMachines = new List<InParallelMachine>();

                    for (int j = 0; j < numberofprocessors; j++)
                    {
                        ParallelGateway newParallelGateway = new ParallelGateway();
                        CodeMemory.CodeMemory? pardoCodeMemory = Compile(pardoCode, masterGateway, regex, out Jumps.JumpMemory pardoJumpMemory, out _, out string pardoErrorMessage, out int pardoReturnLineIndex, newParallelGateway);

                        if (pardoCodeMemory == null)
                        {
                            errorMessage = pardoErrorMessage;
                            returnLineIndex = pardoReturnLineIndex + lineIndex + 1;
                            return null;
                        }

                        inParallelMachines.Add(new InParallelMachine(j, pardoCodeMemory, pardoJumpMemory, newParallelGateway));
                    }

                    parallelMachines.Add(new ParallelMachineContainer(inParallelMachines, pardoCode.Substring(0, pardoCode.IndexOf(regex.ParallelEndString))));

                    //Add the ParallelDo instruction into the master machine
                    newCodeMemory.Instructions.Add(new ParallelDo(new GatewayIndexSet(masterGateway, -1), instructionPointerIndex++, lineIndex));
                    //Skip the lines that were already compiled
                    int originalLineIndex = lineIndex;
                    while (!regex.ParallelEnd.IsMatch(strings[i]))
                    {
                        if (i < strings.Count - 1)
                        {
                            i++;
                            lineIndex++;
                        }
                        else
                        {
                            errorMessage = "Parallel processor not ended";
                            returnLineIndex = originalLineIndex;
                            return null;
                        }
                    }

                    continue;
                }

                //IndirectMultiMemoryToResult
                if (regex.IndirectMultiMemoryToResult.IsMatch(strings[i]))
                {
                    match = regex.IndirectMultiMemoryToResult.Match(strings[i]);

                    string memoryAddressContext = match.Groups[1].Value;
                    string addressingResult = match.Groups[2].Value;
                    string resultIs_any = match.Groups[3].Value;

                    IGateway selectedGateway = foreignGateway;
                    if (IsLocalMemoryAccess(memoryAddressContext))
                    {
                        selectedGateway = localGateway;
                    }

                    try
                    {
                        newCodeMemory.Instructions.Add(new IndirectMultiMemoryToResult(new GatewayIndexSet(selectedGateway, -1), ResultSetResolver(regex, addressingResult), ResultSetResolver(regex, resultIs_any), instructionPointerIndex++, lineIndex));
                    }
                    catch (LocalException e)
                    {
                        errorMessage = e.Message;
                        returnLineIndex = lineIndex;
                        return null;
                    }

                    continue;
                }

                //Parallel instructions ending
                if (regex.ParallelEnd.IsMatch(strings[i]))
                {
                    if (parallelGateway == null)
                    {
                        errorMessage = "Cannot end parallel processors outside a parallel processor";
                        returnLineIndex = lineIndex;
                        return null;
                    }
                    else
                    {
                        errorMessage = string.Empty;
                        returnLineIndex = -1;
                        return newCodeMemory;
                    }
                }

                //Empty line
                if (string.IsNullOrWhiteSpace(strings[i]))
                {
                    continue;
                }

                //Comment
                if (regex.Comment.IsMatch(strings[i]))
                {
                    continue;
                }

                //ReadInput
                if (regex.ReadInput.IsMatch(strings[i]))
                {
                    match = regex.ReadInput.Match(strings[i]);

                    string sharedMemoryAddress = match.Groups[1].Value;
                    string selectedReadMemoryAddress = match.Groups[2].Value;


                    if (selectedReadMemoryAddress == "")
                    {
                        newCodeMemory.Instructions.Add(new ReadInput(new GatewayIndexSet(masterGateway, int.Parse(sharedMemoryAddress)), instructionPointerIndex++, lineIndex));
                    }
                    else
                    {
                        newCodeMemory.Instructions.Add(new ReadInput(new GatewayIndexSet(masterGateway, int.Parse(sharedMemoryAddress)), int.Parse(selectedReadMemoryAddress), lineIndex));
                    }
                    continue;
                }

                //WriteOutput
                if (regex.WriteOutput.IsMatch(strings[i]))
                {
                    match = regex.WriteOutput.Match(strings[i]);

                    string resultIs_any = match.Groups[1].Value;

                    try
                    {
                        newCodeMemory.Instructions.Add(new WriteOutput(new GatewayIndexSet(masterGateway, 0), ResultSetResolver(regex, resultIs_any), instructionPointerIndex++, lineIndex));
                    }
                    catch (LocalException e)
                    {
                        errorMessage = e.Message;
                        returnLineIndex = lineIndex;
                        return null;
                    }
                    continue;
                }

                //SetMemoryToResult
                if (regex.SetMemoryToResult.IsMatch(strings[i]))
                {
                    match = regex.SetMemoryToResult.Match(strings[i]);

                    string memoryAddressContext = match.Groups[1].Value;
                    string sharedMemoryResultAddress = match.Groups[2].Value;
                    string resultIs_any = match.Groups[3].Value;

                    IGateway selectedGateway = foreignGateway;
                    if (IsLocalMemoryAccess(memoryAddressContext))
                    {
                        selectedGateway = localGateway;
                    }

                    try
                    {
                        newCodeMemory.Instructions.Add(new SetMemoryToResult(new GatewayIndexSet(selectedGateway, int.Parse(sharedMemoryResultAddress)), ResultSetResolver(regex, resultIs_any), instructionPointerIndex++, lineIndex));
                        //NOTE: No checks for infinity loops right now
                    }
                    catch (LocalException e)
                    {
                        errorMessage = e.Message;
                        returnLineIndex = lineIndex;
                        return null;
                    }

                    continue;
                }

                //SetPointerToResult
                if (regex.SetPointerToResult.IsMatch(strings[i]))
                {
                    match = regex.SetPointerToResult.Match(strings[i]);

                    string memoryAddressContext = match.Groups[1].Value;
                    string leftPointingIndex = match.Groups[2].Value;
                    string resultIs_any = match.Groups[3].Value;

                    IGateway selectedGateway = foreignGateway;
                    if (IsLocalMemoryAccess(memoryAddressContext))
                    {
                        selectedGateway = localGateway;
                    }

                    newCodeMemory.Instructions.Add(new SetPointerToResult(new GatewayIndexSet(selectedGateway, int.Parse(leftPointingIndex)), ResultSetResolver(regex, resultIs_any), instructionPointerIndex++, lineIndex));

                    continue;
                }

                //JumpToInstruction
                if (regex.JumpToInstruction.IsMatch(strings[i]))
                {
                    match = regex.JumpToInstruction.Match(strings[i]);

                    string jumpName = match.Groups[1].Value;

                    jumpMemory.AddJumpLabel(jumpName);

                    newCodeMemory.Instructions.Add(new JumpTo(new GatewayIndexSet(localGateway, -1), jumpName, instructionPointerIndex++, lineIndex));

                    continue;
                }

                //JumpToLabel
                if (regex.JumpToLabel.IsMatch(strings[i]))
                {
                    match = regex.JumpToLabel.Match(strings[i]);

                    string jumpName = match.Groups[1].Value;

                    jumpMemory.SetJump(jumpName, instructionPointerIndex); //Does not increment the virtual line index, because not an instruction

                    continue;
                }

                //IfJumpTo
                if (regex.IfJumpTo.IsMatch(strings[i]))
                {
                    match = regex.IfJumpTo.Match(strings[i]);

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
                        newCodeMemory.Instructions.Add(new IfJumpTo(new GatewayIndexSet(localGateway, -1), match.Groups[6].Value, instructionPointerIndex++, lineIndex, set));
                    }
                    catch (LocalException e)
                    {
                        errorMessage = e.Message;
                        returnLineIndex = lineIndex;
                        return null;
                    }

                    continue;
                }

                //Halt
                if (regex.Halt.IsMatch(strings[i]))
                {
                    newCodeMemory.Instructions.Add(new Halt(new GatewayIndexSet(localGateway, -1), instructionPointerIndex++, lineIndex));
                    continue;
                }

                //Instruction not recognized
                errorMessage = ExceptionMessages.CompilationInstructionNotRecognized(strings[i]);
                returnLineIndex = lineIndex;
                return null;
            }

            errorMessage = string.Empty;
            returnLineIndex = -1;
            return newCodeMemory;
        }
    }
}
