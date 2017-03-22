using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSFProcessManager {
	public class ProcessManager {
		private bool enableConsole = true;
		private string processName { get; }
		private string precision = "0";
		private bool isfinished = false;
		private Stopwatch clock { get; } = new Stopwatch();
		private bool noFeedback = false;
		private long lastupdate = 0;
		private long fedbackdifference = 100;
		private int top { get; }
		private double lremain;
		private double remain;
		private double percent;

		public ProcessManager(string pn) : this(pn, 2, false) { }

		public ProcessManager(string pn, bool noConsole) : this(pn, 2, noConsole) { }

		public ProcessManager(string pn, int precision, bool noConsole) {
			enableConsole = !noConsole;
			processName = pn;
			if (enableConsole) {
				Console.WriteLine(processName + " starting...");
				top = Console.CursorTop - 1;
			}
			this.precision = "0";
			if (precision > 0) {
				this.precision += ".";
				for (int i = 0; i < precision; i++) {
					this.precision += "0";
				}
			}
			clock.Start();
		}

		public void progress(int now, int all) {
			if (noFeedback) return;
			if (clock.ElapsedMilliseconds - lastupdate <= fedbackdifference) return;
			Task.Run(() => processfunc((double)now, (double)all));
		}

		public void progress(double now, double all) {
			if (noFeedback) return;
			if (clock.ElapsedMilliseconds - lastupdate <= fedbackdifference) return;
			Task.Run(() => processfunc(now, all));
		}

		private void processfunc(double now, double all) {
			lastupdate = clock.ElapsedMilliseconds;
			if (isfinished) throw new Exception("Process already finsihed!");
			if (all > 0) {
				percent = now / all;
			} else {
				percent = now / 100;
			}
			remain = (clock.ElapsedMilliseconds / percent) * (1 - percent) / 1000;
			percent = percent * 100;
			if (enableConsole) {
				string prt;
				if (remain <= lremain) {
					prt = string.Format("{0} {1:" + precision + "}% (Remaining time: {2:0.0} s)...          ", processName, percent, remain);
				} else {
					prt = string.Format("{0} {1:" + precision + "}% ...                                     ", processName, percent);
				}
				Console.SetCursorPosition(0, top);
				if (Console.CursorTop == top) {
					Console.WriteLine(prt);
				}
			}
			lremain = remain;
		}

		public void done() {
			clock.Stop();
			isfinished = true;
			if (!enableConsole) {
				return;
			}
			Console.SetCursorPosition(0, top);
			Console.WriteLine(processName + " finished in " + (double)clock.ElapsedMilliseconds / 1000 + " s                                        ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.WriteLine("                                                                                                                   ");
			Console.SetCursorPosition(0, top + 1);
		}

		public double getRemainingTime() {
			return remain;
		}

		public double getPercentage() {
			return percent;
		}

		public string timeReadable() {
			double lft = remain;

			int h = (int) (lft / 60 / 60);
			lft -= h * 60 * 60;

			int m = (int)(lft / 60);
			lft -= m * 60;

			int s = (int)(lft);
			lft -= s;

			return string.Format("{0:0}h{1:00}m{2:00}s", h, m, s);
		}
	}
}
