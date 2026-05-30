using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
	[Header("Configuración")]
	public int targetCount = 12;
	public float timeBonus = 2f;
	public float timeLimit = 180f;

	[Header("UI")]
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI recoveredCountText;
	public GameObject victoryPanel;
	public TextMeshProUGUI finalScoreText;
	public TextMeshProUGUI finalTimeText;

	public int Score { get; private set; }
	public int RecoveredCount { get; private set; }
	public bool GameOver { get; private set; }
	public float ElapsedTime { get; private set; }

	void OnEnable() { JunkObject.OnCollected += HandleCollected; }
	void OnDisable() { JunkObject.OnCollected -= HandleCollected; }

	void Start()
	{
		if (victoryPanel != null) victoryPanel.SetActive(false);
		UpdateUI();
	}

	void Update()
	{
		if (!GameOver) ElapsedTime += Time.deltaTime;
	}

	void HandleCollected(JunkObject junk)
	{
		if (GameOver) return;

		int pts = junk.JunkData.rarity switch
		{
			JunkRarity.Rare => Mathf.RoundToInt(junk.JunkData.points * 1.5f),
			JunkRarity.Critical => junk.JunkData.points * 3,
			_ => junk.JunkData.points
		};

		Score += pts;
		RecoveredCount += 1;
		UpdateUI();

		if (RecoveredCount >= targetCount)
			StartCoroutine(TriggerVictory());
	}

	IEnumerator TriggerVictory()
	{
		GameOver = true;
		int tBonus = Mathf.RoundToInt(Mathf.Max(0f, timeLimit - ElapsedTime) * timeBonus);
		Score += tBonus;

		yield return new WaitForSeconds(0.8f);

		if (victoryPanel != null) victoryPanel.SetActive(true);

		int mins = Mathf.FloorToInt(ElapsedTime / 60f);
		int secs = Mathf.FloorToInt(ElapsedTime % 60f);
		if (finalScoreText != null) finalScoreText.text = $"PUNTOS: {Score}\n+{tBonus} bonus de tiempo";
		if (finalTimeText != null) finalTimeText.text = $"TIEMPO: {mins:00}:{secs:00}";
	}

	void UpdateUI()
	{
		if (scoreText != null) scoreText.text = Score.ToString("N0");
		if (recoveredCountText != null) recoveredCountText.text = $"{RecoveredCount} / {targetCount}";
	}
}