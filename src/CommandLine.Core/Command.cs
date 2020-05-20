﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Orang
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Command
    {
        public Command(
            string name,
            string description,
            IEnumerable<CommandArgument>? arguments = null,
            IEnumerable<CommandOption>? options = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Arguments = arguments?.ToImmutableArray() ?? ImmutableArray<CommandArgument>.Empty;
            Options = options?.ToImmutableArray() ?? ImmutableArray<CommandOption>.Empty;
        }

        public string Name { get; }

        public string Description { get; }

        public ImmutableArray<CommandArgument> Arguments { get; }

        public ImmutableArray<CommandOption> Options { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Name + "  " + Description;

        public Command WithArguments(IEnumerable<CommandArgument> arguments)
        {
            return new Command(Name, Description, arguments, Options);
        }

        public Command WithOptions(IEnumerable<CommandOption> options)
        {
            return new Command(Name, Description, Arguments, options);
        }
    }
}
