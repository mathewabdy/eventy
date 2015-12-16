using System;

namespace TradeTransfer
{
	public class Trade
	{
		public string 	Symbol 		{get; set;}
		public string 	XRef 		{get; set;}
		public long 	TradeVolume {get; set;}
		public double 	TradePrice 	{get; set;}
		public string 	TradeType 	{get; set;}
		public string 	Account 	{get; set;}
	}
}

