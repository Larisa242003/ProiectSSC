using System;

using System.Security.Cryptography;
using System.Text;
class Program
{
    static void Main()
    {
        string mesaj = "Acesta este un mesaj important.";

        // Test SHA-256
        string hash = HashingUtils.ComputeSHA256(mesaj);
        Console.WriteLine($"SHA-256: {hash}");

        // Test HMAC-SHA256
        string cheieSecreta = "cheie123";
        string hmac = HashingUtils.ComputeHMACSHA256(mesaj, cheieSecreta);
        Console.WriteLine($"HMAC-SHA256: {hmac}");

        // Test Semnătură Digitală
        DigitalSignature ds = new DigitalSignature();
        byte[] semnatura = ds.SignData(mesaj);
        bool valid = ds.VerifySignature(mesaj, semnatura);
        Console.WriteLine($"Semnătura este validă: {valid}");
    }
}

