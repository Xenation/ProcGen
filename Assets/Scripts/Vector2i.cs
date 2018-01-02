using UnityEngine;

namespace ProcGen {
	public struct Vector2i {

		public static Vector2i One {
			get { return new Vector2i(1, 1); }
		}
		public static Vector2i Zero {
			get { return new Vector2i(0, 0); }
		}

		public static Vector2i Front {
			get { return new Vector2i(0, 1); }
		}
		public static Vector2i Right {
			get { return new Vector2i(1, 0); }
		}
		public static Vector2i Back {
			get { return new Vector2i(0, -1); }
		}
		public static Vector2i Left {
			get { return new Vector2i(-1, 0); }
		}

		public int x;
		public int y;

		public Vector2i(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public static float Distance(Vector2i a, Vector2i b) {
			return Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));
		}

		public static Vector2i operator +(Vector2i v1, Vector2i v2) {
			return new Vector2i(v1.x + v2.x, v1.y + v2.y);
		}

		public static Vector2i operator -(Vector2i v1, Vector2i v2) {
			return new Vector2i(v1.x - v2.x, v1.y - v2.y);
		}

		public static Vector2i operator *(Vector2i v, int a) {
			return new Vector2i(v.x * a, v.y * a);
		}

		public static bool operator ==(Vector2i a, Vector2i b) {
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2i a, Vector2i b) {
			return a.x != b.x || a.y != b.y;
		}

	}
}
