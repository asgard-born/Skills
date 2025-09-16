using System;

namespace Extensions.Async
{
  internal interface IDisposableAwaiter : IAwaiter, IDisposable
  {
    
  }
}