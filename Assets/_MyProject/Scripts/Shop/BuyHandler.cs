using UnityEngine;
using UnityEngine.Purchasing;

public class BuyHandler : MonoBehaviour
{
    public void Success(Product _product)
    {
        int _amount = (int)_product.definition.payout.quantity;
        string _type = _product.definition.payout.subtype;
        switch (_type)
        {
            case "paw":
                DataManager.Instance.PlayerData.Paws += _amount;
                break;
            case "biscuit":
                DataManager.Instance.PlayerData.Biscuits += _amount;
                break;
            case "elixir":
                DataManager.Instance.PlayerData.Elixirs += _amount;
                break;
            case "extraLive":
                DataManager.Instance.PlayerData.ExtraLivesPackage += _amount;
                break;
            case "cat":
                DataManager.Instance.PlayerData.AddOwnedCat(_amount);//in this case _amount represents the id of the unlocked cat
                break;
            default:
                UIManager.Instance.OkDialog.Show($"Please contact support!\nDon't know how to resolve {_type} purchase");
                throw new System.Exception($"Don't know how to resolve {_type} purchase");
        }

        UIManager.Instance.OkDialog.Show("Successful purchase");
    }

    public void Failed(Product _product, PurchaseFailureReason _reason)
    {
        UIManager.Instance.OkDialog.Show("Failed to make purchase:\n" + _reason);
    }
}
