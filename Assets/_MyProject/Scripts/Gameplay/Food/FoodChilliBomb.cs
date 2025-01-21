using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FoodChilliBomb : FoodChillies
{
    protected override void SpawnNewChilli(Vector3 _direction)
    {
        FoodChilliBomb _newChilli = Instantiate(this, FoodSpawner.Instance.FoodHolder);
        _newChilli.transform.position = transform.position;
        _newChilli.CanSplit = false;
        _newChilli.Setup();
        _newChilli.image.sprite = image.sprite;
        _newChilli.damageValue = damageValue;
        _newChilli.Move(_direction);
    }
}
