
namespace StandUpYou.Server.Global;

/// <summary>
/// 아무런 전달이 없는 경우 사용할 대리자
/// </summary>
internal delegate void EmptyDelegate();

/// <summary>
/// 로그용 대리자
/// </summary>
/// <param name="nLogType">로그 성격</param>
/// <param name="sMessage"></param>
public delegate void LogDelegate(int nLogType, string sMessage);
