using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OOP_Laba15 {
	class ChannelManager {
		public int ChannelsAmt { get; private set; }
		public int ClientsAmt { get; private set; }

		public bool Running { get; private set; }

		public List<Channel> Channels { get; private set; }
		public List<Client> Clients { get; private set; }

		static readonly Random rndm = new Random();
		static readonly List<string> names;

		public ChannelManager(int channelsAmt, int clientsAmt) {
			ChannelsAmt = channelsAmt;
			ClientsAmt = clientsAmt;

			Channels = new List<Channel>();
			Clients = new List<Client>();

			departed = new List<(Client, bool)>();

			for (int i = 0; i < channelsAmt; i++) {
				var channel = new Channel();
				Channels.Add(channel);
			}
		}

		static ChannelManager() {
			using var sr = new StreamReader("names.txt");
			names = new List<string>();
			string line;
			while ((line = sr.ReadLine()) != null)
				names.Add(line);
		}

		int GetRndmChannel() => rndm.Next(0, Channels.Count);

		public void Start() {
			Running = true;
			while (Running) {
				Console.Clear();
				for (int i = Clients.Count; i < ClientsAmt; i++)
					Clients.Add(new Client(
							names[rndm.Next(0, names.Count)],
							rndm.Next(1000, 10000),
							GetRndmChannel(),
							rndm.Next(1000, 10000),
							this));

				// Вывод каналов
				for (int i = 0; i < ChannelsAmt; i++) {
					string str = $"Канал {i + 1}";
					Console.Write($"{str,-15}");
				}

				Console.WriteLine();
				// Вывод текущих пользователей
				for (int i = 0; i < ChannelsAmt; i++) {
					string str = Channels[i].Client == null ? "Нет клиента" : $"{Channels[i].Client}";
					Console.Write($"{str,-15}");
				}

				Console.WriteLine();
				// Вывод ожидающих пользователей
				for (int i = 0; i < ChannelsAmt; i++) {
					Console.WriteLine($"\nПользователи ожидающие подключение к каналу {i + 1}");
					foreach (var item in Clients) {
						if (item.ChannelN == i && Channels[i].Client != item)
							Console.Write(item + " ");
					}
				}

				Console.WriteLine();
				// Вывод ушедших пользователей
				Console.WriteLine("\nУшедшие пользователи");
				foreach (var item in departed) {
					string res = item.Item2 == false ? "НЕ " : "";
					Console.WriteLine($"{item.Item1} ушёл {res}получив услугу");
				}
				departed.Clear();
				Thread.Sleep(1000);
			}
		}

		List<(Client, bool)> departed = new List<(Client, bool)>();
		public void Leave(Client client, bool state) {
			departed.Add((client, state));
			Clients.Remove(client);
		}

		public void Stop() => Running = false;
	}
}
