using System.Threading;

namespace OOP_Laba15 {
	class Channel {
		public Semaphore Semaphore { get; private set; }
		public Client Client { get; private set; }

		public Channel() {
			Semaphore = new Semaphore(1, 1);
		}

		public void Reserve(Client client) => Client = client;

		public void Free() => Client = null;
	}
}
