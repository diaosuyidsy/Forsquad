using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Enemy_1_Charge : Action
{
    public SharedFloat ChargeCoolDown = 20f;
    public SharedGameObject Target;
    public SharedFloat ChargeMaxRange = 3.6f;
    public SharedFloat ChargeMinRange = 1.5f;
    public SharedFloat ChargeStunTime = 1f;
    public SharedFloat ChargeRawDamage = 0f;
    public SharedFloat ChargeSpeed = 2f;

    bool charging = false;
    float timer = 0f;
    bool timerStart = false;

    public override TaskStatus OnUpdate ()
    {
        if (timerStart)
        {
            timer += Time.deltaTime;
            if (timer >= ChargeCoolDown.Value)
            {
                timerStart = false;
                timer = 0f;
            }
        }
        // If target is null or the target went out of max range or the target went within min range
        // this action will fail and thus return TaskStatus.Success.
        if (Target.Value == null || Vector2.SqrMagnitude (transform.position - Target.Value.transform.position) >= (ChargeMaxRange.Value * ChargeMaxRange.Value)
            || Vector2.SqrMagnitude (transform.position - Target.Value.transform.position) <= (ChargeMinRange.Value * ChargeMinRange.Value))
        {
            charging = false;
            return TaskStatus.Success;
        }
        // If program enters here, then it means we could start charging if everything is set.
        if (!charging && Mathf.Approximately (timer, 0f))
        {
            // Start Charging and start timer
            charging = true;
            timerStart = true;
            // When charging just started
            // We need to stun target for ChargeStunTime
            Target.Value.GetComponent<PlayerStats> ().OnDebuff (Debuff.stun, ChargeStunTime.Value, 0f);
        }
        // If is charging, then return Taskstatus running
        if (charging)
        {
            // TODO:Need to face the player all the time
            // Need to make enemy always face player
            Vector3 diff = Target.Value.transform.position - transform.position;
            diff.Normalize ();

            float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90f);

            transform.position += (transform.up * Time.deltaTime * ChargeSpeed.Value);
            return TaskStatus.Running;
        }

        return TaskStatus.Success;
    }
}