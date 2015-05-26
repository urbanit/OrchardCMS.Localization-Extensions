using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Urbanit.Localization.Extensions.Events;
using Urbanit.Localization.Extensions.Models;

namespace Urbanit.Localization.Extensions.EventHandlers
{
    /// <summary>
    /// Further synchronization for Taxonomy terms.
    /// </summary>
    [OrchardFeature("Urbanit.Localization.Extensions.Taxonomies")]
    public class TermPartSynchronizationEventHandler : ILocalizationExtensionEventHandler
    {
        private readonly ITaxonomyService _taxonomyService;


        public TermPartSynchronizationEventHandler(ITaxonomyService taxonomyService)
        {
            _taxonomyService = taxonomyService;
        }


        public void ContainerSynchronized(IContainerSynchronizedContext containerSynchronizedContext)
        {
            var termPart = containerSynchronizedContext.ContentItem.As<TermPart>();
            if (termPart == null) return;

            var localizedMasterContentItemContainer = containerSynchronizedContext.LocalizedMasterContentItemContainer;

            var localizedTaxonomyId = localizedMasterContentItemContainer.As<TaxonomyPart>() != null ? localizedMasterContentItemContainer.As<TaxonomyPart>().Id : localizedMasterContentItemContainer.As<TermPart>().TaxonomyId;

            termPart.TaxonomyId = localizedTaxonomyId;
            termPart.Container = localizedMasterContentItemContainer;
            _taxonomyService.ProcessPath(termPart);
        }

        public void SynchronizingDataToLocalizedVersions(ISynchronizingDataToLocalizedVersionsContext synchronizingDataToLocalizedVersionsContext) { }
    }
}