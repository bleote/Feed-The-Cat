using Firebase.Analytics;
using System;
using TMPro;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    [SerializeField] private CharacterVisual characterVisual;
    [SerializeField] private float eatAnimationDelayTime;
    [SerializeField] private CoinsDisplay coinsHolder;
    [SerializeField] private GamePlayUI gamePlayUI;

    public static Action OnEatenIceCream;
    public static Action OnEatenIceCube;

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        FoodController _foodController = _collision.gameObject.GetComponent<FoodController>();
        
        if (_foodController == null)
        {
            return;
        }

        _foodController.isInMouth = true;

        switch (_foodController.Type)
        {
            case FoodType.IceCream:
                HandleIceCream(_foodController as FoodIceCream);
                break;

            case FoodType.IceCreamSpecial:
                HandleIceCreamSpecial(_foodController as FoodIceCreamSpecial);
                break;

            case FoodType.IceCreamUnique:
                HandleIceCreamUnique(_foodController as FoodIceCreamUnique);
                break;

            case FoodType.IceCreamGoal:
                HandleIceCreamGoal(_foodController as FoodIceCreamGoal);
                break;

            case FoodType.RewardingIceCream:
                HandleRewardingIceCream();
                break;

            case FoodType.Chilli:
                HandleChilli(_foodController as FoodChillies);
                break;

            case FoodType.ChilliGreen:
                HandleChilliGreen(_foodController as FoodChilliGreen);
                break;

            case FoodType.ChilliBomb:
                HandleChilliBomb(_foodController as FoodChilliBomb);
                break;

            case FoodType.Coin:
                HandleCoin(_foodController as FoodCoin);
                break;

            case FoodType.IceCube:
                HandleIceCube(_foodController as FoodIceCube);
                break;

            default:
                throw new Exception("Don`t know how to eat: " + _foodController.Type);
        }

        _collision.transform.SetParent(characterVisual.mouthMask);
        _foodController.GetComponent<Collider2D>().enabled = false;
        Routine.Scale(_collision.gameObject.transform, Vector3.one, Vector3.one * .5f, 0.2f, () => Destroy(_collision.gameObject));
    }

    private void HandleIceCream(FoodIceCream _foodIceCream)
    {
        if (_foodIceCream != null && !GamePlayManager.levelModeOn)
        {
            var _amountToAdd = _foodIceCream.Score * GamePlayManager.Instance.Multiplier;
            GamePlayManager.Instance.Score += _amountToAdd;
            _foodIceCream.IceCreamValueDisplay(_amountToAdd);
        }

        CallEatIceCreamRoutine();
    }

    private void HandleIceCreamSpecial(FoodIceCreamSpecial _foodIceCreamSpecial)
    {
        if (_foodIceCreamSpecial != null && !GamePlayManager.levelModeOn)
        {
            var _amountToAdd = _foodIceCreamSpecial.Score * GamePlayManager.Instance.Multiplier;
            GamePlayManager.Instance.Score += _amountToAdd;
            _foodIceCreamSpecial.IceCreamValueDisplay(_amountToAdd);
        }

        CallEatIceCreamRoutine();
    }

    private void HandleIceCreamUnique(FoodIceCreamUnique _foodIceCreamUnique)
    {
        if (_foodIceCreamUnique != null && !GamePlayManager.levelModeOn)
        {
            var _amountToAdd = _foodIceCreamUnique.Score * GamePlayManager.Instance.Multiplier;
            GamePlayManager.Instance.Score += _amountToAdd;
            _foodIceCreamUnique.IceCreamValueDisplay(_amountToAdd);
        }

        CallEatIceCreamRoutine();
    }

    private void HandleIceCreamGoal(FoodIceCreamGoal _foodIceCreamGoal)
    {
        if (_foodIceCreamGoal != null)
        {
            var _amountToAdd = _foodIceCreamGoal.Score * GamePlayManager.Instance.Multiplier;
            _foodIceCreamGoal.IceCreamValueDisplay(_amountToAdd);
            gamePlayUI.UpdateGoalDisplay(_foodIceCreamGoal.goalSpawner, _amountToAdd);
            LevelCompletedHandler.Instance.CheckForLevelCompletion();
            PowerWords.Instance.ActivatePowerWord();

            //Vibration.Vibrate(25);
            Vibration.VibratePop();
        }

        CallEatIceCreamRoutine();
        AudioManager.Instance.Play(AudioManager.ICE_CREAM_GOAL_SOUND);
    }

    private void HandleRewardingIceCream()
    {
        Routine.WaitAndCall(eatAnimationDelayTime, () =>
        {
            characterVisual.EatIceCream();
            AudioManager.Instance.Play(AudioManager.ICE_CREAM_COLLECT_SOUND);
        });
        Routine.WaitAndCall(0.05f, () => { RewardedAdHandler.Instance.Setup(); });

        FirebaseAnalytics.LogEvent("rv_ice_cream_eaten");
    }

    private void HandleChilli(FoodChillies _foodChillies)
    {
        if (_foodChillies.isFrozen)
        {
            _foodChillies.fall = true;
            CallEatIceCubeRoutine();
            return;
        }

        Routine.WaitAndCall(eatAnimationDelayTime, () =>
        {
            characterVisual.EatChilly();
            AudioManager.Instance.Play(AudioManager.ANGRY_CAT_SOUND);
        });
        if (!ElixirHandler.IsActive && _foodChillies != null)
        {
            GamePlayManager.Instance.TakeDamage(_foodChillies.damageValue, _foodChillies.Type);
            _foodChillies.DamageValueDisplay(_foodChillies.damageValue);
        }
    }

    private void HandleChilliGreen(FoodChilliGreen _foodChilliGreen)
    {
        if (_foodChilliGreen.isFrozen)
        {
            _foodChilliGreen.fall = true;
            CallEatIceCubeRoutine();
            return;
        }

        Routine.WaitAndCall(eatAnimationDelayTime, () =>
        {
            characterVisual.EatChilly();
            AudioManager.Instance.Play(AudioManager.ANGRY_CAT_SOUND);
        });
        if (!ElixirHandler.IsActive && _foodChilliGreen != null)
        {
            GamePlayManager.Instance.TakeDamage(_foodChilliGreen.damageValue, _foodChilliGreen.Type);
            _foodChilliGreen.DamageValueDisplay(_foodChilliGreen.damageValue);
        }
    }

    private void HandleChilliBomb(FoodChilliBomb _foodChilliBomb)
    {
        if (_foodChilliBomb.isFrozen)
        {
            _foodChilliBomb.fall = true;
            CallEatIceCubeRoutine();
            return;
        }

        Routine.WaitAndCall(eatAnimationDelayTime, () =>
        {
            characterVisual.EatChilly();
            AudioManager.Instance.Play(AudioManager.ANGRY_CAT_SOUND);
        });
        if (!ElixirHandler.IsActive && _foodChilliBomb != null)
        {
            AudioManager.Instance.Play(AudioManager.EXPLOSION_SOUND);
            GamePlayManager.Instance.TakeDamage(_foodChilliBomb.damageValue, _foodChilliBomb.Type);
            _foodChilliBomb.DamageValueDisplay(_foodChilliBomb.damageValue);
        }
    }

    private void HandleCoin(FoodCoin foodCoin)
    {
        int _amountOfCoins = foodCoin.Score * GamePlayManager.Instance.Multiplier;
        DataManager.Instance.PlayerData.Coins += _amountOfCoins;
        foodCoin.CoinValueDisplay(_amountOfCoins);
        coinsHolder.ShowCoins();

        CallEatIceCreamRoutine();
        AudioManager.Instance.Play(AudioManager.COIN_COLLECT_SOUND);
    }

    private void HandleIceCube(FoodIceCube foodIceCube)
    {
        CallEatIceCubeRoutine();
    }

    private void CallEatIceCreamRoutine()
    {
        Routine.WaitAndCall(eatAnimationDelayTime, () =>
        {
            characterVisual.EatIceCream();
            OnEatenIceCream?.Invoke();
            AudioManager.Instance.Play(AudioManager.ICE_CREAM_COLLECT_SOUND);
        });
    }

    private void CallEatIceCubeRoutine()
    {
        Routine.WaitAndCall(eatAnimationDelayTime, () =>
        {
            characterVisual.EatIceCream();
            OnEatenIceCube?.Invoke();
            AudioManager.Instance.Play(AudioManager.ICE_CREAM_COLLECT_SOUND);
            AudioManager.Instance.Play(AudioManager.ICE_CUBE_COLLECT_SOUND);
        });
    }
}
