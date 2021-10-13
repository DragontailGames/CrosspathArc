using UnityEngine;

public class DayDurationController : MonoBehaviour
{

	public GameObject Sky;
	public GameObject Glow;

	public void StartDay ()
	{
		Sky.GetComponent<Animator>().Play("SkyDay");
		Glow.GetComponent<Animator>().Play("Day");
	}

	public void StartNight()
	{
		Sky.GetComponent<Animator>().Play("SkyNight");
		Glow.GetComponent<Animator>().Play("Night");
	}
}
