using UnityEngine;
using UnityEngine.UI;

public class ScreenKeyboard : MonoBehaviour
{
    public GameObject EngLayoutSml, EngLayoutBig, SymbLayout;
    InputField TextField;

    public static ScreenKeyboard inst;
    void Awake()
    {
        inst = this;
        gameObject.SetActive(false);
    }
    public void Show(InputField textField)
    {
        TextField = textField;
        this.gameObject.SetActive(true);
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    public void Hide()
    {
        //gameObject.SetActive(false);
    }

    public void alphabetFunction(string alphabet)
    {
        TextField.text = TextField.text + alphabet;
        this.gameObject.SetActive(true);
    }

    public void BackSpace()
    {
        if (TextField.text.Length > 0) 
            TextField.text = TextField.text.Remove(TextField.text.Length - 1);
    }

    public void CloseAllLayouts()
    {
        EngLayoutSml.SetActive(false);
        EngLayoutBig.SetActive(false);
        SymbLayout.SetActive(false);
    }

    public void ShowLayout(GameObject SetLayout)
    {

        CloseAllLayouts();
        SetLayout.SetActive(true);
    }

}
