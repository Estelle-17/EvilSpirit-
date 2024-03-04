/*
* Writer : KimJunWoo
*
* 이 소스코드는 플레이어 버스트 스킬의 콜라이더 코드가 작성되어 있음
*
* Last Update : 2024/03/04
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstSkillColliderScript : MonoBehaviour
{
    public Attack player;
    public AttackCollider attackCollider;
    private Animator anim;
    public GameObject Skill1Collider;
    public GameObject Skill2Collider;
    public GameObject Skill3Collider;
    public List<GameObject> HitEnemies;

    public bool SlowColliderMoveOn;
    public bool SoundOn;
    public float t;
    public float ResetTime;

    void Start()
    {
        anim = player.GetComponent<Animator>();
        HitEnemies = new List<GameObject>();
        SlowColliderMoveOn = false;
        SoundOn = false;
        t = 0;
        ResetTime = 0;
    }

    void Update() {
        //버스트 스킬 콜라이더 이동
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && 
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f) {
            if (player.SlowBurstOn) {
                if (!SlowColliderMoveOn) {
                    SlowColliderMoveOn = true;
                    StartCoroutine(SlowColliderMove());
                }
            } else {
                Skill1Collider.SetActive(true);
                Skill1Collider.transform.localPosition += new Vector3(0, 0, 5f);
                if (player.stats.SkillLevels[0] > 0) {
                    Skill2Collider.SetActive(true);
                    Skill2Collider.transform.localPosition += new Vector3(-1.45f, 0, 4.46f);
                    Skill3Collider.SetActive(true);
                    Skill3Collider.transform.localPosition += new Vector3(1.45f, 0, 4.46f);
                }
            }
        }
        //스킬이 끝나면 콜라이더 위치 초기화
        if (!player.SlowBurstOn && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
            Skill1Collider.SetActive(false);
            Skill1Collider.transform.localPosition = new Vector3(0, 1.65f, 0);
            Skill2Collider.SetActive(false);
            Skill2Collider.transform.localPosition = new Vector3(0, 1.65f, 0);
            Skill3Collider.SetActive(false);
            Skill3Collider.transform.localPosition = new Vector3(0, 1.65f, 0);
            SlowColliderMoveOn = false;
            ResetHitEnemy();
        }
    }

    void OnCollisionStay(Collision col) {
        if (anim != null) {
            //콜라이더 내부에 있는 적을 판별 후 적들만 피해를 입힘
            if (!HitEnemies.Contains(col.transform.gameObject) && col.transform.gameObject.CompareTag("Enemy")) {
                GameObject EnemyObject = col.transform.gameObject;

                //스킬 사운드
                if (!SoundOn) {
                    int num = Random.Range(0, 2);
                    attackCollider.sound.PlayOneShot(attackCollider.soundClip[num]);
                    SoundOn = true;
                }
                attackCollider.HitEffectOn(col);

                HitEnemies.Add(EnemyObject);
                attackCollider.totalDamage = 0;
                //스킬의 맞은 적의 피격 움직임 설정
                if (EnemyObject.GetComponent<UmbMonsterMoveScript>()) {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1")) {
                        EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveDown = false;
                        EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUp = false;
                        EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUpUp = true;
                    }
                }
                //스택이 더 많을수록 총 데미지가 증가되도록 설정
                attackCollider.totalDamage += attackCollider.damage;
                if (player.SlowBurstOn) {
                    switch (attackCollider.Skill1Stack) {
                        case 1:
                            attackCollider.totalDamage *= 0.3f;
                            break;
                        case 2:
                            attackCollider.totalDamage *= 0.5f;
                            break;
                        case 3:
                            attackCollider.totalDamage *= 0.7f;
                            break;
                        case 4:
                            attackCollider.totalDamage *= 1.2f;
                            break;
                        case 5:
                            attackCollider.totalDamage *= 2.5f;
                            break;
                    }
                    attackCollider.totalDamage += player.GetComponent<Stats>().SkillLevels[1] * 5;       //여러번 업글 추가데미지
                } else {
                    switch (attackCollider.Skill1Stack) {
                        case 1:
                            attackCollider.totalDamage *= 2f;
                            break;
                        case 2:
                            attackCollider.totalDamage *= 3f;
                            break;
                        case 3:
                            attackCollider.totalDamage *= 5f;
                            break;
                        case 4:
                            attackCollider.totalDamage *= 8f;
                            break;
                        case 5:
                            attackCollider.totalDamage *= 12f;
                            break;
                    }
                    attackCollider.totalDamage += player.GetComponent<Stats>().SkillLevels[0] * 50;     //여러번 업글 추가데미지
                }
                //업그레이드 시 스택별 추가데미지 계산
                if (player.GetComponent<Stats>().SkillLevels[2] > 0) 
                    attackCollider.totalDamage *= 1 + ((0.05f + ((player.GetComponent<Stats>().SkillLevels[2] - 1) * 0.03f)) * attackCollider.Skill1Stack);

                if (player.GetComponent<Stats>().SkillLevels[3] != 0)
                    attackCollider.totalDamage *= 1.3f;

                EnemyObject.GetComponent<EnemyDamage>().TakeDamage((int)attackCollider.totalDamage, player.gameObject);
            }
        }
    }

    public void ResetHitEnemy() {
        HitEnemies = new List<GameObject>();
        SoundOn = false;
    }

    public void SetColliderPos() {
        transform.localPosition = new Vector3(player.transform.position.x, player.transform.position.y + 4.29f, player.transform.position.z);
        transform.localRotation = player.transform.localRotation;
    }
    
    IEnumerator SlowColliderMove() {
        //느려지는 업그레이드를 진행하면 같은 적을 여러 번 타격이 가능하도록 설정
        while (true) {
            Skill1Collider.SetActive(true);
            Skill1Collider.transform.localPosition += new Vector3(0, 0, 1.5f);
            if (player.stats.SkillLevels[0] > 0) {
                Skill2Collider.SetActive(true);
                Skill2Collider.transform.localPosition += new Vector3(-0.432f, 0, 1.338f);
                Skill3Collider.SetActive(true);
                Skill3Collider.transform.localPosition += new Vector3(0.432f, 0, 1.338f);
            }
            t += Time.deltaTime;
            ResetTime++;
            yield return null;
            if(ResetTime >= 5) {
                ResetTime = 0;
                ResetHitEnemy();
            }
            if (t > 1)
                break;
        }
        //스킬이 끝나면 콜라이더 위치 초기화
        Skill1Collider.SetActive(false);
        Skill1Collider.transform.localPosition = new Vector3(0, 1.65f, 0);
        Skill2Collider.SetActive(false);
        Skill2Collider.transform.localPosition = new Vector3(0, 1.65f, 0);
        Skill3Collider.SetActive(false);
        Skill3Collider.transform.localPosition = new Vector3(0, 1.65f, 0);
        SlowColliderMoveOn = false;
        ResetHitEnemy();
        t = 0;
        ResetTime = 0;
    }
}
