using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandUpYou.Global;

/// <summary>
/// 서버/클라이언트간 통신에 사용할 명령어
/// </summary>
public enum ChatCommandType
{
    /// <summary>
    /// 기본 상태
    /// </summary>
    None = 0,

    /// <summary>
    /// [S->C] 클라이언트가 정상적으로 접속되어 다음 메시지를 춘비하고 있는 상태
    /// </summary>
    Client_Ready,


    /// <summary>
    /// [C->S] 서버에게 사인을 요청한다.
    /// <para>사인인용 데이터를 같이 보내야함</para>
    /// </summary>
    SignIn,
    /// <summary>
    /// [S->C] 사인인이 성공하여 서버에서 사인인 과정이 완료됨
    /// </summary>
    SignIn_Ok,
    /// <summary>
    /// [S->C] 사인인이 실패하여 서버에서 끊김 처리가 진행중
    /// </summary>
    SignIn_Fail,

    /// <summary>
    /// [C->S] 사인 아웃 요청
    /// </summary>
    Signout,


    /// <summary>
    /// [S->C] 접속한 유저가 있다.
    /// <para>사인인 까지 완료하여 다른 유저에게 접속자를 알리는 상태</para>
    /// </summary>
    User_Connect,
    /// <summary>
    /// [S->C] 접속을 끊은 유저가 있다.
    /// </summary>
    User_Disonnect,
    /// <summary>
    /// [S->C] 유저 리스트를 보낸다.
    /// </summary>
    User_List,
    /// <summary>
    /// [C->S] 유저 리스트를 갱신을 요청 한다.
    /// </summary>
    User_List_Get,



    /// <summary>
    /// 메시지 전송
    /// </summary>
    Msg,
}

/// <summary>
/// 체팅 명령어 처리 유틸
/// </summary>
public class ChatCommand
{
    /// <summary>
    /// 문자열로된 숫자를 명령어 타입으로 바꿔줍니다.
    /// 입력된 문자열이 올바르지 않다면 기본상태를 줍니다.
    /// </summary>
    /// <param name="sData"></param>
    /// <returns></returns>
    public ChatCommandType StrIntToType(string sData)
    {
        //넘어온 명령
        ChatCommandType typeCommand = ChatCommandType.None;

        int nCommand = 0;

        if (true == Int32.TryParse(sData, out nCommand))
        {//숫자 변환 성공

            //입력된 명령이 숫자라면 명령 타입으로 변환한다.
            typeCommand = (ChatCommandType)nCommand;
        }

        return typeCommand;
    }

    /// <summary>
    /// 채팅에 사용할 명령어 구조를 문자열로 만들어 리턴한다.
    /// </summary>
    /// <param name="typeChatCommand"></param>
    /// <param name="sMessage"></param>
    /// <returns></returns>
    public string ChatCommandString(
        ChatCommandType typeChatCommand
        , string sMessage)
    {
        StringBuilder sReturn = new StringBuilder();

        sReturn.Append(typeChatCommand.GetHashCode());
        sReturn.Append(ChatSetting.Delimeter);
        sReturn.Append(sMessage);

        return sReturn.ToString();
    }

    /// <summary>
    /// 채팅에 사용할 명령어 구조를 구분자로 잘라 리턴한다.
    /// </summary>
    /// <param name="sMessage"></param>
    /// <returns></returns>
    public string[] ChatCommandCut(string sMessage)
    {
        //구분자로 명령을 구분 한다.
        return sMessage.Split(ChatSetting.Delimeter);
    }
}
