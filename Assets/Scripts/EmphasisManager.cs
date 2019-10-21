using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmphasisManager : MonoBehaviour
{
    public Vector2Int pos;

    public void instantiate(Vector2Int _vc)
    {
        pos = _vc;
    }

    public void destroy()
    {
        Destroy(this.gameObject);
    }

    public IEnumerator WaitOpen()
    {
        if (true)
        {
            yield break;
        }
        yield return null;
    }
}
