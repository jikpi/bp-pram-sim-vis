using PRAM_lib.Code.Gateway.Interface;
using PRAM_lib.Machine.Container;
using PRAM_lib.Machine.InstructionPointer;
using PRAM_lib.Memory;
using System.Reflection.Metadata.Ecma335;

namespace PRAM_lib.Code.Gateway
{
    //A class that represents a gateway between a processor and a memory
    internal class MasterGateway : IGateway
    {
        private MachineMemory SharedMemory { get; set; }
        private IOMemory InputMemory { get; set; }
        private IOMemory OutputMemory { get; set; }
        private InstrPointer InstructionPointer { get; set; }
        private Jumps.JumpMemory JumpMemory { get; set; }
        public event Action<int, int> ParallelDoLaunch;
        public event Action HaltNotify;

        private bool AccessingParallel;

        //Dictionary with memory cell as index, and a list of accesses to that cell by parallel processors
        public Dictionary<int, List<ParallelAccessInfo>> ParallelAccessCycle { get; private set; }
        //Hashset with memory cell index that was accessed, always for a single parallel machine
        private HashSet<int> ReadSingleInstructionAccess;
        private Dictionary<int,int> WriteSingleInstructionAccess;

        public MasterGateway(MachineMemory refSharedMemory,
            IOMemory refInputMemory,
            IOMemory refOutputMemory,
            InstrPointer refInstructionPointer,
            Jumps.JumpMemory refJumpMemory)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;
            ParallelDoLaunch = delegate { };
            HaltNotify = delegate { };

            ReadSingleInstructionAccess = new HashSet<int>();
            WriteSingleInstructionAccess = new Dictionary<int, int>();
            ParallelAccessCycle = new Dictionary<int, List<ParallelAccessInfo>>();

            AccessingParallel = false;
        }

        public void Refresh(MachineMemory refSharedMemory,
            IOMemory refInputMemory,
            IOMemory refOutputMemory,
            InstrPointer refInstructionPointer,
            Jumps.JumpMemory refJumpMemory)
        {
            SharedMemory = refSharedMemory;
            InputMemory = refInputMemory;
            OutputMemory = refOutputMemory;
            InstructionPointer = refInstructionPointer;
            JumpMemory = refJumpMemory;

            ParallelAccessCycle.Clear();
            ReadSingleInstructionAccess.Clear();
            WriteSingleInstructionAccess.Clear();

            AccessingParallel = false;
        }

        // Parallel processors were launched, and will now be accessing memory
        public void AccessingParallelStart()
        {
            AccessingParallel = true;
            ParallelAccessCycle.Clear();
            ReadSingleInstructionAccess.Clear();
            WriteSingleInstructionAccess.Clear();
        }

        // Summarize the memory access of a single parallel processor
        private void SummarizeParallelMemoryAccess(int parallelMachineIndex, int relevantParallelMachineCodeIndex)
        {
            //Add read accesses
            foreach (int index in ReadSingleInstructionAccess)
            {
                if(ParallelAccessCycle.ContainsKey(index) == false)
                {
                    ParallelAccessCycle.Add(index, new List<ParallelAccessInfo>());
                }

                ParallelAccessCycle[index].Add(new ParallelAccessInfo(ParallelAccessType.Read, index, parallelMachineIndex, relevantParallelMachineCodeIndex, null));
            }

            //Add write accesses
            foreach (var entry in WriteSingleInstructionAccess)
            {
                if(ParallelAccessCycle.ContainsKey(entry.Key) == false)
                {
                    ParallelAccessCycle.Add(entry.Key, new List<ParallelAccessInfo>());
                }

                ParallelAccessCycle[entry.Key].Add(new ParallelAccessInfo(ParallelAccessType.Write, entry.Key, parallelMachineIndex, relevantParallelMachineCodeIndex, entry.Value));
            }
        }

        private void NoteSingleReadAccess(int index)
        {
            if (AccessingParallel)
            {
                ReadSingleInstructionAccess.Add(index);
            }
        }

        private void NoteSingleWriteAccess(int index, int value)
        {
            if (AccessingParallel)
            {
                WriteSingleInstructionAccess.Add(index, value);
            }
        }

        //A single parallel processor has finished in the cycle, summarize the memory access
        public void AccessingParallelStepInCycle(int parallelMachineIndex, int parallelMachineRelevantCodeIndex)
        {
            if (AccessingParallel)
            {
                SummarizeParallelMemoryAccess(parallelMachineIndex, parallelMachineRelevantCodeIndex);

                ReadSingleInstructionAccess.Clear();
                WriteSingleInstructionAccess.Clear();
            }
        }

        //A cycle of parallel processors has finished, clear the memory access information
        public void AccessingParallelFinishCycle()
        {
            if (AccessingParallel)
            {
                ParallelAccessCycle.Clear();
                ReadSingleInstructionAccess.Clear();
                WriteSingleInstructionAccess.Clear();
            }
        }

        // The parallel processors have all finished, and the machine is now in a not parallel state
        public void AccessingParallelEnd()
        {
            AccessingParallel = false;
        }

        // Primary read access
        public int Read(int cellIndex)
        {
            NoteSingleReadAccess(cellIndex);
            return SharedMemory.Read(cellIndex).Value;
        }

        // Primary write access
        public void Write(int cellIndex, int value)
        {
            NoteSingleWriteAccess(cellIndex, value);
            SharedMemory.Write(cellIndex, value);
        }

        public int ReadInput(int cellIndex)
        {
            return InputMemory.Read(cellIndex).Value;
        }

        public int ReadInput()
        {
            return InputMemory.Read().Value;
        }

        public void WriteOutput(int value)
        {
            OutputMemory.Write(value);
        }

        public int GetJump(string label)
        {
            return JumpMemory.GetJump(label);
        }

        public void JumpTo(int index)
        {
            InstructionPointer.Value = index;
        }
        public void ParallelDoStart(int count, int index)
        {
            ParallelDoLaunch?.Invoke(count, index);
        }

        public int GetParallelIndex()
        {
            return 0;
        }

        public void Halt()
        {
            HaltNotify?.Invoke();
        }
    }
}
