namespace PRAM_lib.Code.Gateway.Interface
{
    /// <summary>
    /// A class that represents a gateway between a processor and a memory, and contextualizes a simple memory access
    ///  in the context of the processor. So, a PRAM will access a shared memory with it, and parallel processors will access their ownmemory with it.
    /// </summary>

    internal interface IGateway
    {
        /// <summary>
        /// Reads a value from a memory at specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int Read(int index);
        /// <summary>
        /// Writes a value to a memory at specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        void Write(int index, int value);

        /// <summary>
        /// IO memory
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int ReadInput(int index);
        int ReadInput();
        void WriteOutput(int value);

        /// <summary>
        /// Jumps
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        int GetJump(string label);
        void JumpTo(int index);

        /// <summary>
        /// Notify PRAM about the launch of a parallel processors
        /// </summary>
        /// <param name="count"></param>
        /// <param name="index"></param>
        void ParallelDoStart(int count, int index);

        int GetParallelIndex();

        ///Halt
        void Halt();
    }
}
