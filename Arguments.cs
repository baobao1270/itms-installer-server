namespace ITMSInstallerServer{
    public class Arguments {
        public static int DefaultPort = 7120;
        public static string DefaultIpkDirectory = "./IPA";

        public int WebPort { get; private set; }
        public int DataPort => Endpoint.GetDataPortFromWebPort(WebPort);
        public string IPADirectory { get; private set; } = DefaultIpkDirectory;

        public static bool Create(string[] arguments, out Arguments? outArguments) {
            outArguments = null;
            if (!TryParesArguments(arguments, out var parsedArgumentsMapping)) {
                PrintUsage();
                return false;
            }
            if (parsedArgumentsMapping.ContainsKey("h")) {
                PrintUsage();
                return false;
            }
            if (parsedArgumentsMapping.ContainsKey("v")) {
                PrintVersion();
                return false;
            }
            if (!int.TryParse(parsedArgumentsMapping.GetValueOrDefault("p", DefaultPort.ToString()), out var port)) {
                Console.Error.WriteLine("端口号必须是一个整数");
                return false;
            }

            outArguments = new Arguments {
                WebPort = port,
                IPADirectory = parsedArgumentsMapping.GetValueOrDefault("d", DefaultIpkDirectory)
            };
            return true;
        }

        private static bool TryParesArguments(string[] args, out Dictionary<string, string> parsed) {
            parsed = new Dictionary<string, string>();
            try {
                for (var i = 0; i < args.Length; i++) {
                    if (args[i].StartsWith("-")) {
                        parsed.Add(args[i].Substring(1), args[i + 1]);
                    }
                }
            } catch (IndexOutOfRangeException) {
                return false;
            }
            return true;
        }

        public static void PrintUsage() {
            PrintVersion();
            Console.WriteLine($"使用方法: {Program.Name} [参数]\n");
            Console.WriteLine($"参数列表:");
            Console.WriteLine($"  -p <port>          监听的端口");
            Console.WriteLine($"                     默认: {DefaultPort}");
            Console.WriteLine($"  -d <dir>           IPA 文件的目录");
            Console.WriteLine($"                     默认: {DefaultIpkDirectory}");
            Console.WriteLine($"  -h                 显示此帮助信息");
            Console.WriteLine($"  -v                 显示版本信息");
        }

        public static void PrintVersion() {
            Console.WriteLine($"{Program.Name} {Program.Version}");
        }
    }
}