using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionHelper(IConfiguration configuration)
    {
        _key = Convert.FromBase64String(configuration["Encryption:Key"]);
        _iv = Convert.FromBase64String(configuration["Encryption:IV"]);
    }

    public string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}



#region "Console Project Code"
//using System;
//using System.Security.Cryptography;

//class Program
//{
//    static void Main()
//    {
//        using (var aes = Aes.Create())
//        {
//            // Generate a new key and IV
//            aes.GenerateKey();
//            aes.GenerateIV();

//            // Convert the key and IV to Base64 strings
//            string base64Key = Convert.ToBase64String(aes.Key);
//            string base64IV = Convert.ToBase64String(aes.IV);

//            // Print the Base64-encoded key and IV
//            Console.WriteLine("Key (Base64): " + base64Key);
//            Console.WriteLine("IV (Base64): " + base64IV);
//        }
//    }
//}
#endregion