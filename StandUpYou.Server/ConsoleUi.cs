using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandUpYou.Server;

/// <summary>
/// UI 처리
/// </summary>
internal class ConsoleUi
{
    /// <summary>
    /// 유저 이름 리스트
    /// </summary>
    private List<string> UserNameList = new List<string>();

    #region 유저 리스트 UI 관련
    /// <summary>
    /// 유저 리스트 UI에 ID 추가
    /// </summary>
    /// <param name="sId"></param>
    public void UserListUi_Add(string sId)
    {
        this.UserNameList.Add(sId);
    }

    /// <summary>
    /// 유저 리스트 UI에 ID 제거
    /// </summary>
    /// <param name="sId"></param>
    public void UserListUi_Remove(string sId)
    {
        this.UserNameList.Remove(sId);
    }

    /// <summary>
    /// 유저 리스트 비우기
    /// </summary>
    public void UserList_Clear()
    {
        this.UserNameList.Clear();
    }
    #endregion

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
