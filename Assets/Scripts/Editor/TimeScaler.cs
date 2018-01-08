using UnityEditor;
using UnityEngine;

namespace LD40 {
	public class TimeScaler : EditorWindow {

		public float timeScale = 1f;
		public string fileName = "";

		[MenuItem("Window/Time Scaler")]
		public static void Init() {
			GetWindow<TimeScaler>().Show();
		}

		public void OnGUI() {
			GUILayout.Label("Time Scale:");
			timeScale = EditorGUILayout.Slider(timeScale, 0.001f, 4f);
			fileName = EditorGUILayout.TextField(fileName);
			if (GUILayout.Button("SaveScreen")) {
				string fName = fileName + ".png";
				ScreenCapture.CaptureScreenshot(fName);
				EditorUtility.RevealInFinder(Application.dataPath);
			}
			if (GUILayout.Button("16K Screenshot")) {
				string fName = fileName + ".png";
				ScreenCapture.CaptureScreenshot(fName, 2);
				EditorUtility.RevealInFinder(Application.dataPath);
			}
		}

		public void Update() {
			if (EditorApplication.isPlaying) {
				Time.timeScale = timeScale;
			} else {
				Time.timeScale = 1f;
			}
		}

	}
}
