using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandUpYou.Server.Configs;

/// <summary>
/// 서버 설정 파일용 데이터모델
/// </summary>
internal class ClientConfig
{

    /// <summary>
    /// 접속할 서버 IP
    /// <para>127.0.0.1</para>
    /// </summary>
    public string ServiceIp { get; set; } = "127.0.0.1";
    /// <summary>
    /// 접속할 서버의 포트
    /// <para>기본값 7000</para>
    /// </summary>
    public int ServicePort { get; set; } = 7000;

    /// <summary>
    /// 사인인에 사용할 이름
    /// <para>다른 클라이언트와 중복되면 안된다.</para>
    /// </summary>
    public string SignIn_Name { get; set; } = "Test1";

}
