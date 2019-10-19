using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public int mineCount = 0;
    public bool hasMine = false;

    public GameObject gameObj;
    public Vector2Int pos;
    public SpriteRenderer sprren;

    public void instantiate(Vector2Int _vc)
    {
        gameObj = this.gameObject; //今の所使う予定はない
        pos = _vc;
        sprren = gameObj.GetComponent<SpriteRenderer>();
    }

    public void installMine()
    {

    }

    public void setMineCount()
    {
        sprren.sprite = ResourcesManager.sprCounts[mineCount];
    }
}
