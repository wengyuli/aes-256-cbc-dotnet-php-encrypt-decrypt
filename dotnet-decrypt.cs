using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace mqtttest
{
    class MainClass
    {
        static void Main(string[] args)
        {
           

            try
            {
                //EncryptTest();

                DecriptTest();

                Console.ReadKey();

            }
            catch (Exception ex)
            {//ex.StackTrace
                var message = ex.Message;
            }
            //BlockInput(true);
            Console.Read();
        }

        static void DecriptTest(){
            string file = "/Users/wengyuli/origin999.mp4.qs";
            string newFile = "/Users/wengyuli/origin999_dotnet111.mp4";
           
            FileStream fsNew = File.Create(newFile);

            try
            {
                using ( FileStream fs = File.Open(file, FileMode.Open) )
                {

                    int trunkSize = 16 * (1000 + 1);
                    byte[] b = new byte[trunkSize];

                    fs.Read(_iv, 0, 16);

                    while (fs.Position < fs.Length)
                    {
                        var remainlength = fs.Length - fs.Position;

                        if (remainlength > trunkSize)
                        {
                            fs.Read(b, 0, trunkSize);
                        }else{
                            fs.Read(b, 0, (int)remainlength);
                        }
 
                        byte[] tmp = DecryptBytes(b);
                        fsNew.Write(tmp, 0, tmp.Length);

                    }
                }

                fsNew.Close();

            }catch(Exception ex){
                var message = ex.Message;
            }

        }

        static void EncryptTest()
        {

            string file = "/Users/wengyuli/origin.mp4";
            string newFile = "/Users/wengyuli/origin_1048576.mp4.qs";

            FileStream fsNew = File.Create(newFile);
            fsNew.Close();

            using (FileStream fs = File.Open(file, FileMode.Open))
            {
                byte[] bt = new byte[4096];

                while (fs.Read(bt, 0, bt.Length) > 0)
                {
                    string btStr = Convert.ToBase64String(bt);
                    string encrypted64Str = EncryptString(btStr);
                    File.AppendAllText(newFile, encrypted64Str);
                }
            }
            fsNew.Close();
        }
 

        static string _password = "3sc3RLrpd17";

        // Create secret IV
        static byte[] _iv = new byte[16];
        // { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

        public static byte[] DecryptBytes(byte[] cipherBytes)
        {
            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(_password));

            byte[] iv = _iv;

            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;

            // Set key and IV
            byte[] aesKey = new byte[32];
            Array.Copy(key, 0, aesKey, 0, 32);
            encryptor.Key = aesKey;
            encryptor.IV = iv;

            //encryptor.Padding = PaddingMode.None;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            // string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                // byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                new MemoryStream(cipherBytes).Read(_iv, 0, 16);

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();
                return plainBytes;
                // Convert the decrypted byte array to string
                // plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            catch (Exception ex)
            {
                return new byte[0];
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string

        }
         

        public static string EncryptString(string plainText)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(_password));
 
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;

            // Set key and IV
            byte[] aesKey = new byte[32];
            Array.Copy(key, 0, aesKey, 0, 32);
            encryptor.Key = aesKey;
            encryptor.IV = _iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

            // Convert the plainText string into a byte array
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Encrypt the input plaintext string
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            // Complete the encryption process
            cryptoStream.FlushFinalBlock();

            // Convert the encrypted data from a MemoryStream to a byte array
            byte[] cipherBytes = memoryStream.ToArray();

            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();

            // Convert the encrypted byte array to a base64 encoded string
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

            // Return the encrypted data as a string
            return cipherText;
        }


    }
}
