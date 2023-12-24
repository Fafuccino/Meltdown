using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fafuccino_Meltdown
{
    internal class ZaHando
    {
        public static void Clean(string outputPath)  // Cleaning existing files
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
