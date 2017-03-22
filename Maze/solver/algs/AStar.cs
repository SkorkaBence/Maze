using CSFProcessManager;
using System.Collections.Generic;

namespace Maze.solver.algs {
    public class AStar : MazeSolver {

        public AStar(bool[,] maze) : base(maze) { }

        public override Point[] solve(int fromid, int toid) {
            List<Distance> distances = new List<Distance>();
            bool[] processed = new bool[mazeGraph.Count];

            distances.Add(new Distance(fromid, mazeGraph[fromid].getX(), mazeGraph[fromid].getY()));

            int pro = 0;
            ProcessManager process = new ProcessManager("A*");

            while (true) {
                int next = 0;
                while (next < distances.Count && processed[distances[next].id]) {
                    next++;
                }
                if (next >= distances.Count) {
                    process.done();
                    return null;
                }

                List<Point> connections = mazeGraph[distances[next].id].getConnections();
                //List<int> length = mazeGraph[distances[next].id].getLengths();

                for (int i = 0; i < connections.Count; i++) {
                    int calcid = connections[i].getId();
                    if (!processed[calcid]) {
                        int did = -1;
                        for (int j = 0; j < distances.Count; j++) {
                            if (distances[j].id == calcid) {
                                did = j;
                                break;
                            }
                        }

                        int calcdis = distances[next].distance + mazeGraph[distances[next].id].getLengths()[i];

                        if (did == -1) {
                            // add new
                            distances.Add(new Distance(calcid, mazeGraph[calcid].getX(), mazeGraph[calcid].getY()));
                            did = distances.Count - 1;
                            distances[did].distance = calcdis;
                            distances[did].path = new List<Point>(distances[next].path);
                            distances[did].path.Add(mazeGraph[distances[next].id]);
                        } else if (distances[did].distance > calcdis) {
                            distances[did].distance = calcdis;
                            distances[did].path = new List<Point>(distances[next].path);
                            distances[did].path.Add(mazeGraph[distances[next].id]);
                        }

                        if (saveHistory) {
                            history.Add(distances[did].path.ToArray());
                        }

                        if (distances[did].id == toid) {
                            distances[did].path.Add(mazeGraph[distances[did].id]);
                            process.done();
                            return distances[did].path.ToArray();
                        }
                    }
                }

                processed[distances[next].id] = true;
                distances.RemoveAt(next);
                pro++;

                distances.Sort((a, b) => { return a.distance.CompareTo(b.distance); });

                process.progress(pro, processed.Length);
            }
        }

    }
}
