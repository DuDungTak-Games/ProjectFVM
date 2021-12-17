using System;
using System.Collections.Generic;
using UnityEngine;

// NOTE : UI Manager (Panel Manager)
public class CanvasManager : MonoBehaviour
{

    [Serializable]
    public class PanelOption
    {
        public bool isActive;
        public bool isIgnore;
    }

    [Serializable]
    public class PanelSet
    {
        public string NAME = "Empty";

        public List<PanelOption> options = new List<PanelOption>();
    }

    public List<Panel> panelList;
    public List<PanelSet> panelSetList;

    public void SetPanel(string targetName)
    {
        PanelSet panelSet = panelSetList.Find(x => (x.NAME.ToLower() == targetName.ToLower()));
        if (panelSet != null)
        {
            SetPanel(panelSet);
        }
        else
        {
            Debug.LogWarningFormat("[CanvasManager] SetPanel - 해당되는 PanelSet Name ({0}) 이 존재하지 않습니다!", targetName);
        }
    }

    public void SetPanel(int targetIdx)
    {
        if(targetIdx < panelSetList.Count)
        {
            SetPanel(panelSetList[targetIdx]);
        }
        else
        {
            Debug.LogWarningFormat("[CanvasManager] SetPanel - 해당되는 PanelSet Index ({0}) 가 존재하지 않습니다!", targetIdx);
        }
    }

    void SetPanel(PanelSet panelSet)
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            if (panelList[i] != null)
            {
                if (!panelSet.options[i].isIgnore)
                {
                    panelList[i].SetPanel(panelSet.options[i].isActive);
                }
            }
        }
    }
}
