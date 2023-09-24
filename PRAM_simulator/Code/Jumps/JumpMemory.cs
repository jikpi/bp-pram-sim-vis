using PRAM_lib.Code.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.Jumps
{
    internal class JumpMemory
    {
        private Dictionary<string, int> JumpMemoryDictionary { get; set; }

        public JumpMemory()
        {
            JumpMemoryDictionary = new Dictionary<string, int>();
        }

        //A jump like "goto :jump"
        public void AddJumpLabel (string jumpName)
        {
            if (JumpMemoryDictionary.ContainsKey(jumpName))
                return;

            JumpMemoryDictionary.Add(jumpName, -2);
        }

        //Flag like ":jump" on a certain VirtualIndex
        public void SetJump (string jumpName, int jumpVirtualIndex)
        {
            JumpMemoryDictionary[jumpName] = jumpVirtualIndex - 1;
        }

        public int GetJump (string jumpName)
        {
            if(!JumpMemoryDictionary.ContainsKey(jumpName))
                throw new LocalException($"Error: Jump \"{jumpName}\" is not defined.");

            if (JumpMemoryDictionary[jumpName] == -2)
                throw new LocalException($"Error: Jump \"{jumpName}\" is not set.");

            return JumpMemoryDictionary[jumpName];
        }

        public void Clear()
        {
            JumpMemoryDictionary.Clear();
        }
    }
}
