using System;
using System.IO;
using System.Threading;

namespace OOP_Laba15 {
	class Counter {
		Thread oddThread;
		Thread evenThread;
		int oddThreadNum;
		int evenThreadNum;
		bool firstTurn;
		StreamWriter sw;

		public CounterState State { get; private set; }
		public CounterMode Mode { get; private set; }
		public int Number { get; private set; }
		public string File { get; private set; }
		public int OddThreadSpeed { get; private set; }
		public int EvenThreadSpeed { get; private set; }

		public Counter(int number, CounterMode mode, string file, int speed1 = 250, int speed2 = 500) {
			State = CounterState.Ready;
			Mode = mode;
			Number = number;
			File = file;
			sw = new StreamWriter(File);
			OddThreadSpeed = speed1;
			EvenThreadSpeed = speed2;

			oddThread = new Thread(new ThreadStart(Odd));
			evenThread = new Thread(new ThreadStart(Even));
			oddThreadNum = 1;
			evenThreadNum = 2;
			firstTurn = true;
		}

		public void Start() {
			State = CounterState.Working;
			oddThread.Start();
			evenThread.Start();

			oddThread.Join();
			evenThread.Join();
			State = CounterState.Completed;
			Console.WriteLine("Счётчик завершил свою работу");
		}

		// Сброс счётчика
		public void Reset() {
			oddThread = new Thread(Odd);
			evenThread = new Thread(Even);
			oddThreadNum = 1;
			evenThreadNum = 2;
			firstTurn = true;
			State = CounterState.Ready;
		}

		// Обработчик нечётного потока
		void Odd() {
			while (oddThreadNum <= Number) {
				if (Mode == CounterMode.AtOnce) {
					Output();
				}
				else {
					while (!firstTurn)
						;
					Output();
					firstTurn = false;
				}
				oddThreadNum += 2;
			}

			void Output() {
				Thread.Sleep(OddThreadSpeed);
				Console.WriteLine("Нечётный поток: " + oddThreadNum);
				sw.WriteLine("Нечётный поток: " + oddThreadNum);
				sw.Flush();
			}
		}

		// Обработчик чётного потока
		void Even() {
			if (Mode == CounterMode.AtOnce)
				oddThread.Join();

			while (evenThreadNum <= Number) {
				if (Mode == CounterMode.AtOnce) {
					Output();
				}
				else {
					while (firstTurn)
						;
					Output();
					firstTurn = true;
				}
				evenThreadNum += 2;
			}

			void Output() {
				Thread.Sleep(EvenThreadSpeed);
				Console.WriteLine("Чётный поток:   " + evenThreadNum);
				sw.WriteLine("Чётный поток: " + evenThreadNum);
				sw.Flush();
			}
		}


		// Изменения параметров счётчика
		public void SetPriority(CounterThread thread, ThreadPriority priority) {
			if (thread == CounterThread.Odd)
				oddThread.Priority = priority;
			else
				evenThread.Priority = priority;
		}

		public void SetFile(string file) {
			if (State == CounterState.Working)
				throw new Exception("Нельзя изменить файл во время работы счётчика");
			File = file;
			sw = new StreamWriter(File);
		}

		public void SetMode(CounterMode mode) {
			if (State == CounterState.Working)
				throw new Exception("Нельзя изменить способ подсчёта во время работы счётчика");
			Mode = mode;
		}

		public void SetNumber(int number) {
			if (State == CounterState.Working)
				throw new Exception("Нельзя изменить число во время работы счётчика");
			Number = number;
		}

		public void SetSpeed(CounterThread thread, int speed) {
			if (thread == CounterThread.Odd)
				OddThreadSpeed = speed;
			else
				EvenThreadSpeed = speed;
		}
	}

	enum CounterThread {
		Odd,
		Even
	}

	enum CounterState {
		Ready,
		Working,
		Completed
	}

	enum CounterMode {
		AtOnce,
		InTurn
	}
}
