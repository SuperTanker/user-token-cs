﻿using NUnit.Framework;
using Sodium;
using System;
using System.Text;

namespace Tanker
{
    class CryptoTests
    {
        [Test]
        public void TestHash()
        {
            string hexVector = "BA80A53F981C4D0D6A2797B69F12F6E94C212F14685AC4B74B12BB6FDBFFA2D17D87C5392AAB792DC252D5DE4533CC9518D38AA8DBF1925AB92386EDD4009923";
            int numChars = hexVector.Length;
            byte[] vector = new byte[numChars / 2];
            for (int i = 0; i < numChars; i +=2)
            {
                vector[i / 2] = Convert.ToByte(hexVector.Substring(i, 2), 16);
            }

            byte[] buffer = Encoding.ASCII.GetBytes("abc");
            byte[] output = Crypto.GenericHash(buffer, 64);

            Assert.AreEqual(output, vector);
        }

        [Test]
        public void TestSignValidSignature()
        {
            byte[] message = Encoding.ASCII.GetBytes("message");
            KeyPair keyPair = Crypto.SignKeyPair();
            byte[] signature = Crypto.SignDetached(message, keyPair.PrivateKey);

            Crypto.VerifySignDetached(message, signature, keyPair.PublicKey);
        }

        [Test]
        public void TestSignInvalidMessage()
        {
            byte[] message = Encoding.ASCII.GetBytes("message");
            KeyPair keyPair = Crypto.SignKeyPair();
            byte[] signature = Crypto.SignDetached(message, keyPair.PrivateKey);

            byte[] invalidMessage = Encoding.ASCII.GetBytes("m3ssage");

            Assert.Throws(typeof(InvalidSignatureException),
                delegate { Crypto.VerifySignDetached(invalidMessage, signature, keyPair.PublicKey); } );

        }

        [Test]
        public void TestInvalidSignature()
        {
            byte[] message = Encoding.ASCII.GetBytes("message");
            KeyPair keyPair = Crypto.SignKeyPair();
            byte[] signature = Crypto.SignDetached(message, keyPair.PrivateKey);
            byte[] invalidSignature = CorruptBuffer(signature);
            Assert.Throws(typeof(InvalidSignatureException),
                delegate { Crypto.VerifySignDetached(message, invalidSignature, keyPair.PublicKey); } );
        }

        private byte[] CorruptBuffer(byte[] buffer)
        {
            byte[] res = buffer;
            res[0] = buffer[0] = 1;
            return res;
        }
    }
}