using System;
using log4net;
using System.Threading;

namespace TradeTransferFramework
{
	class MainClass
	{
		private static ILog Log = LogManager.GetLogger(typeof(MainClass));
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			log4net.Config.XmlConfigurator.Configure();
			Log.InfoFormat("Starting Trade Transfer");
			TradeTransfer.TradeTransferAdapter adapter = new TradeTransfer.TradeTransferAdapter();
			adapter.Run();

			while (adapter.IsRunning) {
				Thread.Sleep(1000);
				if (System.Console.KeyAvailable) {
					if (ManualShutDown()) {
						adapter.Stop();
					}
				}
			}
			adapter.Stop();
		}

		private static bool ManualShutDown ()
		{
			var keyInfo = System.Console.ReadKey(true);
			return keyInfo.Key == ConsoleKey.C && (keyInfo.Modifiers & ConsoleModifiers.Control) != 0;
		}
	}
}
