using CSFProcessManager;

namespace Maze.generators {
	public class Noise : Generator {

		public override bool[,] generate(int width, int height) {
			bool[,] maze = new bool[width, height];

			ProcessManager process = new ProcessManager("Generating Maze");

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					maze[x, y] = rand.Next(2) == 0;
					if (saveHistory) {
						mazeHistory.Add((bool[,]) maze.Clone());
					}
				}
				process.progress(x, width);
			}

			process.done();

			return maze;
		}

	}
}
