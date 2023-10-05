using PRAM_lib.Code.CustomExceptions;
using PRAM_lib.Code.CustomExceptions.Other;
using PRAM_lib.Memory.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Memory
{
    internal class IOMemory : IMemory
    {
        public int MemoryPointer { get; set; }
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
            if(Cells.Count == 0)
            {
                throw new LocalException(ExceptionMessages.IOInputIsEmpty());
            }

            if(MemoryPointer >= Cells.Count)
            {
                return Cells[Cells.Count - 1];
            }

            return Cells[MemoryPointer++];

        }

        public void Write(int value)
        {
            if(MemoryPointer >= Cells.Count)
            {
                for(int i = Cells.Count; i <= MemoryPointer; i++)
                {
                    Cells.Add(new MemoryCell());
                }
            }

            Cells[MemoryPointer++].Value = value;
        }

        public void Write(int address, int value)
        {
            //An exception that occurs only, when there is bug in code
            throw new Exception("Cannot write to specific address in IO memory");
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
