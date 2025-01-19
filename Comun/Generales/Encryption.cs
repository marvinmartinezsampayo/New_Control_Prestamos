using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Generales
{
    public class Encryption
    {
        // Encriptar un string con una llave personalizada
        public static string EncryptString(string plainText, string key)
        {
            // Convertir la llave en bytes (debe tener una longitud de 16, 24 o 32 bytes para AES)
            var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV(); // Generar un vector de inicialización (IV)

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // Escribir el IV al principio del mensaje encriptado
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Desencriptar un string con una llave personalizada
        public static string DecryptString(string cipherText, string key)
        {
            // Convertir la llave en bytes
            var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

            var fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;

                // Leer el IV del principio del mensaje encriptado
                var iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, iv, iv.Length);
                aes.IV = iv;

                var cipherBytes = new byte[fullCipher.Length - iv.Length];
                Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
