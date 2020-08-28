using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void AddListElemIfNotExists<T>(List<T> list, T elem)
    {
        if (!list.Contains(elem))
        {
            list.Add(elem);
        }
    }
}
