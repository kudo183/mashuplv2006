// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.IO;
using PivotServerTools;

namespace PivotServer
{
    /// <summary>
    /// Handle a request for any CXML file. See the associated entry in web.config
    /// This handler finds all implementations of CollectionFactoryBase in any assembly in the bin folder.
    /// To add your own collection using this method, add a class that implements CollectionFactoryBase
    ///  into the CollectionFactories assembly.
    /// </summary>
    public class CxmlHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string url = context.Request["url"];
            string name = context.Request["Name"];
            string dataElement = context.Request["DataElement"];
            string title = context.Request["Title"];
            string link = context.Request["Link"];
            string des = context.Request["Description"];
            string imageUrl = context.Request["ImageUrl"];

            string facets = context.Request["Facets"];
            string[] str = facets.Split(',');
            if (str.Length % 2 == 1)
                return;
            
            Collection collection = new Collection();
            collection.Name = name;

            WebClient web = new WebClient();
            StreamReader sr = new StreamReader(web.OpenRead(url));
            string s = sr.ReadToEnd();
            sr.Close();

            XDocument doc = XDocument.Parse(s);
            string strTitle, strLink, strDes, strImageUrl;
            foreach (XElement element in doc.Descendants(dataElement))
            {
                strTitle = (element.Element(title) != null) ? element.Element(title).Value : "";
                strDes = (element.Element(des) != null) ? element.Element(des).Value : "";
                strLink = (element.Element(link) != null) ? element.Element(link).Value : "";
                strImageUrl = (element.Element(imageUrl) != null) ? element.Element(imageUrl).Value : "";
                collection.AddItem(strTitle, strLink, strDes, new ItemImage(new Uri(strImageUrl, UriKind.RelativeOrAbsolute)), MakeFacet(str, element).ToArray());
                //string title, des, link, pub, href, content, image, price;
                //title = (element.Element("title") != null) ? element.Element("title").Value : "";
                //des = (element.Element("description") != null) ? element.Element("description").Value : "";
                //link = (element.Element("link") != null) ? element.Element("link").Value : "";
                //href = (element.Element("href") != null) ? element.Element("href").Value : "";
                //content = (element.Element("content") != null) ? element.Element("content").Value : "";
                //image = (element.Element("image") != null) ? element.Element("image").Value : "";
                //pub = (element.Element("pubDate") != null) ? element.Element("pubDate").Value : "";
                //price = (element.Element("price") != null) ? element.Element("price").Value : "";                
                //collection.AddItem(title, href, content, new ItemImage(new Uri(image, UriKind.RelativeOrAbsolute)),
                //    new Facet("Title", title),
                //    new Facet("Price", price));

            }

            PivotHttpHandlers.ServeCxml(context, collection);
        }
        private List<Facet> MakeFacet(string[] mapping, XElement element)
        {
            List<Facet> result = new List<Facet>();
            for (int i = 0; i < mapping.Length; i += 2)
            {
                XElement e = element.Element(mapping[i + 1]);
                if (e == null)
                    continue;
                result.Add(new Facet(mapping[i], e.Value));

            }
            return result;
        }
        public bool IsReusable
        {
            get { return true; }
        }
    }

    /*
        //You may use the steps above to create your own collections using the provided generic
        // CXML handler. Alternatively, if you want to directly implement your own specific CXML
        // handler, uncomment this sample implementation and add a corresponding entry in the
        // handlers section of web.config to use this handler. E.g.
        //  <add name="MyCXML" verb="GET" path="my.cxml" type="PivotServer.MyCxmlHandler"/>
        public class MyCxmlHandler : IHttpHandler
        {
            public void ProcessRequest(HttpContext context)
            {
                Collection collection = new Collection();
                collection.Name = "My specific collection";
                for (int i = 0; i < 10; ++i)
                {
                    collection.AddItem(i.ToString(), null, null, null);
                }

                PivotHttpHandlers.ServeCxml(context, collection);
            }

            public bool IsReusable
            {
                get { return true; }
            }
        }
    */


    public class DzcHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            PivotHttpHandlers.ServeDzc(context);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }


    public class ImageTileHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            PivotHttpHandlers.ServeImageTile(context);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }


    public class DziHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            PivotHttpHandlers.ServeDzi(context);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }


    public class DeepZoomImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            PivotHttpHandlers.ServeDeepZoomImage(context);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }

}
