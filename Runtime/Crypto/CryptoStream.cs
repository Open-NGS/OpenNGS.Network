using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OpenNGS.Crypto
{
    public class CryptoStream : FileStream
    {
        byte[] Key;

        public CryptoStream(string path, string key, FileMode mode) : base(path, mode, mode == FileMode.Open ? FileAccess.Read : FileAccess.ReadWrite, FileShare.Read)
        {
            Key = Encoding.ASCII.GetBytes(key);
        }

        public override int Read(byte[] array, int offset, int count)
        {
            int pos = (int)base.Position;
            int len = base.Read(array, offset, count);
            if (pos == 0)
            {
                int max = System.Math.Min(array.Length, 512);
                for (int i = 0; i < max; i++)
                {
                    var ki = (pos + i) % Key.Length;
                    array[i] ^= Key[ki];
                }
            }
            return len;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            int pos = (int)base.Position;
            if (pos == 0)
            {
                int max = System.Math.Min(array.Length, 512);
                for (int i = 0; i < max; i++)
                {
                    var ki = (pos + i) % Key.Length;
                    array[i] ^= Key[ki];
                }
            }
            base.Write(array, offset, count);
        }
    }
}
