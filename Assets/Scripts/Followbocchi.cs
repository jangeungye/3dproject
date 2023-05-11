using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Followbocchi : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private bool isCamerastop;
    [SerializeField] private GameObject targetGameObject;

    public GameObject Camera;
    public GameObject Box;
    private float Dist;
    void Update()
    {
        Dist = Vector3.Distance(Camera.transform.position, Box.transform.position);
        if (Dist < 20)
        {
            isCamerastop = true;
        }
    }
    void FixedUpdate()
    {
        if (isCamerastop == false)
        {
            transform.position = target.position + offset;
        }
        else
        {
            transform.LookAt(targetGameObject.transform);
        }
    }
    void LateUpdate()
    {
        print(Dist);
    }

}
