using PRAM_lib.Code.Gateway.Interface;

namespace PRAM_lib.Code.Gateway
{
    internal class GatewayIndexSet
    {
        private IGateway Gateway { get; set; }
        private int MemoryAddressIndex { get; set; }
        public GatewayIndexSet(IGateway gateway, int memoryAddressIndex)
        {
            Gateway = gateway;
            MemoryAddressIndex = memoryAddressIndex;
        }

        public int ReadInput(int? inputIndex)
        {
            if (inputIndex.HasValue)
            {
                return Gateway.ReadInput(inputIndex.Value);
            }
            else
            {
                return Gateway.ReadInput();
            }
        }

        public void WriteOutput(int value)
        {
            Gateway.WriteOutput(value);
        }

        public int Read()
        {
            return Gateway.Read(MemoryAddressIndex);
        }

        public void Write(int value)
        {
            Gateway.Write(MemoryAddressIndex, value);
        }

        public int Read(int address)
        {
            return Gateway.Read(address);
        }

        public void Write(int address, int value)
        {
            Gateway.Write(address, value);
        }

        public int GetJump(string label)
        {
            return Gateway.GetJump(label);
        }

        public void JumpTo(int index)
        {
            Gateway.JumpTo(index);
        }

        public void ParallelDo()
        {
            Gateway.ParallelDoStart();
        }

        public int GetParallelIndex()
        {
            return Gateway.GetParallelIndex();
        }
    }
}
