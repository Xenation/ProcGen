using UnityEngine;

namespace ProcGen {
	public class Player : MonoBehaviour {

		public float speed;
		public float sprintMultiplier;
		public float jetpackForce;
		public float lookSpeed;

		private Camera cam;
		private Rigidbody rb;
		
		private bool mouseLock = false;
		private Vector3 vel;
		private float vertVel;

		private void Awake() {
			rb = GetComponent<Rigidbody>();
			cam = GetComponentInChildren<Camera>();
		}

		private void Update() {
			vel.z = Input.GetAxisRaw("Vertical");
			vel.x = Input.GetAxisRaw("Horizontal");
			if (Input.GetButton("Sprint")) {
				vel = vel.normalized * speed * sprintMultiplier;
			} else {
				vel = vel.normalized * speed;
			}
			vel = transform.rotation * Vector3.forward * vel.z + transform.rotation * Vector3.right * vel.x;

			if (Input.GetButton("Jump")) {
				rb.AddForce(Vector3.up * jetpackForce, ForceMode.Acceleration);
				//vel.y = jumpSpeed;
			} else {
				//vel.y = 0f;
			}

			if (mouseLock) {
				Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
				cam.transform.rotation *= Quaternion.Euler(-mouseDelta.y * lookSpeed, 0f, 0f);
				transform.rotation *= Quaternion.Euler(0f, mouseDelta.x * lookSpeed, 0f);
			}
			
			if (Input.GetMouseButtonDown(1)) {
				if (mouseLock) {
					mouseLock = false;
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				} else {
					mouseLock = true;
					Cursor.lockState = CursorLockMode.Locked | CursorLockMode.Confined;
					Cursor.visible = false;
				}
			}
		}

		private void FixedUpdate() {
			vertVel = rb.velocity.y;
			rb.velocity = vel + Vector3.up * vertVel;
		}

	}
}
