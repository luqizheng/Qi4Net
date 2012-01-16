using System;
using System.IO;
using System.Linq;

namespace Qi
{
    public class ApplicationHelper
    {
        /// <summary>
        /// 获取物理路径
        /// </summary>
        public static string PhysicalApplicationPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">path is null </exception>
        public static string MapPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.StartsWith("~"))
                path = path.Substring(1);
            return MapPath(new DirectoryInfo(PhysicalApplicationPath), path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootDirectory">以这个路基为开始</param>
        /// <param name="mapPath"></param>
        /// <returns></returns>
        /// <remarks>
        /// <code>
        /// var rootDirect="a/b/c";
        /// var result=MapPath(new DirectoryInfo(rootDirect),"../b/b2"); //result is /a/b/b2
        /// </code>
        /// </remarks>
        public static string MapPath(DirectoryInfo rootDirectory, string mapPath)
        {
            if (rootDirectory == null)
                throw new ArgumentNullException("rootDirectory");
            if (mapPath == null)
                throw new ArgumentNullException("mapPath");

            string currentPath = rootDirectory.FullName;

            string[] currentPathAry = currentPath.Split(
                new[]
                    {
                        currentPath.IndexOf('/') != -1 ? '/' : Path.DirectorySeparatorChar
                    },
                StringSplitOptions.RemoveEmptyEntries
                );
            string[] mapPathAry = mapPath.Split(new[]
                                                    {
                                                        mapPath.IndexOf('/') != -1 ? '/' : Path.DirectorySeparatorChar
                                                    },
                                                StringSplitOptions.RemoveEmptyEntries);
            int subFolder = mapPathAry.Count(pathParty => pathParty == "..");

            var resultSeparator = new string(Path.DirectorySeparatorChar, 1);
            currentPath = subFolder >= currentPathAry.Length
                              ? Path.GetPathRoot(currentPath)
                              : String.Join(resultSeparator, currentPathAry, 0, currentPathAry.Length - subFolder);
            return Path.Combine(currentPath ?? "",
                                string.Join(resultSeparator, mapPathAry, subFolder, mapPathAry.Length - subFolder));
        }
    }
}