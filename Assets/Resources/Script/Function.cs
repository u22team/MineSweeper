using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function : MonoBehaviour
{
    protected class SomeIEnumerator
    {
        int wait_ienumerator_num;
        MonoBehaviour mono;

        public SomeIEnumerator(MonoBehaviour this_)
        {
            mono = this_;
        }

        public IEnumerator StartCoroutine(params IEnumerator[] enumerators)
        {
            if(wait_ienumerator_num != 0)
            {
                Debug.LogError("複数に使うことは出来ません。");
            }
            foreach (IEnumerator enumerator in enumerators)
            {
                mono.StartCoroutine( StartCoroutine_(enumerator));
            }
            while (wait_ienumerator_num < enumerators.Length - 1)
            {
                yield return null;
            }
            wait_ienumerator_num = 0;
        }
        IEnumerator StartCoroutine_(IEnumerator enumerator)
        {
            yield return mono.StartCoroutine(enumerator);
            wait_ienumerator_num++;
        }
    }
    protected  IEnumerator Wait(float time)
    {
        while(time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
        }
    }

}
