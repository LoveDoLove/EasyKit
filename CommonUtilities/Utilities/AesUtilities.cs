using System.Security.Cryptography;
using CommonUtilities.Common;

namespace CommonUtilities.Utilities;

public static class AesUtilities
{
    // WARNING: ECB mode is generally insecure as it doesn't use an IV and identical plaintext blocks encrypt to identical ciphertext blocks.
    // Consider using CBC or GCM mode if security is a high priority.
    public static string Aes256EcbEncrypt(string szText, string szKey)
    {
        try
        {
            byte[] text = Encoding.UTF8.GetBytes(szText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = ConvertUtilities.szConvertKeyToBytes(szKey);
                aes.Mode = CipherMode.ECB; // Insecure mode, see warning above.
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cTransform = aes.CreateEncryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(text, 0, text.Length);
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
            }
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Error during AES ECB encryption: {ex.Message}", ex);
        }
    }

    // WARNING: ECB mode is generally insecure. See comment on Aes256EcbEncrypt.
    public static string Aes256EcbDecrypt(string szText, string szKey)
    {
        try
        {
            byte[] text = Convert.FromBase64String(szText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = ConvertUtilities.szConvertKeyToBytes(szKey);
                aes.Mode = CipherMode.ECB; // Insecure mode, see warning above.
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cTransform = aes.CreateDecryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(text, 0, text.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Error during AES ECB decryption: {ex.Message}", ex);
        }
    }

    public static string Aes256CbcEncrypt(string szText, string szKey, string szIv = Constants.DefaultIv)
    {
        try
        {
            byte[] iv = Encoding.UTF8.GetBytes(szIv);
            if (iv.Length != 16)
                throw new ArgumentException("IV must be 16 bytes long for AES.", nameof(szIv));

            byte[] keyBytes = ConvertUtilities.szConvertKeyToBytes(szKey);
            if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
                throw new ArgumentException("Key must be 16, 24, or 32 bytes long for AES.", nameof(szKey));

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform aesCreateEncrypt = aes.CreateEncryptor())
                using (MemoryStream memoryStream = new())
                {
                    using (CryptoStream cryptoStream = new(memoryStream, aesCreateEncrypt, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(szText);
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    byte[] cipherBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Error during AES CBC encryption: {ex.Message}", ex);
        }
    }

    public static string Aes256CbcDecrypt(string szText, string szKey, string szIv = Constants.DefaultIv)
    {
        try
        {
            byte[] iv = Encoding.UTF8.GetBytes(szIv);
            if (iv.Length != 16)
                throw new ArgumentException("IV must be 16 bytes long for AES.", nameof(szIv));

            byte[] keyBytes = ConvertUtilities.szConvertKeyToBytes(szKey);
            if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
                throw new ArgumentException("Key must be 16, 24, or 32 bytes long for AES.", nameof(szKey));

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform aesCreateDecrypt = aes.CreateDecryptor())
                using (MemoryStream memoryStream = new())
                {
                    using (CryptoStream cryptoStream = new(memoryStream, aesCreateDecrypt, CryptoStreamMode.Write))
                    {
                        byte[] cipherBytes = Convert.FromBase64String(szText);
                        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    byte[] plainBytes = memoryStream.ToArray();
                    return Encoding.UTF8.GetString(plainBytes); // Use UTF8 for consistency
                }
            }
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Error during AES CBC decryption: {ex.Message}", ex);
        }
    }
}