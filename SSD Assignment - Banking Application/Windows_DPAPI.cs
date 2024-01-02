using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace SSD_Assignment___Banking_Application
{
    internal class Windows_DPAPI
    {

        static readonly int No_Of_IV_Bytes = 16;//16 Bytes => 128 Bits

        public static byte[] Protect(byte[] plaintext, bool includeAdditionalEntropy, DataProtectionScope dpsValue)
        {

            if (includeAdditionalEntropy == false)
            {
                return ProtectedData.Protect(plaintext, null, dpsValue);//null => No IV Value Supplied, Resulting In Deterministically Encrypted Value.
            }

            else
            {
                byte[] iv = RandomNumberGenerator.GetBytes(Windows_DPAPI.No_Of_IV_Bytes);//Generate IV Value
                byte[] ciphertext = ProtectedData.Protect(plaintext, iv, dpsValue);//Encrypt Data Using Windows DPAPI

                byte[] result = new byte[iv.Length + ciphertext.Length];//Byte Array Where IV And Ciphertext Values Will Be Stored Together
                Array.Copy(iv, result, iv.Length);//Store IV Value At Beginning Of The 'result' Array.
                Array.Copy(ciphertext, 0, result, iv.Length, ciphertext.Length);//Store Ciphertext Value Directly After IV Value In The 'result' Array.

                return result;

            }

        }

        public static byte[] Unprotect(byte[] ciphertextInclIV, bool includeAdditionalEntropy, DataProtectionScope dpsValue)
        {
            if (includeAdditionalEntropy == false)
            {
                return ProtectedData.Unprotect(ciphertextInclIV, null, dpsValue);
            }

            else
            {
                byte[] iv = new byte[Windows_DPAPI.No_Of_IV_Bytes];//Byte Array Where IV Value Will Be Extracted To.
                byte[] ciphertext = new byte[ciphertextInclIV.Length - Windows_DPAPI.No_Of_IV_Bytes];//Byte Array Where Ciphertext Value Will Be Extracted To.

                Array.Copy(ciphertextInclIV, iv, iv.Length);//Extract IV Value And Assign To 'iv' Byte Array.
                Array.Copy(ciphertextInclIV, iv.Length, ciphertext, 0, ciphertext.Length);//Extract Ciphertext Value And Assign To 'ciphertext' Byte Array.
                return ProtectedData.Unprotect(ciphertext, iv, dpsValue);//Decrypt Data Using Windows DPAPI.

            }
        }

    }
}
