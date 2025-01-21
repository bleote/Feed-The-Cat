using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCat", menuName = "ScriptableObjects/Cat")]
public class CatSO : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public List<Sprite> EatChilliSprites { get; private set; }
    [field: SerializeField] public List<Sprite> EatIceCreamSprites { get; private set; }
    [field: SerializeField] public List<Sprite> NormalSprites { get; private set; }
    [field: SerializeField] public List<Sprite> OpenMouthSprites { get; private set; }
    [field: SerializeField] public List<Sprite> ReadyToEatSprites { get; private set; }

    public static CatSO SelectedCat;

    private static List<CatSO> allCats;

    public static void Init()
    {
        allCats = Resources.LoadAll<CatSO>("Cats/").ToList();
    }

    public static CatSO Get(int _id)
    {
        return allCats.First(_element => _element.Id == _id);
    }

    public static int Count => allCats.Count;
}
