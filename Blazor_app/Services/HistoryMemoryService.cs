using PRAM_lib.Memory;
using System.Collections.ObjectModel;


namespace Blazor_app.Services
{
    public class HistoryMemoryService
    {
        public int HistoryIndex { get; private set; } = 0;
        public int MaxHistoryIndex { get; private set; } = 1000;

        private List<ObservableCollection<MemoryCell>> InputHistory { get; set; } = [];
        private List<ObservableCollection<MemoryCell>> OutputHistory { get; set; } = [];
        private List<ObservableCollection<MemoryCell>> SharedMemoryHistory { get; set; } = [];
        private List<int> MasterCodeIndexHistory { get; set; } = [];
        private List<List<ObservableCollection<MemoryCell>>?> ParallelMemoryHistory { get; set; } = [];
        private List<List<int>?> ParallelCodeIndexHistory { get; set; } = [];
        private List<List<bool>> ParallelMachineHaltHistory { get; set; } = [];


        private static void SaveMemory(ObservableCollection<MemoryCell> input, List<ObservableCollection<MemoryCell>> target)
        {
            ObservableCollection<MemoryCell> newCollection = [];
            foreach (var cell in input)
            {
                newCollection.Add(new MemoryCell(cell.Value));
            }
            target.Add(newCollection);
        }

        public void SaveState(PRAM_lib.Machine.PramMachine machine)
        {
            if(HistoryIndex >= MaxHistoryIndex)
            {
                return;
            }

            SaveMemory(machine.GetInputMemory(), InputHistory);
            SaveMemory(machine.GetOutputMemory(), OutputHistory);
            SaveMemory(machine.GetSharedMemory(), SharedMemoryHistory);
            MasterCodeIndexHistory.Add(machine.GetCurrentCodeLineIndex());

            if (!machine.IsRunningParallel)
            {
                ParallelMemoryHistory.Add(null);
                ParallelCodeIndexHistory.Add(null);
            }
            else
            {
                List<ObservableCollection<MemoryCell>>? parallelMemory = [];
                List<int>? parallelIP = new List<int>();
                List<bool> parallelMachineHalt = new List<bool>();
                for (int i = 0; i < machine.ParallelMachinesCount; i++)
                {
                    ObservableCollection<MemoryCell>? currentParMachineMemory = machine.GetParallelMachinesMemory(i);
                    int? currentParMachineCodeLineIndex = machine.GetParallelMachineCodeLineIndex(i);
                    bool currentParMachineHalt = machine.GetParallelMachineIsHalted(i);
                    if (currentParMachineMemory != null && currentParMachineCodeLineIndex != null)
                    {
                        SaveMemory(currentParMachineMemory, parallelMemory);
                        parallelIP.Add(currentParMachineCodeLineIndex.Value);
                    }
                    else
                    {
                        parallelMemory = null;
                        parallelIP = null;
                        break;
                    }
                }
                ParallelMemoryHistory.Add(parallelMemory);
                ParallelCodeIndexHistory.Add(parallelIP);
                ParallelMachineHaltHistory.Add(parallelMachineHalt);
            }

            HistoryIndex++;
        }

        public bool GetAt(int index, out ObservableCollection<MemoryCell>? input,
            out ObservableCollection<MemoryCell>? output,
            out ObservableCollection<MemoryCell>? sharedMemory,
            out int? masterCodeIndex,
            out List<ObservableCollection<MemoryCell>>? parallelMemory,
            out List<int>? parallelCodeIndex)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                input = null;
                output = null;
                sharedMemory = null;
                masterCodeIndex = null;
                parallelMemory = null;
                parallelCodeIndex = null;
                return false;
            }

            input = InputHistory[index];
            output = OutputHistory[index];
            sharedMemory = SharedMemoryHistory[index];
            masterCodeIndex = MasterCodeIndexHistory[index];
            parallelMemory = ParallelMemoryHistory[index];
            parallelCodeIndex = ParallelCodeIndexHistory[index];
            return true;
        }

        public ObservableCollection<MemoryCell>? GetInputAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return null;
            }
            return InputHistory[index];
        }

        public ObservableCollection<MemoryCell>? GetOutputAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return null;
            }
            return OutputHistory[index];
        }

        public ObservableCollection<MemoryCell>? GetSharedMemoryAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return null;
            }
            return SharedMemoryHistory[index];
        }

        public int? GetMasterCodeIndexAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return null;
            }
            return MasterCodeIndexHistory[index];
        }

        public List<ObservableCollection<MemoryCell>>? GetParallelMemoryAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return null;
            }
            return ParallelMemoryHistory[index];
        }

        public List<int>? GetParallelCodeIndexAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return null;
            }
            return ParallelCodeIndexHistory[index];
        }

        public bool GetParallelMachineHaltAt(int index, int machineIndex)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return false;
            }
            return ParallelMachineHaltHistory[index][machineIndex];
        }

        public int GetParallelMachineCountAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return 0;
            }
            return ParallelMemoryHistory[index] != null ? ParallelMemoryHistory[index]!.Count : 0;
        }

        public void Reset()
        {
            HistoryIndex = 0;
            ParallelMemoryHistory.Clear();
            ParallelCodeIndexHistory.Clear();
            InputHistory.Clear();
            OutputHistory.Clear();
            SharedMemoryHistory.Clear();
            MasterCodeIndexHistory.Clear();
        }
    }
}
