using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DontScaleWithParent : MonoBehaviour
{
    private Vector3 parentScale;
    private Vector3 localScale;
    private Vector3 scale;
    
    void Start()
    {
        scale = transform.localScale;
    }
    
    void Update()
    {
        parentScale = transform.parent.localScale;
        localScale = new Vector3(scale.x / parentScale.x, scale.y / parentScale.y, scale.z / parentScale.z);
        transform.localScale = localScale;
    }
}
