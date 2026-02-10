using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace QFramework
{
    public static class BubbleSort
    {
        /// <summary>
        /// 升序冒泡排序
        /// </summary>
        /// <param name="array">原始数组</param>
        public static int[] Bubble_Rise(this int[] array)
        {
            int[] copy = new int[array.Length];
            array.CopyTo(copy, 0);

            for (int i = 0; i < copy.Length - 1; i++)
            {
                for (int j = 0; j < copy.Length - 1 - i; j++)
                {
                    if (copy[j] > copy[j + 1])
                    {
                        int temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }

        /// <summary>
        /// 升序冒泡排序
        /// </summary>
        /// <param name="array">原始数组</param>
        public static List<int> Bubble_Rise(this List<int> list)
        {
            List<int> copy = list.Copy();

            for (int i = 0; i < copy.Count - 1; i++)
            {
                for (int j = 0; j < copy.Count - 1 - i; j++)
                {
                    if (copy[j] > copy[j + 1])
                    {
                        int temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }

        /// <summary>
        /// 升序冒泡排序
        /// </summary>
        /// <param name="array">原始数组</param>
        public static float[] Bubble_Rise(this float[] array)
        {
            float[] copy = new float[array.Length];
            array.CopyTo(copy, 0);

            for (int i = 0; i < copy.Length - 1; i++)
            {
                for (int j = 0; j < copy.Length - 1 - i; j++)
                {
                    if (copy[j] > copy[j + 1])
                    {
                        float temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }

        /// <summary>
        /// 升序冒泡排序
        /// </summary>
        /// <param name="list">原始链表</param>
        public static List<float> Bubble_Rise(this List<float> list)
        {
            List<float> copy = list.Copy();

            for (int i = 0; i < copy.Count - 1; i++)
            {
                for (int j = 0; j < copy.Count - 1 - i; j++)
                {
                    if (copy[j] > copy[j + 1])
                    {
                        float temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }

        /// <summary>
        /// 降序冒泡排序
        /// </summary>
        /// <param name="array">原始数组</param>
        public static int[] Bubble_Low(this int[] array)
        {
            int[] copy = new int[array.Length];
            array.CopyTo(copy, 0);

            for (int i = 0; i < copy.Length - 1; i++)
            {
                for (int j = 0; j < copy.Length - 1 - i; j++)
                {
                    if (copy[j] < copy[j + 1])
                    {
                        int temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }

        /// <summary>
        /// 降序冒泡排序
        /// </summary>
        /// <param name="list">原始链表</param>
        public static List<int> Bubble_Low(this List<int> list)
        {
            List<int> copy = list.Copy();

            for (int i = 0; i < copy.Count - 1; i++)
            {
                for (int j = 0; j < copy.Count - 1 - i; j++)
                {
                    if (copy[j] < copy[j + 1])
                    {
                        int temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }

        /// <summary>
        /// 降序冒泡排序
        /// </summary>
        /// <param name="array">原始数组</param>
        public static float[] Bubble_Low(this float[] array)
        {
            float[] copy = new float[array.Length];
            array.CopyTo(copy, 0);

            for (int i = 0; i < copy.Length - 1; i++)
            {
                for (int j = 0; j < copy.Length - 1 - i; j++)
                {
                    if (copy[j] < copy[j + 1])
                    {
                        float temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }

        /// <summary>
        /// 降序冒泡排序
        /// </summary>
        /// <param name="list">原始链表</param>
        public static List<float> Bubble_Low(this List<float> list)
        {
            List<float> copy = list.Copy();

            for (int i = 0; i < copy.Count - 1; i++)
            {
                for (int j = 0; j < copy.Count - 1 - i; j++)
                {
                    if (copy[j] < copy[j + 1])
                    {
                        float temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            return copy;
        }


        public static void Bubble_Rise<T>(this List<T> tList, Func<T, int> valueSelecter)
        {
            int n = tList.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    // 使用选择器获取比较的关键值
                    if (valueSelecter(tList[j]) > valueSelecter(tList[j + 1]))
                    {
                        // 交换两个元素
                        T temp = tList[j];
                        tList[j] = tList[j + 1];
                        tList[j + 1] = temp;
                    }
                }
            }
        }

        public static void Bubble_Rise<T>(this List<T> tList, Func<T, float> valueSelecter)
        {
            int n = tList.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    // 使用选择器获取比较的关键值
                    if (valueSelecter(tList[j]) > valueSelecter(tList[j + 1]))
                    {
                        // 交换两个元素
                        T temp = tList[j];
                        tList[j] = tList[j + 1];
                        tList[j + 1] = temp;
                    }
                }
            }
        }

        public static void BubbleSortArray<T>(this T[] array, Func<T, int> keySelector)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (keySelector(array[j]) > keySelector(array[j + 1]))
                    {
                        T temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

        public static void BubbleSortArray<T>(this T[] array, Func<T, float> keySelector)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (keySelector(array[j]) > keySelector(array[j + 1]))
                    {
                        T temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

        public static void Bubble_Low<T>(this List<T> list, Func<T, int> keySelector)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (keySelector(list[j]) < keySelector(list[j + 1]))
                    {
                        T temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
        }

        public static void Bubble_Low<T>(this List<T> list, Func<T, float> keySelector)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (keySelector(list[j]) < keySelector(list[j + 1]))
                    {
                        T temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
        }

        public static void Bubble_Low<T>(this T[] array, Func<T, int> keySelector)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (keySelector(array[j]) < keySelector(array[j + 1]))
                    {
                        T temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

        public static void Bubble_Low<T>(this T[] array, Func<T, float> keySelector)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (keySelector(array[j]) < keySelector(array[j + 1]))
                    {
                        T temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

    }
}