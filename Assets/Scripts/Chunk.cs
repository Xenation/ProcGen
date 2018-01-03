using System;
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
			}
		}

		[Header("Mesh")]
		public Vector2 size;
		[Range(1, 250)]
		public int maxQuadsX;
		[Range(1, 250)]
		public int maxQuadsZ;
		private int quadsX;
		private int quadsZ;
		public int lod = -1;
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

		private Dictionary<Orientation, bool> edgeSeamsFixed = new Dictionary<Orientation, bool>();

		public static Chunk Create(TerrainGen generator, Vector2i chunkPos, Vector2 size, int quadsX, int quadsZ) {
			GameObject go = new GameObject("Chunk");
			go.transform.SetParent(generator.transform);
			Chunk chk = go.AddComponent<Chunk>();
			chk.generator = generator;
			chk.size = size;
			chk.maxQuadsX = quadsX;
			chk.maxQuadsZ = quadsZ;
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

		public void Generate() {
			quadsX = (lod == 0) ? maxQuadsX : maxQuadsX / (generator.lodQuadsDivider * lod) / 2 * 2;
			quadsZ = (lod == 0) ? maxQuadsZ : maxQuadsZ / (generator.lodQuadsDivider * lod) / 2 * 2;
			if (quadsX == 0 || quadsZ == 0) {
				Debug.Log("Generating quadsX=" + quadsX + "quadsZ=" + quadsZ);
			}
			mesh.GeneratePlane(size, quadsX, quadsZ);
			GenerateTerrain(generator.noise, generator.frequency, generator.amplitude, generator.octaveCount, generator.lacunarity, generator.gain, generator.offsetX, generator.offsetZ, generator.mountainessFrequency);
			meshCol.sharedMesh = mesh;
			UpdateFixedSeamIndicator(Orientation.North, false);
			UpdateFixedSeamIndicator(Orientation.East, false);
			UpdateFixedSeamIndicator(Orientation.South, false);
			UpdateFixedSeamIndicator(Orientation.West, false);
		}

		private void GenerateTerrain(MathUtils.NoiseFunction noise, float frequency, float amplitude, int octaveCount, float lacunarity, float gain, float offsetX, float offsetZ, float mountFreq) {
			Vector3[] verts = mesh.vertices;
			for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++) {
				float mountainess = Mathf.PerlinNoise((verts[vertIndex].x + transform.position.x) * mountFreq + offsetX, (verts[vertIndex].z + transform.position.z) * mountFreq + offsetZ);
				verts[vertIndex].y = noise((verts[vertIndex].x + transform.position.x) * frequency + offsetX, (verts[vertIndex].z + transform.position.z) * frequency + offsetZ, octaveCount, lacunarity, gain) * amplitude;
			}

			mesh.vertices = verts;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}

		public void FixEdgeSeams() { // TODO corner seams, change normals of adjacents and avoid adjacent to recalculate on top
			Vector3[] normals = mesh.normals;
			Vector3[] verts = mesh.vertices;
			Vector3[] adjNormals;
			Vector3[] adjVerts;

			//Debug.Log("Fixing Edge Seams");

			// Front Edge
			if (front != null && !edgeSeamsFixed[Orientation.North]) {
				//Debug.Log("EdgeSeam Front");
				adjNormals = front.mesh.normals;
				if (front.lod == lod) {
					FixEdgeSeam(Orientation.North, quadsX, quadsZ, normals, verts, front.quadsX, front.quadsZ, adjNormals, false, 0);
				} else {
					int lodDiff = lod - front.lod;
					if (lodDiff < 0) {
						lodDiff = -lodDiff;
						FixEdgeSeam(Orientation.North, quadsX, quadsZ, normals, verts, front.quadsX, front.quadsZ, adjNormals, true, 2 * lodDiff);
					} else {
						adjVerts = front.mesh.vertices;
						FixEdgeSeam(Orientation.South, front.quadsX, front.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, 2 * lodDiff);
					}
				}
				UpdateFixedSeamIndicator(Orientation.North, true);
			}

			// Back Edge
			if (back != null && !edgeSeamsFixed[Orientation.South]) {
				adjNormals = back.mesh.normals;
				if (back.lod == lod) {
					FixEdgeSeam(Orientation.South, quadsX, quadsZ, normals, verts, back.quadsX, back.quadsZ, adjNormals, false, 0);
				} else {
					int lodDiff = lod - back.lod;
					if (lodDiff < 0) {
						lodDiff = -lodDiff;
						FixEdgeSeam(Orientation.South, quadsX, quadsZ, normals, verts, back.quadsX, back.quadsZ, adjNormals, true, 2 * lodDiff);
					} else {
						adjVerts = back.mesh.vertices;
						FixEdgeSeam(Orientation.North, back.quadsX, back.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, 2 * lodDiff);
					}
				}
				UpdateFixedSeamIndicator(Orientation.South, true);
			}

			// Right Edge
			if (right != null && !edgeSeamsFixed[Orientation.East]) {
				adjNormals = right.mesh.normals;
				if (right.lod == lod) {
					FixEdgeSeam(Orientation.East, quadsX, quadsZ, normals, verts, right.quadsX, right.quadsZ, adjNormals, false, 0);
				} else {
					int lodDiff = lod - right.lod;
					if (lodDiff < 0) {
						lodDiff = -lodDiff;
						FixEdgeSeam(Orientation.East, quadsX, quadsZ, normals, verts, right.quadsX, right.quadsZ, adjNormals, true, 2 * lodDiff);
					} else {
						adjVerts = right.mesh.vertices;
						FixEdgeSeam(Orientation.West, right.quadsX, right.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, 2 * lodDiff);
					}
				}
				UpdateFixedSeamIndicator(Orientation.East, true);
			}

			// Left Edge
			if (left != null && !edgeSeamsFixed[Orientation.West]) {
				adjNormals = left.mesh.normals;
				if (left.lod == lod) {
					FixEdgeSeam(Orientation.West, quadsX, quadsZ, normals, verts, left.quadsX, left.quadsZ, adjNormals, false, 0);
				} else {
					int lodDiff = lod - left.lod;
					if (lodDiff < 0) {
						lodDiff = -lodDiff;
						FixEdgeSeam(Orientation.West, quadsX, quadsZ, normals, verts, left.quadsX, left.quadsZ, adjNormals, true, 2 * lodDiff);
					} else {
						adjVerts = left.mesh.vertices;
						FixEdgeSeam(Orientation.East, left.quadsX, left.quadsZ, adjNormals, adjVerts, quadsX, quadsZ, normals, true, 2 * lodDiff);
					}
				}
				UpdateFixedSeamIndicator(Orientation.West, true);
			}

			mesh.normals = normals;
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
						startIndexlr = hrQuadsX;
						break;
				}
				if (hrside == Orientation.North || hrside == Orientation.South) {
					for (int i = 0; i <= hrQuadsX; i++) {
						int midPointIndex = i % lrTohrQuadsDivider;
						int midPointToNextLRPoint = lrTohrQuadsDivider - midPointIndex;
						if (midPointIndex != 0) {
							hrNormals[startIndexhr + i] = Vector3.Slerp(hrNormals[startIndexhr + i - midPointIndex], Vector3.Slerp(hrNormals[startIndexhr + i + midPointToNextLRPoint], lrNormals[startIndexlr + (i + midPointToNextLRPoint) / lrTohrQuadsDivider], .5f), 1f / lrTohrQuadsDivider);
							hrVertices[startIndexhr + i].y = hrVertices[startIndexhr + i - midPointIndex].y + (hrVertices[startIndexhr + i + midPointToNextLRPoint].y - hrVertices[startIndexhr + i - midPointIndex].y) * (1f / midPointIndex);
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
							int nextLRPointInLR = startIndexlr + (i / lrTohrQuadsDivider + 1) * (lrQuadsX + 1);
							try {
							hrNormals[indexhr] = Vector3.Slerp(hrNormals[prevLRPointInHR], Vector3.Slerp(hrNormals[nextLRPointInHR], lrNormals[nextLRPointInLR], .5f), 1f / lrTohrQuadsDivider);
							} catch (IndexOutOfRangeException) {
								Debug.Log("i=" + i + " hrside=" + hrside.ToString() + " hrQuadsX=" + hrQuadsX + " hrQuadsZ=" + hrQuadsZ + " lrQuadsX=" + lrQuadsX + " lrQuadsZ=" + lrQuadsZ + " hrNormals.Length=" + hrNormals.Length + " lrNormals.Length=" + lrNormals.Length + " indexhr=" + indexhr + " indexlr=" + indexlr + " midPointIndex=" + midPointIndex + " midPointToNextLRPoint=" + midPointToNextLRPoint + " prevLRPointInHR=" + prevLRPointInHR + " nextLRPointInHR=" + nextLRPointInHR + " lrTohrQuadsDivider=" + lrTohrQuadsDivider + " nextLRPointInLR=" + nextLRPointInLR);
							}
							hrVertices[indexhr].y = hrVertices[prevLRPointInHR].y + (hrVertices[nextLRPointInHR].y - hrVertices[prevLRPointInHR].y) * (1f / midPointIndex);
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
