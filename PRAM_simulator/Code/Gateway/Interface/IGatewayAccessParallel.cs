using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.Gateway.Interface
{
    //A class that represents a gateway between a parallel processor and shared memory, and contextualizes the memory access.
    // The gateway monitors memory access
    internal interface IGatewayAccessParallel
    {
        public object PRead(int memoryIndex);
        public void PWrite(int memoryIndex, int value);
    }
}
