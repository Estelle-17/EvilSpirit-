/*
* Writer : KimJunWoo
*
* 이 소스코드는 업그레이드 신단 트리거에 대한 내용이 작성되어 있음
*
* Last Update : 2024/03/03
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTrigger : MonoBehaviour
{
    public GameObject InformationUI;
    public GameObject DamageUpUI;
    public Image DamageUpPressImage;
    public GM GameMaster;
    public GameObject Target;
    public GameObject UI;

    public float cnt;
    private bool isPlayerPortalIn;
    public bool PlayingFadeInOut;
    public bool ButtonDown;
    private bool isDamageUp;

    void Start() {
        GameMaster = GameObject.Find("GM").GetComponent<GM>();
        UI = GameObject.Find("UI");
        isPlayerPortalIn = false;
        PlayingFadeInOut = false;
        isDamageUp = false;
        ButtonDown = false;
        cnt = 0;
    }

    void Update() {
        //트리거 내에 플레이어가 있을 시 일정량의 포인트를 사용해 업그레이드 가능
        if (isPlayerPortalIn && Input.GetButtonDown("Get")) {
            if (!PlayingFadeInOut && Target.transform.root.GetChild(0).GetComponent<Stats>().GhostPoint >= 300) {
                Target.transform.root.GetChild(0).GetComponent<Stats>().GhostPoint -= 300;
                PlayingFadeInOut = true;
                StartCoroutine(GameMaster.GetComponent<StatSelectScript>().FadeIn(this));
            }
        }
        if (isPlayerPortalIn && Input.GetButtonDown("Upgrade")) {
            ButtonDown = true;
        }else if (Input.GetButtonUp("Upgrade")) {
            ButtonDown = false;
            isDamageUp = false;
            cnt = 0;
            DamageUpPressImage.fillAmount = cnt;
        }
        //일정 시간 데미지 증가 버튼을 눌렀을 시 플레이어의 데미지가 증가함
        //플레이어가 데미지를 증가시킨 횟수에 비례하여 사용해야하는 포인트 양이 증가함
        if (ButtonDown) {
            cnt += Time.deltaTime;
            DamageUpPressImage.fillAmount = cnt;
            if (cnt >= 1 && !isDamageUp) {
                //일정량의 포인트가 있을 시 업그레이드 가능
                if (Target.transform.root.GetChild(0).GetComponent<Stats>().GhostPoint >= 50 * Target.transform.root.GetChild(0).GetComponent<Stats>().Level) {
                Target.transform.root.GetChild(0).GetComponent<Stats>().GhostPoint -= 50 * Target.transform.root.GetChild(0).GetComponent<Stats>().Level;

                Target.transform.root.GetChild(0).GetComponent<Stats>().Level++;
                Target.transform.root.GetChild(0).GetComponent<Stats>().LevelUpStats();
                UI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = "" + Target.transform.root.GetChild(0).GetComponent<Stats>().Level + "";
                DamageUpUI.transform.GetChild(0).GetComponent<Text>().text = "데미지 증가 및 회복 충전 - 혼" + Target.transform.root.GetChild(0).GetComponent<Stats>().Level * 50 + "개\n";
                Target.transform.root.GetChild(0).GetComponent<Stats>().UpgradeEffect.Play();
                isDamageUp = true;
                ButtonDown = false;
                cnt = 0;
                DamageUpPressImage.fillAmount = cnt;
            }
            }
        }
    }

    //신단 앞 지정된 자리에 서있을 시 보여지는 UI내용
    void OnTriggerEnter(Collider col) {
        Debug.Log(col.gameObject.tag + " In");
        if (col.gameObject.tag == "Player") {
            Target = col.gameObject;
            isPlayerPortalIn = true;
            InformationUI.transform.GetChild(0).GetComponent<Text>().text = "제단을 활성화해서 스킬 업그레이드 - 혼300개";
            DamageUpUI.transform.GetChild(0).GetComponent<Text>().text = "데미지 증가 및 회복 충전 - 혼" + Target.transform.root.GetChild(0).GetComponent<Stats>().Level * 50 + "개\n";
            DamageUpUI.SetActive(true);
            InformationUI.SetActive(true);
        }
    }

    //신단 앞 지정된 자리에 있다가 벗어났을 때 UI내용을 지워줌
    private void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Player") {
            isPlayerPortalIn = false;
            DamageUpUI.SetActive(false);
            InformationUI.SetActive(false);
        }
    }
}