using StandUpYou.Global;
using StandUpYou.Server.Faculty.Server;


namespace StandUpYou.Server.Faculty;

/// <summary>
/// 리스트로 관리될 유저 모델
/// </summary>
public class User
{
	#region 외부에 연결될 이벤트
	/// <summary>
	/// 유저 준비
	/// </summary>
	/// <param name="sender"></param>
	public delegate void UserReadyDelegate(User sender);
	/// <summary>
	/// 유저 준비
	/// </summary>
	public event UserReadyDelegate OnUserReady;
	/// <summary>
	/// 유저 준비가 완료되었음을 외부에 알림
	/// </summary>
	private void UserReadyCall()
        {
		if (null != OnUserReady)
		{
			this.OnUserReady(this);
		}
        }

	/// <summary>
	/// 유저 메시지 요청
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void MessageDelegate(User sender, MessageEventArgs e);
	/// <summary>
	/// 메시지
	/// </summary>
	public event MessageDelegate OnMessaged;

	/// <summary>
	/// 유저 로그인 완료
	/// </summary>
	/// <param name="sender"></param>
	public delegate void LoginCompletDelegate(User sender);
	/// <summary>
	/// 유저 로그인 완료되면 발생함
	/// </summary>
	public event LoginCompletDelegate OnLoginComplet;

	/// <summary>
	/// 클라이언트 끊김 처리 완료
	/// </summary>
	/// <param name="sender"></param>
	public delegate void DisconnectCompletedDelegate(User sender);
	/// <summary>
	/// 클라이언트가 끊김처리가 완료 되었다.
	/// </summary>
	public event DisconnectCompletedDelegate OnDisconnectCompleted;
	/// <summary>
	/// 클라이언트 끊김 처리 완료되었음을 외부에 알림
	/// </summary>
	private void DisconnectCompletedCall()
	{
		if (null != OnDisconnectCompleted)
		{
			this.OnDisconnectCompleted(this);
		}
	}
	#endregion


	/// <summary>
	/// 이 유저의 클라이언트 리스너 개체
	/// </summary>
	public ClientListener ClientListenerMe { get; private set; }

	/// <summary>
	/// 이 유저의 아이디
	/// </summary>
	public string UserID { get; set; }


	/// <summary>
	/// 유저 객체를 생성합니다.
	/// </summary>
	/// <param name="newClientListener">접속된 클라이언트의 리스너 개체</param>
	public User(ClientListener newClientListener)
	{
		//소켓 저장
		this.ClientListenerMe = newClientListener;
		
		//메시지 분석 연결
		this.ClientListenerMe.OnMessaged += ClientListenerMe_OnMessaged;
            //끊김 이벤트 연결
            this.ClientListenerMe.OnDisconnectCompleted += ClientListenerMe_OnDisconnectCompleted;

		//유저 준비를 알림
		this.UserReadyCall();

		//아이디 체크를 시작하라고 알림
		this.SendMsg_User(ChatCommandType.ID_Check, "");
	}

        private void ClientListenerMe_OnDisconnectCompleted(ClientListener sender)
        {
		DisconnectCompletedCall();
	}

        private void ClientListenerMe_OnMessaged(ClientListener sender, string message)
	{
		//구분자로 명령을 구분 한다.
		string[] sData = GloblaStatic.ChatCmd.ChatCommandCut(message);


		//데이터 개수 확인
		if ((1 <= sData.Length))
		{
			//0이면 빈메시지이기 때문에 별도의 처리는 없다.

			//넘어온 명령
			ChatCommandType typeCommand
				= GloblaStatic.ChatCmd.StrIntToType(sData[0]);

			switch (typeCommand)
			{
				case ChatCommandType.None:   //없다
					break;
				case ChatCommandType.Msg:    //메시지인 경우
					SendMeg_Main(sData[1], typeCommand);
					break;
				case ChatCommandType.ID_Check:   //아이디 체크
					SendMeg_Main(sData[1], typeCommand);
					break;

				case ChatCommandType.User_List_Get:  //유저리스트 갱신 요청
					SendMeg_Main("", typeCommand);
					break;

				case ChatCommandType.Login:  //로그인 완료
					OnLoginComplet(this);
					break;
			}
		}
	}


	/// <summary>
	/// 서버로 메시지를 보냅니다.
	/// </summary>
	/// <param name="sMag"></param>
	/// <param name="typeCommand"></param>
	private void SendMeg_Main(string sMag, ChatCommandType typeCommand)
	{
		MessageEventArgs e = new MessageEventArgs(sMag, typeCommand);

		OnMessaged(this, e);
	}

	/// <summary>
	/// 이 유저에게 체팅 명령 문자열을 만들어 메시지를 보낸다.
	/// </summary>
	/// <param name="typeChatCommand"></param>
	/// <param name="sMsg"></param>
	public void SendMsg_User(ChatCommandType typeChatCommand, string sMsg)
	{
		string sToss
			= GloblaStatic.ChatCmd.ChatCommandString(
				typeChatCommand
				, sMsg);
		this.ClientListenerMe.Send(sToss);
	}

	/// <summary>
	/// 이 유저에게 메시지를 보낸다.
	/// </summary>
	/// <param name="sMsg"></param>
	public void SendMsg_User(string sMsg)
	{
		this.ClientListenerMe.Send(sMsg);
	}

	/// <summary>
	/// 이 유저를 끊는다.
	/// </summary>
	public void Disconnect()
	{
		this.ClientListenerMe.Disconnect(true);
	}
}
