using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float velocity = 2f;
    public GameObject BulletPrefab;
    // Used for turning
    private Vector3 turnSignPos;

    // Update is called once per frame
    void Update ()
    {
        Move ();
    }

    private void Move ()
    {
        Vector3 moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
        transform.Translate (moveDirection * velocity * Time.deltaTime, Space.World);

        // Turn Function, only works if player hit secondary joystick
        float turnHorizontal = Input.GetAxis ("FaceHorizontal");
        float turnVertical = Input.GetAxis ("FaceVertical");
        if (turnHorizontal != 0 || turnVertical != 0)
        {
            turnSignPos = transform.position + new Vector3 (turnHorizontal, -1f * turnVertical);
            Vector3 diff = turnSignPos - transform.position;
            diff.Normalize ();

            float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90f);
        }
    }

}
