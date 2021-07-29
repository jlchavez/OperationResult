using System;
using System.Runtime.CompilerServices;

namespace WebPx
{
    public struct OperationTaskMethodBuilder<TResult>
    {
        // This builder contains *either* an AsyncTaskMethodBuilder, *or* a result.
        // At the moment someone retrieves its Task, that's when we collapse to the real AsyncTaskMethodBuilder
        // and it's task, or just use the result.
        internal AsyncTaskMethodBuilder<OperationResult<TResult?>> _taskBuilder;
        internal bool GotBuilder;
        internal bool GotResult;
        internal OperationResult<TResult?>? _result;

        public static OperationTaskMethodBuilder<TResult> Create() => new();

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            OperationScope.Push();
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            EnsureTaskBuilder();
            _taskBuilder.SetStateMachine(stateMachine);
        }

        public void SetResult(TResult result)
        {
            OperationScope.Pop();
            OperationResult<TResult?> operationResult = OperationResult<TResult?>.FromResult(result);
            if (GotBuilder)
                _taskBuilder.SetResult(operationResult);
            else
                _result = operationResult;
            GotResult = true;
        }

        public void SetException(System.Exception exception)
        {
            OperationScope.Pop();
            EnsureTaskBuilder();
            _taskBuilder.SetException(exception);
        }

        private void EnsureTaskBuilder()
        {
            if (!GotBuilder && GotResult)
                throw new InvalidOperationException();
            if (!GotBuilder)
                _taskBuilder = new AsyncTaskMethodBuilder<OperationResult<TResult?>>();
            GotBuilder = true;
        }

        public OperationTask<TResult?> Task
        {
            get
            {
                if (GotResult && !GotBuilder)
                    return new OperationTask<TResult?>(_result);
                EnsureTaskBuilder();
                return new OperationTask<TResult?>(_taskBuilder.Task);
            }
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            EnsureTaskBuilder();
            _taskBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            EnsureTaskBuilder();
            _taskBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }
    }

}