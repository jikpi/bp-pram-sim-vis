﻿/*
 * Author: Jan Kopidol
 */

using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Memory.Interface;
using System.Collections.ObjectModel;

namespace PRAM_lib.Memory
{
    internal class IOMemory : IMemory
    {
        private int MemoryPointer { get; set; }
        public ObservableCollection<MemoryCell> Cells { get; set; }
        public IOMemory()
        {
            Cells = new ObservableCollection<MemoryCell>();
            MemoryPointer = 0;
        }

        public MemoryCell Read(int address)
        {
            if (Cells.Count <= address)
            {
                for (int i = Cells.Count; i <= address; i++)
                {
                    Cells.Add(new MemoryCell());
                }
            }

            return Cells[address];
        }

        public MemoryCell Read()
        {
            if (Cells.Count == 0)
            {
                throw new LocalException(ExceptionMessages.IOInputIsEmpty());
            }

            if (MemoryPointer >= Cells.Count)
            {
                return Cells[Cells.Count - 1];
            }

            return Cells[MemoryPointer++];

        }

        public void Write(int value)
        {
            if (MemoryPointer >= Cells.Count)
            {
                for (int i = Cells.Count; i <= MemoryPointer; i++)
                {
                    Cells.Add(new MemoryCell());
                }
            }

            Cells[MemoryPointer++].Value = value;
        }

        public void Write(int address, int value)
        {
            MachineMemory.AddressSanityCheck(address);

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

        public void ResetMemoryPointer()
        {
            MemoryPointer = 0;
        }
    }
}
