using StandUpYou.Global;

using StandUpYou.Server.Faculty;
using StandUpYou.Server.Configs;


namespace StandUpYou.Server.Global
{


    /// <summary>
    /// 전역 프로그램 변수
    /// </summary>
    static class GlobalStatic
    {
        /// <summary>
        /// 프로젝트 제목
        /// </summary>
        public const string Title = "FIPA 관리자";

        /// <summary>
        /// 읽어들이 설정
        /// </summary>
        public static ServerConfig ServerCfg = new ServerConfig();

        /// <summary>
        /// 사용할 메인폼(컨트롤러 + View)
        /// </summary>
        public static ConsoleUi? MainForm = null;

        /// <summary>
        /// 서버 동작을 할 리스너
        /// </summary>
        public static ServerModel MainServer = new ServerModel();

        /// <summary>
        /// 체팅 명령어 처리 유틸
        /// </summary>
        public static readonly ChatCommand ChatCmd = new ChatCommand();

    }
}
