using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCont : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons; //플레이어 무기관련 배열 함수1
    public bool[] hasWeapons; //플레이어 무기관련 배열 함수2
    public GameObject[] grenades; //플레이어 무기관련 배열 함수3
    public int hasGrenades; //플레이어 무기관련 배열 함수4
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;
    

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;


    float hAxis;
    float vAxis;
    
    
    bool wDown;
    bool jDown;
    bool fDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    Vector3 movevec;
    Vector3 doDodgevec;

    Animator anim;
    Rigidbody rigid;

    GameObject nearObject; //근처 무기 감지
    Weapon equipWeapon; //무기 오브젝트 장착
    int equipWeaponIndex = -1; //무기 번호 저장 초기값 -1
    float fireDelay; //공격딜레이
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput(); //키입력함수
        Move(); //움직임함수
        Turn(); //회전함수
        Jump(); //점프함수
        Attack(); //공격함수
        Dodge(); //회피함수
        Interation(); //상호작용함수
        Swap(); //무기교체함수
        Reload(); //재장전함수
    }
    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal"); //수평움직임
        vAxis = Input.GetAxis("Vertical"); //수직움직임
        wDown = Input.GetButton("Walk"); //걷기 버튼 쉬프트
        jDown = Input.GetButtonDown("Jump"); //점프 버튼 스페이스바
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload"); //재장전
        iDown = Input.GetButtonDown("Interation"); //상호작용 버튼 E
        sDown1 = Input.GetButtonDown("Swap1"); //무기1
        sDown2 = Input.GetButtonDown("Swap2"); //무기2
        sDown3 = Input.GetButtonDown("Swap3"); //무기3
    }
    void Move()
    {
        movevec = new Vector3(hAxis, 0, vAxis).normalized;
        if (isDodge)
        {
            movevec = doDodgevec;
        }
        if(isSwap || isReload || !isFireReady)
        {
            movevec = Vector3.zero;
        }
        if (!isBorder)
            transform.position += movevec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; //삼항연산자 쉬프트 누르면 0.3 아니면 1

        anim.SetBool("isRun", movevec != Vector3.zero); //움직일 때
        anim.SetBool("isWalk", wDown); //걸을 때

        
    }
    void Turn()
    {
        //#1.키보드에 의한 회전
        transform.LookAt(transform.position + movevec); //LookAt 지정된 벡터를 향해서 회전시켜주는 함수

        //#2.마우스에 의한 회전
        //ray 마우스포지션을 rayhit에 저장 Physics.Raycast로 충돌감지
        //RayCastHit의 마우스 클릭 위치 활용하여 회전을 구현
        //out : 리턴처럼 반환값을 주어진 변수에 저장하는 키워드
        //100은 ray의 길이
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextvec = rayHit.point - transform.position;
                nextvec.y = 0;
                transform.LookAt(transform.position + nextvec);
            }
        }
        
    }

    void Jump()
    {
        if (jDown && movevec == Vector3.zero && !isJump && !isDodge && !isSwap) //움직이지 않을 때(액션X) 점프 
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //Impulse 즉각적인 힘 주기
            anim.SetBool("isJump", true);
            anim.SetTrigger("DoJump");
            isJump = true;
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime; //공격딜레이에 시간을 더해주고
        isFireReady = equipWeapon.rate < fireDelay; //공격가능 여부를 확인
        //점프할때도 공격가능
        if (fDown && isFireReady && !isDodge && !isSwap) {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //무기 타입에 따라 트리거 실행
            fireDelay = 0;
        }
        
    }

    void Reload()
    {
        if (equipWeapon == null)    
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady) 
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f);
        }
            
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }
    void Dodge()
    {
        if (jDown && movevec != Vector3.zero && !isJump && !isDodge && !isSwap) //움직이고 있을 때 회피
        {
            doDodgevec = movevec; // 회피할 때 방향벡터로 바꾸기
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); //Invoke로 시간차 함수 호출
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;


        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
                
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }
    void Interation()
    {
        if(iDown && nearObject != null &&  !isJump && !isDodge) //오브젝트 근처 상호작용
        {
            if (nearObject.CompareTag("Weapon")) //무기 태그
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //무기 숫자 정보 가져오기
                hasWeapons[weaponIndex] = true; //헤당 무기 입수 체크

                Destroy(nearObject); //파괴
            }
        }
    }

    void FreezeRotation()
    {
        
        rigid.angularVelocity = Vector3.zero; //angularVelocity : 물리 회전 속도
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Floor")) //바닥감지
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other) //Item 태그를 가진 무기 유형이라면 Item컴포넌트 부여 & 최댓값까지 획득
    {
        if (other.CompareTag("item"))
        {
            Item item = other.GetComponent<Item>();
            switch (item.type) {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    if (hasGrenades == maxHasGrenades)
                        return;
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon")) //무기 감지
        {
            nearObject = other.gameObject;

            print(nearObject.name);
        }
            
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon")) //무기감지 끝처리
        {
            nearObject = null;
        }
    }
}
