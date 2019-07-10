﻿using System;
namespace HashTable
{
    public class HashTable<TKey, TValue>
    {

        private Entry[] m_Table = new Entry[8];
        private int m_OccupiedCount;

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

        public bool ContainsKey(TKey key)
        {
            return 0 <= FindKeyIndex(key, true);
        }

        public TValue GetValue(TKey key)
        {
            return m_Table[FindKeyIndex(key, false)].Value;
        }

        public void Remove(TKey key)
        {
            var indexKey = FindKeyIndex(key, false);
            ShiftBackEntries(indexKey);
            m_OccupiedCount--;
        }

        private int GuessIndex(TKey key, int tableLen)
        {
            return Math.Abs(key.GetHashCode()) % tableLen;
        }

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

        private struct Entry
        {
            public bool Occupied;
            public int Hash;
            public TKey Key;
            public TValue Value;
        }
    }
}
