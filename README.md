개발중.... 0.3


# 기립하시오! 당신도! (Stand Up! You!) - 네트워크 확인 툴(Network Check Tool)

네트워크간에 접속을 유지하고 확인하는 툴입니다.

서버/클라이언트를 연결하고 주기적으로 이를 확인하므로서 장시간 네트워크 상태를 모니터링 할 수 있습니다.

서버는 주기적으로 클라이언트에서 메시지를 보냅니다.

클라이언트는 여기에 응답합니다.

운영체제에서 관리하는 'KeepAlive'와 직접 메시지를 보내는 두가지 방법을 통해 네트워크가 정상인지 확인합니다.


_프로그램이름은 밈을 페러디 한것입니다.
이 프로그램은 어떠한 사상이나 가치관과 전혀 상관없습니다._



## 사용방법

1. 서버와 클라이언트에 파일 복사
서버와 클라이언트 역할을 할 장치를 정하고 
- 서버에는 'StandUpYou.Server'를 복사하고
- 클라이언트에는 StandUpYou.Client을 복사합니다.
   클라이언트는 여러대를 사용할 수 있습니다.


2. 서버 설정
'ServerConfig.json'를 열어

1. 클라이언트 설정
'ClientConfig.json'를 열어


## 동작
서버는 설정된 시간마다 클라이언트에게 일어서기 명령(PleaseStandUp)을 보내기 전에 준비하라는 명령(AttentionPlease)을 보냅니다.명령을 보냅니다.

1초정도 대기한 이후 일어서기 명령(PleaseStandUp)을 보냅니다.

클라이언트는 일어서기 명령(PleaseStandUp)명령을 받으면 바로 일어나기(GetUp)명령을 서버로 보냅니다.


서버와 연결이 끊기면 5초 대기 후 다시 접속을 시도합니다.


