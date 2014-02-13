using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using selfies.Models;

namespace selfies.Controllers
{
    public class AesController : ApiController
    {
        //
        // GET: /Aes/

        public RODResponseMessage Get()
        {

            string Plain_Text;
            string Decrypted;
            string Encrypted_Text;
            byte[] Encrypted_Bytes;

            string TestKey = "SETHAwQFBgcICQoLDA0ODw==";

            //This class here the Rijndael is what will have most all of the methods we need to do aes encryption.
            //When this is called it will create both a key and Initialization Vector to use.
            RijndaelManaged Crypto = new RijndaelManaged();

            //This is just here to convert the Encrypted byte array to a string for viewing purposes.
            System.Text.UTF8Encoding UTF = new System.Text.UTF8Encoding();

            Crypto.Key = Convert.FromBase64String(TestKey);

            Console.WriteLine("Current Key: " + System.Text.Encoding.UTF8.GetString(Crypto.Key));
            Console.WriteLine("Please put in the text to be encrypted.");
            Plain_Text = "hi from seth";

            Encrypted_Bytes = encrypt_function(Plain_Text, Crypto.Key, Crypto.IV);
            Encrypted_Text = UTF.GetString(Encrypted_Bytes);
            Decrypted = decrypt_function(Encrypted_Bytes, Crypto.Key, Crypto.IV);

            RODResponseMessage m = new RODResponseMessage();
            m.message = Encrypted_Text;
            m.result = 1;

            return m;

        }


        private static byte[] encrypt_function(string Plain_Text, byte[] Key, byte[] IV)
        {

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.
            ICryptoTransform Encryptor = null;
            //Crypto streams allow for encryption in memory.
            CryptoStream Crypto_Stream = null;

            System.Text.UTF8Encoding Byte_Transform = new System.Text.UTF8Encoding();

            //Just grabbing the bytes since most crypto functions need bytes.
            byte[] PlainBytes = Byte_Transform.GetBytes(Plain_Text);

            try
            {
                Crypto = new RijndaelManaged();
                Crypto.Key = Key;
                Crypto.IV = IV;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call
                //If these are changed nothing will work right.
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)
                Crypto_Stream.Write(PlainBytes, 0, PlainBytes.Length);

            }
            finally
            {
                //if the crypto blocks are not clear lets make sure the data is gone
                if (Crypto != null)
                    Crypto.Clear();
                //Close because of my need to close things then done.
                Crypto_Stream.Close();
            }
            //Return the memory byte array
            return MemStream.ToArray();
        }

        private static string decrypt_function(byte[] Cipher_Text, byte[] Key, byte[] IV)
        {
            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;
            string Plain_Text;

            try
            {
                Crypto = new RijndaelManaged();
                Crypto.Key = Key;
                Crypto.IV = IV;

                MemStream = new MemoryStream(Cipher_Text);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.
                Stream_Read = new StreamReader(Crypto_Stream);
                Plain_Text = Stream_Read.ReadToEnd();
            }
            finally
            {
                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }
            return Plain_Text;
        }

    }
}
