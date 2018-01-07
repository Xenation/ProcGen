using UnityEngine;

namespace ProcGen {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class CameraWriteDepth : MonoBehaviour {

		public void OnEnable() {
			GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

	}
}
