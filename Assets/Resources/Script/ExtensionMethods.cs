using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ExtensionMethods
{
    public static bool Range(this int original, int min, int max, bool equal = true)
    {
        if(equal && (original == min || original == max))
            return true;
        if (original > min && original < max)
            return true;
        return false;

    }
}
