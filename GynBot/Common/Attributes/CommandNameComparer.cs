﻿using Discord.Commands;
using System;
using System.Collections.Generic;

namespace GynBot.Common.Attributes
{
    public class CommandNameComparer : IEqualityComparer<CommandInfo>
    {
        #region Public Methods

        public bool Equals(CommandInfo x, CommandInfo y) => x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) && x.Module.Name != y.Module.Name;

        public int GetHashCode(CommandInfo obj) => obj.Name.ToLowerInvariant().GetHashCode();

        #endregion Public Methods
    }
}