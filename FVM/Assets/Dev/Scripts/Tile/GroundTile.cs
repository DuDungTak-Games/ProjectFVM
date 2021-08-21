using System.Collections;
using UnityEngine;

public class GroundTile : Floor
{

    [SerializeField] Texture testTexture;
    
    private bool hasDown, hasUp;
    
    public void SetFloorInfo(bool hasUp, bool hasDown)
    {
        this.hasDown = hasDown;
        this.hasUp = hasUp;

        if (hasUp)
        {
            GetComponentInChildren<MeshRenderer>().material.mainTexture = testTexture;
        }
        
        StartCoroutine(OnEnableMotion());
    }

    IEnumerator OnEnableMotion()
    {
        Vector3 scaleOri = transform.localScale;

        transform.localScale *= 0.1f;
        
        float t = 0;

        while (t < 1)
        {
            t = Mathf.Lerp(t, 1.1f, 0.25f * Time.deltaTime);

            transform.localScale = Vector3.Lerp(transform.localScale, scaleOri, t);
            
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = scaleOri;
    }
}
