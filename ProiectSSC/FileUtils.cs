using System;
using System.IO;

public class FileUtils
{
    public static string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public static void WriteFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    public static void WriteBytesToFile(string filePath, byte[] data)
    {
        File.WriteAllBytes(filePath, data);
    }

    public static byte[] ReadBytesFromFile(string filePath)
    {
        return File.ReadAllBytes(filePath);
    }
}
