using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

[RequireComponent(typeof(Raycaster))]
public class GimicBox : GimicTrigger
{

    [SerializeField]
    float moveSpeed = 10f;

    Tile tile, bottomTile;

    Raycaster raycaster;

    Coroutine pushCoroutine;
    
    protected override void Awake()
    {
        base.Awake();

        raycaster = this.GetComponent<Raycaster>();

        transform.rotation = Quaternion.identity;

        ID = -1;
    }

    public override void OnActive()
    {
        tile = this.GetComponent<Tile>();
    }

    public override void OnTrigger()
    {
        if(pushCoroutine == null)
        {
            PushCoroutine().Start(ref pushCoroutine, this);
        }
    }

    bool CheckDirection(Vector3 direction)
    {
        GameObject rayObj = raycaster.CheckDirection(direction);
        if (rayObj == null)
        {
            return CheckDirectionFloor(direction);
        }

        return false;
    }

    bool CheckDirectionFloor(Vector3 direction)
    {
        GameObject rayObj = raycaster.CheckFloor(direction);
        if (rayObj)
        {
            if (rayObj.TryGetComponent(out Tile rayTile))
            {
                float result = (rayTile.floor - tile.floor);
                if (Mathf.Abs(result) == 0.5f)
                    return false;
                return true;
            }
        }
        else
        {
            return CheckDeepFloor(direction);
        }

        return false;
    }

    bool CheckDeepFloor(Vector3 direction)
    {
        int layerMask = (1 << LayerMask.NameToLayer("Tile") | 1 << LayerMask.NameToLayer("Trigger"));
        GameObject rayObj = raycaster.CheckRayOrigin(direction * TileManager.tileUnit, Vector3.down, Mathf.Infinity, layerMask);
        if (rayObj)
        {
            if (rayObj.TryGetComponent(out Tile rayTile))
            {
                bottomTile = rayTile;
                return true;
            }
        }

        return false;
    }

    IEnumerator PushCoroutine()
    {
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        playerPos.y = this.transform.position.y;

        Vector3 dir = (this.transform.position - playerPos).normalized;
        Vector3 rotDir = (playerPos - this.transform.position).normalized * 20f;

        gameObject.layer = LayerMask.NameToLayer("Tile");

        if(CheckDirection(dir))
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + (dir * TileManager.tileUnit);

            Quaternion startRot = Quaternion.Euler(rotDir.z * -1f, rotDir.y, rotDir.x);
            Quaternion endRot = Quaternion.identity;
            Quaternion targetRot = Quaternion.identity;

            yield return CoroutineExtensions.ProcessAction(moveSpeed, (t) =>
            {
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                targetRot = Quaternion.Lerp(startRot, endRot, t);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, t);
            });

            if (bottomTile != null)
            {
                startPos = transform.position;
                targetPos = bottomTile.GetDirPos(Vector3.up);

                tile.floor = (bottomTile.floor + 1);

                bottomTile = null;

                yield return CoroutineExtensions.ProcessAction(moveSpeed, (t) =>
                {
                    transform.position = Vector3.Lerp(startPos, targetPos, t);
                });
            }

            transform.position = targetPos;
            transform.rotation = Quaternion.identity;
        }

        gameObject.layer = LayerMask.NameToLayer("Trigger");

        pushCoroutine = null;
    }
}
