using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Data : MonoBehaviour
{
    public static Sprite SpriteMine;
    public static Sprite[] SpriteNum, // 0～8
        SpriteMark;
    public static GameObject SquarePrefab;

    public static Square[,] map;
    [HideInInspector]
    public static readonly float SquareSize = 0.32f*3;

    void Start()
    {
        SpriteMine = Resources.Load<Sprite>("Sprite/Mine");
        SpriteNum = new Sprite[]
            {
                Resources.Load<Sprite>("Sprite/None"),
                Resources.Load<Sprite>("Sprite/One"),
                Resources.Load<Sprite>("Sprite/Two"),
                Resources.Load<Sprite>("Sprite/Three"),
                Resources.Load<Sprite>("Sprite/Four"),
                Resources.Load<Sprite>("Sprite/Five"),
                Resources.Load<Sprite>("Sprite/Six"),
                Resources.Load<Sprite>("Sprite/Seven"),
                Resources.Load<Sprite>("Sprite/Eight")
            };
        SpriteMark = new Sprite[]
        {
                Resources.Load<Sprite>("Sprite/Normal"),
                Resources.Load<Sprite>("Sprite/Mark1"),
                Resources.Load<Sprite>("Sprite/Mark2"),
                Resources.Load<Sprite>("Sprite/Mark3")
        };
        SquarePrefab = Resources.Load<GameObject>("Prefab/Square");
        SceneManager.LoadScene("Game");
    }
}
