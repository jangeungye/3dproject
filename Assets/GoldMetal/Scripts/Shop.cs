using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj; //아이템 프리펩, 가격, 위치 변수 생성
    public int[] itemPrice;
    public Transform[] itemPos;
    public string[] talkData;
    public Text talkText; //금액 부족을 알려주기 위해서 대사 텍스트도 변수에 저장

    PlayerCont enterPlayer;
    // Start is called before the first frame update
    public void Enter(PlayerCont player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; //화면 정중앙
    }

    // Update is called once per frame
    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin) //금액이 부족하면 return으로 구입로직 건너뛰기
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                       + Vector3.forward * Random.Range(-3, 3);

        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f); //코루틴으로 금액 부족 대사 몇초간 띄우기
        talkText.text = talkData[0];
    }
}
