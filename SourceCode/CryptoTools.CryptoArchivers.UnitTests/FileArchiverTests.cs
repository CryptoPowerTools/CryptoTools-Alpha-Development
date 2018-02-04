using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptoTools.Cryptography.Utils;
using CryptoTools.Common.FileSystems;
using System.IO;
using System.Collections.Generic;
using CryptoTools.Cryptography.Hashing;
using System.Linq;
using System.Diagnostics;

namespace CryptoTools.CryptoArchivers.UnitTests
{
	[TestClass]
	public class FileArchiverTests
	{
		[TestMethod]
		public void FileArchiver_BasicUsage()
		{
			// Arrange
			FileManager fileMan = new FileManager();
			Hasher hasher = new Hasher();
			CryptoString passPhrase = new CryptoString(CryptoString.StringToSecureString("My Pass Phrase"));
			int fileSize = 2000;

			// Define Some Directories & Files
			string directoryName1 = "Test1";
			string directoryName2 = "Test2";
			string extractDirectory = "TestExtract";


			List<DirectoryInfo> directories = new List<DirectoryInfo>
			{
				new DirectoryInfo(directoryName1),
				new DirectoryInfo(directoryName2),
				new DirectoryInfo(extractDirectory)
			};
			List<FileInfo> files = new List<FileInfo>
			{
				new FileInfo(Path.Combine(directoryName1,"file1.bin")),
				new FileInfo(Path.Combine(directoryName1,"file2.bin")),
				new FileInfo(Path.Combine(directoryName1,"file3.bin")),
				new FileInfo(Path.Combine(directoryName1,"file4.bin")),
				new FileInfo(Path.Combine(directoryName1,"file5.bin")),
				new FileInfo(Path.Combine(directoryName1,"file6.bin")),
				new FileInfo(Path.Combine(directoryName1,"file7.bin")),
				new FileInfo(Path.Combine(directoryName1,"file8.bin")),
				new FileInfo(Path.Combine(directoryName1,"file9.bin")),
				new FileInfo(Path.Combine(directoryName1,"file10.bin")),
				new FileInfo(Path.Combine(directoryName1,"file11.bin")),
				new FileInfo(Path.Combine(directoryName1,"file12.bin")),
				new FileInfo(Path.Combine(directoryName1,"file13.bin")),
				new FileInfo(Path.Combine(directoryName2,"file14.bin")),
				new FileInfo(Path.Combine(directoryName2,"file15.bin")),
				new FileInfo(Path.Combine(directoryName2,"file16.bin")),
				new FileInfo(Path.Combine(directoryName2,"file17.bin")),
				new FileInfo(Path.Combine(directoryName2,"file18.bin")),
				new FileInfo(Path.Combine(directoryName2,"file19.bin")),
				new FileInfo(Path.Combine(directoryName2,"file20.bin")),
			};

			// Make sure any existing are deleted
			fileMan.DeleteAllFilesAndDirectories(files, directories, true);

			// Save & Extract Archive To/From a File
			using (CreateAutoDeleteDirectory d = new CreateAutoDeleteDirectory(directories))
			{
				using (CreateAutoDeleteFiles f = new CreateAutoDeleteFiles(files, true, fileSize))
				{
					// Create the Archiver, Add Directory and Save (Zip/Archive)
					CryptoArchiver archiver = new CryptoArchiver();
					archiver.FullFileName = "TestArchive.bin";
					archiver.Passphrase = passPhrase;
					archiver.AddDirectory(directoryName1, directoryName1);
					archiver.Save();

					// Extract all files
					archiver.ExtractAll(extractDirectory);

					// Compare the two directories htey should be the same
					string dir1 = directoryName1;
					string dir2 = Path.Combine(extractDirectory, directoryName1);
					Assert.IsTrue(hasher.CompareDirectoryHashSignatures(dir1, dir2));
				}
			}


			// Save & Extract Archive To/From a From Bytes
			using (CreateAutoDeleteDirectory d = new CreateAutoDeleteDirectory(directories))
			{
				using (CreateAutoDeleteFiles f = new CreateAutoDeleteFiles(files, true, fileSize))
				{
					// Create the Archiver, Add Directory and Save (Zip/Archive) from Bytes
					CryptoArchiver archiver = new CryptoArchiver();
					archiver.Passphrase = passPhrase;
					archiver.AddDirectory(directoryName1, directoryName1);
					byte[] bytes = archiver.SaveToBytes();


					// Extract all files from the bytes
					archiver.ExtractAllFromBytes(extractDirectory, bytes);

					// Compare the two directories htey should be the same
					string dir1 = directoryName1;
					string dir2 = Path.Combine(extractDirectory, directoryName1);
					Assert.IsTrue(hasher.CompareDirectoryHashSignatures(dir1, dir2));

				}
			}
		}

		[TestMethod]
		public void FileArchiver_ExampleReference()
		{
			//CryptoArchiver archiver = new CryptoArchiver();
			//archiver.FullFileName = "TestArchive.bin";
			//archiver.AddDirectory("Archived Files");

			//// Create Archive File & Extract Files
			//archiver.Save();
			//archiver.ExtractAll("Extracted Files");


			//// Create Archive Byte Buffer & Extract - Useful if you want to include the encrypted file in another structure
			//byte[] bytes = archiver.SaveToBytes();
			//archiver.ExtractAllFromBytes("Extracted Files", bytes);

		}



		private string GetRandomDirName(List<DirectoryInfo> directories)
		{
			CryptoRandomizer randomizer = new CryptoRandomizer();
			int index = randomizer.Next(0, directories.Count);
			return directories[index].FullName;
		}
		
		
		[TestMethod]
		public void FileArchiver_StressTest()
		{
			// Arrange
			CryptoRandomizer random = new CryptoRandomizer();
			FileManager fileMan = new FileManager();
			Hasher hasher = new Hasher();
			CryptoString passPhrase = new CryptoString(CryptoString.StringToSecureString("Tests"));
			int fileSize = 10000;

			// Define Some Directories & Files
			//string archiveDirectory = "ArchiveDirectory";
			string extractDirectory = "ExtractDirectory";
			int iterationSeed = 30;


			int iterations = random.Next(iterationSeed/2, iterationSeed*2);

			try
			{
				

				for (int i = 0; i < iterations; i++)
				{
					Stopwatch sw = Stopwatch.StartNew();

					int directoryCount = random.Next(iterationSeed / 2, iterationSeed * 2);
					List<DirectoryInfo> directories = new List<DirectoryInfo>();
					for (int d = 0; d < directoryCount; d++)
					{
						directories.Add(new DirectoryInfo(fileMan.GenerateTempDirectoryName()));
					}
					//	directories.Add(new DirectoryInfo(extractDirectory));


					int fileCount = random.Next(iterationSeed * 5, iterationSeed * 10);
					List<FileInfo> files = new List<FileInfo>();
					for (int f = 0; f < fileCount; f++)
					{
						string fileName = $"{Path.Combine(GetRandomDirName(directories), fileMan.GenerateTempFileName("bin", false))}";
						files.Add(new FileInfo(fileName));
					}
					using (CreateAutoDeleteDirectory d = new CreateAutoDeleteDirectory(directories))
					{
						using (CreateAutoDeleteFiles f = new CreateAutoDeleteFiles(files, true, fileSize))
						{


							//////////////////////////////////////////////////////
							// Save to File and Verify
							/////////////////////////////////////////////////////// Create the Archiver, Add Directory and Save (Zip/Archive)
							CryptoArchiver archiver = new CryptoArchiver();
							string fileName = fileMan.GenerateTempFileName("archive", true);
							archiver.FullFileName = fileName;
							archiver.Passphrase = passPhrase;
							foreach (var dirs in directories)
							{
								archiver.AddDirectory(dirs.Name, dirs.Name);
							}
							archiver.Save();

							// Extract all files
							archiver.ExtractAll(extractDirectory);

							foreach (var dirs in directories)
							{
								string dir1 = dirs.Name;
								string dir2 = Path.Combine(extractDirectory, dir1);
								Assert.IsTrue(hasher.CompareDirectoryHashSignatures(dir1, dir2));
							}

							//////////////////////////////////////////////////////
							// Save the Bytes and Verify
							/////////////////////////////////////////////////////
							CryptoArchiver archiver2 = new CryptoArchiver();
							//string fileName2 = fileMan.GenerateTempFileName("archive", true);
							//archiver2.FullFileName = fileName;
							archiver2.Passphrase = passPhrase;
							foreach (var dirs in directories)
							{
								archiver2.AddDirectory(dirs.Name, dirs.Name);
							}
							byte[] bytes = archiver2.SaveToBytes();
							byte[] rawBytes = File.ReadAllBytes(fileName);
							Assert.IsTrue(bytes.SequenceEqual(rawBytes));

							// Clean up files
							fileMan.DeleteFile(fileName);
							//fileMan.DeleteFile(fileName2);
							fileMan.DeleteDirectory(extractDirectory, true);

						}
					}
					Debug.WriteLine($"Test Loop Elapsed Milliseconds : {sw.ElapsedMilliseconds}");


				}

			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception.Message);

				throw;
			}
		}
	}
}
