using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public enum UIState { VIEW_2D, VIEW_3D }

public class UIController : MonoBehaviour//, IToggleGroup
{

    public delegate void HandleOnNavigateTo(UIState state);
    public static event HandleOnNavigateTo OnNavigateTo;

    UIState State;

    public ButtonManager_2D UIView2D;
    public ButtonManager_3D UIView3D;

    //public Button NavigateButton;
    public Image NavigateImage;
    public Sprite NavigateTo3D;
    public Sprite NavigateTo2D;



    void Start() {
        Show2D();
    }

    public void Show2D()
    {
        NavigateImage.sprite = NavigateTo2D;

        if (!UIView2D.gameObject.activeSelf)
            UIView2D.gameObject.SetActive(true);

        UIView3D.gameObject.SetActive(false);

        State = UIState.VIEW_2D;
    }

    public void Show3D()
    {
        NavigateImage.sprite = NavigateTo3D;

        if (!UIView3D.gameObject.activeSelf)
            UIView3D.gameObject.SetActive(true);

        UIView2D.gameObject.SetActive(false);

        State = UIState.VIEW_3D;
    }

    public void HandleOnNavigate()
    {
        if (State == UIState.VIEW_2D)
            Show3D();
            else
            Show2D();

        OnNavigateTo(State);
    }
    
}
