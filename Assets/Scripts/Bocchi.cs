using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bocchi : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    float force = 70f; // ÈûÀ» °¡ÇÒ °ª
    bool isblocking = false;
    float A = 0.1f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(run());
        }

    }

    IEnumerator run()
    {
        if (isblocking == false)
        {
            rb.AddForce(Vector3.right * -1 * force, ForceMode.Impulse);
            while (!isblocking)
            {
                force += Time.deltaTime;
                transform.Rotate(0, 0, A);
                yield return null;
            }
        }
        else
        {
            Vector3 Vector3 = Vector3.zero;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("block"))
        {
            isblocking = true;
        }
    }

}
