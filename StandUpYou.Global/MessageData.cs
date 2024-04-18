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
	public struct MessageData
	{
		/// <summary>
		/// 데이터의 길이
		/// <para>버퍼 헤더에서 읽은 데이터의 길이가 들어 있다.</para>
		/// </summary>
		public int DataLength
		{
			get { return m_nDataLength; }
		}
		/// <summary>
		/// 데이터의 길이.(원본)
		/// <para>버퍼 헤더에서 읽은 데이터의 길이가 들어 있다.</para>
		/// </summary>
		private int m_nDataLength;

		/// <summary>
		/// 데이터가 들어있는 버퍼
		/// </summary>
		public byte[] Data
		{
			get { return m_byteData; }
			set { m_byteData = value; }
		}
		/// <summary>
		/// 데이터가 들어있는 버퍼(원본)
		/// </summary>
		private byte[] m_byteData;

		
		/// <summary>
		/// 전달받은 데이터의 길이를 세팅한다.
		/// </summary>
		/// <param name="byteBuffer">세팅해야할 데이터 길이 정보</param>
		public void SetLength(byte[] byteBuffer)
		{
			if (byteBuffer.Length < SettingData.BufferHeaderSize)
			{//설정된 버퍼헤더 크기보다 작다.
				//최소한의 크기도 오지 않았다는 뜻이다.
				return;
			}
			
			//전달받은 버퍼 크기를 설정한다.
			m_nDataLength = BitConverter.ToInt32(byteBuffer, 0);
		}

		/// <summary>
		/// 데이터 버퍼를 설정한다.
		/// <para>설정된 데이터의 크기만큼 버퍼를 만들어 준다.</para>
		/// </summary>
		public void InitData()
		{
			m_byteData = new byte[m_nDataLength];
		}

		/// <summary>
		/// 버퍼에 들어있는 데이터를 'Unicode'로 디코딩하여 리턴한다.
		/// </summary>
		/// <returns>'Unicode'로 인코딩된 데이터</returns>
		public string GetData()
		{
			return Encoding.Unicode.GetString(m_byteData);
		}

		/// <summary>
		/// 버퍼헤더를 리턴 받는다.
		/// <para>설정된 버퍼헤더의 크기로 비어있는 버퍼헤더를 만들어 리턴한다.</para>
		/// </summary>
		/// <returns>완성된 비어있는 버퍼헤더</returns>
		public byte[] GetBufferHeader()
		{
			return new byte[SettingData.BufferHeaderSize];
		}

		/// <summary>
		/// 데이터를 'Unicode'로 인코딩하여 버퍼에 저장한다.
		/// </summary>
		/// <param name="Data"></param>
		public void SetData(string Data)
		{
			m_byteData = Encoding.Unicode.GetBytes(Data);
			//완성된 데이터의 버퍼크기를 설정 한다.
			m_nDataLength = m_byteData.Length;
		}
	}
}
