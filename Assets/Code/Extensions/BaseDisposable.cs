using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions.Async;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extensions
{
    public abstract class BaseDisposable : IDisposable
    {
        private bool _isDisposed;
        private List<IDisposable> _mainThreadDisposables;
        private List<Object> _unityObjects;
        private HashSet<IDisposableAwaiter> _operations;
        private List<Task> _taskOperations;

        protected bool IsDisposed => _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            if (_operations != null)
            {
                foreach (IDisposableAwaiter operation in _operations)
                {
                    operation.Dispose();
                }

                _operations.Clear();
                _operations = null;
            }

            if (_mainThreadDisposables != null)
            {
                List<IDisposable> mainThreadDisposables = _mainThreadDisposables;

                for (int i = mainThreadDisposables.Count - 1; i >= 0; i--)
                {
                    mainThreadDisposables[i]?.Dispose();
                }

                mainThreadDisposables.Clear();
            }

            try
            {
                OnDispose();
            }
            catch (Exception e)
            {
                Debug.Log($"This exception can be ignored. Disposable of {GetType().Name}: {e}");
            }

            if (_unityObjects != null)
            {
                foreach (Object obj in _unityObjects)
                {
                    if (obj)
                    {
                        Object.Destroy(obj);
                    }
                }
            }
        }

        public TDisposable AddUnsafe<TDisposable>(TDisposable disposable) where TDisposable : IDisposable
        {
            if (_isDisposed)
            {
                Debug.Log("disposed");

                return default;
            }

            if (disposable == null)
            {
                return default;
            }

            if (_mainThreadDisposables == null)
            {
                _mainThreadDisposables = new List<IDisposable>(1);
            }

            _mainThreadDisposables.Add(disposable);

            return disposable;
        }

        protected virtual void OnDispose()
        {
        }
    }
}