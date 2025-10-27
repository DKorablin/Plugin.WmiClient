using System;
using System.Management;
using System.Threading;
using Plugin.WmiClient.Events;

namespace Plugin.WmiClient.Dal
{
	internal class WmiObserver : IDisposable
	{
		private ManagementOperationObserver _observer;

		public event EventHandler<ObjectReadyEventArgs> OnObjectReady;
		public event EventHandler<FinishedLoadingEventArgs> OnCompleted;

		/// <summary>Observer is cancelled and pending disposing</summary>
		public Boolean IsCancelled { get; private set; }

		/// <summary>Observer is in progress receiving new events</summary>
		public Boolean InProgress { get; private set; }

		public void Cancel()
		{
			this.IsCancelled = true;
			this.RemoveObserver();
		}

		public void Dispose()
			=> this.RemoveObserver();

		public ManagementOperationObserver GetObserver()
		{
			if(this._observer != null)
				throw new InvalidOperationException("Observer not disposed after last use");

			this.CreateObserver();
			return this._observer;
		}

		private void CreateObserver()
		{
			this._observer = new ManagementOperationObserver();
			this._observer.Completed += this.Observer_Completed;
			this._observer.ObjectReady += this.Observer_ObjectReady;
			this.IsCancelled = false;
			this.InProgress = true;
		}

		private void RemoveObserver()
		{
			ManagementOperationObserver observer = Interlocked.Exchange(ref this._observer, null);
			if(observer != null)
			{
				observer.Completed -= this.Observer_Completed;
				observer.ObjectReady -= this.Observer_ObjectReady;
				if(this.IsCancelled)
				{
					observer.Cancel();
					this.InProgress = false;
					this.OnCompleted?.Invoke(observer, new FinishedLoadingEventArgs() { Status = ManagementStatus.OperationCanceled, });
				}
			}
			this.IsCancelled = false;
		}

		private void Observer_ObjectReady(Object sender, ObjectReadyEventArgs e)
			=> this.OnObjectReady?.Invoke(sender, e);

		private void Observer_Completed(Object sender, CompletedEventArgs e)
		{
			this.InProgress = false;
			this.RemoveObserver();
			FinishedLoadingEventArgs args = new FinishedLoadingEventArgs() { Status = e.Status, };
			if(e.Status == ManagementStatus.NoError && this.IsCancelled)
				args.Status = ManagementStatus.OperationCanceled;

			this.OnCompleted?.Invoke(sender, args);
		}
	}
}