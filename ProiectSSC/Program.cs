using System;

using System.Security.Cryptography;
using System.Text;


class Program
{
    static void Main()
    {
        string filePath = "document.txt";
        string signaturePath = "signature.txt";
        string publicKeyPath = "public_key.txt";

        // Creăm documentul
        string mesaj = "Acesta este un document important.";
        FileUtils.WriteFile(filePath, mesaj);
        Console.WriteLine($"✅ Documentul a fost creat: {filePath}");

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
    }
}


