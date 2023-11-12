﻿using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Jumps;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;
using PRAM_lib.Processor.Interface;
using System.Collections.ObjectModel;

namespace PRAM_lib.Processor
{
    internal class InParallelMachine : IProcessor
    {
        internal MachineMemory LocalMemory { get; private set; }
        internal CodeMemory CodeMemory { get; private set; }
        internal JumpMemory JumpMemory { get; private set; }
        internal ParallelGateway Gateway { get; private set; }
        public string? ExecutionErrorMessage { get; private set; }
        public int? ExecutionErrorLineIndex { get; private set; }
        public bool IsCrashed { get; private set; }
        public bool IsHalted { get; private set; }
        public InstrPointer IP { get; private set; }
        public int ProcessorIndex { get; private set; }


        public InParallelMachine(int processorIndex, CodeMemory codeMemory, JumpMemory jumpMemory, ParallelGateway parallelGateway)
        {
            LocalMemory = new MachineMemory();
            CodeMemory = codeMemory;
            JumpMemory = jumpMemory;
            IP = new InstrPointer(0);
            Gateway = parallelGateway;
            IsCrashed = false;
            IsHalted = false;
            ProcessorIndex = processorIndex;

            Gateway.Memory = LocalMemory;
            Gateway.InstructionPointer = IP;
            Gateway.jumpMemory = JumpMemory;
            Gateway.ParallelIndex = ProcessorIndex;
        }

        public ObservableCollection<MemoryCell> GetMemory()
        {
            return LocalMemory.Cells;
        }

        private bool CheckIfCanContinue()
        {
            //Check if compiled
            if (LocalMemory == null)
            {
                ExecutionErrorMessage = ExceptionMessages.NotCompiled();
                return false;
            }

            //Check if reached the end of code
            if (IP.Value >= CodeMemory.Instructions.Count || IsHalted)
            {
                IsHalted = true;
                ExecutionErrorMessage = ExceptionMessages.HasHalted();
                return false;
            }

            //Check if crashed
            if (IsCrashed)
            {
                ExecutionErrorMessage = ExceptionMessages.HasCrashed();
                return false;
            }

            return true;
        }

        public bool ExecuteNextInstruction()
        {
            if (!CheckIfCanContinue())
            {
                return false;
            }

            try
            {
                //DEBUG: Check for internal inconsistencies regarding the MPIP
                if (IP.Value != CodeMemory!.Instructions[IP.Value].InstructionPointerIndex)
                {
                    throw new Exception("Debug error: MPIP is not equal to the virtual instruction index. Bug in code.");
                }

                CodeMemory!.Instructions[IP.Value].Execute();
                IP.Value++;
            }
            catch (LocalException e)
            {
                ExecutionErrorMessage = e.Message;
                IsCrashed = true;
                return false;
            }


            return true;
        }

        public int GetCurrentCodeLineIndex()
        {
            if (!CheckIfCanContinue())
            {
                return -1;
            }

            return CodeMemory!.Instructions[IP.Value].CodeInstructionLineIndex;
        }



        public void Restart()
        {
            IP.Value = 0;
            IsCrashed = false;
            IsHalted = false;
            ClearMemory();
        }

        public void ClearMemory()
        {
            LocalMemory.Clear();
        }


    }
}
