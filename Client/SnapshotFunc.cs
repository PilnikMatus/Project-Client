using ClientDemon.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public class SnapshotFunc
    {
        public List<snapshot> ReadDeleted(string filepath)
        {

            using (StreamReader file = new StreamReader(filepath))
            {
                List<snapshot> deletedold = JsonConvert.DeserializeObject<snapshot[]>(file.ReadToEnd()).ToList();
                return deletedold;
            }

        }
        public void SaveSnapshot(string ssource, string ttarget)
        {
            DirectoryInfo target = new DirectoryInfo(ttarget);

            string filepath = target + @"\snapshot.txt";

            List<snapshot> snapshots = GetSnapshotList(ssource);

            using (StreamWriter file = System.IO.File.CreateText(filepath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, snapshots);
            }
        }
        public List<snapshot> GetSnapshotList(string ssource)
        {
            DirectoryInfo source = new DirectoryInfo(ssource);

            List<snapshot> snapshots = new List<snapshot>();

            foreach (FileInfo file in source.GetFiles())
            {
                snapshot s = new snapshot();
                s.change = file.LastWriteTime.ToString();
                s.path = file.FullName;
                s.type = "file";

                snapshots.Add(s);
            }

            foreach (DirectoryInfo SourceSubDir in source.GetDirectories())
            {
                snapshot s = new snapshot();
                s.change = SourceSubDir.LastWriteTime.ToString();
                s.path = SourceSubDir.FullName;
                s.type = "folder";

                snapshots.Add(s);

                snapshots.AddRange(GetSnapshotList(SourceSubDir.FullName));
            }
            return snapshots;
        }
        public List<snapshot> ReadPreviousSnapshot(string target)
        {
            FileSystemInfo fileInfo = new DirectoryInfo(target).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();

            using (StreamReader file = new StreamReader(target + @"\snapshot.txt"))
            {
                List<snapshot> snapshots = JsonConvert.DeserializeObject<snapshot[]>(file.ReadToEnd()).ToList();
                return snapshots;
            }
        }
        public void DeletedSave(string ttarget, List<snapshot> deleted)
        {

            DirectoryInfo target = new DirectoryInfo(ttarget);

            string filepath = target + @"\deleted.txt";

            if (System.IO.File.Exists(filepath))
                deleted.AddRange(ReadDeleted(filepath));

            using (StreamWriter file = System.IO.File.CreateText(filepath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, deleted);
            }
        }
        public void SaveDeleted(List<snapshot> rs, List<snapshot> cs, string targetPath)
        {
            List<snapshot> deleted = new List<snapshot>();

            bool pokus;

            foreach (snapshot item1 in rs)
            {
                pokus = false;
                foreach (snapshot item2 in cs)
                {
                    if (item1.path == item2.path &&
                       item1.type == item2.type &&
                       item1.change == item2.change)
                        pokus = true;
                }
                if (!pokus)
                    deleted.Add(item1);
            }

            DeletedSave(targetPath, deleted);
        }
        public List<snapshot> GetSnapshotDifference(List<snapshot> rs, List<snapshot> cs)
        {
            bool same;

            List<snapshot> difference = new List<snapshot>();

            foreach (snapshot item1 in cs)
            {
                same = false;
                foreach (snapshot item2 in rs)
                {
                    if (item1.path == item2.path &&
                       item1.type == item2.type &&
                       item1.change == item2.change)
                        same = true;
                }
                if (!same)
                    difference.Add(item1);
            }

            return difference;
        }
    }
}
