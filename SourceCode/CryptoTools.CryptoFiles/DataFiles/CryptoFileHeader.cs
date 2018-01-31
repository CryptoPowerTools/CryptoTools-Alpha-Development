using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.CryptoFiles.DataFiles
{
	public class CryptoFileHeader
	{
		#region Public Data Fields
		public byte[] FileFormat { get; set; } = new byte[2];
		public byte[] ContentFormat { get; set; } = new byte[2];
		public int ContentSize { get; set; } = 0;
		public byte[] ContentHash { get; set; } = new byte[32];
		public byte[] ReservedArea { get; set; } = new byte[64]
			{
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7,
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7,
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7,
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7,
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7,
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7,
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7,
			0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7, 0x7
			};
		#endregion

		#region Internal Data Field Sizes
		internal int FileFormatSize { get { return FileFormat.Length; } }
		internal int ContentFormatSize { get { return ContentFormat.Length; } }
		internal int ContentSizeSize { get { return sizeof(int); } }
		internal int ContentHashSize { get { return ContentHash.Length; } }
		internal int ReservedAreaSize { get { return ReservedArea.Length; } }
		internal int HeaderLength { get { return FileFormatSize + ContentFormatSize + ContentSizeSize + ContentHashSize + ReservedAreaSize; } }
		#endregion
	}
}