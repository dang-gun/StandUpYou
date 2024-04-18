using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StandUpYou.Server.Faculty.Server
{
    /// <summary>
    /// 클라이언트의 접속을 기다리는 서버
    /// </summary>
    public class ServerModel
    {
        #region 서버 이벤트
        /// <summary>
        /// 서버 시작 델리게이트
        /// </summary>
        public delegate void StartDelegate();
        /// <summary>
        /// 서버가 시작됨
        /// </summary>
        public event StartDelegate OnStart;
        /// <summary>
        /// 서버 시작됨을 외부에 알림
        /// </summary>
        public void StartCall()
        {
            if (null != OnStart)
            {
                OnStart();
            }
        }

        /// <summary>
        /// 서버 멈춤 대리자
        /// </summary>
        public delegate void StopDelegate();
        /// <summary>
        /// 서버가 멈춤
        /// </summary>
        public event StopDelegate OnStop;
        /// <summary>
        /// 서버 멈춤을 외부에 알림
        /// </summary>
        public void StopCall()
        {
            if (null != OnStop)
            {
                OnStop();
            }
        }
        #endregion

        #region 클라이언트 이벤트
        /// <summary>
        /// 클라이언트 접속
        /// </summary>
        /// <param name="sender"></param>
        public delegate void OnConnectedDelegate(ClientListener sender);
        /// <summary>
        /// 클라이언트 접속함.
        /// </summary>
        public event OnConnectedDelegate OnConnected;
        /// <summary>
        /// 클라이언트 접속을 외부에 알림
        /// </summary>
        /// <param name="sender"></param>
        public void ConnectedCall(ClientListener sender)
        {
            if (null != OnConnected)
            {
                OnConnected(sender);
            }
        }

        /// <summary>
        /// 무결성 검사 완료
        /// </summary>
        /// <param name="sender"></param>
        public delegate void ValidationCompleteDelegate(ClientListener sender);
        /// <summary>
        /// 무결성 검사가 끝남.
        /// 로그인 처리와 같은 무결성 검사가 끝나서 서버에 진입한(혹은 가능한)
        /// 상태가 되면 발생한다.
        /// </summary>
        public event ValidationCompleteDelegate OnValidationComplete;
        /// <summary>
        /// 무결성 검사 끝남을 외부에 알림
        /// </summary>
        /// <param name="sender"></param>
        public void ValidationCompleteCall(ClientListener sender)
        {
            if (null != OnConnected)
            {
                OnValidationComplete(sender);
            }
        }

        /// <summary>
        /// 클라이언트가 끊김 처리가 시작됨
        /// </summary>
        /// <param name="sender"></param>
        public delegate void DisconnectDelegate(ClientListener sender);
        /// <summary>
        /// 클라이언트가 끊김 처리가 시작되었다.
        /// </summary>
        public event DisconnectDelegate OnDisconnect;
        /// <summary>
        /// 클라이언트가 끊김 처리가 시작되었음을 외부에 알림
        /// </summary>
        /// <param name="sender"></param>
        public void DisconnectCall(ClientListener sender)
        {
            if (null != OnDisconnect)
            {
                OnDisconnect(sender);
            }
        }

        /// <summary>
        /// 클라이언트 끊김 처리 완료
        /// </summary>
        /// <param name="sender"></param>
        public delegate void DisconnectCompletedDelegate(ClientListener sender);
        /// <summary>
        /// 클라이언트가 끊김처리가 완료 되었다.
        /// </summary>
        public event DisconnectCompletedDelegate OnDisconnectCompleted;
        /// <summary>
        /// 클라이언트 끊김 처리 완료되었음을 외부에 알림
        /// </summary>
        private void DisconnectCompletedCall(ClientListener sender)
        {
            if (null != OnDisconnectCompleted)
            {
                OnDisconnectCompleted(sender);
            }
        }

        /// <summary>
        /// 메시지가 수신
        /// </summary>
        /// <param name="sender"></param>
        public delegate void MessagedDelegate(ClientListener sender, string message);
        /// <summary>
        /// 메시지가 수신됨
        /// </summary>
        public event MessagedDelegate OnMessaged;
        /// <summary>
        /// 메시지 수신을 외부에 알림
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void MessagedCall(
            ClientListener sender
            , string message)
        {
            if (null != OnMessaged)
            {
                OnMessaged(sender, message);
            }
        }
        #endregion

        /// <summary>
        /// 접속한 클라이언트 리스트
        /// </summary>
        public List<ClientListener> ClientList = new List<ClientListener>();

        /// <summary>
        /// 서버 소켓
        /// </summary>
        private Socket socketServer;

        /// <summary>
        /// 서버 생성
        /// </summary>
        /// <param name="nPort">사용할 포트</param>
        public ServerModel(int nPort)
        {
            //유저 리스트 생성
            ClientList = new List<ClientListener>();

            //서버 세팅
            socketServer
                = new Socket(AddressFamily.InterNetwork
                            , SocketType.Stream
                            , ProtocolType.Tcp);
            //서버 ip 및 포트
            IPEndPoint ipServer
                = new IPEndPoint(IPAddress.Any, nPort);
            socketServer.Bind(ipServer);
        }

        /// <summary>
        /// 서버 시작.
        /// tcp 요청을 대기한다.
        /// </summary>
        public void Start()
        {
            Debug.WriteLine("서버 시작...");

            //수신 대기 시작
            //매개변수는 연결 대기 숫자.
            //.NET 5 이상에서는 자동으로 설정가능하다.
            //https://docs.microsoft.com/ko-kr/dotnet/api/system.net.sockets.socket.listen?view=net-6.0
            socketServer.Listen(40);

            OnStart();
            Debug.WriteLine("첫번째 클라이언트 접속 대기");

            //클라이언트 연결시 사용될 SocketAsyncEventArgs
            SocketAsyncEventArgs saeaUser = new SocketAsyncEventArgs();
            //클라이언트가 연결되었을때 이벤트
            saeaUser.Completed -= ClientConnect_Completed;
            saeaUser.Completed += ClientConnect_Completed;

            //클라이언트 접속 대기 시작
            //첫 클라이언트가 접속되기 전까지 여기서 대기를 하게 된다.
            socketServer.AcceptAsync(saeaUser);

        }

        /// <summary>
        /// 클라이언트 접속 완료.
        /// <para>하나의 클라이언트가 접속했음을 처리한다.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ClientConnect_Completed(object sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine("클라이언트 접속됨 : {0}"
                , ((IPEndPoint)e.AcceptSocket.RemoteEndPoint).ToString());

            //유저 객체를 만든다.
            ClientListener newUser = new ClientListener(e.AcceptSocket);
            //각 이벤트 연결
            newUser.OnValidationComplete += NewUser_OnValidationComplete;
            newUser.OnDisconnect += NewUser_OnDisconnect;
            newUser.OnDisconnectCompleted += NewUser_OnDisconnectCompleted;
            newUser.OnMessaged += NewUser_OnMessaged;

            //리스트에 클라이언트 추가
            ClientList.Add(newUser);
            //클라이언트 접속을 알림.
            ConnectedCall(newUser);

            //클라이언트의 데이터 전송을 대기한다.
            newUser.FirstListening();


            //다시 클라이언트 접속 대기 시작
            Debug.WriteLine("클라이언트 접속 대기");
            //이렇게 구성하는 이유는 'Start'에서 무한 루프 없이
            //클라이언트 대기를 구현하기 위해서이다.
            Socket socketServer = (Socket)sender;
            e.AcceptSocket = null;
            socketServer.AcceptAsync(e);
        }


        /// <summary>
        /// 로그인 처리와 같은 무결성 검사가 끝났다.
        /// </summary>
        /// <param name="sender"></param>
        private void NewUser_OnValidationComplete(ClientListener sender)
        {
            //무결성 검사 끝남을 알림
            ValidationCompleteCall(sender);
        }

        /// <summary>
        /// 클라이언트가 끊김 처리가 시작되었다.
        /// </summary>
        /// <param name="sender"></param>
        private void NewUser_OnDisconnect(ClientListener sender)
        {
            DisconnectCall(sender);
        }
        /// <summary>
        /// 클라이언트 끊김 처리가 완료됨
        /// </summary>
        /// <param name="sender"></param>
        private void NewUser_OnDisconnectCompleted(ClientListener sender)
        {
            //연결이 끊긴 클라이언트를 제거한다.
            ClientList.Remove(sender);
            sender = null;

            DisconnectCompletedCall(sender);
        }

        /// <summary>
        /// 메시지 수신
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void NewUser_OnMessaged(ClientListener sender, string message)
        {
            MessagedCall(sender, message);
        }

        /// <summary>
        /// 서버를 정지 시킨다.
        /// </summary>
        public void Stop()
        {
            try
            {
                socketServer.Close();
                socketServer = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("서버 종료중 오류 : " + ex.Message);
            }

            StopCall();
            Console.WriteLine("서버를 종료합니다.");
        }

        /// <summary>
        /// w전체 유저에게 메시지를 전달한다.
        /// </summary>
        /// <param name="sMsg"></param>
        public void AllMessage(string sMsg)
        {
            foreach (ClientListener itemCL in ClientList)
            {
                itemCL.Send(sMsg);
            }//end foreach itemCL
        }
    }
}
