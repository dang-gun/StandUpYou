using Newtonsoft.Json;

using StandUpYou.Client.Faculty;
using StandUpYou.Client.Global;
using StandUpYou.Server.Configs;
using static StandUpYou.Client.ConsoleUi;

namespace StandUpYou.Client;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Stand Up You(internal network check tool)");

        //처리 성공여부
        bool bSuccess = true;

        //클라이언트 처리
        ClientModel Client;

        if (true == bSuccess)
        {
            Console.WriteLine("load 'ClientConfig.json'");
            {
                string sPath = Path.Combine("Configs", "ClientConfig.json");
                if (false == File.Exists(sPath))
                {
                    Console.WriteLine("There is no file.");
                    bSuccess = false;
                }
                else
                {
                    //파일을 읽고
                    string sClientConfig = File.ReadAllText(Path.Combine("Configs", "ClientConfig.json"));
                    GlobalStatic.ClientCfg
                        = JsonConvert.DeserializeObject<ClientConfig>(sClientConfig)!;
                }
            }
        }


        if (true == bSuccess)
        {
            GlobalStatic.MainForm = new ConsoleUi();


            if(string.Empty == GlobalStatic.ClientCfg.SignIn_Name)
            {
                Console.WriteLine("사인인 할 이름이 없습니다.");
            }
            else
            {
                //유아이를 세팅하고
                GlobalStatic.MainForm.UI_Setting(UiStateType.Connecting);

                //서버연결 처리 개체 
                //서버연결 시작
                GlobalStatic.MainClient
                    .ConnectStart(
                        GlobalStatic.ClientCfg.ServiceIp
                        , GlobalStatic.ClientCfg.ServicePort
                        , GlobalStatic.ClientCfg.SignIn_Name);
                //게임루프 시작
                GlobalStatic.MainClient.StartLoop().Wait();
            }

        }


        Console.WriteLine(" ");
        Console.WriteLine(" ");
        Console.WriteLine("------- Press 'R' to exit the program ------");

        ConsoleKeyInfo keyinfo;
        do
        {
            keyinfo = Console.ReadKey();
        } while (keyinfo.Key != ConsoleKey.R);
    }
}
