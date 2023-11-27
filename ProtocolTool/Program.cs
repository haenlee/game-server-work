using System.Diagnostics;

namespace ProtocolTool
{
    public class ProtoCodeGen
    {
        public List<string> ProtoGenFiles = new List<string>();
        public string OutputFile = string.Empty;
        public CodeGenerator? CodeGenerator;
    }

    public class ProtoConfig
    {
        public string ProtoPath = string.Empty;
        public string MainProtoFileName = string.Empty;
        public ProtoCodeGen? ProtoCodeGen = null;
    }

    public class Program
    {
        static string _outputDir = "../GameServer/Proto";

        static ProtoConfig GameProto()
        {
            var files = Directory.GetFiles("./proto", "*.proto", SearchOption.TopDirectoryOnly).Select(e => Path.GetFileNameWithoutExtension(e));

            return new ProtoConfig
            {
                ProtoPath = "./proto",
                MainProtoFileName = "Game.proto",
                ProtoCodeGen = new ProtoCodeGen
                {
                    ProtoGenFiles = files.Select(e => Path.Combine(_outputDir, $"{e}.cs")).ToList(),
                    OutputFile = Path.Combine(_outputDir, "ProtoHelper.cs"),
                    CodeGenerator = new CodeGenerator("GameProto")
                }
            };
        }

        static async Task Main(string[] args)
        {
            // Proto 배치 파일 실행
            Console.WriteLine($"{Directory.GetCurrentDirectory()}");

            var psFile = Path.GetFullPath("./Bat/GenProto.ps1");
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy ByPass -File \"{psFile}\"",
                UseShellExecute = false,
                WorkingDirectory = Path.GetFullPath("./Bat/"),
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Console.WriteLine($"run. {psFile}");

            Process? proc = Process.Start(startInfo);
            if (proc != null)
            {
                proc.WaitForExit();
                StreamReader reader = proc.StandardError;
                string output = reader.ReadToEnd();
                Console.WriteLine(output);

                Console.WriteLine($"ExitCode: {proc.ExitCode}");
            }

            // Proto Helper 스크립트 생성
            ProtoConfig gameProto = GameProto();
            var codeGen = gameProto.ProtoCodeGen;
            if (codeGen == null)
                return;

            if (codeGen.CodeGenerator == null)
                return;

            string content = codeGen.CodeGenerator.Generate(gameProto.ProtoPath, gameProto.MainProtoFileName);
            await File.WriteAllTextAsync(codeGen.OutputFile, content);

            Console.WriteLine($"{codeGen.OutputFile} generated.");
        }
    }
}