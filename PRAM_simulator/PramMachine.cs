﻿using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.Compiler;
using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Jumps;
using PRAM_lib.Instruction.Other;
using PRAM_lib.Memory;
using PRAM_lib.Processor;
using PRAM_lib.Processor.Interface;
using System.Collections.ObjectModel;

namespace PRAM_simulator
{
    public class PramMachine : IProcessor
    {
        internal SharedMemory SharedMemory { get; private set; }

        internal IOMemory InputMemory { get; private set; }

        internal IOMemory OutputMemory { get; private set; }

        private CodeMemory? MasterCodeMemory { get; set; }

        private JumpMemory JumpMemory { get; set; }

        internal Gateway MasterGateway { get; private set; }

        private CodeCompiler Compiler { get; set; }

        private InstructionRegex InstructionRegex { get; set; }

        public bool IsCompiled => MasterCodeMemory != null;

        public string? CompilationErrorMessage { get; private set; }

        public int? CompilationErrorLineIndex { get; private set; }

        public string? ExecutionErrorMessage { get; private set; }

        public int? ExecutionErrorLineIndex { get; private set; }
        public bool IsErrored { get; private set; }

        public bool IsHalted { get; private set; }

        public InstructionPointer MPIP { get; private set; }

        public PramMachine()
        {
            SharedMemory = new SharedMemory();
            InputMemory = new IOMemory();
            OutputMemory = new IOMemory();
            Compiler = new CodeCompiler();
            InstructionRegex = new InstructionRegex();
            MPIP = new InstructionPointer(0);
            IsErrored = false;
            IsHalted = false;
            this.JumpMemory = new JumpMemory();



            MasterGateway = new Gateway(SharedMemory, InputMemory, OutputMemory, MPIP, JumpMemory);
        }

        public ObservableCollection<MemoryCell> GetInputMemory()
        {
            return InputMemory.Cells;
        }

        public ObservableCollection<MemoryCell> GetOutputMemory()
        {
            return OutputMemory.Cells;
        }

        public ObservableCollection<MemoryCell> GetSharedMemory()
        {
            return SharedMemory.Cells;
        }

        //Calls the compiler and sets the MasterCodeMemory, or checks if it compiled and sets appropriate flags
        public void Compile(string code)
        {
            string ErrorMessage;
            int ErrorLineIndex;

            MasterCodeMemory = Compiler.Compile(code, InstructionRegex, out JumpMemory newJumpMemory, out ErrorMessage, out ErrorLineIndex);

            if (MasterCodeMemory == null) //Compilation failed
            {
                this.CompilationErrorMessage = ErrorMessage;
                this.CompilationErrorLineIndex = ErrorLineIndex;
            }
            else //Compilation successful
            {
                this.CompilationErrorMessage = null;
                this.CompilationErrorLineIndex = null;

                //Set the new jump memory
                //Note: Standardise this gateway
                this.JumpMemory = newJumpMemory;
                MasterGateway.jumpMemory = JumpMemory;

                MPIP.Value = 0;
                IsHalted = false;
                //SharedMemory = new SharedMemory();
                //InputMemory = new IOMemory();
                //OutputMemory = new IOMemory();
            }
        }

        public bool ExecuteNextInstruction()
        {
            //Check if compiled
            if (MasterCodeMemory == null)
            {
                ExecutionErrorMessage = "Code is not compiled";
                return false;
            }

            //Check if reached the end of code
            if (MPIP.Value >= MasterCodeMemory.Instructions.Count || IsHalted)
            {
                IsHalted = true;
                ExecutionErrorMessage = "Machine has halted";
                return false;
            }


            try
            {
                //DEBUG: Check for internal inconsistencies regarding the MPIP
                if (MPIP.Value != MasterCodeMemory.Instructions[MPIP.Value].InstructionPointerIndex)
                {
                    throw new Exception("Debug error: MPIP is not equal to the virtual instruction index. Bug in code.");
                }

                MasterCodeMemory.Instructions[MPIP.Value].Execute(MasterGateway);
                MPIP.Value++;
            }
            catch (LocalException e)
            {
                ExecutionErrorMessage = e.Message;
                IsHalted = true;
                return false;
            }


            return true;
        }

        public void Restart()
        {
            MPIP.Value = 0;
            IsHalted = false;
            InputMemory.ResetMemoryPointer();
            OutputMemory.ResetMemoryPointer();
        }

        //NOTE: WARNING: Not future proof
        public void Clear()
        {
            MPIP.Value = 0;
            SharedMemory = new SharedMemory();
            InputMemory = new IOMemory();
            OutputMemory = new IOMemory();
            JumpMemory.Clear();
            MasterGateway = new Gateway(SharedMemory, InputMemory, OutputMemory, MPIP, JumpMemory);
            MasterCodeMemory = null;
        }

        public void ClearMemory()
        {
            SharedMemory.Clear();
        }

    }
}