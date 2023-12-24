using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Fafuccino_Meltdown
{
    using static MeltdownGlobal;
    internal class CrazyDiamond
    {
        public static (byte[] Key, byte[] IV) defaultKey;   // Stores the defalt key
        public static (byte[] Key, byte[] IV) currentKey;  // Stores the current key

        static CrazyDiamond()
        {
            defaultKey = LoadDefaultKey(); // Loads the default key

            if (!Directory.Exists("./config/")) // Creates the config folder if it doesn't exists yet and adds the default keys
            {
                Directory.CreateDirectory("./config/");
                SaveKey(defaultKey);
                generateConfigReadme();
            }

            // Loads the previous key from the files or generates a new one if a file is missing
            if (File.Exists("./config/key") && File.Exists("./config/IV"))
            {
                LoadKey();
            }
            else
            {
                currentKey = KeyGen();
            }
        }

        public static (byte[] Key, byte[] IV) KeyGen()
            // Generates a new key
        {
            using (Aes aes = Aes.Create())
            {
                SaveKey((aes.Key, aes.IV));
                return (aes.Key, aes.IV);
            }
        }

        public static void SaveKey((byte[] Key, byte[] IV) aesKey)
            // Saves the key to ./config
        {
            using (BinaryWriter keyFile = new BinaryWriter(File.OpenWrite("./config/key")))
            {
                keyFile.Write(aesKey.Key);
            }
            using (BinaryWriter ivFile = new BinaryWriter(File.OpenWrite("./config/IV")))
            {
                ivFile.Write(aesKey.IV);
            }
        }

        public static void LoadKey()
            // Loads the key from ./config
        {
            currentKey.Key = File.ReadAllBytes("./config/key");
            currentKey.IV = File.ReadAllBytes("./config/IV");
        }

        public static (byte[] Key, byte[] IV) LoadDefaultKey()
        // Loads the default key
        {
            byte[] key = Convert.FromHexString("A5193318DCEF82CEE0337E06069F356863F50AAF4683BC378F1BD44D9E691202");
            byte[] iv = Convert.FromHexString("7E0B710EC24309C2B18F586F09E65D2A");
            return (key, iv);
        }

        public static (string Key, string IV) GetKey()
        // Returns the hexadecimal string of the current key
        {
            return (Convert.ToHexString(currentKey.Key), Convert.ToHexString(currentKey.IV));
        }

        public static void PrintKey()
        {
            Console.WriteLine($"Current AES Key : {Convert.ToHexString(CrazyDiamond.currentKey.IV)}");
            Console.WriteLine($"Current AES IV : {Convert.ToHexString(CrazyDiamond.currentKey.IV)}");
            Console.WriteLine("\n");
            Console.WriteLine($"Default AES Key : {Convert.ToHexString(CrazyDiamond.defaultKey.IV)}");
            Console.WriteLine($"Default AES IV : {Convert.ToHexString(CrazyDiamond.defaultKey.IV)}");
        }

        public static void Encrypt(string filePath)
        {
            using (FileStream fsInput = new FileStream(filePath, FileMode.Open))
            {
                using (FileStream fsOutput = new FileStream(Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, Path.GetFileNameWithoutExtension(filePath)) + targetExtension(), FileMode.Create))
                {
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Padding = PaddingMode.Zeros;

                        ICryptoTransform encryptor = aesAlg.CreateEncryptor(currentKey.Key, currentKey.IV);

                        using (CryptoStream csEncrypt = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                        {
                            fsInput.CopyTo(csEncrypt);
                        }
                    }
                }
            }
            ZaHando.Clean(filePath);    // Deletes the temporary file.

            Console.WriteLine($"{Path.GetFileNameWithoutExtension(_outputFile) + targetExtension()} created successfully.");
        }

        public static void Decrypt(string sourceFilePath, string outputFolder)
        {
            Int16 extensionType = sourceExtension(sourceFilePath);

            if (extensionType == -1)
            {
                Console.WriteLine("Unsupported file type.");
            }
            else
            {
                byte[] decryptKey;
                byte[] decryptIV;

                using (FileStream fsInput = new FileStream(sourceFilePath, FileMode.Open))
                {
                    using (FileStream fsOutput = new FileStream(Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(sourceFilePath)) + TEMP_FILE_EXT, FileMode.Create))
                    {
                        using (Aes aesAlg = Aes.Create())
                        {
                            aesAlg.Padding = PaddingMode.Zeros;

                            if (extensionType == 0)
                            {
                                // .molten file, default key
                                decryptKey = defaultKey.Key;
                                decryptIV = defaultKey.IV;
                            }
                            else
                            {
                                // .moltenx file, current key
                                decryptKey = currentKey.Key;
                                decryptIV = currentKey.IV;
                            }

                            ICryptoTransform decrypt = aesAlg.CreateDecryptor(decryptKey, decryptIV);

                            using (CryptoStream csDecrypt = new CryptoStream(fsOutput, decrypt, CryptoStreamMode.Write))
                            {
                                fsInput.CopyTo(csDecrypt);
                            }
                        }
                    }
                }
            }
        }

        public static Int16 sourceExtension(string filePath)
        {
            Int16 type;
            string extension = Path.GetExtension(filePath);

            if (extension == FILE_EXT)  // .molten
            {
                type = 0;
            }
            else if (extension == SPECIAL_EXT)  // .moltenx
            {
                type = 1;
            }
            else
            {
                type = -1;  // Unsupported file type
            }

            return type;
        }

        public static string targetExtension()
        {
            if (Equals(Convert.ToHexString(currentKey.Key), Convert.ToHexString(defaultKey.Key)))   // if both are the same, then we want a .molten
            {
                return FILE_EXT;
            }
            else
            {
                return SPECIAL_EXT; // else, we want a .moltenX
            }
        }
    }
}
