using System;
using System.Linq;
using System.Threading;

namespace WebPx
{
    public class OperationScope : IDisposable
    {
        public static OperationScope Current { get => GetAsyncLocalCurrent(); }

        private static OperationScope GetAsyncLocalCurrent()
        {
            var wrapper = AsyncLocal.Value;
            if (wrapper != null)
                return wrapper.Scope!;
            wrapper = new Wrapper { Scope = new OperationScope() };
            AsyncLocal.Value = wrapper;
            return wrapper.Scope;
        }

        public static OperationScope Push()
        {
            var _current = Current;
            var instance = new OperationScope();
            var wrapper = new Wrapper { Scope = instance };
            if (_current != null)
            {
                instance.parent = _current;
                wrapper.Parent = AsyncLocal.Value;
            }
            AsyncLocal.Value = wrapper;
            return instance;
        }

        public static void Pop()
        {
            var wrapper = AsyncLocal.Value;
            if (wrapper == null)
            {
                Console.WriteLine("--- Pop NO Wrapper ---");
                return;
            }
            Console.WriteLine("--- Pop Wrapper ---");
            var current = AsyncLocal.Value;
            AsyncLocal.Value = wrapper.Parent!;
            Console.WriteLine($"On Pop OperationScope to {AsyncLocal.Value?.Scope?.Id}");
            if (current == null)
                return;
            if (current.Scope != null)
                current.Scope.parent = null;
            current.Parent = null;
        }

        private static readonly AsyncLocal<Wrapper> AsyncLocal = new();

        internal sealed class Wrapper : MarshalByRefObject
        {
            public Wrapper()
            {
            }

            private static int _id = 0;
            private int? __id = null;

            public int Id { get { return __id ??= _id++; } }

            internal Wrapper? Parent { get; set; }

            public OperationScope? Scope { get; set; }
        }

        private OperationScope? parent;

        internal OperationScope? Parent { get => parent; }

        private OperationScope()
        {
            Id = _id++;
        }

        private static int _id = 0;

        public int Id { get; }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Console.WriteLine($"OperationScope {Id} Disposed ");
                    OperationScope.Pop();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}