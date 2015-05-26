using Orchard.ContentManagement;

namespace Urbanit.Localization.Extensions.Models
{
    public class ContainerSynchronizedContext : IContainerSynchronizedContext
    {
        public IContent ContentItem { get; set; }

        public IContent LocalizedMasterContentItemContainer { get; set; }
    }
}