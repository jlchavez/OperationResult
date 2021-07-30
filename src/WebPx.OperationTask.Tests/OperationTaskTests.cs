using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebPx.OperationScope;

namespace WebPx.Tests
{
    class OperationTaskTests
    {
        [SetUp]
        public void SetUp()
        {
            SayHello("SetUp");
            OperationScope.Push();
        }

        [TearDown]
        public void TearDown()
        {
            OperationScope.Pop();
            SayHello("TearDown");
        }

        private void SayHello(string? specifics = null)
        {
            Console.WriteLine($"[{OperationScope.Current.Id}] {specifics}");
        }

        private async OperationTask AsyncStep3()
        {
            SayHello($"{nameof(AsyncStep3)} Pushing new scope");
            using var scope = OperationScope.Push();
            SayHello($"{nameof(AsyncStep3)} Start");
            await Task.Delay(10);
            SayHello("AsyncStep1 End");
        }

        [Test]
        public async Task AsyncFlow3()
        {
            var scopeId = OperationScope.Current.Id;
            SayHello("Calling async method");
            await AsyncStep3();
            SayHello("Back from async method");
            Assert.AreEqual(scopeId, OperationScope.Current.Id);
            Console.WriteLine("Success");
        }

        private async OperationTask AsyncStep4()
        {
            using var scope = OperationScope.Push();
            SayHello($"{nameof(AsyncStep4)} Start");
            await Task.Delay(10);
            SayHello("AsyncStep1 End");
        }

        [Test]
        public async Task AsyncFlow4()
        {
            var scopeId = OperationScope.Current.Id;
            SayHello("Calling async method");
            await AsyncStep4();
            SayHello("Back from async method");
            Assert.AreEqual(scopeId, OperationScope.Current.Id);
            Console.WriteLine("Success");
        }
    }
}
