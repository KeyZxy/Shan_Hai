using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class born_detection_sc : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> born_obj = new List<GameObject>();

    private bool isAct = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isAct)
            return;
        if (other.CompareTag(SaveKey.Character))
        {
            foreach (GameObject obj in born_obj)
            {
                obj.GetComponent<Enemy_born_sc>().start_born();
                isAct = true;
            }

        }
    }

}
