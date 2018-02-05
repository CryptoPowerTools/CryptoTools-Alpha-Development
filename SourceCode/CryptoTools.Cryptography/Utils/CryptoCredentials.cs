using CryptoTools.Cryptography.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.Cryptography.Utils
{
	public class CryptoCredentials
	{
		public CryptoString Passphrase { get; set; }
		public CryptoString Pin { get; set; }
		public CryptoString PadlockCombination { get; set; }


		public bool RequiresPassphrase { get; set; } = true;
		public bool RequiresPadlockCombination { get; set; } = false;
		public bool RequiresPin { get; set; } = false;

		public CryptoString GenerateKey()
		{
			PasswordHasher passHasher = new PasswordHasher(SHA512.Create());
			Hasher hasher = new Hasher(SHA512.Create());

			string passphraseHash = $"{hasher.Hash(CryptoString.SecureStringToString(Passphrase.GetSecureString()))}";
			string combinationHash = $"{hasher.Hash(CryptoString.SecureStringToString(PadlockCombination.GetSecureString()))}";
			string pinHash = $"{hasher.Hash(CryptoString.SecureStringToString(Pin.GetSecureString()))}";

			string credentialsHash = hasher.Hash($"{passphraseHash}{pinHash}{combinationHash}");

			CryptoString cryptoHash = new CryptoString(CryptoString.StringToSecureString(credentialsHash));

			string key = passHasher.PasswordHash(cryptoHash);

		}


	}
}
