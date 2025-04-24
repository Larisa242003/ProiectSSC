using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    public static string ComputeFileHash(string path)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(path);
        byte[] hash = sha256.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    public static string ComputeHMAC(string path, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        using var hmac = new HMACSHA256(keyBytes);
        using var stream = File.OpenRead(path);
        byte[] hash = hmac.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    static void Main()
    {
        Console.WriteLine("Alege opțiunea:");
        Console.WriteLine("1 - Generează hash SHA-256 și salvează");
        Console.WriteLine("2 - Verifică integritatea cu SHA-256");
        Console.WriteLine("3 - Generează HMAC și salvează");
        Console.WriteLine("4 - Verifică integritatea cu HMAC");
        Console.Write("Opțiune: ");
        string opt = Console.ReadLine()!;


        string path = "mesaj.txt";

        if (opt == "1")
        {
            string hash = ComputeFileHash(path);
            File.WriteAllText("hash_original.txt", hash);
            Console.WriteLine("🔐 Hash SHA-256 a fost salvat.");
        }
        else if (opt == "2")
        {
            string saved = File.ReadAllText("hash_original.txt");
            string current = ComputeFileHash(path);
            Console.WriteLine("Hash actual:   " + current);
            Console.WriteLine("Hash salvat:   " + saved);
            Console.WriteLine(current == saved
                ? "\n✅ Fișierul NU a fost modificat."
                : "\n❌ Fișierul A FOST modificat!");
        }
        else if (opt == "3")
        {
            Console.Write("Introduceți cheia secretă: ");
            string key = Console.ReadLine()!;
            string hmac = ComputeHMAC(path, key);
            File.WriteAllText("hmac_original.txt", hmac);
            Console.WriteLine("🔐 HMAC a fost generat și salvat.");
        }
        else if (opt == "4")
        {
            Console.Write("Introduceți cheia secretă: ");
            string key = Console.ReadLine()!;
            string saved = File.ReadAllText("hmac_original.txt");
            string current = ComputeHMAC(path, key);
            Console.WriteLine("HMAC actual:   " + current);
            Console.WriteLine("HMAC salvat:   " + saved);
            Console.WriteLine(current == saved
                ? "\n✅ Fișierul este autentic și NU a fost modificat."
                : "\n❌ Fișierul A FOST modificat sau cheia e greșită!");
        }
        else
        {
            Console.WriteLine("Opțiune invalidă.");
        }
    }
}



