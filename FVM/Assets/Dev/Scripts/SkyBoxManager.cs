using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxManager : MonoBehaviour
{

    public Material skyboxMaterialOri;

    public Transform target;
    
    public List<ColorSet> colorList;

    private Material skyboxMaterial;
    
    private ColorSet curColorSet;

    private float prevHeight = 0;
    
    private int curIndex = 0;
    
    [Serializable]
    public struct ColorSet
    {
        public int targetHeight;
        
        public Color top;
        public Color center;
        public Color bottom;
    }

    void Start()
    {
        skyboxMaterial = RenderSettings.skybox;
        
        curColorSet = colorList[curIndex];
        UpdateColor();

        curIndex++;
    }
    
    void Update()
    {
        UpdateSkyBox();
    }

    void UpdateSkyBox()
    {
        float curHeight = target != null ? target.position.y : 0;
        float targetHeight = colorList[curIndex].targetHeight;
        
        if (curHeight < prevHeight)
        {
            curIndex--;
            curIndex = Mathf.Clamp(curIndex, 0, colorList.Count-1);
            
            prevHeight = colorList[curIndex].targetHeight;
        }
        else if (curHeight < targetHeight)
        {
            float t = (curHeight - prevHeight) / targetHeight;

            curColorSet.top = Color.Lerp(colorList[curIndex-1].top, colorList[curIndex].top, t);
            curColorSet.center = Color.Lerp(colorList[curIndex-1].center, colorList[curIndex].center, t);
            curColorSet.bottom = Color.Lerp(colorList[curIndex-1].bottom, colorList[curIndex].bottom, t);
            
            UpdateColor();
        }
        else if(curHeight >= targetHeight)
        {
            prevHeight = targetHeight;
            
            curIndex++;
            curIndex = Mathf.Clamp(curIndex, 0, colorList.Count-1);
        }
    }

    void UpdateColor()
    {
        skyboxMaterial.SetColor("_Top", curColorSet.top);
        skyboxMaterial.SetColor("_Center", curColorSet.center);
        skyboxMaterial.SetColor("_Bottom", curColorSet.bottom);
    }

    private void OnApplicationQuit()
    {
        RenderSettings.skybox.SetColor("_Top", skyboxMaterialOri.GetColor("_Top"));
        RenderSettings.skybox.SetColor("_Center", skyboxMaterialOri.GetColor("_Center"));
        RenderSettings.skybox.SetColor("_Bottom", skyboxMaterialOri.GetColor("_Bottom"));
    }
}
