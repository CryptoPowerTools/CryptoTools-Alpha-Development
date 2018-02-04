using CryptoTools.Common.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.Cryptography.Utils
{
	public class CryptoString
	{
		private SecureString _secureString;

		public CryptoString(SecureString secureString)
		{
			_secureString = secureString;			
		}

		public SecureString GetSecureString()
		{
			return _secureString;
		}


		public static string GenerateRandomText(int approximateLength)
		{
			ByteGenerator generator = new ByteGenerator();
			byte[] bytes = generator.GenerateBytes(approximateLength/2);
			string result = BytesToString(bytes);
			return result;

		}

		public static string BytesToString(byte[] bytes)
		{
			StringBuilder builder = new StringBuilder();
			foreach (Byte b in bytes)
			{
				builder.Append(b.ToString("x2"));
				
			}
			return builder.ToString();

		}

		public static SecureString StringToSecureString(string value)
		{
			SecureString secure = new SecureString();
			foreach (char c in value)
			{
				secure.AppendChar(c);
			}
				return secure;
		}


		public static String SecureStringToString(SecureString value)
		{
			IntPtr valuePtr = IntPtr.Zero;
			try
			{
				valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
				return Marshal.PtrToStringUni(valuePtr);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
			}
		}
	}
}
