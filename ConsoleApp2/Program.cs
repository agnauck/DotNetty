using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        public static void CollectRuntimeInfo()
        {
            var typeAppContext = Type.GetType("System.AppContext");
            var typeSslStream = typeof(SslStream);
            var assemblySystem = typeSslStream.GetTypeInfo().Assembly;

            Console.WriteLine("Version1: " + Type.GetType("System.Environment")?.GetProperty("Version")?.GetValue(null));
            Console.WriteLine("Version2: " + typeAppContext?.GetProperty("TargetFrameworkName")?.GetValue(null));
            try
            {
                dynamic hklm = Type.GetType("Microsoft.Win32.Registry")?.GetField("LocalMachine").GetValue(null);
                if (hklm != null)
                {
                    var k1 = hklm.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
                    var ver = $"{k1.GetValue("Version")}({k1.GetValue("Release")})";
                    var sku = string.Join(", ", hklm.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319\SKUs").GetSubKeyNames());

                    Console.WriteLine($"Version3: {ver}, skus= {sku}");
                }
            }
            catch { }
#if !NETCOREAPP1_1 && !NETCOREAPP2_0
            Console.WriteLine("Version4: " + AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName);
#endif
            {
                object[] pars = new object[2] { "Switch.System.Net.DontEnableSchUseStrongCrypto", false };
                var res = typeAppContext?.GetMethod("TryGetSwitch")?.Invoke(null, pars);
                if (res == (object)true)
                {
                    Console.WriteLine("DontEnableSchUseStrongCrypto(>netfx46): " + pars[1]);
                }
            }
            var typeServicePointManager = assemblySystem.GetType("System.Net.ServicePointManager");
            var typeSecurityProtocol = assemblySystem.GetType("System.Net.SecurityProtocol");
            Console.WriteLine("SecurityProtocol: " + typeServicePointManager?.GetProperty("SecurityProtocol", BindingFlags.Static | BindingFlags.Public)?.GetValue(null));
            Console.WriteLine("DisableStrongCrypto: " + typeServicePointManager?.GetProperty("DisableStrongCrypto", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null));
            Console.WriteLine("DefaultSslProtocols(netfx): " + typeServicePointManager?.GetProperty("DefaultSslProtocols", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null));
            Console.WriteLine("DefaultSslProtocols2(netfx): " + typeSslStream.GetMethod("DefaultProtocols", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(new SslStream(new MemoryStream()), new object[0]));
            Console.WriteLine("AllowedSecurityProtocols(netcore): " + typeSecurityProtocol?.GetField("AllowedSecurityProtocols", BindingFlags.Static | BindingFlags.Public)?.GetValue(null));
            Console.WriteLine("DefaultSecurityProtocols(netcore): " + typeSecurityProtocol?.GetField("DefaultSecurityProtocols", BindingFlags.Static | BindingFlags.Public)?.GetValue(null));
            Console.WriteLine("SystemDefaultSecurityProtocols(netcore): " + typeSecurityProtocol?.GetField("SystemDefaultSecurityProtocols", BindingFlags.Static | BindingFlags.Public)?.GetValue(null));

            Console.WriteLine("------------");
        }

        static void Main(string[] args)
        {
            CollectRuntimeInfo();

#if !NETCOREAPP1_1
            AppDomain.CurrentDomain.FirstChanceException += (s, e) => Console.Error.WriteLine("FirstChanceException: " + e.Exception);
#endif

            try
            {
                EndPoint servAddr;
                using (var server = new Socket(SocketType.Stream, ProtocolType.Tcp))
                {
                    server.Bind(new IPEndPoint(IPAddress.Loopback, 62222));
                    servAddr = server.LocalEndPoint;

                    server.Listen(10);

                    using (var cli = new Socket(SocketType.Stream, ProtocolType.Tcp))
                    {
                        cli.Connect(servAddr);
                        using (var ss = new SslStream(new NetworkStream(server.Accept()), false, (sender, certificate, chain, sslPolicyErrors) =>true))
                        using (var cs = new SslStream(new NetworkStream(cli), false, (sender, certificate, chain, sslPolicyErrors) => true))
                        {
                            var cert = GetTestCertificate2();
                            var host = cert.GetNameInfo(X509NameType.DnsName, false);

                            var t1 = Task.Factory.StartNew(() => cs.AuthenticateAsClientAsync(host, null, SslProtocols.Tls12, false)).Unwrap();
                            var t2 = Task.Factory.StartNew(() => ss.AuthenticateAsServerAsync(cert)).Unwrap();

                            Task.WaitAll(t1, t2);
                            Console.WriteLine(cs.IsAuthenticated + ", " + ss.IsAuthenticated);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            Console.WriteLine("Press <any key> to contine...");
            Console.ReadKey(true);
        }
        public static X509Certificate2 GetTestCertificate2()
        {
            byte[] certData;
            using (Stream resStream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream(typeof(Program).Namespace + "." + "localhost.pfx"))
            using (var memStream = new MemoryStream())
            {
                resStream.CopyTo(memStream);
                certData = memStream.ToArray();
            }

            return new X509Certificate2(certData, "123456");
        }
    }
}
