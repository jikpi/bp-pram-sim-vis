namespace PRAM_lib.Code.Gateway.Interface
{
    //A class that represents a gateway between a processor and a memory, and contextualizes a simple memory access
    //in the context of the processor. So, a PRAM will access a shared memory with it, and parallel processors will access their own
    //memory with it.
    internal interface IGateway
    {
        //Reads a value from a memory at specified index
        int Read(int index);
        //Writes a value to a memory at specified index
        void Write(int index, int value);

        //IO memory
        int ReadInput(int index);
        int ReadInput();
        void WriteOutput(int value);

        //Jumps
        int GetJump(string label);
        void JumpTo(int index);

        //Notify PRAM about the launch of a parallel processors
        void ParallelDoStart(int count, int index);

        int GetParallelIndex();

        //Halt
        void Halt();
    }
}
