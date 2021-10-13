using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicTestBox : GimicActor
{
    public override void OnAction()
    {
        this.gameObject.SetActive(false);
    }
}
