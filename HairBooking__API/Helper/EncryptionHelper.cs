using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

public class EncryptionHelper
{
    private readonly byte[] _key;

    public EncryptionHelper(IConfiguration configuration)
    {
        var keyString = configuration["EncryptionSettings:Key"];
        if (string.IsNullOrEmpty(keyString) || keyString.Length != 32)
        {
            throw new Exception("Encryption key must be 32 characters long.");
        }
        _key = Encoding.UTF8.GetBytes(keyString);
    }

    public string Encrypt(string text)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;
            aes.GenerateIV(); 
            byte[] iv = aes.IV;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(text);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                byte[] combinedData = new byte[iv.Length + encryptedBytes.Length];
                Buffer.BlockCopy(iv, 0, combinedData, 0, iv.Length);
                Buffer.BlockCopy(encryptedBytes, 0, combinedData, iv.Length, encryptedBytes.Length);

                return Convert.ToBase64String(combinedData);
            }
        }
    }

    public string Decrypt(string encryptedText)
    {
        byte[] combinedData = Convert.FromBase64String(encryptedText);
        
        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;

            // tách IV và dữ liệu mã hóa
            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[combinedData.Length - iv.Length];

            Buffer.BlockCopy(combinedData, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(combinedData, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
