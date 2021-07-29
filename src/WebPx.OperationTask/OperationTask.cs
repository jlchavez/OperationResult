using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WebPx
{
    [AsyncMethodBuilder(typeof(OperationTaskMethodBuilder))]
    public class OperationTask : IAsyncResult
    {
        internal OperationTask()
        {
            _isCompleted = true;
            _completedSynchronously = true;
            _task = null;
        }

        internal OperationTask(Task task)
        {
            _isCompleted = false;
            _completedSynchronously = false;
            _task = task;
        }

        internal readonly Task? _task;

        private readonly bool _isCompleted = false;
        private readonly bool _completedSynchronously = false;

        public bool IsCompleted => _isCompleted;

        public object AsyncState => null!;

        public WaitHandle AsyncWaitHandle => null!;

        public bool CompletedSynchronously => _completedSynchronously;

        public OperationTaskAwaiter GetAwaiter()
        {
            return new OperationTaskAwaiter(this);
        }

        public Task AsTask() => _task ?? Task.CompletedTask;
    }

    [AsyncMethodBuilder(typeof(OperationTaskMethodBuilder<>))]
    public class OperationTask<TResult> : IAsyncResult
    {
        internal OperationTask(OperationResult<TResult>? result)
        {
            _isCompleted = true;
            _completedSynchronously = true;
            _result = result;
            _task = null;
        }

        internal readonly OperationResult<TResult>? _result;

        internal OperationTask(Task<OperationResult<TResult>> task)
        {
            _isCompleted = false;
            _completedSynchronously = false;
            _task = task;
            _result = null;
        }

        internal readonly Task<OperationResult<TResult>>? _task;

        private readonly bool _isCompleted;
        private readonly bool _completedSynchronously;

        public bool IsCompleted => _isCompleted;

        public object AsyncState => null!;

        public WaitHandle AsyncWaitHandle => null!;

        public bool CompletedSynchronously => _completedSynchronously;

        public OperationTaskAwaiter<TResult> GetAwaiter()
        {
            return new OperationTaskAwaiter<TResult>(this);
        }

        public Task AsTask() => _task ?? Task.CompletedTask;

        public static implicit operator OperationTask<TResult>(TResult value) => new(value);
    }
}