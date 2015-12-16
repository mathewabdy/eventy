using System;
using System.Threading;
using log4net;
using EventQueue;

namespace TradeTransfer
{
	public class TradeTransferAdapter : ITradeTranferAdapter
	{
		private static ILog Log = LogManager.GetLogger (typeof(TradeTransferAdapter));
		/// <summary>
		/// The _transfer queue.
		/// </summary>
		public EventQueue.EventQueue _transferQueue;
		private DataLoader _dataLoader;
		/// <summary>
		/// Initializes a new instance of the <see cref="TradeTransfer.TradeTransferAdapter"/> class.
		/// </summary>
		public TradeTransferAdapter ()
		{
			_transferQueue = new EventQueue.EventQueue ("TradeTransferQueueThread", DoTransfer);
			_dataLoader = new DataLoader();
		}

		public bool IsRunning {get; set;}

		private void DoTransfer (object item)
		{
			if(item is Trade) {
				Trade trade = item as Trade;
				// check trade is for an active pair
				if (_dataLoader.ValidAccount(trade.Account)) {
					Log.InfoFormat ("Issuing the transfer request for this trade {0} account {1}", trade.TradePrice, trade.Account);
				} else {
					Log.InfoFormat("Account is not valid or enabled so not processing for account {0}", trade.Account);
				}
				// check there are from and to orders in the cache


			} else {
				Log.InfoFormat("item was not a trade {0}", item.GetType());
			}
		}

		private Thread TransferThread;
		/// <summary>
		/// Run this instance.
		/// </summary>
		public void Run()
		{
			TransferThread = new Thread (InitiateTradeTransfer) {Name = "TradeTransferMainThread"};
			TransferThread.Start ();
			IsRunning = true;
		}

		public void Stop() {
			IsRunning = false;
		}

		private void Wait (int seconds)
		{
			Thread.Sleep (seconds * 1000);
		}

		private void OnTrade (object obj, TradeEventArgs trade)
		{
			Log.DebugFormat("Adding trade to queue... {0}", trade.Trade.TradeType);
			_transferQueue.Add (trade.Trade);
			Log.DebugFormat("Current queue count ... {0}", _transferQueue.Count());
		}

		private void InitiateTradeTransfer (object trade)
		{
			DateTime today = DateTime.Today;
			DateTime end = new DateTime(today.Year, today.Month, today.Day, 23, 59, 0); 
			Log.InfoFormat ("This is where we setup the webservices stuff");
			// Initiate the IOS Plus adapter
			IosPlusAdapter adapter = new IosPlusAdapter();
			adapter.TradeArrived += OnTrade;
			adapter.Start();

			// Start the transfer queue work
			_transferQueue.Start();

			while (DateTime.Now.TimeOfDay < end.TimeOfDay && IsRunning) {
				Wait(1);
			}

			IsRunning = false;

			_transferQueue.Stop();


		}
	}
}