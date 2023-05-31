using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LaserBall : Bullet
{
    //레이저볼스피드
    public float laserballspeed = 3f;
    //isLaserball true 초기화
    public bool isLaserball = true;
    //목적지
    private Transform target;
    //레이저라인 클래스 가져오기
    public LaserLine laserline;
    //요원
    NavMeshAgent agent;
    //secondboss 클래스 가져오기
    //SecondBoss secondboss;
    void Awake()
    {
        //요원에게 목적지를 알려준다.
        agent = GetComponent<NavMeshAgent>();
        //agent.destination = target.transform.position;
    }
    Transform playerTrans; //플레이어 위치
    private void Start()
    { 
        StartCoroutine(isLaser()); //이거 없애면 제자리로 감
        //secondboss = FindObjectOfType<SecondBoss>(); //SecondBoss컴포넌트를 가진 드론을 찾아 변수를 할당함
        GameObject obj  = GameObject.Find("Player"); //생성될때 목적지(Player)를 찾는다
        playerTrans = obj.transform; //위치 할당
    }
    private void Update()
    {
        //레이저볼의 포지션과 레이저라인의 플레이어 포지션 x축z축 비교해서 isLaserball이 true일 때 
        if (isLaserball == true && 
            transform.localPosition.x + 0.5f > laserline.LookPlayer.x && 
            transform.localPosition.x - 0.5f < laserline.LookPlayer.x && 
            transform.localPosition.z + 0.5f > laserline.LookPlayer.z && 
            transform.localPosition.z - 0.5f < laserline.LookPlayer.z)
        {
            isLaserball = false; //false로 만들고 멈춤
            laserline.LookPlayer.x = 9999;
            laserline.LookPlayer.z = -9999;
        }
        else if (laserline.islook == true)
        {
            chu();           
        }
        if (isLaserball == false)
        {
            laserballspeed = 3f; 
        }
    }
    IEnumerator isLaser() //초반 2초 navmash로 거리 쟀다가 추적할 의도로 썼는데 잘 안됨
    {
        agent.stoppingDistance = 15.0f; // 원하는 거리를 설정합니다      
        yield return new WaitForSeconds(2f);
        agent.enabled = false;   
    }
    void chu()
    {
        if (isLaserball) //플레이어 위치 추적 이동
        {
            transform.position = Vector3.Lerp(transform.position, playerTrans.position, laserballspeed * Time.deltaTime);
            //laserballspeed += Time.deltaTime * 2f;
        }            
    }    
}
