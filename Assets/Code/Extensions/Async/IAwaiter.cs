using System.Runtime.CompilerServices;

namespace Extensions.Async
{
  public interface IAwaiter : INotifyCompletion
  {
    bool IsCompleted { get; }
    void GetResult();
    IAwaiter GetAwaiter();
  }
}