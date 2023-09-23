using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAM_lib.Code.CustomExceptions
{
    internal class LocalException : Exception
    {
        //A custom exception that handles runtime errors, which may occur due to user error. Normal exceptions are not handled, to show debug info.
        public LocalException(string message) : base(message)
        {
        }
    }
}
