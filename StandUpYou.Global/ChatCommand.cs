using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandUpYou.Global;

public enum ChatCommandType
{
    /// <summary>
    /// 기본 상태
    /// </summary>
    None = 0,

    /// <summary>
    /// 아이디 체크(S->C)
    /// </summary>
    ID_Check,
    /// <summary>
    /// 아이디 체크 성공
    /// </summary>
    ID_Check_Ok,
    /// <summary>
    /// 아이디 체크 실패
    /// </summary>
    ID_Check_Fail,

    /// <summary>
    /// 접속한 유저가 있다.
    /// </summary>
    User_Connect,
    /// <summary>
    /// 접속을 끊은 유저가 있다.
    /// </summary>
    User_Disonnect,
    /// <summary>
    /// 유저 리스트를 보냅니다.
    /// </summary>
    User_List,
    /// <summary>
    /// 유저 리스트를 갱신을 요청 합니다.
    /// </summary>
    User_List_Get,

    /// <summary>
    /// 아이디 무셜성이 확인 된후 호출함
    /// </summary>
    Login,
    /// <summary>
    /// 서버에서 모든 로그인 과정이 완료 되었다고 클라이언트에게 알림
    /// </summary>
    Login_Complete,
    /// <summary>
    /// 로그아웃
    /// </summary>
    Logout,

    /// <summary>
    /// 메시지 전송
    /// </summary>
    Msg,
}
