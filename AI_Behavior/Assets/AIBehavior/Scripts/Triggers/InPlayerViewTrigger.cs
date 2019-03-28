using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class InPlayerViewTrigger : BaseTrigger
	{
		public CachePoint updateViewBounds = CachePoint.Awake;
		protected Camera mainCamera = null;
		protected Bounds viewBounds;


		protected override void Awake ()
		{
			base.Awake ();

			if ( updateViewBounds == CachePoint.Awake )
			{
				viewBounds = GetCombinedBounds(transform.parent);
			}
		}


		protected override void Init(AIBehaviors fsm)
		{
			if ( updateViewBounds == CachePoint.StateChanged )
			{
				viewBounds = GetCombinedBounds(fsm.aiTransform);
			}

			mainCamera = GetMainCamera();

			if ( mainCamera == null )
			{
				Debug.LogWarning("No main camera found, 'InPlayerViewTrigger' will not work without a main camera.");
			}
		}


		protected override bool Evaluate(AIBehaviors fsm)
		{
			if ( updateViewBounds == CachePoint.EveryFrame )
			{
				viewBounds = GetCombinedBounds(fsm.aiTransform);
			}

			if ( mainCamera == null )
			{
				mainCamera = GetMainCamera();
			}

			return mainCamera != null && CheckIfInPlayerCameraView(fsm.aiTransform, fsm.raycastLayers);
		}


		public bool CheckIfInPlayerCameraView(Transform fsmTransform, LayerMask raycastLayers)
		{
			Vector3[] checkPoints = GetRaycastPoints(fsmTransform);

			for ( int i = 0; i < checkPoints.Length; i++ )
			{
				if ( PointIsWithinScreenCoordinates(checkPoints[i]) )
				{
					// Make sure there's nothing between the AI and the main camera
					if ( NoRaycastAgainstBounds(fsmTransform, checkPoints[i], raycastLayers) )
					{
						return true;
					}
				}
			}

			return false;
		}


		bool PointIsWithinScreenCoordinates (Vector3 checkPoint)
		{
			Vector3 screenPoint = mainCamera.WorldToScreenPoint(checkPoint);

			if ( screenPoint.z > 0.0f )
			{
				if ( screenPoint.x > 0.0f && screenPoint.x < Screen.width )
				{
					if ( screenPoint.y > 0.0f && screenPoint.y < Screen.height )
					{
						return true;
					}
				}
			}

			return false;
		}


		bool NoRaycastAgainstBounds (Transform fsmTransform, Vector3 checkPoint, LayerMask raycastLayers)
		{
			RaycastHit hit;
			bool result = false;
			float raycastDistance = mainCamera.farClipPlane;

			Vector3 cameraPosition = mainCamera.transform.position;
			Ray ray = new Ray(cameraPosition, checkPoint - cameraPosition);
			bool wasHit = Physics.Raycast(ray, out hit, raycastDistance, raycastLayers) && !hit.transform.IsChildOf(fsmTransform);

			if ( !wasHit )
			{
				result = true;
			}

			return result;
		}


		Vector3[] GetRaycastPoints (Transform fsmTransform)
		{
			Vector3 center = fsmTransform.TransformPoint(viewBounds.center);
			Vector3 extents = viewBounds.extents;
			Vector3[] checkPoints;

			if ( viewBounds.extents.sqrMagnitude > 0.001f )
			{
				checkPoints = new Vector3[9];

				checkPoints[1] = center + new Vector3(extents.x, extents.y, extents.z); // right top back
				checkPoints[2] = center + new Vector3(-extents.x, extents.y, extents.z); // left top back
				checkPoints[3] = center + new Vector3(extents.x, -extents.y, extents.z); // right bottom back
				checkPoints[4] = center + new Vector3(-extents.x, -extents.y, extents.z); // left bottom back
				checkPoints[5] = center + new Vector3(extents.x, extents.y, -extents.z); // right top front
				checkPoints[6] = center + new Vector3(-extents.x, extents.y, -extents.z); // left top front
				checkPoints[7] = center + new Vector3(extents.x, -extents.y, -extents.z); // right bottom front
				checkPoints[8] = center + new Vector3(-extents.x, -extents.y, -extents.z); // left bottom front
			}
			else
			{
				checkPoints = new Vector3[1];
			}

			checkPoints[0] = center;

			return checkPoints;
		}


		protected virtual Camera GetMainCamera ()
		{
			return Camera.main;
		}


		protected virtual Bounds GetCombinedBounds (Transform fsmTransform)
		{
			Renderer[] renderers = fsmTransform.GetComponentsInChildren<Renderer>();
			Bounds newBounds = new Bounds();
			float minX = 0.0f;
			float maxX = 0.0f;
			float minY = 0.0f;
			float maxY = 0.0f;
			float minZ = 0.0f;
			float maxZ = 0.0f;

			if ( renderers.Length > 0 )
			{
				UpdateMinMaxBoundsValues(renderers[0].bounds, ref minX, ref maxX, ref minY, ref maxY, ref minZ, ref maxZ, true);

				for ( int i = 1; i < renderers.Length; i++ )
				{
					UpdateMinMaxBoundsValues(renderers[i].bounds, ref minX, ref maxX, ref minY, ref maxY, ref minZ, ref maxZ);
				}

				newBounds = GetBoundsFromMinMaxValues(minX, maxX, minY, maxY, minZ, maxZ);
				newBounds.center = fsmTransform.InverseTransformPoint(newBounds.center);
			}
			else
			{
				newBounds.center = fsmTransform.position;
				newBounds.extents = Vector3.zero;
			}

			return newBounds;
		}


		void UpdateMinMaxBoundsValues(Bounds rendererBounds,
																ref float minX, ref float maxX,
																ref float minY, ref float maxY,
																ref float minZ, ref float maxZ,
																bool force = false)
		{
			Vector3 center = rendererBounds.center;
			Vector3 extents = rendererBounds.extents;
			float newMinX = center.x - extents.x;
			float newMaxX = center.x + extents.x;
			float newMinY = center.y - extents.y;
			float newMaxY = center.y + extents.y;
			float newMinZ = center.z - extents.z;
			float newMaxZ = center.z + extents.z;
					
			if ( newMinX < minX || force )
			{
				minX = newMinX;
			}

			if ( newMaxX > maxX || force )
			{
				maxX = newMaxX;
			}
					
			if ( newMinY < minY || force )
			{
				minY = newMinY;
			}

			if ( newMaxY > maxY || force )
			{
				maxY = newMaxY;
			}
					
			if ( newMinZ < minZ || force )
			{
				minZ = newMinZ;
			}

			if ( newMaxZ > maxZ || force )
			{
				maxZ = newMaxZ;
			}
		}


		Bounds GetBoundsFromMinMaxValues(float minX, float maxX,
																		float minY, float maxY,
																		float minZ, float maxZ)
		{
			Bounds newBounds = new Bounds();
			float x = Mathf.Lerp(minX, maxX, 0.5f);
			float y = Mathf.Lerp(minY, maxY, 0.5f);
			float z = Mathf.Lerp(minZ, maxZ, 0.5f);
			float width = maxX - minX;
			float height = maxY - minY;
			float depth = maxZ - minZ;

			newBounds.center = new Vector3(x, y, z);
			newBounds.extents = new Vector3(width / 2.0f, height / 2.0f, depth / 2.0f);

			return newBounds;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "In Player View";
		}
	}
}