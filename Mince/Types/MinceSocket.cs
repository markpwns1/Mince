
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mince.Types
{
    [Instantiatable("Socket")]
    public class MinceSocket : MinceObject
    {
        public MinceSocket()
        {
            this.value = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            CreateMembers();
        }

        public MinceSocket(Socket sock)
        {
            this.value = sock;

            CreateMembers();
        }

        [Exposed]
        public MinceNull bind(MinceString ipAddress, MinceNumber port)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ipAddress.ToString(), out ip))
            {
                throw new Exception("Invalid IP address! '" + ipAddress + "'");
            }

            GetValue().Bind(new IPEndPoint(ip, port.ToInt()));

            return new MinceNull();
        }

        [Exposed]
        public MinceNull listen()
        {
            GetValue().Listen(100);
            return new MinceNull();
        }

        [Exposed]
        public MinceSocket accept()
        {
            var s = GetValue().Accept();
            return new MinceSocket(s);
        }

        [Exposed]
        public MinceBool connect(MinceString ipAddress, MinceNumber port)
        {
            try
            {
                IPAddress ip;
                if (!IPAddress.TryParse(ipAddress.ToString(), out ip))
                {
                    throw new Exception("Invalid IP address! '" + ipAddress + "'");
                }

                GetValue().Connect(new IPEndPoint(ip, port.ToInt()));
            }
            catch
            {
                return new MinceBool(false);
            }

            return new MinceBool(true);
        }

        [Exposed]
        public MinceString receive()
        {
            byte[] buffer = new byte[GetValue().ReceiveBufferSize];
            int size = GetValue().Receive(buffer);

            byte[] formatted = new byte[size];

            for (int i = 0; i < size; i++)
            {
                formatted[i] = buffer[i];
            }

            string output = Encoding.ASCII.GetString(formatted);

            return new MinceString(output);
        }

        [Exposed]
        public MinceNull send(MinceString text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text.ToString());
            GetValue().Send(data);
            return new MinceNull();
        }

        [Exposed]
        public MinceBool isConnected()
        {
            bool part1 = GetValue().Poll(1000, SelectMode.SelectRead);
            bool part2 = GetValue().Available == 0;
            return new MinceBool(part1 && part2);
        }

        [Exposed]
        public MinceNull close()
        {
            GetValue().Close();
            return new MinceNull();
        }

        public Socket GetValue()
        {
            return this.value as Socket;
        }
    }
}
