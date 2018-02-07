using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptoTools.Common.FileSystems;
using CryptoTools.CryptoFiles.DataFiles;
using System.IO;
using CryptoTools.Cryptography.Hashing;
using System.Text;
using System.Linq;
using CryptoTools.Cryptography.Utils;
using CryptoTools.CryptoFiles.Exceptions;

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
		
		[TestMethod]
		public void CryptoDataFile_SaveLoadFromBytesTest()
		{
			string dataFileName = "testFile.bin";
			string fileContent = "My Test Content";

			// Save some data
			CryptoDataFile dataFile = new CryptoDataFile(dataFileName);
			byte[] content1 = Encoding.UTF8.GetBytes(fileContent);
			dataFile.Content = content1;
			dataFile.Save();
			byte[] fileBytes1 = dataFile.SaveToBytes();

			// Now load the data and compare
			CryptoDataFile dataLoader = new CryptoDataFile(dataFileName);
			dataLoader.Load();
			dataLoader.LoadFromBytes(fileBytes1);
			byte[] content2 = dataLoader.Content;


			Assert.IsTrue(content1.SequenceEqual(content2));
			Assert.IsTrue(dataFile.Content.SequenceEqual(dataLoader.Content));
		}

		[TestMethod]
		public void CryptoDataFile_EncryptedEntireFileTest()
		{
			string passphraseString = "My Passphrase";
			CryptoString passphrase = new CryptoString(CryptoString.StringToSecureString(passphraseString));

			CryptoCredentials credentials = new CryptoCredentials
			{
				Passphrase = passphrase,
			};

			string dataFileName = "EncryptedFile.bin";
			string fileContent = "My Test Content";
			

			using (AutoDeleteFiles autoDeleteFile = new AutoDeleteFiles(dataFileName))
			{
				// Save some data
				CryptoDataFile dataFile = new CryptoDataFile(dataFileName);
				byte[] content1 = Encoding.UTF8.GetBytes(fileContent);
				dataFile.Credentials = credentials;
				dataFile.EncryptFile = true;
				dataFile.Content = content1;
				dataFile.Save();
				byte[] fileBytes1 = dataFile.SaveToBytes();

				// Now load the data and compare
				CryptoDataFile dataLoader = new CryptoDataFile(dataFileName);
				dataLoader.Credentials = credentials;
				dataLoader.EncryptFile = true;
				dataLoader.Load();
				dataLoader.LoadFromBytes(fileBytes1);
				byte[] content2 = dataLoader.Content;


				Assert.IsTrue(content1.SequenceEqual(content2));
				Assert.IsTrue(dataFile.Content.SequenceEqual(dataLoader.Content));
			}
		}

		[TestMethod]
		public void CryptoDataFile_EncryptedEntireFileInvalidCredentialTest()
		{
			CryptoCredentials credentials = new CryptoCredentials
			{
				Passphrase = new CryptoString("My Passphrase"),
				Pin = 2222
			};
			CryptoCredentials credentialsWrongPassphrase = new CryptoCredentials
			{
				Passphrase = new CryptoString("My Wrong Passphrase"),
				Pin = 2222
			};
			CryptoCredentials credentialsWrongPin = new CryptoCredentials
			{
				Passphrase = new CryptoString("My Passphrase"),
				Pin = 1234
			};

			string dataFileName = "EncryptedFile.bin";
			string fileContent = "My Test Content";
			byte[] fileContentBytes = Encoding.UTF8.GetBytes(fileContent);


			using (AutoDeleteFiles autoDeleteFile = new AutoDeleteFiles(dataFileName))
			{
				// Save some data
				CryptoDataFile dataFile = new CryptoDataFile(dataFileName);
				dataFile.Credentials = credentials;
				dataFile.EncryptFile = true;
				dataFile.Content = fileContentBytes;
				dataFile.Save();

				try
				{
					// TEST WRONG PASSPHRASE
					CryptoDataFile dataFile2 = new CryptoDataFile(dataFileName);
					dataFile2.Credentials = credentialsWrongPassphrase;
					dataFile2.EncryptFile = true;
					dataFile2.Load();
				}
				//catch (CryptoFileInvalidFormatException exception)
				//{
				//	Assert.IsTrue(exception is CryptoFileInvalidFormatException);

				//}
				catch (CryptoFileInvalidFormatException exception)
				{
					Assert.IsTrue(exception is CryptoFileInvalidFormatException);

				}
				catch (Exception exception)
				{
					Assert.Fail("Test was not expecting an Exception");	
				}
			




				
				//Assert.IsTrue(dataFile.Content.SequenceEqual(dataFile2.Content));
				
			}
		}


		[TestMethod]
		public void CryptoDataFile_CustomOptions()
		{
			string dataFileName = "EncryptedFileCustomOptions.bin";
			string fileContent = "My Test Content";


			using (AutoDeleteFiles autoDeleteFile = new AutoDeleteFiles(dataFileName))
			{
				// Save some data
				CryptoDataFile dataFile = new CryptoDataFile(dataFileName);
				byte[] content1 = Encoding.UTF8.GetBytes(fileContent);
				dataFile.Content = content1;
				dataFile.Save();
				
				// Now load the data and compare
				CryptoDataFile dataLoader = new CryptoDataFile(dataFileName);
				dataLoader.Load();
				byte[] content2 = dataLoader.Content;


				Assert.IsTrue(content1.SequenceEqual(content2));
				Assert.IsTrue(dataFile.Content.SequenceEqual(dataLoader.Content));
			}
		}
	}
}

