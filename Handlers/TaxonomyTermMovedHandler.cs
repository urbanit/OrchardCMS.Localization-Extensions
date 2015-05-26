using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using System.Linq;

namespace Urbanit.Localization.Extensions.Handlers
{
    [OrchardFeature("Urbanit.Localization.Extensions.Taxonomies")]
    public class TaxonomyTermMovedHandler : ContentHandler
    {
        public TaxonomyTermMovedHandler(ITaxonomyService taxonomyService, ILocalizationService localizationService)
        {
            OnPublished<LocalizationPart>((ctx, part) =>
            {
                var contentItem = ctx.ContentItem;

                if (contentItem.As<TermPart>() == null) return;

                var commonPart = contentItem.As<ICommonPart>();
                var localizationPart = part;

                if (commonPart == null || localizationPart == null || contentItem.Version < 2) return;

                var localizedVersions = localizationService.GetLocalizations(contentItem);

                if (localizedVersions == null) return;

                var container = commonPart.Container;

                if (container == null) return;

                var localizedContainers = localizationService.GetLocalizations(container.ContentItem);

                if (localizedContainers == null) return;

                foreach (var localizedVersion in localizedVersions)
                {
                    var localizedContainer = localizedVersion.As<ICommonPart>().Container;

                    if (localizedContainer != null && !localizedContainers.Contains(localizedContainer.As<LocalizationPart>()))
                    {
                        var localizedTaxonomyId = localizedContainer.As<TaxonomyPart>() != null ? localizedContainer.As<TaxonomyPart>().Id : localizedContainer.As<TermPart>().TaxonomyId;

                        var newContainer = localizedContainers.Where(t => t.Culture.Culture == localizedVersion.Culture.Culture).FirstOrDefault();

                        // If a localized version doesn't exist for the new container, then (because of inconsistency) we have to delete the moved term's localized branch.
                        if (newContainer == null)
                        {
                            taxonomyService.DeleteTerm(localizedVersion.As<TermPart>());
                        }
                        else
                        {
                            taxonomyService.MoveTerm(taxonomyService.GetTaxonomy(localizedTaxonomyId), localizedVersion.As<TermPart>(), newContainer.As<TermPart>());
                        }
                    }
                }
            });
        }
    }
}