using UnityEngine;


namespace AIBehaviorExamples
{
	public class Alarm : MonoBehaviour
	{
		public AudioClip alarmSound;


		public void OnGetHelp()
		{
			GetComponent<AudioSource>().PlayOneShot(alarmSound);
		}
	}
}