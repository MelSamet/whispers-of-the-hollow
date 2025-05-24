using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleCombatManager : MonoBehaviour
{
    [Header("Character Sprites")]
    public Image playerVisual;
    public Image foeVisual;

    [Header("Dice Images")]
    public Sprite[] diceFaceSprites;
    public Image playerDiceUI;
    public Image foeDiceUI;

    [Header("UI Elements")]
    public Button rollDiceButton;
    public TextMeshProUGUI resultText;
    public Slider playerHealthBar;
    public Slider foeHealthBar;

    [Header("Audio & Video")]
    public VideoPlayer backgroundVideoPlayer;
    public VideoClip backgroundClip;
    public AudioSource musicSource;
    public AudioClip battleMusicClip;

    private int playerHP;
    private int foeHP = 100;
    private int playerStrength;
    private int foeStrength;

    void Start()
    {
        playerStrength = PlayerPrefs.GetInt("Strength", 5);
        playerHP = PlayerPrefs.GetInt("HP", 100);
        foeStrength = Random.Range(4, 9);

        playerHealthBar.maxValue = 100;
        foeHealthBar.maxValue = 100;
        playerHealthBar.value = playerHP;
        foeHealthBar.value = foeHP;

        if (musicSource != null && battleMusicClip != null)
        {
            musicSource.clip = battleMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (backgroundVideoPlayer != null && backgroundClip != null)
        {
            backgroundVideoPlayer.clip = backgroundClip;
            backgroundVideoPlayer.isLooping = true;
            backgroundVideoPlayer.Play();
        }

        resultText.text = "Click to Roll!";
        playerDiceUI.enabled = false;
        foeDiceUI.enabled = false;
        if (rollDiceButton != null)
            rollDiceButton.onClick.AddListener(() => StartCoroutine(RollDiceAndFight()));
    }

    IEnumerator RollDiceAndFight()
    {
        rollDiceButton.interactable = false;

        int dicePlayer = Random.Range(1, 7);
        int diceFoe = Random.Range(1, 7);

        playerDiceUI.sprite = diceFaceSprites[dicePlayer - 1];
        foeDiceUI.sprite = diceFaceSprites[diceFoe - 1];
        playerDiceUI.enabled = true;
        foeDiceUI.enabled = true;

        yield return new WaitForSeconds(1f);

        int playerTotal = dicePlayer + playerStrength;
        int foeTotal = diceFoe + foeStrength;

        if (playerTotal > foeTotal)
        {
            foeHP -= 20;
            resultText.text = $"You hit! {playerTotal} vs {foeTotal}";
        }
        else if (playerTotal < foeTotal)
        {
            playerHP -= 20;
            resultText.text = $"You were hit! {playerTotal} vs {foeTotal}";
        }
        else
        {
            resultText.text = $"Draw! {playerTotal} vs {foeTotal}";
        }

        playerHealthBar.value = playerHP;
        foeHealthBar.value = foeHP;

        yield return new WaitForSeconds(0.5f);
        rollDiceButton.interactable = true;

        if (playerHP <= 0 || foeHP <= 0)
        {
            string result = playerHP <= 0 ? "Defeat!" : "Victory!";
            resultText.text = result;

            string nextId = playerHP <= 0
                ? PlayerPrefs.GetString("DefeatNextId", "after_defeat")
                : PlayerPrefs.GetString("VictoryNextId", "after_victory");

            PlayerPrefs.SetString("ResumeFrom", nextId);
            PlayerPrefs.SetInt("HP", Mathf.Max(10, playerHP));
            PlayerPrefs.Save();

            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("SampleScene");
        }
    }
}