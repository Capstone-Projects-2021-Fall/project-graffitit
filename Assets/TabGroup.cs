using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
	public List<TabButton> tabButtons;
    public List<GameObject> pages;

	public void AddButton(TabButton button)
	{
		if (tabButtons == null)
		{
			tabButtons = new List<TabButton>();
		}

		tabButtons.Add(button);
	}
    
    public void OnTabSelected(TabButton button)
    {
        int index = button.transform.GetSiblingIndex();
        for(int i=0; i<pages.Count; i++)
        {
            if (i == index) 
            {
                pages[i].SetActive(true);
            } 
            else
            {
                pages[i].SetActive(false);
            }
        }
    }
}
