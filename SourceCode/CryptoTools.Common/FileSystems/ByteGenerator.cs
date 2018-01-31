using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.Common.FileSystems
{
	/// <summary>
	/// Generates Random Bytes. This is useful for various encryption operations or even unit tests.
	/// </summary>
	public class ByteGenerator
	{
		public ByteGenerator()
		{
		}

		public byte[] GenerateBytes(int size)
		{
			Random random = new Random();
			byte[] bytes = new byte[size];
			random.NextBytes(bytes);
			return bytes;
		}
	}
}
