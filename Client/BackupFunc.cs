using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public static class BackupFunc
    {
        public static void DoFullBackup(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath);
        }
       /* public static void Copy()
        {
            if (IsFile())
                File.Copy(Path.Combine(path, name), Path.Combine(destinationpath, name));
            else
            {
                DirectoryInfo Source = new DirectoryInfo(path + name);
                DirectoryInfo Target = new DirectoryInfo(destinationpath);

                CopyDirectories(Source, Target);
            }
        }
        public static void CopyDirectories(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName + name);

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (DirectoryInfo SourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
                CopyDirectories(SourceSubDir, nextTargetSubDir);
            }
        }
        public static bool IsFile()
        {
            if (Tables[WindowSwitch].FileInput.LoadList[Tables[WindowSwitch].CurrentPos].File)
                return true;
            return false;
        }*/
    }
}
