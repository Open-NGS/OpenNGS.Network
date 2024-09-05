using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MemoryStreamExternsion
{
    public static string ReadString(this MemoryStream ms)
    {
        short len = ms.ReadInt16();
        byte[] buffer = new byte[len];
        ms.Read(buffer, 0, len);
        return Encoding.UTF8.GetString(buffer, 0, len);
    }

    public static void WriteString(this MemoryStream ms, string val)
    {
        var byteCount = Encoding.UTF8.GetByteCount(val);
        byte[] buffer = Encoding.UTF8.GetBytes(val);
        ms.WriteInt16((short)byteCount);
        ms.Write(buffer, 0, byteCount);
    }

    public static int ReadInt(this MemoryStream ms)
    {
        byte[] buffer = new byte[4];
        ms.Read(buffer, 0, 4);
        return BitConverter.ToInt32(buffer);
    }

    public static void WriteInt(this MemoryStream ms, int val)
    {
        byte[] buffer = BitConverter.GetBytes(val);
        ms.Write(buffer, 0, buffer.Length);
    }

    public static short ReadInt16(this MemoryStream ms)
    {
        byte[] buffer = new byte[2];
        ms.Read(buffer, 0, 2);
        return BitConverter.ToInt16(buffer);
    }

    public static void WriteInt16(this MemoryStream ms, short val)
    {
        byte[] buffer = BitConverter.GetBytes(val);
        ms.Write(buffer, 0, buffer.Length);
    }
}