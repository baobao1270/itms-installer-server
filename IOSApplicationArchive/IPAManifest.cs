using System.IO;
using System.IO.Compression;

namespace ITMSInstallerServer.IOSApplicationArchive {
    public class IPAManifest {
        public IPAFile File { get; private set; }
        public string BundleId { get; private set; } = "";
        public string BundleName { get; private set; } = "";
        public string BundleVersion { get; private set; } = "";

        public IPAManifest(IPAFile file) {
            File = file;
            using (var zipFile = ZipFile.OpenRead(file.FilePath)) {
                var manifest = zipFile.GetEntry("iTunesMetadata.plist");
                if (manifest == null)
                    throw new FileNotFoundException($"iTunesMetadata.plist not found in {file.FilePath}");
                using (var stream = manifest.Open()) {
                    var plist = new IPAPlist(stream);
                    BundleId = plist.GetEntryValue("softwareVersionBundleId");
                    BundleName = plist.GetEntryValue("bundleDisplayName");
                    BundleVersion = plist.GetEntryValue("bundleShortVersionString");
                }
            }
        }
    }
}
