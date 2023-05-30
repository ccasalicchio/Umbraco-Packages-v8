# Default Value Property Editor

##### Umbraco v8.18.7+

- Creates a Data Type with a Default Value (used for properties that cannot be modified)

This was useful for a project that implements the schema.org types, so each page (article, news, foto) has a default value (Article, newsArticle, Photograph) that may be overridden by another schema.org object type, but if the user does not override anything, the default is used. Therefore, the default cannot be changed.

There can be plenty of other reasons to add a "cannot be modified" property to a document, so the package is now public.

![Imgur](https://i.imgur.com/LQTGJ5F.png)
![Imgur](https://i.imgur.com/1N41CY3.png)
![Imgur](https://i.imgur.com/PapRvV3.png)
![Imgur](https://i.imgur.com/2DWgL0J.png)

Visit the [Project Page](https://our.umbraco.org/projects/backoffice-extensions/default-value/) in the Umbraco Community

Install via nuget

		Install-Package SplatDev.Umbraco.Plugins.DefaultValue

##### Specs
- Value Type: TEXT


[Feedback](mailto:feedback@splatdev.com) is appreciated