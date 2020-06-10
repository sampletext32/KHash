using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHash
{
    class Program
    {
        // метод для генерации хеш таблицы из n элементов
        static MyHashTable<long, string> GenerateHashTable(int n)
        {
            MyHashTable<long, string> myHashTable = new MyHashTable<long, string>();

            var stringHasher = new StringHasher();

            for (int i = 0; i < n; i++)
            {
                myHashTable.Set(stringHasher.Hash("test" + i), "KURGOOZZZ");
                // if (i % 100 == 0)
                // {
                //     Console.WriteLine(i);
                // }
            }

            return myHashTable;
        }

        static void Main(string[] args)
        {
            // генерация таблиц из 10^i элементов
            for (int i = 0; i < 5; i++)
            {
                var hashTable = GenerateHashTable((int) Math.Pow(10, i));
                Console.WriteLine(
                    "На {0} элементов: {1} проверок, {2} коллизий",
                    (int) Math.Pow(10, i),
                    hashTable.Checks,
                    hashTable.Collisions);
            }

            Console.ReadKey();
        }
    }

    // пара ключ-значение
    public class KeyValue<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }

    public class MyHashTable<K, V>
    {
        public int Checks { get; set; }
        public int Collisions { get; set; }
        private List<KeyValue<K, V>> _items;

        private int GetKeyPosition(K key)
        {
            int index = -1;
            for (var i = 0; i < _items.Count; i++)
            {
                if (_items[i].Key.Equals(key))
                {
                    index = i;
                }

                Checks++;
            }

            return index;
        }

        public V Get(K key)
        {
            int keyIndex = GetKeyPosition(key);
            if (keyIndex == -1) return default;
            return _items[keyIndex].Value;
        }

        public void Set(K key, V value)
        {
            int keyIndex = GetKeyPosition(key);
            if (keyIndex == -1)
            {
                KeyValue<K, V> item = new KeyValue<K, V> {Key = key, Value = value};
                _items.Add(item);
            }
            else
            {
                _items[keyIndex].Value = value;
                Collisions++;
            }

            Checks++;
        }

        public MyHashTable()
        {
            _items = new List<KeyValue<K, V>>();
        }

        public void PrintHashtable()
        {
            foreach (var keyValue in _items)
            {
                Console.WriteLine("{0}: {1}", keyValue.Key, keyValue.Value);
            }
        }
    }

    // универсальный класс хешера, получает K хеш из V значения
    public abstract class Hasher<K, V>
    {
        // инициатор хеша
        protected static long HashSource = 19;

        // соль
        protected static long Salt = 31;

        // алгоритм хеширования
        public abstract K Hash(V obj);
    }

    public class StringHasher : Hasher<long, string>
    {
        // Алгоритм хеширования строк в long
        public override long Hash(string obj)
        {
            if (obj.Length < 16)
            {
                // если строка меньше 16 символов, дополнить справа 0 до 16
                // работает только в тестовом варианте, т.к. нет строк длиннее 16 символов
                obj = obj.PadRight(16, 'a');
            }

            // инициализация хеша
            long res = HashSource;
            // вычисление хеша как сдвиг байт для каждого символа и сдвиг на 'соль'
            for (int i = 0; i < obj.Length; i++)
            {
                res += res * Salt + ((int) obj[i] | (int) obj[i] << 16);
            }

            return res;
        }
    }
}