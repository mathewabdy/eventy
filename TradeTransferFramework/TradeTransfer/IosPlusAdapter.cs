using System;
using log4net;
using System.Threading;
namespace TradeTransfer
{

	/// <summary>
	/// Ios plus adapter.
	/// This is where we connect to ios plus and subscribe for trades from webservices
	/// </summary>
	public class IosPlusAdapter
	{
		private static ILog Log = LogManager.GetLogger(typeof(IosPlusAdapter));
		public IosPlusAdapter()
		{
		}

		private Thread iosPlusAdapterThread;

		public event EventHandler<TradeEventArgs> TradeArrived;

		private bool IsRunning;

		public void Start(){
			Log.InfoFormat("Starting the IOS Plus adapter...");
			iosPlusAdapterThread = new Thread (InitialiseWebServiceQueues) {Name = "IosPlusAdapterThread"};
			iosPlusAdapterThread.Start ();
			Thread secondThread = new Thread (SecondQueuePopulatingThread) {Name = "IosPlusAdapterThread"};
			secondThread.Start();
			IsRunning = true;
		}

		public void Stop() {
			IsRunning = false;
		}
		private void InitialiseWebServiceQueues ()
		{
			int i = 1;

			while (IsRunning) {
				int account = Random.Next(1, 100);
				TradeEventArgs args = new TradeEventArgs();
				args.Trade = new Trade();
				args.Trade.Symbol = "NAB";
				args.Trade.TradePrice = i;
				args.Trade.TradeType = "FirstThread";
				args.Trade.Account = "to" + account;
				if (TradeArrived != null) {
					TradeArrived(this, args);
					i++;
				}

				Thread.Sleep(1000);
			}
			//Start the webservices details here
			//for the purpose of this we should just add some queues and have them populated with some trade events
			// until such time as its connected to webservices.
		}

		private void SecondQueuePopulatingThread ()
		{

			int i = 10;
			while (IsRunning) {
				int account = Random.Next(1, 100);
				TradeEventArgs args = new TradeEventArgs();
				args.Trade = new Trade();
				args.Trade.Symbol = "NAB";
				args.Trade.TradePrice = i;
				args.Trade.TradeType = "SecondThread";
				args.Trade.Account = "from" + account;
				if (TradeArrived != null) {
					TradeArrived(this, args);
					i++;
				}
				
				Thread.Sleep(500);
			}
		}

		private static Random Random = new Random();
	}
}

