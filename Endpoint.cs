using System;
using Microsoft.AspNetCore.Http;
using ITMSInstallerServer.IOSApplicationArchive;

namespace ITMSInstallerServer {
    class Endpoint {
        public string Host { get; private set; }
        public int WebPort { get; private set; }
        public int DataPort => WebPort + 1;

        public Endpoint(string host, int webPort) {
            Host = host;
            WebPort = webPort;
        }

        public Endpoint(HostString hostString) {
            Host = hostString.Host;
            WebPort = hostString.Port ?? Arguments.DefaultPort;
        }

        public static int GetDataPortFromWebPort(int webPort) => webPort + 1;

        public string GetWebAddress(string scheme = "https") => $"{scheme}://{Host}:{WebPort}/";
        public string GetDataAddress(string scheme = "http") => $"{scheme}://{Host}:{DataPort}/";
        public string GetIPAManifestAddress(string filename, string scheme = "https") =>
            GetWebAddress(scheme) + "manifest/" + Uri.EscapeDataString(filename);
        public string GetIPAManifestAddress(IPAFile file, string scheme = "https") =>
            GetIPAManifestAddress(file.FileName, scheme);
        public string GetIPAFileAddress(string filename, string scheme = "http") {
            filename = filename.EndsWith(".ipa") ? filename : filename + ".ipa";
            return GetDataAddress(scheme) + "packages/" + Uri.EscapeDataString(filename);
        }
        public string GetIPAFileAddress(IPAFile file, string scheme = "http") =>
            GetIPAFileAddress(file.FileName, scheme);
        public string GetIPAInstallITMSAddress(string filename, string scheme = "itms-services") =>
            $"{scheme}://?action=download-manifest&url={Uri.EscapeDataString(GetIPAManifestAddress(filename))}";
        public string GetIPAInstallITMSAddress(IPAFile file, string scheme = "itms-services") =>
            GetIPAInstallITMSAddress(file.FileName, scheme);
    }
}
