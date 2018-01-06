using System.Collections.Generic;
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
				cachedPos = transform.position;
			}
		}

		[Header("Mesh")]
		public Vector2 size;
		private int quadsX;
		private int quadsZ;
		public int lodIndex = -1;
		private MeshFilter filter;
		private Mesh mesh;
		private PreparedMesh prepMesh;
		private MeshCollider meshCol;
		private MeshRenderer meshRenderer;
		public bool MeshAltered { get; private set; }

		private TerrainGen generator;

		//Adjacents
		private Chunk front;
		private Chunk right;
		private Chunk back;
		private Chunk left;

		private Dictionary<Orientation, bool> edgeSeamsFixed = new Dictionary<Orientation, bool>();
		private Vector3 cachedPos;
		public bool meshRegenerated;
		private List<Vector3> treePositions = new List<Vector3>();
		private bool treePositionsGenerated = false;
		private bool treesSpawned = false;
		private List<GameObject> treeContainers = new List<GameObject>();
		private List<Mesh> treeMeshes = new List<Mesh>();

		public static Chunk Create(TerrainGen generator, Vector2i chunkPos, Vector2 size) {
			GameObject go = new GameObject("Chunk");
			go.transform.SetParent(generator.transform);
			Chunk chk = go.AddComponent<Chunk>();
			chk.generator = generator;
			chk.size = size;
			chk.ChunkPos = chunkPos;
			chk.Init();
			return chk;
		}

		private void Init() {
			filter = GetComponent<MeshFilter>();
			meshCol = GetComponent<MeshCollider>();
			mesh = filter.mesh;
			prepMesh = mesh.CreatePreparedMesh();
			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material = new Material(generator.terrainMat);
			InitEdgeSeamsFixed();
			UpdateAdjacents();
		}

		public void InitEdgeSeamsFixed() {
			edgeSeamsFixed.Add(Orientation.North, false);
			edgeSeamsFixed.Add(Orientation.East, false);
			edgeSeamsFixed.Add(Orientation.South, false);
			edgeSeamsFixed.Add(Orientation.West, false);
		}

		public Vector3 WorldPosition() {
			return new Vector3(ChunkPos.x * size.x, 0f, ChunkPos.y * size.y);
		}

		public Vector3 Get3DSize() {
			return mesh.bounds.size;
		}

		public Mesh GetMesh() {
			return mesh;
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

		private Chunk GetAdjacent(Orientation side) {
			switch (side) {
				case Orientation.North:
					return front;
				case Orientation.East:
					return right;
				case Orientation.South:
					return back;
				case Orientation.West:
					return left;
				default:
					return null;
			}
		}

		public void Generate(float offsetX, float offsetZ, AnimationCurve continentToAmpCurve) {
			quadsX = generator.GetLODLevel(lodIndex).quads;
			quadsZ = generator.GetLODLevel(lodIndex).quads;
			if (quadsX == 0 || quadsZ == 0) {
				Debug.Log("Generating quadsX=" + quadsX + "quadsZ=" + quadsZ);
			}
			prepMesh.GeneratePlane(size, quadsX, quadsZ);
			GenerateTerrain(generator.noise, generator.frequency, generator.amplitude, generator.octaveCount, generator.lacunarity, generator.gain, offsetX, offsetZ, generator.continentsFrequency, continentToAmpCurve);
			UpdateFixedSeamIndicator(Orientation.North, false);
			UpdateFixedSeamIndicator(Orientation.East, false);
			UpdateFixedSeamIndicator(Orientation.South, false);
			UpdateFixedSeamIndicator(Orientation.West, false);
		}

		public void Finalise() {
			prepMesh.Apply();
			if (!treePositionsGenerated) {
				GenerateForest(generator.forestsFrequency, generator.forestsThreashold, generator.offsetX, generator.offsetZ);
			}
			if (meshRegenerated) {
				meshCol.sharedMesh = mesh;
				RefreshTrees();
			}
		}

		private void GenerateTerrain(MathUtils.NoiseFunction noise, float frequency, float amplitude, int octaveCount, float lacunarity, float gain, float offsetX, float offsetZ, float continentsFreq, AnimationCurve continentToAmpCurve) {
			Vector3[] verts = prepMesh.vertices;
			for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++) {
				float mountainess = Mathf.PerlinNoise((verts[vertIndex].x + cachedPos.x) * continentsFreq + offsetX, (verts[vertIndex].z + cachedPos.z) * continentsFreq + offsetZ);
				float ampModifier = continentToAmpCurve.Evaluate(mountainess);
				verts[vertIndex].y = noise((verts[vertIndex].x + cachedPos.x) * frequency + offsetX, (verts[vertIndex].z + cachedPos.z) * frequency + offsetZ, octaveCount, lacunarity, gain) * amplitude * ampModifier;
			}

			prepMesh.vertices = verts;
			prepMesh.RecalculateNormals();
			prepMesh.RecalculateBounds();
			MeshAltered = false;
		}

		private float GetHeightAt(Vector3 pos) {
			float mountainess = Mathf.PerlinNoise(pos.x * generator.continentsFrequency + generator.offsetX, pos.z * generator.continentsFrequency + generator.offsetZ);
			float ampModifier = generator.continentValueToAmplification.Evaluate(mountainess);
			return generator.noise(pos.x * generator.frequency + generator.offsetX, pos.z * generator.frequency + generator.offsetZ, generator.octaveCount, generator.lacunarity, generator.gain) * generator.amplitude * ampModifier;
		}

		private void GenerateForest(float forestFreq, float forestThr, float offsetX, float offsetZ) {
			for (int i = 0; i < generator.treesPerChunk; i++) {
				Vector3 treePos = new Vector3(Random.Range(0, size.x), 0f, Random.Range(0, size.y));
				bool forest = (Mathf.PerlinNoise((cachedPos.x + treePos.x) * generator.forestsFrequency + generator.offsetX, (cachedPos.z + treePos.z) * generator.forestsFrequency + generator.offsetZ) > generator.forestsThreashold);
				if (forest) {
					treePos.y = GetHeightAt(cachedPos + treePos);
					if (treePos.y > generator.waterLevel + generator.shoreHeight && treePos.y < generator.forestsMaxAltitude) {
						treePositions.Add(treePos);
					}
				}
			}
			treePositionsGenerated = true;
		}

		private void RefreshTrees() {
			if (generator.GetLODLevel(lodIndex).genForest) {
				if (!treesSpawned) {
					for (int containerIndex = 0; containerIndex < treePositions.Count / generator.treesPerContainer; containerIndex++) {
						GameObject treesContainer = new GameObject("TreesContainer" + containerIndex, typeof(MeshFilter), typeof(MeshRenderer));
						treesContainer.transform.SetParent(transform, false);
						treeContainers.Add(treesContainer);
						Mesh treesMesh = treesContainer.GetComponent<MeshFilter>().mesh;
						treeMeshes.Add(treesMesh);

						MeshFilter[] prefabMeshes = generator.treePrefab.GetComponentsInChildren<MeshFilter>();
						CombineInstance[] prefabCombines = new CombineInstance[prefabMeshes.Length * generator.treesPerContainer];

						int combineIndex = 0;
						int startPosIndex = containerIndex * generator.treesPerContainer;
						for (int i = startPosIndex; i < startPosIndex + generator.treesPerContainer && i < treePositions.Count; i++) {
							for (int meshIndex = 0; meshIndex < prefabMeshes.Length; meshIndex++) {
								prefabCombines[combineIndex].mesh = prefabMeshes[meshIndex].sharedMesh;
								prefabCombines[combineIndex].transform = Matrix4x4.Translate(treePositions[i]) * prefabMeshes[meshIndex].transform.localToWorldMatrix;
								combineIndex++;
							}
						}

						treesMesh.CombineMeshes(prefabCombines);
						treesContainer.GetComponent<MeshRenderer>().material = generator.treeMaterial;
					}
					treesSpawned = true;
				}
			} else {
				foreach (GameObject container in treeContainers) {
					Destroy(container);
				}
				treeContainers.Clear();
				treeMeshes.Clear();
				treesSpawned = false;
			}
		}

		public void FixEdgeSeams() { // TODO corner seams, change normals of adjacents and avoid adjacent to recalculate on top
			Vector3[] normals = prepMesh.normals;
			Vector3[] verts = prepMesh.vertices;
			Vector3[] adjNormals;
			Vector3[] adjVerts;

			// Front Edge
			if (front != null && !edgeSeamsFixed[Orientation.North]) {
				adjNormals = front.prepMesh.normals;
				if (front.lodIndex == lodIndex) {
					FixEdgeSeam(Orientation.North, quadsX, quadsZ, normals, verts, front.quadsX, front.quadsZ, adjNormals, false, 0);
				} else {
					float lodDiff = quadsX / (float) front.quadsX;
					if (lodDiff >= 1f) {
						FixEdgeSeam(Orientation.North, quadsX, quadsZ, normals, verts, front.quadsX, front.quadsZ, adjNormals, true, (int) lodDiff);
					} else {
						lodDiff = 1f / lodDiff;
						adjVerts = front.prepMesh.vertices;
						FixEdgeSeam(Orientation.South, front.quadsX, front.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, (int) lodDiff);
						front.prepMesh.vertices = adjVerts;
						front.MeshAltered = true;
					}
					MeshAltered = true;
				}
				front.prepMesh.normals = adjNormals;
				UpdateFixedSeamIndicator(Orientation.North, true);
			}

			// Back Edge
			if (back != null && !edgeSeamsFixed[Orientation.South]) {
				adjNormals = back.prepMesh.normals;
				if (back.lodIndex == lodIndex) {
					FixEdgeSeam(Orientation.South, quadsX, quadsZ, normals, verts, back.quadsX, back.quadsZ, adjNormals, false, 0);
				} else {
					float lodDiff = quadsX / (float) back.quadsX;
					if (lodDiff >= 1f) {
						FixEdgeSeam(Orientation.South, quadsX, quadsZ, normals, verts, back.quadsX, back.quadsZ, adjNormals, true, (int) lodDiff);
					} else {
						lodDiff = 1f / lodDiff;
						adjVerts = back.prepMesh.vertices;
						FixEdgeSeam(Orientation.North, back.quadsX, back.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, (int) lodDiff);
						back.prepMesh.vertices = adjVerts;
						back.MeshAltered = true;
					}
					MeshAltered = true;
				}
				back.prepMesh.normals = adjNormals;
				UpdateFixedSeamIndicator(Orientation.South, true);
			}

			// Right Edge
			if (right != null && !edgeSeamsFixed[Orientation.East]) {
				adjNormals = right.prepMesh.normals;
				if (right.lodIndex == lodIndex) {
					FixEdgeSeam(Orientation.East, quadsX, quadsZ, normals, verts, right.quadsX, right.quadsZ, adjNormals, false, 0);
				} else {
					float lodDiff = quadsX / (float) right.quadsX;
					if (lodDiff >= 1f) {
						FixEdgeSeam(Orientation.East, quadsX, quadsZ, normals, verts, right.quadsX, right.quadsZ, adjNormals, true, (int) lodDiff);
					} else {
						lodDiff = 1f / lodDiff;
						adjVerts = right.prepMesh.vertices;
						FixEdgeSeam(Orientation.West, right.quadsX, right.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, (int) lodDiff);
						right.prepMesh.vertices = adjVerts;
						right.MeshAltered = true;
					}
					MeshAltered = true;
				}
				right.prepMesh.normals = adjNormals;
				UpdateFixedSeamIndicator(Orientation.East, true);
			}

			// Left Edge
			if (left != null && !edgeSeamsFixed[Orientation.West]) {
				adjNormals = left.prepMesh.normals;
				if (left.lodIndex == lodIndex) {
					FixEdgeSeam(Orientation.West, quadsX, quadsZ, normals, verts, left.quadsX, left.quadsZ, adjNormals, false, 0);
				} else {
					float lodDiff = quadsX / (float) left.quadsX;
					if (lodDiff >= 1f) {
						FixEdgeSeam(Orientation.West, quadsX, quadsZ, normals, verts, left.quadsX, left.quadsZ, adjNormals, true, (int) lodDiff);
					} else {
						lodDiff = 1f / lodDiff;
						adjVerts = left.prepMesh.vertices;
						FixEdgeSeam(Orientation.East, left.quadsX, left.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, (int) lodDiff);
						left.prepMesh.vertices = adjVerts;
						left.MeshAltered = true;
					}
					MeshAltered = true;
				}
				left.prepMesh.normals = adjNormals;
				UpdateFixedSeamIndicator(Orientation.West, true);
			}

			prepMesh.vertices = verts;		
			prepMesh.normals = normals;
		}

		private void UpdateFixedSeamIndicator(Orientation side, bool state) {
			edgeSeamsFixed[side] = state;
			Chunk adj = GetAdjacent(side);
			if (adj != null) {
				adj.edgeSeamsFixed[side.GetOposite()] = state;
			}
		}

		private void FixEdgeSeam(Orientation hrside, int hrQuadsX, int hrQuadsZ, Vector3[] hrNormals, Vector3[] hrVertices, int lrQuadsX, int lrQuadsZ, Vector3[] lrNormals, bool inequalRes, int lrTohrQuadsDivider) {
			int startIndexhr = 0;
			int startIndexlr = 0;

			if (inequalRes) {
				switch (hrside) {
					case Orientation.North:
						startIndexhr = (hrQuadsX + 1) * (hrQuadsZ + 1) - (hrQuadsX + 1);
						startIndexlr = 0;
						break;

					case Orientation.East:
						startIndexhr = hrQuadsX;
						startIndexlr = 0;
						
						break;

					case Orientation.South:
						startIndexhr = 0;
						startIndexlr = (lrQuadsX + 1) * (lrQuadsZ + 1) - (lrQuadsX + 1);
						break;

					case Orientation.West:
						startIndexhr = 0;
						startIndexlr = lrQuadsX;
						break;
				}
				if (hrside == Orientation.North || hrside == Orientation.South) {
					for (int i = 0; i <= hrQuadsX; i++) {
						int midPointIndex = i % lrTohrQuadsDivider;
						int midPointToNextLRPoint = lrTohrQuadsDivider - midPointIndex;
						if (midPointIndex != 0) {
							hrNormals[startIndexhr + i] = Vector3.Slerp(hrNormals[startIndexhr + i - midPointIndex], Vector3.Slerp(hrNormals[startIndexhr + i + midPointToNextLRPoint], lrNormals[startIndexlr + (i + midPointToNextLRPoint) / lrTohrQuadsDivider], .5f), 1f / lrTohrQuadsDivider);
							//Debug.Log("base height = " + hrVertices[startIndexhr + i].y);
							hrVertices[startIndexhr + i].y = hrVertices[startIndexhr + i - midPointIndex].y + (hrVertices[startIndexhr + i + midPointToNextLRPoint].y - hrVertices[startIndexhr + i - midPointIndex].y) * (midPointIndex / (float) lrTohrQuadsDivider);
							//Debug.Log("new height = " + hrVertices[startIndexhr + i].y + " between " + hrVertices[startIndexhr + i - midPointIndex].y + " and " + hrVertices[startIndexhr + i + midPointToNextLRPoint].y);
						} else {
							hrNormals[startIndexhr + i] = Vector3.Slerp(hrNormals[startIndexhr + i], lrNormals[startIndexlr + i / lrTohrQuadsDivider], .5f);
							lrNormals[startIndexlr + i / lrTohrQuadsDivider] = hrNormals[startIndexhr + i];
						}
					}
				} else if (hrside == Orientation.East || hrside == Orientation.West) {
					for (int i = 0; i <= hrQuadsZ; i++) {
						int indexhr = startIndexhr + i * (hrQuadsX + 1);
						int indexlr = startIndexlr + (i / lrTohrQuadsDivider) * (lrQuadsX + 1);
						int midPointIndex = i % lrTohrQuadsDivider;
						int midPointToNextLRPoint = lrTohrQuadsDivider - midPointIndex;
						if (midPointIndex != 0) {
							int prevLRPointInHR = startIndexhr + (i - midPointIndex) * (hrQuadsX + 1);
							int nextLRPointInHR = startIndexhr + (i + midPointToNextLRPoint) * (hrQuadsX + 1);
							int nextLRPointInLR = indexlr + (lrQuadsX + 1);
							//try {
							hrNormals[indexhr] = Vector3.Slerp(hrNormals[prevLRPointInHR], Vector3.Slerp(hrNormals[nextLRPointInHR], lrNormals[nextLRPointInLR], .5f), 1f / lrTohrQuadsDivider);
							//} catch (IndexOutOfRangeException) {
							//	Debug.Log("i=" + i + " ilr=" + (i / lrTohrQuadsDivider) + " hrside=" + hrside.ToString() + " hrQuadsX=" + hrQuadsX + " hrQuadsZ=" + hrQuadsZ + " lrQuadsX=" + lrQuadsX + " lrQuadsZ=" + lrQuadsZ + " hrNormals.Length=" + hrNormals.Length + " lrNormals.Length=" + lrNormals.Length + " indexhr=" + indexhr + " indexlr=" + indexlr + " midPointIndex=" + midPointIndex + " midPointToNextLRPoint=" + midPointToNextLRPoint + " prevLRPointInHR=" + prevLRPointInHR + " nextLRPointInHR=" + nextLRPointInHR + " lrTohrQuadsDivider=" + lrTohrQuadsDivider + " nextLRPointInLR=" + nextLRPointInLR);
							//}
							hrVertices[indexhr].y = hrVertices[prevLRPointInHR].y + (hrVertices[nextLRPointInHR].y - hrVertices[prevLRPointInHR].y) * (midPointIndex / (float) lrTohrQuadsDivider);
						} else {
							hrNormals[indexhr] = Vector3.Slerp(hrNormals[indexhr], lrNormals[indexlr], .5f);
							lrNormals[indexlr] = hrNormals[indexhr];
						}
					}
				} 
			} else {
				switch (hrside) {
					case Orientation.North:
						startIndexhr = (hrQuadsX + 1) * (hrQuadsZ + 1) - (hrQuadsX + 1);
						startIndexlr = 0;
						for (int i = 0; i <= hrQuadsX; i++) {
							hrNormals[startIndexhr + i] = Vector3.Slerp(hrNormals[startIndexhr + i], lrNormals[startIndexlr + i], .5f);
						}
						break;

					case Orientation.East:
						startIndexhr = hrQuadsX;
						startIndexlr = 0;
						for (int i = 0; i <= hrQuadsZ; i++) {
							int indexhr = startIndexhr + i * (hrQuadsX + 1);
							int indexlr = startIndexlr + i * (lrQuadsX + 1);
							hrNormals[indexhr] = Vector3.Slerp(hrNormals[indexhr], lrNormals[indexlr], .5f);
						}
						break;

					case Orientation.South:
						startIndexhr = 0;
						startIndexlr = (lrQuadsX + 1) * (lrQuadsZ + 1) - (lrQuadsX + 1);
						for (int i = 0; i <= hrQuadsX; i++) {
							hrNormals[startIndexhr + i] = Vector3.Slerp(hrNormals[startIndexhr + i], lrNormals[startIndexlr + i], .5f);
						}
						break;

					case Orientation.West:
						startIndexhr = 0;
						startIndexlr = lrQuadsX;
						for (int i = 0; i <= hrQuadsZ; i++) {
							int indexCur = startIndexhr + i * (hrQuadsX + 1);
							int indexAdj = startIndexlr + i * (lrQuadsX + 1);
							hrNormals[indexCur] = Vector3.Slerp(hrNormals[indexCur], lrNormals[indexAdj], .5f);
						}
						break;
				}
				
			}
		}

	}
}