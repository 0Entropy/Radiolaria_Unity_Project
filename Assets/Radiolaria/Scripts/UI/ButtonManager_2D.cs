using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonManager_2D : MonoBehaviour, IRadioGroup, IToggleGroup, IButtonGroup
{
    public delegate void HandleOnCheckRadio(string name);
    public static event HandleOnCheckRadio OnCheckRadio;

    public delegate void HandleOnCheckToggle(string name, bool isOn);
    public static event HandleOnCheckToggle OnCheckToggle;

    public delegate void HandleOnChechButton(string name);
    public static event HandleOnChechButton OnCheckButton;

    //public ToggleButtonHandler NavigateButton;

    public RadioButtonHandler RadioPrefab;
    public ToggleButtonHandler TogglePrefab;
    public BasicButtonHandler ButtonPrefab;
    public Image SeparatorPrefab;

    public RectTransform gridTransform;

    public List<RadioButtonHandler> RadioButtons;

    public List<ToggleButtonHandler> toggleButtons;

    public List<BasicButtonHandler> basicButtons;

    public List<Image> separators;

    string itemName = "CellDivision,CellUnion,Separator,ForceAttractive,ForceRepulsive,ForceClockwise,ForceAnticlockwise,Separator,Border_Toggle,Separator,Refresh_Basic";
    //Cell_Toggle,//ForceClear,Force_Toggle,

    public void Start()
    {
        RadioPrefab.CreatePool();
        TogglePrefab.CreatePool();
        ButtonPrefab.CreatePool();
        SeparatorPrefab.CreatePool();

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
            RectTransform itemTransform = null;

            if (name.Contains("Toggle"))
            {
                name = name.Split('_')[0];
                //Debug.Log("Toggle name : " + name);
                var toggle = TogglePrefab.Spawn();

                toggle.OnShow(name, this);
                toggleButtons.Add(toggle);
                itemTransform = (RectTransform)toggle.transform;

            }
            else if (name.Contains("Separator"))
            {
                var separator = SeparatorPrefab.Spawn();
                separator.gameObject.name = name;
                itemTransform = (RectTransform)separator.transform;

                separators.Add(separator);
            }
            else if (name.Contains("Basic"))
            {
                name = name.Split('_')[0];
                //Debug.Log("Basic name : " + name);
                var but = ButtonPrefab.Spawn();
                but.OnShow(name, this);

                basicButtons.Add(but);

                itemTransform = (RectTransform)but.transform;
            }
            else
            {
                //Debug.Log("Radio name : " + name);
                RadioButtonHandler radio = RadioPrefab.Spawn();

                radio.OnShow(name, this);
                RadioButtons.Add(radio);

                itemTransform = (RectTransform)radio.transform;

            }

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
        if (toggleButtons == null)
            toggleButtons = new List<ToggleButtonHandler>();
        if (basicButtons == null)
            basicButtons = new List<BasicButtonHandler>();
        if (separators == null)
            separators = new List<Image>();

        foreach (var item in RadioButtons)
            item.Recycle();
        foreach (var toggle in toggleButtons)
            toggle.Recycle();
        foreach (var basic in basicButtons)
            basic.Recycle();
        foreach (var img in separators)
            img.Recycle();
            //DestroyImmediate(img.gameObject);

        RadioButtons.Clear();
        toggleButtons.Clear();
        basicButtons.Clear();
        separators.Clear();
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

    public void OnToggle(string name, bool isOn)
    {
        //Debug.Log("OnToggle " + name + ", switch to : " + isOn);
        OnCheckToggle(name, isOn);
    }

    public void OnClick(string name)
    {
        Clear();
        Spawn();
        //Debug.Log("OnClick " + name);
        OnCheckButton(name);
    }
}
