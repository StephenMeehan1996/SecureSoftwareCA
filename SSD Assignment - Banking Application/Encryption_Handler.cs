using Banking_Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SSD_Assignment___Banking_Application
{
    public class Encryption_Handler
    {
        private Aes aesInstance;

        public  Aes genKey(byte[] iv)

        {
            Console.WriteLine("\n\nPassed in IV {0}", string.Join(", ", iv));
            Console.WriteLine("Base64 Encoded IV: " + Convert.ToBase64String(iv));
            Console.WriteLine("\nGenerate and store key");
            Console.WriteLine("-----------------------");

            if (aesInstance == null) // if AES instance is null get/generate key
            {

                String crypto_key_name = "key123 test9999";
                CngProvider key_storage_provider = CngProvider.MicrosoftSoftwareKeyStorageProvider;
                if (!CngKey.Exists(crypto_key_name, key_storage_provider))
                {
                    Console.WriteLine("Inside Key Gen IF");
                    CngKeyCreationParameters key_creation_parameters = new CngKeyCreationParameters()
                    {
                        Provider = key_storage_provider
                    };

                    CngKey.Create(new CngAlgorithm("AES"), crypto_key_name, key_creation_parameters);
                }

                aesInstance = new AesCng(crypto_key_name, key_storage_provider);
                aesInstance.KeySize = 128;
                aesInstance.Mode = CipherMode.CBC;
                aesInstance.Padding = PaddingMode.PKCS7;
            }

            Console.WriteLine("AES Key: " + Convert.ToBase64String(aesInstance.Key)); // print key
            return aesInstance;
        }

        public byte[] CreateIV()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] iv = new byte[16];//Randomly Generate 128-Bit IV Value To Be Used In Modes Other Than ECB.
            rng.GetBytes(iv);
            return iv;
        }

        static byte[] Encrypt(byte[] plaintext_data, Aes aes)
        {

            byte[] ciphertext_data;//Byte Array Where Result Of Encryption Operation Will Be Stored.

            ICryptoTransform encryptor = aes.CreateEncryptor();//Object That Contains The AES Encryption Algorithm (Using The Key and IV Value Specified In The AES Object). 

            MemoryStream msEncrypt = new MemoryStream();//MemoryStream That Will Store The Output Of The Encryption Operation.

            /*
                Calling The Write() Method On The CryptoStream Object Declared Below Will Store/Write 
                The Result Of The Encryption Operation To The Memory Stream Object Specified In The Constructor.
            */
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            csEncrypt.Write(plaintext_data, 0, plaintext_data.Length);//Writes All Data Contained In plaintext_data Byte Array To CryptoStream (Which Then Gets Encrypted And Gets Written to the msEncrypt MemoryStream).
            csEncrypt.Dispose();//Closes CryptoStream

            ciphertext_data = msEncrypt.ToArray();//Output Result Of Encryption Operation In Byte Array Form.
            msEncrypt.Dispose();//Closes MemoryStream

            return ciphertext_data;

        }

        static byte[] Decrypt(byte[] ciphertext_data, Aes aes)
        {

            byte[] plaintext_data;//Byte Array Where Result Of Decryption Operation Will Be Stored.

            ICryptoTransform decryptor = aes.CreateDecryptor();//Object That Contains The AES Decryption Algorithm (Using The Key and IV Value Specified In The AES Object). 

            MemoryStream msDecrypt = new MemoryStream();//MemoryStream That Will Store The Output Of The Decryption Operation.

            /*
                Calling The Write() Method On The CryptoStream Object Declared Below Will Store/Write 
                The Result Of The Decryption Operation To The Memory Stream Object Specified In The Constructor.
            */
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);//Writes All Data Contained In Byte Array To CryptoStream (Which Then Gets Decrypted).
            csDecrypt.Write(ciphertext_data, 0, ciphertext_data.Length);//Writes All Data Contained In ciphertext_data Byte Array To CryptoStream (Which Then Gets Decrypted And Gets Written to the msDecrypt MemoryStream).
            csDecrypt.Dispose();//Closes CryptoStream

            plaintext_data = msDecrypt.ToArray();//Output Result Of Decryption Operation In Byte Array Form.
            msDecrypt.Dispose();//Closes MemoryStream

            return plaintext_data;

        }

        private static string EncryptProperty(string propertyValue, Aes aes)
        {
            ICryptoTransform encryptor = aes.CreateEncryptor();

            MemoryStream msEncrypt = new MemoryStream();


            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                byte[] valueBytes = Encoding.ASCII.GetBytes(propertyValue);
                csEncrypt.Write(valueBytes, 0, valueBytes.Length);
                csEncrypt.Dispose();//Closes CryptoStream
            }
            msEncrypt.Dispose();//Closes MemoryStream

            return Convert.ToBase64String(msEncrypt.ToArray());

            //MemoryStream msEncrypt = new MemoryStream();


            //using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            //{
            //    byte[] valueBytes = Encoding.ASCII.GetBytes(propertyValue);
            //    csEncrypt.Write(valueBytes, 0, valueBytes.Length);
            //    csEncrypt.Dispose();//Closes CryptoStream
            //}
            //msEncrypt.Dispose();//Closes MemoryStream

            //return Convert.ToBase64String(msEncrypt.ToArray());

        }

        private string DecryptProperty(string propertyValue, Aes aes)
        {

            //////////////

            byte[] valueBytes = Convert.FromBase64String(propertyValue); //converts back into byte array 
            ICryptoTransform decryptor = aes.CreateDecryptor();

            MemoryStream msDecrypt = new MemoryStream();

            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);
            csDecrypt.Write(valueBytes, 0, valueBytes.Length);
            csDecrypt.Dispose();//Closes CryptoStream

            byte[] plaintext_data = msDecrypt.ToArray();
            string text = Encoding.ASCII.GetString(plaintext_data);
            msDecrypt.Dispose();//Closes

            return text;





        }

        public Bank_Account EncrypCurrentAccount(Bank_Account originalAccount)
        {
            Aes aes = genKey(originalAccount.iv);

          
                Current_Account encryptedAccount = new Current_Account
                {
                    accountNo = EncryptProperty(originalAccount.accountNo, aes),
                    name = EncryptProperty(originalAccount.name, aes),
                    address_line_1 = EncryptProperty(originalAccount.address_line_1, aes),
                    address_line_2 = EncryptProperty(originalAccount.address_line_2, aes),
                    address_line_3 = EncryptProperty(originalAccount.address_line_3, aes),
                    town = originalAccount.town,
                    balance = originalAccount.balance,
                    iv = originalAccount.iv
                };
              
                return encryptedAccount;
            

         
        }

        public Bank_Account DecrypCurrentAccount(Bank_Account originalAccount)
        {
            Aes aes = genKey(originalAccount.iv);
          
                Current_Account encryptedAccount = new Current_Account
                {
                    accountNo = DecryptProperty(originalAccount.accountNo, aes),
                    name = DecryptProperty(originalAccount.name, aes),
                    address_line_1 = DecryptProperty(originalAccount.address_line_1, aes),
                    address_line_2 = DecryptProperty(originalAccount.address_line_2, aes),
                    address_line_3 = DecryptProperty(originalAccount.address_line_3, aes),
                    town = originalAccount.town,
                    balance = originalAccount.balance,
                    iv = originalAccount.iv
                };

                return encryptedAccount;
            }
        
    }
}