using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandUpYou.Client;

internal class ConsoleUi
{
    /// <summary>
    /// 나의 상태
    /// </summary>
    public enum typeState
    {
        /// <summary>
        /// 없음
        /// </summary>
        None = 0,
        /// <summary>
        /// 연결중
        /// </summary>
        Connecting,
        /// <summary>
        /// 연결 완료
        /// </summary>
        Connect,

        /// <summary>
        /// 연결 끊김
        /// </summary>
        Disconnect,
    }

    /// <summary>
    /// 나의 상태
    /// </summary>
    private typeState m_typeState = typeState.None;

    public ConsoleUi()
    { 
    }

    #region 유저 리스트 UI

    #endregion


    /// <summary>
    /// UI 세팅
    /// </summary>
    /// <param name="typeSet"></param>
    public void UI_Setting(typeState typeSet)
    {
        //들어온 값을 세팅하고
        m_typeState = typeSet;

        switch (typeSet)
        {
            case typeState.None://기본
            case typeState.Disconnect: //끊김

                //처음으로 돌리기위해 typeState.None로 초기화 한다.
                m_typeState = typeState.None;
                break;

            case typeState.Connecting:  //연결중
                break;
            case typeState.Connect: //연결완료
                break;
        }
    }


    public void DisplayLog(string nMessage)
    {
        StringBuilder buffer = new StringBuilder();

        //시간 추가
        buffer.Append(
            String.Format("[{0}] "
                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

        //출력할 메시지 완성
        buffer.Append(nMessage);

        Console.WriteLine(buffer.ToString());
    }
}
