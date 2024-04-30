using DG_SocketAssist6.Client;
using GameLoopProc;
using StandUpYou.Client.Global;
using StandUpYou.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StandUpYou.Client.Faculty
{
    /// <summary>
    /// 클라이언트 처리를 위한 메인 모델
    /// </summary>
    internal class ClientModel
    {

        /// <summary>
        /// 서버 연결
        /// </summary>
        /// <remarks>
        /// 이 개체가 생성되기 전에 이 개체를 사용하려고 한다면 throw를 일으켜야 한다.
        /// </remarks>
        private ClientSocket? ClientMy;

        /// <summary>
        /// 사용할 게임 루프
        /// </summary>
        private GameLoopStopwatch GameLoop;

        /// <summary>
        /// 사용할 아이디
        /// </summary>
        public string Id { get; private set; } = string.Empty;

#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
        public ClientModel()
        {
            
        }
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.

        /// <summary>
        /// 서버 연결 시작
        /// </summary>
        public void ConnectStart(string sIp, int nPort, string sId)
        {
            this.Id = sId;

            //클라이언트 개체 생성
            this.ClientMy = new ClientSocket(sIp, nPort);
            this.ClientMy.OnLog += ClientMy_OnLog;

            this.ClientMy.OnConnectionComplete += Client_OnConnectionComplete;
            this.ClientMy.OnDisconnect += Client_OnDisconnect;
            this.ClientMy.OnDisconnectCompleted += Client_OnDisconnectCompleted;

            this.ClientMy.OnReceiveReady += Client_OnReceiveReady;
            this.ClientMy.OnMessaged += Client_OnMessaged;

            this.Log("클라이언트 준비 완료");

            this.Log("서버 연결 시작");
            this.ClientMy.ConnectServer();
        }

        public async Task StartLoop()
        {
            this.GameLoop = new GameLoopStopwatch(60);

            //게임 루프 시작 - 프로그램 스래드 대기
            await this.GameLoop.Start();
        }


        /// <summary>
        /// 접속 끊기
        /// </summary>
        public void Disconnect()
        {
            if(null != this.ClientMy)
            {
                this.ClientMy.Disconnect();
            }

        }

        #region 클라이언트 이벤트 콜백
        private void ClientMy_OnLog(int nLogType, string sMessage)
        {
            //this.Log(string.Format("[Client:{0}] {1}"
            //            , nLogType
            //            , sMessage));
        }

        /// <summary>
        /// 서버 연결 완료
        /// </summary>
        /// <param name="sender">이 이벤트를 발생시킨 개체</param>
        private void Client_OnConnectionComplete(ClientSocket sender)
        {
            this.Log("*** 서버 연결 성공 ***");

            //서버에서 유저가 준비될때까지 로그인을 할수 없다.
            //일단 대기
        }

        /// <summary>
        /// 서버와 끊김처리가 시작됨
        /// </summary>
        /// <param name="sender">이 이벤트를 발생시킨 개체</param>
        private void Client_OnDisconnect(ClientSocket sender)
        {
            this.Log("*** 서버 끊김 ***");
            GlobalStatic.MainForm!.UI_Setting(ConsoleUi.UiStateType.Disconnect);
        }
        /// <summary>
        /// 서버와 끊김 처리가 완료됨
        /// </summary>
        /// <param name="sender">이 이벤트를 발생시킨 개체(개체가 null일수 있다.)</param>
        private void Client_OnDisconnectCompleted(ClientSocket sender)
        {
            //끊김 처리 완료
        }

        /// <summary>
        /// 메시지 수신 준비가 완료됨
        /// </summary>
        /// <param name="sender"></param>
        private void Client_OnReceiveReady(ClientSocket sender)
        {
            //DisplayMsg("메시지 수신 준비");
        }

        /// <summary>
        /// 서버로 부터 메시지가 수신됨.
        /// <para>서버로 부터 온 메시지를 해석하여 처리한다.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="byteData"></param>
        private void Client_OnMessaged(ClientSocket sender, byte[] byteData)
        {
            //원본 데이터를 문자열로 바꾼다.
            string sDataOri = Encoding.UTF8.GetString(byteData);

            //this.Log(string.Format("[Client_OnMessaged] {0}"
            //                        , sDataOri));

            //구분자로 명령을 구분 한다.
            string[] sData = GlobalStatic.ChatCmd.ChatCommandCut(sDataOri);

            //데이터 개수 확인
            if ((1 <= sData.Length))
            {
                //0이면 빈메시지이기 때문에 별도의 처리는 없다.

                //넘어온 명령
                ChatCommandType typeCommand
                    = GlobalStatic.ChatCmd.StrIntToType(sData[0]);

                switch (typeCommand)
                {
                    case ChatCommandType.None://없다
                        break;
                    case ChatCommandType.Msg://메시지인 경우
                        this.Commd_ReceiveMsg(sData[1]);
                        break;

                    case ChatCommandType.Client_Ready:
                        //로그인 시작
                        this.Log($"[Client_OnMessaged] SignIn : {this.Id}");
                        this.SendMsg(ChatCommandType.SignIn, this.Id);
                        break;
                    case ChatCommandType.SignIn_Ok:
                        this.Log("사인인 성공 : " + this.Id);
                        GlobalStatic.MainForm!.UI_Setting(ConsoleUi.UiStateType.Connect);
                        break;
                    case ChatCommandType.SignIn_Fail:
                        this.Log("사인인 실패 : " + this.Id);
                        GlobalStatic.MainForm!.UI_Setting(ConsoleUi.UiStateType.None);
                        break;
                }
            }
        }

        /// <summary>
        /// 메시지를 서버에게 보낸다.
        /// </summary>
        /// <param name="typeChatCommand"></param>
        /// <param name="sMessage"></param>
        public void SendMsg(
            ChatCommandType typeChatCommand
            , string sMessage)
        {
            

            string sToss
                = GlobalStatic.ChatCmd
                    .ChatCommandString(
                        typeChatCommand
                        , sMessage);

            //this.Log($"메시지 보내기 요청 : {sToss}");

            //원본 데이터를 문자열로 바꾼다.
            byte[] byteDataOri = Encoding.UTF8.GetBytes(sToss);

            this.ClientMy!.Send(byteDataOri);
        }
        #endregion



        #region 명령 처리 관련
        /// <summary>
        /// 체팅 메시지 표시
        /// </summary>
        /// <param name="sMsg"></param>
        private void Commd_ReceiveMsg(string sMsg)
        {
            //GlobalStatic.MainForm!.DisplayMsg(sMsg);
        }
        #endregion

        /// <summary>
        /// 로그 출력
        /// </summary>
        /// <param name="sMsg"></param>
        private void Log(string sMsg)
        {
            GlobalStatic.MainForm!.DisplayLog(sMsg);
        }
    }
}
