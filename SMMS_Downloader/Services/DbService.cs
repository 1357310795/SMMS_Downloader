using SQLite;
using System.IO;

namespace SMMS_Downloader.Services
{
    public class DbService
    {
        public static SQLiteConnection db;
        public static string dbpath { get; set; }
        public static void Init()
        {
            dbpath = Path.Combine(PathHelper.AppPath, "smms.db");
            Directory.CreateDirectory(PathHelper.AppPath);
            db = new SQLiteConnection(dbpath);
        }
    }
}
