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
                var sortedEntries = entry.History.OrderByDescending(h => h.LastModificationTime).ToList();
                for (var i=0; i < sortedEntries.Count(); i++)
                {
                    if(!entry.Strings.GetSafe(PwDefs.PasswordField).ValueEquals(sortedEntries[i].Strings.GetSafe(PwDefs.PasswordField)))
                    {
                        // the last time the password was changed was the entry before the current one (ordered new to old)
                        return i==0 ? entry.LastModificationTime : sortedEntries[i-1].LastModificationTime;
                    }
                }

                // if the password is the same in the entry and all history items all we know is that it is at least as old as the earliest history item
                return sortedEntries.Last().LastModificationTime;
            }

            // no history means either the password has just been set, or the history has been pruned in which case we have no other data
            return entry.LastModificationTime;
        }
    }
}
