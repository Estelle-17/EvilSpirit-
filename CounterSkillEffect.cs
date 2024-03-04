/*
* Writer : KimJunWoo
*
* 이 소스코드는 플레이어 카운터 스킬 및 업그레이드 코드가 작성되어 있음
*
* Last Update : 2024/03/04
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterSkillEffect : MonoBehaviour
{
    public GameObject player;
    public Attack attack;
    public bool UpgradeOn1;

    float count;
    bool CountOn;

    void Start() {
        attack = player.GetComponent<Attack>();
        count = 0;
        CountOn = false;
        UpgradeOn1 = false;
    }

    void Update() {
        //카운터 스킬 쿨타임
        if (attack.CounterEffectOn && count >= 2.5) {
            CountOn = true;
            count = 0;
        }
        if (!CountOn)
            count += Time.deltaTime;
        //카운터 스킬 이펙트 위치 조정
        if (CountOn) {
            gameObject.transform.position = player.transform.position + new Vector3(0, 4, 0);
            gameObject.transform.rotation = player.transform.rotation;
            //gameObject.transform.localEulerAngles += new Vector3(0, -90, 0);
            CountOn = false;
        }
    }
}
