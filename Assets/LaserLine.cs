using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserLine : MonoBehaviour
{

    public Transform PlayerTransform;
    [SerializeField]
    LaserBall laserBall;
    [SerializeField]
    Transform SecondBoss;
    public float dsahu;
    public Vector3 LookPlayer;
    public bool islook = true;

    SecondBoss secondboss;
    public Enemy enemy;
    public GameManager manager;
    private void Start()
    {
        laserBall = FindObjectOfType<LaserBall>();
        secondboss = FindObjectOfType<SecondBoss>();
        PlayerTransform = GameObject.Find("Player").transform;
        enemy = FindObjectOfType<Enemy>();
        manager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        if (enemy.isDead == false && manager.stage == 10)
        {


            RaycastHit hit;
            transform.LookAt(PlayerTransform.position); //바라보기

            Vector3 targetPosition = PlayerTransform.position; // 대상 게임 오브젝트의 위치
            Vector3 currentPosition = this.gameObject.transform.position; // 현재 게임 오브젝트의 위치
            targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
            currentPosition = new Vector3(currentPosition.x, 0, currentPosition.z);

            float distance = Vector3.Distance(targetPosition, currentPosition); // 대상과의 거리 계산
            dsahu = distance;
            if (Physics.Raycast(transform.position, transform.forward, out hit) && !hit.collider.gameObject.CompareTag("Player")) //충돌 객체 레이캐스트로 정보 얻기
            {

                LookPlayer = PlayerTransform.transform.localPosition;
                laserBall.isLaserball = false;
                laserBall.gameObject.SetActive(false);
                laserBall.gameObject.transform.localPosition = new Vector3(0, 0, 96.1f);

                secondboss.lr.enabled = false;
                StartCoroutine(Rest());
            }
            else if (distance <= 150f)
            {
                laserBall.isLaserball = true;
                laserBall.gameObject.SetActive(true);

                secondboss.lr.enabled = true;
                Vector3 dronePosition = new Vector3(SecondBoss.position.x, 20, SecondBoss.position.z);
                SecondBoss.position = Vector3.Lerp(transform.position, dronePosition, 1f * Time.deltaTime);
            }
            else
            {
                laserBall.isLaserball = false;
                laserBall.gameObject.SetActive(false);
                laserBall.gameObject.transform.localPosition = new Vector3(0, 0, 96.1f);

                secondboss.lr.enabled = false;
                StartCoroutine(Rest());
            }

            Debug.DrawLine(transform.position, hit.point);
        }
    }
    IEnumerator Rest()
    {
        yield return new WaitForSeconds(1f);
        Vector3 dronePosition = new Vector3(SecondBoss.position.x, 5, SecondBoss.position.z);
        SecondBoss.position = Vector3.Lerp(transform.position, dronePosition, 0.5f * Time.deltaTime);
    }
}
