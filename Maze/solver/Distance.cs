using System.Collections.Generic;

namespace Maze.solver {
	class Distance {

		public int id { get; set; }
		public int x { get; set; }
		public int y { get; set; } 
		public int distance { get; set; } = 0;
		public List<Point> path { get; set; } = new List<Point>();

		public Distance(int id, int x, int y) {
			this.id = id;
			this.x = x;
			this.y = y;
		}

	}
}