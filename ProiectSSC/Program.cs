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

    static void ShowMenu()
    {
        Console.WriteLine("\n==================== MENIU ====================");
        Console.WriteLine("1 - Generează hash SHA-256 și salvează");
        Console.WriteLine("2 - Verifică integritatea cu SHA-256");
        Console.WriteLine("3 - Generează HMAC și salvează");
        Console.WriteLine("4 - Verifică integritatea cu HMAC");
        Console.WriteLine("0 - Ieșire");
        Console.WriteLine("===============================================");
        Console.Write("Opțiune: ");
    }

    static string ReadSecretInput()
{
    StringBuilder input = new StringBuilder();
    ConsoleKeyInfo key;

    while (true)
    {
        key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }
        else if (key.Key == ConsoleKey.Backspace)
        {
            if (input.Length > 0)
            {
                input.Length--;
                Console.Write("\b \b");
            }
        }
        else
        {
            input.Append(key.KeyChar);
            Console.Write("*");
        }
    }

    return input.ToString();
}


    static void Main()
    {
        string path = "mesaj.txt";
        string opt;

        do
        {
            ShowMenu();
            opt = Console.ReadLine()!;
            Console.WriteLine(); // spațiu între meniu și rezultat

            switch (opt)
            {
                case "1":
                    string hash = ComputeFileHash(path);
                    File.WriteAllText("hash_original.txt", hash);
                    Console.WriteLine("🔐 Hash SHA-256 a fost generat și salvat.");
                    break;

                case "2":
                    string savedHash = File.ReadAllText("hash_original.txt");
                    string currentHash = ComputeFileHash(path);
                    Console.WriteLine("Hash actual:   " + currentHash);
                    Console.WriteLine("Hash salvat:   " + savedHash);
                    Console.WriteLine(currentHash == savedHash
                        ? "\n✅ Fișierul NU a fost modificat."
                        : "\n❌ Fișierul A FOST modificat!");
                    break;

                case "3":
                    Console.Write("Introduceți cheia secretă: ");
                    string keyHmac = ReadSecretInput();
                    string hmac = ComputeHMAC(path, keyHmac);
                    File.WriteAllText("hmac_original.txt", hmac);
                    Console.WriteLine("🔐 HMAC a fost generat și salvat.");
                    break;

                case "4":
                    Console.Write("Introduceți cheia secretă: ");
                    string keyVerify = ReadSecretInput();
                    string savedHmac = File.ReadAllText("hmac_original.txt");
                    string currentHmac = ComputeHMAC(path, keyVerify);
                    Console.WriteLine("HMAC actual:   " + currentHmac);
                    Console.WriteLine("HMAC salvat:   " + savedHmac);
                    Console.WriteLine(currentHmac == savedHmac
                        ? "\n✅ Fișierul este autentic și NU a fost modificat."
                        : "\n❌ Fișierul A FOST modificat sau cheia e greșită!");
                    break;

                case "0":
                    Console.WriteLine("👋 La revedere!");
                    break;

                default:
                    Console.WriteLine("⚠️ Opțiune invalidă. Încearcă din nou.");
                    break;
            }

            if (opt != "0")
            {
                Console.WriteLine("\nApasă Enter pentru a continua...");
                Console.ReadLine();
                Console.Clear();
            }

        } while (opt != "0");
    }
}


