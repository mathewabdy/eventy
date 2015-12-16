using System;
using log4net;
using NUnit.Framework;

namespace EventQueue
{
	[TestFixture()]
	public class EventQueueTest
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(EventQueueTest));
		EventQueue queue;

		[Test()]
		public void TestCase ()
		{
			if (queue != null) {
				string text = "This text goes on the queue";
				queue.Add(text);
			}
		}

		[SetUp()]
		public void Setup()
		{
			queue = new EventQueue("TestQueue", ProcessEventQueueAction);
		}

		private void ProcessEventQueueAction(object queueObject) {
			Log.DebugFormat ("Processing event queue item forever {0}", queueObject.GetType());
		}
	}
}

