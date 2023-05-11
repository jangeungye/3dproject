using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCont : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons; //�÷��̾� ������� �迭 �Լ�1
    public bool[] hasWeapons; //�÷��̾� ������� �迭 �Լ�2
    public GameObject[] grenades; //�÷��̾� ������� �迭 �Լ�3
    public int hasGrenades; //�÷��̾� ������� �迭 �Լ�4
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

    GameObject nearObject; //��ó ���� ����
    Weapon equipWeapon; //���� ������Ʈ ����
    int equipWeaponIndex = -1; //���� ��ȣ ���� �ʱⰪ -1
    float fireDelay; //���ݵ�����
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput(); //Ű�Է��Լ�
        Move(); //�������Լ�
        Turn(); //ȸ���Լ�
        Jump(); //�����Լ�
        Attack(); //�����Լ�
        Dodge(); //ȸ���Լ�
        Interation(); //��ȣ�ۿ��Լ�
        Swap(); //���ⱳü�Լ�
        Reload(); //�������Լ�
    }
    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal"); //���������
        vAxis = Input.GetAxis("Vertical"); //����������
        wDown = Input.GetButton("Walk"); //�ȱ� ��ư ����Ʈ
        jDown = Input.GetButtonDown("Jump"); //���� ��ư �����̽���
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload"); //������
        iDown = Input.GetButtonDown("Interation"); //��ȣ�ۿ� ��ư E
        sDown1 = Input.GetButtonDown("Swap1"); //����1
        sDown2 = Input.GetButtonDown("Swap2"); //����2
        sDown3 = Input.GetButtonDown("Swap3"); //����3
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
            transform.position += movevec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; //���׿����� ����Ʈ ������ 0.3 �ƴϸ� 1

        anim.SetBool("isRun", movevec != Vector3.zero); //������ ��
        anim.SetBool("isWalk", wDown); //���� ��

        
    }
    void Turn()
    {
        //#1.Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + movevec); //LookAt ������ ���͸� ���ؼ� ȸ�������ִ� �Լ�

        //#2.���콺�� ���� ȸ��
        //ray ���콺�������� rayhit�� ���� Physics.Raycast�� �浹����
        //RayCastHit�� ���콺 Ŭ�� ��ġ Ȱ���Ͽ� ȸ���� ����
        //out : ����ó�� ��ȯ���� �־��� ������ �����ϴ� Ű����
        //100�� ray�� ����
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
        if (jDown && movevec == Vector3.zero && !isJump && !isDodge && !isSwap) //�������� ���� ��(�׼�X) ���� 
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //Impulse �ﰢ���� �� �ֱ�
            anim.SetBool("isJump", true);
            anim.SetTrigger("DoJump");
            isJump = true;
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime; //���ݵ����̿� �ð��� �����ְ�
        isFireReady = equipWeapon.rate < fireDelay; //���ݰ��� ���θ� Ȯ��
        //�����Ҷ��� ���ݰ���
        if (fDown && isFireReady && !isDodge && !isSwap) {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //���� Ÿ�Կ� ���� Ʈ���� ����
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
        if (jDown && movevec != Vector3.zero && !isJump && !isDodge && !isSwap) //�����̰� ���� �� ȸ��
        {
            doDodgevec = movevec; // ȸ���� �� ���⺤�ͷ� �ٲٱ�
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); //Invoke�� �ð��� �Լ� ȣ��
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
        if(iDown && nearObject != null &&  !isJump && !isDodge) //������Ʈ ��ó ��ȣ�ۿ�
        {
            if (nearObject.CompareTag("Weapon")) //���� �±�
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //���� ���� ���� ��������
                hasWeapons[weaponIndex] = true; //��� ���� �Լ� üũ

                Destroy(nearObject); //�ı�
            }
        }
    }

    void FreezeRotation()
    {
        
        rigid.angularVelocity = Vector3.zero; //angularVelocity : ���� ȸ�� �ӵ�
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
        if(collision.gameObject.CompareTag("Floor")) //�ٴڰ���
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other) //Item �±׸� ���� ���� �����̶�� Item������Ʈ �ο� & �ִ񰪱��� ȹ��
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
        if (other.CompareTag("Weapon")) //���� ����
        {
            nearObject = other.gameObject;

            print(nearObject.name);
        }
            
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon")) //���Ⱘ�� ��ó��
        {
            nearObject = null;
        }
    }
}