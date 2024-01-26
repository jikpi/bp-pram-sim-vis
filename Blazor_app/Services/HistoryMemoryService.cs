using PRAM_lib.Memory;
using System.Collections.ObjectModel;


namespace Blazor_app.Services
{
    public class HistoryMemoryService
    {
        public int HistoryIndex { get; private set; } = 0;

        private List<ObservableCollection<MemoryCell>> InputHistory { get; set; } = [];
        private List<ObservableCollection<MemoryCell>> OutputHistory { get; set; } = [];
        private List<ObservableCollection<MemoryCell>> SharedMemoryHistory { get; set; } = [];
        private List<int> MasterCodeIndexHistory { get; set; } = [];
        private List<List<ObservableCollection<MemoryCell>>?> ParallelMemoryHistory { get; set; } = [];
        private List<List<int>?> ParallelCodeIndexHistory { get; set; } = [];


        private static void SaveMemory(ObservableCollection<MemoryCell> input, List<ObservableCollection<MemoryCell>> target)
        {
            ObservableCollection<MemoryCell> newCollection = [];
            foreach (var cell in input)
            {
                newCollection.Add(new MemoryCell(cell.Value));
            }
            target.Add(newCollection);
        }

        public void SaveNextStep(PRAM_lib.Machine.PramMachine machine)
        {
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
                for (int i = 0; i < machine.ParallelMachinesCount; i++)
                {
                    ObservableCollection<MemoryCell>? currentParMachineMemory = machine.GetParallelMachinesMemory(i);
                    int? currentParMachineCodeLineIndex = machine.GetParallelMachineCodeLineIndex(i);
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
