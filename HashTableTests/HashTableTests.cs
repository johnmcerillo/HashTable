using System;
using System.Collections.Generic;
using HashTable;
using Xunit;

namespace HashTableTests
{
    public class HashTableTests
    {

        /// <summary>
        /// Test collisions without expanding the circular array.
        /// </summary>
        [Fact]
        public void Test1()
        {
            var ht = new HashTable<int, int>();
            ht.Add(4, -4);
            ht.Add(6, -6);
            ht.Add(12, -12);
            ht.Add(20, -20);
            ht.Add(14, -14);
            ht.Add(13, -13);
            ht.Add(0, 0);
            ht.Remove(12);
            bool noExcept = false;
            try
            {
                var fail = ht.GetValue(12);
                noExcept = true;
            }
            catch
            {
                //pass
            }
            Assert.False(noExcept || ht.ContainsKey(12) || ht.GetValue(0) != 0 || ht.GetValue(13) != -13 || ht.GetValue(14) != -14
                || ht.GetValue(20) != -20 || ht.GetValue(6) != -6 || ht.GetValue(4) != -4);
        }
        /// <summary>
        /// Test more collisions without expanding the circular array.
        /// </summary>
        [Fact]
        public void Test2()
        {
            var ht = new HashTable<int, int>();
            ht.Add(4, -4);
            ht.Add(6, -6);
            ht.Add(12, -12);
            ht.Add(20, -20);
            ht.Add(0, 0);
            ht.Add(8, -8);
            ht.Remove(12);
            bool noExcept = false;
            try
            {
                var fail = ht.GetValue(12);
                noExcept = true;
            }
            catch
            {

            }
            Assert.False(noExcept || ht.ContainsKey(12) || ht.GetValue(8) != -8 || ht.GetValue(0) != 0 || ht.GetValue(20) != -20
                || ht.GetValue(6) != -6 || ht.GetValue(4) != -4);
        }

        /// <summary>
        /// Randomized test on the circular array without expansion.
        /// </summary>
        [Fact]
        public void Test3()
        {
            var ht = new HashTable<int, int>();
            var rand = new Random(7);
            var keys = new int[7];
            var swapIndices = new int[3];
            for (var i = 0; i < 7; i++)
            {
                var key = GetUnique(rand, keys);
                keys[i] = key;
                ht.Add(key, -key);
            }
            for (var i = 0; i < 1000; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var swapIndex = GetUnique(rand, swapIndices, true);
                    swapIndices[j] = swapIndex;
                    ht.Remove(keys[swapIndex]);
                }
                for (var j = 0; j < 3; j++)
                {
                    var key = GetUnique(rand, keys);
                    keys[swapIndices[j]] = key;
                    ht.Add(key, -key);
                }
                for (var j = 0; j < 7; j++)
                {
                    var key = keys[j];
                    var notKey = GetUnique(rand, keys);
                    Assert.False(ht.ContainsKey(notKey) || ht.GetValue(key) != -key);
                }
            }
        }

        private int GetUnique(Random rand, int[] list, bool bounded = false)
        {
            while (true)
            {
                var key = bounded ? rand.Next(0, 7) : ((rand.Next() % 2 == 0) ? rand.Next() : -rand.Next());
                var unique = true;
                for (var i = 0; i < list.Length; i++)
                {
                    if (list[i] == key)
                    {
                        unique = false;
                        break;
                    }
                }
                if (unique)
                {
                    return key;
                }
            }
        }

        /// <summary>
        /// Randomized test that expands the circular array. Repeatedly adds a
        /// batches of 717 new entries and removes every fifth entry. Uses a
        /// C# built in hash set for reference.
        /// </summary>
        [Fact]
        public void Test4()
        {
            var hs = new HashSet<int>();
            var ht = new HashTable<int, int>();
            var rand = new Random(7);

            FillTo717(rand, hs, ht);

            for(var unused = 0; unused < 10000; unused++)
            {
                DrainEvery5thEntry(hs, ht);
                FillTo717(rand, hs, ht);
            }
        }

        private int FullSpectrumRand(Random rand)
        {
            return rand.Next() % 2 == 0 ? rand.Next() : -rand.Next();
        }

        private void FillTo717(Random rand, HashSet<int> hs, HashTable<int, int> ht)
        {
            while (hs.Count < 717)
            {
                int next;
                if (!hs.Contains(next = FullSpectrumRand(rand)))
                {
                    hs.Add(next);
                    ht.Add(next, -next);
                }
            }
            TestAllValues(hs, ht);
        }

        private void DrainEvery5thEntry(HashSet<int> hs, HashTable<int, int> ht)
        {
            var i = 0;
            var hsArr = new int[hs.Count];
            hs.CopyTo(hsArr);
            foreach (var entry in hsArr)
            {
                if (i % 5 == 0)
                {
                    hs.Remove(entry);
                    ht.Remove(entry);
                    Assert.True(!ht.ContainsKey(entry));
                    Assert.True(hs.Count == ht.Count);
                }
                i++;
            }
        }

        private void TestAllValues(HashSet<int> hs, HashTable<int, int> ht)
        {
            foreach (var entry in hs)
            {
                Assert.True(ht.GetValue(entry) == -entry);
            }
            Assert.True(hs.Count == ht.Count);
        }
    }
}
