using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.CryptoFiles.Exceptions
{
	/// <summary>
	/// Base Exception for all Crypto File Exception
	/// </summary>
	public class CryptoFileException : Exception
	{
		public string FullFileName { get; internal set; }
		public CryptoFileException(string fileName, string message) : base(message)
		{
		}
	}

	public class CryptoFileInvalidFormatException : CryptoFileException
	{
		public CryptoFileInvalidFormatException(string fileName, string message) : base(fileName, message) { }
	}

	public class CryptoFileFailedVerificationException : CryptoFileException
	{
		public CryptoFileFailedVerificationException(string fileName, string message) : base(fileName, message) { }
	}
	
}
