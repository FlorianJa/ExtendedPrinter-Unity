using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Assets._ExtendedPrinter.Scripts.Helper
{
    public static class ExtensionMethods
    {
        public static TaskAwaiter GetAwaiter(this UnityEngine.AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}
