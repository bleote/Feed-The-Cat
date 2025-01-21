using UnityEngine;

public class LevelUpHandler : MonoBehaviour
{
    private bool DidLevelUp => pawsEarned > 0 || biscuitsEarned > 0 || elixirEarned > 0|| extraLivesEarned>0;

    private int pawsEarned;
    private int biscuitsEarned;
    private int elixirEarned;
    private int extraLivesEarned;
    private int startingScore;

    public int PawsEarned => pawsEarned;
    public int BiscuitsEarned => biscuitsEarned;
    public int ElixirsEarned => elixirEarned;
    public int ExtraLivesEarned => extraLivesEarned;

    private void OnEnable()
    {
        GamePlayManager.UpdatedScore += ManageLevel;
    }

    private void OnDisable()
    {
        GamePlayManager.UpdatedScore -= ManageLevel;
    }

    private void Start()
    {
        startingScore = DataManager.Instance.PlayerData.HighScore;
    }

    public void Check()
    {
        if (DidLevelUp)
        {
            AudioManager.Instance.Play(AudioManager.VICTORY_SOUND);
        }

        DataManager.Instance.PlayerData.Biscuits += biscuitsEarned;
        DataManager.Instance.PlayerData.Paws += pawsEarned;
        DataManager.Instance.PlayerData.Elixirs += elixirEarned;
    }

    private void AddPaw(int _amount)
    {
        pawsEarned += _amount;
        DataManager.Instance.PlayerData.HighScore = GamePlayManager.Instance.Score;
    }

    private void AddBiscuit(int _amount)
    {
        biscuitsEarned += _amount;
        DataManager.Instance.PlayerData.HighScore = GamePlayManager.Instance.Score;
    }

    private void AddElixir(int _amount)
    {
        elixirEarned += _amount;
        DataManager.Instance.PlayerData.HighScore = GamePlayManager.Instance.Score;
    }

    private void ManageLevel()
    {
        if (GamePlayManager.Instance.Score <= startingScore)
        {
            return;
        }

        int _score = GamePlayManager.Instance.Score;

        if (_score >= 10000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 10000)
            {
                startingScore = 10000;
                AddElixir(1);
            }
        }
        if (_score >= 15000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 15000)
            {
                startingScore = 15000;
                AddElixir(1);
                AddPaw(1);
            }
        }
        if (_score >= 20000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 20000)
            {
                startingScore = 20000;
                AddElixir(1);
                AddBiscuit(1);
            }
        }
        if (_score >= 25000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 25000)
            {
                startingScore = 25000;
                AddElixir(1);
                AddPaw(1);
                AddBiscuit(1);
            }
        }
        if (_score >= 30000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 30000)
            {
                startingScore = 30000;
                AddElixir(1);
                AddBiscuit(1);
            }
        }
        if (_score >= 35000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 35000)
            {
                startingScore = 35000;
                AddElixir(1);
                AddBiscuit(1);
                AddPaw(1);
            }
        }
        if (_score >= 40000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 40000)
            {
                startingScore = 40000;
                AddElixir(1);
                AddBiscuit(1);
                AddPaw(1);
            }
        }
        if (_score >= 45000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 45000)
            {
                startingScore = 45000;
                AddElixir(1);
                AddBiscuit(1);
                AddPaw(1);
            }
        }
        if (_score >= 50000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 50000)
            {
                startingScore = 50000;
                AddElixir(1);
                AddBiscuit(1);
                AddPaw(1);
            }
        }
        if (_score >= 55000)
        {
            if (DataManager.Instance.PlayerData.HighScore < 55000)
            {
                startingScore = 55000;
                AddElixir(1);
                AddBiscuit(1);
                AddPaw(1);
            }
        }

    }
}
