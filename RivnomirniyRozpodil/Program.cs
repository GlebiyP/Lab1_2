using System;
using System.Diagnostics;

static class RandomExtensions
{
    // Метод розширення для генерації випадкових чисел з нормальним розподілом
    public static double NextGaussian(this Random random, double mean, double stdDev)
    {
        double u1 = 1.0 - random.NextDouble(); // Забезпечуємо ненульову ймовірність
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Генеруємо масив з рівномірним розподілом
        //int[] array = GenerateUniformArray(10000, minValue: 0, maxValue: 10000);

        // Генеруємо масив з нормальним розподілом
        int[] array = GenerateNormalArray(100000, mean: 5000, stdDev: 1000);

        // Запускаємо алгоритм сортування методом бульбашки 100 разів
        long comparisonsSum = 0;
        long swapsSum = 0;
        double timeSum = 0;
        double memorySum = 0;

        for (int i = 0; i < 100; i++)
        {
            int[] copyArray = (int[])array.Clone(); // Клонуємо масив для кожного запуску

            // Виконуємо сортування методом бульбашки та вимірюємо час виконання
            long startMemory = GC.GetTotalMemory(true);
            Stopwatch stopWatch = Stopwatch.StartNew();
            QuickSort(copyArray, out long comparisons, out long swaps);
            stopWatch.Stop();

            long endMemory = GC.GetTotalMemory(true);

            // Додаємо значення порівнянь, свапів та часу виконання до сумарних значень
            comparisonsSum += comparisons;
            swapsSum += swaps;
            timeSum += stopWatch.Elapsed.TotalMilliseconds;
            memorySum += endMemory - startMemory;
        }

        // Обчислюємо середні значення
        double avgComparisons = comparisonsSum / 100.0;
        double avgSwaps = swapsSum / 100.0;
        double avgTime = timeSum / 100.0;
        double avgMemory = memorySum / 100.0;

        // Виводимо результати
        Console.WriteLine("Середня кiлькiсть порiвнянь: " + avgComparisons);
        Console.WriteLine("Середня кiлькiсть свапiв: " + avgSwaps);
        Console.WriteLine("Середнiй час виконання (в мiлiсекундах): " + avgTime);
        Console.WriteLine("Середня використана пам'ять (в байтах): " + avgMemory);
    }

    // Функція для генерації масиву з рівномірним розподілом
    static int[] GenerateUniformArray(int size, int minValue, int maxValue)
    {
        Random random = new Random();
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = random.Next(minValue, maxValue + 1);
        }
        return array;
    }

    // Функція для генерації масиву з нормальним розподілом
    static int[] GenerateNormalArray(int size, double mean, double stdDev)
    {
        Random random = new Random();
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = (int)(random.NextGaussian(mean, stdDev));
        }
        return array;
    }

    // Алгоритм сортування методом бульбашки
    static void BubbleSort(int[] array, out long comparisons, out long swaps)
    {
        comparisons = 0;
        swaps = 0;
        bool swapped;
        for (int i = 0; i < array.Length - 1; i++)
        {
            swapped = false;
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                comparisons++;
                if (array[j] > array[j + 1])
                {
                    Swap(array, j, j + 1);
                    swaps++;
                    swapped = true;
                }
            }
            // Якщо не відбувся жоден обмін на даній ітерації, масив вже відсортований
            if (!swapped) break;
        }
    }

    // Функція для обміну двох елементів масиву
    static void Swap(int[] array, int i, int j)
    {
        int temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }

    // Алгоритм сортування методом вставки
    static void InsertionSort(int[] array, out long comparisons, out long swaps)
    {
        comparisons = 0;
        swaps = 0;
        for (int i = 1; i < array.Length; i++)
        {
            int current = array[i];
            int j = i - 1;
            while (j >= 0 && array[j] > current)
            {
                comparisons++;
                array[j + 1] = array[j];
                swaps++;
                j--;
            }
            array[j + 1] = current;
        }
    }

    // Алгоритм сортування методом злиття
    static void MergeSort(int[] array, out int comparisons, out int swaps)
    {
        comparisons = 0;
        swaps = 0;
        MergeSortRecursive(array, 0, array.Length - 1, ref comparisons, ref swaps);
    }

    static void MergeSortRecursive(int[] array, int left, int right, ref int comparisons, ref int swaps)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            MergeSortRecursive(array, left, mid, ref comparisons, ref swaps);
            MergeSortRecursive(array, mid + 1, right, ref comparisons, ref swaps);
            Merge(array, left, mid, right, ref comparisons, ref swaps);
        }
    }

    static void Merge(int[] array, int left, int mid, int right, ref int comparisons, ref int swaps)
    {
        int n1 = mid - left + 1;
        int n2 = right - mid;

        int[] leftArray = new int[n1];
        int[] rightArray = new int[n2];

        Array.Copy(array, left, leftArray, 0, n1);
        Array.Copy(array, mid + 1, rightArray, 0, n2);

        int i = 0, j = 0, k = left;
        while (i < n1 && j < n2)
        {
            comparisons++;
            if (leftArray[i] <= rightArray[j])
            {
                array[k] = leftArray[i];
                i++;
            }
            else
            {
                array[k] = rightArray[j];
                j++;
            }
            k++;
            swaps++;
        }

        while (i < n1)
        {
            array[k] = leftArray[i];
            i++;
            k++;
            swaps++;
        }

        while (j < n2)
        {
            array[k] = rightArray[j];
            j++;
            k++;
            swaps++;
        }
    }

    // Алгоритм сортування швидким методом
    static void QuickSort(int[] array, out long comparisons, out long swaps)
    {
        comparisons = 0;
        swaps = 0;
        QuickSortRecursive(array, 0, array.Length - 1, ref comparisons, ref swaps);
    }

    static void QuickSortRecursive(int[] array, int left, int right, ref long comparisons, ref long swaps)
    {
        if (left < right)
        {
            int partitionIndex = Partition(array, left, right, ref comparisons, ref swaps);
            QuickSortRecursive(array, left, partitionIndex - 1, ref comparisons, ref swaps);
            QuickSortRecursive(array, partitionIndex + 1, right, ref comparisons, ref swaps);
        }
    }

    static int Partition(int[] array, int left, int right, ref long comparisons, ref long swaps)
    {
        int pivot = array[right];
        int i = left - 1;
        for (int j = left; j < right; j++)
        {
            comparisons++;
            if (array[j] < pivot)
            {
                i++;
                Swap(array, i, j);
                swaps++;
            }
        }
        Swap(array, i + 1, right);
        swaps++;
        return i + 1;
    }
}