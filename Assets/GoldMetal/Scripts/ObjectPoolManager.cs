using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>//싱글톤 객체 받아와서 관리함
{
    public GameObject BulletPrefab = null; //객체를 저장시킬 변수 지정
    public GameObject CasePrefab = null;

    public Queue<GameObject> Bullet_queue = new Queue<GameObject> ();//객체를 저장시킬 풀(큐) 생성
    public Queue<GameObject> Case_queue = new Queue<GameObject>();//객체를 저장시킬 풀(큐) 생성

    private void Start()//객체를 생성한 뒤, 풀(큐)에 저장
    {
        //총알
        for(int i = 0; i < 100;  i++)
        {
            GameObject t_Object = Instantiate(BulletPrefab, Vector3.zero, Quaternion.identity);
            Bullet_queue.Enqueue(t_Object);
            t_Object.transform.SetParent(transform);
            t_Object.SetActive(false);
        }
        //탄피
        for(int i = 0; i < 100; i++)
        {
            GameObject t_Object = Instantiate(CasePrefab, Vector3.zero, Quaternion.identity);
            Case_queue.Enqueue(t_Object);
            t_Object.transform.SetParent(transform);
            t_Object.SetActive(false);
        }
    }
    //총알

    public void BulletInsertQueue(GameObject p_Object)//사용한 객체를 풀(큐)에 반납시키는 회수
    {
        Bullet_queue.Enqueue(p_Object);
        p_Object.SetActive(false);
    }

    public GameObject BulletGetQueue()
    {
        GameObject t_Object = Bullet_queue.Dequeue();//내보내기
        t_Object.SetActive(true);
        return t_Object;//반환
    }
    // 탄피
    public void CaseInsertQueue(GameObject p_Object)//사용한 객체를 풀(큐)에 반납시키는 회수
    {
        Case_queue.Enqueue(p_Object);
        p_Object.SetActive(false);
    }

    public GameObject CaseGetQueue()
    {
        GameObject t_Object = Case_queue.Dequeue();//내보내기
        t_Object.SetActive(true);
        return t_Object;//반환
    }


}
