using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Memory.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Memory
{
    internal class SharedMemory : IMemory
    {
        public ObservableCollection<MemoryCell> Cells { get; set; }
        public int MaxCellSize { get; set; }

        public SharedMemory()
        {
            Cells = new ObservableCollection<MemoryCell>();
            MaxCellSize = 1000000;
        }

        public void AddressSanityCheck(int memoryAddress)
        {
            if (memoryAddress < 0)
                throw new LocalException($"Address cannot be negative. Tried to access address at {memoryAddress}");
            if (memoryAddress >= MaxCellSize)
                throw new LocalException($"Address is too big. Tried to access {memoryAddress}, but max size is {MaxCellSize}");
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
