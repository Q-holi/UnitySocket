using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    public enum ServerPackets
    {
        welcome = 1,
        spawnPlayer,
        playerPosition,
        playerRotation
    }//서버에서 클라이언드로 보냄

    public enum ClientPackets
    {
        welcomeReceived = 1,
        playerMovement
    }//클라이언트에서 서버로 보냄

    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        public Packet()
        {
            buffer = new List<byte>(); //생성 시 초기화
            readPos = 0;
        }//패킷 생성

        public Packet(int _id)
        {
            buffer = new List<byte>();
            readPos = 0;

            Write(_id);
        }//보낸 ID로 새로운 패킷 생성(int)

        public Packet(byte[] _data)
        {
            buffer = new List<byte>(); 
            readPos = 0; 

            SetBytes(_data);
        }//수신에 사용되는 데이터 패킷을 생성 (읽기)

        #region Functions

        public void SetBytes(byte[] _data)
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }//패킷 데이터를 설정하고 사용할 준비

        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
        }//버퍼 실행 시 패킷의 길이를 입력

        public void InsertInt(int _value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(_value)); 
        }//버퍼 실행 시 value(int) 입력

        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }//Array 형으로 패킷의 내용을 받기

        public int Length()
        {
            return buffer.Count;
        }//패킷의 길이 받기

        public int UnreadLength()
        {
            return Length() - readPos; 
        }//읽지 못한 패킷 내용의 길이를 추가로 받아온다.

        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); // 퍼버 초기화
                readableBuffer = null;
                readPos = 0; 
            }
            else
            {
                readPos -= 4;// 왜 -4인가 ?
            }
        }//패킷을 재설정
        #endregion

        #region Write Data

        public void Write(byte _value)
        {
            buffer.Add(_value);
        }//패킷에 _value(byte)를 추가한다. 

        public void Write(byte[] _value)
        {
            buffer.AddRange(_value);
        }//패킷에 _value(byte[])을 추가한다.

        public void Write(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }//패킷에 _value(short)을 추가한다.

        public void Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }//패킷에 _value(int)을 추가한다.

        public void Write(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }//패킷에 _value(long)을 추가한다.

        public void Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }//패킷에 _value(float)을 추가한다.

        public void Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }//패킷에 _value(bool)을 추가한다.

        public void Write(string _value)
        {
            Write(_value.Length); 
            buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // 인코딩을 걸쳐서 추가해주어야 한다.
        }//패킷에 _value(string)을 추가한다.
        public void Write(Vector3 _value)
        {
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
        }//Unity3D Vector3는 X,Y,Z 값 3가지가 들어간다.
        public void Write(Quaternion _value)
        {
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
            Write(_value.W);
        }
        #endregion

        #region Read Data
        //각 알맞는 데이터 형에 대하여 패킷 데이터를 읽어온다. 
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte _value = readableBuffer[readPos]; // readPos에서부터 데이터 가져오기(읽기)
                if (_moveReadPos)
                {
                    readPos += 1;//다음 데이터 읽을 위치설정(1증가)
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] _value = buffer.GetRange(readPos, _length).ToArray(); 
                if (_moveReadPos)
                {
                    readPos += _length; 
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        public short ReadShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short _value = BitConverter.ToInt16(readableBuffer, readPos); 
                if (_moveReadPos)
                {
   
                    readPos += 2;
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int _value = BitConverter.ToInt32(readableBuffer, readPos); 
                if (_moveReadPos)
                {

                    readPos += 4; 
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        public long ReadLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long _value = BitConverter.ToInt64(readableBuffer, readPos); 
                if (_moveReadPos)
                {
                    readPos += 8;
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        public float ReadFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float _value = BitConverter.ToSingle(readableBuffer, readPos); 
                if (_moveReadPos)
                {
                    readPos += 4; 
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos); 
                if (_moveReadPos)
                {

                    readPos += 1;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt(); 
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); 
                if (_moveReadPos && _value.Length > 0)
                {

                    readPos += _length; 
                }
                return _value; 
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public Vector3 ReadVector3(bool _moveReadPos = true)
        {
            return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }

        public Quaternion ReadQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)//주로 관리되지 않는 리소스를 해제하는 데 사용
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }//https://docs.microsoft.com/ko-kr/dotnet/standard/garbage-collection/implementing-dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}