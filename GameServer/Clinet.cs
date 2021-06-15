using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
    class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }//클라이언트 선언과 동시에 id와 TCP 통신과 UDP 통신 형태 2가지로 생성한다.
        public class UDP
        {
            public IPEndPoint endPoint;//IP번호와 Port번호

            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });
            }
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receiveData;
            private byte[] recieveBuffer;
            public TCP(int _id)
            {
                id = _id;
            }
            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receiveData = new Packet();
                recieveBuffer = new byte[dataBufferSize];

                stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the Server!");
            }
            public void SendData(Packet _packet)
            {
                try
                {
                    if(socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch(Exception _ex)
                {
                    Console.WriteLine($"Error sending data to player {id} TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if(_byteLength <= 0 )
                    {
                        return;
                    }
                    byte[] _data = new byte[_byteLength];
                    Array.Copy(recieveBuffer, _data, _byteLength);

                    receiveData.Reset(HandleData(_data));
                    stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch(Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data : {_ex}");
                }
            }
            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;
                receiveData.SetBytes(_data);
                if (receiveData.UnreadLength() >= 4)
                {
                    _packetLength = receiveData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
                while (_packetLength > 0 && _packetLength <= receiveData.UnreadLength())
                {
                    byte[] _packetBytes = receiveData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receiveData.UnreadLength() >= 4)
                    {
                        _packetLength = receiveData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }
                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public void SendIntoGame(string _playerName)
        {
            Random r = new Random();
            int randX = r.Next(30);// 플레이어 X좌표 0~30 까지 랜덤한 위치에 생성 해주기

            player = new Player(id, _playerName, new Vector3(randX, 0, 0));//플래이어 생성 

            foreach(Client _client in Server.clients.Values)
            {
                if(_client.player != null)
                {
                    if(_client.id != id)
                    {
                        ServerSend.SpawnPlayer(id, _client.player);
                    }
                }
            }

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerSend.SpawnPlayer(_client.id,player);
                }
            }

        }
    }
}
