using System;
using System.Text;

// SHA-256, care transformă orice text (mesaj) într-un hash fix de 64 caractere hexazecimale (32 bytes = 256 biți,1 caracter hexa=4 biti).
public static class SHA256Manual
{
    private static readonly uint[] K = new uint[]//vector de 64 de constante rotative pe cate 32 de biti
    {
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
        0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
        0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
        0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
        0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
        0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
        0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
        0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
        0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
        0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
        0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
        0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
        0x90befffa, 0xa4506ce2, 0xbef9a3f7, 0xc67178f2
    };

    public static string ComputeHash(string input)//Funcția principală care primește un string și returnează SHA-256-ul său sub formă de string hexazecimal.
    {
        byte[] message = Encoding.UTF8.GetBytes(input);//Convertește input(caracterele) în bytes UTF-8;Encoding.UTF8 e standardul care convertește caracterele în format byte
        byte[] padded = PadMessage(message);//Apel la funcția care face padding(Mesajul se completează astfel încât să aibă o lungime multiplă de 512 biți (64 bytes).) 

        uint[] H = new uint[] //Acesta este hash-ul inițial – 8 numere (H0...H7), standard în SHA-256.
        {
            0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
            0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
        };

        for (int i = 0; i < padded.Length; i += 64)//Parcurge fiecare bloc de 512 biți (64 bytes) din mesajul padat.
        {
            uint[] w = new uint[64];
            for (int t = 0; t < 16; t++)//Primele 16 cuvinte (w[0]..w[15]) sunt extrase din blocul de date.
            {
                w[t] = BitConverter.ToUInt32(padded, i + t * 4);//Fiecare grup de 4 bytes este convertit într-un uint
                w[t] = SwapEndian(w[t]); // important!SwapEndian: deoarece SHA-256 folosește big-endian, iar C# folosește little-endian, inversăm ordinea byte-ilor.
            }

            for (int t = 16; t < 64; t++)//Extindem w[16..63] folosind o formulă standard.
            {
                uint s0 = RightRotate(w[t - 15], 7) ^ RightRotate(w[t - 15], 18) ^ (w[t - 15] >> 3);
                uint s1 = RightRotate(w[t - 2], 17) ^ RightRotate(w[t - 2], 19) ^ (w[t - 2] >> 10);
                w[t] = w[t - 16] + s0 + w[t - 7] + s1;
            }

            uint a = H[0], b = H[1], c = H[2], d = H[3];//Salvăm valorile din vectorul H[] în variabile temporare.Acestea vor fi transformate timp de 64 de runde.
            uint e = H[4], f = H[5], g = H[6], h = H[7];

            for (int t = 0; t < 64; t++)
            {
                uint S1 = RightRotate(e, 6) ^ RightRotate(e, 11) ^ RightRotate(e, 25);//S1 și S0: funcții de rotire.
                uint ch = (e & f) ^ (~e & g);//ch: „choose” – selectează între f și g în funcție de e.
                uint temp1 = h + S1 + ch + K[t] + w[t];//temp1 și temp2 sunt intermediate pentru actualizarea valorilor.
                uint S0 = RightRotate(a, 2) ^ RightRotate(a, 13) ^ RightRotate(a, 22);
                uint maj = (a & b) ^ (a & c) ^ (b & c);//maj: „majority” – valorile cele mai frecvente dintre a, b, c.
                uint temp2 = S0 + maj;
                //La final, actualizăm a...h.

                h = g; g = f; f = e; e = d + temp1;
                d = c; c = b; b = a; a = temp1 + temp2;
            }
            //La finalul fiecărui bloc, adunăm rezultatul rundei cu valorile inițiale.

            H[0] += a; H[1] += b; H[2] += c; H[3] += d;
            H[4] += e; H[5] += f; H[6] += g; H[7] += h;
        }

        StringBuilder sb = new StringBuilder();
        foreach (var val in H)
            sb.Append(SwapEndian(val).ToString("x8"));//Convertim fiecare valoare H[i] înapoi în big-endian.ToString("x8") => convertește în hex, cu padding la 8 caractere.

        return sb.ToString();
    }

    private static byte[] PadMessage(byte[] message)
    {
        ulong bitLen = (ulong)message.Length * 8;//bitLen: lungimea originală a mesajului în biți, nu în bytes.
        int padLen = (56 - ((message.Length + 1) % 64) + 64) % 64;//Se calculează câți 0 trebuie adăugați după byte-ul 0x80.Această formulă matematică asigură că mesajul + padding + lungime va fi multiplu de 64.
        byte[] padded = new byte[message.Length + 1 + padLen + 8];//Cream un vector nou în care vom pune:mesajul original,un 0x80,padLen bytes de 0,8 bytes pentru lungimea mesajului
        Array.Copy(message, padded, message.Length);//Copiem mesajul original în vectorul padded.
        padded[message.Length] = 0x80;//Adăugăm byte-ul 0x80 imediat după mesaj
        byte[] lenBytes = BitConverter.GetBytes(SwapEndian((ulong)bitLen));//Lungimea mesajului (în biți) trebuie convertită în 8 bytes, big-endian.BitConverter.GetBytes() produce little-endian (specific C#), așa că facem un SwapEndian() pentru a obține big-endian (așa cere SHA-256).
        Array.Copy(lenBytes, 0, padded, padded.Length - 8, 8);//Adăugăm cei 8 bytes care reprezintă lungimea originală, la finalul mesajului.
        return padded;
    }

    private static uint RightRotate(uint x, int n) => (x >> n) | (x << (32 - n));//Face o rotire circulară la dreapta pe 32 de biți.
    private static uint SwapEndian(uint x) =>
        ((x & 0x000000FFU) << 24) | ((x & 0x0000FF00U) << 8) |   //Inversează ordinea celor 4 bytes dintr-un uint (32 biți).SHA-256 lucrează cu date în format big-endian.C# stochează valorile ca little-endian (cel mai mic byte la început), deci trebuie convertit.
        ((x & 0x00FF0000U) >> 8) | ((x & 0xFF000000U) >> 24);
    private static ulong SwapEndian(ulong x) =>
        ((x & 0x00000000000000FFUL) << 56) | ((x & 0x000000000000FF00UL) << 40) |
        ((x & 0x0000000000FF0000UL) << 24) | ((x & 0x00000000FF000000UL) << 8) |
        ((x & 0x000000FF00000000UL) >> 8) | ((x & 0x0000FF0000000000UL) >> 24) |
        ((x & 0x00FF000000000000UL) >> 40) | ((x & 0xFF00000000000000UL) >> 56);
}
