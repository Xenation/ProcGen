using UnityEngine;

namespace ProcGen {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class Chunk : MonoBehaviour {

		private Vector2i _chkPos;
		public Vector2i ChunkPos {
			get {
				return _chkPos;
			}
			set {
				_chkPos = value;
				transform.position = WorldPosition();
			}
		}

		[Header("Mesh")]
		public Vector2 size;
		[Range(1, 250)]
		public int quadsX;
		[Range(1, 250)]
		public int quadsZ;
		private MeshFilter filter;
		private Mesh mesh;
		private MeshCollider meshCol;
		private MeshRenderer meshRenderer;

		private TerrainGen generator;

		//Adjacents
		private Chunk front;
		private Chunk right;
		private Chunk back;
		private Chunk left;

		public static Chunk Create(TerrainGen generator, Vector2i chunkPos, Vector2 size, int quadsX, int quadsZ) {
			GameObject go = new GameObject("Chunk");
			go.transform.SetParent(generator.transform);
			Chunk chk = go.AddComponent<Chunk>();
			chk.generator = generator;
			chk.size = size;
			chk.quadsX = quadsX;
			chk.quadsZ = quadsZ;
			chk.ChunkPos = chunkPos;
			chk.Init();
			return chk;
		}

		private void Init() {
			filter = GetComponent<MeshFilter>();
			meshCol = GetComponent<MeshCollider>();
			mesh = filter.mesh;
			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material = new Material(generator.terrainMat);
			UpdateAdjacents();
		}

		public Vector3 WorldPosition() {
			return new Vector3(ChunkPos.x * size.x, 0f, ChunkPos.y * size.y);
		}

		public void UpdateAdjacents() {
			//Front
			front = generator.GetChunkAt(ChunkPos + Vector2i.Front);
			if (front != null) {
				front.back = this;
			}
			//Right
			right = generator.GetChunkAt(ChunkPos + Vector2i.Right);
			if (right != null) {
				right.left = this;
			}
			//Back
			back = generator.GetChunkAt(ChunkPos + Vector2i.Back);
			if (back != null) {
				back.front = this;
			}
			//Left
			left = generator.GetChunkAt(ChunkPos + Vector2i.Left);
			if (left != null) {
				left.right = this;
			}
		}

		public void Generate() {
			mesh.GeneratePlane(size, quadsX, quadsZ);
			GenerateTerrain(generator.noise, generator.frequency, generator.amplitude, generator.octaveCount, generator.lacunarity, generator.gain, generator.offsetX, generator.offsetZ);
			meshCol.sharedMesh = mesh;
		}

		private void GenerateTerrain(MathUtils.NoiseFunction noise, float frequency, float amplitude, int octaveCount, float lacunarity, float gain, float offsetX, float offsetZ) {
			Vector3[] verts = mesh.vertices;
			for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++) {
				verts[vertIndex].y = noise((verts[vertIndex].x + transform.position.x) * frequency + offsetX, (verts[vertIndex].z + transform.position.z) * frequency + offsetZ, octaveCount, lacunarity, gain) * amplitude;
			}

			mesh.vertices = verts;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}

		public void FixEdgeSeams() { // TODO corner seams, change normals of adjacents and avoid adjacent to recalculate on top
			Vector3[] normals = mesh.normals;
			Vector3[] adjNormals;

			int startIndexCur;
			int startIndexAdj;

			// Front Edge
			if (front != null) {
				adjNormals = front.mesh.normals;
				startIndexCur = (quadsX + 1) * (quadsZ + 1) - (quadsX + 1);
				startIndexAdj = 0;
				for (int i = 0; i <= quadsX; i++) {
					normals[startIndexCur + i] = Vector3.Slerp(normals[startIndexCur + i], adjNormals[startIndexAdj + i], .5f);
				}
			}

			// Back Edge
			if (back != null) {
				adjNormals = back.mesh.normals;
				startIndexCur = 0;
				startIndexAdj = (quadsX + 1) * (quadsZ + 1) - (quadsX + 1);
				for (int i = 0; i <= quadsX; i++) {
					normals[startIndexCur + i] = Vector3.Slerp(normals[startIndexCur + i], adjNormals[startIndexAdj + i], .5f);
				}
			}

			// Right Edge
			if (right != null) {
				adjNormals = right.mesh.normals;
				startIndexCur = quadsX;
				startIndexAdj = 0;
				for (int i = 0; i <= quadsZ; i++) {
					int indexCur = startIndexCur + i * (quadsX + 1);
					int indexAdj = startIndexAdj + i * (quadsX + 1);
					normals[indexCur] = Vector3.Slerp(normals[indexCur], adjNormals[indexAdj], .5f);
				}
			}

			// Left Edge
			if (left != null) {
				adjNormals = left.mesh.normals;
				startIndexCur = 0;
				startIndexAdj = quadsX;
				for (int i = 0; i <= quadsZ; i++) {
					int indexCur = startIndexCur + i * (quadsX + 1);
					int indexAdj = startIndexAdj + i * (quadsX + 1);
					normals[indexCur] = Vector3.Slerp(normals[indexCur], adjNormals[indexAdj], .5f);
				}
			}

			mesh.normals = normals;
		}

	}
}
