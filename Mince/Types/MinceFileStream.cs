using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    [Instantiatable("File")]
    public class MinceFileStream : MinceObject
    {
        [Exposed]
        public MinceNumber streamPosition
        {
            get { return new MinceNumber(stream.Position); }
            set { stream.Position = value.ToInt(); }
        }

        [Exposed]
        public MinceNumber byteLength
        {
            get { return new MinceNumber(stream.Length); }
        }

        public FileStream stream;
        public StreamWriter writer;
        public StreamReader reader;

        public MinceFileStream() { }

        public MinceFileStream(MinceString path)
        {
            stream = new FileStream(path.ToString(), FileMode.OpenOrCreate);
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            CreateMembers();
        }

        [Exposed]
        public MinceString readLine()
        {
            return new MinceString(reader.ReadLine());
        }

        [Exposed]
        public MinceString readAllText()
        {
            return new MinceString(reader.ReadToEnd());
        }

        [Exposed]
        public MinceNull writeString(MinceObject obj)
        {
            writer.Write(obj.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull writeLine(MinceObject obj)
        {
            writer.WriteLine(obj.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceByte readByte()
        {
            return new MinceByte((byte)stream.ReadByte());
        }

        [Exposed]
        public MinceArray readBytes(MinceNumber amount)
        {
            MinceArray a = new MinceArray();
            for (int i = 0; i < amount.ToInt(); i++)
            {
                a.add(readByte());
            }
            return a;
        }

        [Exposed]
        public MinceArray readAllBytes()
        {
            return readBytes(new MinceNumber(stream.Length));
        }

        [Exposed]
        public MinceNull writeBytes(MinceArray array)
        {
            List<byte> bytes = new List<byte>();

            foreach (MinceObject b in array.GetItems())
            {
                if (b.GetType() == typeof(MinceByte))
                {
                    bytes.Add(((MinceByte)b).ToByte());
                }
                else if (b.GetType() == typeof(MinceNumber))
                {
                    bytes.Add(((MinceNumber)b).toByte().ToByte());
                }
            }

            stream.Write(bytes.ToArray(), 0, bytes.Count);
            return new MinceNull();
        }

        [Exposed]
        public MinceNull writeByte(MinceByte b)
        {
            stream.WriteByte(b.ToByte());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull dispose()
        {
            writer.Dispose();
            reader.Dispose();
            stream.Dispose();
            return new MinceNull();
        }

        [Exposed]
        public MinceNull goToEnd()
        {
            stream.Position = stream.Length;
            return new MinceNull();
        }
    }
}
