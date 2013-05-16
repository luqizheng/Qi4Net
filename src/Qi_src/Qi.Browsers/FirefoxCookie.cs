using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace Qi.Browsers
{
    public class FirefoxCookie : ICookie
    {
        /*TABLE moz_cookies (
    id INTEGER PRIMARY KEY, 
    name TEXT, 
    value TEXT, 
    host TEXT, 
    path TEXT,
    expiry INTEGER, 
    lastAccessed INTEGER, 
    isSecure INTEGER, 
    isHttpOnly INTEGER
)*/

        public void SetCookie(HttpCookie cookie)
        {
        }

        public HttpCookie GetCookie(string name)
        {
            throw new NotImplementedException();
        }
        public string[] GetCookieNames(string hostName)
        {
            return null;
        }
        public void Clear(string name)
        {
            int procCount = Process.GetProcessesByName("firefox").Length;
            if (procCount > 0)
                throw new ApplicationException(string.Format("There are {0} instances of Firefox still running",
                                                             procCount));

            try
            {
                using (var conn = new SQLiteConnection("Data Source=" + GetFirefoxCookiesFileName()))
                {
                    conn.Open();
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "delete from moz_cookies where name='" + name + "'";
                    int count = command.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                if (
                    !(ex.ErrorCode == Convert.ToInt32(SQLiteErrorCode.Busy) ||
                      ex.ErrorCode == Convert.ToInt32(SQLiteErrorCode.Locked)))
                    throw new ApplicationException("The Firefox cookies.sqlite file is locked");
            }
        }

        private static string GetFirefoxCookiesFileName()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                       @"Mozilla/Firefox/Profiles");
            if (!Directory.Exists(path))
                throw new ApplicationException("Firefox profiles folder not found");

            string[] fileNames = Directory.GetFiles(path, "cookies.sqlite", SearchOption.AllDirectories);
            if (fileNames.Length != 1 || !File.Exists(fileNames[0]))
                throw new ApplicationException("Firefox cookies.sqlite file not found");
            return fileNames[0];
        }
    }
}