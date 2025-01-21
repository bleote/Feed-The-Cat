using System;
using UnityEngine;
using UnityEngine.UI;

public class SuperPowersHandler : MonoBehaviour
{
    public static Action FireClicked;
    public static Action BiscuitsClicked;
    public static Action ElixirClicked;
    public static Action ExtraLivesClicked;

    [SerializeField] private Button fireButton;
    [SerializeField] private Button biscuitsButton;
    [SerializeField] private Button elixirButton;
    [SerializeField] private Button extraLivesButton;

    private void OnEnable()
    {
        fireButton.onClick.AddListener(UseFire);
        elixirButton.onClick.AddListener(UseElixir);
        biscuitsButton.onClick.AddListener(UseBiscuits);
        extraLivesButton.onClick.AddListener(UseExtraLives);
    }

    private void OnDisable()
    {
        fireButton.onClick.RemoveListener(UseFire);
        elixirButton.onClick.RemoveListener(UseElixir);
        biscuitsButton.onClick.RemoveListener(UseBiscuits);
        extraLivesButton.onClick.RemoveListener(UseExtraLives);
    }

    private void UseFire()
    {
        FireClicked?.Invoke();
    }

    private void UseElixir()
    {
        ElixirClicked?.Invoke();
    }

    private void UseBiscuits()
    {
        BiscuitsClicked?.Invoke();
    }

    private void UseExtraLives()
    {
        ExtraLivesClicked?.Invoke();
    }
}
