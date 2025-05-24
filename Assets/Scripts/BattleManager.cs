using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Image playerDiceImage;
    public Image enemyDiceImage;
    public Sprite[] diceSprites; // 6 adet zar sprite’ı
    public Button rollButton;
    public TMP_Text text;

    void Start()
    {
        rollButton.onClick.AddListener(RollDice);
    }

    void RollDice()
    {
        int playerRoll = Random.Range(1, 7);
        int enemyRoll = Random.Range(1, 7);

        playerDiceImage.sprite = diceSprites[playerRoll - 1];
        enemyDiceImage.sprite = diceSprites[enemyRoll - 1];
        text.text = $"Player rolled: {playerRoll}\nEnemy rolled: {enemyRoll}";
        Debug.Log($"Player rolled {playerRoll} | Enemy rolled {enemyRoll}");
        if (playerRoll > enemyRoll)
            Debug.Log("Player wins the round!");
            
        else if (playerRoll < enemyRoll)
            Debug.Log("Enemy wins the round!");
        else
            Debug.Log("It's a draw!");
    }
}
