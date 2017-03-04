
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Mince.Types
{
    [StaticClass("file")]
    public class MinceStaticFile : MinceObject
    {
        public MinceStaticFile()
        {
            CreateMembers();
        }

        [Exposed]
        public MinceFileStream open(MinceString path)
        {
            if (File.Exists(path.ToString()))
            {
                return new MinceFileStream(path);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        [Exposed]
        public MinceBool exists(MinceString path)
        {
            return new MinceBool(File.Exists(path.value.ToString()));
        }

        [Exposed]
        public MinceString read(MinceString path)
        {
            return new MinceString(File.ReadAllText(path.value.ToString()));
        }

        [Exposed]
        public MinceArray readLines(MinceString path)
        {
            return new MinceArray(File.ReadAllLines(path.ToString()).Select(x => new MinceString(x)).ToArray());
        }

        [Exposed]
        public MinceNull write(MinceString path, MinceString contents)
        {
            File.WriteAllText(path.value.ToString(), contents.value.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull move(MinceString path1, MinceString path2)
        {
            File.Move(path1.value.ToString(), path2.value.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull run(MinceString path)
        {
            System.Diagnostics.Process.Start(path.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull copy(MinceString path1, MinceString path2)
        {
            File.Copy(path1.value.ToString(), path2.value.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull rename(MinceString path1, MinceString path2)
        {
            File.Move(path1.value.ToString(), path2.value.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull delete(MinceString path)
        {
            File.Delete(path.value.ToString());
            return new MinceNull();
        }
    }
}
