using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay1: MonoBehaviour
{
    public AK.Wwise.Event foot;
    public AK.Wwise.Event cloth;

    public void PlayFootSound()
    {
        foot.Post(gameObject);
    }

    public void PlayClothSound()
    {
        cloth.Post(gameObject);
    }
}
