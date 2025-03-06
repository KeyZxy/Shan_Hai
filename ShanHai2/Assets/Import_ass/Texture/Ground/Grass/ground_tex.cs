using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ground_tex : MonoBehaviour
{
    public Material material;
    private Camera the_camera;

    private int frameIndex;

    // Start is called before the first frame update
    void Start()
    {   
        the_camera = GetComponent<Camera>();
        //只需激活一帧
        the_camera.enabled = true;
        Vector2 camSize = new Vector2(the_camera.orthographicSize * 2 / 9 * 16, the_camera.orthographicSize * 2);
        material.SetVector("_camerapos", transform.position);
        material.SetVector("_camerasize", camSize);
        frameIndex = 0;
    }

    // Update is called once per frame

    private void Update()
    {
        if (frameIndex > 0){
            the_camera.targetTexture = null;
            the_camera.enabled = false;
        }

        frameIndex++;
        
    }

}
