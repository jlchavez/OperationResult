using System;
using System.Runtime.CompilerServices;

namespace WebPx
{
    public class OperationTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly OperationTask _value;
        internal OperationTaskAwaiter(OperationTask value) { _value = value; }
        public bool IsCompleted => _value.IsCompleted;
        public void GetResult() { _value._task?.GetAwaiter().GetResult(); }
        public void OnCompleted(Action continuation) => _value.AsTask().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().OnCompleted(continuation);
        public void UnsafeOnCompleted(Action continuation) => _value.AsTask().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().UnsafeOnCompleted(continuation);
    }

    public class OperationTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly OperationTask<TResult> _value;
        internal OperationTaskAwaiter(OperationTask<TResult> value) => _value = value;
        public bool IsCompleted => _value.IsCompleted;
        public OperationResult<TResult>? GetResult() => (_value._task == null) ? _value._result : _value._task.GetAwaiter().GetResult();
        public void OnCompleted(Action continuation) => _value.AsTask().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().OnCompleted(continuation);
        public void UnsafeOnCompleted(Action continuation) => _value.AsTask().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().UnsafeOnCompleted(continuation);
    }

}