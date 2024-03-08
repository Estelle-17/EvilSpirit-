/*
* Writer : KimJunWoo
*
* 이 소스코드는 플레이어 공격 콜라이더와 데미지 계산, 타격감을 위한 애니메이션 조정이 포함되 있음
*
* Last Update : 2024/03/02
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour {
    //Player stat
    public float damage;
    public float totalDamage;
    public bool isColliderActive;
    public bool isSkillPointUp;
    public int Skill1Stack;
    public int HitMonsterCount;
    public bool BonusCounterAttack;
    public bool SmashUpgradeOn;

    public LayerMask layerMask;

    //playerAnimation
    public Animator OwnerAnim;
    public GameObject Owner;

    //Particles
    public GameObject HitParticleObj;
    public GameObject HitParticleObj1;
    public SpecialHitEffects HitEffect;

    //Enemy
    public List<GameObject> HitEnemies;

    //Sound
    public AudioSource sound;
    public AudioClip[] soundClip;
    private bool SoundOn;

    void Start() {
        HitMonsterCount = 0;
        Skill1Stack = 0;
        isColliderActive = false;
        isSkillPointUp = false;
        BonusCounterAttack = false;
        damage = 0f;
        SoundOn = false;
        SmashUpgradeOn = false;
        totalDamage = 0;
        OwnerAnim = null;
        HitEnemies = new List<GameObject>();
    }

    void Update() {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    void OnCollisionStay(Collision col) {
        if (OwnerAnim != null) {
            if (isColliderActive) {
                //범위 내에 몬스터가 존재하는지 확인
                if (!HitEnemies.Contains(col.transform.gameObject) && col.transform.gameObject.CompareTag("Enemy")) {
                    GameObject EnemyObject = col.transform.gameObject;
                    //타격 시 사운드 재생
                    if (!SoundOn) {
                        int num = Random.Range(0, 2);
                        sound.PlayOneShot(soundClip[num]);
                        SoundOn = true;
                    }
                    //이펙트 실행
                    HitEffectOn(col);

                    //몬스터 때렸을 시 채워지는 게이지 조절
                    //각 공격마다 채워지는 게이지 양이 다름
                    //isSkillPointUp을 통해 한 번의 공격 당 게이지는 한 번만 채워지게 됨 
                    if (EnemyObject.GetComponent<UmbMonsterMoveScript>())
                        EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUpUp = false;
                    else if (EnemyObject.GetComponent<LampMonsterMoveScript1>())
                        EnemyObject.GetComponent<LampMonsterMoveScript1>().HitMoveUpUp = false;
                    //기본공격
                    if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") ||
                        OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") ||
                        OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3")) {
                        if (!isSkillPointUp) {
                            Owner.GetComponent<Stats>().SkillBar += 18;
                            isSkillPointUp = true;
                        }
                        //몬스터가 공격에 맞았을 시 작은 몬스터들의 넉백 거리 설정
                        if (EnemyObject.GetComponent<UmbMonsterMoveScript>()) {
                            EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveDown = false;
                            EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUp = false;
                        }else if (EnemyObject.GetComponent<LampMonsterMoveScript1>()){
                            EnemyObject.GetComponent<LampMonsterMoveScript1>().HitMoveDown = false;
                            EnemyObject.GetComponent<LampMonsterMoveScript1>().HitMoveUp = false;
                        }
                    } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1")) {    //강공격1
                        if (!isSkillPointUp) {
                            Owner.GetComponent<Stats>().SkillBar += 40;
                            isSkillPointUp = true;
                        }
                        //몬스터가 공격에 맞았을 시 작은 몬스터들의 넉백 거리 설정
                        if (EnemyObject.GetComponent<UmbMonsterMoveScript>()){
                            EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveDown = false;
                            EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUp = false;
                        }else if (EnemyObject.GetComponent<LampMonsterMoveScript1>()){
                            EnemyObject.GetComponent<LampMonsterMoveScript1>().HitMoveDown = false;
                            EnemyObject.GetComponent<LampMonsterMoveScript1>().HitMoveUp = false;
                        }
                    } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2")) {    //강공격2
                        if (!isSkillPointUp) {
                            Owner.GetComponent<Stats>().SkillBar += 25;
                            isSkillPointUp = true;
                        }
                        //몬스터가 공격에 맞았을 시 작은 몬스터들의 넉백 거리 설정
                        if (EnemyObject.GetComponent<UmbMonsterMoveScript>()) {
                            EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUp = true;
                        } else if (EnemyObject.GetComponent<LampMonsterMoveScript1>()) {
                            EnemyObject.GetComponent<LampMonsterMoveScript1>().HitMoveUp = true;
                        }
                    } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") ||    //강공격3
                                    OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") ||
                                    OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") ||
                                    OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") ||
                                    OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2")) {
                        if (!isSkillPointUp) {
                            Owner.GetComponent<Stats>().SkillBar += 25;
                            isSkillPointUp = true;
                        }
                        //몬스터가 공격에 맞았을 시 작은 몬스터들의 넉백 거리 설정
                        if (EnemyObject.GetComponent<UmbMonsterMoveScript>()){
                            if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2"))
                                EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveDown = false;
                            else
                                EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveDown = true;
                            EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUp = false;
                        }else if (EnemyObject.GetComponent<LampMonsterMoveScript1>()){
                            if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2"))
                                EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveDown = false;
                            else
                                EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveDown = true;
                            EnemyObject.GetComponent<UmbMonsterMoveScript>().HitMoveUp = false;
                        }
                    }

                    HitEnemies.Add(EnemyObject);
                    HitMonsterCount++;
                    StartCoroutine(WaitMomentAnimator()); //관통력 설정
                    totalDamage = 0;    //데미지를 전부 합산해서 띄워줌

                    //데미지 입력
                    if (GetComponentInParent<Weapons>().UniqueEffect == 1) {
                        EnemyObject.GetComponent<EnemyDamage>().isContinueDamagedIn = true;
                    }
                    if (HitEffect != null)
                        HitEffect.HitActive(EnemyObject, damage);
                    else
                        totalDamage = damage / 2f;
                    //애니메이션을 참고하여 실행한 공격에 따라 데미지 계산
                    if (OwnerAnim.GetBool("param_isSmashAttack1"))
                        if (SmashUpgradeOn)
                            totalDamage = (damage * 2f) * (1 + 0.2f * Owner.GetComponent<Stats>().SkillLevels[6]);
                        else
                            totalDamage = damage * 2f;
                    else if (OwnerAnim.GetBool("param_isSmashAttack2"))
                        if (SmashUpgradeOn)
                            totalDamage = (damage * 1.5f) * (1 + 0.2f * Owner.GetComponent<Stats>().SkillLevels[6]);
                        else
                            totalDamage = damage * 1.5f;
                    else if (OwnerAnim.GetBool("param_isSmashAttack3") || OwnerAnim.GetBool("param_isSmashAttack3_1") || OwnerAnim.GetBool("param_isSmashAttack3_2")) {
                        if (SmashUpgradeOn)
                            totalDamage = (damage * 1.4f) * (1 + 0.2f * Owner.GetComponent<Stats>().SkillLevels[6]);
                        else
                            totalDamage = damage * 1.4f;
                    }
                    //카운터 스킬 데미지 계산
                    if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess"))
                        totalDamage = damage * (5f * (1 + 0.2f * Owner.GetComponent<Stats>().SkillLevels[4]));

                    EnemyObject.GetComponent<EnemyDamage>().TakeDamage(totalDamage, Owner);
                    if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && BonusCounterAttack)
                        EnemyObject.GetComponent<EnemyDamage>().TakeDamage(totalDamage * (0.5f + (0.25f * Owner.GetComponent<Stats>().SkillLevels[5])), Owner);
                }
            }
        }
    }

    //타격 이펙트 생성 함수
    public void HitEffectOn(Collision col) {
        //몬스터 때렸을 시 생기는 타격 이펙트 각도 조절
        if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1")) {
            Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 165,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2")) {
            Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x,
                                                                                                               Owner.transform.localEulerAngles.y + 90,
                                                                                                               Owner.transform.localEulerAngles.z));
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3")) {
            Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x - 20,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1")) {
            if(!SmashUpgradeOn)
                Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x - 80,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
            else
                Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x - 80,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2")) {
            if (!SmashUpgradeOn)
                Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x,
                                                                                                                Owner.transform.localEulerAngles.y,
                                                                                                                Owner.transform.localEulerAngles.z));
            else
                Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x,
                                                                                                                Owner.transform.localEulerAngles.y,
                                                                                                                Owner.transform.localEulerAngles.z));
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3")) {
            if (OwnerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5) {
                if (!SmashUpgradeOn)
                    Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 15,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
                else
                    Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 15,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
            } else {
                if (!SmashUpgradeOn)
                    Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 165,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
                else
                    Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 165,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
            }
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1")) {
            if (OwnerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5) {
                if (!SmashUpgradeOn)
                    Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 95,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
                else
                    Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 95,
                                                                                                                    Owner.transform.localEulerAngles.y + 90,
                                                                                                                    Owner.transform.localEulerAngles.z));
            } else {
                if (!SmashUpgradeOn)
                    Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x - 105,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
                else
                    Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x - 105,
                                                                                                                    Owner.transform.localEulerAngles.y + 90,
                                                                                                                    Owner.transform.localEulerAngles.z));
            }
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1")) {
            if (OwnerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5)
                Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x + 95,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
            else
                Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x - 105,
                                                                                                                Owner.transform.localEulerAngles.y + 90,
                                                                                                                Owner.transform.localEulerAngles.z));
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1")) {
            if (!SmashUpgradeOn)
                Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x,
                                                                                    Owner.transform.localEulerAngles.y + 90,
                                                                                    Owner.transform.localEulerAngles.z));
            else
                Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x,
                                                                                    Owner.transform.localEulerAngles.y + 90,
                                                                                    Owner.transform.localEulerAngles.z));
        } else if (OwnerAnim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2")) {
            Instantiate(HitParticleObj1, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x,
                                                                                                            Owner.transform.localEulerAngles.y + 90,
                                                                                                            Owner.transform.localEulerAngles.z));
        } else {
            Instantiate(HitParticleObj, col.contacts[0].point, Quaternion.Euler(Owner.transform.localEulerAngles.x,
                                                                                                                Owner.transform.localEulerAngles.y,
                                                                                                                Owner.transform.localEulerAngles.z));
        }
    }
    //플레이어 설정
    public void SetOwner(GameObject owner) {
        Owner = owner;
        OwnerAnim = Owner.GetComponent<Animator>();
        HitEffect = GetComponentInParent<Weapons>().HitEffect;
        SetDamage();
    }
    //플레이어의 데미지 설정
    public void SetDamage() {
        if (Owner != null) {
            damage = Owner.GetComponent<Stats>().Damage + Owner.GetComponent<Stats>().AdditionalDamage + GetComponentInParent<Weapons>().damage + GetComponentInParent<Weapons>().additionalDamage;
            Owner.GetComponent<Stats>().TotalDamage = damage;
        }
    }
    //타격이 끝난 후 몬스터들을 담아둔 배열 초기화
    public void ResetHitEnemy() {
        HitEnemies = new List<GameObject>();
        HitMonsterCount = 0;
        isSkillPointUp = false;
        SoundOn = false;
    }
    //타격감을 위해 애니메이션을 일정 시간 멈춤
    //더 많이 때릴수록 멈추는 시간이 줄어듬
    IEnumerator WaitMomentAnimator() {
        OwnerAnim.speed = 0f;
        Owner.GetComponent<PlayerControl>().isAnimStop = true;
        if (HitMonsterCount == 1)
            yield return new WaitForSeconds(0.06f);
        else if (HitMonsterCount == 2)
            yield return new WaitForSeconds(0.02f);
        else if (HitMonsterCount >= 3 && HitMonsterCount >= 5)
            yield return new WaitForSeconds(0.005f);
        else
            yield return null;
        Owner.GetComponent<PlayerControl>().isAnimStop = false;
        OwnerAnim.speed = 1f;
    }
}
