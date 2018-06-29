using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float velocity = 2f;

    [Tooltip ("A actual game obejct that Hold skills picked up")]
    public GameObject SkillPhysicalHodler;
    private GameObject[] SkillHolder;

    [Tooltip ("Layer for skills that can be picked up")]
    public LayerMask SkillLayer;
    // Used for turning
    private Vector3 turnSignPos;

    private void Start ()
    {
        SkillHolder = new GameObject[4];
    }
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
            transform.Translate (moveDirection * velocity * Time.deltaTime * GetComponent<PlayerStats> ().getWalkSpeed (), Space.World);
            turnSignPos = transform.position + new Vector3 (moveHorizontal, moveVertical);
            Vector3 diff = turnSignPos - transform.position;
            diff.Normalize ();

            float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90f);
        }
    }

    // This method is what happens is player push down A/B/X/Y
    // A = 0, B = 1, X = 2, Y = 3
    private void OnSkillButtonDown (int num)
    {
        Collider2D co = Physics2D.OverlapCircle (transform.position, 1f, SkillLayer);

        // IF there is a skill nearby, then try to pickup/swap the skill
        if (co != null)
        {
            OnTryPickUp (num, co.gameObject);
        }// If there is no skill nearby, then try to use current skill(if has one)
        else
        {
            if (SkillHolder[num] != null)
            {
                SkillHolder[num].SendMessage ("OnUseSkill");
            }
        }
    }

    // num is for the index number in skillholder[]
    private void OnTryPickUp (int num, GameObject skill)
    {
        // First, if player already has a skill at [num], then spill that out
        if (SkillHolder[num] != null)
        {
            // Detach physical skill would be sufficient, but you know
            SkillHolder[num].transform.parent = null;
            SkillHolder[num] = null;
        }
        // Pick the skill up && Physicall pick it up
        SkillHolder[num] = skill.gameObject;
        skill.transform.SetParent (SkillPhysicalHodler.transform);
        // TODO: Make original skill seem invisible


    }


}
