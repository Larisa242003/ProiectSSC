using System;
using System.Security.Cryptography;
using System.Text;

using System;
//using System.Security.Cryptography;
//using System.Text;

public class DigitalSignature
{
    private RSA _rsa;

    public DigitalSignature()
    {
        _rsa = RSA.Create();
    }

    public void SavePublicKey(string filePath)
    {
        string publicKey = Convert.ToBase64String(_rsa.ExportRSAPublicKey());
        FileUtils.WriteFile(filePath, publicKey);
    }

    public byte[] SignFile(string filePath)
    {
        string content = FileUtils.ReadFile(filePath);
        byte[] dataBytes = Encoding.UTF8.GetBytes(content);
        return _rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool VerifyFileSignature(string filePath, byte[] signature, string publicKeyPath)
    {
        string content = FileUtils.ReadFile(filePath);
        byte[] dataBytes = Encoding.UTF8.GetBytes(content);
        byte[] publicKeyBytes = Convert.FromBase64String(FileUtils.ReadFile(publicKeyPath));

        using (RSA rsaVerify = RSA.Create())
        {
            rsaVerify.ImportRSAPublicKey(publicKeyBytes, out _);
            return rsaVerify.VerifyData(dataBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}

