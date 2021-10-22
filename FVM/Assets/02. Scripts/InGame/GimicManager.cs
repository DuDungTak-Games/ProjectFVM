using System;
using System.Collections.Generic;
using UnityEngine;

public class GimicManager : MonoBehaviour
{

    [SerializeField] 
    List<GimicTrigger> gimicTriggers = new List<GimicTrigger>();
    
    [SerializeField] 
    List<GimicActor> gimicActors = new List<GimicActor>();

    private void Awake()
    {
        ResetData();
    }

    public void Init()
    {
        MatchingGimic();
    }

    void MatchingGimic()
    {
        foreach (var trigger in gimicTriggers)
        {
            bool isMatch = false;
            
            foreach (var actor in gimicActors)
            {
                if (trigger.ID == actor.ID)
                {
                    isMatch = true;
                    
                    trigger.AddEvent(actor.OnAction);
                }
            }

            if (!isMatch)
            {
                Debug.LogWarningFormat("[GimicManager] GimicTrigger ({0}) 매칭이 되지 않아서 삭제함!", trigger.ID);
                
                Destroy(trigger.gameObject);
            }
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
    }

    public void ResetData()
    {
        gimicTriggers.Clear();
        gimicActors.Clear();
    }
}
