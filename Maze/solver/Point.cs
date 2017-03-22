using System.Collections.Generic;

namespace Maze.solver {
	public class Point {

		private readonly int id;
		private List<Point> connections = new List<Point>();
		private List<int> length = new List<int>();
		private int x;
		private int y;

		public Point(int id, int x, int y) {
			this.id = id;
			this.x = x;
			this.y = y;
		}

		public int getX() {
			return x;
		}

		public int getY() {
			return y;
		}

		public List<Point> getConnections() {
			return connections;
		}

		public List<int> getLengths() {
			return length;
		}

		public void addConnections(Point o, int l) {
			connections.Add(o);
			length.Add(l);
		}

		public int getId() {
			return id;
		}

	}
}