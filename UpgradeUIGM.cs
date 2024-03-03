/*
* Writer : KimJunWoo
*
* 이 소스코드는 스킬 및 성능 업그레이드UI에 대한 내용이 작성되어 있음
*
* Last Update : 2024/03/02
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIGM : MonoBehaviour
{
    public GameObject SkillUIimage;
    public GameObject attackImage;
    public GameObject defenceImage;
    public GameObject utilityImage;
    public Text DamageText;
    GameObject obj;
    public int[,] selectArray;
    public GameObject[] skillUIArray;

    void Start()
    {
        selectArray = new int[2,8];
        SetRandomUpgradeCategory();
        setUpgradeCategoryUI();
    }
    //UI에 글씨
    public void SetDamageText(int num) {
        DamageText.text = "데미지 증가 - 혼" + num * 50 + "개\n";
    }
    //모든 업그레이드 중 4가지를 골라 배열에 저장
    public void SetRandomUpgradeCategory() {
        for(int  j = 0; j < 4; j++) {
            selectArray[0, j] = Random.Range(0, 7);
            for(int i = 0; i < j; i++) {
                if(selectArray[0, i] == selectArray[0, j]) {
                    i = j;
                    j--;
                }
            }
        }
    }
    //배열에 저장된 업그레이드 4가지를 화면에 띄워줌
    public void setUpgradeCategoryUI() {
        for (int j = 0; j < 4; j++) {
            obj = Instantiate<GameObject>(skillUIArray[selectArray[0, j]], this.transform);
            obj.GetComponent<UpgradeUIMouseClick>().ItemNumber = j + 1;
            obj.GetComponent<UpgradeUIMouseClick>().SkillNum = selectArray[0, j];
            switch (j + 1) {
                case 1:
                    obj.transform.localPosition = new Vector3(-675, 0, 0);
                    break;
                case 2:
                    obj.transform.localPosition = new Vector3(-225, 0, 0);
                    break;
                case 3:
                    obj.transform.localPosition = new Vector3(225, 0, 0);
                    break;
                case 4:
                    obj.transform.localPosition = new Vector3(675, 0, 0);
                    break;
            }
        }
    }
    //기존 배열에 저장된 업그레이드 내용을 초기화 후 새로운 업그레이드 4가지를 화면에 띄워줌
    public void setUpgradeResetCategoryUI() {
        for (int j = 0; j < 4; j++) {
            obj = Instantiate<GameObject>(skillUIArray[selectArray[0, j]], this.transform);

            obj.GetComponent<UpgradeUIMouseClick>().IsReseting = true;
            obj.transform.localScale = new Vector3(0, 1, 1);
            obj.GetComponent<UpgradeUIMouseClick>().ItemNumber = j + 1;
            obj.GetComponent<UpgradeUIMouseClick>().SkillNum = selectArray[0, j];

            switch (j + 1) {
                case 1:
                    obj.transform.localPosition = new Vector3(-675, 0, 0);
                    break;
                case 2:
                    obj.transform.localPosition = new Vector3(-225, 0, 0);
                    break;
                case 3:
                    obj.transform.localPosition = new Vector3(225, 0, 0);
                    break;
                case 4:
                    obj.transform.localPosition = new Vector3(675, 0, 0);
                    break;
            }
        }
    }
}