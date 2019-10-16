using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    bool[,] mines;
    int[,] mineCount;
    Vector2Int mapSize;

    void Start()
    {
        mapSize = new Vector2Int(10, 10);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mines = CreateMap(mapSize, MousePosToMap(Input.mousePosition));
            mineCount = CreateMineCounts(mines);
        }
    }

    bool[,] CreateMap(Vector2Int _mapSize, Vector2Int _mousePos)
    {
        bool[,] stage;
        stage = new bool[_mapSize.x, _mapSize.y];
        for (int i = 0; i < _mapSize.x * _mapSize.y / 10; i++)
        {
            Vector2Int _mapPos = new Vector2Int(); //初期化必要？
            bool isfilled = true;
            while (isfilled)
            {
                _mapPos.x = (int)Random.Range(0, (float)_mapSize.x + 0.99f);
                _mapPos.y = (int)Random.Range(0, (float)_mapSize.y + 0.99f);
                isfilled = stage[_mapPos.x, _mapPos.y] || _mapPos == _mousePos;
            }
            stage[_mapPos.x, _mapPos.y] = true;
        }
        return stage;
    }

    int[,] CreateMineCounts(bool[,] _mines)
    {
        Vector2Int _mapSize = new Vector2Int(_mines.GetLength(0), _mines.GetLength(1));
        int[,] _mineCount = new int[_mapSize.x, _mapSize.y];
        for (int i = 0; i < _mapSize.x; i++)
            for (int j = 0; j < _mapSize.y; j++)
                GetMineCount(new Vector2Int(i, j), _mines);
        return _mineCount;
    }

    Vector2Int MousePosToMap(Vector3 _mousePos) //作りかけ screenを利用してクリック先を取得
    {
        Vector2Int mapPos = new Vector2Int(0, 0);
        return mapPos;
    }

    int GetMineCount(Vector2Int _squarePos, bool[,] _mines)
    {
        Vector2Int _mapSize = new Vector2Int(_mines.GetLength(0), _mines.GetLength(1));
        int[,] around = new int[8, 2] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
        int _minecount = 0;
        for (int i = 0; i < 8; i++)
        {
            Vector2Int _checkPos = new Vector2Int(_squarePos.x + around[i, 0], _squarePos.x + around[i, 1]);
            if (0 <= _checkPos.x && _checkPos.x <= _mapSize.x && 0 <= _checkPos.y && _checkPos.y <= _mapSize.y) _minecount += _mines[_checkPos.x, _checkPos.y] ? 1 : 0;
        }
        return _minecount;
    }


}
