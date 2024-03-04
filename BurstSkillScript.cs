/*
* Writer : KimJunWoo
*
* 이 소스코드는 플레이어 버스트 스킬 및 업그레이드 코드가 작성되어 있음
*
* Last Update : 2024/03/04
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstSkillScript : MonoBehaviour
{
    public GameObject player;
    public GameObject burst1;
    public GameObject burst2;
    public Attack attack;
    public bool UpgradeOn1;

    float count;
    bool CountOn;

    void Start()
    {
        attack = player.GetComponent<Attack>();
        count = 0;
        CountOn = false;
        UpgradeOn1 = false;
    }

    void Update()
    {
        //스킬 쿨타임
        if (attack.SkillEffectOn)
            CountOn = true;
        if (CountOn)
            count += Time.deltaTime;
        if (count >= 2.5) {
            CountOn = false;
            count = 0;
        }
        //업그레이드 시 양쪽으로 나가는 추가 이펙트도 위치 조정
        if (!CountOn) {
            gameObject.transform.position = player.transform.position + new Vector3(0, 4, 0);
            gameObject.transform.rotation = player.transform.rotation;
            gameObject.transform.localEulerAngles += new Vector3(0, -90, 0);
            if (UpgradeOn1) {
                burst1.transform.rotation = player.transform.rotation;
                burst1.transform.localEulerAngles += new Vector3(0, -110, 0);
                burst2.transform.rotation = player.transform.rotation;
                burst2.transform.localEulerAngles += new Vector3(0, -70, 0);
            }
        }
    }
}
