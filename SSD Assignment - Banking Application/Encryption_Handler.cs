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
       // private static Aes aesInstance;

        private Aes GenKey(byte[] iv)
        {
            Console.WriteLine("\n\nPassed in IV {0}", string.Join(", ", iv));
            Console.WriteLine("Base64 Encoded IV: " + Convert.ToBase64String(iv));
            Console.WriteLine("\nGenerate and store key");
            Console.WriteLine("-----------------------");

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

            Aes aesInstance = new AesCng(crypto_key_name, key_storage_provider);
            aesInstance.KeySize = 128;
            aesInstance.Mode = CipherMode.CBC;
            aesInstance.Padding = PaddingMode.PKCS7;

            Console.WriteLine("AES Key: " + Convert.ToBase64String(aesInstance.Key)); // print key
            return aesInstance;
        }

        private byte[] EncryptProperty(string propertyValue, Aes aes)
        {
            ICryptoTransform encryptor = aes.CreateEncryptor();

            byte[] valueBytes = Encoding.UTF8.GetBytes(propertyValue);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(valueBytes, 0, valueBytes.Length);
                    csEncrypt.FlushFinalBlock();
                }
                return msEncrypt.ToArray();
            }
        }

        private string DecryptProperty(byte[] encryptedData, Aes aes)
        {
            ICryptoTransform decryptor = aes.CreateDecryptor();

            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                string plaintext = srDecrypt.ReadToEnd();
                return plaintext;
            }
        }

        public Bank_Account EncryptCurrentAccount(Bank_Account originalAccount)
        {
            Aes aes = GenKey(originalAccount.iv);

            Bank_Account encryptedAccount = new Bank_Account
            {
                // Other properties...
                accountNo = Convert.ToBase64String(EncryptProperty(originalAccount.accountNo, aes)),
                name = Convert.ToBase64String(EncryptProperty(originalAccount.name, aes)),
                // Other encrypted properties...
            };

            return encryptedAccount;
        }

        public Bank_Account DecryptCurrentAccount(Bank_Account encryptedAccount, byte[] iv)
        {
            Aes aes = GenKey(iv);

            Bank_Account decryptedAccount = new Bank_Account
            {
                // Other properties...
                accountNo = DecryptProperty(Convert.FromBase64String(encryptedAccount.accountNo), aes),
                name = DecryptProperty(Convert.FromBase64String(encryptedAccount.name), aes),
                // Other decrypted properties...
            };

            return decryptedAccount;
        }
    }
}