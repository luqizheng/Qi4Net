using System.Collections.Generic;
using System.IO;

namespace Qi.IO
{
    public static class IoExtender
    {
        /// <summary>
        /// 创建一个完成Path的目录,如 c:\NoExist1\NoExist2\NoExist3,那么他会自动创建
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectories(string path)
        {
            var info = new DirectoryInfo(path);
            var infos = new List<DirectoryInfo>();
            while (info != null)
            {
                infos.Insert(0, info);
                info = info.Parent;
            }

            foreach (DirectoryInfo a in infos)
            {
                if (!a.Exists)
                    a.Create();
            }
        }

        public static FileInfo[] GetFilesEx(this DirectoryInfo parentDirectory, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            var files = new List<FileInfo>();
            foreach (string sp in searchPatterns)
            {
                files.AddRange(parentDirectory.GetFiles(sp, searchOption));
            }
            return files.ToArray();
        }

        public static FileInfo[] GetFilesEx(this DirectoryInfo parentDirectory, string searchPattern)
        {
            return GetFilesEx(parentDirectory, searchPattern, SearchOption.TopDirectoryOnly);
        }
    }
}