using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class EnemyMelee : Action
{
    public SharedFloat RawDamage;
    public SharedFloat AttackCoolDown;
    public SharedGameObject Target;
    public SharedFloat Range;

    float timer;

    public override TaskStatus OnUpdate ()
    {
        if (Target.Value == null || Vector2.SqrMagnitude (transform.position - Target.Value.transform.position) > (Range.Value * Range.Value))
        {
            return TaskStatus.Success;
        }
        timer += Time.deltaTime;

        // Need to make enemy always face player
        Vector3 diff = Target.Value.transform.position - transform.position;
        diff.Normalize ();

        float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90f);

        if (timer > AttackCoolDown.Value)
        {
            Attack ();
        }
        return TaskStatus.Running;
    }

    private void Attack ()
    {
        timer = 0f;


        Target.Value.GetComponent<PlayerStats> ().OnHit (RawDamage.Value);
    }
}