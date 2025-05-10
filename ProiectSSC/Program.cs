using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    // Calculează hash-ul unui fișier folosind algoritmul specificat (SHA-256 sau SHA-512)
    public static string ComputeFileHash(string path, string algorithm)
    {
        // Creează instanța de algoritm corespunzătoare
        using HashAlgorithm hasher = algorithm switch
        {
            "SHA256" => SHA256.Create(),
            "SHA512" => SHA512.Create(),
            _ => throw new ArgumentException("Algoritm invalid.") // Aruncă excepție pentru algoritmi neacceptați
        };

        // Deschide fișierul pentru citire
        using var stream = File.OpenRead(path);

        // Calculează hash-ul fișierului
        byte[] hash = hasher.ComputeHash(stream);

        // Convertește rezultatul în șir hexazecimal
        return Convert.ToHexString(hash);
    }

    // Calculează HMAC-ul unui fișier folosind cheia secretă și algoritmul specificat
    public static string ComputeHMAC(string path, string key, string algorithm)
    {
        // Codifică cheia secretă în bytes
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        // Creează instanța de HMAC corespunzătoare
        using HMAC hmac = algorithm switch
        {
            "SHA256" => new HMACSHA256(keyBytes),
            "SHA512" => new HMACSHA512(keyBytes),
            _ => throw new ArgumentException("Algoritm invalid.") // Aruncă excepție pentru algoritmi neacceptați
        };

        // Deschide fișierul pentru citire
        using var stream = File.OpenRead(path);

        // Calculează HMAC-ul
        byte[] hash = hmac.ComputeHash(stream);

        // Convertește rezultatul în șir hexazecimal
        return Convert.ToHexString(hash);
    }

    // Afișează meniul principal
    static void ShowMenu()
    {
        Console.WriteLine("\n==================== MENIU ====================");
        Console.WriteLine("1 - Generează hash și salvează");
        Console.WriteLine("2 - Verifică integritatea cu hash");
        Console.WriteLine("3 - Generează HMAC și salvează");
        Console.WriteLine("4 - Verifică integritatea cu HMAC");
        Console.WriteLine("0 - Ieșire");
        Console.WriteLine("===============================================");
        Console.Write("Opțiune: ");
    }

    // Citește un input secret de la utilizator (nu afișează caracterele tastate)
    static string ReadSecretInput()
    {
        StringBuilder input = new StringBuilder();
        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(intercept: true); // Ascunde tasta apăsată
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
                    Console.Write("\b \b"); // Șterge caracterul afișat cu "*"
                }
            }
            else
            {
                input.Append(key.KeyChar);
                Console.Write("*"); // Afișează simbolul "*" pentru fiecare caracter
            }
        }

        return input.ToString();
    }

    // Permite alegerea algoritmului de hash/HMAC de la utilizator
    static string SelectAlgorithm(string context)
    {
        Console.Write($"Alege algoritmul pentru {context} (1 = SHA-256, 2 = SHA-512): ");
        string choice = Console.ReadLine()!;
        return choice == "2" ? "SHA512" : "SHA256";
    }

    // Funcția principală care rulează aplicația
    static void Main()
    {
        // Se cere calea către fișierul pe care se vor aplica operațiile
        Console.Write("Introduceți calea către fișier: ");
        string path = Console.ReadLine()!;
        string opt;

        // Buclă principală de meniu
        do
        {
            ShowMenu();
            opt = Console.ReadLine()!;
            Console.WriteLine();

            try
            {
                switch (opt)
                {
                    case "1":
                        // Generează hash și îl salvează în fișier
                        string algo1 = SelectAlgorithm("hash");
                        string hash = ComputeFileHash(path, algo1);
                        File.WriteAllText("hash_original.txt", hash);
                        Console.WriteLine("🔐 Hash a fost generat și salvat.");
                        break;

                    case "2":
                        // Verifică integritatea fișierului comparând hash-ul actual cu cel salvat
                        string algo2 = SelectAlgorithm("hash");
                        string savedHash = File.ReadAllText("hash_original.txt");
                        string currentHash = ComputeFileHash(path, algo2);
                        Console.WriteLine("Hash actual:   " + currentHash);
                        Console.WriteLine("Hash salvat:   " + savedHash);
                        Console.WriteLine(currentHash == savedHash
                            ? "\n✅ Fișierul NU a fost modificat."
                            : "\n❌ Fișierul A FOST modificat!");
                        break;

                    case "3":
                        // Generează HMAC și îl salvează în fișier
                        string algo3 = SelectAlgorithm("HMAC");
                        Console.Write("Introduceți cheia secretă: ");
                        string keyHmac = ReadSecretInput();
                        string hmac = ComputeHMAC(path, keyHmac, algo3);
                        File.WriteAllText("hmac_original.txt", hmac);
                        Console.WriteLine("🔐 HMAC a fost generat și salvat.");
                        break;

                    case "4":
                        // Verifică integritatea și autenticitatea fișierului cu HMAC
                        string algo4 = SelectAlgorithm("HMAC");
                        Console.Write("Introduceți cheia secretă: ");
                        string keyVerify = ReadSecretInput();
                        string savedHmac = File.ReadAllText("hmac_original.txt");
                        string currentHmac = ComputeHMAC(path, keyVerify, algo4);
                        Console.WriteLine("HMAC actual:   " + currentHmac);
                        Console.WriteLine("HMAC salvat:   " + savedHmac);
                        Console.WriteLine(currentHmac == savedHmac
                            ? "\n✅ Fișierul este autentic și NU a fost modificat."
                            : "\n❌ Fișierul A FOST modificat sau cheia e greșită!");
                        break;

                    case "0":
                        // Ieșire din program
                        Console.WriteLine("👋 La revedere!");
                        break;

                    default:
                        // Opțiune invalidă
                        Console.WriteLine("⚠️ Opțiune invalidă.");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Afișează orice eroare apărută în timpul execuției
                Console.WriteLine($"❌ Eroare: {ex.Message}");
            }

            if (opt != "0")
            {
                Console.WriteLine("\nApasă Enter pentru a continua...");
                Console.ReadLine();
                Console.Clear(); // Curăță consola pentru următoarea interacțiune
            }

        } while (opt != "0");
    }
}




