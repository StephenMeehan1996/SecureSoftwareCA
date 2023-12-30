using Banking_Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;


namespace SSD_Assignment___Banking_Application
{
    public class Encryption_Handler
    {
        public Aes GenOrCreateKey(byte[]? iv, string mode){

            String crypto_key_name = "SSD_Encryption_Key_2023";
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

                if(mode == "CBC"){
                    aesInstance.Mode = CipherMode.CBC;
                    aesInstance.IV = iv;
                }
                else {
                    aesInstance.Mode = CipherMode.ECB;
                }
               
            aesInstance.Padding = PaddingMode.PKCS7;
            return aesInstance;
        }
        public byte[] CreateIV()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] iv = new byte[16];//Randomly Generate 128-Bit IV Value To Be Used In Modes Other Than ECB.
            rng.GetBytes(iv);
            return iv;
        }

        private static string EncryptProperty(string propertyValue, Aes aes)
        {
            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] ciphertext_data;
            byte[] plaintext_data = Encoding.UTF8.GetBytes(propertyValue);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(plaintext_data, 0, plaintext_data.Length);
                }
                ciphertext_data = msEncrypt.ToArray();
            }

            return Convert.ToBase64String(ciphertext_data);

        }

        private string DecryptProperty(string propertyValue, Aes aes)
        {
            byte[] valueBytes = Convert.FromBase64String(propertyValue); // Convert from Base64 to byte array
            byte[] plaintext_data;

            ICryptoTransform decryptor = aes.CreateDecryptor();

            using (MemoryStream msDecrypt = new MemoryStream(valueBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream output = new MemoryStream())
                    {
                        csDecrypt.CopyTo(output);
                        plaintext_data = output.ToArray();
                    }
                }
            }

            // Convert byte array to string using UTF-8 encoding
            string text = Encoding.UTF8.GetString(plaintext_data);
            return text;
        }

        public Current_Account EncrypCurrentAccount(Current_Account originalAccount)
        {
            Aes aes = GenOrCreateKey(originalAccount.iv, "CBC");
            Aes ecb = GenOrCreateKey(originalAccount.iv, "ECB");

            Current_Account encryptedAccount = new Current_Account
                {
                    accountNo = EncryptProperty(originalAccount.accountNo, ecb),
                    name = EncryptProperty(originalAccount.name, aes),
                    address_line_1 = EncryptProperty(originalAccount.address_line_1, aes),
                    address_line_2 = EncryptProperty(originalAccount.address_line_2, aes),
                    address_line_3 = EncryptProperty(originalAccount.address_line_3, aes),
                    town = EncryptProperty(originalAccount.town, aes),
                    balance = originalAccount.balance,
                    overdraftAmount= originalAccount.overdraftAmount,
                    iv = originalAccount.iv
                };
            aes = null;
            ecb = null;
            GC.Collect();
            return encryptedAccount;
        }

        public Savings_Account EncryptSavingsAccount(Savings_Account originalAccount)
        {
            Aes aes = GenOrCreateKey(originalAccount.iv, "CBC");
            Aes ecb = GenOrCreateKey(originalAccount.iv, "ECB");


            Savings_Account encryptedAccount = new Savings_Account
            {
                accountNo = EncryptProperty(originalAccount.accountNo, ecb),
                name = EncryptProperty(originalAccount.name, aes),
                address_line_1 = EncryptProperty(originalAccount.address_line_1, aes),
                address_line_2 = EncryptProperty(originalAccount.address_line_2, aes),
                address_line_3 = EncryptProperty(originalAccount.address_line_3, aes),
                town = EncryptProperty(originalAccount.town, aes),
                balance = originalAccount.balance,
                interestRate = originalAccount.interestRate,
                iv = originalAccount.iv,
            };
            aes.Clear();
            ecb.Clear();
            GC.Collect();
            return encryptedAccount;
        }

        public Current_Account DecrypCurrentAccount(Current_Account originalAccount)
        {
            Aes aes = GenOrCreateKey(originalAccount.iv, "CBC");
            Aes ecb = GenOrCreateKey(originalAccount.iv, "ECB");

            Current_Account decryptedAccount = new Current_Account {
                    accountNo = DecryptProperty(originalAccount.accountNo, ecb),
                    name = DecryptProperty(originalAccount.name, aes),
                    address_line_1 = DecryptProperty(originalAccount.address_line_1, aes),
                    address_line_2 = DecryptProperty(originalAccount.address_line_2, aes),
                    address_line_3 = DecryptProperty(originalAccount.address_line_3, aes),
                    town = DecryptProperty(originalAccount.town, aes),
                    balance = originalAccount.balance,
                    overdraftAmount = originalAccount.overdraftAmount,
                    iv = originalAccount.iv,
                };
            aes.Clear();
            ecb.Clear();
            GC.Collect();
            return decryptedAccount;
         }

        public Savings_Account DecryptSavingsAccount(Savings_Account originalAccount) {

            Aes aes = GenOrCreateKey(originalAccount.iv, "CBC");
            Aes ecb = GenOrCreateKey(originalAccount.iv, "ECB");

            Savings_Account decryptedAccount = new Savings_Account
            {
                accountNo = DecryptProperty(originalAccount.accountNo, ecb),
                name = DecryptProperty(originalAccount.name, aes),
                address_line_1 = DecryptProperty(originalAccount.address_line_1, aes),
                address_line_2 = DecryptProperty(originalAccount.address_line_2, aes),
                address_line_3 = DecryptProperty(originalAccount.address_line_3, aes),
                town = DecryptProperty(originalAccount.town, aes),
                balance = originalAccount.balance,
                interestRate = originalAccount.interestRate,
                iv = originalAccount.iv
            
            };

            aes.Clear();
            ecb.Clear();
            GC.Collect();
            return decryptedAccount;
        }

       public string serializeObject(Bank_Account b) {
        
            byte[] secretKey = Encoding.UTF8.GetBytes("SSD_HashKey_2023"); //protect in memory 

           // ProtectKeyInMemory(secretKey);

            string serializedObject = JsonConvert.SerializeObject(b);

            // Hashing the serialized object using HMACSHA256
            byte[] hashedData = ComputeHMACSHA256(serializedObject, secretKey);

            string hash = Convert.ToBase64String(hashedData);

            return hash;
        }

        static byte[] ComputeHMACSHA256(string data, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return hash;
            }
        }

       //public static void ProtectKeyInMemory(byte[] secretKey)
       // {
       //     try
       //     {
       //         // Protect the secret key in memory
       //         ProtectedMemory.Protect(secretKey, MemoryProtectionScope.SameLogon);
       //         Console.WriteLine("Secret key protected in memory.");
       //     }
       //     catch (CryptographicException e)
       //     {
       //         Console.WriteLine("Error protecting secret key in memory: " + e.Message);
       //     }
       // }

        public string EncryptForAccountSearch(string accountNum) {

            Aes key = GenOrCreateKey(null, "ECB");

            return EncryptProperty(accountNum, key);

        }

    }
}