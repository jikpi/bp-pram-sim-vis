/*
 * Author: Jan Kopidol
 */

namespace PRAM_lib.Machine.Container
{
    public enum ParallelAccessType
    {
        Read,
        Write
    }

    public enum ParallelAccessError
    {
        Read,
        Write,
        Common
    }
    /// <summary>
    /// Class that holds information about parallel access to memory. Is checked for illegal access.
    /// </summary>
    public class ParallelAccessInfo
    {
        public ParallelAccessType Type { get; private set; }
        public int MemoryIndex { get; private set; }
        public int ParallelMachineIndex { get; private set; }
        public int CodeLineIndex { get; private set; }

        public int? WriteValue { get; set; }

        public ParallelAccessInfo(ParallelAccessType type, int index, int parallelMachineIndex, int codeLineIndex, int? writeValue)
        {
            Type = type;
            MemoryIndex = index;
            ParallelMachineIndex = parallelMachineIndex;
            CodeLineIndex = codeLineIndex;
            WriteValue = writeValue;
        }
    }
}
