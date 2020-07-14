using System;
using System.Security.Cryptography;
using System.Text;

public static class Md5Utils
{
    private static readonly Lazy<MD5CryptoServiceProvider> LazyProvider = new Lazy<MD5CryptoServiceProvider>(() => new MD5CryptoServiceProvider());
    private static MD5CryptoServiceProvider MD5 => LazyProvider.Value;
    
    public static string Md5Hash(this byte[] bytes) =>  Encoding.UTF8.GetString(MD5.ComputeHash(bytes));

    public static byte[] ToBytes(this int[,] input)
    {
        var byteArray = new byte[input.Length * 2];

        var idx = 0;
        foreach (var v in input)
        {
            var bytes = BitConverter.GetBytes(v);
            byteArray[idx] = bytes[0];
            byteArray[idx + 1] = bytes[1];
            idx += 2;
        }

        return byteArray;
    }
}