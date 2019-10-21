using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public int mineCount = 0;
    public bool hasMine = false;

    public Vector2Int pos;
    public SpriteRenderer sprren;

    public void instantiate(Vector2Int _vc)
    {
        pos = _vc;
        sprren = this.gameObject.GetComponent<SpriteRenderer>();
    }

    public void installMine()
    {
        hasMine = true;
        sprren.sprite = ResourcesManager.sprMine;
    }

    public void plusMineCount()
    {
        mineCount++;
    }

    public void setMineCountSpr()
    {
        if(hasMine == false) sprren.sprite = ResourcesManager.sprCounts[mineCount];
    }
}
