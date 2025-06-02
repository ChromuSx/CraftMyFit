namespace CraftMyFit.Helpers.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Verifica se la lista è null o vuota
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IList<T>? list) => list == null || list.Count == 0;

        /// <summary>
        /// Verifica se la lista ha elementi
        /// </summary>
        public static bool HasItems<T>(this IList<T>? list) => list != null && list.Count > 0;

        /// <summary>
        /// Aggiunge più elementi alla lista
        /// </summary>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if(list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if(items == null)
            {
                return;
            }

            foreach(T? item in items)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Rimuove tutti gli elementi che soddisfano la condizione
        /// </summary>
        public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            if(list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if(match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            int count = 0;
            for(int i = list.Count - 1; i >= 0; i--)
            {
                if(match(list[i]))
                {
                    list.RemoveAt(i);
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Mescola casualmente gli elementi della lista
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            if(list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            Random random = new();
            int n = list.Count;
            while(n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// Restituisce una copia della lista mescolata casualmente
        /// </summary>
        public static List<T> Shuffled<T>(this IEnumerable<T> source)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            List<T> list = source.ToList();
            list.Shuffle();
            return list;
        }

        /// <summary>
        /// Divide la lista in chunks di dimensione specificata
        /// </summary>
        public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize));
            }

            List<T> list = source.ToList();
            for(int i = 0; i < list.Count; i += chunkSize)
            {
                yield return list.GetRange(i, Math.Min(chunkSize, list.Count - i));
            }
        }

        /// <summary>
        /// Trova l'elemento più comune nella lista
        /// </summary>
        public static T? MostCommon<T>(this IEnumerable<T> source) => source == null
                ? throw new ArgumentNullException(nameof(source))
                : (source
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key);

        /// <summary>
        /// Rimuove duplicati mantenendo l'ordine
        /// </summary>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            HashSet<TKey> seenKeys = new();
            foreach(T? element in source)
            {
                if(seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Trova l'indice dell'elemento massimo
        /// </summary>
        public static int IndexOfMax<T>(this IList<T> list) where T : IComparable<T>
        {
            if(list.IsNullOrEmpty())
            {
                return -1;
            }

            T maxValue = list[0];
            int maxIndex = 0;

            for(int i = 1; i < list.Count; i++)
            {
                if(list[i].CompareTo(maxValue) > 0)
                {
                    maxValue = list[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        /// <summary>
        /// Trova l'indice dell'elemento minimo
        /// </summary>
        public static int IndexOfMin<T>(this IList<T> list) where T : IComparable<T>
        {
            if(list.IsNullOrEmpty())
            {
                return -1;
            }

            T minValue = list[0];
            int minIndex = 0;

            for(int i = 1; i < list.Count; i++)
            {
                if(list[i].CompareTo(minValue) < 0)
                {
                    minValue = list[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        /// <summary>
        /// Sostituisce un elemento se esiste, altrimenti lo aggiunge
        /// </summary>
        public static void ReplaceOrAdd<T>(this IList<T> list, T item, Predicate<T> match)
        {
            if(list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if(match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            int index = list.ToList().FindIndex(match);
            if(index >= 0)
            {
                list[index] = item;
            }
            else
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Restituisce un elemento casuale dalla lista
        /// </summary>
        public static T? Random<T>(this IList<T> list)
        {
            if(list.IsNullOrEmpty())
            {
                return default;
            }

            Random random = new();
            return list[random.Next(list.Count)];
        }

        /// <summary>
        /// Restituisce n elementi casuali dalla lista
        /// </summary>
        public static IEnumerable<T> Random<T>(this IList<T> list, int count)
        {
            if(list.IsNullOrEmpty())
            {
                yield break;
            }

            if(count <= 0)
            {
                yield break;
            }

            List<T> shuffled = list.Shuffled();
            int takeCount = Math.Min(count, shuffled.Count);

            for(int i = 0; i < takeCount; i++)
            {
                yield return shuffled[i];
            }
        }

        /// <summary>
        /// Controlla se due liste hanno gli stessi elementi (ignorando l'ordine)
        /// </summary>
        public static bool HasSameElements<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if(first == null && second == null)
            {
                return true;
            }

            if(first == null || second == null)
            {
                return false;
            }

            List<T> firstList = first.ToList();
            List<T> secondList = second.ToList();

            return firstList.Count == secondList.Count && firstList.All(secondList.Contains) && secondList.All(firstList.Contains);
        }

        /// <summary>
        /// Sposta un elemento in una nuova posizione
        /// </summary>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            if(list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if(oldIndex < 0 || oldIndex >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(oldIndex));
            }

            if(newIndex < 0 || newIndex >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(newIndex));
            }

            if(oldIndex == newIndex)
            {
                return;
            }

            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// Converte la lista in una stringa separata da virgole
        /// </summary>
        public static string ToDelimitedString<T>(this IEnumerable<T> source, string delimiter = ", ") => source == null ? string.Empty : string.Join(delimiter, source);

        /// <summary>
        /// Controlla se la lista è ordinata
        /// </summary>
        public static bool IsSorted<T>(this IList<T> list) where T : IComparable<T>
        {
            if(list.IsNullOrEmpty() || list.Count == 1)
            {
                return true;
            }

            for(int i = 1; i < list.Count; i++)
            {
                if(list[i].CompareTo(list[i - 1]) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Restituisce tutti gli elementi tranne l'ultimo
        /// </summary>
        public static IEnumerable<T> ExceptLast<T>(this IEnumerable<T> source, int count = 1)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            List<T> list = source.ToList();
            return list.Take(Math.Max(0, list.Count - count));
        }

        /// <summary>
        /// Restituisce gli ultimi n elementi
        /// </summary>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            List<T> list = source.ToList();
            return list.Skip(Math.Max(0, list.Count - count));
        }
    }
}