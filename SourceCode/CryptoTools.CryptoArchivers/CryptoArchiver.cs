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
		public CryptoCredentials Credentials
		{
			get
			{
				if (_archiver.Credentials == null)
					throw new CryptoCredentialsNullException(typeof(CryptoArchiver));

				return _archiver.Credentials;
			}
			set
			{
				_archiver.Credentials = value;
			}
		}

		public bool RemoveFilesAfterSave { get; set; }

		private IFileArchiver _archiver;


		public CryptoArchiver(IFileArchiver archiver = null)
		{
			if(archiver==null)
			{
				archiver = new DotNetZipArchiver();
			}
			_archiver = archiver;
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
			byte[] bytes;
			string tempFileName = fileMan.GenerateTempFileName("SaveToBytes");

			using (AutoDeleteFiles autoDelete = new AutoDeleteFiles(tempFileName))
			{
				_archiver.FullFileName = tempFileName;
				_archiver.Save();
				_archiver.FullFileName = ""; // For safety reset the filename

				// Read Bytes and delete file
				bytes = File.ReadAllBytes(tempFileName);
			}
			return bytes;
		}

		public void ExtractAllFromBytes(string directoryName, byte[] bytes)
		{
			FileManager fileMan = new FileManager();
			string tempFileName = fileMan.GenerateTempFileName("ExtractAllFromBytes");

			using (AutoDeleteFiles autoDelete = new AutoDeleteFiles(tempFileName))
			{
				// Create/Write the bytes to a file
				fileMan.CreateWriteBytes(tempFileName, bytes);
				_archiver.FullFileName = tempFileName;
				_archiver.ExtractAll(directoryName);
				_archiver.FullFileName = "";
			}
		}

	}
}
