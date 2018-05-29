using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float velocity = 2f;
    // Used for turning
    private Vector3 turnSignPos;

    // Update is called once per frame
    void Update ()
    {
        Move ();
    }

    private void Move ()
    {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            Vector3 moveDirection = new Vector3 (moveHorizontal, moveVertical);
            transform.Translate (moveDirection * velocity * Time.deltaTime, Space.World);
            turnSignPos = transform.position + new Vector3 (moveHorizontal, moveVertical);
            Vector3 diff = turnSignPos - transform.position;
            diff.Normalize ();

            float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90f);
        }


    }

}
