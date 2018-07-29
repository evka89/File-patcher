using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch_Maker.Data
{
    public class PatchMakerGeneratingEventArgs
    {
        public int currentDone { get; set; }
        public int countFiles { get; set; }
    }
}
