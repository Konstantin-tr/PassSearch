using MassTransit;
using PasswordQueryTool.Backend.Database;
using PasswordQueryTool.Backend.QueueContracts;
using PasswordQueryTool.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.DatabaseAccess.Consumers
{
    public class PasswordCombinationsConsumer : IConsumer<IPasswordCombinationsReceived>
    {
        private readonly IDatabaseHelper _databaseHelper;

        public PasswordCombinationsConsumer(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public async Task Consume(ConsumeContext<IPasswordCombinationsReceived> context)
        {
            var loginData = context.Message.PasswordCombinations.Select(p => new LoginData()
            {
                Password = p.Password,
                Email = new EmailData(p.Email)
            });

            var errorCount = await _databaseHelper.BulkInsertDataAsync(loginData);

            await context.Publish<IChunkImportResultReceived>(new
            {
                ImportId = context.Message.ImportId,
                ChunkJobId = context.Message.ChunkJobId,
                ImportedLines = context.Message.PasswordCombinations.Count - errorCount,
                DiscardedLines = context.Message.DiscardedLinesCount,
                TotalLines = context.Message.DiscardedLinesCount + context.Message.PasswordCombinations.Count
            });
        }
    }
}