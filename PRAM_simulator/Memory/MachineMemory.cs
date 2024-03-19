/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Memory.Interface;
using System.Collections.ObjectModel;

namespace PRAM_lib.Memory
{
    internal class MachineMemory : IMemory
    {
        public ObservableCollection<MemoryCell> Cells { get; set; }
        public static int MaxCellAddress { get; set; } = 1_000_000;

        public MachineMemory()
        {
            Cells = new ObservableCollection<MemoryCell>();
        }

        public static void AddressSanityCheck(int memoryAddress)
        {
            if (memoryAddress < 0)
                throw new LocalException(ExceptionMessages.AddressIsNegative(memoryAddress));
            if (memoryAddress >= MaxCellAddress)
                throw new LocalException(ExceptionMessages.AddressIsTooBig(memoryAddress, MaxCellAddress));
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
