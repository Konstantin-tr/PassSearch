using MassTransit;
using PasswordQueryTool.Backend.QueueContracts;
using PasswordQueryTool.Backend.Services.Parsing.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.Consumers
{
    public class ChunkJobConsumer : IConsumer<IChunkJobReceived>
    {
        private readonly IFileService _fileService;

        public ChunkJobConsumer(IFileService fileService)
        {
            _fileService = fileService;
        }

        public static bool IsValidEmail(string line)
        {
            var emailStructure = Regex.IsMatch(line,
                    @"^[^@\s]+@[^@\s\.]+[^@\s]*\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

            if (!emailStructure)
            {
                return false;
            }

            var multipleDotCheck = Regex.IsMatch(line,
                    @"^[^@\s]+@[\s\S]*[.]{2,}[\s\S]*$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

            if (multipleDotCheck)
            {
                return false;
            }

            return true;
        }

        public static string NormalizeEmail(string input)
        {
            var email = Regex.Replace(input, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }

            return email;
        }

        public static ParsingResult SplitLine(string line)
        {
            var seperatorIndex = line.IndexOfAny(new char[] { ';', ':' });

            if (seperatorIndex == -1)
            {
                return null;
            }

            var email = line.Substring(0, seperatorIndex) ?? string.Empty;

            var password = line.Substring(seperatorIndex + 1) ?? string.Empty;

            return new ParsingResult { Email = email, Password = password };
        }

        public async Task Consume(ConsumeContext<IChunkJobReceived> context)
        {
            await _fileService.GetFileStream(context.Message.Filename, context.Message.Chunk.Offset, context.Message.Chunk.Length, stream => ProcessStream(stream, context));
        }

        private ParsingResult ParseLine(string line)
        {
            try
            {
                if (line == null)
                {
                    return null;
                }

                var parsingResult = SplitLine(line);

                if (parsingResult == null)
                {
                    return null;
                }

                if (!IsValidEmail(parsingResult.Email))
                {
                    return null;
                }

                parsingResult.Email = NormalizeEmail(parsingResult.Email);

                return parsingResult;
            }
            catch (Exception)
            {
                // TODO log errors
                return null;
            }
        }

        private async Task ProcessStream(Stream stream, ConsumeContext<IChunkJobReceived> context)
        {
            using var reader = new StreamReader(stream);

            var lines = new List<string>();

            while (true)
            {
                var line = await reader.ReadLineAsync();

                if (line == null)
                {
                    break;
                }

                lines.Add(line);
            }

            var validLines = lines
                .AsParallel()
                .Select(i => ParseLine(i))
                .Where(i => i != null)
                .Select(i => new PasswordCombination { Email = i.Email, Password = i.Password }).ToList();

            var discaredLines = lines.Count - validLines.Count;

            await context.Publish<IPasswordCombinationsReceived>(new
            {
                ImportId = context.Message.ImportId,
                ChunkJobId = context.Message.ChunkJobId,
                PasswordCombinations = validLines,
                DiscardedLinesCount = discaredLines,
                Timestamp = InVar.Timestamp
            });
        }

        public class ParsingResult
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}