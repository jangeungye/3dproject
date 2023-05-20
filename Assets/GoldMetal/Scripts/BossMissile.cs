using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;
    // Monobehaviour를 Bullet으로 교체하여 상속하기
    //스크립트를 상속하면 변수와 함수를 그대로 유지하면서 로직 추가 가능
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.position); // 추적
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
            yield return new WaitForSeconds(5f);
            this.gameObject.SetActive(false);  
    }
}
