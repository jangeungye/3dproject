using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bulletPrefab;
    //public List<GameObject> bullet = new List<GameObject>();
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public AudioSource ShotSound;
    public AudioSource SwingSound;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
            SwingSound.Play();
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--; //현재 탄약을 조건에 추가하고, 발사했을 때 감소하도록 작성
            StartCoroutine("Shot");
            ShotSound.Play();
        }               
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.45f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
    
    IEnumerator Shot()
    {
        print(bulletPrefab);
        print(bulletPrefab.name);
        //ObjectPool objectPool = ObjectPool.Instance;
        //GameObject bullet = objectPool.PopFromPool(bulletPrefab.name);

        //#1.총알 발사

       GameObject BulletObj = ObjectPoolManager.Instance.BulletGetQueue();
        BulletObj.transform.position = bulletPos.position;
        //GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);// 총알 복제
         Rigidbody bulletRigid = BulletObj.GetComponent<Rigidbody>(); //총알 힘
        bulletRigid.velocity = bulletPos.forward * 50;//바라보는 방향 앞에서 발사위치 지정 

        yield return null;//한 프레임 대기 

        //#2.탄피 배출
        GameObject CaseObj = ObjectPoolManager.Instance.CaseGetQueue();
        CaseObj.transform.position = bulletCasePos.position;
        //탄피 힘
        Rigidbody caseRigid = CaseObj.GetComponent<Rigidbody>();
        //인스턴스화 된 탄피에 랜덤한 힘 가하기
        Vector3 casevec = bulletCasePos.forward * Mathf.Lerp(-3, -2, 0) + Vector3.up * Mathf.Lerp(2, 3, 0); 
        caseRigid.AddForce(casevec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); 
    }
}
