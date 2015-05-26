using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.UI.Notify;
using System.Linq;
using Urbanit.Localization.Extensions.Events;
using Urbanit.Localization.Extensions.Models;


namespace Urbanit.Localization.Extensions.Handlers
{
    public class LocalizationHandler : ContentHandler
    {
        public Localizer T { get; set; }


        public LocalizationHandler(
            ILocalizationService localizationService,
            IOrchardServices orchardServices,
            ILocalizationExtensionEventHandler localizationExtensionEventHandler)
        {
            T = NullLocalizer.Instance;


            OnUpdateEditorShape<LocalizationPart>((ctx, part) =>
            {
                var contentItem = ctx.ContentItem;

                // When the content item's id is 0, then it's the GET editor or the unsuccessful POST shape so we return.
                if (contentItem.Id == 0) return;

                var commonPart = contentItem.As<ICommonPart>();
                var localizationPart = part;

                // If no CommonPart attached, then we don't do anything.
                if (commonPart == null) return;

                var masterContentItem = localizationPart.MasterContentItem;
                // This can be true if we try to create a localized version with the "new translation link" so we have to set the container properly.
                if (masterContentItem != null && masterContentItem.As<ICommonPart>().Container != null && commonPart.Container == null)
                {
                    // Getting the selected culture.
                    if (!CultureIsSet(localizationPart)) return;
                    var selectedCulture = localizationPart.Culture.Culture;

                    var masterContentItemContainer = masterContentItem.As<ICommonPart>().Container;

                    // Getting the container's localized pair.
                    var localizedMasterContentItemContainer = localizationService.GetLocalizations(masterContentItemContainer).Where(l => l.Culture.Culture == selectedCulture).FirstOrDefault();

                    // If no localized container pair created yet, then we delete the currenly created content item because it can confuse the user.
                    if (localizedMasterContentItemContainer == null)
                    {
                        // Notifying the user about the error.
                        orchardServices.Notifier.Error(T("Creating translation was unsuccessful, because no container (parent) element found. E.g. parent term missing from translation tree or no translated blog for blog post. This item will be deleted!"));

                        // Removing the currently created contentitem.
                        orchardServices.TransactionManager.Cancel();

                        return;
                    }

                    // Setting the container. This is enough for the conventional content items like blogs and blog posts. However in special cases (like taxonomies) further synchronization is needed.
                    commonPart.Container = localizedMasterContentItemContainer;

                    // Firing events for further synchronization. See TermPart synchronization example in TermPartSynchronizationEventHandler.cs.
                    localizationExtensionEventHandler.ContainerSynchronized(new ContainerSynchronizedContext { ContentItem = contentItem, LocalizedMasterContentItemContainer = localizedMasterContentItemContainer });
                }
                // If this is true then we possibly have to set the content item's culture.
                else if (masterContentItem == null && commonPart.Container != null)
                {
                    var containerLocalizationPart = commonPart.Container.As<LocalizationPart>();
                    if (!CultureIsSet(localizationPart) || !CultureIsSet(containerLocalizationPart)) return;

                    // This can be happen if the user tries to create a content item from a container but the selected culture differs from the container's culture. We can't let this happen because this makes no sense and confuses the user.
                    if (!localizationPart.Culture.Culture.Equals(containerLocalizationPart.Culture.Culture))
                    {
                        localizationService.SetContentCulture(contentItem, containerLocalizationPart.Culture.Culture);

                        orchardServices.Notifier.Warning(T("You tried to create a content item from a container but the selected culture differed from the container's culture. So we fixed the content item's culture."));
                    }
                }

                // Firing data synchronization events.
                var localizedItems = localizationService.GetLocalizations(contentItem);
                if (localizedItems.Count() > 0)
                {
                    localizationExtensionEventHandler.SynchronizingDataToLocalizedVersions(new SynchronizingDataToLocalizedVersionsContext { ContentItem = contentItem, LocalizedVersions = localizedItems });
                }
            });
        }


        private bool CultureIsSet(LocalizationPart localizationPart)
        {
            return localizationPart.Culture != null && !string.IsNullOrEmpty(localizationPart.Culture.Culture);
        }
    }
}