using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineManager : MonoBehaviour
{
    mapStat map;
    Vector2Int mapSize;
    float scale = 1.4f;
    bool isGroundCreated;
    int isCoroutineRunning; //複数コルーチンに対応できないので、intにして１足し１引くみたいな方がいい
    [SerializeField] GameObject gameover;
    [SerializeField] GameObject timeobj;
    float startTime = 0;
    bool start = false;
    float laps;

    void Start()
    {
        ResourcesManager.LoadResources(); //普通に書けば終了するまで次へは進まないはず 勉強のためasyncにしてみよう
        mapSize = new Vector2Int(10, 10); //Mapのサイズによってカメラを制御せんとな
        map = new mapStat(mapSize);
        ResourcesManager.parent = this.gameObject;
        //StartCoroutine(CreateUpper());
        isGroundCreated = false;
        isCoroutineRunning = 0;
    }

    void Update()
    {
        laps = Time.time - startTime;
        string strtime = ((int)laps / 60).ToString() + "m\n" + (laps % 60).ToString("f1") + "s";
        if (start) timeobj.GetComponent<Text>().text = strtime;
        if (Input.GetMouseButtonDown(0)) //ここもう少しスマートに書きたい コルーチンにするといいらしい
        {
            if (isCoroutineRunning == 0)
            {
                Vector2Int mousePos;
                if (MousePosToMap(Input.mousePosition, out mousePos)) //outにしたのでopen不要説
                {
                    if (!isGroundCreated)
                    {
                        CreateGround(mousePos);
                        isGroundCreated = true;
                        startTime = Time.time;
                        start = true;
                    }

                    if (!map.ugS[mousePos.x, mousePos.y].upMana.open //openで判断するよりもクリックしたgameobjectで判断するほうが確実のような気がする
                        && !map.ugS[mousePos.x, mousePos.y].upMana.flag) //ここで判定かますのはあんまいい考えとは思えない
                    {
                        OpenSquares(mousePos);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (isCoroutineRunning == 0)
            {
                Vector2Int mousePos;
                if (MousePosToMap(Input.mousePosition, out mousePos))
                {
                    map.ugS[mousePos.x, mousePos.y].upMana.Flag();
                }
            }
        }

        //if (Input.GetKeyDown(KeyCode.H)) Hint1(); //まだ作りかけ
    }

    // =============================================== メジャーなメソッド群 ================================================================================

    public void StartCreateUpper()
    {
        timeobj.SetActive(true);
        StartCoroutine(CreateUpper());
    }

    IEnumerator CreateUpper()
    {
        isCoroutineRunning++;
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                /* こういうカオスなこともできる 記念にとっておこう
                (isUp ? ugObjects.uppers : ugObjects.grounds)[i, j] = Instantiate((isUp ? upper : ground), scale * new Vector3(i, j, 0), Quaternion.identity, parent.transform);
                SquareManager sm = (true ? ugObjects.uppers : ugObjects.grounds)[i, j].GetComponent<SquareManager>();
                */
                Vector2Int _vc = new Vector2Int(i, j);
                map.ugS[i, j].instantiateUpper(_vc, Instantiate(ResourcesManager.upper, scale * new Vector3(i, j, 0), Quaternion.identity, ResourcesManager.parent.transform));
                map.ugS[i, j].instantiateGround(_vc, Instantiate(ResourcesManager.ground, scale * new Vector3(i, j, 0), Quaternion.identity, ResourcesManager.parent.transform));
                yield return new WaitForSeconds(.01f);
            }
        isCoroutineRunning--;
    }

    void CreateGround(Vector2Int _mousePos)
    {
        InstallMines(_mousePos);
        CountMines();
    }

    void InstallMines(Vector2Int _mousePos)
    {
        /*
        List<Vector2Int> squarePosList = new List<Vector2Int>();
        for (int i = 0; i < mapSize.x; i++) //これデリゲートにすればいいということがわかったぞ ラムダ式を引数に？
            for (int j = 0; j < mapSize.y; j++)
                if (!IsAroundMousePos(new Vector2Int(i, j), _mousePos))
                    squarePosList.Add(new Vector2Int(i, j));

        for (int i = 0; i < map.mineNum; i++)
        {
            //Vector2Int _minePos = squarePosList[Random.Range(0, squarePosList.Count)];
            Vector2Int _minePos = squarePosList[Random.Range(0, Random.Range(squarePosList.Count, 10000)) % squarePosList.Count];
            map.ugS[_minePos.x, _minePos.y].grMana.installMine();
            map.minesPos.Add(_minePos);
            squarePosList.Remove(_minePos); //こうすることで判定せず設置でき、安定的に地雷を設置できる
        }
        */
        // ↑何か偏りが生じる気がする

        for (int i = 0; i < map.mineNum; i++) //とりあえずの地雷の個数
        {
            Vector2Int _minePos = new Vector2Int(); //初期化必要？
            bool cannotInstall = true;
            while (cannotInstall) //これ、場合によっては抜けられなくなるかも
            {
                _minePos.x = Random.Range(0, mapSize.x);
                _minePos.y = Random.Range(0, mapSize.y);
                cannotInstall = map.ugS[_minePos.x, _minePos.y].grMana.hasMine || IsAroundMousePos(_minePos, _mousePos);
            }
            map.ugS[_minePos.x, _minePos.y].grMana.installMine();
            map.minesPos.Add(_minePos);
        }
    }

    void CountMines() //最終的に数字のスプライトに差し替えるならはじめから各マス検索方式のほうがいいような気もする
    {
        foreach (Vector2Int _minePos in map.minesPos)
        {
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++) //正直いいやり方ではない
                {
                    Vector2Int _pos = _minePos + new Vector2Int(i, j);
                    if (IsInMap(_pos) && _pos != _minePos) map.ugS[_pos.x, _pos.y].grMana.plusMineCount();
                }
        }
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
                map.ugS[i, j].grMana.setMineCountSpr();
    }

    void OpenSquares(Vector2Int _mousePos)
    {
        if (map.ugS[_mousePos.x, _mousePos.y].grMana.hasMine) //地雷あり game over処理
        {
            start = false;
            map.ugS[_mousePos.x, _mousePos.y].upMana.Open();
            Camera.main.GetComponent<CameraManager>().ShakeCamera();
            StartCoroutine(OpenMinesAnimation(_mousePos));
        }
        else
        {
            if (map.ugS[_mousePos.x, _mousePos.y].grMana.mineCount > 0)
            { //数字が書いてある これ数字が書いてないときの処理と同じでいいような 軽量化のためこれでいいか
                map.ugS[_mousePos.x, _mousePos.y].upMana.Open();
                map.openedSquares.Add(new hintNode(_mousePos));
                if (map.openedSquares.Count + map.mineNum == mapSize.x * mapSize.y) Clear(); //クリア処理
            }
            else StartCoroutine(OpenAnimation(_mousePos)); //数字が書いてない
        }
    }

    void Clear()
    {
        start = false;
        gameover.GetComponent<GameOverManager>().GameClear(laps);
    }

    IEnumerator OpenMinesAnimation(Vector2Int vc)
    {
        isCoroutineRunning++;
        yield return new WaitForSeconds(1);
        for (int i = 1; i < mapSize.x; i++) //広がるようにやりたいからこのクソ手間のかかる方法をとりあえず
        {
            for (int j = -i; j <= i; j++)
                for (int k = -i; k <= i; k++)
                {
                    Vector2Int vc2 = vc + new Vector2Int(j, k);
                    if (IsInMap(vc2) && map.ugS[vc2.x, vc2.y].grMana.hasMine && !map.ugS[vc2.x, vc2.y].upMana.open) map.ugS[vc2.x, vc2.y].upMana.Open();
                }
            yield return new WaitForSeconds(.2f);
        }
        gameover.GetComponent<GameOverManager>().GameOver();
    }

    IEnumerator OpenAnimation(Vector2Int _mousePos)
    {
        isCoroutineRunning++;
        //この配列２つ、二重配列かジャグ配列にしたほうがいいかも まあ数が少ないからいいか いやクラスにしてtrueとかも持たせるとめんどいわ
        Vector2Int[] searchDirs = { new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(1, 0) };
        Vector2Int[] searchDirsDiag = { new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 1) };

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
                    map.openedSquares.Add(new hintNode(searchPos.position));
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
            yield return new WaitForSeconds(.07f);
        }
        if (map.openedSquares.Count + map.mineNum == mapSize.x * mapSize.y) Clear(); //クリア処理

        isCoroutineRunning--;
        yield break; //これ必要ないのか
    }

    public void Hint1() //作りかけ 間違ったflagの強調、flagがないときの強調、flag数が十分なときの
    {
        /*
        for (int i = 0; i < mapSize.x; i++) //まず間違ったflagの指摘 flagもlistにぶち込んだほうが効率は良い 意図的に適当なflagを置いて確認するという戦法が有り得てしまう
            for (int j = 0; j < mapSize.y; j++)
            {
                if (map.ugS[i, j].upMana.flag == true && map.ugS[i, j].grMana.hasMine == false)
                {
                    map.ugS[i, j].instantiateEmphasis(new Vector2Int(i, j), Instantiate(ResourcesManager.emphasis, scale * new Vector3(i, j, 0), Quaternion.identity, ResourcesManager.parent.transform));
                    return;
                }
            }

        foreach (hintNode openedSquare in map.openedSquares) // 次に、flagが立ってなかったらその時点でそこを強調表示し、flagを立てさせる。
        { 
            int closedCount = 0;
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++) //destroyのタイミング管理結構面倒だぞ 生成と同時にコルーチンを呼び出し、1.openされたら削除、2.flagを立てられたら削除、3.数字に等しくなったら削除
                {
                    Vector2Int _pos = openedSquare.pos + new Vector2Int(i, j);
                    if (IsInMap(_pos)) if (map.ugS[_pos.x, _pos.y].upMana.open == false) closedCount++;
                }
            if (closedCount == map.ugS[openedSquare.pos.x, openedSquare.pos.y].grMana.mineCount)
            {
                //Instantiate(ResourcesManager.emphasis, scale * new Vector3(i, j, 0), Quaternion.identity, ResourcesManager.parent.transform);
            }
        }

        while (true)
        {
            foreach (hintNode openedSquare in map.openedSquares)
            {
                Instantiate(ResourcesManager.emphasis, scale * new Vector3(openedSquare.pos.x, openedSquare.pos.y, 0), Quaternion.identity, ResourcesManager.parent.transform);
            }
        }
        */
    }

    // ================================================== 細かいメソッド ==================================================================================

    bool MousePosToMap(Vector3 _mousePos, out Vector2Int _mousePosToMap) //これなんとかして各Upperに持たせられないかな
    {
        _mousePosToMap = new Vector2Int();
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

        if (hit2d)
        {
            _mousePosToMap = hit2d.transform.gameObject.GetComponent<UpperManager>().pos;
            return true;
        }
        return false;
    }

    bool IsInMap(Vector2Int _pos)
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

    // ======================================= 型の宣言 ===================================================================================================

    class mapStat //全部staticにすればインスタンス地獄に悩むこともないが、拡張性の観点からこのままでもいいかも
    {
        public ugArray[,] ugS;
        public List<Vector2Int> minesPos = new List<Vector2Int>();

        public int mineNum;
        public mapStat(Vector2Int _mapSize) //配列はlistと違って必ずここでインスタンス化の必要がある サイズを必ず決めなければならないから
        {
            this.ugS = new ugArray[_mapSize.x, _mapSize.y];
            for (int i = 0; i < _mapSize.x; i++)
                for (int j = 0; j < _mapSize.y; j++)
                    this.ugS[i, j] = new ugArray();
            mineNum = _mapSize.x * _mapSize.y / 8;
        }

        public class ugArray //Instantiate -> GameObjectからManagerを取得 -> Managerの中にGameObjectを代入 の方がいいか
        {
            public UpperManager upMana;
            public GroundManager grMana;
            public EmphasisManager emMana;

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

            public void instantiateEmphasis(Vector2Int _vc, GameObject go)
            {
                this.emMana = go.GetComponent<EmphasisManager>();
                this.emMana.instantiate(_vc);
            }
        }

        public List<hintNode> openedSquares = new List<hintNode>();
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


    public class hintNode
    {
        public Vector2Int pos;
        public bool ischecked;

        public hintNode(Vector2Int _pos)
        {
            this.pos = _pos;
            this.ischecked = false;
        }
    }

    public enum waitType
    {
        open,
        flag,
        check
    }
}