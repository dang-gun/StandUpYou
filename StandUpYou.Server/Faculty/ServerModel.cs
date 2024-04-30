
using StandUpYou.Global;
using DG_SocketAssist6.Server;

using StandUpYou.Server.Faculty.User;
using StandUpYou.Server.Global;
using System.Text;
using GameLoopProc;
using System.Timers;

namespace StandUpYou.Server.Faculty
{
    internal class ServerModel
    {
        /// <summary>
        /// 사용할 서버 소켓
        /// </summary>
        private ServerSocket Server;

        /// <summary>
        /// 사용할 게임 루프
        /// </summary>
        private GameLoopStopwatch GameLoop;


        /// <summary>
        /// 관리할 유저 리스트
        /// </summary>
        private UserListModel UserList;

#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
        public ServerModel()
        {
            
        }
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.

        /// <summary>
        /// 서버 시작
        /// <para>루프는 하지 안으므로 루프를 하려면 Start를 호출해야 한다.</para>
        /// </summary>
        public void StartServer(int nPort)
        {
            //서버 개체 생성
            this.Server = new ServerSocket(nPort);
            this.Server.OnLog += this.Server_OnLog;
            this.Server.OnConnected += this.Server_OnConnected;
            this.Server.OnDisconnect += Server_OnDisconnect;
            this.Server.OnDisconnectCompleted += Server_OnDisconnectCompleted;


            this.UserList = new UserListModel();
            this.UserList.OnLog += UserList_OnLog;

            this.UserList.OnMessaged += UserList_OnMessaged;


            this.Server.Start();
            Console.WriteLine("★★★★ Server ready");
        }

        public void StartLoop()
        {
            this.GameLoop = new GameLoopStopwatch(60);

            //게임 루프 시작 - 프로그램 스래드 대기
            this.GameLoop.Start().Wait();
        }


        /// <summary>
        /// 서버 종료
        /// </summary>
        public void StopServer()
        {
            this.Server.Stop();
            this.GameLoop.Stop();
        }

        /// <summary>
        /// Update가 호출되면 호출되는 함수
        /// <para>Update는 온전히 게임 루프 관련 처리만 하도록 함수를 분리한다.</para>
        /// </summary>
        public void UpdateLoop()
        {
            //this.KeyPress(Console.ReadKey());
        }


        #region 서버 소켓 이벤트

        /// <summary>
        /// 서버에서 절달된 로그
        /// </summary>
        /// <param name="nLogType"></param>
        /// <param name="sMessage"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void Server_OnLog(int nLogType, string sMessage)
        {
            //this.Log(string.Format("[server:{0}] {1}"
            //                        , nLogType
            //                        , sMessage));
        }

        /// <summary>
        /// 클라이언트가 접속 성공함
        /// </summary>
        /// <param name="sender"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void Server_OnConnected(ClientModel sender)
        {
            //유저 체크 시작
            this.UserList.UserCheckStart(sender);
        }

        
        /// <summary>
        /// 클라이언트 접속끊김이 감지되어 끊김 작업이 시작됨
        /// </summary>
        /// <param name="sender"></param>
        private void Server_OnDisconnect(ClientModel sender)
        {
            //끊김 처리가 시작되었으면 중간에 취소될리가 없으므로 그냥 끊어졌다고 판단하고 작업한다.

            //끊어진 대상 이름
            string sName = string.Empty;
            UserDataModel? udTemp = this.UserList.FindUser(sender.ClientIndex);
            if (null != udTemp)
            {
                sName = udTemp.UserName;
            }

            //UI에서 제거
            this.UserListUi_Remove(sName);
            //리스트에서 제거
            this.UserList.UserList_Remove(sender);

            this.Log($"[Server_OnDisconnect] {sName}");
            this.Log($"[Server_OnDisconnect] Total : {this.UserList.UserCount}");
        }

        /// <summary>
        /// 클라이언트 끊김 작업이 완료됨
        /// </summary>
        /// <param name="nClientIndex"></param>
        private void Server_OnDisconnectCompleted(long nClientIndex)
        {
            
        }
        #endregion


        #region 유저 리스트 이벤트

        private void UserList_OnLog(int nLogType, string sMessage)
        {
            //this.Log(string.Format("[UserList:{0}] {1}"
            //            , nLogType
            //            , sMessage));
        }


        /// <summary>
        /// 유저로 부터 전달된 메시지
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserList_OnMessaged(
            UserDataModel sender
            , ChatCommandType typeCommand
            , string sMag)
        {


            //this.Log(string.Format("[UserList_OnMessaged] {0} : {1}"
            //                        , typeCommand
            //                        , sMag));

            StringBuilder sbMsg = new StringBuilder();

            switch (typeCommand)
            {
                case ChatCommandType.Msg:
                    this.Commd_ReceiveMsg(sender, sMag);
                    break;

                case ChatCommandType.SignIn:   //id체크
                    this.Commd_SignIn(sender, sMag);
                    break;
            }
        }
        #endregion

        #region 명령 처리 관련


        /// <summary>
        /// 받은 메시지 체팅창에 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sMsg"></param>
        private void Commd_ReceiveMsg(UserDataModel sender, string sMsg)
        {
            string sTossMsg
                = string.Format("{0} : {1}"
                    , sender.UserName
                    , sMsg);

            //GlobalStatic.MainForm.DisplayMsg(sTossMsg);

            //모든 유저에게 메시지 전달
            this.SendMsg_All(sTossMsg);
        }

        /// <summary>
		/// 명령 처리 - 이름 체크
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="sName"></param>
		private void Commd_SignIn(UserDataModel sender, string sName)
        {
            //사용 가능 여부
            bool bReturn = true;

            //모든 유저의 아이디 체크
            if("server" == sName)
            {//예약된 아이디이다.
                bReturn = false;
            }
            else if(null != this.UserList.FindUser(sName))
            {//같은 유저가 있다!
                bReturn = false;
            }

            if (true == bReturn)
            {//사용 가능

                //이름을 지정하고
                sender.UserName = sName;

                //명령어 만들기
                string sSendData
                    = GlobalStatic.ChatCmd
                        .ChatCommandString(
                            ChatCommandType.SignIn_Ok
                            , string.Empty);

                sender.SendMsg_User(sSendData);

                //유저 체크 성공
                this.UserList.UserCheckOk(sender);
                this.UserListUi_Add(sender.UserName);

                this.Log($"[Commd_SignIn] {sender.UserName}");
                this.Log($"[Commd_SignIn] Total : {this.UserList.UserCount}");
            }
            else
            {
                //검사 실패를 알린다.
                string sSendData
                    = GlobalStatic.ChatCmd
                        .ChatCommandString(
                            ChatCommandType.SignIn_Fail
                            , string.Empty);

                sender.SendMsg_User(sSendData);

                //유저 체크 실패
                this.UserList.UserCheckFail(sender);
            }
        }

        #endregion

        #region 유저 리스트 UI 관련
        /// <summary>
        /// 유저 리스트 UI에 ID 추가
        /// </summary>
        /// <param name="sId"></param>
        public void UserListUi_Add(string sId)
        {
            GlobalStatic.MainForm!.UserListUi_Add(sId);
        }

        /// <summary>
        /// 유저 리스트 UI에 ID 제거
        /// </summary>
        /// <param name="sId"></param>
        public void UserListUi_Remove(string sId)
        {
            GlobalStatic.MainForm!.UserListUi_Remove(sId);
        }

        /// <summary>
        /// 유저 리스트 UI를 모두 지우고 접속자 리스트 기준으로 다시 ID를 넣는다.
        /// </summary>
        public void UserListUi_Refresh()
        {
            GlobalStatic.MainForm!.UserList_Clear();

            string[] arrUserId = this.UserList.UserNameList;
            for (int i = 0; i < arrUserId.Length; i++)
            {
                string sItem = arrUserId[i];

                this.UserListUi_Add(sItem);
            }
        }
        #endregion

        #region 메시지 관련

        /// <summary>
        /// 접속 완료된 모든 유저에게 지정된 명령을 보낸다.
        /// </summary>
        /// <param name="targetExcept">제외할 대상</param>
        /// <param name="typeChatCmd"></param>
        /// <param name="sMsg"></param>
        public void Send_All(
            UserDataModel targetExcept
            , ChatCommandType typeChatCmd
            , string sMsg)
        {
            string sSendData
                = GlobalStatic.ChatCmd
                    .ChatCommandString(typeChatCmd, sMsg);
            this.UserList.SendMsg_All(targetExcept, sSendData);
        }

        /// <summary>
        /// 접속 완료된 모든 유저에게 지정된 명령을 보낸다.
        /// </summary>
        /// <param name="typeChatCmd"></param>
        /// <param name="sMsg"></param>
        public void Send_All(ChatCommandType typeChatCmd, string sMsg)
        {
            string sSendData
                = GlobalStatic.ChatCmd
                    .ChatCommandString(typeChatCmd, sMsg);
            this.UserList.SendMsg_All(sSendData);
        }

        /// <summary>
        /// 접속 완료된 모든 유저에게 메시지를 보낸다.
        /// </summary>
        /// <param name="sMsg"></param>
        public void SendMsg_All(string sMsg)
        {
            this.Send_All(ChatCommandType.Msg, sMsg);
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
