using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI regionText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI diceResultText;

    public Button choiceButton1;
    public Button choiceButton2;
    public Button choiceButton3;

    public TextMeshProUGUI choiceText1;
    public TextMeshProUGUI choiceText2;
    public TextMeshProUGUI choiceText3;

    public Slider healthBar;
    public Image backgroundImage;
    public AudioSource audioSource;

    private Dictionary<string, StoryNode> storyNodes;
    private StoryNode currentNode;

    private int playerHP;
    private Dictionary<StatType, int> stats;

    void Start()
    {
        // Reset stats on every start
        PlayerPrefs.SetInt("HP", 70);
        PlayerPrefs.SetInt("Strength", 5);
        PlayerPrefs.SetInt("Charisma", 5);
        PlayerPrefs.SetInt("Perception", 5);
        PlayerPrefs.SetInt("Wisdom", 5);
        PlayerPrefs.Save();

        LoadStats();
        LoadStory();

        string resumeScene = PlayerPrefs.GetString("ResumeFrom", "start");
        PlayerPrefs.DeleteKey("ResumeFrom");
        LoadScene(resumeScene);
    }

    void LoadStats()
    {
        stats = new Dictionary<StatType, int>
        {
            { StatType.Strength, PlayerPrefs.GetInt("Strength", 5) },
            { StatType.Charisma, PlayerPrefs.GetInt("Charisma", 5) },
            { StatType.Perception, PlayerPrefs.GetInt("Perception", 5) },
            { StatType.Wisdom, PlayerPrefs.GetInt("Wisdom", 5) }
        };
        playerHP = PlayerPrefs.GetInt("HP", 70);
        UpdateStatsUI();
    }

    void SaveStats()
    {
        foreach (var pair in stats)
            PlayerPrefs.SetInt(pair.Key.ToString(), pair.Value);

        PlayerPrefs.SetInt("HP", playerHP);
        PlayerPrefs.Save();
    }

    void LoadStory()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "story.json");
        if (!File.Exists(path))
        {
            Debug.LogError("story.json not found at path: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        StoryData data = JsonUtility.FromJson<StoryData>(json);
        storyNodes = new Dictionary<string, StoryNode>();
        foreach (StoryNode node in data.nodes)
            storyNodes[node.id] = node;
    }

    void LoadScene(string sceneId)
    {
        if (sceneId == "battle_1")
        {
            PlayerPrefs.SetString("ReturnScene", currentNode.id);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Dicebattle");
            return;
        }

        if (!storyNodes.ContainsKey(sceneId))
        {
            Debug.LogError("Scene not found: " + sceneId);
            return;
        }

        currentNode = storyNodes[sceneId];

        storyText.text = currentNode.text;
        regionText.text = "Region: " + currentNode.region;

        SetBackground(currentNode.background);
        PlayMusic(currentNode.music);
        UpdateStatsUI();

        Button[] buttons = { choiceButton1, choiceButton2, choiceButton3 };
        TextMeshProUGUI[] texts = { choiceText1, choiceText2, choiceText3 };

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
            buttons[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < currentNode.choices.Count && i < buttons.Length; i++)
        {
            Choice choice = currentNode.choices[i];
            Enum.TryParse(choice.requiredStat, out StatType stat);
            int requiredValue = choice.requiredStatValue;
            int playerValue = stats.ContainsKey(stat) ? stats[stat] : 0;

            string statInfo = "";
            if (stat != StatType.None && choice.useProbability)
            {
                float chance = (float)playerValue / (playerValue + requiredValue);
                int percentage = Mathf.RoundToInt(chance * 100);
                statInfo = $" ({stat} {requiredValue}) [Chance: {percentage}%]";
            }

            texts[i].text = choice.text + statInfo;
            buttons[i].gameObject.SetActive(true);

            int index = i;
            buttons[i].onClick.AddListener(() => HandleChoice(choice));
        }
    }

    void HandleChoice(Choice choice)
    {
        Enum.TryParse(choice.requiredStat, out StatType stat);
        int requiredValue = choice.requiredStatValue;
        int playerValue = stats.ContainsKey(stat) ? stats[stat] : 0;
        bool success = true;

        if (stat != StatType.None && choice.useProbability)
        {
            float chance = (float)playerValue / (playerValue + requiredValue);
            success = UnityEngine.Random.value <= chance;

            if (diceResultText != null)
                diceResultText.text = $"({stat}) Check: {Mathf.RoundToInt(chance * 100)}% chance — " + (success ? "Success!" : "Failed!");
        }

        Enum.TryParse(choice.gainStat, out StatType gainStat);

        if (success)
        {
            if (gainStat != StatType.None)
                stats[gainStat] += choice.gainAmount;

            playerHP += choice.hpChangeOnSuccess;
            LoadScene(choice.nextId);
        }
        else
        {
            if (stat != StatType.None)
                stats[stat] = Mathf.Max(0, stats[stat] - choice.lossAmount);

            playerHP = Mathf.Max(0, playerHP + choice.hpChangeOnFail);
            LoadScene(string.IsNullOrEmpty(choice.failureNextId) ? choice.nextId : choice.failureNextId);
        }

        SaveStats();
        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        statusText.text = $"HP: {playerHP} | STR: {stats[StatType.Strength]} | CHA: {stats[StatType.Charisma]} | PER: {stats[StatType.Perception]} | WIS: {stats[StatType.Wisdom]}";
        healthBar.maxValue = 100;
        healthBar.value = playerHP;
    }

    void SetBackground(string bgName)
    {
        if (string.IsNullOrEmpty(bgName)) return;
        Sprite bg = Resources.Load<Sprite>("Background/" + bgName);
        if (bg != null)
            backgroundImage.sprite = bg;
    }

    void PlayMusic(string musicName)
    {
        if (string.IsNullOrEmpty(musicName)) return;
        AudioClip clip = Resources.Load<AudioClip>("Music/" + musicName);
        if (clip != null && audioSource.clip != clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}