
using System;
using System.Collections.Generic;
using System.Threading;

namespace Mince.Types
{
    [Instantiatable("Thread")]
    public class MinceThread : MinceObject
    {
        private MinceUserFunction _func;

        [Exposed]
        public MinceUserFunction function { get { return _func; } set { _func = value; } }

        public MinceThread(MinceUserFunction func)
        {
            this.value = new Thread(new ThreadStart(doThread));
            this.function = func;

            CreateMembers();
        }

        public MinceThread()
        {
            this.value = new MinceNull();
            CreateMembers();
        }

        public void doThread()
        {
            function.call(new MinceObject[0]);
        }

        [Exposed]
        public MinceNull start()
        {
            GetValue().Start();
            return new MinceNull();
        }

        [Exposed]
        public MinceNull abort()
        {
            GetValue().Abort();
            return new MinceNull();
        }

        public Thread GetValue()
        {
            return this.value as Thread;
        }
    }
}
