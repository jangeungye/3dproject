using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone3 : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(manager.Item3, manager.Itemposition.position, manager.Itemposition.rotation);           
            manager.StageStart();
        }           
    }
}
