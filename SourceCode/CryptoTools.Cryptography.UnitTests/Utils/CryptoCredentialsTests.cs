using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptoTools.Cryptography.Utils;
using CryptoTools.Cryptography.Hashing;

namespace CryptoTools.Cryptography.UnitTests.Utils
{
	[TestClass]
	public class CryptoCredentialsTests
	{
		[TestMethod]
		public void CryptoCredential_BasicUsage()
		{
			CryptoCredentials credentials = new CryptoCredentials
			{
				Passphrase = new CryptoString("My Passphrase"),
				Pin = 2222
			};

			
			// Check what  credentials are used
			bool passphraseUsed = credentials.UsePassphrase;
			bool pinUsed = credentials.UsePin;

			// Generate Key to be used for your encryption
			string key = credentials.Key;

		}


		[TestMethod]
		public void CryptoCredential_ConfirmPropertiesSet()
		{
			string passphraseString = "My Passphrase";
			CryptoString passphrase = new CryptoString(CryptoString.StringToSecureString(passphraseString));
			int pin = 1234;
			
			// Create Credentials
			CryptoCredentials credentials = new CryptoCredentials();

			// Assign Credentials and ensure the 'Use' properties get updated
			Assert.IsFalse(credentials.UsePassphrase);
			credentials.Passphrase = passphrase;
			Assert.IsTrue(credentials.UsePassphrase);

			Assert.IsFalse(credentials.UsePin);
			credentials.Pin = pin;
			Assert.IsTrue(credentials.UsePin);
			
			string key = credentials.Key;
			Assert.IsTrue(Hasher.IsHashValid(key));
		}
	}
}
