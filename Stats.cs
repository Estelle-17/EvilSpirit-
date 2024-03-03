/*
* Writer : KimJunWoo
*
* 이 소스코드는 플레이어의 스탯 대한 내용이 작성되어 있음
*
* Last Update : 2024/03/03
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    //플레이어 기본 스탯
    public int Level;
    public int RequireExp;
    public int Exp;
    public int GhostPoint;
    public float Hp;
    public float HpRestoration;
    public float SkillBar;
    public float SkillStack;
    public float Damage;
    public float AdditionalDamage;
    public float TotalDamage;
    public float Armor;
    public float AttackSpeed;
    public float MoveSpeed;
    public float RunSpeed;

    //
    public int[] SkillLevels;
    public bool isHolding;
    public ItemSearch itemsearch;
    public GameObject weaponHand;
    public GameObject currentWeapon;
    public GameObject[] Skills;
    public GameObject MyScroll;
    public int MyScrollUpgradeCount;
    public GameObject[] LootItems;
    public int LootItemCount;
    public int ScrollItemKind;

    public PlayerDamageCalculation damageCalc;
    public ParticleSystem UpgradeEffect;

    void Start()
    {
        weaponHand = GameObject.Find("WarrerRightHand");
        LootItems = new GameObject[9];
        GhostPoint = 0;
        Level = 1;
        LootItemCount = 0;
        AdditionalDamage = 0;
        RequireExp = 100;
        MyScrollUpgradeCount = 1;
        SkillBar = 0;
        SkillStack = 0;
        SkillLevels = new int[8];
        damageCalc = GetComponent<PlayerDamageCalculation>();
    }
    //몬스터를 죽일 시 경험치를 얻으며 100%가 되었을 시 레벨업
    void GetExp(int EnemyExp)
    {
        Exp += EnemyExp;
        if(Exp >= RequireExp)
        {
            ++Level;
            LevelUpStats();
            Exp -= RequireExp;
            RequireExp += 50 * Level;
        }
    }
    //레벨업 시 증가하는 스탯
    public void LevelUpStats()
    {
        Hp += 15;
        Damage += 3;
        damageCalc.MaxHP = Hp;
        damageCalc.currentHp += 15;
        GetComponent<PlayerControl>().ui.HealCount = 5;
        GetComponent<PlayerControl>().ui.HealBar();
    }
    //시작할 때 들고 있는 무기를 플레이어와 연결
    public void GetWeapon(GameObject newWeapon)
    {
        currentWeapon = newWeapon;
        currentWeapon.GetComponent<Weapons>().Player = this.gameObject;
        currentWeapon.GetComponent<Weapons>().ApplyAttackSpeed();
        GetComponent<Attack>().SetWeaponCollider(currentWeapon);
        currentWeapon.GetComponentInChildren<AttackCollider>().SetOwner(this.gameObject);
    }

    public void GetScroll(GameObject NewScroll) {
        if(MyScroll == null) {
            MyScroll = NewScroll;
            NewScroll.GetComponent<ScrollItemsScript>().Owner = this.gameObject;
            NewScroll.GetComponent<ScrollItemsScript>().ApplyItem();
  
            GetComponent<Attack>().ScrollItem = NewScroll.GetComponent<ScrollItemsScript>();
            ScrollItemKind = NewScroll.GetComponent<ScrollItemsScript>().ItemKinds;
 
            itemsearch.ItemRemove(NewScroll);
        }
    }
    public void DropScroll() {
        if(MyScroll != null) {
            MyScroll = null;
            GetComponent<Attack>().ScrollItem = null;
            if (ScrollItemKind == 2) {
                ScrollItemKind = 0;
            }
        }
    }
    //아이템 획득 시 점수 설정
    public void GetItem(GameObject newItem)
    {
        if (LootItemCount < 9) {
            for (int i = 0; i < 9; i++) {
                if (LootItems[i] == null) {
                    LootItems[i] = newItem;
                    LootItemCount++;
                    newItem.GetComponent<ItemScript>().Owner = this.gameObject;
                    newItem.GetComponent<ItemScript>().ApplyItem();
                    itemsearch.ItemRemove(newItem);
                    break;
                }
            }
        }
    }
    public void DropItem(int Num) {
        if(LootItems[Num] != null) {
            LootItems[Num] = null;
            LootItemCount--;
            currentWeapon.GetComponentInChildren<AttackCollider>().SetDamage();
        }
    }
    //데미지가 변동될 시 스탯에 적용
    public void UpdateDamage() {
        currentWeapon.GetComponentInChildren<AttackCollider>().SetDamage();
    }

    public void BloodScrollHealthRecovery() {
        if(MyScroll != null)
            if(MyScroll.GetComponent<ScrollItemsScript>().ItemKinds == 2)
                MyScroll.GetComponent<ScrollItemsScript>().BloodScrollHealthRecovery();
    }
}
