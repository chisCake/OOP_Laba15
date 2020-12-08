using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OOP_Laba15 {
	class Storage : IDisposable {
		Thread thread;

		public string File { get; private set; }
		public int ProductsAmt { get; private set; }
		public Queue<Car> Queue { get; private set; }
		public List<Car> Cars { get; private set; }
		public bool Loader { get; private set; }

		public Storage(string file, int productsAmt, IEnumerable<Car> cars) {
			File = file;
			ProductsAmt = productsAmt;
			Cars = (List<Car>)cars;

			Queue = new Queue<Car>();

			using var sw = new StreamWriter(File);
			Console.WriteLine("Содержимое склада:");
			for (int i = 0; i < productsAmt; i++) {
				string data = $"{i + 1}) {Math.Pow((i + 1) * DateTime.Now.Millisecond, Math.PI)}";
				sw.WriteLine(data);
				Console.WriteLine(data);
			}
		}

		public void StartUnloading() {
			Cars.ForEach(car => car.StartUnloading(File, this));
			thread = new Thread(Holder);
			thread.Start();
			thread.Join();
		}

		private void Holder() {
			while (ProductsAmt != 0) {
				foreach (var car in Cars)
					if (!Queue.Contains(car))
						Queue.Enqueue(car);

				if (Queue.Count != 0) {
					var car = Queue.Peek();
					if (car != null && Loader == false) {
						Console.Write("Текущая очередь: ");
						foreach (var item in Queue)
							Console.Write(item.Name + " / ");
						car.Turn();
						ProductsAmt--;
						Loader = true;
					}
				}
			}
			while (Loader)
				;
			Cars.ForEach(car => car.FinishRequest());
		}

		public bool IsRegistered(Car car) => Queue.Contains(car);

		public void Register(Car car) {
			Console.WriteLine($"{car.Name} register request");
			if (IsRegistered(car)) {
				Console.WriteLine("Deny");
				return;
			}
			Console.WriteLine("Ok");
			Queue.Enqueue(car);
		}

		public void Next(Car car) {
			if (Queue.Peek() != car)
				Console.WriteLine("!");
			Queue.Dequeue();
			Loader = false;
		}

		public void Dispose() {
			new FileInfo(File).Delete();
		}
	}
}
