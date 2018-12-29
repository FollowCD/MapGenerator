using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyArea{
    public int UnitLenght
    {
        get
        {
            if (!Areas.IsNullOrEmpty())
            {
                return Areas[0].Count;
            }

            return 0;
        }
    }

    public EmptyArea()
    {
        Areas = new List<List<Vector2Int>>();
    }
    public List<List<Vector2Int>> Areas { get; private set; }

    public List<Vector2Int> Dequeue()
    {
        if (!Areas.IsNullOrEmpty())
        {
            var index = Random.Range(0, Areas.Count);

            var item = Areas[index];

            Areas.Remove(item);

            for (int i = Areas.Count - 1; i > -1; i--)
            {
                var temp = Areas[i];

                for (int j = 0; j < item.Count; j++)
                {
                    if (temp.Contains(item[j]))
                    {
                        Areas.Remove(temp);
                        break;
                    }
                }
            }

            return item;
        }

        return null;
    }

    public void Enqueue(List<Vector2Int> list)
    {
        if (list.IsNullOrEmpty())
        {
            Debug.LogError("要添加的list不包含数据");
            return;
        }

        if (Areas==null)
        {
            Areas = new List<List<Vector2Int>>();
        }
        else if (UnitLenght != list.Count && UnitLenght != 0)
        {
            Debug.LogError(string.Format("要添加的数据于存在数据单元长度不符,存在长度：{0}，添加长度：{1}", UnitLenght, list.Count));
            return;
        }

        Areas.Add(list);
    }

    public static EmptyArea Merge(int listCount,params EmptyArea[] data)
    {
        if (data==null||data.Length<1)
        {
            return null;
        }
        else
        {
            var result = new EmptyArea();
            var count = listCount;
            foreach (var item in data)
            {
                if (item!=null && !item.Areas.IsNullOrEmpty())
                {
                    if (count!=item.UnitLenght)
                    {
                        continue;
                    }

                    result.Areas.AddRange(item.Areas);
                }
            }

            return result;
        }
    }

    public int AreaCount
    {
        get
        {
            if (!Areas.IsNullOrEmpty())
            {
                return Areas.Count;
            }

            return 0;
        }
    }
}

