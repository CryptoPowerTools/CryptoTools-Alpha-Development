using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptoTools.Common.FileSystems;
using CryptoTools.CryptoFiles.DataFiles;
using System.IO;
using CryptoTools.Cryptography.Hashing;

namespace CryptoTools.CryptoFiles.UnitTests.DataFiles
{
	[TestClass]
	public class CryptoDataFileTests
	{
		[TestMethod]
		public void TestMethod1()
		{
		}

		[TestMethod]
		public void CreateWriteReadDataFile()
		{
			FileManager fileOps = new FileManager();
			Hasher hasher = new Hasher();

			CryptoDataFile file1 = null;
			CryptoDataFile file2 = null;
			string dataFileName1 = "EncryptedTestDataFile1.dat";
			string dataFileName2 = "EncryptedTestDataFile2.dat";
			string passphrase = "testpassphrase";
			string wrongPassphrase = "wrongtestpassphrase";

			fileOps.DeleteFile(dataFileName1);



			////////////////////////////////////////////////////////////
			// Write
			////////////////////////////////////////////////////////////
			try
			{
				// Write Content to data file
				file1 = new CryptoDataFile(dataFileName1);
				//file1.EncryptContent = true; // This is the default
				//file1.Passphrase = passphrase;
				file1.Content = new ByteGenerator().GenerateBytes(1000);
				file1.Save();

			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			////////////////////////////////////////////////////////////
			// Read / Load
			////////////////////////////////////////////////////////////
			try
			{
				// Read & Load File
				file2 = new CryptoDataFile(dataFileName1);
				//file2.Passphrase = passphrase;
				//file2.EncryptContent = true;
				file2.Load();

				// make a copy of the file to compare
				File.Copy(dataFileName1, dataFileName2);

				// Compare
				string h1 = hasher.HashFile(dataFileName1);
				string h2 = hasher.HashFile(dataFileName2);
				Assert.IsTrue(h1 == h2);

			}
			catch (Exception ex)
			{
				// Delete Files
				fileOps.DeleteFile(dataFileName1);
				Assert.Fail(ex.Message);

			}
			finally
			{								
			}

			////////////////////////////////////////////////////////////
			// Read / Load Wrong Password
			////////////////////////////////////////////////////////////
			try
			{
				// Read & Load File
				file2 = new CryptoDataFile(dataFileName1);
				//file2.Passphrase = wrongPassphrase;
				//file2.EncryptContent = true;
				file2.Load();

				Assert.Fail("Load should fail with wrong password");
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			finally
			{
				// Delete Files
				fileOps.DeleteFile(dataFileName1);
				fileOps.DeleteFile(dataFileName2);
			}


		}
	}
}

