﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Orang.FileSystem;
using Orang.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal abstract class FileSystemCommandOptions : CommonRegexCommandOptions
    {
        private string _doubleIndent;

        internal FileSystemCommandOptions()
        {
        }

        public ImmutableArray<PathInfo> Paths { get; internal set; }

        public Filter? NameFilter { get; internal set; }

        public FileNamePart NamePart { get; internal set; }

        public Filter? ExtensionFilter { get; internal set; }

        public Filter? ContentFilter { get; internal set; }

        public Filter? DirectoryFilter { get; internal set; }

        public FileNamePart DirectoryNamePart { get; internal set; }

        public SearchTarget SearchTarget { get; internal set; }

        public FileAttributes Attributes { get; internal set; }

        public FileAttributes AttributesToSkip { get; internal set; }

        public bool RecurseSubdirectories { get; internal set; }

        public bool Progress { get; internal set; }

        public Encoding DefaultEncoding { get; internal set; }

        public FileEmptyOption EmptyOption { get; internal set; }

        public int MaxMatchingFiles { get; internal set; }

        public SortOptions? SortOptions { get; internal set; }

        public FilePropertyFilter FilePropertyFilter { get; internal set; }

        public FilterPredicate<DateTime>? CreationTimePredicate { get; internal set; }

        public FilterPredicate<DateTime>? ModifiedTimePredicate { get; internal set; }

        public FilterPredicate<long>? SizePredicate { get; internal set; }

        public ContentDisplayStyle ContentDisplayStyle => Format.ContentDisplayStyle;

        public PathDisplayStyle PathDisplayStyle => Format.PathDisplayStyle;

        public bool OmitPath => PathDisplayStyle == PathDisplayStyle.Omit;

        public bool DisplayRelativePath => PathDisplayStyle == PathDisplayStyle.Relative;

        public string Indent => Format.Indent;

        internal string DoubleIndent => _doubleIndent ?? (_doubleIndent = Indent + Indent);

        internal bool IncludeBaseDirectory => Format.IncludeBaseDirectory;

        internal MatchOutputInfo? CreateOutputInfo(FileMatch fileMatch)
        {
            //TODO: 
#pragma warning disable CS8604 // Possible null reference argument.
            return CreateOutputInfo(fileMatch.ContentText, fileMatch.ContentMatch);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        internal MatchOutputInfo? CreateOutputInfo(string input, Match match)
        {
            if (ContentDisplayStyle != ContentDisplayStyle.ValueDetail)
                return null;

            //TODO: 
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int groupNumber = ContentFilter.GroupNumber;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return MatchOutputInfo.Create(
                MatchData.Create(input, ContentFilter.Regex, match),
                groupNumber,
                includeGroupNumber: groupNumber >= 0,
                includeCaptureNumber: false,
                new OutputCaptions(shortMatch: ""));
        }
    }
}
