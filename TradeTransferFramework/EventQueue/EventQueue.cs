using System;
using System.Collections;
using System.Threading;
using log4net;

namespace EventQueue
{
	public delegate void EventQueueDelegate(object item); 

	public class EventQueue
	{
		private static ILog Log = LogManager.GetLogger(typeof(EventQueue));

		protected Queue TheQueue;
		protected Thread DispatcherThread;
		protected EventQueueDelegate EventQueueDelegate;
		protected bool Running;
		protected AutoResetEvent QueueEvent;
		protected AutoResetEvent StopEvent;

		private String _threadName;

		public EventQueue (string threadName, EventQueueDelegate eventQueueDelegate)
		{
			EventQueueDelegate = eventQueueDelegate;
			Init(threadName);
		}

		public EventQueue (string threadName)
		{
			Init(threadName);
		}


		~EventQueue(){
			Stop();
			Clear();
		}

		private void Init(string threadName) {
			try {
				_threadName = threadName;
				QueueEvent = new AutoResetEvent(false);
				StopEvent = new AutoResetEvent(false);
				TheQueue = new Queue(100, 10);
			}
			catch(Exception e){
				Log.ErrorFormat("Exception initialising event queue {0}", threadName, e);
			}
		}

		protected virtual void OnThreadInitialize() {

		}

		protected virtual void OnThreadFinalize() {
			
		}

		protected virtual void OnThreadProcessItem(object item) {
			if (EventQueueDelegate != null) {
				EventQueueDelegate(item);
			}
		}

		public void Start() {
			try {
				StopEvent.Reset();

				DispatcherThread = new Thread(ThreadDispacthItems) { Name = _threadName };

				Running = true;
				DispatcherThread.Start();
			}
			catch (Exception e){
				Log.Error ("Exception starting event queue", e);
				throw;
			}
		}

		public void Stop() {
			try {
				Running = false;

				StopEvent.Set();
				

				try {
					DispatcherThread.Join (1000);

				} catch (Exception e) {
					Log.Error("Error when stopping event queue", e);
				}
			}
			catch (Exception e){
				Log.Error("Exception starting event queue", e);
				throw;
			}
		}

		public void Add(object item) {
			try {
				lock(TheQueue) {
					TheQueue.Enqueue(item);
					QueueEvent.Set();
				}
			} catch (Exception e) {
				Log.Error ("Error adding item from the queue", e);
			}
		}

		public object Remove(object item) {
			try {
				lock(TheQueue) {
					return (TheQueue.Count > 0)? TheQueue.Dequeue():null;
				}
			} catch (Exception e) {
				Log.Error ("Error removing item from the queue", e);
				return null;
			}
		}

		public int Count() {
			try {
				lock(TheQueue) {
					return TheQueue.Count;
				}
			} catch (Exception e) {
				Log.Error ("Error getting queue length", e);
				return 0;
			}
		}

		public void Clear() {
			try {
				lock(TheQueue) {
					TheQueue.Clear();
				}
			} catch (Exception e) {
				Log.Error ("Error clearing the queue", e);
			}
		}

		private void ThreadDispacthItems() {
			try {
			OnThreadFinalize();

			try {
				var handles = new WaitHandle[2];
				handles[0] = StopEvent;
				handles[1] = QueueEvent;

				Log.InfoFormat("Started thread {0}", DispatcherThread.Name);

				while (Running) {
					int result = WaitHandle.WaitAny(handles);
					switch (result) {
						case 0:
						Log.InfoFormat("Stopping thread {0}", DispatcherThread.Name);
							break;
						case 1:
							{
							bool moreItems = true;
							while (moreItems) {
								object item = null;
								//try {} catch (Exception e) {}
								try {
									lock (TheQueue) {
										moreItems = TheQueue.Count > 0;
										if (moreItems)
											item = TheQueue.Dequeue();
									}
									if (item != null) {
										OnThreadProcessItem(item);
									}

								} catch (Exception e) {
									Log.InfoFormat("An error occurred dispacthing queue event {0}", e);
								}
							}
						}
						break;
					default:
						Log.InfoFormat("Unexpected wait result, not stop or queue {0}", result);
						break;
					}
				}
				Log.InfoFormat("Stopped thread {0}", DispatcherThread.Name);
			}
			finally {
				OnThreadFinalize();
			}
			} catch (Exception e) {
				Log.InfoFormat("Exception when dispatching event queue item", e);	
			}
		}
	}
}

