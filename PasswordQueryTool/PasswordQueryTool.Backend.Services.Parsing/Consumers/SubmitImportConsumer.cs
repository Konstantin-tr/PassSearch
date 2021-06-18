using MassTransit;
using PasswordQueryTool.Backend.QueueContracts;
using PasswordQueryTool.Backend.Services.Parsing.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.Consumers
{
    public class SubmitImportConsumer : IConsumer<ISubmitImport>
    {
        private readonly IFileService _fileService;

        public SubmitImportConsumer(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task Consume(ConsumeContext<ISubmitImport> context)
        {
            if (!(await _fileService.FileExists(context.Message.FileName)))
            {
                await context.RespondAsync<IImportRejected>(new
                {
                    ErrorCode = "File does not exist",
                    ImportId = context.Message.ImportId,
                    Timestamp = InVar.Timestamp
                });

                return;
            }

            await context.Publish<IImportReceived>(new
            {
                ImportId = context.Message.ImportId,
                FileName = context.Message.FileName,
                Timestamp = InVar.Timestamp
            });

            await context.RespondAsync<IImportSubmitted>(new
            {
                InVar.Timestamp,
                ImportId = context.Message.ImportId
            });
        }
    }
}