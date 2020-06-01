using ClientDemon.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public class FileFunc
    {
        public void CheckParentDirectory(string targetPath, bool ZIP)
        {
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
        }
        public void DeleteOldBackups(string targetPath, bool ZIP)
        {

            if (Directory.GetFiles(targetPath).Length > 5) //smazat nejstarší - max 5
            {
                FileSystemInfo fileInfo = new DirectoryInfo(targetPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                File.Delete(fileInfo.FullName);
            }
            else if (Directory.GetDirectories(targetPath).Length > 5) //smazat nejstarší - max 5
            {
                FileSystemInfo fileInfo = new DirectoryInfo(targetPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                Directory.Delete(fileInfo.FullName, true);
            }
        }
        public void Copy(string source, string target, List<snapshot> previousSnapshot = null)
        {

            DirectoryInfo Source = new DirectoryInfo(source);
            DirectoryInfo Target = new DirectoryInfo(target);

            CopyDirectories(Source, Target, previousSnapshot);

        }
        public void CopyDirectories(DirectoryInfo source, DirectoryInfo target, List<snapshot> previousSnapshot)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo file in source.GetFiles())
            {
                if (previousSnapshot != null)
                {
                    foreach (snapshot item in previousSnapshot)
                    {
                        if (item.path == file.FullName)
                            file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                    }
                }
                else
                    file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (DirectoryInfo SourceSubDir in source.GetDirectories())
            {
                if (previousSnapshot != null)
                {
                    foreach (snapshot item in previousSnapshot)
                    {
                        if (item.path == SourceSubDir.FullName)
                        {
                            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
                            CopyDirectories(SourceSubDir, nextTargetSubDir, previousSnapshot);
                        }
                    }
                }
                else
                {
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
                    CopyDirectories(SourceSubDir, nextTargetSubDir, previousSnapshot);
                }
            }
        }
        public string CheckDirectoriesGetRealName(string targetPath, bool ZIP)
        {
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            CheckParentDirectory(targetPath, ZIP);

            if (Directory.GetDirectories(targetPath).Length > 0)
            {
                FileSystemInfo fileInfo2 = new DirectoryInfo(targetPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last();
                if (Directory.GetDirectories(fileInfo2.FullName).Length > 5 - 1 || Directory.GetFiles(fileInfo2.FullName).Length > 5 + 1)
                {
                    targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                    Directory.CreateDirectory(targetPath);
                }   
                else
                    targetPath = fileInfo2.FullName; 
            }
            else if (Directory.GetDirectories(targetPath).Length == 0)
            {
                targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Directory.CreateDirectory(targetPath);
            }

            return targetPath;
        }
    }
}