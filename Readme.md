# Localization Extensions Module Readme



This [Orchard](http://orchardproject.net/) module extends the functionality of Orchard.Localization. Fixes the container-containable localization synchronization and other further special synchronization-related things for Taxonomies.


## How to localize items step-by-step

1. Turn on the module.
2. Attach LocalizationPart both to the container item's type definition and to the containable item's type definition.
3. First create the container and localized containers then the containable and localized containable items. It's obvious that always need a parent to create a child.


## Example 1 - Blogs

1. Turn on the module.
2. Add a new culture besides the default one. Let's say the site has 2 cultures: en-US and el-GR.
3. Attach LocalizationPart to the Blog content type.
4. Attach LocalizationPart to the BlogPost content type.
5. Create a new Blog with en-US culture and "Blog EN" title.
6. From "Blog EN"'s editor create a localized Blog with the "+ New translation" link. With el-GR culture and "Blog GR" title.
7. When you click on the Blog link on the dashboard you'll see these 2 Blogs.
8. Add a new BlogPost to "Blog EN" with the title "BlogPost EN". This will be listed under "Blog EN".
9. From "BlogPost EN"'s editor create a localized BlogPost with the "+ New translation" link with el-GR culture and "BlogPost GR" title. This will be listed under "Blog GR".
10. Now you have a Blog with 2 cultures. You can translate the individual BlogPosts easily and the two Blogs' posts are nicely separated.


## Example 2 - Taxonomies

1. Turn on the module.
2. Add a new culture besides the default one. Let's say the site has 2 cultures: en-US and el-GR.
3. Attach LocalizationPart to the Taxonomy content type. Now you can translate Taxonomies (not Terms).
4. Create a new Taxonomy ("Categories EN") and translate it ("Categories GR").
5. Attach LocalizationPart to the automatically created "Categories EN Term"'s and "Categories GR Term"'s type definition. This has to be done always when you add a new Taxonomy or Taxonomy translation.
6. Add "Term 1 EN" Term to "Categories EN" Taxonomy. It will be listed under "Categories EN" Taxonomy.
7. Translate it with the "+ New translation" link in "Term 1 EN"'s editor. Name it "Term 1 GR". It will be listed under "Categories GR" Taxonomy.
8. Add some more Terms and translate them. At the end you will see 2 nicely synchronized Taxonomy trees.


## Special cases in Taxonomy translation

* When you delete a Taxonomy or Term, all the children will be deleted too. But all the translated pairs will be left.
* In case you try to translate a Term whose parent hasn't a translated version, you can't save that Term because we can't synchronize it into the translation tree.
* When you move a Term, its translated pair will be moved too. The only exception is when you move a Term with such a parent that doesn't have a translated pair, in this case the Term's translated pair and its children will be deleted. This feature is needed to preserve consistency so use the move feature wisely!


## Other cases

The module doesn't let you to add content with different culture to the container. E.g. if you try to add a Greek BlogPost to an english Blog, you can't do that, the BlogPost's culture will be automatically set to English.


## Localized content items with TaxonomyPickerField step-by-step

1. Create some localized Taxonomies, see "Example 2 - Taxonomies".
2. Attach LocalizationPart to the content item.
3. Attach as many TaxonomyPickerFields for all the cultures you have Taxonomies for.
4. Select 1-1 Taxonomies for the attached TaxonomyPickerFields. E.g. attach 2 TaxonomyPickerFields, "English Category" and "Greek Category". Select "Categories EN" taxonomy for the "English Category" field and select "Categories GR" taxonomy for the "Greek Category" field.
5. Next time when you create a new content item you'll get a culture-first editor and after saving you can select only from those TaxonomyPickerFields where the cultures match.