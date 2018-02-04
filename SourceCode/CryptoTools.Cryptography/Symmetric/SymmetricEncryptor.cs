﻿using CryptoTools.Common.Logging;
using CryptoTools.Cryptography.Hashing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.Cryptography.Symmetric
{
	public class SymmetricEncryptor : IDisposable	
	{
		#region Private Fields
		private ILog Log = Logger.GetInstance(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private SymmetricAlgorithm _algorithm;
		private Hasher _hasher;
		#endregion

		#region Public Properties
		public SymmetricEncryptorOptions Options { get; private set; }
		#endregion

		#region Constructors
		public SymmetricEncryptor(SymmetricAlgorithm algorithm = null, SymmetricEncryptorOptions options = null, HashAlgorithm hashAlgorithm = null)
		{
			_algorithm = algorithm != null ? algorithm : Aes.Create();
			_hasher = hashAlgorithm != null ? new Hasher(hashAlgorithm) : new Hasher();
			Options = SymmetricEncryptorOptions.CreateMergedInstance(options);			
		}
		#endregion

		#region Private Helper Methods
		private byte[] MakeKey(string key)
		{
			byte[] salt = UTF8Encoding.UTF8.GetBytes(Options.KeySalt);
			Rfc2898DeriveBytes generator = new Rfc2898DeriveBytes(key, salt);
			return generator.GetBytes(_algorithm.KeySize / 8);
		}
		private byte[] MakeIV(string iv)
		{
			byte[] salt = UTF8Encoding.UTF8.GetBytes(Options.IVSalt);
			Rfc2898DeriveBytes generator = new Rfc2898DeriveBytes(iv, salt);
			return generator.GetBytes(_algorithm.BlockSize / 8);
		}
		private void ReInitialize(string key)
		{
			// Hash again for ABSOLUTE security
			key = _hasher.Hash(key);

			_algorithm.Key = MakeKey(key);
			_algorithm.IV = MakeIV(Options.InitializationVector);
			_algorithm.Mode = CipherMode.CBC; // Seems to be a good default, could change if good reason is found
			_algorithm.Padding = PaddingMode.ISO10126; // ISO Mode seems to provide the most Obsfucation
		}
		#endregion

		#region Public Methods
		public byte[] EncryptBytes(byte[] bytesIn, string key)
		{
			byte[] bytesOut;
			try
			{
				ReInitialize(key);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					ICryptoTransform transform = _algorithm.CreateEncryptor();
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						if (!cryptoStream.CanWrite) Debug.Assert(false, "Stream is not writable");

						cryptoStream.Write(bytesIn, 0, bytesIn.Length);

					}
					bytesOut = memoryStream.ToArray();
				}

			}
			catch (Exception exception)
			{
				Log.ErrorException(exception, this);
				throw;
			}
			finally
			{
				//Options.SymmetricAlgorithm.Clear();
			}
			return bytesOut;

		}

		public byte[] DecryptBytes(byte[] encryptedBytes, string key)
		{
			byte[] output;
			try
			{
				ReInitialize(key);
				ICryptoTransform transform = _algorithm.CreateDecryptor();
				output = transform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
			
			}
			catch (Exception exception)
			{
				Log.ErrorException(exception, this);
				throw;
			}
			return output;
		}
				

		public void Clear()
		{
			_algorithm.Clear();
		}

		public void Dispose()
		{
			Clear();
		}
		#endregion
	}
}
