using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pastr.Extensions
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
