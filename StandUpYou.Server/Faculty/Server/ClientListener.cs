using DGSocketAssist1_Global;
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
    /// 연결된 클라이언트
    /// </summary>
    public class ClientListener
    {
        #region 클라이언트 이벤트
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
        /// 무결성 검사 완료되었음을 외부에 알린다.
        /// </summary>
        private void ValidationCompleteCall()
        {
            if (null != OnValidationComplete)
            {
                OnValidationComplete(this);
            }
        }

        /// <summary>
        /// 클라이언트 끊김 처리가 시작되었음을 알린다.
        /// </summary>
        /// <param name="sender"></param>
        public delegate void DisconnectDelegate(ClientListener sender);
        /// <summary>
        /// 클라이언 끊김 처리가 시작되었음을 알린다.
        /// <para>클라이언트가 어떤 사유에서든 끊겼음을 의미한다.</para>
        /// <para>정상 종료라면 서버에서 먼저 메시지를 보내 직접 끊는 것이 좋다.</para>
        /// </summary>
        public event DisconnectDelegate OnDisconnect;
        /// <summary>
        /// 클라이언트 끊김 처리가 시작되었음을 외부에 알림
        /// </summary>
        private void DisconnectCall()
        {
            if (null != OnDisconnect)
            {
                OnDisconnect(this);
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
        private void DisconnectCompletedCall()
        {
            if (null != OnDisconnectCompleted)
            {
                OnDisconnectCompleted(this);
            }
        }

        /// <summary>
        /// 메시지 수신
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public delegate void MessagedDelegate(ClientListener sender, string message);
        /// <summary>
        /// 메시지가 수신됨
        /// </summary>
        public event MessagedDelegate OnMessaged;
        /// <summary>
        /// 메시지가 수신되었음을을 외부에 알림
        /// </summary>
        private void MessagedCall(string message)
        {
            if (null != OnMessaged)
            {
                OnMessaged(this, message);
            }
        }
        #endregion

        /// <summary>
        /// 이 클라이언트가 연결된 Socket
        /// </summary>
        public Socket SocketMe { get; private set; }

        /// <summary>
        /// 이 클라이언트에게 전송용 SocketAsyncEventArgs
        /// </summary>
        private SocketAsyncEventArgs m_saeaSend = null;
        /// <summary>
        /// 이 클라이언트에게서오는 수신용 SocketAsyncEventArgs
        /// </summary>
        private SocketAsyncEventArgs m_saeaReceive = null;

        #region 클라이언트 유효성(validation) 검사용 함수 정의
        /// <summary>
        /// 유효성 검사에 사용할 함수를 전달하기위한 대리자.
        /// </summary>
        /// <param name="sender">검사를 요청한 클라이언트</param>
        /// <returns></returns>
        public delegate bool ValidationDelegate(ClientListener sender);
        /// <summary>
        /// 유효성 검사에 사용할 함수
        /// </summary>
        private ValidationDelegate m_ValidationFunc = null;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketMe">전달받은 Socket</param>
        public ClientListener(Socket socketMe)
        {
            //소캣을 저장한다.
            SocketMe = socketMe;

            //전송용 SocketAsyncEventArgs 세팅
            m_saeaSend = new SocketAsyncEventArgs();
            m_saeaSend.Completed -= SaeaSend_Completed;
            m_saeaSend.Completed += SaeaSend_Completed;

            //수신용 SocketAsyncEventArgs 세팅
            m_saeaReceive = new SocketAsyncEventArgs();
            m_saeaReceive.SetBuffer(new byte[SettingData.BufferFullSize], 0, SettingData.BufferFullSize);
            m_saeaReceive.Completed -= SaeaReceive_Completed;
            m_saeaReceive.Completed += SaeaReceive_Completed;


            //여기서 바로 Listening을 시작하면 이벤트가 연결되기 전에 동작이 진행될수 있다.
        }

        /// <summary>
        /// 연결된 클라이언트에서 전송한 첫 데이터를 읽기위해 대기한다.
        /// </summary>
        /// <remarks>
        /// 모든 이벤트 연결이 끝난 후 호출하는 것이 좋다.
        /// </remarks>
        public void FirstListening()
        {
            //데이터 구조 생성
            MessageData MsgData = new MessageData();
            //커낵트용 데이터 구조 지정
            m_saeaReceive.UserToken = MsgData;

            Debug.WriteLine("첫 데이터 받기 준비");
            //첫 데이터 받기 시작
            SocketMe.ReceiveAsync(m_saeaReceive);


            if (null != m_ValidationFunc)
            {//유효성 검사용 함수가 있다.
                if (false == m_ValidationFunc(this))
                {//유효성 검사 실패
                 //접속을 끊는다.
                    Disconnect(true);
                    return;
                }
            }
            //유효성 검사 함수가 없다면 검사를 하지 않는다.

            //외부에 접속허가를 알림
            ValidationCompleteCall();
        }

        /// <summary>
        /// 클라리언트에서 넘어온 데이터 받음 완료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaeaReceive_Completed(object sender, SocketAsyncEventArgs e)
        {
            //서버에서 넘어온 정보
            Socket socketClient = (Socket)sender;
            //서버에서 넘어온 데이터
            MessageData MsgData = (MessageData)e.UserToken;
            MsgData.SetLength(e.Buffer);
            MsgData.InitData();

            //유저가 연결 상태인지?
            if (true == socketClient.Connected)
            {//연결 상태이다

                //넘어온 메시지 읽기
                socketClient.Receive(MsgData.Data, MsgData.DataLength, SocketFlags.None);
                //넘어온 메시지 전달
                MessagedCall(MsgData.GetData());
                Debug.WriteLine("전달된 데이터 : {0}", MsgData.GetData());

                //다음 데이터를 기다린다.
                //'Read'에서 무한루프 없이 구현하기 위해 두번째부터는 여기서 대기하도록
                //구성되어 있다.
                socketClient.ReceiveAsync(e);
                Debug.WriteLine("데이터 받기 준비");
            }
            else
            {//아니다
             //접속 끊김을 알린다.
                Disconnect(true);
            }
        }

        /// <summary>
        /// 연결된 이 클라이언트에게 메시지를 전송 한다.
        /// </summary>
        /// <param name="sMsg"></param>
        public void Send(string sMsg)
        {
            MessageData mdMsg = new MessageData();
            mdMsg.SetData(sMsg);

            //데이터 길이 세팅
            m_saeaSend.SetBuffer(BitConverter.GetBytes(mdMsg.DataLength), 0, 4);
            //보낼 데이터 설정
            m_saeaSend.UserToken = mdMsg;
            Debug.WriteLine("데이터 전달 : {0}", sMsg);
            //보내기
            SocketMe.SendAsync(m_saeaSend);
        }

        private void SaeaSend_Completed(object sender, SocketAsyncEventArgs e)
        {
            //유저 소켓
            Socket socketClient = (Socket)sender;
            MessageData mdMsg = (MessageData)e.UserToken;
            //데이터 보내기 마무리
            socketClient.Send(mdMsg.Data);
        }

        /// <summary>
        /// 연결을 끊는다.
        /// <para>이미 끊는 이벤트가 발생했는데 bEvent를 true로 사용하는 경우
        /// 무한루프에 빠질수 있으니 조심해야 한다.</para>
        /// </summary>
        /// <param name="bEvent">연결끊김 이벤트 발생 여부.</param>
        public void Disconnect(bool bEvent)
        {
            if (true == bEvent)
            {
                DisconnectCall();
            }

            if (null != SocketMe)
            {
                SocketMe.Close();
                SocketMe = null;
            }

            if (true == bEvent)
            {
                DisconnectCompletedCall();
            }
        }

        /// <summary>
        /// 연결을 끊는다.
        /// 외부용 - 이벤트 발생 안함.
        /// </summary>
        public void Disconnect()
        {
            Disconnect(false);
        }

    }
}
