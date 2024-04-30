
namespace StandUpYou.Server.Configs;

/// <summary>
/// 서버 설정 파일용 데이터모델
/// </summary>
internal class ServerConfig
{
    /// <summary>
    /// 응답을 대기할 포트
    /// <para>기본값 5000</para>
    /// </summary>
    public int ServicePort { get; set; } = 7000;

    /// <summary>
    /// 접속된 클라이언트들에게 일어서기 명령을 보낼 주기(ms)
    /// <para>기본값 10초(10 * 1000)</para>
    /// </summary>
    public int StandUpCallInterval { get; set; } = 10 * 1000;
}
