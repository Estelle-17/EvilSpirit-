/*
* Writer : KimJunWoo
*
* 이 소스코드는 플레이어를 따라다니는 카메라 코드가 작성되어 있음
*
* Last Update : 2024/03/03
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    public float lookSensitivity = 2;
    public float cameraRotationLimit = 65;
    public float currentCameraRotationX;
    public float currentCameraRotationY;
    public bool StopInput;
    public bool CameraZoomOutMotion;

    public Transform Target;
    public Animator TargetAnim;
    public Transform LastPosition;
    public GameObject MainCamera;
    public LayerMask layerMask;

    private Vector3 dir;
    private float camera_dist = 0f;
    private float camera_fixDist = 0.7f;
    private float camera_width = -20f;
    private float camera_height = 3f;

    void Start() {
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        TargetAnim = Target.transform.GetComponent<Animator>();
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);
        dir = new Vector3(0, camera_height, camera_width).normalized;
        StopInput = true;
        CameraZoomOutMotion = false;
    }

    void Update() {
        if(!StopInput)
            CameraRotation();
        CameraRaycast();
    }

    void LateUpdate() {
        CameraMove();
    }

    private void CameraMove() {
        var _pControl = Target.GetComponent<PlayerControl>();
        //플레이어의 이동 상태에 따른 카메라 위치 변경
        if (CameraZoomOutMotion) {
            camera_width = Mathf.Lerp(camera_width, -45, 3 * Time.deltaTime);
            camera_height = Mathf.Lerp(camera_height, 4, 3 * Time.deltaTime);
        } else {
            if (_pControl.isRun) {
                camera_width = Mathf.Lerp(camera_width, -30, 3 * Time.deltaTime);
                camera_height = Mathf.Lerp(camera_height, 4, 3 * Time.deltaTime);
            } else {
                camera_width = Mathf.Lerp(camera_width, -20, 3 * Time.deltaTime);
                camera_height = Mathf.Lerp(camera_height, 3, 3 * Time.deltaTime);
            }
        }
        transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 12, Target.transform.position.z);
    }
    private void CameraRotation() {
        //마우스를 통해 플레이어의 카메라를 회전
        float _xRotation = Input.GetAxisRaw("Mouse X");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;

        float _yRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationY = _yRotation * lookSensitivity;
        currentCameraRotationY -= _cameraRotationY;

        currentCameraRotationY = Mathf.Clamp(currentCameraRotationY, -cameraRotationLimit + 20, cameraRotationLimit);

        transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, currentCameraRotationY, 15 * Time.deltaTime),
                                                                            Mathf.LerpAngle(transform.localEulerAngles.y, -currentCameraRotationX, 15 * Time.deltaTime), 0);
    }

    private void CameraRaycast() {
        camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);
        dir = new Vector3(0, camera_height, camera_width).normalized;
        //레이저를 쏘는 벡터값
        Vector3 ray_target = transform.up * camera_height + transform.forward * camera_width;

        RaycastHit hit;
        Physics.Raycast(transform.position, ray_target, out hit, camera_dist, layerMask);

        if(hit.point != Vector3.zero) {//레이캐스트 성공시
            //point로 옮긴다
            LastPosition.position = hit.point;
            LastPosition.Translate(dir * -1 * camera_fixDist);
        } else {
            //로컬좌표를 0으로 맞춘다.
            LastPosition.localPosition = Vector3.zero;
            //카메라위치까지의 방향벡터 * 카메라 최대거리로 옮긴다
            LastPosition.Translate(dir * camera_dist);
            LastPosition.Translate(dir * -1 *  camera_fixDist);
        }
        MainCamera.transform.position = new Vector3(Mathf.Lerp(MainCamera.transform.position.x, LastPosition.position.x, 14 * Time.deltaTime), 
                                                                                    Mathf.Lerp(MainCamera.transform.position.y, LastPosition.position.y, 14 * Time.deltaTime),
                                                                                    Mathf.Lerp(MainCamera.transform.position.z, LastPosition.position.z, 14 * Time.deltaTime));
    }
}
