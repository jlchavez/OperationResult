using System;
using System.Text.Json.Serialization;

namespace WebPx
{
    [Serializable]
    public class OperationResult<TResult> : OperationResult
    {
        public OperationResult(TResult item)
        {
            Value = item;
        }

        public TResult Value { get; set; }

        public static implicit operator OperationResult<TResult>(TResult value) => FromResult(value);

        public static implicit operator TResult?(OperationResult<TResult>? result) => result == null ? default : result.Value;

        public static OperationResult<TResult> FromResult(TResult value)
        {
            var result = new OperationResult<TResult>(value);
            return result;
        }
    }
}