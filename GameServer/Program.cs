using System;
using System.Threading;
namespace GameServer
{
    class Program
    {
        private static bool isRunning = false;
        static void Main(string[] args)
        {
            Console.Title = "Game Server";//콘솔 창 이름을 게임 서버로 바꿈
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));//mainThread 선언
            mainThread.Start();//Thread running

            Server.Start(50, 26950);//서버를 시작한다 (최대 인원수, 서버를 사용할 포드 번호)
        }

        private static void MainThread()//서버의 CPU의 사용량을 줄인다. 
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while(_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if(_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
