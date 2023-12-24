using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fafuccino_Meltdown
{
    using static MeltdownGlobal;
    internal class ZipperMan
    {
        public void StickyFingers(DirectoryInfo inputFolderInfo, string outputFile, string outputFolder)
        {
            outputFile += TEMP_FILE_EXT;    // Adds the .moltmp extension

            FileInfo[] filePaths = inputFolderInfo.GetFiles("*.*", SearchOption.AllDirectories);

            string outputPath = Path.Combine(outputFolder, outputFile);

            Console.WriteLine(String.Format("Creating '{0}' ...", outputFile));

            using var zip = ZipFile.Open(outputPath, ZipArchiveMode.Create);

            for (int i = 0; i < filePaths.Length; i++)  // Adds the files to the .moltmp
            {
                string path = filePaths[i].FullName;
                string fileName = Path.GetFileName(path);

                Console.WriteLine(String.Format("Compressing '{0}' ... | {1}/{2} files", fileName, i+1, filePaths.Length));

                zip.CreateEntryFromFile(path, Path.GetRelativePath(inputFolderInfo.FullName, path));

                Console.WriteLine(String.Format("Successfully added '{0}' ... | {1}/{2} files", fileName, i+1, filePaths.Length));
            }

            zip.Dispose();  // Frees the file so it can be encrypted

            Console.WriteLine("Attempting encryption");

            CrazyDiamond.Encrypt(_outputFolder + outputFile);
        }

        public void UnZipperMan(string filePath, string outputFolder)
        {

            CrazyDiamond.Decrypt(filePath, outputFolder); // Decrypts the .molten into a .moltmp

            string fileName = Path.GetFileNameWithoutExtension(filePath) + TEMP_FILE_EXT;

            filePath = Path.Combine(outputFolder, fileName);

            Console.WriteLine(String.Format("Preparing to unpack {0} ...", fileName));

            try
            {
                using var zip = ZipFile.OpenRead(filePath); // Reads the .moltmp

                string extractTo = outputFolder + Path.GetFileNameWithoutExtension(fileName);

                if (!Directory.Exists(extractTo))  // Creates a new folder to extract everything to
                {
                    Directory.CreateDirectory(extractTo);
                }

                for (int i = 0; i < zip.Entries.Count; i++)
                {
                    ZipArchiveEntry entry = zip.Entries[i];

                    string entryOutFolder = Path.Combine(extractTo, Path.GetDirectoryName(entry.FullName) ?? string.Empty);

                    if (!Directory.Exists(entryOutFolder))  // handles the folders
                    {
                        Directory.CreateDirectory(entryOutFolder);
                    }

                    if (!string.IsNullOrEmpty(entry.Name))  // unpack the file
                    {
                        string entryOutPath = Path.Combine(entryOutFolder, entry.Name);

                        Console.WriteLine(String.Format("Unpacking {0} ... | {1}/{2} files", entry.Name, i + 1, zip.Entries.Count));

                        entry.ExtractToFile(entryOutPath, true);
                    }
                }
                zip.Dispose();  // Frees the file so it can get obliterated

                ZaHando.Clean(filePath);    // Deletes the temporary file.
            }
            catch (Exception)
            {
                Console.WriteLine("You are very likely using the wrong keys.");
                Console.WriteLine("I hope you didn't lost them ! Otherwise... sorry mate..");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You're fucked. ＞_＜");

                Thread.Sleep(2000); // gives the user some time to realized they're fucked

                Console.ForegroundColor = ConsoleColor.White;

                ZaHando.Clean(filePath);    // Deletes the temporary file.
                throw;
            }
        }
    }
}
