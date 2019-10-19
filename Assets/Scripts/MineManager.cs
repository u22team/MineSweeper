using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    mapStat map;
    Vector2Int mapSize;
    float scale = 1.4f;
    bool isGroundCreated;
    bool isCoroutineRunning; //複数コルーチンに対応できないので、intにして１足し１引くみたいな方がいい
    GameObject parent;

    void Start()
    {
        ResourcesManager.LoadResources(); //普通に書けば終了するまで次へは進まないはず
        mapSize = new Vector2Int(10, 10); //Mapのサイズによってカメラを制御せんとな
        map = new mapStat(mapSize);
        StartCoroutine(CreateUpper());
        isGroundCreated = false;
        isCoroutineRunning = false;
        parent = this.gameObject;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //ここもう少しスマートに書きたい コルーチンにするといいらしい
        {
            if (!isCoroutineRunning)
            {
                Vector2Int mousePos;
                if (MousePosToMap(Input.mousePosition, out mousePos)) //outにしたのでopen不要説
                {
                    if (!isGroundCreated)
                    {
                        CreateGround(mousePos);
                        isGroundCreated = true;

                    }

                    if (!map.ugS[mousePos.x, mousePos.y].upMana.open //openで判断するよりもクリックしたgameobjectで判断するほうが確実のような気がする
                        && !map.ugS[mousePos.x, mousePos.y].upMana.flag) //というかgroundクリックする必要ないんだからコライダをつけなければいいだけでは
                    {
                        OpenSquares(mousePos);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!isCoroutineRunning)
            {
                Vector2Int mousePos;
                if (MousePosToMap(Input.mousePosition, out mousePos))
                {
                    map.ugS[mousePos.x, mousePos.y].upMana.Flag();
                }
            }
        }
    }

    //メジャーなメソッド群

    IEnumerator CreateUpper()
    {
        isCoroutineRunning = true;
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                /* こういうカオスなこともできる 記念にとっておこう
                (isUp ? ugObjects.uppers : ugObjects.grounds)[i, j] = Instantiate((isUp ? upper : ground), scale * new Vector3(i, j, 0), Quaternion.identity, parent.transform);
                SquareManager sm = (true ? ugObjects.uppers : ugObjects.grounds)[i, j].GetComponent<SquareManager>();
                */
                Vector2Int _vc = new Vector2Int(i, j);
                map.ugS[i, j].instantiateUpper(_vc, Instantiate(ResourcesManager.upper, scale * new Vector3(i, j, 0), Quaternion.identity, parent.transform));
                map.ugS[i, j].instantiateGround(_vc, Instantiate(ResourcesManager.ground, scale * new Vector3(i, j, 0), Quaternion.identity, parent.transform));
                yield return new WaitForSeconds(.01f);
            }
        isCoroutineRunning = false;
    }

    void CreateGround(Vector2Int _mousePos)
    {
        InstallMines(_mousePos);
        CountMines();
    }

    void InstallMines(Vector2Int _mousePos) //詰まないように修正の必要あり //検討箇所を絞っていかないと不安定
    {
        for (int i = 0; i < mapSize.x * mapSize.y / 10; i++) //とりあえずの地雷の個数
        {
            Vector2Int _minePos = new Vector2Int(); //初期化必要？
            bool cannotInstall = true;
            while (cannotInstall) //これ、場合によっては抜けられなくなるかも
            {
                _minePos.x = Random.Range(0, mapSize.x);
                _minePos.y = Random.Range(0, mapSize.y);
                cannotInstall = map.ugS[_minePos.x, _minePos.y].grMana.hasMine || IsAroundMousePos(_minePos, _mousePos);
            }
            map.ugS[_minePos.x, _minePos.y].grMana.installMine(); //hasMine = true;
            map.minesPos.Add(new Vector2Int(_minePos.x, _minePos.y));
            //map.ugS[_minePos.x, _minePos.y].grMana.sprren.sprite = sprMine;
        }
    }

    void CountMines() //最終的に数字のスプライトに差し替えるならはじめから各マス検索方式のほうがいいような気もする
    {
        foreach (Vector2Int _minePos in map.minesPos)
        {
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int _pos = _minePos + new Vector2Int(i, j);
                    if (IsInMap(_pos) && _pos != _minePos) map.ugS[_pos.x, _pos.y].grMana.mineCount += 1; //mineCountを直接いじるのはあんま気持ちよくない
                }
        }
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
                map.ugS[i, j].grMana.setMineCount();
    }

    void OpenSquares(Vector2Int _mousePos)
    {
        if (map.ugS[_mousePos.x, _mousePos.y].grMana.hasMine) //地雷あり
        {
            //game over処理
        }
        else
        {
            if (map.ugS[_mousePos.x, _mousePos.y].grMana.mineCount > 0) //数字が書いてある これ数字が書いてないときの処理と同じでいいような
            {
                map.ugS[_mousePos.x, _mousePos.y].upMana.Open();
            }
            else //数字が書いてない
            {
                StartCoroutine(OpenAnimation(_mousePos));
            }
            //if () Clear(); //クリア処理
        }
    }

    IEnumerator OpenAnimation(Vector2Int _mousePos) //広がり方がキモい。斜めへの展開は1回遅延させたほうがいいかもしれない
    {
        //この配列２つ、二重配列かジャグ配列にしたほうがいいかも まあ数が少ないからいいか いやクラスにしてtrueとかも持たせるとめんどいわ
        Vector2Int[] searchDirs = { new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(1, 0) };
        Vector2Int[] searchDirsDiag = { new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 1) };

        isCoroutineRunning = true;
        List<node> searchPosS = new List<node>();
        searchPosS.Add(new node(_mousePos, false));

        int i = 0;
        while (searchPosS.Count > 0 && i < 100) //フリーズ回避策を一応いれておく
        {
            List<node> _searchPosS = new List<node>(searchPosS);
            foreach (node searchPos in searchPosS)
            {
                if (searchPos.delay) //遅延あり
                {
                    searchPos.delay = false;
                }
                else //遅延なし
                {
                    map.ugS[searchPos.position.x, searchPos.position.y].upMana.Open();
                    _searchPosS.Remove(searchPos);
                    if (map.ugS[searchPos.position.x, searchPos.position.y].grMana.mineCount == 0) //数字が書いてないときだけ予約を入れられる
                    {
                        search(searchDirs, false); //垂直方向
                        search(searchDirsDiag, true); //斜め方向 順番を入れ替えてはならない
                    }

                    void search(Vector2Int[] _vc, bool tf)
                    {
                        foreach (Vector2Int searchDir in _vc) //for文２回方式とどちらがいいのか ここは無理だが
                        {
                            Vector2Int nextSearchPos = searchPos.position + searchDir;
                            if (IsInMap(nextSearchPos)
                                && !map.ugS[nextSearchPos.x, nextSearchPos.y].upMana.open
                                && !map.ugS[nextSearchPos.x, nextSearchPos.y].upMana.flag)
                            {
                                _searchPosS.Add(new node(nextSearchPos, tf));
                                map.ugS[nextSearchPos.x, nextSearchPos.y].upMana.PlanToOpen();
                            }
                        }
                    }
                }
            }
            i++;
            searchPosS = new List<node>(_searchPosS); //listはデフォルトで参照渡しのため、コンストラクタを利用する
            yield return new WaitForSeconds(.05f);
        }
        isCoroutineRunning = false;
        yield break; //これ必要ないのか
    }

    //細かいメソッド

    bool MousePosToMap(Vector3 _mousePos, out Vector2Int _mousePosToMap)
    {
        _mousePosToMap = new Vector2Int(-100, -100);
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

        if (hit2d)
        {
            _mousePosToMap = hit2d.transform.gameObject.GetComponent<UpperManager>().pos;
            return true;
        }
        return false;
    }

    bool IsInMap(Vector2Int _pos) //名前を修正しろ //mapサイズはグローバル変数で良くないか
    {
        return 0 <= _pos.x && _pos.x < mapSize.x && 0 <= _pos.y && _pos.y < mapSize.y;
    }

    bool IsAroundMousePos(Vector2Int _minePos, Vector2Int _mousePos)
    {
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int _pos = _mousePos + new Vector2Int(i, j);
                if (_pos == _minePos) return true;
            }
        return false;
    }

    //型の宣言

    class mapStat //全部staticにすればインスタンス地獄に悩むこともないが、拡張性の観点からこのままでもいいかも
    {
        public ugArray[,] ugS;
        public List<Vector2Int> minesPos = new List<Vector2Int>();
        public mapStat(Vector2Int vc) //配列はlistと違って必ずインスタンス化の必要がある
        {
            this.ugS = new ugArray[vc.x, vc.y];
            for (int i = 0; i < vc.x; i++) for (int j = 0; j < vc.y; j++) this.ugS[i, j] = new ugArray();
        }

        public class ugArray //Instantiate -> GameObjectからManagerを取得 -> Managerの中にGameObjectを代入 の方がいいか
        {
            public UpperManager upMana;
            public GroundManager grMana;

            public void instantiateUpper(Vector2Int _vc, GameObject go)
            {
                this.upMana = go.GetComponent<UpperManager>();
                this.upMana.instantiate(_vc);
            }

            public void instantiateGround(Vector2Int _vc, GameObject go)
            {
                this.grMana = go.GetComponent<GroundManager>();
                this.grMana.instantiate(_vc);
            }
        }
    }

    class node
    {
        public Vector2Int position;
        public bool delay;
        public node(Vector2Int _position, bool _delay)
        {
            this.position = _position;
            this.delay = _delay;
        }
    }
}