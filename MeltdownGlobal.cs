using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fafuccino_Meltdown
{
    public static class MeltdownGlobal
    {
        public const string TEMP_FILE_EXT = ".moltmp";
        public const string FILE_EXT = ".molten";
        public const string SPECIAL_EXT = ".moltenx";

        public static string _inputFolder = "./input/";
        public static string _outputFolder = "./output/";
        public static string _outputFile = "output";

        public static void generateConfigReadme()
        {
            string[] lines = {
                "These files allow you to unpack a moltenX file.",
                "If you loose them, you're fucked.",
                "I suggest you pack them in a .zip file before",
                "using another AES key.\n",
                "By default, these are the Default Keys.",
                "So, if you try to pack a file, it'll spit out",
                "a .molten file, with the default key.",
                "However, if you wish to change the keys and",
                "make a .moltenX file instead, just delete both",
                "the 'key' file and the 'IV' file.",
                "If you want the default keys back, just delete",
                "the 'config' folder. It'll reset everything."
            };
            using (StreamWriter readMe = new StreamWriter("./config/README.txt"))
            {
                foreach (string line in lines)
                {
                    readMe.WriteLine(line);
                }
            }
        }
    }
}
