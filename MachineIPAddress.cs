using System.Net;

namespace ITMSInstallerServer {
    class MachineIPAddress {
        public IPAddress IP { get; private set; }
        public int WebPort { get; private set; }
        public int DataPort => WebPort + 1;

        public MachineIPAddress(IPAddress ip, int port = 0) {
            IP = ip;
            WebPort = port;
        }

        public MachineIPAddress(string ip, int port = 0) {
            IP = IPAddress.Parse(ip);
            WebPort = port;
        }

        public string GetWebAddress(string scheme = "https") => $"{scheme}://{IP}:{WebPort}/";

        public string GetDataAddress(string scheme = "http") => $"{scheme}://{IP}:{DataPort}/";

        public static string[] GetAllIPv4String() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList
                .Select(ip => ip.ToString())
                .Where(ip => ip.Contains('.'))
                .Where(ip => !ip.StartsWith("127."))
                .ToArray();
        }

        public static IPAddress[] GetAllIPv4() => GetAllIPv4String()
            .Select(ip => IPAddress.Parse(ip))
            .ToArray();
    }
}