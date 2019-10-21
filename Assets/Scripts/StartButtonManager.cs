using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonManager : MonoBehaviour
{
    public void DestroyOneself()
    {
        this.gameObject.SetActive(false);
    }
}
