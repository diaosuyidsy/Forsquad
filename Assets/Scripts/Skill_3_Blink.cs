using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_3_Blink : MonoBehaviour
{

    [Tooltip ("The distance of the blink")]
    public float Range = 10f;

    public float CoolDown = 10f;

    private float timer = 10f;

    // Update is called once per frame
    void Update ()
    {
        // Add the time since Update was last called to the timer.
        if (timer <= CoolDown)
            timer += Time.deltaTime;

        //// If the Fire1 button is being press and it's time to fire...
        //if (Input.GetButton ("A") && timer >= timeBetweenBullets)
        //{
        //    // ... shoot the gun.
        //    Shoot ();
        //}

    }

    /// <summary>
    /// This method is the only method that communicates with other class
    /// Will be called mostly from PlayerController
    /// </summary>
    public void OnUseSkill ()
    {
        if (timer >= CoolDown)
        {
            Blink ();
        }
    }

    private void Blink ()
    {
        // First set timer to 0
        timer = 0f;

        transform.parent.parent.position += (transform.parent.parent.up * Range);
    }
}
