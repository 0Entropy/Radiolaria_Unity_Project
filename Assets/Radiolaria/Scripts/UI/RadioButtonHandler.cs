using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadioButtonHandler : MonoBehaviour {

    public Image ButtonImage;
    public Image ContentImage;

    string mName;
    IRadioGroup mManager;
    
    public bool IsSelected { set; get; }

    public void OnShow(string name, IRadioGroup manager)
    {
        mName = name;

        gameObject.name = name;

        mManager = manager;
        /*transform.SetParent(manager.gridTransform);
        transform.localScale = Vector3.one;*/

        ContentImage.sprite = Resources.Load<Sprite>("ItemSprites/U_icon_" + name);

        IsSelected = false;
        UpdateState();
    }
         
    
    public void HandleOnClick()
    {
        bool toSelected = !IsSelected;

        if (!toSelected)
            return;
        
        mManager.UncheckOtherRadios(this);

        mManager.OnRadio(mName);

        IsSelected = toSelected;

        UpdateState();
        
    }

    public void UpdateState()
    {
        ButtonImage.color = IsSelected ? Color.yellow : Color.gray;//new Color(0, 255, 196, 255)
    }

}


