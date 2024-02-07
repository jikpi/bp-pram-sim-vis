using PRAM_lib.Code;
using PRAM_lib.Code.CodeMemory;
using PRAM_lib.Code.Compiler;
using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Code.Gateway;
using PRAM_lib.Code.Jumps;
using PRAM_lib.Machine.Container;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;
using PRAM_lib.Processor;
using PRAM_lib.Processor.Interface;
using System.Collections.ObjectModel;

namespace PRAM_lib.Machine
{
    public class PramMachine : IProcessor
    {
        internal MachineMemory SharedMemory { get; private set; }
        internal IOMemory InputMemory { get; private set; }
        internal IOMemory OutputMemory { get; private set; }
        private CodeMemory? MasterCodeMemory { get; set; }
        private JumpMemory JumpMemory { get; set; }
        internal MasterGateway MasterGateway { get; private set; }
        private CodeCompiler Compiler { get; set; }
        public InstructionRegex InstructionRegex { get; set; }
        public int ParallelBatchIndex { get; private set; }
        private List<ParallelMachineContainer> ContainedParallelMachines { get; set; }
        private List<InParallelMachine>? LaunchedParallelMachines { get; set; }
        public bool IsCompiled => MasterCodeMemory != null;
        public string? CompilationErrorMessage { get; private set; }
        public int? CompilationErrorLineIndex { get; private set; }
        public string? ExecutionErrorMessage { get; private set; }
        public int? ExecutionErrorLineIndex { get; private set; }
        public bool IsCrashed { get; private set; }
        public bool IsHalted { get; private set; }
        public bool IsRunningParallel => LaunchedParallelMachines != null;
        public int ParallelMachinesCount => LaunchedParallelMachines?.Count ?? 0;
        private bool CRXW { get; set; }
        private bool ERXW { get => !CRXW; set => CRXW = !value; }
        private bool XRCW { get; set; }
        private bool XRXW { get => !XRCW; set => XRCW = !value; }
        public List<IllegalMemoryAccesInfo> IllegalMemoryAccesses { get; private set; }
        public bool IllegalMemoryAccess => IllegalMemoryAccesses.Count > 0;
        public int PreviousCodeLineIndex => MPIP.Value > 0 ? MasterCodeMemory!.Instructions[MPIP.Value - 1].CodeInstructionLineIndex : -1;


        //Master Processor Instruction Pointer. Instructions themselves also remember their own IP index (Which is currently only used for validation)
        public InstrPointer MPIP { get; private set; }

        public PramMachine()
        {
            SharedMemory = new MachineMemory();
            InputMemory = new IOMemory();
            OutputMemory = new IOMemory();
            Compiler = new CodeCompiler();
            InstructionRegex = new InstructionRegex();
            MPIP = new InstrPointer(0);
            IsCrashed = false;
            IsHalted = false;
            JumpMemory = new JumpMemory();
            ContainedParallelMachines = new List<ParallelMachineContainer>();
            ParallelBatchIndex = 0;
            CRXW = true;
            XRCW = true;
            IllegalMemoryAccesses = new List<IllegalMemoryAccesInfo>();

            MasterGateway = new MasterGateway(SharedMemory, InputMemory, OutputMemory, MPIP, JumpMemory, CRXW);
            MasterGateway.ParallelDoLaunch += ParallelDo;
            MasterGateway.HaltNotify += delegate { IsHalted = true; };
        }

        public int GetCurrentCodeLineIndex()
        {
            if (!CheckIfCanContinue())
            {
                return -1;
            }

            return MasterCodeMemory!.Instructions[MPIP.Value].CodeInstructionLineIndex;
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

        public ObservableCollection<MemoryCell>? GetParallelMachinesMemory(int index)
        {
            if (LaunchedParallelMachines == null || index >= LaunchedParallelMachines.Count)
            {
                return null;
            }

            return LaunchedParallelMachines[index].GetMemory();
        }

        private void RefreshGateway()
        {
            MasterGateway.Refresh(SharedMemory, InputMemory, OutputMemory, MPIP, JumpMemory, CRXW, XRCW);
        }

        //Calls the compiler and sets the MasterCodeMemory, or checks if it compiled and sets appropriate flags
        public void Compile(string code)
        {
            string ErrorMessage;
            int ErrorLineIndex;

            MasterCodeMemory = Compiler.Compile(code, MasterGateway, InstructionRegex, out JumpMemory newJumpMemory, out List<ParallelMachineContainer> parallelMachines, out ErrorMessage, out ErrorLineIndex);

            if (MasterCodeMemory == null) //Compilation failed
            {
                CompilationErrorMessage = ErrorMessage;
                CompilationErrorLineIndex = ErrorLineIndex;
            }
            else //Compilation successful
            {
                CompilationErrorMessage = null;
                CompilationErrorLineIndex = null;

                //Save the parallel machine containers
                ContainedParallelMachines = parallelMachines;

                //Set the new jump memory
                JumpMemory = newJumpMemory;
                RefreshGateway();

                Restart();
            }
        }

        private bool CheckIfCanContinue()
        {
            //Check if compiled
            if (MasterCodeMemory == null)
            {
                ExecutionErrorMessage = ExceptionMessages.NotCompiled();
                return false;
            }

            //Check if reached the end of code
            if (MPIP.Value >= MasterCodeMemory.Instructions.Count || IsHalted)
            {
                IsHalted = true;
                ExecutionErrorMessage = ExceptionMessages.HasHalted();
                return false;
            }

            //Check if crashed
            if (IsCrashed)
            {
                if(ExecutionErrorMessage == null)
                {
                    ExecutionErrorMessage = ExceptionMessages.HasCrashed();
                }
                ExecutionErrorLineIndex = MasterCodeMemory.Instructions[MPIP.Value].CodeInstructionLineIndex;
                return false;
            }

            return true;
        }

        // Start parallel
        private void ParallelDo()
        {
            LaunchedParallelMachines = ContainedParallelMachines[ParallelBatchIndex].ParallelMachines;
            MasterGateway.AccessingParallelStart();
        }

        internal void ExecuteNextParallel(out InParallelMachine? relParallelMachine, out bool? illegalMemoryAccess)
        {
            illegalMemoryAccess = null;
            for (int i = 0; i < LaunchedParallelMachines!.Count; i++)
            {
                if (LaunchedParallelMachines[i].IsHalted)
                {
                    continue;
                }

                int previousCodeLineIndex = LaunchedParallelMachines[i].GetCurrentCodeLineIndex();
                if (!LaunchedParallelMachines[i].ExecuteNextInstruction())
                {
                    if (LaunchedParallelMachines[i].IsCrashed)
                    {
                        relParallelMachine = LaunchedParallelMachines[i];
                        return;
                    }

                    if (LaunchedParallelMachines[i].IsHalted)
                    {
                        continue;
                    }
                }

                //Note single instruction in parallel access
                MasterGateway.SingleParallelInstructionExecuted(i);

                //Check memory access
                if (MasterGateway.IllegalMemoryReadIndex != null)
                {
                    int illegalReadIndex = MasterGateway.IllegalMemoryReadIndex.Value;

                    IllegalMemoryAccesses.Add(new IllegalMemoryAccesInfo(MasterGateway.ReadAccessed[illegalReadIndex].AccessingParallelMachineIndex,
                        LaunchedParallelMachines[i].GetMemory(), previousCodeLineIndex,
                        readIndex: illegalReadIndex));

                    ExecutionErrorMessage = ExceptionMessages.IllegalMemoryRead();
                    illegalMemoryAccess = true;
                }

                if (MasterGateway.IllegalMemoryWriteIndex != null)
                {
                    int illegalWriteIndex = MasterGateway.IllegalMemoryWriteIndex.Value;

                    IllegalMemoryAccesses.Add(new IllegalMemoryAccesInfo(MasterGateway.WriteAccessed[illegalWriteIndex].AccessingParallelMachineIndex,
                        LaunchedParallelMachines[i].GetMemory(), previousCodeLineIndex,
                        writeIndex: illegalWriteIndex));

                    ExecutionErrorMessage = ExceptionMessages.IllegalMemoryWrite();
                    illegalMemoryAccess = true;
                }

            }

            if (LaunchedParallelMachines == null)
            {
                throw new Exception("Debug error: LaunchedParallelMachines is null. Bug in code.");
            }

            //Check if all parallel machines have finished, if so, end parallel.
            if (LaunchedParallelMachines.All(x => x.IsHalted))
            {
                LaunchedParallelMachines = null;
                ParallelBatchIndex++;
                relParallelMachine = null;
                MasterGateway.AccessingParallelEnd();
            }
            else
            {
                relParallelMachine = null;
            }
        }

        public bool ExecuteNextInstruction()
        {
            if (!CheckIfCanContinue())
            {
                return false;
            }

            //Check if there are any parallel machines to handle
            if (LaunchedParallelMachines != null)
            {
                ExecuteNextParallel(out InParallelMachine? relParallelMachine, out bool? illegalMemoryAccess);
                if (relParallelMachine != null)
                {
                    ExecutionErrorMessage = relParallelMachine.ExecutionErrorMessage;
                    ExecutionErrorLineIndex = relParallelMachine.GetCurrentCodeLineIndex() + GetCurrentCodeLineIndex();
                    IsCrashed = true;
                    return false;
                }

                if (illegalMemoryAccess != null && illegalMemoryAccess.Value)
                {
                    IsCrashed = true;
                    return false;
                }

                return true;
            }

            try
            {
                //DEBUG: Check for internal inconsistencies regarding the MPIP
                if (MPIP.Value != MasterCodeMemory!.Instructions[MPIP.Value].InstructionPointerIndex)
                {
                    throw new Exception("Debug error: MPIP is not equal to the virtual instruction index. Bug in code.");
                }

                MasterCodeMemory.Instructions[MPIP.Value].Execute();
                MPIP.Value++;
            }
            catch (LocalException e)
            {
                ExecutionErrorMessage = e.Message;
                IsCrashed = true;
                if(MasterCodeMemory != null)
                {
                    ExecutionErrorLineIndex = MasterCodeMemory.Instructions[MPIP.Value].CodeInstructionLineIndex;
                }
                return false;
            }


            return true;
        }

        public void Restart()
        {
            MPIP.Value = 0;
            IsHalted = false;
            IsCrashed = false;
            InputMemory.ResetMemoryPointer();
            OutputMemory.ResetMemoryPointer();
            IllegalMemoryAccesses.Clear();
            ExecutionErrorLineIndex = null;

            ParallelBatchIndex = 0;
            LaunchedParallelMachines = null;

            //Restart all parallel machines
            foreach (ParallelMachineContainer container in ContainedParallelMachines)
            {
                foreach (InParallelMachine machine in container.ParallelMachines)
                {
                    machine.Restart();
                }
            }
        }

        public void Clear()
        {
            Restart();

            SharedMemory = new MachineMemory();
            InputMemory = new IOMemory();
            OutputMemory = new IOMemory();
            JumpMemory.Clear();
            ContainedParallelMachines.Clear();
            ParallelBatchIndex = 0;
            LaunchedParallelMachines = null;
            RefreshGateway();
            MasterCodeMemory = null;
        }

        public void ClearMemory()
        {
            SharedMemory.Clear();

            //Clear all parallel machines
            foreach (ParallelMachineContainer container in ContainedParallelMachines)
            {
                foreach (InParallelMachine machine in container.ParallelMachines)
                {
                    machine.ClearMemory();
                }
            }
        }

        public void SetCRXW(bool state)
        {
            CRXW = state;
            RefreshGateway();
        }

        public void SetXRCW(bool state)
        {
            XRCW = state;
            RefreshGateway();
        }

        public string? GetCurrentParallelMachineCode()
        {
            if (LaunchedParallelMachines == null)
            {
                return null;
            }

            return ContainedParallelMachines[ParallelBatchIndex].ParallelMachineCode;
        }

        public string GetParallelMachineCode(int index)
        {
            return ContainedParallelMachines[index].ParallelMachineCode;
        }

        private InParallelMachine? GetParallelMachine(int index)
        {
            if (LaunchedParallelMachines == null || index >= LaunchedParallelMachines.Count)
            {
                return null;
            }

            return LaunchedParallelMachines[index];
        }

        public int? GetParallelMachineCodeLineIndex(int index)
        {
            InParallelMachine? machine = GetParallelMachine(index);

            if (machine == null)
            {
                return null;
            }

            return machine.GetCurrentCodeLineIndex();
        }

        public bool GetParallelMachineIsCrashed(int index)
        {
            InParallelMachine? machine = GetParallelMachine(index);

            if (machine == null)
            {
                return false;
            }

            return machine.IsCrashed;
        }

        public bool GetParallelMachineIsHalted(int index)
        {
            InParallelMachine? machine = GetParallelMachine(index);

            if (machine == null)
            {
                return false;
            }

            return machine.IsHalted;
        }

        public int GetParallelMachineErrorLineIndex(int index)
        {
            InParallelMachine? machine = GetParallelMachine(index);

            if (machine == null)
            {
                return -1;
            }

            return machine.ExecutionErrorLineIndex ?? -1;
        }

        public bool GetParallelMachineIsAfterHalted(int index)
        {
            InParallelMachine? machine = GetParallelMachine(index);

            if (machine == null)
            {
                return false;
            }

            return machine.AfterHaltedExecution;
        }
    }
}