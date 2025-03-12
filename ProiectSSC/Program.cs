using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string filePath = "document.txt";
        string signaturePath = "signature.txt";
        string publicKeyPath = "public_key.txt";

        // Creăm documentul doar dacă nu există deja
        string mesaj = "Acesta este un document important.";

        // Verificăm dacă fișierul există deja și dacă da, calculăm hash-ul său
        string fileHash = ComputeFileHash(filePath);
        Console.WriteLine($"Hash-ul fișierului inițial: {fileHash}");

        // Creăm documentul doar dacă nu există deja
        if (!File.Exists(filePath))
        {
            FileUtils.WriteFile(filePath, mesaj);
            Console.WriteLine($"✅ Documentul a fost creat: {filePath}");
        }

        // 🔹 Generăm și salvăm cheia publică
        DigitalSignature ds = new DigitalSignature();
        ds.SavePublicKey(publicKeyPath);
        Console.WriteLine($"✅ Cheia publică a fost salvată: {publicKeyPath}");

        // 🔹 Semnăm documentul
        byte[] signature = ds.SignFile(filePath);
        FileUtils.WriteBytesToFile(signaturePath, signature);
        Console.WriteLine($"✅ Semnătura a fost salvată: {signaturePath}");

        // 🔹 Verificăm semnătura
        bool isValid = ds.VerifyFileSignature(filePath, FileUtils.ReadBytesFromFile(signaturePath), publicKeyPath);
        Console.WriteLine($"🔎 Semnătura este validă: {isValid}");

        // 🔹 Simulăm modificarea fișierului
        FileUtils.WriteFile(filePath, "Acesta este un document modificat.");
        Console.WriteLine("⚠️ Fișierul a fost modificat.");

        // 🔹 Verificăm dacă fișierul a fost modificat
        string newFileHash = ComputeFileHash(filePath);
        Console.WriteLine($"Hash-ul fișierului după modificare: {newFileHash}");

        if (fileHash != newFileHash)
        {
            Console.WriteLine("⚠️ Fișierul a fost modificat după semnătura inițială.");
        }
        else
        {
            Console.WriteLine("✅ Fișierul nu a fost modificat.");
        }
    }

    static string ComputeFileHash(string filePath)
    {
        // Calculează hash-ul fișierului
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] hashBytes = sha256.ComputeHash(fileBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
