using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using PasswordQueryTool.Backend.QueueContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.Consumers
{
    public class BatchImportResultConsumer : IConsumer<Batch<IChunkImportResultReceived>>
    {
        public async Task Consume(ConsumeContext<Batch<IChunkImportResultReceived>> context)
        {
            Dictionary<Guid, IBulkImportResultsReceived> messages = new();

            foreach (var message in context.Message)
            {
                if (!messages.ContainsKey(message.Message.ImportId))
                {
                    messages.Add(message.Message.ImportId, new ImportResult
                    {
                        ImportId = message.Message.ImportId
                    });
                }

                messages[message.Message.ImportId].Chunks.Add(message.Message);
            }

            await context.PublishBatch(messages.Values);
        }

        private class ImportResult : IBulkImportResultsReceived
        {
            public Guid ImportId { get; set; }

            public List<IChunkImportResultReceived> Chunks { get; } = new List<IChunkImportResultReceived>();
        }
    }

    class BatchImportResultConsumerDefinition :
        ConsumerDefinition<BatchImportResultConsumer>
    {
        public BatchImportResultConsumerDefinition()
        {
            Endpoint(x => x.PrefetchCount = 1000);
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<BatchImportResultConsumer> consumerConfigurator)
        {
            consumerConfigurator.Options<BatchOptions>(options => options
                .SetMessageLimit(200)
                .SetTimeLimit(TimeSpan.FromSeconds(10))
                .SetConcurrencyLimit(10));
        }
    }
}
