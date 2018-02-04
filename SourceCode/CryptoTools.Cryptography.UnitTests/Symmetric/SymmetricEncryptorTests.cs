using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptoTools.Cryptography.Symmetric;
using CryptoTools.Common.FileSystems;
using System.Linq;
using System.Security.Cryptography;
using CryptoTools.Cryptography.Utils;

namespace CryptoTools.Cryptography.UnitTests.Symmetric
{
	[TestClass]
	public class SymmetricEncryptorTests
	{
		[TestMethod]
		public void SymmetricEncryptor_BasicUsage()
		{
			// Arrange
			int blockSize = 1000;
			byte[] buffer = new ByteGenerator().GenerateBytes(blockSize);
			string key = "test key";

			byte[] decryptedBuffer;
			byte[] encryptedBuffer;

			using (SymmetricEncryptor encryptor = new SymmetricEncryptor())
			{
				//Encrypt
				encryptedBuffer = encryptor.EncryptBytes(buffer, key);
				
				// Decrypt
				decryptedBuffer = encryptor.DecryptBytes(encryptedBuffer, key);
			} // IDispose - Closes and clears the keys in memory
			
			// Assert - Check to make sure the bytes are all the same
			Assert.IsTrue(buffer.SequenceEqual(decryptedBuffer));
		}

		[TestMethod]
		public void SymmetricEncryptor_HeavyUsage()
		{
			CryptoRandomizer random = new CryptoRandomizer();
			const int Iterations = 100;
			const int MaxMemoryBlock = 100000;


			for (int i = 0; i < Iterations; i++)
			{
				int blockSize = random.Next(MaxMemoryBlock);
				byte[] buffer = new ByteGenerator().GenerateBytes(blockSize);
				string key = CryptoString.GenerateRandomText(1000);

				byte[] decryptedBuffer;
				byte[] encryptedBuffer;

				using (SymmetricEncryptor encryptor = new SymmetricEncryptor())
				{
					//Encrypt
					encryptedBuffer = encryptor.EncryptBytes(buffer, key);

					// Decrypt
					decryptedBuffer = encryptor.DecryptBytes(encryptedBuffer, key);
				} // IDispose - Closes and clears the keys in memory

				// Assert - Check to make sure the bytes are all the same
				Assert.IsTrue(buffer.SequenceEqual(decryptedBuffer));
			}			
		}



		[TestMethod]
		public void BasicUsage_MultiThreadHeavyUsuage()
		{
			// TODO
			
			//// Create a thread callback
			
			//// Act - Encrypt
			
			//// Launce all the threads

			
			//// Wait for all threads to finish
			
			///Wait
			
		}
	}
}
