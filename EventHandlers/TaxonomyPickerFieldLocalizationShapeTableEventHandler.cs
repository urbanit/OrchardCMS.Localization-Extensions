using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Taxonomies.Fields;
using Orchard.Taxonomies.Services;
using Piedone.HelpfulLibraries.Contents;
using System.Linq;

namespace Urbanit.Localization.Extensions.EventHandlers
{
    [OrchardFeature("Urbanit.Localization.Extensions.Taxonomies")]
    public class TaxonomyPickerFieldLocalizationShapeTableEventHandler : IShapeTableEventHandler
    {
        private readonly IWorkContextAccessor _wca;


        public TaxonomyPickerFieldLocalizationShapeTableEventHandler(IWorkContextAccessor wca)
        {
            _wca = wca;
        }


        public void ShapeTableCreated(ShapeTable shapeTable)
        {
            var filteredDescriptors = shapeTable.Descriptors.Where(k =>
                {
                    var lowerKey = k.Key.ToLower();
                    return lowerKey.Contains("edit") || lowerKey.Contains("save") || lowerKey.Contains("publish");
                });

            foreach (var descriptor in filteredDescriptors)
            {
                var value = descriptor.Value;
                var existingPlacement = value.Placement;

                value.Placement = ctx =>
                {
                    var taxonomyService = _wca.GetContext().Resolve<ITaxonomyService>();

                    var contentItem = ctx.Content.ContentItem;
                    var localizationPart = contentItem.As<LocalizationPart>();

                    if (localizationPart == null) return existingPlacement(ctx);

                    if (!HasLocalizedTaxonomyField(contentItem, taxonomyService)) return existingPlacement(ctx);

                    var selectedCulture = localizationPart.Culture != null && !string.IsNullOrEmpty(localizationPart.Culture.Culture) ? localizationPart.Culture.Culture : null;

                    if (selectedCulture == null)
                    {
                        if (descriptor.Key.Equals("Parts_Localization_ContentTranslations_Edit") || descriptor.Key.Equals("Content_SaveButton") || descriptor.Key.Equals("Parts_Title_Edit")) return existingPlacement(ctx);

                        return new PlacementInfo { Location = "-" };
                    }
                    else
                    {
                        if (!descriptor.Key.Equals("Fields_TaxonomyField_Edit")) return existingPlacement(ctx);

                        var taxonomyField = contentItem.AsField<TaxonomyField>(contentItem.TypeDefinition.Name, ctx.Differentiator);

                        if (taxonomyField == null) return existingPlacement(ctx);

                        var taxonomy = taxonomyService.GetTaxonomyByName(taxonomyField.PartFieldDefinition.Settings["TaxonomyFieldSettings.Taxonomy"]);

                        if (taxonomy == null) return existingPlacement(ctx);

                        var taxonomyLocalizationPart = taxonomy.As<LocalizationPart>();

                        if (taxonomyLocalizationPart == null) return existingPlacement(ctx);

                        var selectedCultureForTaxonomy = taxonomyLocalizationPart.Culture != null && !string.IsNullOrEmpty(taxonomyLocalizationPart.Culture.Culture) ? taxonomyLocalizationPart.Culture.Culture : null;

                        if (selectedCultureForTaxonomy == null) return existingPlacement(ctx);

                        if (selectedCulture == selectedCultureForTaxonomy) return existingPlacement(ctx);

                        return new PlacementInfo { Location = "-" };
                    }
                };
            }
        }


        private bool HasLocalizedTaxonomyField(ContentItem contentItem, ITaxonomyService taxonomyService)
        {
            var taxonomyFields = contentItem.Parts
                .Where(part => part.PartDefinition.Name == contentItem.TypeDefinition.Name)
                .SelectMany(part => part.Fields.Where(field => field.FieldDefinition.Name.Equals("TaxonomyField")));

            foreach (var taxonomyField in taxonomyFields)
            {
                var taxonomy = taxonomyService.GetTaxonomyByName(taxonomyField.PartFieldDefinition.Settings["TaxonomyFieldSettings.Taxonomy"]);

                if (taxonomy == null) continue;

                var taxonomyLocalizationPart = taxonomy.As<LocalizationPart>();

                if (taxonomyLocalizationPart == null) continue;

                var selectedCultureForTaxonomy = taxonomyLocalizationPart.Culture != null && !string.IsNullOrEmpty(taxonomyLocalizationPart.Culture.Culture) ? taxonomyLocalizationPart.Culture.Culture : null;

                if (selectedCultureForTaxonomy == null) continue;

                return true;
            }

            return false;
        }
    }
}