using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    public static string ComputeFileHash(string path, string algorithm)
    {
        using HashAlgorithm hasher = algorithm switch
        {
            "SHA256" => SHA256.Create(),
            "SHA512" => SHA512.Create(),
            _ => throw new ArgumentException("Algoritm invalid.")
        };

        using var stream = File.OpenRead(path);
        byte[] hash = hasher.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    public static string ComputeHMAC(string path, string key, string algorithm)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        using HMAC hmac = algorithm switch
        {
            "SHA256" => new HMACSHA256(keyBytes),
            "SHA512" => new HMACSHA512(keyBytes),
            _ => throw new ArgumentException("Algoritm invalid.")
        };

        using var stream = File.OpenRead(path);
        byte[] hash = hmac.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    public static void GenerateRSAKeys()
    {
        using var rsa = RSA.Create();
        File.WriteAllText("private_key.xml", rsa.ToXmlString(true));
        File.WriteAllText("public_key.xml", rsa.ToXmlString(false));
        Console.WriteLine("🔑 Cheile RSA au fost generate.");
    }

    public static void SignFile(string path)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(File.ReadAllText("private_key.xml"));

        byte[] data = File.ReadAllBytes(path);
        byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        File.WriteAllBytes("signature.sig", signature);
        Console.WriteLine("✍️ Semnătura digitală a fost creată.");
    }

    public static void VerifySignature(string path)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(File.ReadAllText("public_key.xml"));

        byte[] data = File.ReadAllBytes(path);
        byte[] signature = File.ReadAllBytes("signature.sig");

        bool valid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        Console.WriteLine(valid
            ? "\n✅ Semnătura este validă."
            : "\n❌ Semnătura NU este validă sau fișierul a fost modificat.");
    }

    static void ShowMenu()
    {
        Console.WriteLine("\n==================== MENIU ====================");
        Console.WriteLine("1 - Generează hash și salvează");
        Console.WriteLine("2 - Verifică integritatea cu hash");
        Console.WriteLine("3 - Generează HMAC și salvează");
        Console.WriteLine("4 - Verifică integritatea cu HMAC");
        Console.WriteLine("5 - Generează chei RSA");
        Console.WriteLine("6 - Semnează digital un fișier");
        Console.WriteLine("7 - Verifică semnătura digitală");
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

    static string SelectAlgorithm(string context)
    {
        Console.Write($"Alege algoritmul pentru {context} (1 = SHA-256, 2 = SHA-512): ");
        string choice = Console.ReadLine()!;
        return choice == "2" ? "SHA512" : "SHA256";
    }

    static void Main()
    {
        Console.Write("Introduceți calea către fișier: ");
        string path = Console.ReadLine()!;
        string opt;

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
                        string algo1 = SelectAlgorithm("hash");
                        string hash = ComputeFileHash(path, algo1);
                        File.WriteAllText("hash_original.txt", hash);
                        Console.WriteLine("🔐 Hash a fost generat și salvat.");
                        break;

                    case "2":
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
                        string algo3 = SelectAlgorithm("HMAC");
                        Console.Write("Introduceți cheia secretă: ");
                        string keyHmac = ReadSecretInput();
                        string hmac = ComputeHMAC(path, keyHmac, algo3);
                        File.WriteAllText("hmac_original.txt", hmac);
                        Console.WriteLine("🔐 HMAC a fost generat și salvat.");
                        break;

                    case "4":
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

                    case "5":
                        GenerateRSAKeys();
                        break;

                    case "6":
                        SignFile(path);
                        break;

                    case "7":
                        VerifySignature(path);
                        break;

                    case "0":
                        Console.WriteLine("👋 La revedere!");
                        break;

                    default:
                        Console.WriteLine("⚠️ Opțiune invalidă.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Eroare: {ex.Message}");
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



