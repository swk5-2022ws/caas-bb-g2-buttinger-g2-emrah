using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Util
{
    /// <summary>
    /// Informations and a general idea for a cryptographic utility was brought by 
    /// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/security/encrypting-data"/>. 
    /// Detailed information about using this method on a plainText string and not on files was brought by
    /// <see cref="https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp"/>.
    /// </summary>
    public static class CryptographyUtil
    {
        //keysize in bytes
        private const int KEYSIZE = 256;
        private const int BLOCKSIZE = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltedBytes = GetRandomized128Bits();
            var ivBytes = GetRandomized128Bits();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltedBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(KEYSIZE / 8);
                using (Aes aes = Aes.Create())
                {
                    //init AES encryption
                    aes.IV = ivBytes;
                    aes.Key = keyBytes;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var memoryStream = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        //write plaintext in encrypted bytes
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        //get cipheredBytes but prepend salt and iv as they are generated randomly
                        var cipheredBytes = saltedBytes.Concat(ivBytes).Concat(memoryStream.ToArray()).ToArray();

                        //retrieve the sring as base64 encode
                        return Convert.ToBase64String(cipheredBytes);
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            //watch out salt and iv are here prepended see the first 32 bytes for salt and the next 32 for iv
            var plainTextWithSaltAndIvAsBytes = Convert.FromBase64String(cipherText);

            //retrieve salt
            var salt = plainTextWithSaltAndIvAsBytes.Take(BLOCKSIZE / 8).ToArray();
            //skip salt and get iv
            var iv = plainTextWithSaltAndIvAsBytes.Skip(BLOCKSIZE / 8).Take(BLOCKSIZE / 8).ToArray();

            //skip iv and get the cipheredText as bytes.
            var cipherTextBytes = plainTextWithSaltAndIvAsBytes.Skip((BLOCKSIZE / 8) * 2).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, salt, DerivationIterations))
            {
                var keyBytes = password.GetBytes(KEYSIZE / 8);
                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor(keyBytes, iv))
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }

        private static byte[] GetRandomized128Bits() => RandomNumberGenerator.GetBytes(BLOCKSIZE / 8);
    }
}
