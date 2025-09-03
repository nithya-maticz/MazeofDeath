using System;
using System.Security.Cryptography;
using System.Text;

[System.Serializable]
public class AESUtility 
{
   // private static readonly string encryptionKey = "POk90+21+@POt24+"; 

    // Encrypt a string
    public static string Encrypt(string plainText,string secretKey)
    {
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = new byte[16]; 
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes); 
        }
    }

    // Decrypt a string
    public static string Decrypt(string encryptedText, string secretKey)
    {
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = new byte[16]; 
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes); 
        }

    }
}
