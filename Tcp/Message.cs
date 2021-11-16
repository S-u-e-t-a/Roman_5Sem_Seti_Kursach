using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Tcp
{
    public class Message
    {
        private readonly bool _autoTrim;
        private readonly Encoding _encoder;
        private readonly byte _writeLineDelimiter;


        public byte[] Data { get; }

        public string MessageString
        {
            get
            {
                if (_autoTrim)
                {
                    return _encoder.GetString(Data).Trim();
                }

                return _encoder.GetString(Data);
            }
        }

        public TcpClient TcpClient { get; }


        internal Message(byte[] data, TcpClient tcpClient, Encoding stringEncoder, byte lineDelimiter)
        {
            Data = data;
            TcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
        }


        internal Message(byte[] data, TcpClient tcpClient, Encoding stringEncoder, byte lineDelimiter, bool autoTrim)
        {
            Data = data;
            TcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
            _autoTrim = autoTrim;
        }


        public void Reply(byte[] data)
        {
            TcpClient.GetStream().Write(data, 0, data.Length);
        }


        public void Reply(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            Reply(_encoder.GetBytes(data));
        }


        public void ReplyLine(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            if (data.LastOrDefault() != _writeLineDelimiter)
            {
                Reply(data + _encoder.GetString(new[] {_writeLineDelimiter}));
            }
            else
            {
                Reply(data);
            }
        }
    }
}