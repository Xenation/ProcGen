using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ProcGen {
	public class GenerationThread {

		private bool _stop = false;
		private object _stopLock = new object();
		private bool stop {
			get {
				lock (_stopLock) {
					return _stop;
				}
			}
			set {
				lock (_stopLock) {
					_stop = value;
				}
			}
		}

		private bool _genRequest = false;
		private object _genRequestLock = new object();
		private bool genRequest {
			get {
				lock (_genRequestLock) {
					return _genRequest;
				}
			}
			set {
				lock (_genRequestLock) {
					_genRequest = value;
				}
			}
		}

		private bool _hasFinished = true;
		private object _hasFinishedLock = new object();
		public bool HasFinished {
			get {
				lock (_hasFinishedLock) {
					return _hasFinished;
				}
			}
			private set {
				lock (_hasFinishedLock) {
					_hasFinished = value;
				}
			}
		}

		private bool _cancel;
		private object _cancelLock = new object();
		private bool cancel {
			get {
				lock (_cancelLock) {
					return _cancel;
				}
			}
			set {
				lock (_cancelLock) {
					_cancel = value;
				}
			}
		}
		
		private TerrainGen _generator;
		private object _generatorLock = new object();
		private TerrainGen generator {
			get {
				lock (_generatorLock) {
					return _generator;
				}
			}
			set {
				lock (_generatorLock) {
					_generator = value;
				}
			}
		}

		private List<Chunk> chunks;

		private List<Chunk> _nextChunks;
		private object _nextChunksLock = new object();
		private List<Chunk> nextChunks {
			get {
				lock (_nextChunksLock) {
					return _nextChunks;
				}
			}
			set {
				lock (_nextChunksLock) {
					_nextChunks = value;
				}
			}
		}

		public delegate void FinishCallback();
		private FinishCallback _callback;
		private object _callbackLock = new object();
		private FinishCallback callback {
			get {
				lock (_callbackLock) {
					return _callback;
				}
			}
			set {
				lock (_callbackLock) {
					_callback = value;
				}
			}
		}

		private Thread thread;

		public GenerationThread() {
			thread = new Thread(Run);
		}

		private void Run() {

			while (!stop) {
				if (genRequest && nextChunks != null) {
					genRequest = false;
					HasFinished = false;
					chunks = nextChunks;
					// Generation Pass
					int generatedCount = 0;
					foreach (Chunk chk in chunks) {
						int prevLODIndex = chk.lodIndex;
						chk.lodIndex = generator.GetLODIndex(chk);
						if (prevLODIndex != chk.lodIndex || chk.MeshAltered) {
							chk.meshRegenerated = true;
							chk.Generate();
							generatedCount++;
						} else {
							chk.meshRegenerated = false;
						}
						if (cancel) {
							break;
						}
					}
					if (!cancel) {
						// Edge Seams Pass
						foreach (Chunk chk in chunks) {
							chk.FixEdgeSeams();
							if (cancel) {
								break;
							}
						}
					}
					if (cancel) {
						cancel = false;
					}
					Debug.Log("Generated: " + generatedCount);
					HasFinished = true;
					if (callback != null) {
						callback();
					}
				}
			}

		}

		public void Start() {
			thread.Start();
		}

		public void Stop() {
			stop = true;
		}

		public void Init(TerrainGen gen, FinishCallback fc) {
			generator = gen;
			callback = fc;
		}

		public void Generate(List<Chunk> chks) {
			genRequest = true;
			nextChunks = chks;
		}

		public void Cancel() {
			cancel = true;
		}

	}
}
