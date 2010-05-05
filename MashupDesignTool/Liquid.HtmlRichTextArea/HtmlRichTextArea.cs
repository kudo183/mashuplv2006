using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Text;
using System.Windows.Media.Imaging;

namespace Liquid
{
    public class HtmlRichTextArea : RichTextBox
    {
        #region Public Properties

        public Dictionary<string, HtmlRichTextAreaStyle> Styles { get; set; }

        #endregion

        #region Constructor

        public HtmlRichTextArea()
        {
            Styles = new Dictionary<string, HtmlRichTextAreaStyle>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the default styles H1, H2, H3 and Normal
        /// </summary>
        public void SetDefaultStyles()
        {
            Styles.Add("H1", new HtmlRichTextAreaStyle("H1", "Arial", 24, FontWeights.Bold, FontStyles.Normal, null, new SolidColorBrush(Colors.Black), HtmlElementDisplay.Block));
            Styles.Add("H2", new HtmlRichTextAreaStyle("H2", "Arial", 16, FontWeights.Bold, FontStyles.Normal, null, new SolidColorBrush(Colors.Black), HtmlElementDisplay.Block));
            Styles.Add("H3", new HtmlRichTextAreaStyle("H3", "Arial", 14, FontWeights.Bold, FontStyles.Normal, null, new SolidColorBrush(Colors.Black), HtmlElementDisplay.Block));
            Styles.Add("Normal", new HtmlRichTextAreaStyle("Normal", "Arial", 12, FontWeights.Normal, FontStyles.Normal, null, new SolidColorBrush(Colors.Black), HtmlElementDisplay.Inline));
        }

        /// <summary>
        /// Clears the content
        /// </summary>
        public void Clear()
        {
            this.Blocks.Clear();
        }

        /// <summary>
        /// Inserts a block of plain text
        /// </summary>
        /// <param name="text">Text to insert</param>
        public void Insert(string text)
        {
            Run run = new Run();
            run.Text = text;

            this.Selection.Insert(run);
        }

        /// <summary>
        /// Inserts a block of formatted text
        /// </summary>
        /// <param name="text">Text to insert</param>
        /// <param name="style">Text style</param>
        public void Insert(string text, HtmlRichTextAreaStyle style)
        {
            Run run = new Run();

            run.Text = text;
            style.ApplyToElement(run);

            this.Selection.Insert(run);
        }

        /// <summary>
        /// Inserts an image at the cursor
        /// </summary>
        /// <param name="url">Image Url</param>
        public void InsertImage(Uri url)
        {
            Image image = new Image()
            {
                Source = new BitmapImage(url),
                Stretch = Stretch.None
            };

            InsertElement(image);
        }

        /// <summary>
        /// Inserts any UIElement at the cursor
        /// </summary>
        /// <param name="element">Any element derived from UIElement</param>
        public void InsertElement(UIElement element)
        {
            InlineUIContainer imageContainer = new InlineUIContainer();
            imageContainer.Child = element;

            this.Selection.Insert(imageContainer);
        }

        /// <summary>
        /// Attempts to load a block of HTML into the control
        /// </summary>
        /// <param name="html">Valid XHTML</param>
        public void Load(string html)
        {
            XmlReader reader;
            string tag;
            string inlineStyle;
            string styleID;
            Stack<HtmlRichTextAreaStyle> styleStack = new Stack<HtmlRichTextAreaStyle>();
            HtmlRichTextAreaStyle currentStyle;

            Clear();
            reader = XmlReader.Create(new StringReader("<content>" + html.Replace("&nbsp;", "[RTB_HTML_SPACER]") + "</content>"));
            reader.Read();

            currentStyle = new HtmlRichTextAreaStyle(Styles["Normal"]);
            styleStack.Push(currentStyle);

            while (!reader.EOF)
            {
                tag = reader.Name.ToUpper();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        inlineStyle = reader.GetAttribute("style");
                        styleID = reader.GetAttribute("class");
                        currentStyle = styleStack.Peek();

                        if (styleID != null && Styles.ContainsKey(styleID))
                        {
                            currentStyle = new HtmlRichTextAreaStyle(Styles[styleID]);
                        }
                        if (inlineStyle != null)
                        {
                            currentStyle.FromInlineStyle(inlineStyle);
                        }

                        switch (tag)
                        {
                            case "H1":
                            case "H2":
                            case "H3":
                                if (Styles.ContainsKey(tag))
                                {
                                    currentStyle = new HtmlRichTextAreaStyle(Styles[tag]);
                                    if (inlineStyle != null)
                                    {
                                        currentStyle.FromInlineStyle(inlineStyle);
                                    }
                                }
                                CreateParagraph(currentStyle);
                                break;
                            case "IMG":
                                string src = string.Empty;
                                double width, height;
                                width = height = 20;

                                if (reader.GetAttribute("src") != null)
                                {
                                    src = reader.GetAttribute("src");
                                }
                                if (reader.GetAttribute("width") != null)
                                {
                                    width = double.Parse(reader.GetAttribute("width"));
                                }
                                if (reader.GetAttribute("height") != null)
                                {
                                    height = double.Parse(reader.GetAttribute("height"));
                                }

                                InsertImage(new Uri(src, UriKind.RelativeOrAbsolute));
                                //AddImageElement(src, width, height);
                                break;
                            case "BR":
                                break;
                            case "HR":
                                break;
                            case "P":
                            case "DIV":
                                CreateParagraph(currentStyle);
                                break;
                            case "SPAN":
                                currentStyle = new HtmlRichTextAreaStyle(currentStyle);
                                currentStyle.Display = HtmlElementDisplay.Inline;
                                break;
                            case "A":
                                string url = string.Empty;

                                if (reader.GetAttribute("href") != null)
                                {
                                    url = reader.GetAttribute("href");
                                }
                                AddHyperlinkElement(reader.ReadInnerXml(), url, styleStack.Peek());
                                break;
                            case "B":
                            case "STRONG":
                                currentStyle = new HtmlRichTextAreaStyle(currentStyle);
                                currentStyle.FontWeight = FontWeights.Bold;
                                break;
                            case "I":
                            case "ITALIC":
                                currentStyle = new HtmlRichTextAreaStyle(currentStyle);
                                currentStyle.FontStyle = FontStyles.Italic;
                                break;
                            case "U":
                            case "UNDERLINE":
                                currentStyle = new HtmlRichTextAreaStyle(currentStyle);
                                currentStyle.TextDecoration = TextDecorations.Underline;
                                break;
                            case "UL":
                                break;
                            case "OL":
                                break;
                            case "LI":
                                break;
                        }

                        if (tag != "IMG" && tag != "BR" && tag != "CONTENT")
                        {
                            styleStack.Push(currentStyle);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (tag != "CONTENT")
                        {
                            styleStack.Pop();
                        }

                        switch (tag)
                        {
                            case "H1":
                            case "H2":
                            case "H3":
                            case "P":
                            case "DIV":
                                break;
                        }
                        break;
                    case XmlNodeType.Text:
                        AddTextElement(reader.Value.Replace("[RTB_HTML_SPACER]", " "), styleStack.Peek());
                        break;
                }

                reader.Read();
            }
        }

        private void AddImageElement(string url, double width, double height)
        {
            Image MyImage = new Image();
            MyImage.Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            MyImage.Height = height;
            MyImage.Width = width;
            InlineUIContainer MyUI = new InlineUIContainer();
            MyUI.Child = MyImage;
            if (Blocks.Count > 0)
            {
                ((Paragraph)Blocks[Blocks.Count - 1]).Inlines.Add(MyUI);
            }
        }

        /// <summary>
        /// Saves the content as HTML
        /// </summary>
        /// <returns>The control content as HTML</returns>
        public string Save()
        {
            StringBuilder results = new StringBuilder();

            foreach (Block block in Blocks)
            {
                if (block is Paragraph)
                {
                    results.Append(SerializeBlock((Paragraph)block));
                }
            }

            return results.ToString();
        }

        public void SetAlignment(TextAlignment alignment)
        {
            foreach (Block block in Blocks)
            {
                block.TextAlignment = alignment;
            }
        }

        #endregion

        #region Private Methods

        private void CreateParagraph(HtmlRichTextAreaStyle style)
        {
            Paragraph p = new Paragraph();
            Blocks.Add(p);
            style.ApplyToElement(p);
        }

        private string SerializeBlock(Paragraph block)
        {
            StringBuilder results = new StringBuilder();
            HtmlRichTextAreaStyle style;
            HtmlRichTextAreaStyle lastStyle = Styles["Normal"];
            string css = string.Empty;
            string attributes = string.Empty;
            string tag = "p";
            Hyperlink link;
            Run span;

            style = GetStyle(block);
            if (style != null)
            {
                lastStyle = style;

                if (style.ID == "H1" || style.ID == "H2" || style.ID == "H3")
                {
                    tag = style.ID.ToLower();
                }
            }
            results.Append("<" + tag + ">");

            foreach (Inline inline in block.Inlines)
            {
                style = GetStyle(inline);
                if (style == null)
                {
                    style = new HtmlRichTextAreaStyle("custom", inline.FontFamily.ToString(), inline.FontSize, inline.FontWeight, inline.FontStyle, inline.TextDecorations, inline.Foreground, HtmlElementDisplay.Inline);
                }

                css = style.ToInlineCSS(lastStyle);
                lastStyle = style;

                if (css.Length > 0)
                {
                    attributes = " style=\"" + css + "\"";
                }

                switch (inline.GetType().ToString())
                {
                    case "System.Windows.Documents.Run":
                        span = (Run)inline;
                        results.Append("<span" + attributes + ">" + span.Text + "</span>" + Environment.NewLine);
                        break;
                    case "System.Windows.Documents.Hyperlink":
                        link = (Hyperlink)inline;
                        results.Append("<a" + attributes + " href=\"" + link.NavigateUri.ToString() + "\">" + ((Run)link.Inlines[0]).Text + "</a>" + Environment.NewLine);
                        break;
                }
            }

            results.Append("</" + tag + ">" + Environment.NewLine);

            return results.ToString();
        }

        private HtmlRichTextAreaStyle GetStyle(TextElement element)
        {
            HtmlRichTextAreaStyle result = null;

            foreach (KeyValuePair<string, HtmlRichTextAreaStyle> style in Styles)
            {
                if (element.FontFamily.ToString() == style.Value.FontFamily && element.FontSize == style.Value.FontSize.Value &&
                    element.FontStyle == style.Value.FontStyle && element.FontWeight == style.Value.FontWeight &&
                    Utility.CompareBrush(element.Foreground, style.Value.Foreground))
                {
                    result = style.Value;
                    break;
                }
            }

            return result;
        }

        private void AddTextElement(string text, HtmlRichTextAreaStyle style)
        {
            Run inline;

            inline = new Run();
            inline.Text = text;
            style.ApplyToElement(inline);

            if (Blocks.Count > 0)
            {
                ((Paragraph)Blocks[Blocks.Count - 1]).Inlines.Add(inline);
            }
        }

        private void AddHyperlinkElement(string text, string url, HtmlRichTextAreaStyle style)
        {
            Paragraph p;
            Hyperlink inline;

            switch (style.Display)
            {
                case HtmlElementDisplay.Block:
                    p = new Paragraph();
                    Blocks.Add(p);
                    style.ApplyToElement(p);
                    break;
            }

            inline = new Hyperlink();
            //try
            //{
                inline.NavigateUri = new Uri(url, UriKind.RelativeOrAbsolute);
            //}
            //catch
            //{
            //    inline.NavigateUri = new Uri(url, UriKind.Relative);
            //}
            inline.Inlines.Add(text);
            style.ApplyToElement(inline);

            if (Blocks.Count > 0)
            {
                ((Paragraph)Blocks[Blocks.Count - 1]).Inlines.Add(inline);
            }
        }

        #endregion
    }
}
