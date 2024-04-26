
using Newtonsoft.Json;
using StandUpYou.Server.Configs;
using StandUpYou.Server.Faculty;
using StandUpYou.Server.Global;
using System.Text.Json.Serialization;

namespace StandUpYou.Server;

internal class Program
{

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Stand Up You(internal network check tool)");

        //처리 성공여부
        bool bSuccess = true;

        //서버 리스너
        ServerModel server;

        if (true == bSuccess)
        {
            Console.WriteLine("load 'ServerConfig.json'");
            {
                string sPath = Path.Combine("Configs", "ServerConfig.json");
                if (false == File.Exists(sPath))
                {
                    Console.WriteLine("There is no file.");
                    bSuccess = false;
                }
                else
                {
                    //파일을 읽고
                    string sServerConfig = File.ReadAllText(Path.Combine("Configs", "ServerConfig.json"));
                    GlobalStatic.ServerCfg 
                        = JsonConvert.DeserializeObject<ServerConfig>(sServerConfig)!;
                }
            }
        }


        if (true == bSuccess)
        {
            GlobalStatic.MainForm = new ConsoleUi();


            server = new ServerModel();
            server.Start(GlobalStatic.ServerCfg.ServicePort);

        }




        Console.WriteLine("------- Press 'R' to exit the program ------");

        ConsoleKeyInfo keyinfo;
        do
        {
            keyinfo = Console.ReadKey();
        } while (keyinfo.Key != ConsoleKey.R);

    }
}
