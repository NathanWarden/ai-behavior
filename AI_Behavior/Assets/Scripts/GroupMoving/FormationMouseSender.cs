using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIBehavior;

public class FormationMouseSender : MonoBehaviour 
{
	public string unitMovingStateName;
	public string unitSeekPlayerStateName;
	public GameObject groupMovingIndicator;
	AIFormationMoving formationMovingScript;
	AIFormationAttacking formationAttackingScript;
	bool isDragging= false;
	Vector3 mouseDownPoint;
	Vector3 mouseUpPoint;
	Vector3 mouseDragStart;
	List<GameObject> selectableUnits = new List<GameObject>();
	List<AIBehaviors> selectedAIs = new List<AIBehaviors>();

	void Start () 
	{
		formationMovingScript = GetComponent<AIFormationMoving> ();
		formationAttackingScript = GetComponent<AIFormationAttacking> ();
		mouseDownPoint = Vector3.zero;
		mouseUpPoint = Vector3.zero;
		mouseDragStart = Vector2.zero;
		selectableUnits.AddRange(GameObject.FindGameObjectsWithTag ("Enemy"));
	}

	public void AddSelectableUnit(GameObject newUnit)
	{
		selectableUnits.Add (newUnit);
	}

	public void RemoveSelectableUnit(GameObject deadUnit)
	{
		selectableUnits.Remove (deadUnit);
	}

	void Update () 
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if(Input.GetMouseButtonDown(0))
		{
			isDragging = true;
			mouseDragStart = Input.mousePosition;
			mouseDragStart.y = Screen.height - Input.mousePosition.y;

			// Raycast and save the worldPosition where the selection starts
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				mouseDownPoint = hit.point;
			}
		}
		if (Input.GetMouseButtonUp (0)) 
		{
			isDragging = false;
		
			// Raycast and save the worldPosition where the selection starts
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				mouseUpPoint = hit.point;
			}

			// Set selection bounds
			float minX = 0;
			float minZ = 0;
			float maxX = 0;
			float maxZ = 0;
			if (mouseDownPoint.x < mouseUpPoint.x)
			{
				minX = mouseDownPoint.x;
				maxX = mouseUpPoint.x;
			}
			else
			{
				minX = mouseUpPoint.x;
				maxX = mouseDownPoint.x;
			}

			if (mouseDownPoint.z < mouseUpPoint.z)
			{
				minZ = mouseDownPoint.z;
				maxZ = mouseUpPoint.z;
			}
			else
			{
				minZ = mouseUpPoint.z;
				maxZ = mouseDownPoint.z;
			}

			// Clear previous selected units
			for (int i = 0; i < selectedAIs.Count; i++) 
			{
				GameObject selector = selectedAIs[i].transform.Find ("SelectionCylinder").gameObject;
				if (selector != null)
					selector.SetActive (false);
			}
			selectedAIs.Clear();

			// Select units
			for (int i = 0; i < selectableUnits.Count; i++) 
			{
				Vector3 unitPos = selectableUnits [i].transform.position;

				// Check each unit if it is within the selection bounds
				if (unitPos.x > minX && unitPos.x < maxX && unitPos.z > minZ && unitPos.z < maxZ) 
				{
					AIBehaviors newAI = selectableUnits [i].GetComponentInChildren<AIBehaviors> ();
					if (newAI != null) 
					{
						selectedAIs.Add (newAI);
						Transform selector = newAI.transform.Find ("SelectionCylinder");
						if (selector != null)
							selector.gameObject.SetActive (true);
					}
				}
			}
			Debug.Log ("Total units: "+selectableUnits.Count+" - Selected: " + selectedAIs.Count);
		}
		if (Input.GetMouseButtonDown (1)) 
		{
			// Sending selected units
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				// Moving in formation
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer ("Terrain")) 
				{
					StartCoroutine (ShowGoPoint (hit.point));
					formationMovingScript.SetAIs (selectedAIs.ToArray());
					formationMovingScript.SendAIsTo (hit.point, unitMovingStateName);
				}
				// Attacking
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer ("LocalPlayer")) 
				{
					Debug.LogError ("playerLayer");
					StartCoroutine (ShowAttackPoint (hit.transform.root.position));
					formationAttackingScript.SetAIs (selectedAIs.ToArray());
					formationAttackingScript.SendAttack (hit.transform.root.gameObject, unitSeekPlayerStateName);
				}
			}
		}
	}

	IEnumerator ShowGoPoint (Vector3 point)
	{
		GameObject visualPoint = Instantiate (groupMovingIndicator);
		visualPoint.transform.position = point;

		int count = 40;
		for (int i = 0; i < count; i++) 
		{
			float weight = Mathf.InverseLerp (0, count, i);
			visualPoint.transform.localScale = new Vector3 (1.5f - weight, 0.1f, 1.5f - weight);
			yield return null;
		}
		Destroy (visualPoint);
	}

	IEnumerator ShowAttackPoint (Vector3 objective)
	{
		GameObject visualPoint = Instantiate (groupMovingIndicator);
		visualPoint.transform.position = objective;

		bool growing = true;
		float newSize = 7;
		int count = 100;
		for (int i = 0; i < count; i++) 
		{
			visualPoint.transform.localScale = new Vector3 (newSize, 0.05f, newSize);

			if (growing)
				newSize += 0.2f;
			else
				newSize -= 0.2f;
			
			if (newSize >= 8)
				growing = false;
			if (newSize <= 7)
				growing = true;
			
			yield return null;
		}
		Destroy (visualPoint);
	}

	void OnGUI()
	{
		if (isDragging) 
		{
			float xSize = Input.mousePosition.x -  mouseDragStart.x;
			float ySize = Screen.height - Input.mousePosition.y - mouseDragStart.y;
			GUI.Box (new Rect (mouseDragStart.x, mouseDragStart.y, xSize, ySize), "");
		}
	}
}
