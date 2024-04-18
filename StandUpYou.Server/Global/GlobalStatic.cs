

using StandUpYou.Server.Configs;

namespace StandUpYou.Server.Global;

/// <summary>
/// Static으로 선언된 적역 변수들
/// </summary>
internal static class GlobalStatic
{
    /// <summary>
    /// 프로젝트 제목
    /// </summary>
    public const string Title = "FIPA 관리자";

    /// <summary>
    /// 읽어들이 설정
    /// </summary>
    public static ServerConfig ServerCfg = new ServerConfig();
}
