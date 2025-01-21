using UnityEditor;

[InitializeOnLoad]
public class AutoKeystorePasswordFiller
{
    static AutoKeystorePasswordFiller()
    {
        PlayerSettings.keystorePass = "Hesoyam123";
        PlayerSettings.keyaliasPass = "Hesoyam123";
    }
}
