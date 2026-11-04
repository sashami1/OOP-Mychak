using System;
using System.Collections.Generic;
using IndependentWork_7;
using Xunit;

namespace IndependentWork_7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldWorkCorrectly()
        {
            var cache = new Cache<int, string>(3);

            cache.Add(1, "One");
            cache.Add(2, "Two");

            Assert.Equal("One", cache.Get(1));
            Assert.Equal("Two", cache.Get(2));
        }

        [Fact]
        public void Count_ShouldReturnCorrectNumberOfItems()
        {
            var cache = new Cache<string, int>(5);

            cache.Add("a", 10);
            cache.Add("b", 20);

            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
        {
            var cache = new Cache<string, string>(3);
            cache.Add("test", "data");

            Assert.True(cache.ContainsKey("test"));
            Assert.False(cache.ContainsKey("missing"));
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            var cache = new Cache<string, string>(3);

            Assert.Throws<KeyNotFoundException>(() => cache.Get("unknown"));
        }

        [Fact]
        public void Add_ExceedingCapacity_ShouldRemoveOldestElement_FIFO()
        {
            var cache = new Cache<int, string>(2);

            cache.Add(1, "First");
            cache.Add(2, "Second");
            cache.Add(3, "Third");

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
            Assert.True(cache.ContainsKey(3));
        }

        [Fact]
        public void Add_ExistingKey_ShouldUpdateValueWithoutChangingCount()
        {
            var cache = new Cache<string, string>(2);

            cache.Add("key1", "Value1");
            cache.Add("key1", "UpdatedValue1");

            Assert.Equal(1, cache.Count);
            Assert.Equal("UpdatedValue1", cache.Get("key1"));
        }
    }
}