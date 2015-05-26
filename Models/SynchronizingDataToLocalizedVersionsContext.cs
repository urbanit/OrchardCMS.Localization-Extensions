using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Urbanit.Localization.Extensions.Models
{
    public class SynchronizingDataToLocalizedVersionsContext : ISynchronizingDataToLocalizedVersionsContext
    {
        public IContent ContentItem { get; set; }

        public IEnumerable<IContent> LocalizedVersions { get; set; }
    }
}