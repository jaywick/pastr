using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pastr
{
    public static class Extensions
    {
        public static void SafeInvoke(this Action target)
        {
            if (target != null)
                target.Invoke();
        }
    }
}
