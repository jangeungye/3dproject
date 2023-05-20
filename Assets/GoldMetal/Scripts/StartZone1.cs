using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone1 : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(manager.Item1, manager.Itemposition.position, manager.Itemposition.rotation);           
            manager.StageStart();
        }           
    }
}
