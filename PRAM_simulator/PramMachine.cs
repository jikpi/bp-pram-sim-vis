using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.Compiler;
using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Instruction.Other;
using PRAM_lib.Memory;
using PRAM_lib.Processor;
using System.Collections.ObjectModel;

namespace PRAM_simulator
{
    public class PramMachine
    {
        internal SharedMemory SharedMemory { get; private set; }

        internal IOMemory InputMemory { get; private set; }

        internal IOMemory OutputMemory { get; private set; }

        private CodeMemory? MasterCodeMemory { get; set; }

        internal Gateway MasterGateway { get; private set; }

        private CodeCompiler Compiler { get; set; }

        private InstructionRegex InstructionRegex { get; set; }

        public bool IsCompiled => MasterCodeMemory != null;

        public string? CompilationErrorMessage { get; private set; }

        public int? CompilationErrorLineIndex { get; private set; }

        public string? ExecutionErrorMessage { get; private set; }

        public int? ExecutionErrorLineIndex { get; private set; }

        public bool IsHalted {get; private set;}

        public InstructionPointer MPIP { get; private set; } //MasterProcessorInstructionPointer

        public PramMachine()
        {
            SharedMemory = new SharedMemory();
            InputMemory = new IOMemory();
            OutputMemory = new IOMemory();
            Compiler = new CodeCompiler();
            InstructionRegex = new InstructionRegex();
            MPIP = new InstructionPointer(0);


            MasterGateway = new Gateway(SharedMemory, InputMemory, OutputMemory);
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
            MasterCodeMemory = Compiler.Compile(code, InstructionRegex, out ErrorMessage, out ErrorLineIndex);

            if (MasterCodeMemory == null)
            {
                this.CompilationErrorMessage = ErrorMessage;
                this.CompilationErrorLineIndex = ErrorLineIndex;
            }
            else
            {
                this.CompilationErrorMessage = null;
                this.CompilationErrorLineIndex = null;
            }
        }

        public bool ExecuteNextInstruction()
        {
            if (MasterCodeMemory == null)
            {
                ExecutionErrorMessage = "Code is not compiled";
                return false;
            }

            if (MPIP.Value >= MasterCodeMemory.Instructions.Count)
            {
                IsHalted = true;
                ExecutionErrorMessage = "Machine has halted";
                return false;
            }


            try
            {
                MasterCodeMemory.Instructions[MPIP.Value++].Execute(MasterGateway);
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
        }

        public void Clear()
        {
            MPIP.Value = 0;
            SharedMemory = new SharedMemory();
            InputMemory = new IOMemory();
            OutputMemory = new IOMemory();
            MasterGateway = new Gateway(SharedMemory, InputMemory, OutputMemory);
            MasterCodeMemory = null;
        }

    }
}