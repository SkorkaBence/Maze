using System;
using System.Collections.Generic;
using CSFProcessManager;

namespace Maze.solver {
	public abstract class MazeSolver {

		protected bool[,] maze;
		protected List<Point> mazeGraph = new List<Point>();
		protected bool saveHistory = false;
		protected List<Point[]> history = new List<Point[]>();

		protected MazeSolver(bool[,] maze) {
			this.maze = maze;
			generateGraph();
			Console.WriteLine(mazeGraph.Count + " graph points generated");
			maze = null;
		}

		public int getWidth() {
			return maze.GetLength(0);
		}

		public int getHeight() {
			return maze.GetLength(1);
		}

		public int getLength() {
			return mazeGraph.Count;
		}

		public List<Point> getGraphPoints() {
			return mazeGraph;
		}

		public void enableHistory() {
			saveHistory = true;
		}

		public List<Point[]> getHistory() {
			return history;
		}

		private void generateGraph() {
			if (maze == null)
				throw new Exception();

			int[,] addeds = new int[maze.GetLength(0), maze.GetLength(1)];
			int[,] awarr = new int[maze.GetLength(0), maze.GetLength(1)];

			ProcessManager process = new ProcessManager("Building Graph");

			for (int x = 1; x < maze.GetLength(0) - 1; x++) {
				for (int y = 1; y < maze.GetLength(1) - 1; y++) {
					addeds[x, y] = -1;
					if (maze[x, y]) {
						int aw = 0;
						for (int ax = -1; ax <= 1; ax++) {
							for (int ay = -1; ay <= 1; ay++) {
								if (((ax == 0) ^ (ay == 0)) && maze[x + ax, y + ay]) {
									aw++;
								}
							}
						}

						bool added = false;
						awarr[x, y] = aw;
						switch (aw) {
							case 1:
								break;
							case 2:
								if (!((maze[x - 1, y] && maze[x + 1, y]) || (maze[x, y - 1] && maze[x, y + 1]))) {
									mazeGraph.Add(new Point(mazeGraph.Count, x, y));
									added = true;
								}
								break;
							case 3:
								mazeGraph.Add(new Point(mazeGraph.Count, x, y));
								added = true;
								break;
							case 4:
								if (awarr[x, y - 1] != 4 || awarr[x - 1, y] != 4) {
									mazeGraph.Add(new Point(mazeGraph.Count, x, y));
									added = true;
								}
								break;
						}

						if (added) {
							addeds[x, y] = mazeGraph.Count - 1;
							int sx = x;
							int sy = y;
							while (true) {
								sx--;
								if (!maze[sx, y]) {
									break;
								} else {
									if (addeds[sx, y] > -1) {
										int length = Math.Abs(x - sx);
										mazeGraph[addeds[x, y]].addConnections(mazeGraph[addeds[sx, y]], length);
										mazeGraph[addeds[sx, y]].addConnections(mazeGraph[addeds[x, y]], length);
										break;
									}
								}
							}
							while (true) {
								sy--;
								if (!maze[x, sy]) {
									break;
								} else {
									if (addeds[x, sy] > -1) {
										int length = Math.Abs(y - sy);
										mazeGraph[addeds[x, y]].addConnections(mazeGraph[addeds[x, sy]], length);
										mazeGraph[addeds[x, sy]].addConnections(mazeGraph[addeds[x, y]], length);
										break;
									}
								}
							}
						}
					}
				}
				if (x % 100 == 0) {
					process.progress(x, maze.GetLength(0));
				}
			}
			process.done();
		}

		public abstract Point[] solve(int fromid, int toid);
	}
}
