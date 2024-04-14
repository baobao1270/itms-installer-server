using System.IO;

namespace ITMSInstallerServer.IOSApplicationArchive {
    public class IPACollection {
        public string FileDirectory { get; private set; }

        public IPACollection(string path) {
            FileDirectory = Path.GetFullPath(path);
        }

        public IEnumerable<IPAFile> GetFiles() => Directory
            .GetFiles(FileDirectory, "*.ipa")
            .Select(x => new IPAFile(x))
            .ToArray();
    }
}
