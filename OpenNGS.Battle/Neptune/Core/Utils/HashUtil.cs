using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class HashUtil
{
    public enum HashType
    {
        MD5,
        SHA1,
    }

    public static string ComputeFileHash(string fileName, HashType type)
    {
        if (!System.IO.File.Exists(fileName))
            return string.Empty;

        HashAlgorithm algorithm = (type == HashType.SHA1 ? (HashAlgorithm)SHA1.Create() : (HashAlgorithm)MD5.Create());

        byte[] data = File.ReadAllBytes(fileName);
        data = algorithm.ComputeHash(data);
        return BytesToHexString(data);
    }


    public static string ComputeHash(string str, HashType type)
    {
        return ComputeHash(str, type, Encoding.ASCII);
    }

    public static string ComputeHash(string str, HashType type, Encoding encoding)
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
        byte[] hashBytes = HashUtil.ComputeHash(inputBytes, HashUtil.HashType.MD5);
        return BytesToHexString(hashBytes);
    }

    public static byte[] ComputeHash(byte[] data, HashType type)
    {
        HashAlgorithm algorithm = (type == HashType.SHA1 ? (HashAlgorithm)SHA1.Create() : (HashAlgorithm)MD5.Create());
        return algorithm.ComputeHash(data);
    }

    /// <summary>
    /// 字节数组转换为16进制表示的字符串
    /// </summary>
    public static string BytesToHexString(byte[] buf)
    {
        return System.BitConverter.ToString(buf).Replace("-", "").ToLower();
    }
}