using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //메뉴, 게임 카메라
    public GameObject MenuCam;
    public GameObject gameCam;
    //플레이어
    public PlayerCont player;
    //보스 
    public Boss boss;
    public SecondBoss secondboss;
    //레이저라인
    public LaserLine laserline;
    //아이템, 무기 상점
    public GameObject itemShop;
    public GameObject weaponShop;
    //스타트존 리스트
    public GameObject[] startZones = new GameObject[3];
    //스테이지
    public int stage;
    //플레이타임
    public float playTime;
    //배틀중인지 bool값
    public bool isBattle;
    //적 갯수 확인 리스트
    public List<int> enemyCounts;
    //적 생성 위치 리스트
    public Transform[] enemyZones;
    //생성할 적 리스트
    public GameObject[] enemies;
    //적 생성 번호 리스트
    public List<int> enemyList;
    //메뉴, 게임, 게임오버 패널
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    //0.최고점수MaxScore, 1.점수Score, 2.스테이지Stage, 3.플탐time, 4.체력Health, 5.총알갯수Ammo, 6.코인Coin
    public Text[] uiTexts = new Text[7];
    //무기1,2,3,R 이미지
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    //에너미 A,B,C 텍스트
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    //보스 체력 UI 위치
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    //베스트 갱신점수 텍스트
    public Text curScoreText;
    public Text bestText;
    //아이템 생성 위치
    public Transform Itemposition;
    //아이템 1,2,3 옵젝
    public GameObject Item1;
    public GameObject Item2;
    public GameObject Item3;
    //적 Hp 늘리기
    int HPplus;
    //ESC, 보스패널 UI 옵젝
    public GameObject ESCMenuSet;
    public GameObject bossPanel;
    //오디오 소스 매니저
    [System.Serializable]
    public class AudioSourceManager
    {
        public string SoundName;
        public AudioClip Audio;
    }
    //오디오 리스트
   public List<AudioSourceManager> AudioList = new List<AudioSourceManager>();
    AudioSource Audio;

    private void Awake()
    {
        enemyList = new List<int>(); //적 갯수 리스트
        uiTexts[0].text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));  //string.Format 함수로 문자열 양식 적용
        Audio = GetComponent<AudioSource>(); //오디오 컴포넌트 가져오기
    }
    public void GameStart() //카메라, 패널, 플레이어
    {
        //Audio.clip = AudioList[0].Audio;
        MenuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        
    }
    
    public void GameOver() //패널, 점수텍스트
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = uiTexts[1].text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore) //최고 점수라면 갱신, 유니티 기본 제공 플레이어 프리펩 클래스에서 가져오기
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }
    public void ReStart()//재시작 로드씬
    {
        SceneManager.LoadScene(0);
    }
    
    public void StageStart()//상점, 스타트존, isBattle, InBattle
    {       
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZones[0].SetActive(false);
        startZones[1].SetActive(false);
        startZones[2].SetActive(false);
        
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());      
    }
    public void StageEnd()//플레이어 위치, 상점, 스타트존, isBattle, 스테이지++
    {
        
        player.transform.position = new Vector3(-9f, 0f, 71.6f); //플레이어 원위치

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZones[0].SetActive(true);
        startZones[1].SetActive(true);
        startZones[2].SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;      
    }
    IEnumerator BossPanel()//보스패널
    {
        bossPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        bossPanel.SetActive(false);
    }
    IEnumerator InBattle()//배틀중이면
    {       
        if (stage == 6)
        {

        }
        if (stage == 11)
        {

        }
        if (stage == 16)
        {

        }
        if (stage == 5)//첫 번째 보스
        {
            StartCoroutine(BossPanel());
            enemyCounts[3]++;
            yield return new WaitForSeconds(2f);
            GameObject instantEnemy = Instantiate(enemies[3],
                                                      enemyZones[0].position,
                                                      enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
            enemy.maxhealth += HPplus;
            enemy.curhealth += HPplus;
            HPplus += 50;
        }
        if (stage == 10)//두 번째 보스
        {
            enemyCounts[4]++;
            yield return new WaitForSeconds(2f);
            GameObject instantEnemy = Instantiate(enemies[4],
                                      enemyZones[0].position,
                                      enemyZones[0].rotation);
            GameObject instantEnemy1 = Instantiate(enemies[5],
                                      enemyZones[0].position,
                                      enemyZones[0].rotation);
            //드론 Enemy, SecondBoss 스크립트
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            secondboss = instantEnemy.GetComponent<SecondBoss>();
            //레이저볼 LaserBall 스크립트
            LaserBall laserball = instantEnemy1.GetComponent<LaserBall>();
            //게임매니저 laserline 스크립트
            laserline = instantEnemy.GetComponentInChildren<LaserLine>();
            //레이저볼 laserline 스크립트
            laserball.laserline = instantEnemy.GetComponentInChildren<LaserLine>();



            enemy.manager = this;
                    
            enemy.maxhealth += HPplus;
            enemy.curhealth += HPplus;
            HPplus += 50;   
            instantEnemy.SetActive(true);
            instantEnemy1.SetActive(true); //옵젝 활성화
            laserball.enabled = true; //컴포넌트 활성화
            //laserball.target = player.transform; //이건 레이저볼 타겟임 에너미 타겟X
        }
        
        else
        {          
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCounts[0]++;
                        break;
                    case 1:
                        enemyCounts[1]++;
                        break;
                    case 2:
                        enemyCounts[2]++;
                        break;
                }
            }
            while (enemyList.Count > 0) //지속적인 몬스터 소환
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]],
                                                      enemyZones[ranZone].position,
                                                      enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0); //생성 후에는 사용된 데이터는 RemoveAt() 함수로 삭제
                yield return new WaitForSeconds(4f); //안전하게 while문을 돌리기 위해선 꼭 yield return 포함
            }
            
        }

        while (enemyCounts[0] + enemyCounts[1] + enemyCounts[2] + enemyCounts[3] + enemyCounts[4] > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(4f);


        boss = null;
        StageEnd();       
    }
    private void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
        // ESC 버튼      
        if (Input.GetKeyDown(KeyCode.Escape)) //ESC 바꾸려면 GetButtonDown
        {
            if (ESCMenuSet.activeSelf)
                ESCMenuSet.SetActive(false);
            else
                ESCMenuSet.SetActive(true);
        }       
    }
    //저장은 게임 마무리 작업 떄
    public void GameExit()
    {
        Application.Quit();
    }
    private void LateUpdate() //Ui 안성맞춤 LateUpdate
    {
        //상단 UI
        uiTexts[1].text = string.Format("{0:n0}", player.score);
        uiTexts[2].text = "STAGE" + stage;

        int hour = (int)(playTime / 3600); //초단위 시간을 3600, 60으로 나누어 시분초로 계산
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        uiTexts[3].text = string.Format("{0:00}", hour) + ":" 
                                        + string.Format("{0:00}", min) + ":" 
                                         + string.Format("{0:00}", second);

        //플레이어 UI
        uiTexts[4].text = player.health + " / " + player.maxHealth;
        uiTexts[6].text = string.Format("{0:n0}", player.coin);
        if (player.equipWeapon == null)
            uiTexts[5].text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            uiTexts[5].text = "- / " + player.ammo;
        else
            uiTexts[5].text = player.equipWeapon.curAmmo + " / " + player.ammo;

        //무기 UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);
        
        //몬스터 숫자 UI
        enemyATxt.text = enemyCounts[0].ToString();
        enemyBTxt.text = enemyCounts[1].ToString();
        enemyCTxt.text = enemyCounts[2].ToString();

        //보스 체력 UI 이미지의 scale을 남을 체력 비율에 따라 변경
        //보스 변수가 비어있을 때 UI업데이트 하지 않도록 조건 추가    
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curhealth / boss.maxhealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;          
        }           
    }
}
