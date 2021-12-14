using UnityEngine;

[RequireComponent(typeof(Raycaster))]
public class GimicPit : GimicTrigger
{

    public Tile tile { get; private set; }
    public Tile exitTile { get; private set; }

    GimicPit exitPit;

    Raycaster raycaster;

    Animator animator;

    protected override void Awake()
    {
        base.Awake();

        raycaster = this.GetComponent<Raycaster>();
        animator = this.GetComponent<Animator>();
    }

    public override void OnActive()
    {
        tile = this.GetComponent<Tile>();

        FindExitPit();
    }
    
    public override void OnTrigger()
    {
        if (isTrigger || exitPit == null)
            return;

        Vector3 exitDir = exitPit.FindExitDir();
        if (exitDir != Vector3.zero)
        {
            PlayerController pc = GameManager.Instance.player.controller;
            pc.PitMove(this, exitPit, exitDir);
        }
    }

    public void PlayTriggerAnim()
    {
        animator.SetTrigger("OnTrigger");
    }

    bool CheckDirection(Vector3 direction)
    {
        GameObject rayObj = raycaster.CheckDirection(direction);
        if (rayObj == null)
        {
            return CheckFloor(direction);
        }

        return false;
    }

    bool CheckFloor(Vector3 direction)
    {
        GameObject rayObj = raycaster.CheckFloor(direction);
        if (rayObj)
        {
            if (rayObj.TryGetComponent(out Tile dirTile))
            {
                exitTile = dirTile;
                return true;
            }
        }

        return false;
    }

    public Vector3 FindExitDir()
    {
        if (CheckDirection(transform.forward)) return transform.forward;
        if (CheckDirection(-transform.forward)) return -transform.forward;
        if (CheckDirection(transform.right)) return transform.right;
        if (CheckDirection(-transform.right)) return -transform.right;
        return Vector3.zero;
    }

    void FindExitPit()
    {
        GimicManager gimicManager = GameManager.Instance.gimicManager;
        
        var gimicList = gimicManager.FindGimicTrigger(this.ID);
        foreach (var trigger in gimicList)
        {
            if (trigger != this)
            {
                if (trigger is GimicPit)
                {
                    exitPit = (trigger as GimicPit);
                }
            }
        }
    }
}
