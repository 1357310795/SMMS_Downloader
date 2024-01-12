using System.Text;
using System.Security.Cryptography;
using System.IO;
using SMMS_Downloader.Services;

namespace SMMS_Downloader.Modules
{
    public class StorageableBase
    {
        public virtual string FileName { get; set; }
        public virtual StorageMode Mode { get; set; }

        private string _filePath => Path.Combine(
            Mode == StorageMode.AppdataFolder ? PathHelper.AppDataPath : PathHelper.AppPath,
            FileName
        );

        public string FilePath => _filePath;

        public string Read()
        {
            var bytes = File.ReadAllBytes(_filePath);
            bytes = ProtectedData.Unprotect(bytes, null, DataProtectionScope.LocalMachine);
            return Encoding.Default.GetString(bytes);
        }

        public void Save(string text)
        {
            Directory.CreateDirectory(Mode == StorageMode.AppdataFolder ? PathHelper.AppDataPath : PathHelper.AppPath);
            File.WriteAllBytes(_filePath, ProtectedData.Protect(Encoding.Default.GetBytes(text), null, DataProtectionScope.LocalMachine));
        }

        public void Clear()
        {
            File.Delete(_filePath);
        }

        public enum StorageMode
        {
            ProgramFolder,
            AppdataFolder,
        }
    }
}
