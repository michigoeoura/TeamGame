using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractEnemy : Unit
{

    [SerializeField]
    protected MoMath.XZDirection nowDirection;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }



}