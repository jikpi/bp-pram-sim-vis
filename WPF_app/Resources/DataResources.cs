using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_app.Resources
{
    public static class DataResources
    {
        public static PRAM_simulator.PramMachine pram { get; set; }

        static DataResources()
        {
            pram = new PRAM_simulator.PramMachine();
        }
    }
}
