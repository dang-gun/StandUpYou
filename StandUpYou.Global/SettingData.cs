using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGSocketAssist1_Global
{
	/// <summary>
	/// 서버와 클라이언트가 주고/받는 데이터 구조체
	/// </summary>
	public class SettingData
	{
		/// <summary>
		/// 버퍼의 정보가 들어있는 헤더의 크기
		/// </summary>
		public static int BufferHeaderSize = 4;

		/// <summary>
		/// 소켓이 한번에 받을 수 있는 최대 버퍼 크기.<br />
		/// SocketAsyncEventArgs를 생성할때 사용되는 버퍼 크기이다.
		/// </summary>
		public static int BufferFullSize = 8192;

        /// <summary>
        /// 제목
        /// </summary>
        public static string SiteTitle = "Socket - SocketAsyncEventArgs";

        /// <summary>
        /// 명령어 구분용 문자
        /// </summary>
        public static char Delimeter = '▦';
    }
}
