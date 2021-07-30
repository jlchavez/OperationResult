using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using static WebPx.OperationScope;

namespace WebPx.Tests
{
    public class TaskTests
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

        private async Task AsyncStep1()
        {
            using var scope = OperationScope.Push();
            SayHello("AsyncStep1 Start");
            await Task.Delay(10);
            SayHello("AsyncStep1 End");
        }

        [Test]
        public async Task AsyncFlow1()
        {
            var scopeId = OperationScope.Current.Id;
            SayHello("Calling async method");
            using (var scope = OperationScope.Push())
            {
                SayHello("Emulating another scope");
                await AsyncStep1();
            }
            SayHello("Back from async method");
            Assert.AreEqual(scopeId, OperationScope.Current.Id);
            Console.WriteLine("Success");
        }

        private async Task AsyncStep2()
        {
            using var scope = OperationScope.Push();
            SayHello($"{nameof(AsyncStep2)} Start");
            await Task.Delay(10);
            SayHello("AsyncStep1 End");
        }

        [Test]
        public async Task AsyncFlow2()
        {
            var scopeId = OperationScope.Current.Id;
            SayHello("Calling async method");
            await AsyncStep2();
            SayHello("Back from async method");
            Assert.AreEqual(scopeId, OperationScope.Current.Id);
            Console.WriteLine("Success");
        }
    }
}