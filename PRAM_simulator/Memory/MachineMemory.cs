using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Memory.Interface;
using System.Collections.ObjectModel;

namespace PRAM_lib.Memory
{
    internal class MachineMemory : IMemory
    {
        public ObservableCollection<MemoryCell> Cells { get; set; }
        public int MaxCellSize { get; set; }

        public MachineMemory()
        {
            Cells = new ObservableCollection<MemoryCell>();
            MaxCellSize = 1_000_000;
        }

        public void AddressSanityCheck(int memoryAddress)
        {
            if (memoryAddress < 0)
                throw new LocalException(ExceptionMessages.AddressIsNegative(memoryAddress));
            if (memoryAddress >= MaxCellSize)
                throw new LocalException(ExceptionMessages.AddressIsTooBig(memoryAddress, MaxCellSize));
        }

        public MemoryCell Read(int address)
        {
            AddressSanityCheck(address);

            if (Cells.Count <= address)
            {
                for (int i = Cells.Count; i <= address; i++)
                {
                    Cells.Add(new MemoryCell());
                }
            }

            return Cells[address];
        }

        public void Write(int address, int value)
        {
            AddressSanityCheck(address);

            if (Cells.Count <= address)
            {
                for (int i = Cells.Count; i <= address; i++)
                {
                    Cells.Add(new MemoryCell());
                }
            }

            Cells[address].Value = value;
        }

        public void Clear()
        {
            Cells.Clear();
        }
    }
}
