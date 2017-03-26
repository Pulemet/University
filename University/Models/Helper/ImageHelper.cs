using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace University.Models
{
    public static class ImageHelper
    {
        public static MvcHtmlString Image(this HtmlHelper helper, string url, string altText, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("img");
            builder.Attributes.Add("src", url);
            builder.Attributes.Add("alt", altText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }
    }
}