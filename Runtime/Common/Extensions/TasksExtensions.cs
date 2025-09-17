using System.Threading;

namespace Game.Extensions
{
public static class TasksExtensions
{
    public static void SafeCancelAndDispose(this CancellationTokenSource cts)
    {
        if (cts == null)
            return;

        if (cts.IsCancellationRequested == false)
            cts.Cancel();
            
        cts.Dispose();
    }
}
}