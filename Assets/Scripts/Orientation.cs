namespace ProcGen {
	public enum Orientation : int {
		North = 0,
		East = 1,
		South = 2,
		West = 3,
		None = -1
	}

	public static class OrientationExt {

		public static Orientation GetOposite(this Orientation ori) {
			switch (ori) {
				case Orientation.North:
					return Orientation.South;
				case Orientation.East:
					return Orientation.West;
				case Orientation.South:
					return Orientation.North;
				case Orientation.West:
					return Orientation.East;
				default:
					return Orientation.None;
			}
		}

	}
}
