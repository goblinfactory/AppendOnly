using System;
using NUnit.Framework;
using ApprovalTests;
using ApprovalTests.Reporters;
using AppendOnly;

namespace Lib.Tests
{
    [UseReporter(typeof(DiffReporter))]
    public class AppendOnlyListTests
    {
        [Test]
        public void EmptyList_test()
        {
            var list = new AppendOnlyList<Cat>();
            Approvals.VerifyJson(new[] { list }.ToJsons());
        }

        [Test]
        public void AddItem_should_returnAnObjectThatOnlyIteratesOverItsOwnChildren_and_doesNotMutatePreviousChildStates()
        {
            var list = new AppendOnlyList<Cat>();
            var list1 = list.AddItem(new Cat("frodo", 10));
            var list2 = list1.AddItem(new Cat("Gandalf", 25));
            var list3 = list2.AddItem(new Cat("Buttons", 13));
            var json = new[] { list, list1, list2, list3 }.ToJsons();
            Approvals.VerifyJson(json);
        }
    }

    internal class Cat
    {
        public Cat(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; }
        public int Age { get; }
    }
}
