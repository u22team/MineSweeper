using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    //bool[,] mines;
    mapStat map;
    Vector2Int mapSize;

    void Start()
    {
        mapSize = new Vector2Int(10, 10);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (map == null)
                map = CreateMap(mapSize, MousePosToMap(Input.mousePosition));
            else
            {

            }
        }
        else if (Input.GetMouseButtonDown(1))
        {

        }
    }

    mapStat CreateMap(Vector2Int _mapSize, Vector2Int _mousePos)
    {
        mapStat _map = new mapStat();
        _map.squareStats = new squareStat[_mapSize.x, _mapSize.y];
        _map = InstallMines(_map, _mousePos);
        _map = CountMines(_map);
        return _map;
    }

    mapStat InstallMines(mapStat _map, Vector2Int _mousePos)
    {
        Vector2Int _mapSize = GetMapSize(_map.squareStats);
        for (int i = 0; i < _mapSize.x * _mapSize.y / 10; i++)
        {
            Vector2Int _minePos = new Vector2Int(); //初期化必要？
            bool cannotInstall = true;
            while (cannotInstall)
            {
                _minePos.x = (int)Random.Range(0, (float)_mapSize.x + 0.99f);
                _minePos.y = (int)Random.Range(0, (float)_mapSize.y + 0.99f);
                cannotInstall = _map.squareStats[_minePos.x, _minePos.y].mineStat.hasMine || IsAroundMousePos(_minePos, _mousePos, _map.squareStats);
            }
            _map.squareStats[_minePos.x, _minePos.y].mineStat.hasMine = true;
            _map.minesPos.Add(new Vector2Int(_minePos.x, _minePos.y));
        }
        return _map;
    }

    bool IsAroundMousePos(Vector2Int _minePos, Vector2Int _mousePos, squareStat[,] _map)
    {
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int _pos = _mousePos + new Vector2Int(i, j);
                if (IsInMap(_pos, _map)) if (_pos == _minePos) return true;
            }
        return false;
    }

    mapStat CountMines(mapStat _map)
    {
        Vector2Int _mapSize = GetMapSize(_map.squareStats);

        foreach (Vector2Int _minePos in _map.minesPos)
        {
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int _pos = _minePos + new Vector2Int(i, j);
                    if (IsInMap(_pos, _map.squareStats)) if (_pos != _minePos) _map.squareStats[_minePos.x, _minePos.y].mineStat.mineCount += 1;
                }
        }
        return _map;
    }

    //細かいメソッド

    Vector2Int GetMapSize(squareStat[,] _map) //名前を修正しろ
    {
        return new Vector2Int(_map.GetLength(0), _map.GetLength(1));
    }

    Vector2Int MousePosToMap(Vector3 _mousePos) //作りかけ screenを利用してクリック先を取得
    {
        Vector2Int mapPos = new Vector2Int(0, 0);
        return mapPos;
    }

    bool IsInMap(Vector2Int _pos, squareStat[,] _map) //名前を修正しろ
    {
        Vector2Int _mapSize = GetMapSize(_map);
        return 0 <= _pos.x && _pos.x <= _mapSize.x && 0 <= _pos.y && _pos.y <= _mapSize.y;
    }

    //型の宣言

    class squareStat
    {
        bool open;
        public class mineStatType
        {
            public int mineCount;
            public bool hasMine;
        }

        public mineStatType mineStat;

    }

    class mapStat //地雷の位置を記憶しておいたほうが高速なのは間違いない
    {
        public squareStat[,] squareStats;
        public List<Vector2Int> minesPos = new List<Vector2Int>();
    }
}
