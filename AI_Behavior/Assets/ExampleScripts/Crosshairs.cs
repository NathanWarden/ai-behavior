using UnityEngine;


namespace AIBehaviorExamples
{
	public class Crosshairs : MonoBehaviour
	{
		public Texture2D crosshairs;
		public int drawSize = 32;
		float halfDrawSize = 0.0f;

		Rect drawRect = new Rect(0,0,0,0);


		void Start()
		{
			halfDrawSize = drawSize / 2.0f;
			drawRect.width = drawRect.height = drawSize;
		}


		void Update()
		{
			drawRect.x = Screen.width / 2.0f - halfDrawSize;
			drawRect.y = Screen.height / 2.0f - halfDrawSize;
		}


		void OnGUI()
		{
			GUI.DrawTexture(drawRect, crosshairs);
		}
	}
}