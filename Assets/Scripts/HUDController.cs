using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
	[Header("Referencias")]
	public PlayerController player;
	public MassSystem massSystem;
	public DockingSystem docking;
	public ScoreSystem score;

	[Header("Textos HUD")]
	public TextMeshProUGUI velocityText;
	public TextMeshProUGUI massText;
	public TextMeshProUGUI momentumText;
	public TextMeshProUGUI kineticText;
	public TextMeshProUGUI dockedText;
	public TextMeshProUGUI timerText;

	[Header("Barra velocidad")]
	public Slider velocityBar;
	public float maxSpeedDisplay = 10f;

	[Header("Notificación")]
	public TextMeshProUGUI notificationText;
	private Coroutine notifCoroutine;

	void OnEnable()
	{
		JunkObject.OnCollected += n => ShowNotif($"✓  {n.JunkData.label}  +{n.JunkData.points} pts", Color.green);
		JunkObject.OnDocked += n => ShowNotif($"⚙  {n.JunkData.label}  ACOPLADO", Color.cyan);
	}

	void OnDisable()
	{
		JunkObject.OnCollected -= n => ShowNotif("", Color.white);
		JunkObject.OnDocked -= n => ShowNotif("", Color.white);
	}

	void Update()
	{
		if (player == null) return;

		float speed = player.Velocity.magnitude;

		if (velocityText != null) velocityText.text = $"VEL  {speed:F1} m/s";
		if (massText != null && massSystem != null) massText.text = $"MASA  {massSystem.TotalMass:F1} kg";
		if (momentumText != null && massSystem != null) momentumText.text = $"IMPULSO  {massSystem.Momentum:F2} kg·m/s";
		if (kineticText != null && massSystem != null) kineticText.text = $"Ek  {massSystem.KineticEnergy:F1} J";
		if (dockedText != null && docking != null)
			dockedText.text = docking.DockedCount > 0
				? $"ACOPLADOS  {docking.DockedCount}  ({docking.TotalDockedMass:F1} kg)"
				: "ACOPLADOS  —";
		if (velocityBar != null) velocityBar.value = speed / maxSpeedDisplay;

		if (timerText != null && score != null)
		{
			int m = Mathf.FloorToInt(score.ElapsedTime / 60f);
			int s = Mathf.FloorToInt(score.ElapsedTime % 60f);
			timerText.text = $"{m:00}:{s:00}";
		}
	}

	void ShowNotif(string msg, Color color)
	{
		if (notificationText == null) return;
		if (notifCoroutine != null) StopCoroutine(notifCoroutine);
		notifCoroutine = StartCoroutine(NotifRoutine(msg, color));
	}

	IEnumerator NotifRoutine(string msg, Color color)
	{
		notificationText.text = msg;
		notificationText.color = color;
		notificationText.alpha = 1f;
		yield return new WaitForSeconds(1.4f);
		float t = 0f;
		while (t < 0.4f)
		{
			t += Time.deltaTime;
			notificationText.alpha = Mathf.Lerp(1f, 0f, t / 0.4f);
			yield return null;
		}
		notificationText.alpha = 0f;
	}
}