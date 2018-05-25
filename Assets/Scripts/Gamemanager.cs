using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{

    public static Gamemanager GM;

    private void Awake ()
    {
        GM = this;
    }
    // Use this for initialization
    void Start ()
    {

    }
}
