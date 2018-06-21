using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [Tooltip ("Max Health Point")]
    public float maxHealthPoint = 100;

    [Tooltip ("Max Armor, defalt is 1, meaning taking 100% damage")]
    [Range (0f, 2.0f)]
    public float maxArmor = 1f;

    [Tooltip ("Max Walk Speed, default is 1, meaning walking at 100% speed. Can get to infinitly large")]
    public float maxWalkSpeed = 1f;

    [Tooltip ("Poison Rate, tick per 1s")]
    public float poisonRate = 2f;

    [Tooltip ("Continue Healing Rate, tick per 0.5s")]
    public float continueHealRate = 2f;

    public bool Asleep = false;

    public bool Stunned = false;

    public bool Immunity = false;

    public GameObject BloodParPrefab;


    private float Armor;
    private float HealthPoint;
    private float walkSpeed;
    private IEnumerator slowCoroutine;
    private IEnumerator sleepCoroutine;
    private IEnumerator movementCoroutine;
    private float poisonTimer = 0f;
    private float poisonTickTimer = 1f;
    private float maxPosionTickTimer = 1f;
    private float continueHealTimer = 0f;
    private float continueHealTickTimer = 0.5f;
    private float maxContinueHealTickTimer = 0.5f;
    private float stunTimer = 0f;

    // Use this for initialization
    void Start ()
    {
        HealthPoint = maxHealthPoint;
        Armor = maxArmor;
        walkSpeed = maxWalkSpeed;
    }

    void Update ()
    {
        if (poisonTimer > 0f)
        {
            OnPoison ();
        }
        if (continueHealTimer > 0f)
        {
            OnContinueHeal ();
        }
        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            Stunned = true;
        }
        else
        {
            Stunned = false;
        }
    }


    public void OnHit (float rawDmg)
    {
        if (Immunity) return;
        // First, deal with visual， on hit drew blood
        Instantiate (BloodParPrefab, transform.position, Quaternion.identity);

        float realDMG = rawDmg * (1f + maxArmor - Armor);
        HealthPoint -= realDMG;
        if (HealthPoint <= 0f)
        {
            Destroy (gameObject);
        }
    }
    #region Debuff Region
    // deb: Debuff type, according to the enum Debuff below
    // time: Debuff consist time
    // ExtraParam: Extra Parameter, it means different things with different type of 
    // debuffs
    // ArmorBreak: how many armor to break
    // Slow: Slow Rate (Slow effect always choose the later applied effect)
    public void OnDebuff (Debuff deb, float time, float extraParam)
    {
        if (Immunity) return;
        switch (deb)
        {
            // Slow effect works in a overwritten style
            case Debuff.slow:
                // First reset speed and stop previous slow effect
                walkSpeed = maxWalkSpeed;
                if (slowCoroutine != null)
                    StopCoroutine (slowCoroutine);
                // Then start a new slow effect coroutine
                slowCoroutine = OnSlow (time, extraParam);
                StartCoroutine (slowCoroutine);
                break;
            // Armor break works in a stacking style
            case Debuff.armorbreak:
                if (Armor - extraParam >= 0)
                {
                    StartCoroutine (OnArmorBreak (time, extraParam));
                }
                break;
            // Posion stacks on time
            case Debuff.poison:
                poisonTimer += time;
                break;
            // Sleep effect refreshes
            case Debuff.sleep:
                Asleep = false;
                if (sleepCoroutine != null) StopCoroutine (sleepCoroutine);
                sleepCoroutine = OnSleep (time);
                StartCoroutine (sleepCoroutine);
                break;
            // Stun effect chooses which effect last longer and go with it.
            case Debuff.stun:
                if (stunTimer <= time)
                    stunTimer = time;
                break;

        }
    }

    IEnumerator OnSleep (float time)
    {
        Asleep = true;
        yield return new WaitForSeconds (time);
        Asleep = false;
    }

    private void OnPoison ()
    {
        poisonTimer -= Time.deltaTime;
        poisonTickTimer -= Time.deltaTime;
        if (poisonTickTimer <= 0f)
        {
            poisonTickTimer = maxPosionTickTimer;
            OnHit (poisonRate);
        }
    }

    // ArmorBreak effect stacks, but does not exceed 0
    IEnumerator OnArmorBreak (float time, float armorbreakRate)
    {
        Armor -= armorbreakRate;
        yield return new WaitForSeconds (time);
        Armor += armorbreakRate;
    }

    // Rate can be more than 1 but walkSpeed cannot be more than 1
    // When walkspeed is 0, slow becomes root.
    IEnumerator OnSlow (float time, float rate)
    {
        walkSpeed -= rate;
        if (walkSpeed < 0) walkSpeed = 0;
        // TODO: There should be special effect here
        yield return new WaitForSeconds (time);
        walkSpeed = maxWalkSpeed;
    }

    #endregion

    #region Buff Region
    public void OnBuff (Buff buf, float time, float extraParam)
    {
        switch (buf)
        {
            case Buff.armorgain:
                if (Armor + extraParam < 2)
                {
                    StartCoroutine (OnArmorBreak (time, -extraParam));
                }
                break;
            case Buff.cleanse:
                CleanseMovementDebuff ();
                break;
            // Continue Healing stacks on time
            case Buff.continueheal:
                continueHealTimer += time;
                break;
            case Buff.instaheal:
                OnInstaheal (extraParam);
                break;
            case Buff.movement:
                // First reset speed and stop previous speed effect
                walkSpeed = maxWalkSpeed;
                if (movementCoroutine != null)
                    StopCoroutine (movementCoroutine);
                // Then start a new slow effect coroutine
                movementCoroutine = OnSpeed (time, extraParam);
                StartCoroutine (movementCoroutine);
                break;
            case Buff.immunity:
                // First cleanse debuff
                CleanseAllDebuff ();
                StartCoroutine (OnImmunity (time));
                break;
        }
    }

    IEnumerator OnImmunity (float time)
    {
        Immunity = true;
        yield return new WaitForSeconds (time);
        Immunity = false;
    }

    IEnumerator OnSpeed (float time, float rate)
    {
        walkSpeed += rate;
        if (walkSpeed < 0) walkSpeed = 0;
        // TODO: There should be special effect here
        yield return new WaitForSeconds (time);
        walkSpeed = maxWalkSpeed;
    }

    private void OnContinueHeal ()
    {
        continueHealTimer -= Time.deltaTime;
        continueHealTickTimer -= Time.deltaTime;
        if (continueHealTimer <= 0f)
        {
            continueHealTimer = maxContinueHealTickTimer;
            OnInstaheal (continueHealRate);
        }
    }

    private void OnInstaheal (float amount)
    {
        // If added Health is more than max health possible, then don't heal
        HealthPoint = (HealthPoint + amount) > maxHealthPoint ? maxHealthPoint : (HealthPoint + amount);
    }

    private void CleanseMovementDebuff ()
    {
        // First restore speed
        walkSpeed = maxWalkSpeed;
        if (slowCoroutine != null)
            StopCoroutine (slowCoroutine);
        // Restore sleep effect
        Asleep = false;
        if (sleepCoroutine != null) StopCoroutine (sleepCoroutine);
        // Restore stun effect
        stunTimer = 0f;
        Stunned = false;
    }

    private void CleanseAllDebuff ()
    {
        CleanseMovementDebuff ();
        // Still have poison to cleanse;
        poisonTimer = 0f;
    }

    #endregion
    public float getWalkSpeed ()
    {
        return walkSpeed;
    }
}

public enum Debuff { slow, poison, stun, armorbreak, sleep };
public enum Buff { movement, cleanse, instaheal, continueheal, armorgain, immunity }
