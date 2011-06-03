﻿using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Logging
{
    public interface ILogService
    {
        LogEntry Create(LogEntry entry);
        LogEntry Get(Identity id);
        void Truncate();
        IEnumerable<LogEntry> Find(string query, int? skip = null, int? limit = null);
    }
}