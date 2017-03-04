using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    public class LoopPosition
    {
        private static int globalID = 0;

        public int id;
        public int pointer;
        public int depth;
        public int index = -1;

        public LoopPosition(int pos, int depth)
        {
            this.id = globalID;
            this.pointer = pos;
            this.depth = depth;
            globalID++;
        }
    }
}
