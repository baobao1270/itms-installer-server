using System.Xml.Linq;

namespace ITMSInstallerServer.IOSApplicationArchive {
    public class IPAPlist {
        private readonly XDocument document;

        public IPAPlist(Stream stream) {
            document = XDocument.Load(stream);
        }

        public string GetEntryValue(string key) {
            return document.Root
                .Element("dict")
                .Elements("key")
                .Where(x => x.Value == key)
                .Select(x => x.NextNode as XElement)
                .Select(x => x.Value)
                .FirstOrDefault(string.Empty);
        }
    }
}