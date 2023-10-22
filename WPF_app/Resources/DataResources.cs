using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRAM_lib.Machine;

namespace WPF_app.Resources
{
    public static class DataResources
    {
        public static PramMachine PRAM { get; set; }

        static DataResources()
        {
            PRAM = new PramMachine();
        }
    }
}
