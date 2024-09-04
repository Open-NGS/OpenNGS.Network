using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class MemoryStreamExternsion
{

    public static void WriteString(this MemoryStream ms, string val)
    {
        var byteCount = Encoding.UTF8.GetByteCount(val);
        byte[] buffer = Encoding.UTF8.GetBytes(val);
        ms.Write(buffer, 0, byteCount);
    }

    public static void WriteInt(this MemoryStream ms, int val)
    {
        byte[] buffer = BitConverter.GetBytes(val);
        ms.Write(buffer, 0, buffer.Length);
    }

}