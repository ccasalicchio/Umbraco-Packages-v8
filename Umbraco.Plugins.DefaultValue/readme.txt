# Default Value Property Editor

for Umbraco v8.18.7

- Creates a Data Type with a Default Value (used for properties that cannot be modified)

This was useful for a project that implements the schema.org types, so each page (article, news, foto) has a default value (Article, newsArticle, Photograph) that may be overridden by another schema.org object type, but if the user does not override anything, the default is used. Therefore, the default cannot be changed.

There can be plenty of other reasons to add a "cannot be modified" property to a document, so the package is now public.

Visit the Project Page https://our.umbraco.org/projects/backoffice-extensions/default-value/ in the Umbraco Community

Install via nuget

		Install-Package SplatDev.Umbraco.Plugins.DefaultValue -Version 1.3.0.0

Specs
- Value Type: TEXT

feedback@splatdev.com is appreciated