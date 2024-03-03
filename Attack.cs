/*
* Writer : KimJunWoo
*
* 이 소스코드는 플레이어 공격 및 스킬 애니메이션 조절이 포함되 있음
*
* Last Update : 2024/03/02
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //이펙트 on/off
    private Animator anim;
    public int attackCnt;
    public bool SkillEffectOn;
    public bool CounterEffectOn;
    public bool SwordEffectOn;
    public bool BaseAttackOn;
    public bool SmashAttackOn;
    public bool SmashDamageUp;
    bool SettingOn;

    //쿨타임 설정
    public float FillingAmount;
    public float TimeCount;
    public bool isCooltime_3;
    public bool isCooltime_4;
    public bool SlowBurstOn;

    //이펙트
    public bool isAttacking;
    private PlayerControl playerControl;
    private PlayerDamageCalculation playerDamageCalculation;
    public ScrollItemsScript ScrollItem;
    public ParticleSystem SkillEffect1;
    public ParticleSystem SkillEffect2;
    public ParticleSystem SwordEffect1;
    public ParticleSystem SwordEffect2;
    public ParticleSystem SwordEffect3;
    public ParticleSystem SmashEffect1;
    public ParticleSystem SmashEffect2;
    public ParticleSystem[] SmashEffect3;
    public ParticleSystem[] SmashEffect3plus;
    public ParticleSystem CounterEffect1;
    public ParticleSystem CounterEffect2;
    public ParticleSystem CounterEffect3;
    public ParticleSystem CounterHandEffect;
    public ParticleSystem SkillEffect1_1;
    public GameObject WeaponEffect;

    //콜라이더
    public BurstSkillColliderScript BurstCollider;
    private AttackCollider weaponCollider;
    public GameObject Smash2Collider;
    public GameObject Attack3Collider;
    public GameObject CounterCollider;
    public GameObject Skill1Collider;
    public GameObject Skill2Collider;
    public GameObject Skill3Collider;

    //오브젝트
    public Stats stats;
    public UI ui;
    private GameObject weaponTrail;

    //사운드
    public AudioSource sound;
    public AudioClip[] soundClip;
    public bool soundOn;

    //업그레이드
    public bool Smash2ColliderClear;
    public bool SmashUpgradeOn;

    void Start()
    {
        attackCnt = 0;
        FillingAmount = 0;
        TimeCount = 0;
        isAttacking = false;
        SettingOn = false;
        SkillEffectOn = false;
        CounterEffectOn = false;
        SwordEffectOn = false;
        BaseAttackOn = false;
        SmashAttackOn = false;
        soundOn = false;
        isCooltime_3 = false;
        isCooltime_4 = false;
        SlowBurstOn = false;
        Smash2ColliderClear = false;
        SmashUpgradeOn = false;
        Skill1Collider.SetActive(false);
        Skill2Collider.SetActive(false);
        Skill3Collider.SetActive(false);
        playerDamageCalculation = GetComponent<PlayerDamageCalculation>();
        playerControl = GetComponent<PlayerControl>();
        stats = GetComponent<Stats>();
        ui = GameObject.Find("UI").GetComponent<UI>();
        anim = GetComponent<Animator>();
        stats.SkillStack = 0;
    }

    void Update()
    {
        if (playerControl != null && !SettingOn) {
            SettingOn = true;
        }
        if (!playerControl.PlayerDead) {
            PlayerAttack();
        }
    }

    void PlayerAttack() {
        //점프중이거나 회피중이 아닐 시 기본 공격과 스킬 가능
        if (!playerControl.jumping && !playerControl.isDodging && !anim.GetBool("param_isjump")) {
            //좌클릭 : 기본공격
            if (Input.GetMouseButtonDown(0)) {
                BaseAttackOn = true;
            }
            //우클릭 : 강화공격
            if (Input.GetMouseButtonDown(1)) {
                SmashAttackOn = true;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") || anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") || anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2") || anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp1") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp2")) {
                BaseAttackOn = true;
            }
            //기본공격과 강화공격 연계 설정
            if (Input.GetMouseButtonDown(0) && !isAttacking && !playerControl.StopInputs) {
                attackCnt = 1;
                isAttacking = true;
                anim.SetBool("param_isBaseAttack", true);
                BaseAttackOn = false;
                SmashAttackOn = false;
            } else if (SmashAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95f &&
                isAttacking && !anim.GetBool("param_isBaseAttack2")) {
                attackCnt = 1;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack1", true);
                SmashAttackOn = false;
                SwordEffectOn = false;
            } else if (BaseAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                 anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f &&
                 isAttacking && !anim.GetBool("param_isSmashAttack1")) {
                attackCnt = 1;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isBaseAttack2", true);
                BaseAttackOn = false;
                SwordEffectOn = false;
            } else if (SmashAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                 anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95f && isAttacking && !anim.GetBool("param_isSmashAttack2")) {
                attackCnt = 1;
                SmashAttackOn = false;
                SwordEffectOn = false;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack2", true);
            } else if (BaseAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f &&
                  anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95f && isAttacking) {
                attackCnt = 1;
                BaseAttackOn = false;
                SwordEffectOn = false;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isBaseAttack3", true);
            } else if (SmashAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f &&
             anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f && isAttacking) {
                attackCnt = 1;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack3", true);
                SmashAttackOn = false;
                SwordEffectOn = false;
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f &&
              anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f) {
                attackCnt = 1;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack3_1", true);
                SmashAttackOn = false;
                SwordEffectOn = false;
            } else if (SmashAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95f && isAttacking) {
                attackCnt = 1;
                SmashDamageUp = true;
                SmashAttackOn = false;
                SwordEffectOn = false;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack3_1", true);
                anim.SetBool("param_SmashDone", false);
                anim.SetBool("param_NextSmash", true);
            } else if (SmashAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f) {
                attackCnt = 1;
                SmashDamageUp = false;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack3_2", true);
                SmashAttackOn = false;
                SwordEffectOn = false;
            } else if (SmashAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f && isAttacking) {
                attackCnt = 1;
                SmashDamageUp = false;
                SmashAttackOn = false;
                SwordEffectOn = false;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack3_2", true);
            } else if (SmashAttackOn && anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95f && isAttacking) {
                attackCnt = 1;
                SmashDamageUp = true;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isSmashAttack3_2", true);
                anim.SetBool("param_NextSmash", true);
                SmashAttackOn = false;
                SwordEffectOn = false;
            }
            //휘두를 시 나오는 사운드 설정
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.05f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.15f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.33f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.15f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.45f) {
                if (!soundOn) {
                    int num = Random.Range(1, 3);
                    sound.PlayOneShot(soundClip[num]);
                    soundOn = true;
                }
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.42f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.38f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.41f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.61f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.61f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.61f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.61f ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.61f) {
                    soundOn = false;
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.55f &&
               anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.65f) {
                if (!soundOn) {
                    sound.PlayOneShot(soundClip[0]);
                    soundOn = true;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f) {
                soundOn = false;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0f &&
               anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f) {
                if (!soundOn) {
                    sound.PlayOneShot(soundClip[3]);
                    soundOn = true;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterOn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0f &&
               anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterOn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f) {
                if (!soundOn) {
                    sound.PlayOneShot(soundClip[4]);
                    CounterHandEffect.Play();
                    soundOn = true;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterOn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f) {
                soundOn = false;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f) {
                soundOn = false;
            }


            //스킬 1번
            if (Input.GetKeyDown(KeyCode.Q) && !isCooltime_3 && !isAttacking && !playerControl.StopInputs && stats.SkillStack >= 1 &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.HitFallDown") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.HitPush") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dodge")) {
                attackCnt = 2;
                isAttacking = true;
                isCooltime_3 = true;
                ui.currentSkillBar = 0;
                stats.SkillBar = 0;
                ui.SkillBarOverPoint = 0;
                ui.SkillBarOver = false;
                anim.SetBool("param_isSkillAttack1", true);
                weaponCollider.Skill1Stack = (int) stats.SkillStack;
                StartCoroutine(ui.StackBarDown(stats.SkillStack));
                stats.SkillStack = 0;
            }

            //카운터 스킬
            if (Input.GetKeyDown(KeyCode.E) && !isCooltime_4 && !playerControl.StopInputs && stats.SkillStack >= 1 &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.HitFallDown") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.HitPush") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dodge")) {
                CounterOn();
                isAttacking = true;
                isCooltime_4 = true;
                anim.SetBool("param_isCounterAttack", true);
                StartCoroutine(ui.OneStackBarDown((int)stats.SkillStack));
                stats.SkillStack--;
            }

            //공격 콜라이더 설정(원하는 만큼만 때릴수 있도록)
            if (weaponCollider != null && !playerControl.isDodging) {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.65f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.65f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.65f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.65f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f ||
                     anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.68f &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.05f &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f) {
                    weaponCollider.isColliderActive = true;
                } else {
                    weaponCollider.isColliderActive = false;
                }
            }
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.45f) {
                Attack3Collider.SetActive(true);
            } else {
                Attack3Collider.SetActive(false);
            }
            //공격1 끝
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f &&
                                                                                                                                            anim.GetBool("param_isBaseAttack2") != true) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isBaseAttack", false);
            }
            //공격1 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f && 
                                                                                                                                            anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.45f) {
                if (!SwordEffectOn) {
                    SwordEffect1.Play();
                    SwordEffectOn = true;
                }
            }
            //스매시1 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.18f &&
                                                                                                                                           anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.23f) {
                if (!SwordEffectOn) {
                    if (!SmashUpgradeOn)
                        SmashEffect1.Play();
                    else
                        SmashEffect3plus[0].Play();
                    SwordEffectOn = true;
                }
            }
            //스매시1 끝
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isSmashAttack1", false);
            }
            //공격2 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                                                                                                                                            anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f) {
                if (!SwordEffectOn) {
                    SwordEffect2.Play();
                    SwordEffectOn = true;
                }
            }
            //스킬1 콜라이더 이동
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                                                                                                                                         anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f) {
                Smash2Collider.SetActive(true);
            } else {
                Smash2Collider.SetActive(false);
            }
            //스킬1 타격 초기화
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.47f &&
                                                                                                                                         anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f) {
                if (!Smash2ColliderClear) {
                    if (weaponCollider != null) {
                        weaponCollider.ResetHitEnemy();
                        Smash2ColliderClear = true;
                    }
                }
            }
            //스매시2 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f &&
                                                                                                                                           anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.35f) {
                if (!SwordEffectOn) {
                    if(!SmashUpgradeOn)
                        SmashEffect2.Play();
                    else
                        SmashEffect3plus[1].Play();
                    SwordEffectOn = true;
                }
            }
            //스매시2 끝
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                playerControl.RunStopTimer = 0f;
                Smash2ColliderClear = false;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isBaseAttack2", false);
                anim.SetBool("param_isSmashAttack2", false);
            }
            //공격3 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                                                                                                                                            anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f) {
                if (!SwordEffectOn) {
                    SwordEffect3.Play();
                    SwordEffectOn = true;
                }
            }
            //공격2 끝
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && 
                anim.GetBool("param_isBaseAttack3") != true) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isBaseAttack2", false);
            }
            //공격3 끝
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.BaseAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isBaseAttack2", false);
                anim.SetBool("param_isBaseAttack3", false);
            }
            //스매시3 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f &&
                                                                                                                                           anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f) {
                if (!SwordEffectOn) {
                    if(!SmashUpgradeOn)
                        SmashEffect3[0].Play();
                    else
                        SmashEffect3plus[2].Play();
                    SwordEffectOn = true;
                }
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                                                                                                                                           anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f) {
                    SwordEffectOn = false;
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                                                                                                                                           anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f) {
                if (!SwordEffectOn) {
                    if (!SmashUpgradeOn)
                        SmashEffect3[1].Play();
                    else
                        SmashEffect3plus[3].Play();
                    SwordEffectOn = true;
                }
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f &&
                                                                                                                                           anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f) {
                SwordEffectOn = false;
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f) {
                if (!SwordEffectOn) {
                    StartCoroutine(WeaponEffectOn());
                    SwordEffectOn = true;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f &&
                                                                                                                                           anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f) {
                if (!SwordEffectOn) {
                    if (!SmashUpgradeOn)
                        SmashEffect3[2].Play();
                    else
                        SmashEffect3plus[4].Play();
                    SwordEffectOn = true;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                                                                                                                                             anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f) {
                SwordEffectOn = false;
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                                                                                                                                             anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f) {
                if (!SwordEffectOn) {
                    if (!SmashUpgradeOn)
                        SmashEffect3[3].Play();
                    else
                        SmashEffect3plus[5].Play();
                    SwordEffectOn = true;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.15f) {
                if (!SwordEffectOn) {
                    StartCoroutine(WeaponEffectOn());
                    SwordEffectOn = true;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                                                                                                                                             anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f) {
                if (!SwordEffectOn) {
                    if (!SmashUpgradeOn)
                        SmashEffect3[4].Play();
                    else
                        SmashEffect3plus[6].Play();
                    SwordEffectOn = true;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f ||
                         anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f ||
                         anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f ||
                         anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f ||
                         anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                anim.SetBool("param_SmashDone", true);
            }
            //스매시3 끝
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                SmashDamageUp = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_SmashDone", false);
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isBaseAttack2", false);
                anim.SetBool("param_isBaseAttack3", false);
                anim.SetBool("param_isSmashAttack3", false);
                anim.SetBool("param_isSmashAttack3_1", false);
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f ||
                         anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_SmashDone", false);
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isBaseAttack2", false);
                anim.SetBool("param_isBaseAttack3", false);
                anim.SetBool("param_isSmashAttack3", false);
                anim.SetBool("param_isSmashAttack3_1", false);
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashUp2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                SmashDamageUp = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_SmashDone", false);
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isBaseAttack2", false);
                anim.SetBool("param_isBaseAttack3", false);
                anim.SetBool("param_isSmashAttack3", false);
                anim.SetBool("param_isSmashAttack3_1", false);
                anim.SetBool("param_isSmashAttack3_2", false);
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f ||
                         anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_2_2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f) {
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                SmashDamageUp = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                anim.SetBool("param_SmashDone", false);
                anim.SetBool("param_NextSmash", false);
                anim.SetBool("param_isBaseAttack", false);
                anim.SetBool("param_isBaseAttack2", false);
                anim.SetBool("param_isBaseAttack3", false);
                anim.SetBool("param_isSmashAttack3", false);
                anim.SetBool("param_isSmashAttack3_1", false);
                anim.SetBool("param_isSmashAttack3_2", false);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SpecialAttack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f) {
                playerControl.CameraLockOn = false;
                isAttacking = false;
                anim.SetBool("param_isSpecialAttack", false);
            }

            //카운터 어택 설정
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterOn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0f) {
                anim.SetBool("param_isCounterAttack", false);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterOn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                anim.SetBool("param_isCounterAttackFail", true);
            }
            //카운터 어택 실패
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterFail") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                anim.SetBool("param_isCounterAttackFail", false);
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                SmashDamageUp = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
            }
            //카운터 어택 콜라이더
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.05f &&
                                                                                                                                         anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f) {
                CounterCollider.SetActive(true);
            } else {
                CounterCollider.SetActive(false);
            }

            //카운터 어택 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f) {
                if (!CounterEffectOn) {
                    CounterEffect1.Play();
                    CounterEffect2.Play();
                    CounterEffect3.Play();
                    CounterEffectOn = true;
                }
            }

            //카운터 어택 성공
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CounterSuccess") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                playerControl.CounterSkillAttackSpeed = 500;
                this.gameObject.layer = 12;
                anim.SetBool("param_isCounterAttackSuccess", false);
                attackCnt = 0;
                isAttacking = false;
                SwordEffectOn = false;
                CounterEffectOn = false;
                SmashDamageUp = false;
                playerControl.RunStopTimer = 0f;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
            }
            
            //스킬1 이펙트
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f && 
                                                                                                                                          anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.75f) {
                if (!SkillEffectOn && !SlowBurstOn) {
                    SkillEffect1.Play();
                    SkillEffectOn = true;
                }
                else if (!SkillEffectOn && SlowBurstOn) {
                    SkillEffect2.Play();
                    SkillEffectOn = true;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1")&& anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f) {
                if(!SkillEffect1_1.isPlaying)
                    SkillEffect1_1.Play();
                BurstCollider.SetColliderPos();
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f) {
                if (SkillEffect1_1.isPlaying)
                    SkillEffect1_1.Stop();
            }
            //스킬1 끝
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                playerControl.SkillAttackSpeed = 50;
                attackCnt = 0;
                isAttacking = false;
                SkillEffect1.Stop();
                SkillEffect2.Stop();
                SkillEffectOn = false;
                anim.SetBool("param_isSkillAttack1", false);
            }

            //한 공격이 끝날 시 때린 적들 초기화
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && 
                                                                                                                                                anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.65f && attackCnt == 1 ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f && attackCnt == 1 ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("BaseAttack.SmashAttack3_1_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f && attackCnt == 1) {
                SwordEffectOn = false;
                if (weaponCollider != null)
                    weaponCollider.ResetHitEnemy();
                --attackCnt;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SkillAttack1")) {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f && attackCnt == 2 ||
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f && attackCnt == 1 ||
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                    if (weaponCollider != null)
                        weaponCollider.ResetHitEnemy();
                    --attackCnt;
                }
            }
        }
    }

    //공격이 끝나거나 멈출 시 애니메이션 기본자세로 초기화
    public void AttackStop() {
        anim.SetBool("param_isBaseAttack", false);
        anim.SetBool("param_isBaseAttack2", false);
        anim.SetBool("param_isBaseAttack3", false);
        anim.SetBool("param_isSmashAttack1", false);
        anim.SetBool("param_isSmashAttack2", false);
        anim.SetBool("param_isSmashAttack3", false);
        anim.SetBool("param_isSmashAttack3_1", false);
        anim.SetBool("param_isSmashAttack3_2", false);
        anim.SetBool("param_SmashDone", false);
        anim.SetBool("param_NextSmash", false);
        attackCnt = 0;
        isAttacking = false;
        SwordEffectOn = false;
        soundOn = false;
        playerControl.RunStopTimer = 0f;
        if (weaponCollider != null) {
            weaponCollider.ResetHitEnemy();
            weaponCollider.isColliderActive = false;
        }
        isAttacking = false;
    }
    //공격이 끝나거나 멈출 시 애니메이션 기본자세로 초기화
    public void MotionStop() {
        anim.SetBool("param_isBaseAttack", false);
        anim.SetBool("param_isBaseAttack2", false);
        anim.SetBool("param_isBaseAttack3", false);
        anim.SetBool("param_isSmashAttack1", false);
        anim.SetBool("param_isSmashAttack2", false);
        anim.SetBool("param_isSmashAttack3", false);
        anim.SetBool("param_isSmashAttack3_1", false);
        anim.SetBool("param_isSmashAttack3_2", false);
        anim.SetBool("param_SmashDone", false);
        anim.SetBool("param_NextSmash", false);
        anim.SetBool("param_isDodge", false);
        anim.SetBool("param_isDodgeOver", true);
        anim.SetBool("param_isSkillAttack1", false);
        anim.SetBool("param_isCounterAttackFail", false);
        playerControl.SkillAttackSpeed = 50;
        SkillEffect1.Stop();
        SkillEffect2.Stop();
        SkillEffect1_1.Stop();
        attackCnt = 0;
        isAttacking = false;
        SwordEffectOn = false;
        soundOn = false;
        SkillEffectOn = false;
        playerControl.RunStopTimer = 0f;
        if (weaponCollider != null) {
            weaponCollider.ResetHitEnemy();
            weaponCollider.isColliderActive = false;
        }
        isAttacking = false;
    }
    //카운터 스킬 사용 시 애니메이션 설정
    public void CounterOn() {
        anim.SetBool("param_isBaseAttack", false);
        anim.SetBool("param_isBaseAttack2", false);
        anim.SetBool("param_isBaseAttack3", false);
        anim.SetBool("param_isSmashAttack1", false);
        anim.SetBool("param_isSmashAttack2", false);
        anim.SetBool("param_isSmashAttack3", false);
        anim.SetBool("param_isSmashAttack3_1", false);
        anim.SetBool("param_isSmashAttack3_2", false);
        anim.SetBool("param_SmashDone", false);
        anim.SetBool("param_NextSmash", false);
        SwordEffectOn = false;
        soundOn = false;
        playerControl.RunStopTimer = 0f;
        if (weaponCollider != null) {
            weaponCollider.ResetHitEnemy();
            weaponCollider.isColliderActive = false;
        }
    }

    public void SettingSwordEffect() {
        SwordEffect1.Play();
        SwordEffect2.Play();
        SwordEffect3.Play();
        SmashEffect1.Play();
        SmashEffect2.Play();
        for(int i = 0; i < 5; i++) {
            SmashEffect3[i].Play();
            SmashEffect3plus[i].Play();
        }
        CounterEffect1.Play();
    }
    
    IEnumerator WeaponEffectOn() {
        Color color = WeaponEffect.GetComponent<Renderer>().material.GetColor("_TintColor");
        for (int i = 0; i < 10; i++) {
            color.a += 0.1f;
            WeaponEffect.GetComponent<Renderer>().material.SetColor("_TintColor", color);
            yield return null;
        }
        for (int i = 10; i >= 0; i--) {
            color.a -= 0.1f;
            WeaponEffect.GetComponent<Renderer>().material.SetColor("_TintColor", color);
            yield return null;
        }
        color.a = 0;
        WeaponEffect.GetComponent<Renderer>().material.SetColor("_TintColor", color);

    }
    //스킬 업그레이드 시 적용
    public void BurstUpgrade1() {
        SkillEffect1.GetComponent<BurstSkillScript>().UpgradeOn1 = true;
        SkillEffect1.GetComponent<BurstSkillScript>().burst1.SetActive(true);
        SkillEffect1.GetComponent<BurstSkillScript>().burst2.SetActive(true);
        SkillEffect2.GetComponent<BurstSkillScript>().UpgradeOn1 = true;
        SkillEffect2.GetComponent<BurstSkillScript>().burst1.SetActive(true);
        SkillEffect2.GetComponent<BurstSkillScript>().burst2.SetActive(true);
    }
    public void BurstUpgrade2() {
        SlowBurstOn = true;
    }

    public void SetWeaponCollider(GameObject newWeapon) {
        weaponCollider = newWeapon.GetComponent<AttackCollider>();
    }
    public void BloodAdditionalDamage() {
        float maxHp = playerDamageCalculation.MaxHP;
        float currentHp = playerDamageCalculation.currentHp;
        if (currentHp / maxHp >= 0.2f) {
            playerDamageCalculation.TakeDamage(maxHp / 100);
            ScrollItem.wasteHp += (int)maxHp / 100;
        }
    }
}
