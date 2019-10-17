using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperManager : MonoBehaviour
{
    public Vector2Int pos;

    public bool open = false;
    public bool flag = false;

    public void Open()
    {
        this.open = true;
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(OpenAnimation()); //一応Destroyしないでとっておこう
    }

    IEnumerator OpenAnimation()
    {
        for (int i = 0; i < 1000; i++) //カーソルから離れるような挙動のほうが面白いかも
        {
            float x = pos.x - 4.5f;
            float y = pos.y - 4.5f;
            float d = Mathf.Sqrt(x * x + y * y);
            d = (7 - d) / 32;
            this.transform.position = new Vector3(this.transform.position.x + x * d, this.transform.position.y + y * d, this.transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
