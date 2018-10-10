using System;
using Xunit;

namespace BitPoker.Engine.Tests
{
    public class CircularListTests
    {
        [Fact]
        public void Should_Add_New_Loop_List_Element()
        {
            CircularList<Int32> list = new CircularList<Int32>(true);
            list.Add(1);
        }
    }
}