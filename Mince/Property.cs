using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    public class Property : Variable
    {
        public MinceUserFunction getFunc;
        public MinceUserFunction setFunc;

        private Func<MinceObject> _getFunc;
        private Action<MinceObject> _setFunc;

        public void Init()
        {
            _getFunc = () => this.getFunc.call(new MinceObject[0]);
            _setFunc = v => this.setFunc.call(new MinceObject[1] { v });
        }

        public override MinceObject GetValue()
        {
            return _getFunc.Invoke();
        }

        public override void SetValue(MinceObject assignment)
        {
            _setFunc.Invoke(assignment);
        }
    }
}
