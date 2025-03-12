using System;
using System.Security.Cryptography;
using System.Text;
public class DigitalSignature
{
    private RSA _rsa;

    public DigitalSignature()
    {
        _rsa = RSA.Create();
    }

    public string GetPublicKey()
    {
        return Convert.ToBase64String(_rsa.ExportRSAPublicKey());
    }

    public byte[] SignData(string message)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(message);
        return _rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool VerifySignature(string message, byte[] signature)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(message);
        return _rsa.VerifyData(dataBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
