﻿using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Modules
{
	public class PurgeQueuesObserver : IPipelineObserver<OnAfterInitializeQueueFactories>
	{
		private readonly ILog _log;

		public PurgeQueuesObserver()
		{
			_log = Log.For(this);
		}

		public void Execute(OnAfterInitializeQueueFactories pipelineEvent)
		{
			var section = ConfigurationSectionProvider.Open<PurgeQueuesSection>("shuttle", "purgeQueues");

			if (section == null || section.Queues == null)
			{
				return;
			}

			foreach (PurgeQueueElement queueElement in section.Queues)
			{
				var queue = pipelineEvent.Pipeline.State.GetServiceBus().Configuration.QueueManager.GetQueue(queueElement.Uri);
				var purge = queue as IPurgeQueue;

				if (purge == null)
				{
					_log.Warning(string.Format(EsbModuleResources.IPurgeQueueNotImplemented, queue.GetType().FullName));

					continue;
				}

				purge.Purge();
			}
		}
	}
}