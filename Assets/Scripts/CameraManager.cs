using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    float scale = 0.8f;

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraAnimation());
    }

    IEnumerator ShakeCameraAnimation()
    {
        Vector3 defPos = this.transform.position;
        for (int i = 0; i < 25; i++)
        {
            this.transform.position = defPos + new Vector3(scale * Random.Range(-1f, 1f), scale * Random.Range(-1f, 1f), 0) * (25 - i) / 25;

            yield return null;
        }
        this.transform.position = defPos;
    }
}
