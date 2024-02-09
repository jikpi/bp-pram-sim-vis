using PRAM_lib.Memory;
using System.Collections.ObjectModel;


namespace Blazor_app.Services
{
    public class HistoryMemoryService
    {
        public int HistoryIndex { get; private set; } = 0;
        public int MaxHistoryIndex { get; private set; } = 500;

        private List<ObservableCollection<MemoryCell>> InputHistory { get; set; } = [];
        private List<ObservableCollection<MemoryCell>> OutputHistory { get; set; } = [];
        private List<ObservableCollection<MemoryCell>> SharedMemoryHistory { get; set; } = [];
        private List<int> MasterCodeIndexHistory { get; set; } = [];
        private List<int?> ParallelBatchIndex { get; set; } = []; 
        private List<List<ObservableCollection<MemoryCell>>?> ParallelMemoryHistory { get; set; } = [];
        private List<List<int>?> ParallelCodeIndexHistory { get; set; } = [];
        private List<List<bool>?> ParallelMachineHaltHistory { get; set; } = [];


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
                InputHistory.RemoveAt(0);
                OutputHistory.RemoveAt(0);
                SharedMemoryHistory.RemoveAt(0);
                MasterCodeIndexHistory.RemoveAt(0);
                ParallelBatchIndex.RemoveAt(0);
                ParallelMemoryHistory.RemoveAt(0);
                ParallelCodeIndexHistory.RemoveAt(0);
                ParallelMachineHaltHistory.RemoveAt(0);
                HistoryIndex--;
            }

            SaveMemory(machine.GetInputMemory(), InputHistory);
            SaveMemory(machine.GetOutputMemory(), OutputHistory);
            SaveMemory(machine.GetSharedMemory(), SharedMemoryHistory);
            MasterCodeIndexHistory.Add(machine.GetCurrentCodeLineIndex());

            if (!machine.IsRunningParallel)
            {
                ParallelBatchIndex.Add(null);
                ParallelMemoryHistory.Add(null);
                ParallelCodeIndexHistory.Add(null);
                ParallelMachineHaltHistory.Add(null);
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
                        parallelMachineHalt.Add(currentParMachineHalt);
                    }
                    else
                    {
                        parallelMemory = null;
                        parallelIP = null;
                        break;
                    }
                }
                ParallelBatchIndex.Add(machine.ParallelBatchIndex);
                ParallelMemoryHistory.Add(parallelMemory);
                ParallelCodeIndexHistory.Add(parallelIP);
                ParallelMachineHaltHistory.Add(parallelMachineHalt);
            }

            HistoryIndex++;
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
            if (ParallelMachineHaltHistory[index] == null)
            {
                return false;
            }
            return ParallelMachineHaltHistory[index]![machineIndex];
        }

        public int GetParallelMachineCountAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return 0;
            }
            return ParallelMemoryHistory[index] != null ? ParallelMemoryHistory[index]!.Count : 0;
        }

        public int? GetParallelBatchIndexAt(int index)
        {
            if (index < 0 || index >= HistoryIndex)
            {
                return null;
            }
            return ParallelBatchIndex[index];
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
            ParallelMachineHaltHistory.Clear();
            ParallelBatchIndex.Clear();
        }
    }
}
