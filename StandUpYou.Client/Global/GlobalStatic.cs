using System;

using DG_SocketAssist6.Client;
using StandUpYou.Client.Faculty;
using StandUpYou.Global;



namespace StandUpYou.Client.Global
{


    /// <summary>
    /// 전역 프로그램 변수
    /// </summary>
    static class GlobalStatic
    {
        /// <summary>
        /// 사용할 메인폼(컨트롤러 + View)
        /// </summary>
        /// <remarks>
        /// 이 개체가 생성되기 전에 이 개체를 사용하려고 한다면 throw를 일으켜야 한다.
        /// </remarks>
        public static ConsoleUi? MainForm = null;

        /// <summary>
        /// 메인 모델
        /// </summary>
        public static ClientModel MainClient = new ClientModel();

        /// <summary>
		/// 체팅 명령어 처리 유틸
		/// </summary>
		public static readonly ChatCommand ChatCmd = new ChatCommand();

    }
}
