/*
 * Author: Jan Kopidol
 */

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
using PRAM_lib.Memory.Interface;
using PRAM_lib.Processor;
using PRAM_lib.Processor.Interface;
using System.Collections.ObjectModel;

namespace PRAM_lib.Machine
{
    /// <summary>
    /// A parallel random access machine.
    /// </summary>
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
        public int CurrentParallelBatchIndex { get; private set; }
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
        public int RunningParallelMachinesCount => LaunchedParallelMachines?.Count(x => !x.IsHalted) ?? 0;
        private bool CRXW { get; set; }
        private bool ERXW { get => !CRXW; set => CRXW = !value; }
        private bool XRCW { get; set; }
        private bool XREW { get => !XRCW; set => XRCW = !value; }
        public List<ParallelAccessInfo> IllegalMemoryAccesses { get; private set; }
        public bool IllegalMemoryAccess => IllegalMemoryAccesses.Count > 0;
        public ParallelAccessError? IllegalMemoryAccessType { get; private set; }
        public int PreviousCodeLineIndex => MPIP.Value > 0 ? MasterCodeMemory!.Instructions[MPIP.Value - 1].CodeInstructionLineIndex : -1;

        public enum CRCW_AccessType
        {
            Common,
            Arbitrary,
            Priority
        }

        public CRCW_AccessType CRCW_Access { get; set; }


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
            CurrentParallelBatchIndex = -1;
            CRXW = true;
            XRCW = true;
            IllegalMemoryAccesses = new List<ParallelAccessInfo>();

            MasterGateway = new MasterGateway(SharedMemory, InputMemory, OutputMemory, MPIP, JumpMemory);
            MasterGateway.ParallelDoLaunch += ParallelDo;
            MasterGateway.HaltNotify += delegate { IsHalted = true; };

            IllegalMemoryAccessType = null;

            CRCW_Access = CRCW_AccessType.Priority;
        }

        /// <summary>
        /// Return the current code line index.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentCodeLineIndex()
        {
            if (!CheckIfCanContinue())
            {
                return -1;
            }

            return MasterCodeMemory!.Instructions[MPIP.Value].CodeInstructionLineIndex;
        }

        /// <summary>
        /// Return the input memory
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<MemoryCell> GetInputMemory()
        {
            return InputMemory.Cells;
        }

        /// <summary>
        /// Return the output memory
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<MemoryCell> GetOutputMemory()
        {
            return OutputMemory.Cells;
        }

        /// <summary>
        /// Return the shared memory
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<MemoryCell> GetSharedMemory()
        {
            return SharedMemory.Cells;
        }

        /// <summary>
        /// Return the memory of the parallel machine at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
            MasterGateway.Refresh(SharedMemory, InputMemory, OutputMemory, MPIP, JumpMemory);
        }

        /// <summary>
        /// Calls the compiler and sets the MasterCodeMemory, or checks if it compiled and sets appropriate flags
        /// </summary>
        /// <param name="code"></param>
        public void Compile(string code)
        {
            string ErrorMessage;
            int ErrorLineIndex;

            try
            {
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
            catch (Exception)
            {
                CompilationErrorMessage = ExceptionMessages.BugCompilationError();
                CompilationErrorLineIndex = null;
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
                if (ExecutionErrorMessage == null)
                {
                    ExecutionErrorMessage = ExceptionMessages.HasCrashed();
                }
                ExecutionErrorLineIndex = MasterCodeMemory.Instructions[MPIP.Value].CodeInstructionLineIndex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Start parallel execution of the parallel machines
        /// </summary>
        /// <param name="count"></param>
        /// <param name="index"></param>
        private void ParallelDo(int count, int index)
        {
            CurrentParallelBatchIndex = index;
            LaunchedParallelMachines = ContainedParallelMachines[index].CreateParallelMachines(count);
            MasterGateway.AccessingParallelStart();
        }

        /// <summary>
        /// Do a single step of parallel execution
        /// </summary>
        /// <param name="relParallelMachine"></param>
        /// <exception cref="Exception"></exception>
        internal void ExecuteNextParallel(out InParallelMachine? relParallelMachine)
        {
            relParallelMachine = null;

            List<InParallelMachine> orderedParallelMachines = new List<InParallelMachine>();

            if (LaunchedParallelMachines == null)
            {
                throw new Exception("Debug error: LaunchedParallelMachines is null. Bug in code.");
            }

            if (CRXW && XRCW)
            {
                //Order the parallel machines based on the CRCW_Access
                if (CRCW_Access == CRCW_AccessType.Priority || CRCW_Access == CRCW_AccessType.Common)
                {
                    orderedParallelMachines = LaunchedParallelMachines.OrderByDescending(x => x.ProcessorIndex).ToList();
                }
                else if (CRCW_Access == CRCW_AccessType.Arbitrary)
                {
                    // Fisher-Yates shuffle
                    orderedParallelMachines = new List<InParallelMachine>(LaunchedParallelMachines);

                    Random rng = new Random();
                    int n = orderedParallelMachines.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        InParallelMachine value = orderedParallelMachines[k];
                        orderedParallelMachines[k] = orderedParallelMachines[n];
                        orderedParallelMachines[n] = value;
                    }
                }
            }
            else
            {
                orderedParallelMachines = LaunchedParallelMachines;
            }

            for (int i = 0; i < orderedParallelMachines!.Count; i++)
            {
                if (orderedParallelMachines[i].IsHalted)
                {
                    continue;
                }

                int previousCodeLineIndex = orderedParallelMachines[i].GetCurrentCodeLineIndex();

                bool result = orderedParallelMachines[i].ExecuteNextInstruction();

                //Note single instruction in parallel access
                MasterGateway.AccessingParallelStepInCycle(orderedParallelMachines[i].ProcessorIndex, previousCodeLineIndex);

                if (!result)
                {
                    if (orderedParallelMachines[i].IsCrashed)
                    {
                        relParallelMachine ??= orderedParallelMachines[i];
                        continue;
                    }

                    if (orderedParallelMachines[i].IsHalted)
                    {
                        continue;
                    }
                }
            }

            if (relParallelMachine != null)
            {
                return;
            }

            //Check memory access of the last cycle, based on the current memory access rules
            if (ERXW)
            {
                foreach (var access in MasterGateway.ParallelAccessCycle)
                {
                    int readCount = access.Value.Count(x => x.Type == ParallelAccessType.Read);
                    if (readCount > 1)
                    {
                        IllegalMemoryAccesses.AddRange(access.Value.Where(x => x.Type == ParallelAccessType.Read));
                        IllegalMemoryAccessType = ParallelAccessError.Read;
                        relParallelMachine = null;
                        return;
                    }
                }
            }

            if (XREW)
            {
                foreach (var access in MasterGateway.ParallelAccessCycle)
                {
                    int writeCount = access.Value.Count(x => x.Type == ParallelAccessType.Write);
                    if (writeCount > 1)
                    {
                        IllegalMemoryAccesses.AddRange(access.Value);
                        IllegalMemoryAccessType = ParallelAccessError.Write;
                        relParallelMachine = null;
                        return;
                    }

                    if (writeCount > 0 && access.Value.Count(x => x.Type == ParallelAccessType.Read) > 0)
                    {
                        IllegalMemoryAccesses.AddRange(access.Value);
                        IllegalMemoryAccessType = ParallelAccessError.Write;
                        relParallelMachine = null;
                        return;
                    }
                }
            }

            //CRCW Common
            if (CRXW && XRCW && CRCW_Access == CRCW_AccessType.Common)
            {
                //Check that if there is a write access, there is no read access and all write accesses have the same .Value
                foreach (var access in MasterGateway.ParallelAccessCycle)
                {
                    if (access.Value.Count(x => x.Type == ParallelAccessType.Write) > 0)
                    {
                        if (access.Value.Count(x => x.Type == ParallelAccessType.Read) > 0)
                        {
                            IllegalMemoryAccesses.AddRange(access.Value);
                            IllegalMemoryAccessType = ParallelAccessError.Common;
                            relParallelMachine = null;
                            return;
                        }

                        if (access.Value.Select(x => x.WriteValue).Distinct().Count() > 1)
                        {
                            IllegalMemoryAccesses.AddRange(access.Value);
                            IllegalMemoryAccessType = ParallelAccessError.Common;
                            relParallelMachine = null;
                            return;
                        }
                    }
                }
            }

            //If no illegal memory access, mark new cycle
            MasterGateway.AccessingParallelFinishCycle();

            if (orderedParallelMachines == null)
            {
                throw new Exception("Debug error: LaunchedParallelMachines is null. Bug in code.");
            }

            //Check if all parallel machines have finished, if so, end parallel.
            if (orderedParallelMachines.All(x => x.IsHalted))
            {
                LaunchedParallelMachines = null;
                relParallelMachine = null;
                CurrentParallelBatchIndex = -1;
                MasterGateway.AccessingParallelEnd();
            }
            else
            {
                relParallelMachine = null;
            }
        }

        /// <summary>
        /// Execute the next instruction
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool ExecuteNextInstruction()
        {
            if (!CheckIfCanContinue())
            {
                return false;
            }

            //Check if there are any parallel machines to handle
            if (LaunchedParallelMachines != null)
            {
                ExecuteNextParallel(out InParallelMachine? relParallelMachine);
                if (relParallelMachine != null)
                {
                    ExecutionErrorMessage = relParallelMachine.ExecutionErrorMessage + $" (Machine index: {relParallelMachine.ProcessorIndex})";
                    ExecutionErrorLineIndex = relParallelMachine.GetCurrentCodeLineIndex() + GetCurrentCodeLineIndex();
                    IsCrashed = true;
                    return false;
                }

                if (IllegalMemoryAccess)
                {
                    if (IllegalMemoryAccessType == null)
                    {
                        throw new Exception("Debug error: Bug in code.");
                    }

                    if (IllegalMemoryAccessType == ParallelAccessError.Read)
                    {
                        ExecutionErrorMessage = ExceptionMessages.IllegalMemoryRead();
                    }
                    else if (IllegalMemoryAccessType == ParallelAccessError.Write)
                    {
                        ExecutionErrorMessage = ExceptionMessages.IllegalMemoryWrite();
                    }
                    else if (IllegalMemoryAccessType == ParallelAccessError.Common)
                    {
                        ExecutionErrorMessage = ExceptionMessages.IllegalMemoryCommon();
                    }

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
                if (MasterCodeMemory != null)
                {
                    ExecutionErrorLineIndex = MasterCodeMemory.Instructions[MPIP.Value].CodeInstructionLineIndex;
                }
                return false;
            }


            return true;
        }
        /// <summary>
        /// Restart the relevant parts of the machine
        /// </summary>
        public void Restart()
        {
            MPIP.Value = 0;
            IsHalted = false;
            IsCrashed = false;
            InputMemory.ResetMemoryPointer();
            OutputMemory.ResetMemoryPointer();
            IllegalMemoryAccesses.Clear();
            ExecutionErrorLineIndex = null;

            CurrentParallelBatchIndex = -1;
            LaunchedParallelMachines = null;

            IllegalMemoryAccessType = null;

            //Restart all parallel machines
            foreach (ParallelMachineContainer container in ContainedParallelMachines)
            {
                foreach (InParallelMachine machine in container.ParallelMachines)
                {
                    machine.Restart();
                }
            }
        }

        /// <summary>
        /// Clear the machine, unused because of binding
        /// </summary>
        public void Clear()
        {
            Restart();

            SharedMemory = new MachineMemory();
            InputMemory = new IOMemory();
            OutputMemory = new IOMemory();
            JumpMemory.Clear();
            ContainedParallelMachines.Clear();
            CurrentParallelBatchIndex = -1;
            LaunchedParallelMachines = null;
            RefreshGateway();
            MasterCodeMemory = null;
            IllegalMemoryAccessType = null;
        }

        /// <summary>
        /// Clear the memory of the machine
        /// </summary>
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

        /// <summary>
        /// Set concurrent read state
        /// </summary>
        /// <param name="state"></param>
        public void SetCRXW(bool state)
        {
            CRXW = state;
            RefreshGateway();
        }

        /// <summary>
        /// Set concurrent write state
        /// </summary>
        /// <param name="state"></param>
        public void SetXRCW(bool state)
        {
            XRCW = state;
            RefreshGateway();
        }

        public string? GetCurrentParallelMachineCode()
        {
            if (LaunchedParallelMachines == null || CurrentParallelBatchIndex == -1)
            {
                return null;
            }

            return ContainedParallelMachines[CurrentParallelBatchIndex].ParallelMachineCode;
        }

        public string GetParallelMachineCode(int index)
        {
            return ContainedParallelMachines[index].ParallelMachineCode;
        }

        private InParallelMachine? GetParallelMachine(int index)
        {
            if (LaunchedParallelMachines == null || index >= LaunchedParallelMachines.Count || index < 0 || CurrentParallelBatchIndex == -1)
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

        // Getters to get the memory object itself
        public IMemory GetInteractiveInputMemory => InputMemory;
        public IMemory GetInteractiveSharedMemory => SharedMemory;
    }
}