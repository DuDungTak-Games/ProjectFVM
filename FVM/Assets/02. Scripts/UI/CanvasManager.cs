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
            Debug.LogWarningFormat("[CanvasManager] SetPanel - �ش�Ǵ� PanelSet Name ({0}) �� �������� �ʽ��ϴ�!", targetName);
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
            Debug.LogWarningFormat("[CanvasManager] SetPanel - �ش�Ǵ� PanelSet Index ({0}) �� �������� �ʽ��ϴ�!", targetIdx);
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
