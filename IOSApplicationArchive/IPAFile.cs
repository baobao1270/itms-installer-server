using System.IO;

namespace ITMSInstallerServer.IOSApplicationArchive {
    public class IPAFile {
        public string FilePath { get; private set; } = "";
        public string FileName => Path.GetFileNameWithoutExtension(FilePath);
        public string BaseName => Path.GetFileName(FilePath);
        public long Size => new FileInfo(FilePath).Length;

        public IPAFile(string path) {
            FilePath = Path.GetFullPath(path);
        }

        public IPAManifest ReadManifest() {
            return new IPAManifest(this);
        }

        public string GetHumanReadableSize() {
            var units = new string[] { "B", "KiB", "MiB", "GiB", "TiB" };
            decimal size = Size;
            int unit = 0;
            while (size >= 1024) {
                size /= 1024;
                unit++;
            }
            return $"{size.ToString("#.##")} {units[unit]}";
        }
    }
}
