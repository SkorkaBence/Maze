using System.Collections.Generic;
using CSFProcessManager;

namespace Maze.generators {
	public class RecursiveBacktracing : Generator {

		public override bool[,] generate(int width, int height) {
			bool[,] maze = new bool[width, height];

			maze[1, 0] = true;
			maze[width - 2, height - 1] = true;

			int max = ((width - 1) / 2) * ((height - 1) / 2);
			int now = 0;

			ProcessManager process = new ProcessManager("Maze Generation");

			int x = 1 + rand.Next(0, ((width - 1) / 2)) * 2;
			int y = 1 + rand.Next(0, ((height - 1) / 2)) * 2;

			maze[x, y] = true;

			List<Direction> steps = new List<Direction>();
			while (true) {
				Direction[] possibleSteps = getDirections(ref maze, x, y);
				if (possibleSteps.Length > 0) {
					int choose = rand.Next(possibleSteps.Length);
					x += possibleSteps[choose].x;
					y += possibleSteps[choose].y;
					maze[x, y] = true;
					x += possibleSteps[choose].x;
					y += possibleSteps[choose].y;
					maze[x, y] = true;
					steps.Add(possibleSteps[choose]);

					now++;

					process.progress(now, max);

					if (saveHistory) {
						mazeHistory.Add((bool[,]) maze.Clone());
					}
				} else {
					deadends++;
					while (true) {
						x -= steps[steps.Count - 1].x * 2;
						y -= steps[steps.Count - 1].y * 2;
						steps.RemoveAt(steps.Count - 1);

						if (steps.Count == 0) {
							process.done();
							return maze;
						} else if (getDirections(ref maze, x, y).Length > 0) {
							break;
						}
					}
				}
			}
		}

		private Direction[] getDirections(ref bool[,] maze, int x, int y) {
			List<Direction> directions = new List<Direction>();

			if (x < maze.GetLength(0) - 2)
				if (!maze[x + 1, y] && !maze[x + 2, y]) {
					directions.Add(new Direction(1, 0));
				}
			if (x > 1)
				if (!maze[x - 1, y] && !maze[x - 2, y]) {
					directions.Add(new Direction(-1, 0));
				}
			if (y > 1)
				if (!maze[x, y - 1] && !maze[x, y - 2]) {
					directions.Add(new Direction(0, -1));
				}
			if (y < maze.GetLength(1) - 2)
				if (!maze[x, y + 1] && !maze[x, y + 2]) {
					directions.Add(new Direction(0, 1));
				}

			return directions.ToArray();
		}
	}
}
