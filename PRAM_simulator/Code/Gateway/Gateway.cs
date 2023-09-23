using PRAM_lib.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.Gateway
{
    //A class that represents a gateway between a processor and a memory
    internal class Gateway
    {
        internal SharedMemory SharedMemory { get; set; }
        internal IOMemory InputMemory { get; set; }
        internal IOMemory OutputMemory { get; set; }

        public Gateway(SharedMemory sharedMemory, IOMemory inputMemory, IOMemory outputMemory)
        {
            SharedMemory = sharedMemory;
            InputMemory = inputMemory;
            OutputMemory = outputMemory;
        }
    }
}
