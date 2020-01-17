using System.Text;

namespace RecursiveGeek.DotNetLib.Serial.Com
{
    // Convert Data Types

    public class Conversion
    {
        public static byte[] ToByteArray(string str)
        {
            var encoding = new ASCIIEncoding();

            return encoding.GetBytes(str);
        }

        public static string BytesToAsciiHex(byte[] src)
        {
            var sb = new StringBuilder(string.Empty);
            if (src == null) return sb.ToString();
            foreach (var b in src)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

        public static string BytesToAsciiHex(byte[] src, int length)
        {
            var sb = new StringBuilder(string.Empty);
            if (src == null) return sb.ToString();
            for (var i = 0; i < length; i++)
            {
                sb.AppendFormat("{0:X2}", src[i]);
            }
            return sb.ToString();
        }
    }
}
