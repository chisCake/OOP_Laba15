using System.Threading;

namespace OOP_Laba15 {
	class Client {
		Thread thread;

		public ChannelManager Manager { get; private set; }

		public string Name { get; }
		public int WaitTime { get; private set; }
		public int ChannelN { get; private set; }
		public Channel Channel { get; private set; }
		public int ViewingTime { get; private set; }

		public Client(string name, int waitTime, int channelN, int viewingTime, ChannelManager manager) {
			Name = name;
			WaitTime = waitTime;
			ChannelN = channelN;
			ViewingTime = viewingTime;
			Manager = manager;
			Channel = manager.Channels[channelN];
			thread = new Thread(Run);
			thread.Start();
		}

		private void Run() {
			bool nice = false;
			if (Channel.Semaphore.WaitOne(WaitTime)) {
				Channel.Reserve(this);
				Thread.Sleep(ViewingTime);
				Channel.Free();
				Channel.Semaphore.Release();
				nice = true;
			}
			Manager.Leave(this, nice);
		}

		public override string ToString() {
			return Name;
		}
	}
}
