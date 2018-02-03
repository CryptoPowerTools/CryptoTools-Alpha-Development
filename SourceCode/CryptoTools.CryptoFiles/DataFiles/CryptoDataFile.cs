using CryptoTools.CryptoFiles.Exceptions;
using CryptoTools.Cryptography.Hashing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.CryptoFiles.DataFiles
{

	public class CryptoDataFile
	{
		#region Constants
		public readonly byte[] FileFormat = new byte[] { 0xFF, 0xAA };
		public readonly byte[] ContentFormat = new byte[] { 0xA, 0x0 };
		public readonly byte[] EndFileFormat = new byte[] { 0xAA, 0xFF };
		#endregion
		
		#region File Structure
		public CryptoFileHeader Header = new CryptoFileHeader();
		public CryptoFileFooter Footer = new CryptoFileFooter();
		#endregion

		#region Private Fields
		private bool _isLoaded = false;
		private Hasher _hasher = null;
		#endregion		

		#region Constructor
		public CryptoDataFile(string fullFileName, Hasher hasher = null)
		{
			FullFileName = fullFileName;

			_hasher = hasher != null ? hasher : new Hasher();
		}
		#endregion

		#region Public Properties
		public string FullFileName { get; set; }
		public bool IsLoaded { get { return _isLoaded; } }
		public byte[] Content { get; set; }
		#endregion

		#region Private Helper Methods
		private void BuildHeader()
		{
			Header.FileFormat = FileFormat;
			Header.ContentFormat = ContentFormat;
			Header.ContentHash = _hasher.HashToBytes(Content);
			Header.ContentSize = Content.Length;
		}

		private void BuildFooter()
		{
			byte[] fileContents = GetFileContents();
			Footer.FileHash = _hasher.HashToBytes(fileContents);
			Footer.EndFileFormat = EndFileFormat;
		}

		private byte[] GetFileContents()
		{
			// This needs to be updated if the file format changes
			byte[] fileContents = Header.FileFormat.
								Concat(Header.ContentFormat).
								Concat(BitConverter.GetBytes(Header.ContentSize)).
								Concat(Header.ContentHash).
								Concat(Header.ReservedArea).
								Concat(Content).
								Concat(Footer.ReservedArea).
								ToArray();
			return fileContents;
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Checks the Format of the Data File quickly without reading the entire file and then return true if the format is valid based on the File/Content Format Marker. 
		/// Return false if the file does not have the proper format
		/// </summary>
		/// <returns></returns>
		public bool CheckFormat()
		{
			using (FileStream fileStream = new FileStream(FullFileName, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader reader = new BinaryReader(fileStream))
				{
					// Check File Format
					byte[] fileFormat = reader.ReadBytes(Header.FileFormatSize);
					if (!Header.FileFormat.SequenceEqual(fileFormat))
					{
						return false;
					}

					// Check Content format
					byte[] contentFormat = reader.ReadBytes(Header.ContentFormatSize);
					if (!Header.ContentFormat.SequenceEqual(contentFormat))
					{
						return false;
					}

					/////////////////////////////////////////////////////////////////////////////////
					// DONE at this time if the first 4 bytes indicate its the right 
					// format then we assume its valid.
					// NOTE: You must Open the File which will Verify it before using the data
					/////////////////////////////////////////////////////////////////////////////////					
				}
			}
			return true;
		}


		/// <summary>
		/// Verifies that the data is in the expected format. Ensures that the data is valid by checking the hashes of the data. Ensure the file has not been tampered with 
		/// or corrupted. This method will throw an exception with useful debug messages.
		/// </summary>
		public void Verify()
		{

			// Check Formats and Markers
			if (!_isLoaded)
			{
				Load();
			}

			// Check Format Markers
			if (!Header.FileFormat.SequenceEqual(FileFormat))
			{
				throw new CryptoFileFailedVerificationException(FullFileName, $"FileFormat failed verification. Expected:{FileFormat} Actual:{Header.FileFormat}");
			}
			if (!Header.ContentFormat.SequenceEqual(ContentFormat))
			{
				throw new CryptoFileFailedVerificationException(FullFileName, $"ContentFormat failed verification. Expected:{ContentFormat} Actual:{Header.ContentFormat}");
			}
			if (!Footer.EndFileFormat.SequenceEqual(EndFileFormat))
			{
				throw new CryptoFileFailedVerificationException(FullFileName, $"EndFileFormat failed verification. Expected:{EndFileFormat} Actual:{Footer.EndFileFormat}");
			}

			//  Check Content Size
			if (Content.Length != Header.ContentSize)
			{
				throw new CryptoFileFailedVerificationException(FullFileName, $"ContentSize failed verification. Expected(Header.ContentSize):{Header.ContentSize} Actual(Content.Length):{Content.Length}");
			}

			//  Check Content Hash
			byte[] actualHash = _hasher.HashToBytes(Content);
			if (!Header.ContentHash.SequenceEqual(actualHash))
			{
				throw new CryptoFileFailedVerificationException(FullFileName, $"ContentHash failed verification. Expected(Header.ContentHash):{Header.ContentHash} Actual(Hash(Content)):{actualHash}");
			}

			//  Check File Hash
			byte[] fileContents = GetFileContents();
			byte[] actualFileHash = _hasher.HashToBytes(fileContents);
			if (!Footer.FileHash.SequenceEqual(actualFileHash))
			{
				throw new CryptoFileFailedVerificationException(FullFileName, $"FileHash failed verification. Expected(Footer.FileHash):{Footer.FileHash} Actual(Hash('filecontents')):{actualFileHash}");
			}
		}


		public virtual void Load()
		{
			using (FileStream fileStream = new FileStream(FullFileName, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader reader = new BinaryReader(fileStream))
				{
					/////  Header  /////////////////////////////////////////////////
					Header.FileFormat = reader.ReadBytes(Header.FileFormatSize);
					Header.ContentFormat = reader.ReadBytes(Header.ContentFormatSize);
					Header.ContentSize = reader.ReadInt32();
					Header.ContentHash = reader.ReadBytes(Header.ContentHashSize);
					Header.ReservedArea = reader.ReadBytes(Header.ReservedAreaSize);

					/////  Content  /////////////////////////////////////////////////
					Content = reader.ReadBytes(Header.ContentSize);

					/////  Footer  /////////////////////////////////////////////////
					Footer.ReservedArea = reader.ReadBytes(Footer.ReservedAreaSize);
					Footer.FileHash = reader.ReadBytes(Footer.FileHashSize);
					Footer.EndFileFormat = reader.ReadBytes(Footer.EndFileFormatSize);
				}
			}
			_isLoaded = true;

			// Verify
			Verify();
		}

		public virtual void Save()
		{
			// Calculate Header/Footer Hashes, content size etc.
			BuildHeader();
			BuildFooter();

			//////////////////////////////////////////////////////////////////////
			// Consider back up the file with a ".backup.timestamp" extension"
			//////////////////////////////////////////////////////////////////////

			using (FileStream fileStream = new FileStream(FullFileName, FileMode.Create))
			{
				using (BinaryWriter writer = new BinaryWriter(fileStream))
				{
					/////  Header  /////////////////////////////////////////////////
					writer.Write(Header.FileFormat);
					writer.Write(Header.ContentFormat);
					writer.Write(Header.ContentSize);
					writer.Write(Header.ContentHash);
					writer.Write(Header.ReservedArea);

					/////  Content  /////////////////////////////////////////////////
					writer.Write(Content);

					/////  Footer  /////////////////////////////////////////////////
					writer.Write(Footer.ReservedArea);
					writer.Write(Footer.FileHash);
					writer.Write(Footer.EndFileFormat);
				}
			}
		}

		public string GetFileHashFingerprint()
		{
			byte[] fileContents = GetFileContents();
			fileContents.Concat(Footer.FileHash).Concat(Footer.EndFileFormat);
			string hash = _hasher.Hash(fileContents);
			return hash;
		}

		#endregion
	}
}
