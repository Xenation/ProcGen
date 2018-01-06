using UnityEngine;

namespace ProcGen {
	public class AsyncOperationCallback {

		public AsyncOperation AsyncOp { get; private set; }

		public delegate void Callback();
		public event Callback callback;

		private bool called = false;

		public AsyncOperationCallback(AsyncOperation op) {
			AsyncOp = op;
		}

		public void UpdateStatus() {
			if (AsyncOp.isDone && !called) {
				if (callback != null) {
					callback.Invoke();
				}
				called = true;
			}
		}

	}
}
