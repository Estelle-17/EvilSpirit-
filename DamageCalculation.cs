/*
* Writer : KimJunWoo
*
* 이 소스코드는 몬스터들의 데미지 계산 코드가 작성되어 있음
*
* Last Update : 2024/03/03
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DamageCalculation : MonoBehaviour
{
    public float startingHp;
    public float currentHp;
    public float hpBarTimer;
    public float TotalDamage;

    public float DebuffAdditionalSkillDamage;
    public bool isDebuff;
    public bool dead;

    public GameObject recentlyDamagedObject;
    public GameObject DmgTextObj;
    public GameObject DmgText;
    public UI ui;
    public Canvas uiCanvas;

    protected IEnumerator continuousDamage;

    void Start()
    {
        ui = GameObject.Find("UI").GetComponent<UI>();
        uiCanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        hpBarTimer = 11.0f;
        isDebuff = false;
        currentHp = startingHp;
        DebuffAdditionalSkillDamage = 1;
    }

    public void TakeDamage(float damage, GameObject obj) {

        //현재 체력에서 입은 데미지를 감소해줌
        damage = (int)damage;
        hpBarTimer = 0f;
        currentHp -= damage;
        //데미지 화면 출력
        /*GameObject text = Instantiate<GameObject>(DmgTextObj, uiCanvas.transform);
        text.GetComponent<DamageText>().target = this.gameObject;
        text.GetComponent<DamageText>().SetDamageText(damage);*/
        TotalDamage += damage;

        //몬스터들의 체력바를 현재 체력에 맞게 갱신시켜줌
        if (transform.GetComponent<UmbMonsterMoveScript>() != null) {
            if (transform.GetComponent<UmbMonsterMoveScript>().isMiddleBoss) {
                ui.BossHp();
                ui.BossHpTimer = 0;
                ui.BossUIDamage.gameObject.SetActive(true);
                ui.BossHpTotalDamage += damage;
            } else
                transform.GetComponent<UmbMonsterMoveScript>().anim.SetBool("param_Hit", true);
        }
        if (transform.GetComponent<LampMonsterMoveScript1>() != null) {
            transform.GetComponent<LampMonsterMoveScript1>().anim.SetBool("param_Hit", true);
        }
        if (transform.GetComponent<BossMoveScript>() != null) {
            ui.BossHpTimer = 0;
            ui.BossUIDamage.gameObject.SetActive(true);
            ui.BossHpTotalDamage += damage;
            ui.BossHp();
        }
        recentlyDamagedObject = obj; //캐릭터의 효과를 발동하기 위해서 최근에 공격한 사람 저장
    }

    public void DamageText(float dmg) {
        GameObject dmgText = Instantiate(DmgText, transform);
    }
    //지속 데미지 계산
    IEnumerator ContinuousDamage(float damage) {
        while (true) {
            currentHp -= damage;
            TotalDamage += damage;

            yield return new WaitForSeconds(0.5f);
        }
    }
    //지속 데미지 설정
    protected IEnumerator SetContinuousDamage(float damage, float delayTime) {
        if (!isDebuff) {
            isDebuff = true;
            continuousDamage = ContinuousDamage(damage);
            StartCoroutine(continuousDamage);
            yield return new WaitForSeconds(delayTime);
            StopCoroutine(continuousDamage);
            isDebuff = false;
            yield break;
        }
    }
}
