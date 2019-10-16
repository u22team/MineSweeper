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
        map = new mapStat();
        map.squareStats = new squareStat[mapSize.x, mapSize.y];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int mousePos = MousePosToMap(Input.mousePosition); //rayの方が良いかもしれない
            if (IsInMap(mousePos, mapSize))
            {
                if (map == null)
                {
                    map = CreateMap(mapSize, mousePos);
                }
                else if (!map.squareStats[mousePos.x, mousePos.y].open
                    && !map.squareStats[mousePos.x, mousePos.y].flag)
                {
                    OpenSquares(mousePos);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector2Int mousePos = MousePosToMap(Input.mousePosition);
            if (IsInMap(mousePos, mapSize))
            {
                if (!map.squareStats[mousePos.x, mousePos.y].open)
                    map.squareStats[mousePos.x, mousePos.y].flag = true;
            }
        }
    }

    mapStat CreateMap(Vector2Int _mapSize, Vector2Int _mousePos)
    {
        mapStat _map = new mapStat();
        _map.squareStats = new squareStat[_mapSize.x, _mapSize.y]; //サイズを初期化するのはここじゃないか startだ
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
                if (IsInMap(_pos, GetMapSize(_map))) if (_pos == _minePos) return true;
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
                    if (IsInMap(_pos, GetMapSize(_map.squareStats))) if (_pos != _minePos) _map.squareStats[_minePos.x, _minePos.y].mineStat.mineCount += 1;
                }
        }
        return _map;
    }

    void OpenSquares(Vector2Int _mousePos)
    {
        if (map.squareStats[_mousePos.x, _mousePos.y].mineStat.mineCount > 0) //数字が書いてある
        {
            map.squareStats[_mousePos.x, _mousePos.y].open = true;
        }
        else //数字が書いてない
        {
            Vector2Int[] searchDirs = { new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1) };
            List<node> searchPosS = new List<node>();
            List<node> _searchPosS = new List<node>();
            searchPosS.Add(new node(_mousePos, Vector2Int.zero));
            _searchPosS = searchPosS;

            while (searchPosS.Count > 0)
            {
                foreach (node searchPos in searchPosS)
                {
                    _searchPosS.Remove(searchPos);
                    foreach (Vector2Int searchDir in searchDirs) //for文２回方式とどちらがいいのか
                    {
                        if (searchDir != searchPos.exclusion) //来た方向に戻らない
                        {
                            Vector2Int nextSearchPos = searchPos.position + searchDir;
                            if (IsInMap(nextSearchPos, mapSize)
                                && !map.squareStats[nextSearchPos.x, nextSearchPos.y].open
                                && !map.squareStats[nextSearchPos.x, nextSearchPos.y].flag)
                            {
                                map.squareStats[nextSearchPos.x, nextSearchPos.y].open = true;
                                if (map.squareStats[nextSearchPos.x, nextSearchPos.y].mineStat.mineCount == 0) //数字が書いてないときだけ
                                {
                                    _searchPosS.Add(new node(nextSearchPos, oppositeDir(searchDir)));
                                }
                            }
                        }
                    }
                }
                searchPosS = _searchPosS;
            }
        }
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

    bool IsInMap(Vector2Int _pos, Vector2Int _mapSize) //名前を修正しろ //mapサイズはグローバル変数で良くないか
    {
        return 0 <= _pos.x && _pos.x <= _mapSize.x && 0 <= _pos.y && _pos.y <= _mapSize.y;
    }

    Vector2Int oppositeDir(Vector2Int vec)
    {
        return new Vector2Int(-vec.x, -vec.y);
    }

    //型の宣言

    class squareStat
    {
        public bool open = false;
        public bool flag = false;

        public class mineStatType
        {
            public int mineCount;
            public bool hasMine;
        }

        public mineStatType mineStat;

    }

    class mapStat
    {
        public squareStat[,] squareStats;
        public List<Vector2Int> minesPos = new List<Vector2Int>();
    }

    class node
    {
        public Vector2Int position;
        public Vector2Int exclusion;
        public node(Vector2Int position, Vector2Int exclusion)
        {
            this.position = position;
            this.exclusion = exclusion;
        }
    }
}