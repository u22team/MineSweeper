using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    //リソースがもっとたくさん出てくるとき、メモリ消費量を管理してResources.Loadを使うべきなのかもしれない
    //staticにしたらそもそもアタッチが効かなかったわ
    public static GameObject upper;
    public static GameObject ground;
    public static GameObject emphasis;
    public static GameObject parent; //これはプレハブではない

    public static Sprite[] sprCounts;
    public static Sprite sprMine;
    //public static Sprite sprEmphasis;

    public static Sprite sprFlag;
    public static Sprite sprDef;

    public static void LoadResources()
    {
        upper = Resources.Load<GameObject>("Prefabs/upper");
        ground = Resources.Load<GameObject>("Prefabs/ground");
        emphasis = Resources.Load<GameObject>("Prefabs/Emphasis");

        sprCounts = new Sprite[9];
        for (int i = 0; i < 9; i++) sprCounts[i] = Resources.Load<Sprite>("Sprites/" + i.ToString());
        sprMine = Resources.Load<Sprite>("Sprites/mine");
        //sprEmphasis = Resources.Load<Sprite>("Sprites/Emphasis");

        sprFlag = Resources.Load<Sprite>("Sprites/Flag");
        sprDef = Resources.Load<Sprite>("Sprites/Default");

        //this.gameObject.GetComponent<MineManager>().Detonate();
    }
}
