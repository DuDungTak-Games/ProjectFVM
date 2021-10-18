using System.Collections.Generic;
using UnityEngine;

public class GimicManager : MonoBehaviour
{

    [SerializeField] 
    List<GimicTrigger> gimicTriggers = new List<GimicTrigger>();
    
    [SerializeField] 
    List<GimicActor> gimicActors = new List<GimicActor>();

    void Init()
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
                Debug.LogWarningFormat("[GimicManager] GimicTrigger ({0}) 매칭이 되지 않아서 삭제됨!", trigger.ID);
                
                Destroy(trigger.gameObject);
            }
        }
    }

    public void AddTrigger(GimicTrigger trigger)
    {
        gimicTriggers.Add(trigger);
    }

    public void AddActor(GimicActor actor)
    {
        gimicActors.Add(actor);
    }
    
    public void ResetData()
    {
        gimicTriggers.Clear();
        gimicActors.Clear();
    }
}
