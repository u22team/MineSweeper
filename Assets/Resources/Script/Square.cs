using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Square
{
    Vector2Int coordinate; // 座標
    bool is_mine;
    public bool is_open { get; private set; }
    SpriteRenderer sr;
    BoxCollider2D bc;
    int mark; // =0:Markなし

    public Square(Vector2Int coordinate, bool is_mine)
    {
        this.coordinate = coordinate;
        GameObject go = Object.Instantiate(Data.SquarePrefab, new Vector3(Data.SquareSize * coordinate.x, -Data.SquareSize * coordinate.y), Quaternion.identity);
        sr = go.GetComponent<SpriteRenderer>();
        bc = go.GetComponent<BoxCollider2D>();
        this.is_mine = is_mine;
        is_open = false;
        mark = 0;
        sr.sprite = Data.SpriteMark[mark];
        go.GetComponent<Coordinate>().coordinate = coordinate;
    }

    public int Open() // Mineなら-1を返し、違うなら周りのMineの個数を返す。
    {
        if (is_open)
        {
            Debug.LogWarning("すでにopenです。");
        }
        is_open = true;
        bc.enabled = false;
        if (this.is_mine)
        {
            sr.sprite = Data.SpriteMine;
            return -1;
        }
        else
        {
            int num = 0;
            for (int x = coordinate.x - 1; x <= coordinate.x + 1; x++)
            {
                if (!x.Range(0, Data.map.GetLength(0) - 1))
                    continue;
                for (int y = coordinate.y - 1; y <= coordinate.y + 1; y++)
                {
                    if (!y.Range(0, Data.map.GetLength(1) - 1))
                        continue;
                    if (Data.map[x, y].is_mine)
                        num++;
                }
            }
            sr.sprite = Data.SpriteNum[num];
            return num;
        }
    }

    public void Close()
    {
        if (!is_open)
        {
            Debug.LogWarning("すでにcloseです。");
        }
        sr.sprite = Data.SpriteMark[mark];
        is_open = false;
        bc.enabled = true;
    }

    public void Mark() // 次のMarkにする
    {
        if (!is_open)
        {
            mark++;
            if (mark > Data.SpriteMark.Length - 1)
                mark = 0;
            sr.sprite = Data.SpriteMark[mark];
        }
    }
    public void Mark(int num) // 指定したMarkにする
    {
        if (!is_open)
        {
            mark = num;
            if (mark > Data.SpriteMark.Length - 1)
            {
                Debug.LogWarning("想定より大きい値です。値：" + num);
                mark = Data.SpriteMark.Length - 1;
            }
            else if (mark < 0)
            {
                Debug.LogWarning("想定より小さい値です。値：" + num);
                mark = 0;
            }
            sr.sprite = Data.SpriteMark[mark];
        }
    }
}
