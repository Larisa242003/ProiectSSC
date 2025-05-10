using System;
using System.Text;

public static class HMACSHA256Manual
{
    public static string ComputeHMAC(string key, string message)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        if (keyBytes.Length > 64)
        {
            string hashedKey = SHA256Manual.ComputeHash(Encoding.UTF8.GetString(keyBytes));
            keyBytes = StringToByteArray(hashedKey);
        }

        if (keyBytes.Length < 64)
        {
            Array.Resize(ref keyBytes, 64);
        }

        byte[] o_key_pad = new byte[64];
        byte[] i_key_pad = new byte[64];

        for (int i = 0; i < 64; i++)
        {
            o_key_pad[i] = (byte)(keyBytes[i] ^ 0x5c);
            i_key_pad[i] = (byte)(keyBytes[i] ^ 0x36);
        }

        byte[] innerInput = Combine(i_key_pad, messageBytes);
        string innerHash = SHA256Manual.ComputeHash(Encoding.UTF8.GetString(innerInput));

        byte[] finalInput = Combine(o_key_pad, StringToByteArray(innerHash));
        return SHA256Manual.ComputeHash(Encoding.UTF8.GetString(finalInput));
    }

    private static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] result = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, result, 0, first.Length);
        Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
        return result;
    }

    private static byte[] StringToByteArray(string hex)
    {
        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }
}
