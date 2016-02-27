using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ButtonManager_3D : MonoBehaviour, IRadioGroup
{
    public delegate void HandleOnCheckRadio(string name);
    public static event HandleOnCheckRadio OnCheckRadio;

    /*public delegate void HandleOnCheckToggle(string name, bool isOn);
    public static event HandleOnCheckToggle OnCheckToggle;

    public delegate void HandleOnChechButton(string name);
    public static event HandleOnChechButton OnCheckButton;*/

    //public ToggleButtonHandler NavigateButton;

    public RadioButtonHandler RadioPrefab;
    /*public ToggleButtonHandler TogglePrefab;
    public BasicButtonHandler ButtonPrefab;
    public Image SeparatorPrefab;*/

    public RectTransform gridTransform;

    public List<RadioButtonHandler> RadioButtons;

    /*public List<ToggleButtonHandler> toggleButtons;

    public List<BasicButtonHandler> basicButtons;

    public List<Image> separators;*/

    string itemName = "StainlessSteel,Gold,Wood,Black";
    //Cell_Toggle,//ForceClear,Force_Toggle,

    public void Start()
    {
        RadioPrefab.CreatePool();
        /*TogglePrefab.CreatePool();
        ButtonPrefab.CreatePool();
        SeparatorPrefab.CreatePool();*/

        Clear();

        Spawn();

        //UGUIGridFix.refreshGrid(Items.Count, gridTransform, 128, 0, 1080, 0);
 }

    void Spawn()
    {
        string[] names = itemName.Split(',');

        int width = 0;
        for (int i = 0; i < names.Length; i++)
        {

            string name = names[i];
            

            RadioButtonHandler radio = RadioPrefab.Spawn();

            radio.OnShow(name, this);
            RadioButtons.Add(radio);

            RectTransform itemTransform = (RectTransform)radio.transform;

            //itemTransform = (RectTransform)radio.transform;
            itemTransform.SetParent(gridTransform);
            itemTransform.localScale = Vector3.one;


            width += (int)itemTransform.rect.width;

            //Debug.Log(string.Format("name : {2} || ItenWidth : {0}, sunWidth : {1}", (int)itemTransform.rect.width, width, name));
            
        }

        if (width < 1080) width = 1080;
        gridTransform.sizeDelta = new Vector2(width, gridTransform.sizeDelta.y);
        gridTransform.localPosition = new Vector3(width * 0.5F, gridTransform.localPosition.y, gridTransform.localPosition.z);

    }

    void Clear()
    {
        if (RadioButtons == null)
            RadioButtons = new List<RadioButtonHandler>();
        /*if (toggleButtons == null)
            toggleButtons = new List<ToggleButtonHandler>();
        if (basicButtons == null)
            basicButtons = new List<BasicButtonHandler>();
        if (separators == null)
            separators = new List<Image>();*/

        foreach (var item in RadioButtons)
            item.Recycle();
        /*foreach (var toggle in toggleButtons)
            toggle.Recycle();
        foreach (var basic in basicButtons)
            basic.Recycle();
        foreach (var img in separators)
            img.Recycle();*/
            //DestroyImmediate(img.gameObject);

        RadioButtons.Clear();
        /*toggleButtons.Clear();
        basicButtons.Clear();
        separators.Clear();*/
    }

    public void UncheckOtherRadios(RadioButtonHandler radio)
    {
        foreach(var r in RadioButtons)
        {
            if(r != radio)
            {
                r.IsSelected = false;
                r.UpdateState();
            }
        }
    }

    public void OnRadio(string name)
    {
        //Debug.Log("OnRadio " + name);
        OnCheckRadio(name);
    }
    
}
