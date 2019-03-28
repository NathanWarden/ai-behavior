using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace AIBehaviorDemo
{
	public class GameManager : MonoBehaviour
	{
		public int towersDeactivated = 0;

		private bool win = false;
		private bool gameOver = false;

		private Queue<string> labels = new Queue<string>();
		private string currentLabel = "";


		IEnumerator Start()
		{
			QueueLabel("Deactivate the 4 towers\nand then\nget to the space ship!");
			QueueLabel("But, don't get caught!");

			while ( Application.isPlaying )
			{
				yield return null;

				if ( labels.Count > 0 )
				{
					currentLabel = labels.Dequeue();
					yield return new WaitForSeconds(3.0f);
				}

				currentLabel = "";
			}
		}


		void Update()
		{
			if ( Input.GetKeyDown(KeyCode.Escape) )
			{
				Application.Quit();
			}
		}


		void SetTower ()
		{
			towersDeactivated++;

			if ( AllTowersDeactivated() )
			{
				QueueLabel( "Towers Deactivated!\nEscape to the Ship!" );
			}
			else
			{
				QueueLabel( (4 - towersDeactivated) + " Towers Left!" );
			}
		}


		public bool AllTowersDeactivated ()
		{
			return towersDeactivated == 4;
		}
		

		public void Win()
		{
			if ( !gameOver && !win )
			{
				QueueLabel("You Win!");
				win = true;
			}
		}
		

		public void GameOver()
		{
			if ( !win && !gameOver )
			{
				QueueLabel("Game Over!");
				gameOver = true;

				StartCoroutine(ResetGame());
			}
		}


		IEnumerator ResetGame ()
		{
			yield return new WaitForSeconds(5.0f);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}


		public void QueueLabel(string label)
		{
			if ( !gameOver )
			{
				labels.Enqueue(label);
			}
		}


		void OnGUI()
		{
			if ( !string.IsNullOrEmpty(currentLabel) )
			{
				GUI.skin.label.fontSize = Screen.height / 10;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), currentLabel);
			}
		}
	}
}