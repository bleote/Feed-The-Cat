using UnityEngine;

[CreateAssetMenu(fileName = "NewIceCream", menuName = "ScriptableObjects/IceCream")]
public class IceCreamSO : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public Sprite Whole { get; private set; }
    [field: SerializeField] public Sprite SemiMelted { get; private set; }
    [field: SerializeField] public Sprite Melted { get; private set; }

}
