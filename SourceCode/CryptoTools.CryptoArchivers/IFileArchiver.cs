using CryptoTools.Cryptography.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.CryptoArchivers
{
	public interface IFileArchiver
	{
		string FullFileName { get; set; }
		CryptoString Passphrase { get;  set; }

		void AddFile(string testFile, string directoryPathInArchive = "");
		void AddDirectory(string directoryName, string directoryNameInArchive = "");
		void Save();
		void ExtractAll(string destinationPath);
	}
}
