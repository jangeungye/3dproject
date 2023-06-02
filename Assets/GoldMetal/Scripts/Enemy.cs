using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D, E}; // enum으로 타입을 나누고 그것을 지정할 변수를 생성
    public Type enemyType;
    public int maxhealth;
    public int curhealth;
    public int score;
    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea; //공격범위 변수
    public GameObject bullet;
    public GameObject[] coins;
    public bool isChase; //추적을 결정하는 변수
    public bool isAttack;
    public bool isDead;
    

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs; //피격 이펙트를 플레이어처럼 모든 메테리얼로 변경
    public NavMeshAgent nav;
    public Animator anim;
    public RectTransform[] enemyHealthBars = new RectTransform[3];
    //NavMesh : NavAgent가 경로를 그리기 위한 바탕(Mesh) Static 오브젝트만 Bake 가능
    private void Awake() //Awake함수는 자식 스크립트만 함수 실행
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.D && enemyType != Type.E)
        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void Update()
    {
        if (nav.enabled && enemyType != Type.D && enemyType != Type.E)
        {
            nav.SetDestination(target.position); //SetDestination() : 도착할 목표 위치 지정 함수
            nav.isStopped = !isChase; //isStopped를 사용하여 완벽하게 멈추도록 작성
        }                        
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; //angularVelocity : 물리 회전 속도
        }
        
    }
    void Targerting()
    {
        if (!isDead && enemyType != Type.D && enemyType != Type.E)
        {
            float targetRadius = 0f; //SphereCast ()의 반지름, 길이를 조정할 변수 선언
            float targetRange = 0f;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;

            }

            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                                      targetRadius,
                                      transform.forward,
                                      targetRange,
                                      LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack) //rayHit 변수에 데이터가 들어오면 코루틴 실행
            {
                StartCoroutine(Attack());
            }
        }
        
    }

    IEnumerator Attack()
    {
        isChase = false; //먼저 정지를 한 다음, 애니메이션과 함께 공격범위 활성화
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.1f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
     

        }
       
        isChase = true; 
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melee"))
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curhealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;           
            StartCoroutine(OnDamage(reactVec, false));
            
            
        }
        else if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curhealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec, true));
            
        }

    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curhealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)       
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curhealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else if (curhealth <= 0 && !isDead)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
                
            gameObject.layer = 14;
            isDead = true;
            isChase = false;
            if (manager.stage != 10)//드론은 X
            {
                nav.enabled = false; //사망 리액션을 유지하기 위해 NavAgent를 비활성
                anim.SetTrigger("doDie");

                int ranCoin = Random.Range(0, 3);
                Instantiate(coins[ranCoin], transform.position, Quaternion.identity);
            }
            GameObject obj = GameObject.Find("Player"); //생성될때 목적지(Player)를 찾는다
            target = obj.transform; //위치 할당
            PlayerCont player = target.GetComponent<PlayerCont>();
            player.score += score;
            
            
                switch (enemyType)
            {
                case Type.A:
                    manager.enemyCounts[0]--;
                    break;
                case Type.B:
                    manager.enemyCounts[1]--;
                    break;
                case Type.C:
                    manager.enemyCounts[2]--;
                    break;
                case Type.D:
                    manager.enemyCounts[3]--;
                    break;
                case Type.E:
                    manager.enemyCounts[4]--;
                    break;
            }
            
            

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse); //AddForce 함수로 넉백 구하기
                rigid.AddTorque(reactVec * 5, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse); //AddForce 함수로 넉백 구하기
            }
            Destroy(gameObject, 4);           
        }

    }
    private void LateUpdate() //오류 때문에 잠시 주석처리
    {
        switch (enemyType)
        {
            case Type.A:
                enemyHealthBars[0].localScale = new Vector3((float)curhealth / maxhealth, 1, 1);
                break;
            case Type.B:
                enemyHealthBars[1].localScale = new Vector3((float)curhealth / maxhealth, 1, 1);
                break;
            case Type.C:
                enemyHealthBars[2].localScale = new Vector3((float)curhealth / maxhealth, 1, 1);
                break;
        }
    }
}
