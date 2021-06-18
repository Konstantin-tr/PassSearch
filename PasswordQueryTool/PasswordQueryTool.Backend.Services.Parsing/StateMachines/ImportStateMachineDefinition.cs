using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using PasswordQueryTool.Backend.QueueContracts;
using System;

namespace PasswordQueryTool.Backend.Services.Parsing.StateMachines
{
    public class ImportStateMachineDefinition : SagaDefinition<ImportState>
    {
        public ImportStateMachineDefinition()
        {
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<ImportState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => 
            {
                r.Interval(10, TimeSpan.FromMilliseconds(500));
            });
            sagaConfigurator.UseDelayedRedelivery(r => r.Incremental(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));
            sagaConfigurator.UseInMemoryOutbox();

            var partition = endpointConfigurator.CreatePartitioner(8);

            sagaConfigurator.Message<IImportReceived>(x => x.UsePartitioner(partition, m => m.Message.ImportId));
            sagaConfigurator.Message<IFileChunks>(x => x.UsePartitioner(partition, m => m.Message.ImportId));
            sagaConfigurator.Message<IBulkImportResultsReceived>(x => x.UsePartitioner(partition, m => m.Message.ImportId));
            sagaConfigurator.Message<IImportStateRequested>(x => x.UsePartitioner(partition, m => m.Message.ImportId));
            sagaConfigurator.Message<IRequestCancellation>(x => x.UsePartitioner(partition, m => m.Message.ImportId));
        }
    }
}