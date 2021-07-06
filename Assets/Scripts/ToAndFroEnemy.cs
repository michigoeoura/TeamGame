using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToAndFroEnemy : AbstractEnemy
{
    public override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void WhenEndTurn()
    {
        throw new System.NotImplementedException();
    }

    public override void WhenStartTurn()
    {
        TryStartAttack();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
