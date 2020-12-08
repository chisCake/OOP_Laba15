using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OOP_Laba15 {
	class Car {
		Thread thread;
		bool myTurn;

		public string Name { get; private set; }
		public List<string> Products { get; private set; }
		public int Speed { get; private set; }

		public Storage Storage { get; private set; }
		public bool IsRegistered { get; private set; }
		public bool FinishUnloading { get; private set; }
		public string File { get; private set; }

		public Car(string name, int speed) {
			Name = name;
			Speed = speed;

			Products = new List<string>();
		}

		public void StartUnloading(string file, Storage storage) {
			File = file;
			Storage = storage;

			IsRegistered = false;
			FinishUnloading = false;
			myTurn = false;

			thread = new Thread(Holder);
			thread.Start();
		}

		public void Turn() => myTurn = true;

		public void FinishRequest() => FinishUnloading = true;

		private void Holder() {
			while (!FinishUnloading) {
				if (myTurn) {
					Console.WriteLine($"\n{Name} загружает товар");
					Thread.Sleep(Speed);
					string line;
					var items = new List<string>();
					using (var sr = new StreamReader(File)) {
						while ((line = sr.ReadLine()) != null)
							items.Add(line);
					}
					Products.Add(items[0]);
					items.RemoveAt(0);
					using var sw = new StreamWriter(File);
					foreach (var item in items)
						sw.WriteLine(item);
					Storage.Next(this);
					myTurn = false;
				}
			}
		}
	}
}
