﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    [Verb("rename", HelpText = "Renames files and directories.")]
    [OptionValueProvider(nameof(Content), OptionValueProviderNames.PatternOptionsWithoutPart)]
    [OptionValueProvider(nameof(Display), OptionValueProviderNames.Display_NonContent)]
    [OptionValueProvider(nameof(Highlight), OptionValueProviderNames.RenameHighlightOptions)]
    [OptionValueProvider(nameof(Name), OptionValueProviderNames.PatternOptionsWithoutGroupAndNegative)]
    internal sealed class RenameCommandLineOptions : DeleteOrRenameCommandLineOptions
    {
        [Option(longName: OptionNames.Ask,
            HelpText = "Ask for a permission to rename file or directory.")]
        public bool Ask { get; set; }

        [Option(longName: OptionNames.Conflict,
            HelpText = "Defines how to resolve conflict when a file/directory already exists.",
            MetaValue = MetaValues.ConflictResolution)]
        public string Conflict { get; set; } = null!;

        [Option(shortName: OptionShortNames.Content, longName: OptionNames.Content,
            HelpText = "Regular expression for files' content. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Content { get; set; } = null!;

        [Option(shortName: OptionShortNames.DryRun, longName: OptionNames.DryRun,
            HelpText = "Display which files or directories should be renamed but do not actually rename any file or directory.")]
        public bool DryRun { get; set; }

        [Option(longName: OptionNames.Evaluator,
            HelpText = "Path to the evaluator method to compute replacements. The format is \"LibraryPath,FullTypeName.MethodName\".",
            MetaValue = MetaValues.Evaluator)]
        public string Evaluator { get; set; } = null!;

        [Option(shortName: OptionShortNames.MaxCount, longName: OptionNames.MaxCount,
            HelpText = "Stop renaming after specified number is reached.",
            MetaValue = MetaValues.Num)]
        public int MaxCount { get; set; }

        [Option(longName: OptionNames.Modify,
            HelpText = "Functions to modify result.",
            MetaValue = MetaValues.ReplaceModify)]
        public IEnumerable<string> Modify { get; set; } = null!;

        [Option(shortName: OptionShortNames.Name, longName: OptionNames.Name,
            Required = true,
            HelpText = "Regular expression for file or directory name. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Name { get; set; } = null!;

        [Option(shortName: OptionShortNames.Replacement, longName: OptionNames.Replacement,
            HelpText = "Replacement pattern. Syntax is <REPLACEMENT> [<REPLACEMENT_OPTIONS>].",
            MetaValue = MetaValues.Replacement)]
        public IEnumerable<string> Replacement { get; set; } = null!;

        public bool TryParse(RenameCommandOptions options)
        {
            var baseOptions = (DeleteOrRenameCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (RenameCommandOptions)baseOptions;

            if (!TryParseAsEnumFlags(Highlight, OptionNames.Highlight, out HighlightOptions highlightOptions, defaultValue: HighlightOptions.Replacement, provider: OptionValueProviders.RenameHighlightOptionsProvider))
                return false;

            if (!FilterParser.TryParse(Name, OptionNames.Name, OptionValueProviders.PatternOptionsWithoutGroupAndNegativeProvider, out Filter? nameFilter, out FileNamePart namePart, namePartProvider: OptionValueProviders.NamePartKindProvider_WithoutFullName))
                return false;

            if (!FilterParser.TryParse(Content, OptionNames.Content, OptionValueProviders.PatternOptionsWithoutPartProvider, out Filter? contentFilter, allowNull: true))
                return false;

            if (!TryParseReplacement(Replacement, out string? replacement))
                return false;

            if (!DelegateFactory.TryCreateMatchEvaluator(Evaluator, out MatchEvaluator? matchEvaluator))
                return false;

            if (replacement != null && matchEvaluator != null)
            {
                WriteError($"Options '{OptionNames.GetHelpText(OptionNames.Replacement)}' and '{OptionNames.GetHelpText(OptionNames.Evaluator)}' cannot be set both at the same time.");
                return false;
            }

            if (!TryParseReplaceOptions(Modify, OptionNames.Modify, replacement, matchEvaluator, out ReplaceOptions? replaceOptions))
                return false;

            if (!TryParseAsEnum(Conflict, OptionNames.Conflict, out ConflictResolution conflictResolution, defaultValue: ConflictResolution.Ask, provider: OptionValueProviders.ConflictResolutionProvider_WithoutSuffix))
                return false;

            if (!TryParseDisplay(
                values: Display,
                optionName: OptionNames.Display,
                contentDisplayStyle: out ContentDisplayStyle? _,
                pathDisplayStyle: out PathDisplayStyle? pathDisplayStyle,
                lineDisplayOptions: out LineDisplayOptions lineDisplayOptions,
                lineContext: out LineContext lineContext,
                displayParts: out DisplayParts displayParts,
                fileProperties: out ImmutableArray<FileProperty> fileProperties,
                indent: out string? indent,
                separator: out string? separator,
                contentDisplayStyleProvider: OptionValueProviders.ContentDisplayStyleProvider,
                pathDisplayStyleProvider: OptionValueProviders.PathDisplayStyleProvider_Rename))
            {
                return false;
            }

            if (pathDisplayStyle == PathDisplayStyle.Relative
                && options.Paths.Length > 1
                && options.SortOptions != null)
            {
                pathDisplayStyle = PathDisplayStyle.Full;
            }

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: ContentDisplayStyle.None,
                pathDisplayStyle: pathDisplayStyle ?? PathDisplayStyle.Full,
                lineOptions: lineDisplayOptions,
                lineContext: lineContext,
                displayParts: displayParts,
                fileProperties: fileProperties,
                indent: indent,
                separator: separator);

            options.HighlightOptions = highlightOptions;
            options.SearchTarget = GetSearchTarget();
            options.ReplaceOptions = replaceOptions;
            options.Ask = Ask;
            options.DryRun = DryRun;
            options.NameFilter = nameFilter;
            options.NamePart = namePart;
            options.ContentFilter = contentFilter;
            options.MaxMatchingFiles = MaxCount;
            options.ConflictResolution = conflictResolution;

            return true;
        }
    }
}
