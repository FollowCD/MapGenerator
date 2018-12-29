using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension {

    public static bool IsNullOrEmpty<T>(this ICollection<T> list)
    {
        return list == null || list.Count < 1;
    }

    public static bool HasElement<T>(this ICollection<T> collection)
    {
        return collection != null && collection.Count > 0;
    }

    public static bool MergeCollection<T>(this ICollection<T> col,ICollection<T> data)
    {
        if (col==null)
        {
            Debug.LogError("无法并入数据，因为目标集合为空");
            return false;
        }
        else if (data.IsNullOrEmpty())
        {
            Debug.Log("要加入的数据为空或者无元素");
            return true;
        }
        else
        {
            foreach (var item in data)
            {
                if (!col.Contains(item))
                {
                    col.Add(item);
                }
            }
            return true;
        }
    }

    public static bool DeleteCollections<T>(this ICollection<T> col, ICollection<T> data)
    {
        if (col == null)
        {
            Debug.LogError("无法删除数据，因为目标集合为空");
            return false;
        }
        else if (data.IsNullOrEmpty())
        {
            Debug.Log("要删除的集合为空或者无元素");
            return true;
        }
        else
        {
            foreach (var item in data)
            {
                if (col.Contains(item))
                {
                    col.Remove(item);
                }
            }

            return true;
        }
    }

    public static bool BiggerThan(this Vector2 one,Vector2 two)
    {
        return one.x > two.x && one.y > two.y;
    }

    public static V GetValue<T,V>(this Dictionary<T,V> dict,T key)
    {
        var v = default(V);
        if (dict!=null)
        {
            dict.TryGetValue(key, out v);
        }
        else
        {
            Debug.LogError("无法获取字典值，因为字典为空");
        }

        return v;
    }

    public static T GetData<T>(this T[,] array,Vector2Int index)
    {
        if (array==null||array.LongLength<1)
        {
            Debug.LogError("数组为空或元素为0");
            return default(T);
        }
        else
        {
            if (index.x>=0&& index.x<array.GetLength(0) && index.y>=0 && index.y<array.GetLength(1))
            {
                return array[index.x, index.y];
            }
            else
            {
                Debug.LogError(string.Format("下标越界，无法获取数据，数组={0}，{1}，index ={2}", 
                    array.GetLength(0), array.GetLength(1), index.ToString()));
                return default(T);
            }
        }
    }

    public static T GetData<T>(this T[,] array,byte x,byte y)
    {
        if (array == null || array.LongLength < 1)
        {
            Debug.LogError("数组为空或元素为0");
            return default(T);
        }
        else
        {
            if (x < array.GetLength(0) && y < array.GetLength(1))
            {
                return array[x, y];
            }
            else
            {
                Debug.LogError(string.Format("下标越界，无法获取数据，数组={0}，{1}，index =({2},{3})",
                    array.GetLength(0), array.GetLength(1),x,y ));
                return default(T);
            }
        }
    }

    public static int Sum(this ICollection<int> collection)
    {
        if (collection != null)
        {
            int sum = 0;

            foreach (var item in collection)
            {
                sum += item;
            }

            return sum;
        }
        return 0;
    }
}
