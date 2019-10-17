using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Function
{
    // ゲーム設定↓
    readonly Vector2Int MapSize = new Vector2Int(10, 10);
    readonly int MineProb = 0; // Mineの確率 [%]
    //
    void Start()
    {
        StartCoroutine(Corridor());
    }

    IEnumerator Corridor()
    {
        CreateMap();
        yield return null;
        CameraControl();
        StartCoroutine("Player");
    }

    void CreateMap()
    {
        Data.map = new Square[MapSize.x,MapSize.y];
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                bool is_mine = Random(MineProb);
                Data.map[x, y] = new Square(new Vector2Int(x, y), is_mine);
            }
        }
    }

    bool Random(int probability)
    {
        int num = UnityEngine.Random.Range(0, 100);
        return num < probability;
    }

    void CameraControl()
    {
        Camera.main.transform.position = new Vector3(MapSize.x - 1, -(MapSize.y - 1)) * Data.SquareSize / 2f + new Vector3(0, 0, -10);
        float cam_size = MapSize.y / 2f;
        if (cam_size < MapSize.x / 2.5f)
            cam_size = MapSize.x / 2.5f;
        Camera.main.orthographicSize = cam_size;
    }

    IEnumerator Player()
    {
        while (true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
                if (hit)
                {
                    Vector2Int coordinate = hit.transform.GetComponent<Coordinate>().coordinate;
                    int judge = Data.map[coordinate.x, coordinate.y].Open();
                    if (judge == -1)
                    {
                        Debug.LogError("GameOver!!!");
                    }
                    else if (judge == 0)
                    {
                        yield return StartCoroutine(Spread(coordinate.x, coordinate.y));
                    }
                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
                if (hit)
                {
                    Vector2Int coordinate = hit.transform.GetComponent<Coordinate>().coordinate;
                    Data.map[coordinate.x, coordinate.y].Mark();
                }
            }
            yield return null;
        }
    }
    IEnumerator Spread(int x, int y)
    {
        yield return StartCoroutine(Wait(0.05f));
        int judge = Data.map[x, y].Open();
        if (judge == 0)
        {
            Vector2Int[] coordinate = new Vector2Int[4]
                {
                new Vector2Int(x - 1, y),
                new Vector2Int(x + 1, y),
                new Vector2Int(x, y + 1),
                new Vector2Int(x, y - 1)
                };
            SomeIEnumerator sie = new SomeIEnumerator(this);
            List<IEnumerator> enumerators = new List<IEnumerator>();
            foreach(Vector2Int xy in coordinate)
            {
                if(xy.x.Range(0,MapSize.x - 1) && xy.y.Range(0, MapSize.y - 1) && !Data.map[xy.x, xy.y].is_open)
                    enumerators.Add(Spread(xy.x, xy.y));
            }

            yield return sie.StartCoroutine(enumerators.ToArray());
        }
    }
}
