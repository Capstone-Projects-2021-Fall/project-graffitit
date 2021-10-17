using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
	public TabGroup tabGroup;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }


    // Start is called before the first frame update
    void Start()
    {
        tabGroup.AddButton(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
