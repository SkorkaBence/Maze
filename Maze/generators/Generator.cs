using System;
using System.Collections.Generic;

namespace Maze.generators {
	public abstract class Generator {

		protected bool saveHistory = false;
		protected List<bool[,]> mazeHistory = new List<bool[,]>();
		protected int deadends = 0;
		protected Random rand = new Random();

		public abstract bool[,] generate(int width, int height);

		public int getdeadends() {
			return deadends;
		}

		public void enableHistory() {
			saveHistory = true;
		}

		public List<bool[,]> getHistory() {
			return mazeHistory;
		}

	}
}
