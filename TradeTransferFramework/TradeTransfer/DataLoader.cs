using System;
using System.Collections.Generic;

namespace TradeTransfer
{
	public class DataLoader
	{
		public List<Pair> accountPairs = new List<Pair>();
		public Dictionary<long, TransferredTrade> transferredTrades = new Dictionary<long, TransferredTrade>();
		public List<String> symbols = new List<String> {"NAB","BHP","LLC","MQG","CBA","WOW","ABC","RIO","QBE","ORI"};

		public DataLoader ()
		{
			bool enabled = false;
			for (int i = 0; i < 100; i++)
			{
				Pair pair = new Pair {FromAccount = "from" + i, ToAccount = "to" + i, Enabled = !enabled};
				accountPairs.Add(pair);
			}

			for (int i = 0; i < 1000; i++)
			{
				int r = Random.Next(accountPairs.Count);
				int symbol = Random.Next(symbols.Count);

				TransferredTrade transferredTrade = new TransferredTrade {
					Symbol = symbols[symbol], FromAccount = accountPairs[r].FromAccount, ToAccount = accountPairs[r].ToAccount,
					Price = i, Quantity = r * symbol, Posted = !enabled, Cancelled = enabled, TradeSlipNumber = i};
				transferredTrades.Add(transferredTrade.TradeSlipNumber, transferredTrade);
			}
		}

		public bool ValidAccount(string account) {
			return accountPairs.Exists(x=>x.FromAccount == account && x.Enabled == true); 
		}

		private static Random Random = new Random();

	}
	#region Pair
	public class Pair {
		/// <summary>
		/// Gets or sets from account.
		/// </summary>
		/// <value>From account.</value>
		public string 	FromAccount	{get; set;}
		/// <summary>
		/// Gets or sets to account.
		/// </summary>
		/// <value>To account.</value>
		public string 	ToAccount	{get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TradeTransfer.Pair"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool 	Enabled 	{get; set;}
	}
	#endregion Pair

	public class TransferredTrade {
		/// <summary>
		/// Gets or sets the symbol.
		/// </summary>
		/// <value>The symbol.</value>
		public string 	Symbol 			{get; set;}
		/// <summary>
		/// Gets or sets the account.
		/// </summary>
		/// <value>The account.</value>
		public string 	FromAccount 	{get; set;}		
		/// <summary>
		/// Gets or sets to account.
		/// </summary>
		/// <value>To account.</value>
		public string 	ToAccount 	{get; set;}		
		/// <summary>
		/// Gets or sets the xref.
		/// </summary>
		/// <value>The xref.</value>
		public string 	Xref 			{get; set;}		
		/// <summary>
		/// Gets or sets the quantity.
		/// </summary>
		/// <value>The quantity.</value>
		public long 	Quantity 		{get; set;}		
		/// <summary>
		/// Gets or sets the price.
		/// </summary>
		/// <value>The price.</value>
		public double 	Price 			{get; set;}		
		/// <summary>
		/// Gets or sets the trade value.
		/// </summary>
		/// <value>The trade value.</value>
		public double 	TradeValue 		{get; set;}		
		/// <summary>
		/// Gets or sets the trade slip number.
		/// </summary>
		/// <value>The trade slip number.</value>
		public int  	TradeSlipNumber	{get; set;}		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TradeTransfer.TransferredTrade"/> is posted.
		/// </summary>
		/// <value><c>true</c> if posted; otherwise, <c>false</c>.</value>
		public bool 	Posted 			{get; set;}		
		/// <summary>
		/// Gets or sets a value indicating whether this instance cancelled.
		/// </summary>
		/// <value><c>true</c> if this instance cancelled; otherwise, <c>false</c>.</value>
		public bool 	Cancelled 		{get; set;}
	}
}

