
using System;
using System.IO;

namespace Mince.Types
{
    [StaticClass("directory")]
    public class MinceDirectory : MinceObject
    {
        public MinceDirectory()
        {
            CreateMembers();
        }

        [Exposed]
        public MinceBool exists(MinceString path)
        {
            return new MinceBool(Directory.Exists(path.value.ToString()));
        }

        [Exposed]
        public MinceArray getDirectories(MinceString path)
        {
            MinceArray array = new MinceArray(new MinceObject[] { });
            foreach (string item in Directory.GetDirectories(path.value.ToString()))
            {
                array.add(new MinceString(item));
            }
            return array;
        }

        [Exposed]
        public MinceArray getFiles(MinceString path)
        {
            MinceArray array = new MinceArray(new MinceObject[] { });
            foreach (string item in Directory.GetFiles(path.value.ToString()))
            {
                array.add(new MinceString(item));
            }
            return array;
        }

        [Exposed]
        public MinceNull create(MinceString path)
        {
            Directory.CreateDirectory(path.value.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull move(MinceString path1, MinceString path2)
        {
            Directory.Move(path1.value.ToString(), path2.value.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull rename(MinceString path1, MinceString path2)
        {
            Directory.Move(path1.value.ToString(), path2.value.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull delete(MinceString path)
        {
            Directory.Delete(path.value.ToString());
            return new MinceNull();
        }
    }
}
