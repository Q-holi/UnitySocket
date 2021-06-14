## UnitySocket Tutorials  

<TCP/IP 기말 평가 프로젝트>   
1. 자유 주제 : 멀티스레드 기반, 소켓 프로그래밍 (python, C언어 원하는 형태에 랭귀지 선택)
2. 멀티 스레드기반 소켓프로그래밍 +  < 웹서비스 , Docker 를 활용한 서버 구축 및 서비스  등>  ( 외부 인터페이스 연동시 가점)  

이번 프로젝트가 TCP/IP 수업 프로젝트 뿐만 아닌 개인 프로젝트를 추가적으로 진행 중 입니다.      
개인적으로 공부를 하고 싶은 부분인 C#으로 코딩을 하였습니다.  
지금까지의 프로젝트 모든것이 Unity 기반으로 프로젝트를 진행하여서 이번 TCP/IP 프로젝트 또한 Unity를 사용하였습니다.  
개발 예정인 프로젝트는 소켓 통신을 이용한 멀티 플레이 게임 3D (2D로 전환 가능) 입니다.  
  
목적  
[Single game만의 재미를 넘어서 Multigame의 재미를 개발하기 위함]

설계  
1. [게임 클라이언트 실행 시 username을 입력하고 Server Connet 버튼을 클릭]
2. [게임서버에 여러 인원이 들어오면 각 플레이어에게 고유한 id 부여]
3. [개인별 클라이언트에서는 본인은 localPlayer 이고 다른 나머지 인원들은 Player]

기능<미구현>  
1. [랜덤한 위치에 플레이어들이 스폰] -> 스폰 구현(0) 랜덤 위치 (x)  
2. [각 플레이어들이 배틀로얄 진행] -> 미구현
3. [최종 우승자 판별] -> 미구현

차이점 [어려웠던 부분]  
가장 큰 차이점은 바로 C와 Python이 아닌 C#으로 진행을 하였던 것입니다.  
기존 수업시간에 했던 방식보다는 확실히 어려웠던 부분이 있고 인터넷 자료를 찾아가며 만들었습니다.  
Unity 자체를 독하하면서 익히는 중이라 Single game의 자료는 많이 찾아볼 수 있지만   
Multigame의 자료는 소캣 통신으로 기반으로 한 자료는 찾기 힘들었습니다.  
기존에 리눅스 창보다 Unity를 사용하여 UI부분에서 더욱 이해하기 쉽고 편리 했습니다. 하지만   
그 부분까지 통신을 하는 부분 구현이 매우 어려웠습니다.  


### 프로젝트 설명
프로젝트 참여 인원 : 1    
Source Coding : 윤승원  
 
_프로젝트 사용 기술_
1. Unity Engine  
2. Unity 2020.2.1f1 License  
3. Visual Studio 2019  

------------------------------------------------------------------------------------------------------  

[Programs.cs]
```C#
static void Main(string[] args)
        {
            Console.Title = "Game Server";//콘솔 창 이름을 게임 서버로 바꿈
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));//mainThread 선언
            mainThread.Start();//Thread running

            Server.Start(50, 26950);//서버를 시작한다 (최대 인원수, 서버를 사용할 포드 번호)
        }
```  
------------------------------------------------------------------------------------------------------
[Server.cs]
```C#
public static void Start(int _maxPlayers, int _port)//Program에서 받은 최대 인원 수 포드번호로 서버 실행
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();//시작과 동시에 서버 초기화

            tcpListener = new TcpListener(IPAddress.Any, Port);// tcpListener 선언 후 
            tcpListener.Start();//tcpListener 실행 하여 클라이언트 접속 대기
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);//연결을 수락

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
        }
```    
*클라이언트와 서버의 통신은 TCP로 하고 있습니다. 그러나 클라이언트에서 플레이어의 움직임을 통신할 때 UDP를 사용합니다.*  
*아직 추가적인 내용을 이해하는 중에 있어서 왜 UDP를 사용하였는지 찾아보는 중입니다.*    

  
_설명_|_실행화면_|_문제점(어려운 부분)_ 
:---:|:---:|:---:
*서버 입장* | ![ServerConnect](https://github.com/Q-holi/UnitySocket/blob/master/GameClient/IMG/connectServer.gif)|서버 실행과 입장하는 부분은<br> 큰 어려움은 없었습니다.



References  
Port : https://en.wikipedia.org/wiki/List_of_TCP_and_UDP_port_numbers 여유 포트 번호 확인  
URL : https://www.youtube.com/channel/UCYbK_tjZ2OrIZFBvU6CCMiA  
URL : https://www.youtube.com/channel/UCjCpZyil4D8TBb5nVTMMaUw  
URL : https://www.youtube.com/channel/UCa-mDKzV5MW_BXjSDRqqHUw 이쪽 영상을 대부분 참고 하였습니다.  
많은 자료들이 Unity자체적인 기반을 사용하였으나 Socket 통신을 사용하는 자료중에서도 정리가 잘 되어있었습니다.


