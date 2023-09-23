using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.CustomExceptions
{
    internal class LocalException : Exception
    {
        public LocalException(string message) : base(message)
        {
        }
    }
}
