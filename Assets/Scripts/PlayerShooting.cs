using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;                  // The damage inflicted by each bullet.
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public float range = 100f;                      // The distance the gun can fire.
    public LayerMask shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    public GameObject ObstacleHitParticlePrefab;
    float timer;                                    // A timer to determine when to fire.
    Ray2D shootRay;                                   // A ray from the gun end forwards.
    RaycastHit2D shootHit;                            // A raycast hit to get information about what was hit.
    LineRenderer gunLine;                           // Reference to the line renderer.
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
    Light gunLight;

    void Awake ()
    {

        // Set up the references.
        gunLine = GetComponent<LineRenderer> ();
        gunLight = GetComponent<Light> ();
    }

    void Update ()
    {
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the Fire1 button is being press and it's time to fire...
        if (Input.GetButton ("A") && timer >= timeBetweenBullets)
        {
            // ... shoot the gun.
            Shoot ();
        }

        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects ();
        }
    }

    public void DisableEffects ()
    {
        // Disable the line renderer and the light.
        gunLine.enabled = false;
        gunLight.enabled = false;
    }

    void Shoot ()
    {
        // Reset the timer.
        timer = 0f;

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLight.enabled = true;
        //       gunLine.SetPosition (0, transform.position);

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
        shootRay.origin = transform.position;
        shootRay.direction = transform.parent.up;
        shootHit = Physics2D.Raycast (shootRay.origin, shootRay.direction, range, shootableMask);
        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (shootHit.collider != null)
        {

            // Set the second position of the line renderer to the point the raycast hit.
            float distance = shootHit.transform.position.y - transform.position.y;
            gunLine.SetPosition (1, new Vector2 (0, Vector2.Distance (shootHit.point, transform.position) / 2));
            Debug.Log (shootHit.collider.tag);
            if (shootHit.collider.tag == "obstacle")
            {
                Instantiate (ObstacleHitParticlePrefab, shootHit.point, Quaternion.identity);
            }
            else if (shootHit.collider.tag == "Enemy")
            {
                shootHit.collider.gameObject.GetComponent<EnemyStats> ().OnHit (damagePerShot);
            }
        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            // ... set the second position of the line renderer to the fullest extent of the gun's range.
            gunLine.SetPosition (1, new Vector2 (0, range));
        }
    }
}