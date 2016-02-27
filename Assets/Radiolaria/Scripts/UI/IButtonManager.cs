using UnityEngine;
using System.Collections;

public interface IButtonManager {
    
    void UncheckOtherRadios(RadioButtonHandler radio);
    void OnRadio(string name);
    void OnToggle(string name, bool isOn);
    void OnClick(string name);
}

public interface IRadioGroup
{
    void UncheckOtherRadios(RadioButtonHandler radio);
    void OnRadio(string name);
}

public interface IToggleGroup
{
    void OnToggle(string name, bool isOn);
}

public interface IButtonGroup
{
    void OnClick(string name);
}
