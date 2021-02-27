using System;
using System.Text;

public static class StringObfuscation
{
    public static string FromBase64(this string source) => Encoding.UTF8.GetString(Convert.FromBase64String(source));
    
    public static string Caesar(this string source, short shift)
    {
        var maxChar = Convert.ToInt32(char.MaxValue);
        var minChar = Convert.ToInt32(char.MinValue);

        var buffer = source.ToCharArray();

        for (var i = 0; i < buffer.Length; i++)
        {
            var shifted = Convert.ToInt32(buffer[i]) + shift;

            if (shifted > maxChar)
                shifted -= maxChar;
            else if (shifted < minChar) 
                shifted += maxChar;

            buffer[i] = Convert.ToChar(shifted);
        }

        return new string(buffer);
    }
}
