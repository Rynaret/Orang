﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;

namespace Orang.CommandLine
{
    internal class DeleteCommand : DeleteOrRenameCommand<DeleteCommandOptions>
    {
        public DeleteCommand(DeleteCommandOptions options) : base(options)
        {
        }

        protected override void ExecuteMatchCore(
            FileMatch fileMatch,
            SearchContext context,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            string indent = GetPathIndent(baseDirectoryPath);

            if (!Options.OmitPath)
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);

            bool deleted = false;

            if (!Options.Ask || AskToExecute(context, (Options.ContentOnly) ? "Delete content?" : "Delete?", indent))
            {
                try
                {
                    if (!Options.DryRun)
                    {
                        FileSystemHelpers.Delete(
                            fileMatch,
                            contentOnly: Options.ContentOnly,
                            includingBom: Options.IncludingBom,
                            filesOnly: Options.FilesOnly,
                            directoriesOnly: Options.DirectoriesOnly);

                        deleted = true;
                    }

                    if (fileMatch.IsDirectory)
                    {
                        context.Telemetry.ProcessedDirectoryCount++;
                    }
                    else
                    {
                        context.Telemetry.ProcessedFileCount++;
                    }
                }
                catch (Exception ex) when (ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    WriteFileError(ex, indent: indent);
                }
            }

            if (fileMatch.IsDirectory
                && deleted)
            {
                OnDirectoryChanged(new DirectoryChangedEventArgs(fileMatch.Path, null));
            }
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            WriteSearchedFilesAndDirectories(telemetry, Options.SearchTarget, verbosity);

            string filesTitle = (Options.ContentOnly)
                ? "Deleted files content"
                : "Deleted files";

            string directoriesTitle = (Options.ContentOnly)
                ? "Deleted directories content"
                : "Deleted directories";

            WriteProcessedFilesAndDirectories(telemetry, Options.SearchTarget, filesTitle, directoriesTitle, Options.DryRun, verbosity);
        }
    }
}
