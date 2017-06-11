using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pastr.Extensions
{
    public static class Extensions
    {
        public static async Task Wait(this CancellationTokenSource target, int millisecondsTimeout = 5000)
        {
            try
            {
                await Task.Delay(millisecondsTimeout, target.Token);
            }
            catch (TaskCanceledException) { }
        }
    }
}
