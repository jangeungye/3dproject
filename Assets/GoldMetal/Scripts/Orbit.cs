using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //플레이어
    public float orbitspeed;
    Vector3 offset;


    void Start()
    {
        offset = transform.position - target.position; 
    }

    void Update()
    {
        transform.position = target.position + offset; //플레이어와의 거리 차
        transform.RotateAround(target.position, Vector3.up, orbitspeed * Time.deltaTime); //RotateAround()는 목표가 움직이면 일그러지는 단점이 있음
    }
}
