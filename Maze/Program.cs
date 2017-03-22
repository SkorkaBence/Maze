using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using CSFProcessManager;
using Maze.generators;
using Maze.solver.algs;
using Maze.solver;

namespace Maze {
	class MainClass {
		public static void Main(string[] args) {

			bool saveAnimation = false;

			for (int i = 0; i < args.Length; i++) {
				saveAnimation |= (args[i] == "-animation" || args[i] == "/animation");
			}

			Console.Write("Load or Generate? [l/g] ");
			string cmd = Console.ReadLine();
			Image solveit;
			string filename;
			if (cmd == "g") {
				Console.WriteLine("Choose the generator:");
				Console.WriteLine("0 - Recursive backtracking");
				Console.WriteLine("1 - Recursive division");
				Console.WriteLine("2 - Noise");
				Console.Write("#");
				cmd = Console.ReadLine();

				Generator generator;
				switch (cmd) {
					case "0":
						generator = new RecursiveBacktracing();
						break;
					case "1":
						generator = new RecursiveDivision();
						break;
					case "2":
						generator = new Noise();
						break;
					default:
						return;
				}

				int w, h;

				try {
					Console.Write("Width: ");
					w = int.Parse(Console.ReadLine());
					Console.Write("Height: ");
					h = int.Parse(Console.ReadLine());
				} catch (Exception e) {
					Console.WriteLine("Exception: " + e.Message);
					return;
				}

				filename = cmd + "x" + w + "x" + h;

				if (w % 2 == 0) w++;
				if (h % 2 == 0) h++;

				if (saveAnimation)
					generator.enableHistory();

				bool[,] lab = generator.generate(w, h);
				Console.WriteLine(generator.getdeadends() + " dead ends.");
				solveit = saveMazeImage(lab, filename + ".png");
				//saveMazeCheat((Image)solveit.Clone(), generator.solution(), filename + "-cheat.png");

				if (saveAnimation) {
					List<bool[,]> ht = generator.getHistory();
					ProcessManager process = new ProcessManager("Saving Animation");
					for (int i = 0; i < ht.Count; i++) {
						saveMazeImage(ht[i], "mazeanimation\\" + filename + "-" + i + ".png");
						process.progress(i, ht.Count);
					}
					process.done();
				}

				Console.Write("Solve it? [y/n] ");
				cmd = Console.ReadLine();
				if (cmd != "y") {
					return;
				}

				Console.WriteLine("--- Deleting memory, and solving the maze from zero ---");

				lab = null;
				generator = null;
			} else if (cmd == "l") {
				Console.Write("Image: ");
				cmd = Console.ReadLine();

				filename = cmd.Split('.')[0];

				try {
					solveit = Image.FromFile(cmd);
				} catch (Exception e) {
					Console.WriteLine("Exception: " + e.Message);
					return;
				}
			} else if (cmd == "c") {
				double w1, w2;
				try {
					w1 = int.Parse(Console.ReadLine());
					w2 = int.Parse(Console.ReadLine());
				} catch (Exception e) {
					Console.WriteLine(e.Message);
					return;
				}
				Console.WriteLine(string.Format("w12/w22={0:0.0000} w22/w12={1:0.0000}", (w1*w1)/(w2*w2), (w2*w2)/(w1*w1)));
				Console.ReadLine();
				return;
			} else {
				return;
			}

			Console.WriteLine("Choose the solver:");
			Console.WriteLine("0 - A*");
			Console.Write("#");
			cmd = Console.ReadLine();

			MazeSolver solver;
			switch (cmd) {
				case "0":
					solver = new AStar(imageToMaze(solveit));
					break;
				default:
					return;
			}

			Console.Write("Generate graph map? [y/n] ");
			cmd = Console.ReadLine();
			if (cmd == "y") {
				savePathGraph((Image)solveit.Clone(), solver.getGraphPoints(), filename + "-pathgraph.png");
			}

			if (saveAnimation) {
				solver.enableHistory();
			}
            solver.Point[] sol = solver.solve(0, solver.getLength() - 1);
			if (saveAnimation) {
				ProcessManager process = new ProcessManager("Saving Animation");
				List<solver.Point[]> history = solver.getHistory();
				for (int i = 0; i < history.Count; i++) {
					saveMazeSolutinImage((Image)solveit.Clone(), history[i], "animation\\" + filename + "-" + i + ".png");
					process.progress(i, history.Count);
				}
				process.done();
			}

			if (sol != null) {
				Console.WriteLine("Solution saved!");
				saveMazeSolutinImage((Image)solveit.Clone(), sol, filename + "-solution.png");
			} else {
				Console.WriteLine("No solution!");
				Console.ReadLine();
			}
		}

		public static Image saveMazeImage(bool[,] maze, string filename) {
			Image img = new Bitmap(maze.GetLength(0), maze.GetLength(1), PixelFormat.Format24bppRgb);
			Bitmap bmp = (Bitmap)img;

			ProcessManager process = new ProcessManager("Saving Image");

			for (int x = 0; x < maze.GetLength(0); x++) {
				for (int y = 0; y < maze.GetLength(1); y++) {
					if (maze[x, y])
						bmp.SetPixel(x, y, Color.White);
					else
						bmp.SetPixel(x, y, Color.Black);
				}
				process.progress(x, maze.GetLength(0));
			}

			img.Save(filename);

			process.done();

			return img;
		}

		public static bool[,] imageToMaze(Image img) {
			Bitmap bmp = (Bitmap)img;
			bool[,] maze = new bool[bmp.Width, bmp.Height];

			ProcessManager process = new ProcessManager("Loading Image");

			for (int x = 0; x < bmp.Width; x++) {
				for (int y = 0; y < bmp.Height; y++) {
					double brg = bmp.GetPixel(x, y).GetBrightness();
					maze[x, y] = brg > 0.5;
					//maze[x, y] = bmp.GetPixel(x, y).R != 0;
				}
				process.progress(x, bmp.Width);
			}

			process.done();

			return maze;
		}

		public static void saveMazeSolutinImage(bool[,] maze, solver.Point[] path, string filename) {
			Image img = new Bitmap(maze.GetLength(0), maze.GetLength(1), PixelFormat.Format24bppRgb);
			Bitmap bmp = (Bitmap)img;

			for (int x = 0; x < maze.GetLength(0); x++) {
				for (int y = 0; y < maze.GetLength(1); y++) {
					if (maze[x, y])
						bmp.SetPixel(x, y, Color.White);
					else
						bmp.SetPixel(x, y, Color.Black);
				}
			}

			saveMazeSolutinImage(img, path, filename);
		}

		public static void saveMazeSolutinImage(Image img, solver.Point[] path, string filename) {
			Bitmap bmp = (Bitmap)img;

			int x = -1, y = -1;

			double r = 0;

			for (int i = 0; i < path.Length; i++) {

				r = ((double)i / (double)path.Length) * 255;

				if (x != -1 && y != -1) {
					while (x != path[i].getX() || y != path[i].getY()) {
						x += Math.Sign(path[i].getX() - x);
						y += Math.Sign(path[i].getY() - y);
						bmp.SetPixel(x, y, Color.FromArgb(255, (int)r, 0, 255 - (int)r));
					}
				}
				x = path[i].getX();
				y = path[i].getY();
				bmp.SetPixel(x, y, Color.FromArgb(255, (int)r, 0, 255 - (int)r));
			}

			img.Save(filename);
		}

		public static void savePathGraph(Image img, List<solver.Point> graphpoints, string filename) {
			Bitmap bmp = (Bitmap)img;

			ProcessManager process = new ProcessManager("Saving Graph Map");

			for (int i = 0; i < graphpoints.Count; i++) {
				bmp.SetPixel(graphpoints[i].getX(), graphpoints[i].getY(), Color.Green);
				process.progress(i, graphpoints.Count);
			}

			process.done();

			img.Save(filename);
		}
	}
}
