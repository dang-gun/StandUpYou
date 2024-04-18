using StandUpYou.Server.Faculty.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandUpYou.Server.Faculty;

/// <summary>
/// 서버 기능 구현
/// </summary>
internal class MyServer
{
    private ServerModel server;

    public MyServer(int nPort)
    {
        this.server = new ServerModel(nPort);
        this.server.OnStart += Server_OnStart;
        this.server.OnStop += Server_OnStop;

        this.server.OnConnected += Server_OnConnected;
        this.server.OnValidationComplete += Server_OnValidationComplete;
        this.server.OnDisconnect += Server_OnDisconnect;
        this.server.OnMessaged += Server_OnMessaged;

        this.server.Start();

        //유저 리스트 초기화
        this.m_listUser = new List<User>();
    }

    private void Server_OnStart()
    {
        throw new NotImplementedException();
    }

    private void Server_OnStop()
    {
        throw new NotImplementedException();
    }


    private void Server_OnConnected(ClientListener sender)
    {
        throw new NotImplementedException();
    }

    private void Server_OnValidationComplete(ClientListener sender)
    {
        throw new NotImplementedException();
    }

    private void Server_OnDisconnect(ClientListener sender)
    {
        throw new NotImplementedException();
    }

    private void Server_OnMessaged(ClientListener sender, string message)
    {
        throw new NotImplementedException();
    }
}
