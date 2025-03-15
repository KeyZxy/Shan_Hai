using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Grade : MonoBehaviour
{
    // Start is called before the first frame update
    public C_attribute player;
    public Text text;
    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_attribute>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text=player.Get_grade().ToString();
    }
}
