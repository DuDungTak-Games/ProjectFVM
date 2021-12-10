using System;
using System.Collections.Generic;
using UnityEngine;

public class GimicManager : MonoBehaviour
{

    [SerializeField] 
    List<GimicTrigger> gimicTriggers = new List<GimicTrigger>();
    
    [SerializeField] 
    List<GimicActor> gimicActors = new List<GimicActor>();
    
    [SerializeField] 
    List<GimicCustom> gimicCustoms = new List<GimicCustom>();

    public void Init()
    {
        MatchingGimic();
        ActiveGimic();
    }

    void MatchingGimic()
    {
        foreach (var trigger in gimicTriggers)
        {
            // bool isMatch = false;
            foreach (var actor in gimicActors)
            {
                if (trigger.ID == actor.ID)
                {
                    // isMatch = true;
                    trigger.AddEvent(actor.OnAction);
                }
            }

            // if (!isMatch)
            // {
            //     Debug.LogWarningFormat("[GimicManager] GimicTrigger ({0}) 매칭이 되지 않아서 삭제됨!", trigger.ID);
            //     Destroy(trigger.gameObject);
            // }
        }
        
        foreach (var gimic in gimicCustoms)
        {
            foreach (var actor in gimicActors)
            {
                if (gimic.ID == actor.ID)
                {
                    actor.CustomAction(gimic);
                }
            }
        }
    }

    void ActiveGimic()
    {
        foreach (var trigger in gimicTriggers)
        {
            trigger.OnActive();
        }
        
        foreach (var actor in gimicActors)
        {
            actor.OnActive();
        }
        
        foreach (var gimic in gimicCustoms)
        {
            gimic.OnActive();
        }
    }

    public void AddGimic(GimicObject gimic)
    {
        if (gimic is GimicTrigger)
        {
            gimicTriggers.Add(gimic as GimicTrigger);
        }
        
        if(gimic is GimicActor)
        {
            gimicActors.Add(gimic as GimicActor);
        }

        if (gimic is GimicCustom)
        {
            gimicCustoms.Add(gimic as GimicCustom);
        }
    }
    
    public List<GimicTrigger> FindGimicTrigger(int id)
    {
        return gimicTriggers.FindAll(x => x.ID == id);
    }
    
    public List<GimicActor> FindGimicActor(int id)
    {
        return gimicActors.FindAll(x => x.ID == id);
    }

    public List<GimicCustom> FindGimicCustom(int id)
    {
        return gimicCustoms.FindAll(x => x.ID == id);
    }

    public void ResetData()
    {
        gimicTriggers.Clear();
        gimicActors.Clear();
        gimicCustoms.Clear();
    }
}
