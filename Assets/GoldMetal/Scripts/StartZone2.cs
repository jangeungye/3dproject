using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone2 : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(manager.Item2, manager.Itemposition.position, manager.Itemposition.rotation);           
            manager.StageStart();
        }           
    }
}
