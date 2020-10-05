using System;
namespace HashTable
{
    /// <summary>
    /// Hash table implemented using a circular array with chaining
    /// to resolve collisions.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class HashTable<TKey, TValue>
    {

        /// <summary>
        /// The circular array.
        /// </summary>
        private Entry[] m_Table = new Entry[8];

        /// <summary>
        /// The number of occupied entries in the array.
        /// </summary>
        private int m_OccupiedCount;

        /// <summary>
        /// Gets the number of entries in the table.
        /// </summary>
        public int Count => m_OccupiedCount;

        /// <summary>
        /// Adds the <see cref="key"/>, <see cref="value"/> pair to the table.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            if(0.87 < (m_OccupiedCount / (double)m_Table.Length))
            {
                Expand();
            }
            var tableLen = m_Table.Length;
            var indexGuess = GuessIndex(key, tableLen);
            for (var i = 0; i < tableLen; i++)
            {
                var index = (indexGuess + i) % tableLen;
                if (m_Table[index].Occupied)
                {
                    if (key.Equals(m_Table[index].Key))
                    {
                        throw new Exception("Duplicate Keys!");
                    }
                    continue;
                }
                m_Table[index].Occupied = true;
                m_Table[index].Hash = indexGuess;
                m_Table[index].Key = key;
                m_Table[index].Value = value;
                m_OccupiedCount++;
                return;
            }

        }
        /// <summary>
        /// Determines whether or not the table contains <see cref="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the table contains <see cref="key"/>,
        /// false otherwise.</returns>
        public bool ContainsKey(TKey key)
        {
            return 0 <= FindKeyIndex(key, true);
        }

        /// <summary>
        /// Gets the value associated with <see cref="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="Exception">Thrown if <see cref="key"/>
        /// cannot be found.</exception>
        public TValue GetValue(TKey key)
        {
            return m_Table[FindKeyIndex(key, false)].Value;
        }

        /// <summary>
        /// Removes <see cref="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="Exception">Thrown when the key is
        /// not found.</exception>
        public void Remove(TKey key)
        {
            var indexKey = FindKeyIndex(key, false);
            ShiftBackEntries(indexKey);
            m_OccupiedCount--;
        }

        /// <summary>
        /// Gets the index to the array location that <see cref="key"/> is
        /// located at if its location has not been impacted by collisions.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="tableLen">The length of the circular array.</param>
        /// <returns>The first guess index.</returns>
        private int GuessIndex(TKey key, int tableLen)
        {
            return Math.Abs(key.GetHashCode()) % tableLen;
        }

        /// <summary>
        /// Doubles the capacity of the circular array and re-adds all entries.
        /// This is a costly operation.
        /// </summary>
        private void Expand()
        {
            var tableOld = m_Table;
            var tableLenOld = m_Table.Length;
            m_OccupiedCount = 0;
            m_Table = new Entry[tableLenOld * 2];
            for (var i = 0; i < tableLenOld; i++)
            {
                if (!tableOld[i].Occupied)
                {
                    continue;
                }
                Add(tableOld[i].Key, tableOld[i].Value);
            }
        }

        /// <summary>
        /// Finds the index of <see cref="key"/> in the circular array. Accounts
        /// for collisions by searching to the right from the index returned by
        /// <see cref="GuessIndex(TKey, int)"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="safe">True to suppress key not found exceptions.</param>
        /// <returns>The index, or -1 if the key cannot be found and
        /// <see cref="safe"/> is true.</returns>
        /// <exception cref="Exception">Thrown when <see cref="safe"/> is False
        /// and the key is not found.</exception>
        private int FindKeyIndex(TKey key, bool safe)
        {
            var tableLen = m_Table.Length;
            var indexGuess = GuessIndex(key, tableLen);
            for (var i = 0; i < tableLen; i++)
            {
                var index = (indexGuess + i) % tableLen;
                if (!m_Table[index].Occupied)
                {
                    if (safe) return -1;
                    throw new Exception("Key Not Found!");
                }
                if (key.Equals(m_Table[index].Key))
                {
                    return index;
                }
            }

            if (safe) return -1;
            throw new Exception("Key Not Found!");
        }

        /// <summary>
        /// Call immediately after the entry at <see cref="indexKey"/> has been
        /// removed to shift backward collided entries in the circular array.
        /// </summary>
        /// <param name="indexKey">The index of entry that has been removed.
        /// </param>
        private void ShiftBackEntries(int indexKey)
        {
            int tableLen = m_Table.Length;
            var indexSwap = indexKey;
            for (var i = 1; i < tableLen; i++)
            {
                var index = (indexKey + i) % tableLen;
                if (!m_Table[index].Occupied)
                {
                   break;
                }
                var hash = m_Table[index].Hash;
                if (indexSwap < index)
                {
                    if (hash <= indexSwap || index < hash)
                    {
                        m_Table[indexSwap] = m_Table[index];
                        indexSwap = index;
                    }
                    continue;
                }
                // index < indexSwap:
                if (hash <= indexSwap && index < hash)
                {
                    m_Table[indexSwap] = m_Table[index];
                    indexSwap = index;
                }

            }
            m_Table[indexSwap] = new Entry();
        }

        /// <summary>
        /// An entry in the circular array.
        /// </summary>
        private struct Entry
        {
            /// <summary>
            /// True if the entry is occupied with a key, false otherwise.
            /// </summary>
            public bool Occupied;
            /// <summary>
            /// The hash of the key, if the <see cref="Occupied"/> is true.
            /// </summary>
            public int Hash;
            /// <summary>
            /// The key, if <see cref="Occupied"/> is true.
            /// </summary>
            public TKey Key;
            /// <summary>
            /// The value, if <see cref="Occupied"/> is true.
            /// </summary>
            public TValue Value;
        }
    }
}
