using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace FolderNameFormatter
{
    internal class ModifyDirectoryName
    {
        public string SourceDirectoryPath { get; set; }
        public string DestinationDirectoryPath { get; set; }
        public string TempPath { get; set; }
        public DirectoryInfo SourceDirectory { get; set; }
        public DirectoryInfo TempDirectory { get; set; }
        public DirectoryInfo DestinationDirectory { get; set; }
        private string tempDirectoryName = "TempDirectory";

        public ModifyDirectoryName()
        {
            SourceDirectoryPath = ConfigurationManager.AppSettings["SourceDirectoryPath"];
            DestinationDirectoryPath = ConfigurationManager.AppSettings["DestinationDirectoryPath"];
            TempPath = SourceDirectoryPath + "\\" + tempDirectoryName + "\\";

            SourceDirectory = new DirectoryInfo(SourceDirectoryPath);

            ValidateSourceDirectoryPath(SourceDirectory);

            TempDirectory = CreateTempDirectory();
            DestinationDirectory = CreateDestinationDirectory();
        }

        internal void Rename(Tuple<int, int, string> inputResult)
        {
            RenameDirectory(SourceDirectory, TempDirectory, inputResult);
        }

        private void RenameDirectory(DirectoryInfo sourceDirectory, DirectoryInfo tempDirectory, Tuple<int, int, string> inputResult)
        {
            int totalDirectories = 0;
            int totalDirectoriesLeft = 0;

            Console.WriteLine();

            try
            {
                var directories = sourceDirectory.EnumerateDirectories();
                totalDirectories = directories.Count();
                totalDirectoriesLeft = totalDirectories;

                foreach (var directory in directories)
                {
                    // ignore the temporary folder created above.
                    if (directory.Name.Equals(tempDirectoryName) || directory.Name.Equals(DestinationDirectory.Name))
                        continue;

                    // get modified directory name based on the operation to perform (dependent upon theinput received)
                    var modifiedDirectoryName = GetModifiedDirectoryName(directory.Name, inputResult);

                    // create the temp path to move at.
                    var newTemporaryPath = TempPath + modifiedDirectoryName;

                    // move to temporary created folder.
                    directory.MoveTo(newTemporaryPath);

                    // update the destination path to move it back.
                    var destinationPath = DestinationDirectoryPath + "\\" + modifiedDirectoryName;

                    // create a new Directory object of moved folder (to it's new position).
                    var tempDir = new DirectoryInfo(newTemporaryPath);

                    // move it back to the destination folder from temp folder.
                    tempDir.MoveTo(destinationPath);

                    // update the no of directories left to update.
                    totalDirectoriesLeft--;

                    Console.WriteLine("\"" + directory.Name + "\"" + " renamed.\n");
                }
                Console.WriteLine("\nAll folders renamed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Folder is not renamed. " + ex.Message);
                Console.WriteLine("Total Directories = {0}", totalDirectories);
                Console.WriteLine("Total Directories Moved = {0}", (totalDirectories - totalDirectoriesLeft));
                Console.WriteLine("Total Directories Left = {0}", totalDirectoriesLeft);
            }
            finally
            {
                tempDirectory.Delete();
            }
        }

        private string GetModifiedDirectoryName(string directoryName, Tuple<int, int, string> inputResult)
        {
            switch (inputResult.Item1)
            {
                case 1: // Title Case e.g (My Computer)
                    directoryName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(directoryName.ToLower());
                    break;

                case 2: // UPPER CASE e.g (MY COMPUTER)
                    directoryName = directoryName.ToUpper();
                    break;

                case 3: // lower case e.g (my computer)
                    directoryName = directoryName.ToLower();
                    break;

                case 4: // To add the text "TextToAdd" in the beginning of folder name.
                    directoryName = inputResult.Item3 + directoryName;
                    break;

                case 5: // To add the text "TextToAdd" in the end of folder name.
                    directoryName = directoryName + inputResult.Item3;
                    break;

                case 6: // For removing first 'N' characters from folder name.
                    if (directoryName.Length <= inputResult.Item2)
                    {
                        Console.WriteLine("Not enough characters to delete from Folder name.");
                        Console.WriteLine("Folder name won't be changed.");

                        return directoryName;
                    }

                    directoryName = directoryName.Substring(inputResult.Item2, directoryName.Length - inputResult.Item2);
                    break;

                case 7: // For removing last 'N' characters from folder name.
                    if (directoryName.Length <= inputResult.Item2)
                    {
                        Console.WriteLine("Not enough characters to delete from Folder name.");
                        Console.WriteLine("Folder name won't be changed.");

                        return directoryName;
                    }

                    directoryName = directoryName.Substring(0, directoryName.Length - inputResult.Item2);
                    break;

                default: break;
            }
            return directoryName;
        }

        private static void ValidateSourceDirectoryPath(DirectoryInfo sourceDirectory)
        {
            if (!sourceDirectory.Exists)
                throw new Exception("Source Directory Path not found.");

            if (!sourceDirectory.EnumerateDirectories().Any())
                throw new Exception("No folders exists at the specified source path.");
        }

        private DirectoryInfo CreateTempDirectory()
        {
            // create temp directory to move the folders
            var tempDirectory = new DirectoryInfo(TempPath);

            // if temp path doesn't exist, create one.
            if (!tempDirectory.Exists)
                tempDirectory.Create();

            return tempDirectory;
        }

        private DirectoryInfo CreateDestinationDirectory()
        {
            var destinationDirectory = new DirectoryInfo(DestinationDirectoryPath);

            // if destination path doesn't exists, create one.
            if (!destinationDirectory.Exists)
                destinationDirectory.Create();

            return destinationDirectory;
        }
    }
}