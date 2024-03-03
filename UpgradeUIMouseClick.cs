/*
* Writer : KimJunWoo
*
* 이 소스코드는 마우스로 작동하는 UI 내용이 작성되어 있음
*
* Last Update : 2024/03/03
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeUIMouseClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
    float time;
    bool MouseLeftDown;
    bool IsMouseIn;
    bool IsMouseDown;
    public bool IsReseting;
    bool IsReset;
    bool isDestroy;


    public int ItemNumber;
    public int SkillNum;
    public Transform upgradeUI;
    public Image SelectImage;
    public Image UIEffect;
    public GM gm;
    private Stats playerStats;
    private Attack playerAttack;

    void Start() {
        isDestroy = false;
        MouseLeftDown = false;
        IsMouseIn = false;
        IsMouseDown = false;
        IsReset = false;
        IsReseting = false;
        upgradeUI = transform.parent;
        SelectImage = transform.parent.GetChild(0).GetComponent<Image>();
        UIEffect = transform.parent.GetChild(1).GetComponent<Image>();
        gm = GameObject.Find("GM").GetComponent<GM>();
        playerStats = gm.Player.GetComponent<Stats>();
        playerAttack = gm.Player.GetComponent<Attack>();
        UIEffect.gameObject.SetActive(false);
    }

    void Update() {
        if (isDestroy) {
            Destroy(this.gameObject);
        }
        if(IsReseting)
            SelectImage.gameObject.SetActive(false);
    }
   
    public void OnPointerDown(PointerEventData eventData) {     //왼클릭 확인, 눌린것 확인
        //어떤 버튼인지 확인
         if (eventData.button == PointerEventData.InputButton.Left && !IsReseting) {
            if (ItemNumber == 5 && !IsReset) {
                StartCoroutine(ResetUpgradeMenu());
                IsReset = true;
                IsReseting = true;
            }else if(ItemNumber != 5 && ItemNumber != 6) {
                MouseLeftDown = true;
                UIEffect.gameObject.SetActive(true);
                //스킬 업그레이드
                StartCoroutine(gm.GetComponent<StatSelectScript>().SelectUpgrade(ItemNumber));
                SkillApply();
            }
        }
        IsMouseDown = true;
    }
    //데미지 업 버튼
    public void PowerUp() {
        playerStats.AdditionalDamage += 10;
        playerStats.currentWeapon.GetComponentInChildren<AttackCollider>().SetDamage();
    }
    //업그레이드 종류에 따른 스킬 업그레이드
    public void SkillApply() {
        switch (SkillNum) {
            case 0: //검기 3갈래
                playerStats.SkillLevels[0]++;
                playerAttack.BurstUpgrade1();
                break;
            case 1://검기 연속타격
                playerStats.SkillLevels[1]++;
                playerAttack.BurstUpgrade2();
                break;
            case 2: //검기 데미지 증가
                playerStats.SkillLevels[2]++;
                break;
            case 3: //카운터 시 쿨타임 감소 및 기본 쿨 감소
                playerStats.SkillLevels[3]++;
                gm.Player.GetComponent<PlayerDamageCalculation>().GiveSkillBar = true;
                if (gm.UI.GetComponent<UI>().skillCooltime_4 >= 1.5f) {
                    gm.UI.GetComponent<UI>().currentSkillCooltime_4 -= 0.5f;
                    gm.UI.GetComponent<UI>().skillCooltime_4 -= 0.5f;
                }
                break;
            case 4: // 카운터 데미지 20%중가
                playerStats.SkillLevels[4]++;
                break;
            case 5: //카운터 시 공격횟수 1회증가 중첩당 데미지 25%
                playerStats.SkillLevels[5]++;
                gm.Player.transform.GetChild(0).GetComponent<AttackCollider>().BonusCounterAttack = true;
                break;
            case 6: //스매시 강화
                playerStats.SkillLevels[6]++;
                playerAttack.SmashUpgradeOn = true;
                gm.Player.transform.GetChild(0).GetComponent<AttackCollider>().SmashUpgradeOn = true;
                break;
            case 7:
                playerStats.SkillLevels[7]++;
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData) {   //클릭이 끝날 때
        IsMouseDown = false;
        MouseLeftDown = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {    //지정된 화면에 들어왔을 때
        if (!IsReseting) {
            IsMouseIn = true;
            if (ItemNumber == 5 && !IsReset) {
                SelectImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 525);
                SelectImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 125);
            } else {
                SelectImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
                SelectImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 700);
            }
            if (ItemNumber == 1)
                SelectImage.transform.localPosition = new Vector3(-682, 0, 0);
            else if (ItemNumber == 2)
                SelectImage.transform.localPosition = new Vector3(-232, 0, 0);
            else if (ItemNumber == 3)
                SelectImage.transform.localPosition = new Vector3(218, 0, 0);
            else if (ItemNumber == 4)
                SelectImage.transform.localPosition = new Vector3(668, 0, 0);
            else if (ItemNumber == 5 && !IsReset)
                SelectImage.transform.localPosition = new Vector3(0, -450, 0);

            if (ItemNumber == 5 && IsReset)
                SelectImage.gameObject.SetActive(false);
            else
                SelectImage.gameObject.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData) {     //지정된 화면에 나갔을 때
        IsMouseIn = false;
        SelectImage.gameObject.SetActive(false);
    }
    //자식 오브젝트 순서를 보며 일정시간 선택된 UI로 업그레이드를 진행
    public IEnumerator ResetUpgradeMenu() {
        upgradeUI.transform.GetChild(3).GetComponent<UpgradeUIMouseClick>().IsReseting = true;
        upgradeUI.transform.GetChild(4).GetComponent<UpgradeUIMouseClick>().IsReseting = true;
        upgradeUI.transform.GetChild(5).GetComponent<UpgradeUIMouseClick>().IsReseting = true;
        upgradeUI.transform.GetChild(6).GetComponent<UpgradeUIMouseClick>().IsReseting = true;
        time = 0f;
        float cnt;

        while (time < 1f) {
            time += Time.deltaTime / 0.2f;
            cnt = Mathf.Lerp(1, 0, time);

            upgradeUI.transform.GetChild(3).localScale = new Vector3(1, cnt, 1);
            upgradeUI.transform.GetChild(4).localScale = new Vector3(1, cnt, 1);
            upgradeUI.transform.GetChild(5).localScale = new Vector3(1, cnt, 1);
            upgradeUI.transform.GetChild(6).localScale = new Vector3(1, cnt, 1);
            yield return null;
        }
        time = 0f;
        
        transform.parent.GetComponent<UpgradeUIGM>().SetRandomUpgradeCategory();
        transform.parent.GetComponent<UpgradeUIGM>().setUpgradeResetCategoryUI();

        while (time < 1f) {
            time += Time.deltaTime / 0.2f;
            cnt = Mathf.Lerp(0, 1, time);

            upgradeUI.transform.GetChild(7).localScale = new Vector3(1, cnt, 1);
            upgradeUI.transform.GetChild(8).localScale = new Vector3(1, cnt, 1);
            upgradeUI.transform.GetChild(9).localScale = new Vector3(1, cnt, 1);
            upgradeUI.transform.GetChild(10).localScale = new Vector3(1, cnt, 1);
            yield return null;
        }
        upgradeUI.transform.GetChild(3).GetComponent<UpgradeUIMouseClick>().isDestroy = true;
        upgradeUI.transform.GetChild(4).GetComponent<UpgradeUIMouseClick>().isDestroy = true;
        upgradeUI.transform.GetChild(5).GetComponent<UpgradeUIMouseClick>().isDestroy = true;
        upgradeUI.transform.GetChild(6).GetComponent<UpgradeUIMouseClick>().isDestroy = true;

        upgradeUI.transform.GetChild(7).GetComponent<UpgradeUIMouseClick>().IsReseting = false;
        upgradeUI.transform.GetChild(8).GetComponent<UpgradeUIMouseClick>().IsReseting = false;
        upgradeUI.transform.GetChild(9).GetComponent<UpgradeUIMouseClick>().IsReseting = false;
        upgradeUI.transform.GetChild(10).GetComponent<UpgradeUIMouseClick>().IsReseting = false;
        IsReseting = false;
    }
}
