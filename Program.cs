using System.Net;

namespace ITMSInstallerServer {
    class Program {
        public static string Version = "1.0.0";
        public static string Name = System.AppDomain.CurrentDomain.FriendlyName;

        private static int Main(string[] shellArguments) {
            var success = Arguments.Create(shellArguments, out var arguments);
            if (!success) return 0x02;

            Arguments.PrintVersion();
            Console.WriteLine($" + 监听端口 (-p): {arguments.WebPort}");
            Console.WriteLine($" + 数据端口:      {arguments.DataPort}");
            Console.WriteLine($" + 文件目录 (-d): {arguments.IPADirectory}");
            Console.WriteLine($" + 报告问题或提供反馈: https://github.com/baobao1270/itms-installer-server/issues");
            Console.WriteLine(string.Empty);

            Console.WriteLine("您可以通过以下地址访问本服务器，也可以在电脑上打开该地址，网页中有二维码可供手机扫描");
            Console.WriteLine(" + 注意：请访问 HTTPS 开头的地址，否则无法正常使用");
            foreach (var ip in MachineIPAddress.GetAllIPv4String()) {
                var e = new Endpoint(ip, arguments.WebPort);
                Console.WriteLine($" + 访问地址: {e.GetWebAddress()}     [请使用此地址访问]");
                Console.WriteLine($"   传输地址: {e.GetDataAddress()}      [请勿直接访问这个地址，此地址仅用于数据传输]");
            }
            Console.WriteLine(string.Empty);

            Console.WriteLine("正在生成临时 ECC 证书...");
            var certificate = new EphemeralECCCertificate(MachineIPAddress.GetAllIPv4());
            Console.WriteLine(" + 证书生成完毕，证书指纹：" + certificate.Certificate.Thumbprint);
            Console.WriteLine(" + 注意：此证书是自签名证书，浏览器会「此连接非私人连接」。请点击「显示详细信息」，然后点击「访问此网站」。");
            Console.WriteLine(" + 　　　您也可以在该步骤中点击「查看该证书」，并确认该证书的指纹（位于「细节」——「指纹」——「SHA-1」）与上面的指纹是否一致。");
            Console.WriteLine(" + 　　　此证书仅保存于内存中，关闭服务器后即销毁，下次启动服务器时会重新生成。");
            Console.WriteLine(string.Empty);

            Console.WriteLine("正在启动服务器...");
            Console.WriteLine(" + 按 Ctrl + C 退出\n\n");
            var server = new WebServer(arguments, certificate);
            server.Run();
            return 0;
        }
    }
}
