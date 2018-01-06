using UnityEngine;

namespace ProcGen {
	public static class MathUtils {

		public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2) {
			Vector3 lineVec3 = linePoint2 - linePoint1;
			Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
			Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

			float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

			//is coplanar, and not parrallel
			if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f) {
				float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
				intersection = linePoint1 + (lineVec1 * s);
				return true;
			} else {
				intersection = Vector3.zero;
				return false;
			}
		}

		public delegate float NoiseFunction(float x, float y, int octaves, float lacunarity, float gain);

		public static float Perlin(float x, float y, int octaves, float lacunarity, float gain) {
			float octAmp = 1f;
			float octFreq = 1f;
			float output = Mathf.PerlinNoise(x * octFreq, y * octFreq) * octAmp;
			octFreq *= lacunarity;
			octAmp *= gain;
			for (int i = 0; i < octaves - 1; i++) {
				output += Mathf.PerlinNoise(x * octFreq, y * octFreq) * octAmp;
				octFreq *= lacunarity;
				octAmp *= gain;
			}
			return output;
		}

		public static float PerlinAbs(float x, float y, int octaves, float lacunarity, float gain) {
			return Mathf.Abs(Perlin(x, y, octaves, lacunarity, gain));
		}

		public static float PerlinOneMinusAbs(float x, float y, int octaves, float lacunarity, float gain) {
			return 1f - Mathf.Abs(Perlin(x, y, octaves, lacunarity, gain));
		}

	}
}
