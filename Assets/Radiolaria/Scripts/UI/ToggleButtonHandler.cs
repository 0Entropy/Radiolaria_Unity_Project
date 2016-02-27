using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleButtonHandler : MonoBehaviour {

    public Image ButtonImage;
    public Image ContentImage;

    string mName;

    IToggleGroup mManager;

    public bool IsSwitchOn { set; get; }

    Sprite onSprite, offSprite;

    public void OnShow(string name, IToggleGroup manager)
    {
        mName = name;

        gameObject.name = name;

        mManager = manager;

        onSprite = Resources.Load<Sprite>("ItemSprites/U_icon_" + name + "_Toggle_On");
        offSprite = Resources.Load<Sprite>("ItemSprites/U_icon_" + name + "_Toggle_Off");

        IsSwitchOn = true;
        UpdateState();
    }


    public void HandleOnClick()
    {
        bool toSwitch = !IsSwitchOn;

        mManager.OnToggle(mName, toSwitch);

        IsSwitchOn = toSwitch;

        UpdateState();
    }

    public void UpdateState()
    {
            ContentImage.sprite = IsSwitchOn ? onSprite : offSprite;
    }

}
