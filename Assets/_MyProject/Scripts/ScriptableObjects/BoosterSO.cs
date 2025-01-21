using UnityEngine;

[CreateAssetMenu(fileName = "NewBooster", menuName = "ScriptableObjects/Booster")]
public class BoosterSO : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite AvailableSprite { get; private set; }
    [field: SerializeField] public Sprite NotAvailableSprite { get; private set; }
}
