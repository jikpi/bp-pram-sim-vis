using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Memory.Interface
{
    internal interface IMemory
    {
        public ObservableCollection<MemoryCell> Cells { get; set; }

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

        public void Write(int address, int value)
        {
            if (Cells.Count <= address)
            {
                for (int i = Cells.Count; i <= address; i++)
                {
                    Cells.Add(new MemoryCell());
                }
            }

            Cells[address].Value = value;
        }

        public void Clear();
    }
}
