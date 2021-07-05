using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractEnemy : Unit
{

    [SerializeField]
    protected MoMath.XZDirection nowDirection;

    protected bool isAttacking = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        isAttacking = false;


        transform.rotation = Quaternion.LookRotation(MoMath.DirectionMath.FromDirection(nowDirection), Vector3.up);

    }



}