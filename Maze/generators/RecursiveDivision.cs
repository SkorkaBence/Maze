using System;
using CSFProcessManager;

namespace Maze.generators {
	public class RecursiveDivision : Generator {

		private ProcessManager process;
		private int count = 0;

		public override bool[,] generate(int width, int height) {
			bool[,] maze = new bool[width, height];

			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < height - 1; y++) {
					maze[x, y] = true;
				}
			}

			process = new ProcessManager("Generating Maze");

			recursive(ref maze, 1, 1, width - 2, height - 2);

			process.done();

			return maze;
		}

		private void recursive(ref bool[,] maze, int startx, int starty, int endx, int endy) {
			count++;
			process.progress(count, 0);

			if (saveHistory) {
				mazeHistory.Add((bool[,])maze.Clone());
			}

			if ((startx - endx) < (starty - endy)) {
				if (Math.Abs(startx - endx) < 3) {
					return;
				}

				// vizszintesen
				int splitX;
				int cnt = 0;
				do {
					splitX = rand.Next(startx + 2, endx - 1);
					cnt++;
					if (cnt > 10000) return;
				} while (splitX % 2 == 0);

				for (int y = starty; y <= endy; y++) {
					maze[splitX, y] = false;
				}

				int gateY;
				do{
					gateY = rand.Next(starty, endy + 1);
				} while (gateY % 2 == 1);
				maze[splitX, gateY] = true;

				deadends += 2;

				recursive(ref maze, startx, starty, splitX - 1, endy);
				recursive(ref maze, splitX + 1, starty, endx, endy);
			} else {
				if (Math.Abs(starty - endy) < 3) {
					return;
				}

				// fuggolegesen
				int splitY;
				int cnt = 0;
				do {
					splitY = rand.Next(starty + 2, endy - 1);
					cnt++;
					if (cnt > 10000) return;
				} while (splitY % 2 == 0);

				for (int x = startx; x <= endx; x++) {
					maze[x, splitY] = false;
				}

				int gateX;
				do {
					gateX = rand.Next(startx, endx + 1);
				} while (gateX % 2 == 1);
				maze[gateX, splitY] = true;

				recursive(ref maze, startx, starty, endx, splitY - 1);
				recursive(ref maze, startx, splitY + 1, endx, endy);
			}
		}
	}
}
