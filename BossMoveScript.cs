/*
* Writer : KimJunWoo
*
* 이 소스코드는 보스의 전반적인 코드가 작성되어 있음
*
* Last Update : 2024/03/03
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMoveScript : MonoBehaviour
{
    //목표지점 좌표
    public Rigidbody rig;
    public Transform target;
    public Transform Backpos;
    private float VeryCloseDistance;
    private float CloseDistance;
    private float AttackDistance;
    private float MoveDistance;
    private float FarDistance;
    private float TargetAngle;
    public float ActivatePlayerDistance;
    public float SummonSetY;
    public float MoveSpeed;
    public float BackMoveSpeed;

    public bool IsDummy;
    public bool isArounding;
    public bool IsAttackNow;
    public bool IsAttackNow2;
    public bool IsAttacking;
    private bool IsDead;
    private float BossAttackMoveDistance;
    private float BossBackMoveDistance;
    private float AttackMoveDistance;
    public bool HitMoveDown;
    public bool HitMoveUp;
    public bool HitMoveUpUp;
    public float HitMoveDistance;
    public bool IsSummon;
    public bool IsEffectOn;
    public bool IsEffectOn2;
    public bool IsSoundOn;
    public bool IsSoundOn2;
    public bool Pase2On;
    public bool Pase2Clear;

    //오브젝트
    public GameObject AttackTriggerObject;
    public GameObject Attack360TriggerObject;
    public GameObject FrontAttackTriggerObject;
    public GameObject AttackHandTriggerObject;
    public GameObject BackAttackTriggerObject;
    public GameObject SpecialAttackTriggerObject;
    public GameObject objCollider;
    public GameObject objHitCollider;
    public GameObject ForwardPosition;
    public GameObject EffectPosition;
    public GameObject[] BossEffectsPos;
    public GameObject LightningAttackObject;

    //파티클
    public ParticleSystem FrontAttackEffect;
    public ParticleSystem FrontWeaponAttackEffect;
    public ParticleSystem FrontWeaponAttackEffect2;
    public ParticleSystem Weapon360AttackEffect;
    public ParticleSystem BackAttackEffect;
    public ParticleSystem LightningEffect;
    public ParticleSystem Pase2Effect;
    public ParticleSystem Pase2BossEffect;
    public ParticleSystem SpecialAttackHandEffect;
    public ParticleSystem SpecialAttackEffect;
    public ParticleSystem SpawnParticle;
    public ParticleSystem HowlingParticle;
    public ParticleSystem DeadParticle;
    public Vector3 hpBarOffset;
    private Vector3 StartAttackPos;
    private Vector3 EndAttackPos;
    public List<GameObject> HitPlayers;
    public LayerMask layerMask;
    public LayerMask groundLayerMask;

    //스탯
    public float damage;
    private float HitStopTime;
    private float AttackStopTimer;
    public float moveTime;
    public float WaitSeconds;
    public float CurrentWaitSeconds;
    private float MoveAttackTimer;
    private float AttackMoveTimer;
    private float Attack360Timer;
    private float FrontWeaponAttackTimer;
    private float FrontAttackTimer;
    private float BackAttackTimer;
    private float SpecialAttackTimer;
    private float LightningAttackTimer;
    private bool TurnTimer;
    public int BackJumpTimer;

    //애니메이션
    public Animator anim;
    //데미지계산 스크립트
    private EnemyDamage enemy;

    public GM gm;
    public bool SceneOut;

    NavMeshAgent agent;
    public CameraRotate playerCamera;
    private GameObject obj;
    public GameObject BossMapDoor;

    //사운드
    public AudioSource sound;
    public AudioClip[] soundClip;

    public float BossDeadTime;
    public GameObject ClearUI;

    void Start() {
        hpBarOffset = new Vector3(0, 14f, 0);
        BossBackMoveDistance = -0.7f;
        AttackMoveDistance = 1.5f;
        HitMoveDistance = -0.95f;
        MoveSpeed = 25;
        BackMoveSpeed = 55;
        moveTime = 0;
        IsAttackNow = true;
        IsAttackNow2 = true;
        HitMoveDown = false;
        HitMoveUp = false;
        HitMoveUpUp = false;
        isArounding = true;
        IsDead = false;
        IsSummon = false;
        IsEffectOn = false;
        IsEffectOn2 = false;
        IsSoundOn = false;
        IsAttacking = false;
        Pase2On = false;
        Pase2Clear = false;
        SceneOut = false;
        IsSoundOn2 = false;
        FarDistance = 200;
        MoveDistance = 100;
        AttackDistance = 80;
        CloseDistance = 40;
        VeryCloseDistance = 30;
        anim = GetComponent<Animator>();
        anim.speed = 0f;
        rig = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<EnemyDamage>();
        CurrentWaitSeconds = 0;
        damage = 40;
        layerMask = 1 << 12;
        groundLayerMask = 1 << 10;
        agent.speed = 8;
        Attack360Timer = 16;
        FrontWeaponAttackTimer = 5;
        FrontAttackTimer = 5;
        BackAttackTimer = 10;
        MoveAttackTimer = -13f;
        LightningAttackTimer = 0;
        BackJumpTimer = 0;
        TurnTimer = false;
        BossDeadTime = 0;
        SetHpBarOffset();
        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
        AttackTriggerObject.SetActive(false);
        Attack360TriggerObject.SetActive(false);
        AttackHandTriggerObject.SetActive(false);
        BackAttackTriggerObject.SetActive(false);
        FrontAttackTriggerObject.SetActive(false);
        SpecialAttackTriggerObject.SetActive(false);
        gameObject.SetActive(false);
        BossMapDoor.SetActive(false);
        ClearUI.SetActive(false);
    }

    void Update() {
        //일정 이하로 체력 감소 시 2페이즈 진입
        if(enemy.currentHp < enemy.startingHp / 2 && !Pase2Clear) {
            Pase2On = true;
        }
        //각 패턴들의 쿨타임
        if (target != null) {
            CurrentWaitSeconds += Time.deltaTime;
            HitStopTime += Time.deltaTime;
            MoveAttackTimer += Time.deltaTime;
            AttackStopTimer += Time.deltaTime;
            Attack360Timer += Time.deltaTime;
            FrontWeaponAttackTimer += Time.deltaTime;
            FrontAttackTimer += Time.deltaTime;
            BackAttackTimer += Time.deltaTime;
            SpecialAttackTimer += Time.deltaTime;
            LightningAttackTimer += Time.deltaTime;
        }
        //등장씬이 끝난 후 패턴 시작
        if(target != null && WaitSeconds <= CurrentWaitSeconds && !IsSummon) {
            agent.speed = 12;
            isArounding = false;
            IsSummon = true;
        }
        //체력이 0 이하로 됬을 시 사망
        if (enemy.IsDead && !IsDead) {
            gameObject.layer = 18;
            Pase2BossEffect.Stop();
            Pase2Effect.Stop();
            IsDead = true;
        }

        //보스가 사망했을 시 그에 맞는 애니메이션 출력
        if (IsDead) {
            BossDeadTime += Time.deltaTime;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dead") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f) {
                anim.SetBool("param_Dead", false);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dead") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.77f) {
                if (!IsEffectOn) {
                    DeadParticle.Play();
                    IsEffectOn = true;
                }
            }
            //일정 시간 후 UI출력
            if(BossDeadTime >= 5) {
                ClearUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                target.GetComponent<PlayerControl>().StopInputs = true;
                target.transform.root.GetChild(1).GetComponent<CameraRotate>().StopInput = true;
            }
        }
        //정가운데 아래쪽에 보스 체력바UI 설정
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Summon") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.97f) {
            anim.SetBool("param_SummonOn", true);
            NavMeshAgent nav = GetComponent<NavMeshAgent>();
            nav.enabled = true;
            GameObject.Find("UI").GetComponent<UI>().BossName.text = "영혼 먹는 도깨비";
            GameObject.Find("UI").GetComponent<UI>().BossUI.SetActive(true);
            GameObject.Find("UI").GetComponent<UI>().enemyDamage = transform.GetComponent<EnemyDamage>();
            GameObject.Find("UI").GetComponent<UI>().BossHp();
            IsEffectOn = false;
            IsSoundOn = false;
            if (!SceneOut) {
                gm.StartCoroutine(gm.SceneOutFadeOut());
                SceneOut = true;
            }
            //objHitCollider.GetComponent<BoxCollider>().enabled = true;
        }
        //등장 씬 동안 사용되는 파티클
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Summon") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.64f 
                                                                                                          && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.69f) {
            if (!IsEffectOn) {
                SpawnParticle.Play();
                IsEffectOn = true;
            }
            if (!IsSoundOn) {
                sound.PlayOneShot(soundClip[1]);
                IsSoundOn = true;
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Summon") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f
                                                                                                         && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.74f) {
            IsEffectOn = false;
            IsSoundOn = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Summon") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f
                                                                                                          && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f) {
            BossMapDoor.SetActive(true);
            if (!IsEffectOn) {
                HowlingParticle.Play();
                IsEffectOn = true;
            }
            if (!IsSoundOn) {
                sound.PlayOneShot(soundClip[2]);
                IsSoundOn = true;
            }
        }
        //보스의 모든 공격이 들어가있음
        //보스의 공격 범위는 콜라이더를 이용하며 공격의 알맞는 콜라이더를 on/off시킴
        if (!IsDead) {
            //2페이즈 때 랜덤으로 떨어지는 번개 소환
            if (LightningAttackTimer >= 1f && Pase2Clear) {
                obj = Instantiate<GameObject>(LightningAttackObject);
                obj.transform.position = new Vector3(transform.position.x + Random.Range(-75, 75), transform.position.y + 138, transform.position.z + Random.Range(-75, 75));
                obj = Instantiate<GameObject>(LightningAttackObject);
                obj.transform.position = new Vector3(transform.position.x + Random.Range(-75, 75), transform.position.y + 138, transform.position.z + Random.Range(-75, 75));
                LightningAttackTimer = 0;
            }
            //플레이어가 왼쪽 혹은 오른쪽에 있을 때 회전
            //뒤에 있다면 백점프
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LeftTurn")) {
                AttackColliderFalse();
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LeftTurn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    anim.SetBool("param_LeftTurn", false);
                    IsAttacking = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RightTurn")) {
                AttackColliderFalse();
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RightTurn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    anim.SetBool("param_RightTurn", false);
                    IsAttacking = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackJump")) {
                AttackColliderFalse();
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackJump") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    anim.SetBool("param_BackJump", false);
                    BossBackMoveDistance = -0.7f;
                    BackMoveSpeed = 75;
                    IsAttacking = false;
                }
            }
            //앞으로 뛰면서 플레이어한테 무기로 공격
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1")) {
                Attack360TriggerObject.SetActive(false);
                AttackHandTriggerObject.SetActive(false);
                BackAttackTriggerObject.SetActive(false);
                FrontAttackTriggerObject.SetActive(false);
                SpecialAttackTriggerObject.SetActive(false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.52f) {
                    AttackTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.53f) {
                    AttackTriggerObject.SetActive(false);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    anim.SetBool("param_MoveAttack", false);
                    HitPlayers = new List<GameObject>();
                    AttackTriggerObject.SetActive(false);
                    AttackStopTimer = 0;
                    MoveAttackTimer = 0;
                    TurnTimer = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack")) {    //전방을 무기로 크게 공격
                Attack360TriggerObject.SetActive(false);
                AttackHandTriggerObject.SetActive(false);
                BackAttackTriggerObject.SetActive(false);
                FrontAttackTriggerObject.SetActive(false);
                SpecialAttackTriggerObject.SetActive(false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f) {
                    AttackTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.56f) {
                    AttackTriggerObject.SetActive(false);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                    anim.SetBool("param_FrontWeaponAttack", false);
                    HitPlayers = new List<GameObject>();
                    AttackTriggerObject.SetActive(false);
                    AttackStopTimer = 0;
                    TurnTimer = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack")) {  //바로 앞의 있는 플레이어를 공격
                AttackTriggerObject.SetActive(false);
                Attack360TriggerObject.SetActive(false);
                AttackHandTriggerObject.SetActive(false);
                BackAttackTriggerObject.SetActive(false);
                SpecialAttackTriggerObject.SetActive(false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.46f ||
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.77f) {
                    FrontAttackTriggerObject.SetActive(true);
                    AttackTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.47f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f ||
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.78f) {
                    FrontAttackTriggerObject.SetActive(false);
                    AttackTriggerObject.SetActive(false);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.69f) {
                    HitPlayers = new List<GameObject>();
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                    anim.SetBool("param_FrontAttack1", false);
                    HitPlayers = new List<GameObject>();
                    FrontAttackTriggerObject.SetActive(false);
                    AttackTriggerObject.SetActive(false);
                    AttackStopTimer = 0;
                    FrontAttackTimer = 0;
                    TurnTimer = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack")) {    //보스 주변을 크게 공격
                AttackTriggerObject.SetActive(false);
                AttackHandTriggerObject.SetActive(false);
                BackAttackTriggerObject.SetActive(false);
                FrontAttackTriggerObject.SetActive(false);
                SpecialAttackTriggerObject.SetActive(false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.49f) {
                    Attack360TriggerObject.SetActive(true);
                    AttackTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.52f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.87f) {
                    Attack360TriggerObject.SetActive(false);
                    AttackTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.88f) {
                    AttackTriggerObject.SetActive(false);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                    anim.SetBool("param_360Attack", false);
                    HitPlayers = new List<GameObject>();
                    AttackTriggerObject.SetActive(false);
                    AttackStopTimer = 0;
                    FrontWeaponAttackTimer = 0;
                    TurnTimer = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontHandAttack")) {  //바로 앞의 있는 플레이어를 손으로 공격
                AttackTriggerObject.SetActive(false);
                Attack360TriggerObject.SetActive(false);
                BackAttackTriggerObject.SetActive(false);
                FrontAttackTriggerObject.SetActive(false);
                SpecialAttackTriggerObject.SetActive(false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontHandAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontHandAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.49f) {
                    Attack360TriggerObject.SetActive(false);
                    AttackTriggerObject.SetActive(false);
                    AttackHandTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontHandAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f) {
                    AttackHandTriggerObject.SetActive(false);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontHandAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                    anim.SetBool("param_FrontHandAttack", false);
                    HitPlayers = new List<GameObject>();
                    AttackHandTriggerObject.SetActive(false);
                    AttackStopTimer = 0;
                    FrontWeaponAttackTimer = 0;
                    TurnTimer = false;
                }
            }else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack")) {    //뒤에 있는 플레이어를 엉덩이로 앉아버리며 공격
                AttackTriggerObject.SetActive(false);
                Attack360TriggerObject.SetActive(false);
                AttackHandTriggerObject.SetActive(false);
                FrontAttackTriggerObject.SetActive(false);
                SpecialAttackTriggerObject.SetActive(false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.38f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.44f) {
                    BackAttackTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f) {
                    BackAttackTriggerObject.SetActive(false);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    anim.SetBool("param_BackAttack", false);
                    HitPlayers = new List<GameObject>();
                    BackAttackTriggerObject.SetActive(false);
                    AttackStopTimer = 0;
                    BackAttackTimer = 0;
                    TurnTimer = false;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack")) {   //크게 기를 모아 전방의 플레이어를 넓은 범위로 공격
                AttackTriggerObject.SetActive(false);
                Attack360TriggerObject.SetActive(false);
                AttackHandTriggerObject.SetActive(false);
                BackAttackTriggerObject.SetActive(false);
                FrontAttackTriggerObject.SetActive(false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.55f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.58f) {
                    SpecialAttackTriggerObject.SetActive(true);
                } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.59f) {
                    SpecialAttackTriggerObject.SetActive(false);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    anim.SetBool("param_SpecialAttack", false);
                    SpecialAttackTriggerObject.SetActive(false);
                    HitPlayers = new List<GameObject>();
                    AttackStopTimer = 0;
                    SpecialAttackTimer = 0;
                    TurnTimer = false;
                }
            }
            //2페이즈 진입이 끝난 후 초기화
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.97f) {
                anim.SetBool("param_Pase2", false);
                HitPlayers = new List<GameObject>();
                AttackStopTimer = 0;
                BackAttackTimer = 0;
                TurnTimer = false;
                Pase2Clear = true;
            }
        } 
    }
    //보스의 전반적인 움직임 및 사운드 설정
    private void FixedUpdate() {
        if (target.GetComponent<PlayerControl>().PlayerDead) {
            anim.SetBool("param_Move", false);
        }
        if(!enemy.IsDead && target != null && !IsDummy && anim.GetBool("param_SummonOn") && !target.GetComponent<PlayerControl>().PlayerDead) {
            //보스가 백점프를 사용할 때 움직일 거리 조정
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackJump")) {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackJump") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackJump") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.45f) {
                    rig.transform.position += transform.forward * BossBackMoveDistance;
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackJump") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.46f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackJump") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f) {
                    BossAttackMoveDistance += 0.05f;
                    if (BossAttackMoveDistance >= 0)
                        BossAttackMoveDistance = 0;
                    rig.transform.position += transform.forward * BossBackMoveDistance;
                }

            }else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1")) {   //전진하면서 무기로 공격할 때 전진하게 될 속도와 거리 조정
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.45f) {
                    AttackMoveTimer += Time.deltaTime;
                    if (AttackMoveTimer >= 0.9f)
                        AttackMoveTimer = 0.9f;
                    rig.transform.position = Vector3.Lerp(StartAttackPos, EndAttackPos, AttackMoveTimer * 1.3f);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.46f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.55f) {
                    if (!IsEffectOn) {
                        FrontWeaponAttackEffect.transform.position = BossEffectsPos[0].transform.position;
                        FrontWeaponAttackEffect.Play();
                        //공격에 맞는 사운드 실행
                        sound.PlayOneShot(soundClip[1]);
                        IsEffectOn = true;
                    }
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.MoveAttack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    IsEffectOn = false;
                    IsAttacking = false;
                    IsSoundOn2 = false;
                }
                //왼쪽 오른쪽 회전을 진행할 때 움직이는 각도와 속도 설정
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LeftTurn") || anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RightTurn")) {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LeftTurn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LeftTurn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RightTurn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.45f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RightTurn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f) {
                    Turn();
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack")) {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.26f) {
                    Turn();
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.47f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f) {
                    if (!IsEffectOn) {
                        FrontWeaponAttackEffect2.transform.position = BossEffectsPos[1].transform.position;
                        FrontWeaponAttackEffect2.Play();
                        //공격에 맞는 사운드 실행
                        sound.PlayOneShot(soundClip[1]);
                        if (Pase2Clear) {
                            StartCoroutine(LightningAttack2());
                        }
                        IsEffectOn = true;
                    }
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontWeaponAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    IsEffectOn = false;
                    IsAttacking = false;
                    IsSoundOn2 = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack")) {  //전방을 무기로 공격할 때 주시하는 대상을 일정 시간동안 따라가도록 설정
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f ||
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f) {
                    AttackMoveTimer += Time.deltaTime;
                    if (AttackMoveTimer >= 0.9f)
                        AttackMoveTimer = 0.9f;
                    rig.transform.position = Vector3.Lerp(StartAttackPos, EndAttackPos, AttackMoveTimer * 1.2f);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.42f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.48f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.76f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.82f) {
                    if (!IsEffectOn) {
                        FrontAttackEffect.transform.position = BossEffectsPos[2].transform.position;
                        //공격에 맞는 사운드 실행
                        sound.PlayOneShot(soundClip[1]);
                        FrontAttackEffect.Play();
                        if (Pase2Clear) {
                            StartCoroutine(LightningAttack());
                        }
                        IsEffectOn = true;
                    }
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.64f) {
                    AttackTurn();
                    StartAttackPos = transform.position;
                    EndAttackPos = target.position;
                    AttackMoveTimer = 0f;
                    IsEffectOn = false;
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    IsEffectOn = false;
                    IsAttacking = false;
                    IsSoundOn2 = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack")) {    //주위를 범위 공격 후 주시하는 대상을 따라가도록 설정
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.44f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.52f) {
                    transform.localEulerAngles += new Vector3(0, -10, 0);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.52f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f) {
                    AttackTurn();
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f) {
                    if (!IsEffectOn) {
                        Weapon360AttackEffect.transform.position = BossEffectsPos[4].transform.position;
                        Weapon360AttackEffect.Play();
                        IsEffectOn = true;
                    }
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.360Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    IsEffectOn = false;
                    IsAttacking = false;
                    IsSoundOn2 = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack")) {   //백어택을 사용할 때 뒤로 움직이는 거리 조정
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.36f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.41f) {
                    BossAttackMoveDistance += 0.03f;
                    if (BossAttackMoveDistance >= 0)
                        BossAttackMoveDistance = 0;
                    rig.transform.position += transform.forward * BossAttackMoveDistance;
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.42f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f) {
                    if (!IsEffectOn) {
                        BackAttackEffect.transform.position = BossEffectsPos[5].transform.position;
                        BackAttackEffect.Play();
                        sound.PlayOneShot(soundClip[1]);
                        IsEffectOn = true;
                    }
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                    IsEffectOn = false;
                    IsAttacking = false;
                    IsSoundOn2 = false;
                }
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack")) {    //보스가 공격을 위해 기를 모으는 동안 주시하는 대상을 향해 일정시간 회전함
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.51f) {
                    if (!IsEffectOn2) {
                        SpecialAttackHandEffect.Play();
                        IsEffectOn2 = true;
                    }
                    AttackTurn();
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f) {
                        agent.SetDestination(Backpos.position);
                        agent.speed = 16;
                    }
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.55f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.63f) {
                    if (!IsEffectOn) {
                        SpecialAttackEffect.Play();
                        SpecialAttackHandEffect.Stop();
                        //공격에 맞는 사운드 실행
                        sound.PlayOneShot(soundClip[3]);
                        IsEffectOn = true;
                    }
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.97f) {
                    IsEffectOn = false;
                    IsEffectOn2 = false;
                    IsAttacking = false;
                    IsSoundOn2 = false;
                    agent.speed = 8;
                }
            }
            //2페이즈 접근 시 카메라 줌아웃
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f &&
                anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.94f) {
                playerCamera.CameraZoomOutMotion = true;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.38f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.48f) {
                if (!IsSoundOn) {
                    sound.PlayOneShot(soundClip[2]);
                    IsSoundOn = true;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.44f &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.54f) {
                if (!IsEffectOn) {
                    Pase2Effect.Play();
                    Pase2BossEffect.Play();
                    IsEffectOn = true;
                }
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Pase2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                playerCamera.CameraZoomOutMotion = false;
                IsEffectOn = false;
                IsSoundOn = false;
                IsSoundOn2 = false;
                IsAttacking = false;
            }
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack")) {
                SpecialAttackHandEffect.Stop();
            }

                TargetAngle = Vector3.SignedAngle(target.position - transform.position, transform.forward, transform.up);

            //매 프레임마다 주시하는 대상을 바라보며 이동

            if (HitStopTime >= 2 && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") ||
                HitStopTime >= 2 && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Move")) {

                if (!IsAttacking && !TurnTimer && !anim.GetBool("param_Pase2") && Pase2On && !Pase2Clear && AttackStopTimer >= 1f) {
                    anim.SetBool("param_Pase2", true);
                    IsAttacking = true;
                }

                if (!IsAttacking && !TurnTimer && TargetAngle > 40 && TargetAngle < 180 && !anim.GetBool("param_LeftTurn") && AttackStopTimer >= 1f) {
                    anim.SetBool("param_LeftTurn", true);
                    TurnTimer = true;
                    IsAttacking = true;
                }else if (!IsAttacking && !TurnTimer && TargetAngle < -40 && TargetAngle > -180 && !anim.GetBool("param_RightTurn") && AttackStopTimer >= 1f) {
                    anim.SetBool("param_RightTurn", true);
                    TurnTimer = true;
                    IsAttacking = true;
                }
                if (Vector3.Distance(transform.position, target.position) <= CloseDistance && !IsAttacking) {
                    agent.SetDestination(transform.position);
                    anim.SetBool("param_Move", false);
                    //공격 가능한 범위 내에 플레이어가 존재할 시 보스가 사용 가능한 패턴 한가지를 사용하여 플레이어를 공격
                    //한번 사용된 패턴은 일정 시간이 지날 때까지 다시 사용하지 않음
                    if (Vector3.Distance(transform.position, target.position) <= VeryCloseDistance && !IsAttacking && !anim.GetBool("param_MoveAttack")) {
                        if (!IsAttacking && FrontAttackTimer >= 6 && TargetAngle > -15 && TargetAngle < 50 && !anim.GetBool("param_FrontHandAttack") && AttackStopTimer >= 1.5f) {
                            anim.SetBool("param_RightMove", false);
                            anim.SetBool("param_FrontHandAttack", true);
                            StartAttackPos = transform.position;
                            EndAttackPos = target.position;
                            AttackMoveTimer = 0;
                            IsAttacking = true;
                            IsEffectOn = false;
                            BackJumpTimer++;
                            if (!IsSoundOn2)
                                sound.PlayOneShot(soundClip[4]);
                            IsSoundOn2 = true;
                            anim.SetBool("param_360Attack", false);
                            anim.SetBool("param_BackAttack", false);
                            anim.SetBool("param_FrontWeaponAttack", false);
                            anim.SetBool("param_BackJump", false);
                            anim.SetBool("param_FrontAttack1", false);
                            anim.SetBool("param_MoveAttack", false);
                            anim.SetBool("param_SpecialAttack", false);
                        } else if (!IsAttacking && FrontAttackTimer >= 12 && !anim.GetBool("param_FrontAttack1") && AttackStopTimer >= 1.5f) {
                            anim.SetBool("param_RightMove", false);
                            anim.SetBool("param_FrontAttack1", true);
                            StartAttackPos = transform.position;
                            EndAttackPos = target.position;
                            AttackMoveTimer = 0;
                            IsAttacking = true;
                            IsEffectOn = false;
                            BackJumpTimer++;
                            if (!IsSoundOn2)
                                sound.PlayOneShot(soundClip[4]);
                            IsSoundOn2 = true;
                            anim.SetBool("param_360Attack", false);
                            anim.SetBool("param_BackAttack", false);
                            anim.SetBool("param_FrontHandAttack", false);
                            anim.SetBool("param_FrontWeaponAttack", false);
                            anim.SetBool("param_BackJump", false);
                            anim.SetBool("param_MoveAttack", false);
                            anim.SetBool("param_SpecialAttack", false);
                        } else if (!IsAttacking && BackJumpTimer >= 4 && !anim.GetBool("param_BackJump") && AttackStopTimer >= 1.5f) {
                            anim.SetBool("param_RightMove", false);
                            anim.SetBool("param_BackJump", true);
                            BackJumpTimer = 0;
                            IsAttacking = true;
                            IsEffectOn = false;
                            if (!IsSoundOn2)
                                sound.PlayOneShot(soundClip[4]);
                            IsSoundOn2 = true;
                            anim.SetBool("param_360Attack", false);
                            anim.SetBool("param_BackAttack", false);
                            anim.SetBool("param_FrontWeaponAttack", false);
                            anim.SetBool("param_FrontHandAttack", false);
                            anim.SetBool("param_FrontAttack1", false);
                            anim.SetBool("param_MoveAttack", false);
                            anim.SetBool("param_SpecialAttack", false);
                        }
                    }
                    if (!IsAttacking && Attack360Timer >= 16 && !anim.GetBool("param_360Attack") && AttackStopTimer >= 1.5f && !anim.GetBool("param_MoveAttack")) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_360Attack", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        Attack360Timer = 0;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if (!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_BackAttack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_FrontWeaponAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_FrontAttack1", false);
                        anim.SetBool("param_MoveAttack", false);
                        anim.SetBool("param_SpecialAttack", false);
                    } else if (!IsAttacking && BackAttackTimer >= 10 && TargetAngle < -120 && TargetAngle > -180 && !anim.GetBool("param_BackAttack") && AttackStopTimer >= 1.5f ||
                        !IsAttacking && BackAttackTimer >= 10 && TargetAngle > 120 && TargetAngle < 180 && !anim.GetBool("param_BackAttack") && AttackStopTimer >= 1.5f) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_BackAttack", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        BossAttackMoveDistance = -1.5f;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if (!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_360Attack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_FrontWeaponAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_FrontAttack1", false);
                        anim.SetBool("param_MoveAttack", false);
                        anim.SetBool("param_SpecialAttack", false);
                    } else if (!IsAttacking && Pase2Clear && SpecialAttackTimer >= 24 && !anim.GetBool("param_SpecialAttack") && AttackStopTimer >= 1.5f && !anim.GetBool("param_MoveAttack")) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_SpecialAttack", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        Attack360Timer = 0;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if (!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_360Attack", false);
                        anim.SetBool("param_BackAttack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_FrontWeaponAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_FrontAttack1", false);
                        anim.SetBool("param_MoveAttack", false);
                    }
                } else if (Vector3.Distance(transform.position, target.position) <= AttackDistance && !IsAttacking && !anim.GetBool("param_MoveAttack")) {
                    agent.SetDestination(transform.position);
                    anim.SetBool("param_Move", false);
                    if (!IsAttacking && BackAttackTimer >= 10 && TargetAngle < -120 && TargetAngle > -180 && !anim.GetBool("param_BackAttack") && AttackStopTimer >= 1.5f ||
                        !IsAttacking && BackAttackTimer >= 10 && TargetAngle > 120 && TargetAngle < 180 && !anim.GetBool("param_BackAttack") && AttackStopTimer >= 1.5f) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_BackAttack", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        BossAttackMoveDistance = -1.5f;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if (!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_360Attack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_FrontWeaponAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_FrontAttack1", false);
                        anim.SetBool("param_MoveAttack", false);
                        anim.SetBool("param_SpecialAttack", false);
                    } else if (!IsAttacking && Pase2Clear && SpecialAttackTimer >= 24 && !anim.GetBool("param_SpecialAttack") && AttackStopTimer >= 1.5f && !anim.GetBool("param_MoveAttack")) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_SpecialAttack", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        Attack360Timer = 0;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if (!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_360Attack", false);
                        anim.SetBool("param_BackAttack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_FrontWeaponAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_FrontAttack1", false);
                        anim.SetBool("param_MoveAttack", false);
                    } else if (!IsAttacking && FrontAttackTimer >= 14 && !anim.GetBool("param_FrontAttack1") && AttackStopTimer >= 1.5f) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_FrontAttack1", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if (!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_360Attack", false);
                        anim.SetBool("param_BackAttack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_FrontWeaponAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_MoveAttack", false);
                        anim.SetBool("param_SpecialAttack", false);
                    } else if (!IsAttacking && FrontWeaponAttackTimer >= 10 && !anim.GetBool("param_FrontWeaponAttack") && AttackStopTimer >= 1.5f) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_FrontWeaponAttack", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if (!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_360Attack", false);
                        anim.SetBool("param_BackAttack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_FrontAttack1", false);
                        anim.SetBool("param_MoveAttack", false);
                        anim.SetBool("param_SpecialAttack", false);
                    }
                } else if (Vector3.Distance(transform.position, target.position) <= MoveDistance && !IsAttacking) {
                    anim.SetBool("param_Move", true);
                    agent.SetDestination(target.position);
                } else if (Vector3.Distance(transform.position, target.position) <= FarDistance && !IsAttacking) {
                    anim.SetBool("param_Move", true);
                    agent.SetDestination(target.position);
                    if (!IsAttacking && MoveAttackTimer >= 7 && !anim.GetBool("param_MoveAttack") && AttackStopTimer >= 1.5f) {
                        anim.SetBool("param_RightMove", false);
                        anim.SetBool("param_MoveAttack", true);
                        StartAttackPos = transform.position;
                        EndAttackPos = target.position;
                        AttackMoveTimer = 0;
                        IsAttacking = true;
                        IsEffectOn = false;
                        BackJumpTimer++;
                        if(!IsSoundOn2)
                            sound.PlayOneShot(soundClip[4]);
                        IsSoundOn2 = true;
                        anim.SetBool("param_360Attack", false);
                        anim.SetBool("param_BackAttack", false);
                        anim.SetBool("param_FrontHandAttack", false);
                        anim.SetBool("param_FrontWeaponAttack", false);
                        anim.SetBool("param_BackJump", false);
                        anim.SetBool("param_FrontAttack1", false);
                    }
                } else {
                    anim.SetBool("param_Move", true);
                    if (anim.GetBool("param_MoveAttack") || anim.GetBool("param_FrontAttack1")) {
                        agent.SetDestination(transform.position);
                    } else if(!IsAttacking){
                        agent.SetDestination(target.position);
                    }
                }
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Move")) {
                IsAttacking = false;
                IsEffectOn = false;
                IsEffectOn2 = false;
            }
        }
    }
    //보스의 한 패턴이 끝났을 때 보스의 공격에 관련된 콜라이더를 전부 꺼줌
    public void AttackColliderFalse() {
        AttackTriggerObject.SetActive(false);
        Attack360TriggerObject.SetActive(false);
        AttackHandTriggerObject.SetActive(false);
        BackAttackTriggerObject.SetActive(false);
        FrontAttackTriggerObject.SetActive(false);
        SpecialAttackTriggerObject.SetActive(false);
    }

    //보스 회전
    void Turn() {
        Vector3 targetDirection = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200 * Time.deltaTime);
        nextRotation.eulerAngles = new Vector3(0, nextRotation.eulerAngles.y, 0);
        transform.rotation = nextRotation;
    }
    //패턴 중에 플레이어에게 회전할 시 보스의 속도
    void AttackTurn() {
        Vector3 targetDirection = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 400 * Time.deltaTime);
        nextRotation.eulerAngles = new Vector3(0, nextRotation.eulerAngles.y, 0);
        transform.rotation = nextRotation;
    }
    //보스 체력바 위치 설정
    void SetHpBarOffset() {
        EnemyDamage enemydamaged = GetComponent<EnemyDamage>();
        enemydamaged.hpBarOffset = hpBarOffset;
        enemydamaged.SetHpBar();
    }
    //2페이즈에서의 패턴 내에 추가되는 번개 공격
    IEnumerator LightningAttack() {
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[6].transform.position;
        yield return new WaitForSeconds(0.15f);
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[7].transform.position;
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[8].transform.position;
        yield return new WaitForSeconds(0.15f);
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[9].transform.position;
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[10].transform.position;
        yield return null;
    }
    //2페이즈에서의 패턴 내에 추가되는 번개 공격
    IEnumerator LightningAttack2() {
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[11].transform.position;
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[12].transform.position;
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[13].transform.position;
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[14].transform.position;
        obj = Instantiate<GameObject>(LightningAttackObject);
        obj.transform.position = BossEffectsPos[15].transform.position;
        yield return null;
    }
    //플레이어가 보스의 특정한 공격에 맞았을 시 뒤로 밀리는 모션 설정
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (!HitPlayers.Contains(other.gameObject)) {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontHandAttack")) {
                    other.gameObject.GetComponent<PlayerDamageCalculation>().HitPush = true;
                    HitPlayers.Add(other.gameObject);
                    other.gameObject.GetComponent<PlayerDamageCalculation>().TakeBigDamage((int)damage / 2, gameObject);
                } else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpecialAttack")) {
                    other.gameObject.GetComponent<PlayerDamageCalculation>().HitFallDown = true;
                    HitPlayers.Add(other.gameObject);
                    other.gameObject.GetComponent<PlayerDamageCalculation>().TakeBigDamage(damage * 2, gameObject);
                }else {
                    other.gameObject.GetComponent<PlayerDamageCalculation>().HitFallDown = true;
                    HitPlayers.Add(other.gameObject);
                    other.gameObject.GetComponent<PlayerDamageCalculation>().TakeBigDamage(damage, gameObject);
                }
            }
        }
    }
}
