using PRAM_lib.Code.Gateway.Interface;

namespace PRAM_lib.Code.Gateway
{
    internal class GatewayIndexSet
    {
        private IGatewayAccessLocal Gateway { get; set; }
        private int MemoryAddressIndex { get; set; }
        public GatewayIndexSet(IGatewayAccessLocal gateway, int memoryAddressIndex)
        {
            Gateway = gateway;
            MemoryAddressIndex = memoryAddressIndex;
        }

        public int Read()
        {
            return Gateway.Read(MemoryAddressIndex);
        }

        public void Write(int value)
        {
            Gateway.Write(MemoryAddressIndex, value);
        }
    }
}
