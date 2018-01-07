using UnityEngine;

namespace ProcGen {
	public class Building : MonoBehaviour {

		public static Building CreateNew(Transform parent, GameObject prefab, Vector3 pos, float h) {
			GameObject go = Instantiate(prefab, parent);
			go.name = "Building";
			go.transform.SetParent(parent);
			go.transform.position = pos;
			Building building = go.AddComponent<Building>();
			building.Init(h);
			return building;
		}

		public float height;

		public void Init(float h) {
			height = h;
			CubeFit();
		}

		private void CubeFit() {
			transform.position += Vector3.up * ((height / 2f) - 10f);
			transform.localScale = new Vector3(transform.localScale.x, height + 20f, transform.localScale.z);
		}

	}
}
