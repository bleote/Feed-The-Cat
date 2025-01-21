using UnityEngine.UI;
/// <summary>
/// This is a class containing generic UI functions
/// <remarks></remarks>
/// </summary>
public class UI
{
    public static void ActivateInputField(InputField inputField)
    {
        inputField.Select();
        Routine.WaitAndCall(.01f, () => inputField.MoveTextEnd(false));
    }
}
