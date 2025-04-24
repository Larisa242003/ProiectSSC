using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    // Calculează hash SHA-256 pentru un fișier
    public static string ComputeFileHash(string path)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(path);
        byte[] hash = sha256.ComputeHash(stream);
        return Convert.ToHexString(hash); // .NET 6+
    }

    static void Main()
{
    Console.WriteLine("Alege opțiunea:");
    Console.WriteLine("1 - Generează hash și salvează");
    Console.WriteLine("2 - Verifică integritatea fișierului");
    Console.Write("Opțiune: ");
    var opt = Console.ReadLine();

    string path = "mesaj.txt";

    if (opt == "1")
    {
        string hash = ComputeFileHash(path);
        File.WriteAllText("hash_original.txt", hash);
        Console.WriteLine("Hash-ul original a fost salvat.");
    }
    else if (opt == "2")
    {
        string receivedHash = File.ReadAllText("hash_original.txt");
        string currentHash = ComputeFileHash(path);

        Console.WriteLine("Hash actual:   " + currentHash);
        Console.WriteLine("Hash salvat:   " + receivedHash);

        if (currentHash == receivedHash)
            Console.WriteLine("\n✅ Fișierul NU a fost modificat.");
        else
            Console.WriteLine("\n❌ Fișierul A FOST modificat!");
    }
    else
    {
        Console.WriteLine("Opțiune invalidă.");
    }
}

}



