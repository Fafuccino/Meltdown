// IMPORTS

// VARIABLES

using System;
using Fafuccino_Meltdown;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Artworks made by Shirow Miwa");
        Console.WriteLine("https://www.pixiv.net/en/artworks/5991221");

        ZipperMan bucciarati = new();   // handles the zipping

        if (args.Length > 0)
        {
            // Drag and drop mode
            for (int i = 0; i < args.Length; i++)
            {
                MeltdownGlobal._outputFile = Path.GetFileName(args[i]);

                // you drag and drop the .molten or .moltenX file, the input and ouput folder are the same
                if ((Path.GetExtension(args[i]) == MeltdownGlobal.FILE_EXT) || (Path.GetExtension(args[i]) == MeltdownGlobal.SPECIAL_EXT))
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"Unpacking {i+1} / {args.Length} files...");
                    Console.ForegroundColor = ConsoleColor.White;

                    MeltdownGlobal._inputFolder = Path.GetDirectoryName(args[i]) ?? string.Empty;
                    MeltdownGlobal._inputFolder += "/";
                    MeltdownGlobal._outputFolder = MeltdownGlobal._inputFolder;

                    bucciarati.UnZipperMan(args[i], MeltdownGlobal._outputFolder);
                }
                else
                {
                    // you wanna pack a folder
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"Packing {i + 1} / {args.Length} folders...");
                    Console.ForegroundColor = ConsoleColor.White;

                    MeltdownGlobal._inputFolder = args[i];
                    MeltdownGlobal._outputFolder = Path.GetDirectoryName(MeltdownGlobal._inputFolder) ?? string.Empty;
                    MeltdownGlobal._outputFolder += "/";

                    DirectoryInfo inputFolderInfo = new(MeltdownGlobal._inputFolder);

                    ZaHando.Clean(MeltdownGlobal._outputFolder + MeltdownGlobal._outputFile + MeltdownGlobal.TEMP_FILE_EXT);  // Deletes existing file with the same name as {outputFile}

                    bucciarati.StickyFingers(inputFolderInfo, MeltdownGlobal._outputFile, MeltdownGlobal._outputFolder);
                }
            }
        }
        else
        {
            // input/output folder mode

            // Creates the input and output folder if it doesn't exists yet
            if (!Directory.Exists("./output/"))
            {
                Directory.CreateDirectory("./output/");
            }
            if (!Directory.Exists("./input/"))
            {
                Directory.CreateDirectory("./input/");
            }

            Console.WriteLine($"Current AES Key : {Convert.ToHexString(CrazyDiamond.currentKey.IV)}");
            Console.WriteLine($"Current AES IV : {Convert.ToHexString(CrazyDiamond.currentKey.IV)}");

            Console.WriteLine($"Default AES Key : {Convert.ToHexString(CrazyDiamond.defaultKey.IV)}");
            Console.WriteLine($"Default AES IV : {Convert.ToHexString(CrazyDiamond.defaultKey.IV)}");

            string inputPathMolten = Path.Combine(MeltdownGlobal._inputFolder, MeltdownGlobal._outputFile) + CrazyDiamond.targetExtension();    // Theoretical .molten that we'd want to extract back
            if (File.Exists(inputPathMolten))  // If the file exists, then we want to extract it.
            {
                bucciarati.UnZipperMan(inputPathMolten, MeltdownGlobal._outputFolder);
            }
            else
            {
                DirectoryInfo inputFolderInfo = new(MeltdownGlobal._inputFolder);

                ZaHando.Clean(MeltdownGlobal._outputFolder + MeltdownGlobal._outputFile + MeltdownGlobal.TEMP_FILE_EXT);  // Deletes existing file with the same name as {outputFile}

                bucciarati.StickyFingers(inputFolderInfo, MeltdownGlobal._outputFile, MeltdownGlobal._outputFolder);
            }
            //josuke.SaveKey(CrazyDiamond.KeyGen());
            //Console.WriteLine(System.Text.Encoding.Default.GetString(josuke.currentKey.Key));
            //josuke.Decrypt("./output/Ahegao.molten");
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Everything went smoothly ! :D");
    }
}