using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GMFollow : MonoBehaviour
{
    public Transform cameraTarget;
    public Vector3 offset;

    public float speed = 10.0f;


    private Camera thisCamera;
    private Vector3 worldDefalutForward;

    private void Start()
    {
        thisCamera = GetComponent<Camera>();
        worldDefalutForward = transform.forward;
    }
    void Update()
    {
        transform.localPosition = cameraTarget.position + offset;      

        float scroll = Input.GetAxis("Mouse ScrollWheel") * -speed;

        //최대 줌인
        if (thisCamera.fieldOfView <= 20.0f && scroll < 0)
        {
            thisCamera.fieldOfView = 20.0f;
        }
        // 최대 줌 아웃
        else if (thisCamera.fieldOfView >= 60.0f && scroll > 0)
        {
            thisCamera.fieldOfView = 60.0f;
        }
        // 줌인 아웃 하기.
        else
        {
            thisCamera.fieldOfView += scroll;
        }

        // 일정 구간 줌으로 들어가면 캐릭터를 바라보도록 한다.
        if (cameraTarget && thisCamera.fieldOfView <= 30.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation
                , Quaternion.LookRotation(cameraTarget.position - transform.position)
                , 0.15f);
        }
        // 일정 구간 밖에서는 원래의 카메라 방향으로 되돌아 가기.
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation
                , Quaternion.LookRotation(worldDefalutForward)
                , 0.15f);
        }
    }
}
