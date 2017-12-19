using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace JWTtEST
{
    class Program
    {

        static void Main(string[] args)
        {
            //new Activation().ActivateDevice(1L);

            //Console.WriteLine(new ActivationPayload().PreparePayload(1L));

            //Console.WriteLine(new Authentication().GetBearerToken(Scope.General, 1L).AccessToken);

            //new RegistrationManager().RegisterDevice(1);


            //Console.WriteLine(Encoding.ASCII.GetString(Convert.FromBase64String("Q0FUQ09GU0VDUkVU")));

            //Console.Write((new UtcDateTimeProvider().GetNow() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);

            new MessageSender().Send(1, 100);
        }

        static void GenerateRSA()
        {
  

            string plainTextOne = "Hello Crypto";
            string plainTextTwo = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

            SHA512Managed sha512 = new SHA512Managed();

            byte[] hashedValueOfTextOne = sha512.ComputeHash(Encoding.UTF8.GetBytes(plainTextOne));
            byte[] hashedValueOfTextTwo = sha512.ComputeHash(Encoding.UTF8.GetBytes(plainTextTwo));

            string hexOfValueOne = BitConverter.ToString(hashedValueOfTextOne);
            string hexOfValueTwo = BitConverter.ToString(hashedValueOfTextTwo);

            Console.WriteLine(hexOfValueOne);
            Console.WriteLine(hexOfValueTwo);
            Console.ReadKey();



            RSACryptoServiceProvider myRSA = new RSACryptoServiceProvider();
            RSAParameters publicKey = myRSA.ExportParameters(false);
            string xml = myRSA.ToXmlString(true);
            Console.WriteLine(PrettyXml(xml));

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xml);

            IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 1024));

            //AsymmetricKeyParameter privKey = kpGen.GenerateKeyPair().Private;
            var myKeyPair = DotNetUtilities.GetRsaPublicKey(publicKey);

            //DerInteger n = new DerInteger(publicKey.Modulus);
            //DerInteger e = new DerInteger(publicKey.Exponent);
            //DerSequence seq = new DerSequence(new Asn1Encodable[] { n, e });
            //AsymmetricKeyParameter ecPriv = PrivateKeyFactory.CreateKey(seq.GetEncoded());

            var data = new StringWriter();
            PemWriter writer = new PemWriter(data);
            writer.WriteObject(myKeyPair);

            var key = data.ToString().Replace("-----", "").Replace("BEGIN PUBLIC KEY\r\n", "").Replace("\r\nEND PUBLIC KEY", "");



            //Console.WriteLine(Convert.ToBase64String(publicKey.Modulus));

            //Console.WriteLine(Convert.ToBase64String(seq.GetEncoded()));
            Console.WriteLine(key);

            var json = @"{
            ""expires_in"":3600000,
            ""token_type"":""Bearer"",
            ""access_token"":""677b4afa55236110825ae0a3d38275ad""
            }";

            var token = new JsonNetSerializer().Deserialize<BearerAccessToken>(json);
            Console.WriteLine(token.AccessToken);

            Console.ReadLine();
        }

        static string PrettyXml(string xml)
        {
            var stringBuilder = new StringBuilder();

            var element = XElement.Parse(xml);

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }
    }
}
