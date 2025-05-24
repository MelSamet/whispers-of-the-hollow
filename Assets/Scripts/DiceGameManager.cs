using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DiceGameManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI rollText;
    public TextMeshProUGUI instructionText;
    
    public int successThreshold = 4; // örn: 4+ başarı sayılır
    private bool hasRolled = false;

    void Start()
    {
        instructionText.text = "Roll the dice to determine your fate!";
        resultText.text = "";
        rollText.text = "";
    }
    
    public enum StatType
    {
        None,
        Strength,
        Agility,
        Intelligence,
        Luck
    }

    public void RollDice()
    {
        if (hasRolled) return;

        hasRolled = true;
        int roll = Random.Range(1, 7); // 1–6

        rollText.text = $"You rolled a {roll}.";

        if (roll >= successThreshold)
        {
            resultText.text = "<color=green>Success!</color>";

            // Gain stat işle
            string gainStatName = PlayerPrefs.GetString("DiceGainStat", "None");
            int gainAmount = PlayerPrefs.GetInt("DiceGainAmount", 0);

            if (gainStatName != "None")
                AddToStat(gainStatName, gainAmount);
        }
        else
        {
            resultText.text = "<color=red>Failed!</color>";

            // Stat kaybı işle
            string requiredStat = PlayerPrefs.GetString("DiceRequiredStat", "None");
            int lossAmount = PlayerPrefs.GetInt("DiceLossAmount", 0);

            if (requiredStat != "None")
                AddToStat(requiredStat, -lossAmount);
        }

        // 2 saniye sonra geri dön
        Invoke("ReturnToGame", 2f);
    }

    void ReturnToGame()
    {
        string returnScene = PlayerPrefs.GetString("ResumeScene", "start");
        SceneManager.LoadScene("SampleScene"); // Buraya ana sahnenin adını yaz ("SampleScene" ya da "main" gibi)
    }

    void AddToStat(string statName, int amount)
    {
        StatType stat = StatType.None;
        System.Enum.TryParse(statName, out stat);
        int current = PlayerPrefs.GetInt(stat.ToString(), 0);
        current += amount;
        if (current < 0) current = 0;
        PlayerPrefs.SetInt(stat.ToString(), current);
        Debug.Log($"[Dice] {stat}: {current} (Δ {amount})");
    }
}
