/*
* Writer : KimJunWoo
*
* 이 소스코드는 보스 등장 카메라에 대한 내용이 작성되어 있음
*
* Last Update : 2024/03/03
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraSceneScript : MonoBehaviour
{
    public GM gm;
    public Animator anim;
    public BossMoveScript boss;
    public bool FadeIn;
    Vector3 vec;
    
    public AudioSource sound;
    public AudioClip[] BGMClip;

    void Start()
    {
        FadeIn = false;
        anim = GetComponent<Animator>();
        anim.speed = 0f;
        boss.gameObject.SetActive(false);
        transform.root.gameObject.SetActive(false);
        vec = (new Vector3(470.3f, boss.transform.localPosition.y, 1044.1f) - boss.transform.localPosition).normalized;
    }

    void Update()
    {
        //보스 카메라 무빙
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SummonCameraAnim") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f && 
                                                                                                                                     anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.49f) {
            boss.gameObject.SetActive(true);
            boss.transform.localPosition += vec / 7;
            if(boss.transform.localPosition.x < 470.3f) {
                boss.transform.localPosition = new Vector3(470.3f, boss.transform.localPosition.y, boss.transform.localPosition.z);
            }
            if (boss.transform.localPosition.z > 1044.1f) {
                boss.transform.localPosition = new Vector3(boss.transform.localPosition.x, boss.transform.localPosition.y, 1044.1f);
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SummonCameraAnim") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f) {
            boss.anim.speed = 1f;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SummonCameraAnim") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.999f) {
            if (!FadeIn) {
                gm.StartCoroutine(gm.SceneOutFadeIn());
                FadeIn = true;
            }
        }
    }
}
