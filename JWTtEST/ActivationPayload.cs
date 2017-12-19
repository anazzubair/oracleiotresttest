using Newtonsoft.Json.Linq;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    class ActivationPayload
    {
        AppSettingsReader appSettingsReader = new AppSettingsReader();
        public string PreparePayload(long deviceId)
        {
            var deviceRSA = new RSACryptoServiceProvider(2048);
            var activationSecret = String.Empty;
            var activationId = String.Empty;
            using (var db = new IoTContext())
            {
                var device = db.Devices.Find(deviceId);
                deviceRSA.FromXmlString(device.RSAKeyXML);
                activationSecret = device.ActivationSecret;
                activationId = device.ActivationId;
            }

            var dotnetPublicKey = deviceRSA.ExportParameters(false);
            var bouncyPublicKey = DotNetUtilities.GetRsaPublicKey(dotnetPublicKey);

            var pemPublicKey = new StringWriter();
            PemWriter writer = new PemWriter(pemPublicKey);
            writer.WriteObject(bouncyPublicKey);

            var publicKey = pemPublicKey.ToString().Replace("-----", "").Replace("BEGIN PUBLIC KEY\r\n", "").Replace("\r\nEND PUBLIC KEY", "");
            

            byte[] textBytes = Encoding.UTF8.GetBytes(activationId);
            HMACSHA256 hashAlgorithm = new HMACSHA256(Encoding.UTF8.GetBytes(activationSecret));
            byte[] secretHash = hashAlgorithm.ComputeHash(textBytes);

            var payLoadString = activationId + "\n" + "RSA" + "\n" + "X.509" + "\n" + "HmacSHA256" + "\n";
            var payLoadBytes = Encoding.UTF8.GetBytes(payLoadString);
            var publicKeyBytes = Convert.FromBase64String(publicKey);

            byte[] signatureBytes = new byte[payLoadBytes.Length + secretHash.Length + publicKeyBytes.Length];
            Array.Copy(payLoadBytes, 0, signatureBytes, 0, payLoadBytes.Length);
            Array.Copy(secretHash, 0, signatureBytes, payLoadBytes.Length, secretHash.Length);
            Array.Copy(publicKeyBytes, 0, signatureBytes, secretHash.Length + payLoadBytes.Length, publicKeyBytes.Length);

            byte[] signature = deviceRSA.SignData(signatureBytes, new SHA256Managed());


            JObject obj = JObject.FromObject(new
            {
                deviceModels = new[]
                {
                    "urn:oracle:iot:dcd:capability:direct_activation",
                    "urn:com:keurig:coffee:machine"
                },
                certificationRequestInfo = new
                {
                    subject = activationId,
                    subjectPublicKeyInfo = new
                    {
                        algorithm = "RSA",
                        publicKey = publicKey,
                        format = "X.509",
                        secretHashAlgorithm = "HmacSHA256"
                    },
                    attributes = new { }
                },
                signatureAlgorithm = "SHA256withRSA",
                signature = Convert.ToBase64String(signature)
            });

            return obj.ToString();
        }
    }
}
