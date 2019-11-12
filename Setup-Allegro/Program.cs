using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace Setup_Allegro {
    class Program {
        private static string allegroUri = "https://ical168.ddns.net/learning.ical.tw/files/allegro-x86_64-w64-mingw32-gcc-8.2.1-posix-seh-static-5.2.5.1.zip";
        private static string allegroPath = "Allegro.zip";
        private static string allegroUnzipPath = @".\";
        private static string allegroSrcPath = @".\allegro-x86_64-w64-mingw32-gcc-8.2.1-posix-seh-static-5.2.5.1";
        private static string allegroDestPath = @"C:\API\Allegro_5.2.5.1";

        private static string compilerUri = "https://ical168.ddns.net/learning.ical.tw/files/x86_64-w64-mingw32-gcc-8.2.1-posix-seh.zip";
        private static string compilerPath = "compiler.zip";
        private static string compilerUnzipPath = @".\";
        private static string compilerSrcPath = @".\mingw64";
        private static string compilerDestPath = @"C:\Compiler\MinGW_8.2.1";

        private static WebClient client = new WebClient();
        private static string currentFileName = "";

        static void Main(string[] args) {
            client.DownloadProgressChanged += Client_DownloadProgressChanged;

            checkArch();

            killCodeblockProcess();

            downloadFiles(client, allegroUri, allegroPath);
            downloadFiles(client, compilerUri, compilerPath);

            unzipFiles(allegroPath, allegroUnzipPath);
            unzipFiles(compilerPath, compilerUnzipPath);

            createParentDirectory(allegroDestPath);
            createParentDirectory(compilerDestPath);

            moveDirectory(allegroSrcPath, allegroDestPath);
            moveDirectory(compilerSrcPath, compilerDestPath);

            copyCodeblocksConf();
            patchCodeblocks();
            copyBackCodeblocksConf();

            Console.WriteLine("Finish. Press any key to close.");
            Console.ReadLine();
        }

        private static void checkArch() {
            bool is64bit = Environment.Is64BitOperatingSystem;
            if (!is64bit) {
                Console.WriteLine("Not 64 bit operating system.");
                Console.WriteLine("Script stopped.");

                Environment.Exit(1);
            }
        }

        private static void killCodeblockProcess() {
            Process[] codeclockProcess = Process.GetProcessesByName("codeblocks");
            if (codeclockProcess.Length > 0) {
                Console.WriteLine("Try to kill codeblocks");
                try {
                    foreach (var item in codeclockProcess) {
                        item.Kill();
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }

                Console.WriteLine("Please close all the codeblocks to continue.");
                foreach (var item in codeclockProcess) {
                    item.WaitForExit();
                }
            } else {
                Console.WriteLine("No codeblocks running. Continue.");
            }
        }

        private static string getCodeblocksDir() {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string codeblocksDir = Path.Combine(appdata, "CodeBlocks");
            if (!Directory.Exists(codeblocksDir)) {
                throw new DirectoryNotFoundException(codeblocksDir);
            }
            return codeblocksDir;
        }


        private static void copyCodeblocksConf() {
            var codeblocksDir = getCodeblocksDir();
            string configFile = Path.Combine(codeblocksDir, "default.conf");
            Console.WriteLine("Copy {0} to {1}", configFile, @".\default.conf");
            File.Copy(configFile, @".\default.conf", true);
        }

        private static void copyBackCodeblocksConf() {
            var codeblocksDir = getCodeblocksDir();
            string configFile = Path.Combine(codeblocksDir, "default.conf");
            string configBak = Path.Combine(codeblocksDir, "default.conf.bak");
            if (!File.Exists(configBak)) {
                Console.WriteLine("Create backup of {0}", configFile);
                File.Copy(configFile, configBak);
            }
            Console.WriteLine("Override {0}", configFile);
            File.Copy(@".\default.conf.new", configFile, true);
        }

        private static void patchCodeblocks() {

            XmlDocument doc = new XmlDocument();
            doc.Load(@".\default.conf");

            Console.WriteLine("Patch gcc settings.");
            XmlDocument patchGccDoc = new XmlDocument();
            patchGccDoc.LoadXml(PatchContext.gccPatch);

            XmlNode node = doc.DocumentElement.SelectSingleNode("/CodeBlocksConfig/compiler/sets/gcc");
            XmlNode parentNode = node.ParentNode;
            parentNode.ReplaceChild(
                parentNode.OwnerDocument.ImportNode(patchGccDoc.DocumentElement, true),
                node
                );

            Console.WriteLine("Patch gdb settings.");
            XmlDocument patchGdbDoc = new XmlDocument();
            patchGdbDoc.LoadXml(PatchContext.gdbPatch);

            node = doc.DocumentElement.SelectSingleNode("/CodeBlocksConfig/debugger_common/sets/gdb_debugger/conf1");
            parentNode = node.ParentNode;
            parentNode.ReplaceChild(
                parentNode.OwnerDocument.ImportNode(patchGdbDoc.DocumentElement, true),
                node
                );

            Console.WriteLine("Patch done.");

            doc.Save(@".\default.conf.new");
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static void moveDirectory(string src, string dest) {
            string fullSrc = Path.GetFullPath(src);
            string fullDest = Path.GetFullPath(dest);

            if (Directory.Exists(fullDest)) {
                Console.WriteLine("Delete old directory {0}", fullDest);
                Directory.Delete(fullDest, true);
            }

            if (Path.GetPathRoot(fullSrc) == Path.GetPathRoot(fullDest)) {
                Console.WriteLine("Move {0} to {1}", fullSrc, fullDest);
                Directory.Move(fullSrc, fullDest);
                Console.WriteLine("Move done.");
            } else {
                Console.WriteLine("Copy {0} to {1}", fullSrc, fullDest);
                DirectoryCopy(fullSrc, fullDest, true);
                Console.WriteLine("Copy Done.");
                Console.WriteLine("Delete {0}", fullSrc);
                Directory.Delete(fullSrc, true);
                Console.WriteLine("Delete done.");
            }
        }

        private static void createParentDirectory(string dir) {
            dir = Directory.GetParent(dir).FullName;
            Console.WriteLine("Create path: {0}", dir);
            Directory.CreateDirectory(dir);
        }

        private static void unzipFiles(string file, string path) {

            DirectoryInfo di = Directory.CreateDirectory(path);
            string extractPath = di.FullName;

            if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)) {
                extractPath += Path.DirectorySeparatorChar;
            }
            Console.WriteLine("Unzip {0} to {1}", file, extractPath);

            using (ZipArchive archive = ZipFile.OpenRead(file)) {
                for (int idx = 0; idx < archive.Entries.Count; ++idx) {
                    ZipArchiveEntry entry = archive.Entries[idx];

                    string fileDestPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                    if (fileDestPath.EndsWith(extractPath, StringComparison.Ordinal)) {
                        continue;
                    }

                    if (Path.GetFileName(fileDestPath).Length == 0) {
                        Directory.CreateDirectory(fileDestPath);
                    } else {
                        Directory.CreateDirectory(Path.GetDirectoryName(fileDestPath));
                        entry.ExtractToFile(fileDestPath, true);
                    }
                    Console.Write("\rProgress: {0}/{1} files ({2}%)", idx, archive.Entries.Count, idx * 100 / archive.Entries.Count);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Unzip Done.");
        }

        private static void downloadFiles(WebClient client, string uri, string path) {
            Console.WriteLine("Download {0} from {1}", path, uri);
            if (!File.Exists(path + ".downloaded")) {
                Task task = client.DownloadFileTaskAsync(uri, path);
                currentFileName = path;
                task.Wait();
                Console.WriteLine();
                File.Create(path + ".downloaded");
            }
            Console.WriteLine();
            Console.WriteLine("Download complete.");
        }

        private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            Console.Write("\rProgress: {0}/{1} ({2}%)", e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
        }
    }
}
