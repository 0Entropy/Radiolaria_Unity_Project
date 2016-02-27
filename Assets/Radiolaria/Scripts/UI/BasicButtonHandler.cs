using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BasicButtonHandler : MonoBehaviour
{
    public Image ButtonImage;
    public Image ContentImage;

    string mName;
    IButtonGroup mManager;

    public void OnShow(string name, IButtonGroup manager)
    {
        mName = name;

        gameObject.name = name;

        mManager = manager;

        ContentImage.sprite = Resources.Load<Sprite>("ItemSprites/U_icon_" + name + "_Basic");

    }

    public void HandleOnClick()
    {
        mManager.OnClick(mName);
    }
}
