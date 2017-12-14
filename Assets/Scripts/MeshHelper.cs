using UnityEngine;

namespace ProcGen {
	public static class MeshHelper {

		public static void GeneratePlane(this Mesh mesh, Vector2 size, int quadsX, int quadsY) {
			mesh.GeneratePlane(size, quadsX, quadsY, false, false);
		}

		public static void GeneratePlane(this Mesh mesh, Vector2 size, int quadsX, int quadsY, bool centerX, bool centerY) {
			if (quadsX < 1) {
				quadsX = 1;
			}
			if (quadsY < 1) {
				quadsY = 1;
			}

			// Vertices
			Vector3[] verts = new Vector3[(quadsX + 1) * (quadsY + 1)];
			int index = 0;
			for (int y = 0; y <= quadsY; y++) { // vert loop
				for (int x = 0; x <= quadsX; x++) {
					verts[index++] = new Vector3(size.x / quadsX * x - ((centerX) ? size.x / 2f : 0f), 0, size.y / quadsY * y - ((centerY) ? size.y / 2f : 0f));
				}
			}

			// Indices
			index = 0;
			int[] indices = new int[quadsX * quadsY * 6];
			for (int y = 0; y < quadsY; y++) { // quad loop
				for (int x = 0; x < quadsX; x++) {
					int bl = quadsX * y + y + x;
					int br = bl + 1;
					int tl = quadsX * (y + 1) + (y + 1) + x;
					int tr = tl + 1;

					indices[index++] = bl;
					indices[index++] = tl;
					indices[index++] = tr;

					indices[index++] = bl;
					indices[index++] = tr;
					indices[index++] = br;
				}
			}

			// Mesh Update
			mesh.Clear();
			mesh.vertices = verts;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		}

	}
}
