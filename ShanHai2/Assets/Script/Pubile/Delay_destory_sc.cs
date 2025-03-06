using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delay_destory_sc : MonoBehaviour
{

    public float delay_time = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delay_destory(delay_time));
    }

    IEnumerator delay_destory(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
