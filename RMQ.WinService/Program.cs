using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;

namespace RMQ.WinService
{
    static class Program
    {
        /// <summary>
        /// 主要服務進入點
        /// </summary>
        static void Main()
        {
            ServiceBase[] services =
            {
                new NotificationService()//之後改泛型
            };
            if (Environment.UserInteractive)
            {
                RunInteractive(services);
            }
            else
            {
                ServiceBase.Run(services);
            }
        }
        /// <summary>
        /// 當Debug狀態
        /// </summary>
        /// <param name="services"></param>
         public static void RunInteractive(ServiceBase[] services)
        {
            Console.WriteLine();
            Console.WriteLine("Install the services in interactive mode.");
            Console.WriteLine();
            // Get the method to invoke on each service to start it
            var onStartMethod =
                typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            // 啟動服務迴圈
            foreach (var service in services)
            {
                Console.WriteLine("Installing {0} ... ", service.ServiceName);
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Console.WriteLine("Installed {0} ", service.ServiceName);
                Console.WriteLine();
            }
            // 等待
            Console.WriteLine("Press a key to uninstall all services...");
            Console.ReadKey();
            Console.WriteLine();
            // 取得服務關閉功能，關閉服務
            var onStopMethod = typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            // 關閉迴圈
            foreach (var service in services)
            {
                Console.Write("Uninstalling {0} ... ", service.ServiceName);
                onStopMethod.Invoke(service, null);
                Console.WriteLine("Uninstalled {0}", service.ServiceName);
            }
            Console.WriteLine();
            Console.WriteLine("All services are uninstalled.");
            // 等待任何key Enter就結束程序
            if (Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.Write("=== Press a key to quit ===");
                Console.ReadKey();
            }
        }

    }
}
