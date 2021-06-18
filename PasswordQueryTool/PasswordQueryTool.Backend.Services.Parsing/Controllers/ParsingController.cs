using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PasswordQueryTool.Backend.QueueContracts;
using PasswordQueryTool.Backend.Services.Parsing.Database;
using PasswordQueryTool.Backend.Services.Parsing.Services.Interfaces;
using PasswordQueryTool.Backend.Services.Parsing.StateMachines;
using PasswordQueryTool.ImportModels;
using PasswordQueryTool.Parsing.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using ImportState = PasswordQueryTool.ImportModels.ImportState;

namespace PasswordQueryTool.Backend.Services.Parsing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParsingController : ControllerBase
    {
        private readonly ParsingDbContext _dbContext;
        private readonly IFileService _fileService;
        private readonly IRequestClient<IImportStateRequested> _requestImportStateClient;
        private readonly IRequestClient<ISubmitImport> _submitImportClient;
        private readonly IRequestClient<IRequestCancellation> _requestCancellationClient;

        public ParsingController(IRequestClient<ISubmitImport> submitImportClient, IRequestClient<IImportStateRequested> requestImportStateClient, IFileService fileService, ParsingDbContext dbContext, IRequestClient<IRequestCancellation> requestCancellationClient)
        {
            _submitImportClient = submitImportClient;
            _requestImportStateClient = requestImportStateClient;
            _fileService = fileService;
            _dbContext = dbContext;
            _requestCancellationClient = requestCancellationClient;
        }

        [HttpPost]
        [Route("jobs")]
        public async Task<ActionResult<Guid>> CreateJob([FromBody] string fileName)
        {
            var id = NewId.NextGuid();

            var (accepted, rejected) = await _submitImportClient.GetResponse<IImportSubmitted, IImportRejected>(new
            {
                ImportId = id,
                FileName = fileName,
                Timestamp = InVar.Timestamp
            });

            if (accepted.IsCompleted)
            {
                await accepted;

                return Accepted(id);
            }

            var response = await rejected;

            return BadRequest(response.Message.ErrorCode);
        }

        [HttpDelete]
        [Route("jobs/{jobIdString}")]
        public async Task<ActionResult<Guid>> DeleteJob(string jobIdString)
        {
            var jobId = Guid.Parse(jobIdString);

            var (accepted, rejected) = await _requestCancellationClient.GetResponse<IImportCanceled, ICancellationFailed>(new
            {
                ImportId = jobId
            });

            await accepted;

            if (accepted.IsCompletedSuccessfully)
            {
                await accepted;

                return Ok(jobId);
            }

            var response = await rejected;

            return BadRequest(response.Message.ErrorCode);
        }

        [HttpGet]
        [Route("files")]
        public async Task<IActionResult> GetAvailableFiles()
        {
            var files = (await _fileService.GetAllFiles()).ToList();

            if (files == null)
            {
                return Problem();
            }
            return Ok(files.Select(f => new ImportFile() { FileSize = f.FileSize, Name = f.FileName }));
        }

        [HttpGet]
        [Route("jobs")]
        public async Task<IActionResult> GetJob()
        {
            var states = _dbContext.ImportStates.ToList();

            var response = states.Select(s => new ImportDTO
            {
                Id = s.CorrelationId,
                ChunksAmount = s.TotalChunks ?? 0,
                ChunksFinishedAmount = s.ProcessedChunks,
                InvalidLines = s.DiscardedLines,
                LinesFinished = s.ImportedLines,
                Name = s.FileName,
                State = s.CurrentState switch
                {
                    nameof(ImportStateMachine.FileAnalyzing) => ImportState.Analyzing,
                    nameof(ImportStateMachine.Parsing) => ImportState.Importing,
                    nameof(ImportStateMachine.Finished) => ImportState.Finished,
                    nameof(ImportStateMachine.Canceled) => ImportState.Canceled,
                    _ => ImportState.Unknown,
                }
            });

            return Ok(response.ToList());
        }

        [HttpPost]
        [Route("job")]
        public async Task<IActionResult> GetJob([FromBody] Guid jobId)
        {
            var (status, notFound) = await _requestImportStateClient.GetResponse<IImportState, IImportJobNotFound>(new
            {
                ImportId = jobId
            });

            if (notFound.IsCompletedSuccessfully)
            {
                await notFound;

                return NotFound(new
                {
                    BatchId = jobId
                });
            }

            Response<IImportState> response = await status;

            var msg = response.Message;

            var importDto = new ImportDTO
            {
                Id = msg.ImportId,
                ChunksAmount = msg.TotalChunks,
                ChunksFinishedAmount = msg.ProcessedChunks,
                InvalidLines = msg.DiscardedLines,
                LinesFinished = msg.ProcessedLines,
                Name = msg.FileName,
                State = msg.State switch
                {
                    nameof(ImportStateMachine.FileAnalyzing) => ImportState.Analyzing,
                    nameof(ImportStateMachine.Parsing) => ImportState.Importing,
                    nameof(ImportStateMachine.Finished) => ImportState.Finished,
                    nameof(ImportStateMachine.Canceled) => ImportState.Canceled,
                    _ => ImportState.Unknown,
                }
            };

            return Ok(importDto);
        }
    }
}