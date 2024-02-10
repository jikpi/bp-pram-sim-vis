using PRAM_lib.Processor;

namespace PRAM_lib.Machine.Container
{
    internal class ParallelMachineContainer
    {
        internal List<InParallelMachine> ParallelMachines { get; private set; }
        public string ParallelMachineCode { get; private set; }
        public ParallelMachineContainer(List<InParallelMachine> inParallelMachines, string parallelMachineCode)
        {
            ParallelMachines = inParallelMachines;
            ParallelMachineCode = parallelMachineCode;
        }

        public List<InParallelMachine> CreateParallelMachines(int count)
        {
            ParallelMachines[0].Restart();
            List<InParallelMachine> inParallelMachines = new List<InParallelMachine>();
            List<InParallelMachine> inParallelMachinesCopies = ParallelMachines[0].DeepCopy(count - 1);
            inParallelMachines.Add(ParallelMachines[0]);
            inParallelMachines.AddRange(inParallelMachinesCopies);

            return inParallelMachines;
        }
    }
}