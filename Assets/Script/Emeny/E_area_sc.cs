using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_area_sc : MonoBehaviour
{

    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsInsideCollider(GameObject obj)
    {
        if (boxCollider == null || obj == null) return false;

        // 获取物体的位置
        Vector3 objPosition = obj.transform.position;

        // 获取 BoxCollider 的世界坐标 Bounds
        Bounds bounds = boxCollider.bounds;

        // 判断物体是否在 BoxCollider 内部
        return bounds.Contains(objPosition);
    }

}
