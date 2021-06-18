using System;

namespace PasswordQueryTool.ImportModels
{
    /// <summary>
    /// The possible states an import can be in.
    /// </summary>
    public enum ImportState
    {
        Unknown,
        Analyzing,
        Importing,
        Canceled,
        Finished
    }

    /// <summary>
    /// A single instance of an import.
    /// </summary>
    public class ImportDTO
    {
        /// <summary>
        /// Gets or sets the amount of total chunks in the job.
        /// </summary>
        public long ChunksAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the amount of finished chunks in the job.
        /// </summary>
        public long ChunksFinishedAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the import.
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the amount of invalid lines in the job.
        /// </summary>
        public long InvalidLines
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the amount of lines that are finished.
        /// </summary>
        public long LinesFinished
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the import.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current state of the job.
        /// </summary>
        public ImportState State
        {
            get;
            set;
        }
    }
}