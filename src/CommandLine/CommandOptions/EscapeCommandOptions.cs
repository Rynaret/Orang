﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal sealed class EscapeCommandOptions
    {
        internal EscapeCommandOptions()
        {
        }

        public string Input { get; internal set; }

        public bool InCharGroup { get; internal set; }

        public bool Replacement { get; internal set; }
    }
}
