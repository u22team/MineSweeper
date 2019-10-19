using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperManager : MonoBehaviour
{
    public bool open = false;
    public bool flag = false;

    public GameObject gameObj;
    public Vector2Int pos;
    public SpriteRenderer sprren;

    public void instantiate(Vector2Int _vc)
    {
        gameObj = this.gameObject; //今の所使う予定はない
        pos = _vc;
        sprren = gameObj.GetComponent<SpriteRenderer>();
    }

    public void Flag()
    {
        if (!open)
        {
            flag = !flag;
            sprren.sprite = flag ? ResourcesManager.sprFlag : ResourcesManager.sprDef;
        }
    }

    public void PlanToOpen()
    {
        this.open = true;
    }

    public void Open()
    {
        this.open = true;
        StartCoroutine(OpenAnimation());
    }

    IEnumerator OpenAnimation() //一応Destroyしないでとっておこう
    {
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        for (int i = 0; i < 1000; i++) //カーソルから離れるような挙動のほうが面白いかも
        {
            float x = pos.x - 4.5f;
            float y = pos.y - 4.5f;
            float d = Mathf.Sqrt(x * x + y * y);
            d = (7 - d) / 32;
            //this.transform.position = new Vector3(this.transform.position.x + x * d, this.transform.position.y + y * d, this.transform.position.z);
            this.transform.position = new Vector3(this.transform.position.x + 100, this.transform.position.y, this.transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
