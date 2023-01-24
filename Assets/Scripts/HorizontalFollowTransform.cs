using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalFollowTransform : MonoBehaviour
{
    [SerializeField] private Transform followObject;

    void Update()
    {
        transform.position = new Vector3(followObject.position.x, transform.position.y, followObject.position.z);
    }
}
