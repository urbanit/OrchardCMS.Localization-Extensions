using Orchard.Events;
using Urbanit.Localization.Extensions.Models;

namespace Urbanit.Localization.Extensions.Events
{
    /// <summary>
    /// Event handlers for the localization extensions module.
    /// </summary>
    public interface ILocalizationExtensionEventHandler : IEventHandler
    {
        /// <summary>
        /// Called after the container is properly synchronized. Use this event when further synchronization is needed for special content items.
        /// </summary>
        /// <param name="containerSynchronizedContext">The container synchronized context.</param>
        void ContainerSynchronized(IContainerSynchronizedContext containerSynchronizedContext);

        /// <summary>
        /// Called after the user saves a content item with attached LocalizationPart and CommonPart.
        /// </summary>
        /// <param name="synchronizingDataToLocalizedVersionsContext">The synchronizing data to localized versions context.</param>
        void SynchronizingDataToLocalizedVersions(ISynchronizingDataToLocalizedVersionsContext synchronizingDataToLocalizedVersionsContext);
    }
}