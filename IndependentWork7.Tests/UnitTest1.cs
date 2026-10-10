using System;
using System.Collections.Generic;
using IndependentWork7;
using Xunit;

namespace IndependentWork7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldReturnCorrectValue()
        {
            var cache = new Cache<int, string>(3);

            cache.Add(1, "One");

            Assert.Equal("One", cache.Get(1));
        }

        [Fact]
        public void Count_ShouldReflectNumberOfElements()
        {
            var cache = new Cache<string, int>(5);

            cache.Add("A", 10);
            cache.Add("B", 20);

            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
        {
            var cache = new Cache<string, string>(3);
            cache.Add("user", "Ivan");

            Assert.True(cache.ContainsKey("user"));
            Assert.False(cache.ContainsKey("guest"));
        }

        [Fact]
        public void Add_ExceedingCapacity_ShouldEvictOldestElement_FIFO()
        {
            var cache = new Cache<string, string>(2);

            cache.Add("first", "Value1");
            cache.Add("second", "Value2");
            cache.Add("third", "Value3"); // "first" має бути видалено

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey("first"));
            Assert.True(cache.ContainsKey("second"));
            Assert.True(cache.ContainsKey("third"));
        }

        [Fact]
        public void Get_NonExistingKey_ShouldThrowKeyNotFoundException()
        {
            var cache = new Cache<string, string>(3);

            Assert.Throws<KeyNotFoundException>(() => cache.Get("missing_key"));
        }

        [Fact]
        public void Constructor_InvalidCapacity_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Cache<string, string>(0));
        }
    }
}