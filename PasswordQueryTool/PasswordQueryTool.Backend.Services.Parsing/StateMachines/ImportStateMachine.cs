using Automatonymous;
using MassTransit;
using PasswordQueryTool.Backend.QueueContracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.StateMachines
{
    public class ImportStateMachine : MassTransitStateMachine<ImportState>
    {
        private const int ActiveThreshold = 40;

        public ImportStateMachine()
        {
            Event(() => ImportReceived, x => x.CorrelateById(c => c.Message.ImportId));
            Event(() => FileChunks, x => x.CorrelateById(c => c.Message.ImportId));
            Event(() => ChunkImport, x => x.CorrelateById(c => c.Message.ImportId));
            Event(() => StateRequested, x =>
            {
                x.CorrelateById(c => c.Message.ImportId);
                x.OnMissingInstance(m => m.ExecuteAsync(context => context.RespondAsync<IImportJobNotFound>(new
                {
                    context.Message.ImportId,
                    InVar.Timestamp
                })));
            });
            Event(() => CancellationRequested, x =>
            {
                x.CorrelateById(c => c.Message.ImportId);
                x.OnMissingInstance(m => m.ExecuteAsync(context => context.RespondAsync<ICancellationFailed>(new
                {
                    context.Message.ImportId,
                    ErrorCode = "Instance not found"
                })));
            });

            InstanceState(s => s.CurrentState);

            Initially(
                When(ImportReceived)
                    .Then(Initialize)
                    .ThenAsync(InitializeFileChunking)
                    .TransitionTo(FileAnalyzing)
                );

            During(FileAnalyzing,
                When(FileChunks)
                    .Then(ReadFileChunks)
                    .ThenAsync(DispatchJobs)
                    .TransitionTo(Parsing)
                );

            During(Parsing,
                When(ChunkImport)
                    .Then(FinalizeChunk)
                    .IfElse(context => context.Instance.Chunks.Count(c => c.ChunkState != ChunkState.Processed) == 0,
                        binder => binder
                            .TransitionTo(Finished),
                        binder => binder
                            .ThenAsync(DispatchJobs))
                );

            During(Canceled,
                When(ChunkImport)
                    .Then(FinalizeChunk)
                );

            DuringAny(
                When(StateRequested)
                    .RespondAsync(async x => await x.Init<IImportState>(new
                    {
                        ImportId = x.Instance.CorrelationId,
                        State = (await this.GetState(x.Instance)).Name,
                        ImportedLines = x.Instance.ImportedLines,
                        DiscardedLines = x.Instance.DiscardedLines,
                        ProcessedLines = x.Instance.ProcessedLines,
                        TotalChunks = x.Instance.TotalChunks,
                        ProcessedChunks = x.Instance.ProcessedChunks,
                        FileName = x.Instance.FileName
                    })
                ),
                When(CancellationRequested)
                    .TransitionTo(Canceled)
                    .RespondAsync(async x => await x.Init<IImportCanceled>(new
                    {
                        ImportId = x.Instance.CorrelationId
                    })
                )
           );
        }

        public State Canceled { get; private set; }

        public Event<IBulkImportResultsReceived> ChunkImport { get; private set; }

        public State FileAnalyzing { get; private set; }

        public Event<IFileChunks> FileChunks { get; private set; }

        public State Finished { get; private set; }

        public Event<IImportReceived> ImportReceived { get; private set; }

        public State Parsing { get; private set; }

        public Event<IImportStateRequested> StateRequested { get; private set; }

        public Event<IRequestCancellation> CancellationRequested { get; private set; }

        private static void Initialize(BehaviorContext<ImportState, IImportReceived> context)
        {
            context.Instance.FileName = context.Data.FileName;
        }

        private async Task DispatchJobs(BehaviorContext<ImportState> context)
        {
            while (context.Instance.Chunks.Any(c => c.ChunkState == ChunkState.Unprocessed) && context.Instance.Chunks.Count(c => c.ChunkState == ChunkState.Processing) < ActiveThreshold)
            {
                var chunk = context.Instance.Chunks.First(c => c.ChunkState == ChunkState.Unprocessed);
                chunk.ChunkState = ChunkState.Processing;
                await InitiateJob(context, chunk);
            }
        }

        private void FinalizeChunk(BehaviorContext<ImportState, IBulkImportResultsReceived> context)
        {
            foreach (var importedChunk in context.Data.Chunks)
            {
                context.Instance.ProcessedChunks++;
                context.Instance.DiscardedLines += importedChunk.DiscardedLines;
                context.Instance.ProcessedLines += importedChunk.TotalLines;
                context.Instance.ImportedLines += importedChunk.ImportedLines;

                var chunk = context.Instance.Chunks.FirstOrDefault(c => c.Id == importedChunk.ChunkJobId);

                chunk.ChunkState = ChunkState.Processed;
            }
        }

        private async Task InitializeFileChunking(BehaviorContext<ImportState, IImportReceived> context)
        {
            await context.CreateConsumeContext().Publish<ISubmitFileForChunking>(new
            {
                ImportId = context.Data.ImportId,
                FileName = context.Instance.FileName,
                Timestamp = InVar.Timestamp
            });
        }

        private async Task InitiateJob(BehaviorContext<ImportState> context, JobChunk chunk)
        {
            var chunkId = chunk.Id;

            await context.CreateConsumeContext().Publish<IChunkJobReceived>(new
            {
                ChunkJobId = chunkId,
                ImportId = context.Instance.CorrelationId,
                Chunk = new Chunk { Length = chunk.Length, Offset = chunk.Offset },
                Filename = context.Instance.FileName,
                Timestamp = InVar.Timestamp
            });
        }

        private void ReadFileChunks(BehaviorContext<ImportState, IFileChunks> context)
        {
            context.Instance.TotalChunks = context.Data.Chunks.Count;

            foreach (var item in context.Data.Chunks)
            {
                context.Instance.Chunks.Add(new JobChunk()
                {
                    Id = NewId.NextGuid(),
                    ChunkState = ChunkState.Unprocessed,
                    Length = item.Length,
                    Offset = item.Offset
                });
            }
        }
    }
}