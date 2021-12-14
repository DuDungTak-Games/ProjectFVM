using UnityEngine;

public class Raycaster : MonoBehaviour
{

    [Header("Raycast")]
    [SerializeField] float dirDistance = 10f;
    [SerializeField] Vector3 dirOffset = new Vector3(0, 5.5f, 0);

    [SerializeField] float floorDistance = 11f;

    [Header("Raycast Layer")]
    [SerializeField] LayerMask dirLayer = ~(1 << 6 | 1 << 7 | 1 << 9); // ~(Player | Item | Point)
    [SerializeField] LayerMask floorLayer = (1 << 3); // Tile
    [SerializeField] LayerMask triggerLayer = (1 << 3); // Trigger

    public GameObject CheckRay(Vector3 pos, Vector3 dir, float distance, int layerMask)
    {
        RaycastHit hit;
        if(Physics.Raycast(pos, dir, out hit, distance, layerMask))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public GameObject CheckRayOrigin(Vector3 offset, Vector3 dir, float offsetDis, int layerMask)
    {
        RaycastHit hit;
        if (Physics.Raycast(GetRayOrigin() + offset, dir, out hit, (dirDistance + offsetDis), layerMask))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public GameObject CheckDirection(Vector3 direction, int layerMask)
    {
        return CheckRay(GetRayOrigin(), direction, dirDistance, layerMask);
    }

    public GameObject CheckDirection(Vector3 direction)
    {
        return CheckDirection(direction, dirLayer);
    }

    public GameObject CheckFloor(Vector3 direction, int layerMask)
    {
        return CheckRay(GetRayOrigin() + (direction * TileManager.tileUnit), Vector3.down, floorDistance, layerMask);
    }

    public GameObject CheckFloor(Vector3 direction)
    {
        return CheckFloor(direction, floorLayer);
    }

    public GameObject CheckTrigger(Vector3 direction)
    {
        return CheckDirection(direction, triggerLayer);
    }

    public GameObject CheckTriggerFloor(Vector3 direction)
    {
        return CheckFloor(direction, triggerLayer);
    }

    Vector3 GetRayOrigin()
    {
        return (transform.position + dirOffset);
    }

    // NOTE: DEBUG GIZMOS
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(GetRayOrigin(), transform.forward * dirDistance);
        Gizmos.DrawRay(GetRayOrigin() + (transform.forward * dirDistance), Vector3.down * floorDistance);
    }
}
