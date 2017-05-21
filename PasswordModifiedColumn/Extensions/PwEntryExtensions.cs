using KeePassLib;
using System;
using System.Linq;

namespace PasswordModifiedColumn.Extensions
{
    public static class PwEntryExtensions
    {
        public static DateTime GetPasswordLastModified(this PwEntry entry)
        {
            if (entry.History != null && entry.History.Any())
            {
                var sortedEntries = entry.History.OrderByDescending(h => h.LastModificationTime);
                foreach (var historyEntry in sortedEntries)
                {
                    if(!entry.Strings.GetSafe(PwDefs.PasswordField).Equals(historyEntry.Strings.GetSafe(PwDefs.PasswordField)))
                    {
                        return historyEntry.LastModificationTime;
                    }
                }
                return sortedEntries.Last().LastModificationTime;
            }

            return entry.LastModificationTime;
        }
    }
}
