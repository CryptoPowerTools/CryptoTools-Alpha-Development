using CryptoTools.Common.FileSystems;
using CryptoTools.CryptoArchivers.Archivers;
using CryptoTools.Cryptography.Hashing;
using CryptoTools.Cryptography.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.CryptoArchivers
{
	public class CryptoArchiver : IFileArchiver
	{
		public string FullFileName
		{
			get
			{
				return _archiver.FullFileName;
			}
			set
			{
				_archiver.FullFileName = value;
			}
		}
		public CryptoString Passphrase
		{
			get
			{
				return _archiver.Passphrase;
			}
			set
			{
				_archiver.Passphrase = value;
			}
		}
				
		private IFileArchiver _archiver;


		public CryptoArchiver(IFileArchiver archiver = null)
		{
			if(archiver==null)
			{
				_archiver = new DotNetZipArchiver();
			}
		}		
	
		public void AddDirectory(string directoryName, string directoryNameInArchive = "")
		{
			_archiver.AddDirectory(directoryName, directoryNameInArchive);
		}
			
		public void ExtractAll(string directoryName)
		{
			_archiver.ExtractAll(directoryName);
		}

		public void AddFile(string fileName, string directoryPathInArchive = "")
		{
			_archiver.AddFile(fileName, directoryPathInArchive);
		}

		public void Save()
		{
			_archiver.Save();
		}

		public byte[] SaveToBytes()
		{
			FileManager fileMan = new FileManager();
			string tempFileName = fileMan.GenerateTempFileName("SaveToBytes");
			_archiver.FullFileName = tempFileName;
			_archiver.Save();
			_archiver.FullFileName = ""; // For safety reset the filename
			
			// Read Bytes and delete file
			byte[] bytes = File.ReadAllBytes(tempFileName);
			fileMan.DeleteFile(tempFileName);

			return bytes;
		}

		public void ExtractAllFromBytes(string directoryName, byte[] bytes)
		{
			FileManager fileMan = new FileManager();
			string tempFileName = fileMan.GenerateTempFileName();

			
			// Create/Write the bytes to a file
			fileMan.CreateWriteBytes(tempFileName, bytes);
			_archiver.FullFileName = tempFileName;
			_archiver.ExtractAll(directoryName);
			_archiver.FullFileName = "";
		}

	}
}
