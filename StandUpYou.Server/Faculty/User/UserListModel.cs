
using DG_SocketAssist6.Server;
using StandUpYou.Global;
using StandUpYou.Server.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StandUpYou.Server.Faculty.User.UserDataModel;

namespace StandUpYou.Server.Faculty.User
{
    /// <summary>
	/// 접속된 유저 리스트 관리 모델
    /// <para>접속관리를 한다.</para>
	/// </summary>
    /// <remarks>
    /// 리스너 관리는 ServerSocket에서 하고 있으므로 여기서는 
    /// 이 프로젝트에서 사용하는 유저(클라이언트)를 정의한다.
    /// <para>UserDataModel관련 처리는 웬만하면 외부에서 하지 말고 여기에 위임하도록 구현하는 것이 좋다.</para>
    /// <para>접속/끊김은 ServerModel에서 하고 여기는 ServerModel에서 호출할 수 있도록 구현한다.</para>
    /// </remarks>
	public class UserListModel
    {
        #region 외부로 노출할 이벤트
        /// <summary>
        /// 로그 작성 이벤트
        /// </summary>
        internal event LogDelegate? OnLog;
        /// <summary>
        /// 로그 작성 이벤트를 알림
        /// </summary>
        private void OnLogCall(int nLogType, string sMessage)
        {
            if (null != this.OnLog)
            {
                this.OnLog(nLogType, sMessage);
            }
        }




        /// <summary>
        /// 유저로 부터 전달된 메시지 대리자
        /// </summary>
        /// <param name="sender"></param>
        public delegate void MessagedDelegate(
            UserDataModel sender
            , ChatCommandType typeCommand
            , string strMsg);
        /// <summary>
        /// 유저로 부터 전달된 메시지가 있으면 발생하는 이벤트
        /// </summary>
        public event MessagedDelegate? OnMessaged;
        /// <summary>
        /// 유저로 부터 메시지가 전달 되었음을 알림.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessagedCall(
            UserDataModel sender
            , ChatCommandType typeCommand
            , string strMsg)
        {
            if (null != OnMessaged)
            {
                this.OnMessaged(sender, typeCommand, strMsg);
            }
        }
        #endregion

        /// <summary>
		/// 접속을 성공한 클라이언트에게 부여할 고유번호 카운트
		/// </summary>
        /// <remarks>
        /// 0은 없는값으로 사용해야 하므로 실제 사용할때는 1부터 사용해야 관리가 쉽다.
        /// </remarks>
		public long ClientIndexCount { get; private set; } = 0;

        public UserListModel()
        {
            this.UserList_Temp = new List<UserDataModel>();
            this.UserList = new List<UserDataModel>();
        }

        #region 접속자 임시 리스트(사인인 전) 관련
        /// <summary>
        /// 접속자 임시 리스트(사인인 전)
        /// </summary>
        /// <remarks>
        /// 사인인 전과 후로 나누어 리스트를 가지고 있는 이유는 유저 리스트를 좀더 효율 적으로 검색하기 위함이다.
        /// </remarks>
        private List<UserDataModel> UserList_Temp;
        /// <summary>
        /// 접속자 개체를 만들어 임시 리스트에 추가하고 로그인 작업을 시작한다.
        /// </summary>
        /// <param name="sender"></param>
        public void UserCheckStart(ClientModel sender)
        {
            if (null != sender)
            {
                UserDataModel newUser = new UserDataModel(sender);
                newUser.OnLog += NewUser_OnLog;

                newUser.OnSendMessage += NewUser_OnSendMessage;

                //임시 리스트에 추가
                this.UserList_Temp.Add(newUser);


                //아이디 체크 준비가 끝났음을 알림
                newUser.SendMsg_User(ChatCommandType.Client_Ready, "");
            }
        }

        private void NewUser_OnLog(int nLogType, string sMessage)
        {
            this.OnLogCall(nLogType, sMessage);
        }

        /// <summary>
        /// 유저체크가 완료되었다.
        /// </summary>
        /// <remarks>
        /// 임시리스트에서는 제거하고 접속자 리스트로 옮겨준다.
        /// </remarks>
        /// <param name="sender"></param>
        public void UserCheckOk(UserDataModel sender)
        {
            //접속자 리스트에 추가하고
            this.UserList.Add(sender);

            //임시 리스트에서 제거한다.
            this.UserList_Temp.Remove(sender);

            //접속허가가 나면 겹치지 않는 고유번호를 할당해 준다.
            sender.ClientIndex = ++this.ClientIndexCount;

            this.OnLogCall(0, $"접속 허가 완료 : {sender.UserName}({sender.ClientMe.Ip})" );
        }

        public void UserCheckFail(UserDataModel sender)
        {
            sender.Disconnect();

            //임시 리스트에서 제거한다.
            this.UserList_Temp.Remove(sender);
        }

        #endregion

        #region 접속자 리스트 관리
        /// <summary>
        /// 접속한 유저 리스트
        /// </summary>
        private List<UserDataModel> UserList;
        
        /// <summary>
        /// 접속자 숫자
        /// </summary>
        public int UserCount { get { return this.UserList.Count; } }

        /// <summary>
        /// 유저 ID 리스트
        /// </summary>
        public string[] UserNameList 
        { 
            get 
            {
                return this.UserList.Select(s => s.UserName).ToArray();
            } 
        }


        /// <summary>
        /// 클라이언트 모델로 유저를 찾아 리스트에서 제거한다.
        /// </summary>
        /// <param name="sender"></param>
        public void UserList_Remove(ClientModel sender)
        {
            if (null != sender)
            {
                //같은 리스너를 가진 유저를 찾는다.
                UserDataModel? findUser = this.FindUser(sender);
                if (null != findUser)
                {//유저를 찾았다
                 //리스트에서 지운다.
                    this.UserList_Remove(findUser);
                }
            }
        }

        /// <summary>
        /// 유저정보로 유저를 찾아 리스트에서 제거한다.
        /// </summary>
        /// <param name="sender"></param>
        public void UserList_Remove(UserDataModel sender)
        {
            if (null != sender)
            {
                //리스트에서 지운다.
                this.UserList.Remove(sender);
            }
        }

        /// <summary>
        /// 지정한 클라이언트 리스너와 동일한 유저를 찾아 리턴한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public UserDataModel? FindUser(ClientModel sender)
        {
            //같은 리스너를 가진 유저를 찾는다.
            UserDataModel? findUser
                = this.UserList
                    .Where(w => w.ClientMe == sender)
                    .FirstOrDefault();

            return findUser;
        }

        /// <summary>
        /// 지정한 ID를 가지고 있는 유저를 찾아 리턴한다.
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        public UserDataModel? FindUser(string sName)
        {
            //같은 리스너를 가진 유저를 찾는다.
            UserDataModel? findUser
                = this.UserList
                    .Where(w => w.UserName == sName)
                    .FirstOrDefault();

            return findUser;
        }

        /// <summary>
        /// 지정한 인덱스를 가지고 있는 유저를 찾아 리턴한다.
        /// </summary>
        /// <param name="nClientIndex"></param>
        /// <returns></returns>
        public UserDataModel? FindUser(long nClientIndex)
        {
            //같은 리스너를 가진 유저를 찾는다.
            UserDataModel? findUser
                = this.UserList
                    .Where(w => w.ClientIndex == nClientIndex)
                    .FirstOrDefault();

            return findUser;
        }

        #endregion


        #region 유저 이벤트 콜백 처리 관련


        /// <summary>
        /// 유저가 서버에 알리는 메시지 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void NewUser_OnSendMessage(
            UserDataModel sender
            , ChatCommandType typeCommand
            , string strMsg)
        {
            this.OnMessagedCall(sender, typeCommand, strMsg);
        }

        #endregion

        /// <summary>
        /// 지정된 대상에게 메시지를 보낸다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sMsg"></param>
        public void SendMsg(UserDataModel target, string sMsg)
        {
            target.SendMsg_User(sMsg);
        }

        /// <summary>
        /// 접속 완료된 모든 유저에게 메시지를 보낸다.
        /// </summary>
        /// <param name="sMsg"></param>
        public void SendMsg_All(string sMsg)
        {
            this.OnLogCall(0, "UserListModel.SendMsg_All : " + sMsg);

            //모든 유저에게 메시지를 전송 한다.
            foreach (UserDataModel itemUser in this.UserList)
            {
                itemUser.SendMsg_User(sMsg);
            }
        }

        /// <summary>
        /// 지정한 유저를 제외하고 접속 완료된 모든 유저에게 메시지를 보낸다.
        /// </summary>
        /// <param name="targetExcept"></param>
        /// <param name="sMsg"></param>
        public void SendMsg_All(UserDataModel targetExcept, string sMsg)
        {
            //모든 유저에게 메시지를 전송 한다.
            foreach (UserDataModel itemUser in this.UserList)
            {
                if(targetExcept != itemUser)
                {
                    itemUser.SendMsg_User(sMsg);
                }
            }
        }
    }
}
