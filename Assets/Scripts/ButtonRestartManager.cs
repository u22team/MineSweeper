using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonRestartManager : MonoBehaviour
{
    public void Restart()
    {
        //めんどくさいのでシーンをリロードする
        SceneManager.LoadScene("Game_aty");
    }
}
