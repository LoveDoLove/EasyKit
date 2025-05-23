using System.Security.Cryptography;
using System.Text.Json;

namespace CommonUtilities.Utilities;

public static class SignatureUtilities
{
    public static string ObjectBase64Signer(long timeStamp, string pemKey, object request)
    {
        string requestBody = JsonSerializer.Serialize(request);
        string strToSign = string.Concat(timeStamp, requestBody);
        byte[] bytesToSign = Encoding.UTF8.GetBytes(strToSign); // Changed to UTF8
        byte[] hash;

        using (SHA256 sha256 = SHA256.Create())
        {
            hash = sha256.ComputeHash(bytesToSign);
        }

        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportFromPem(pemKey); // Pass the PEM-encoded key directly as a string
            byte[] signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }
    }
}