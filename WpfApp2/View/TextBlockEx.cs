using HtmlAgilityPack;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp2.View
{
    class TextBlockEx : TextBlock
    {
        public static readonly DependencyProperty ParseHTMLProperty =
            DependencyProperty.Register(
                nameof(ParseHTML),
                typeof(bool),
                typeof(TextBlockEx),
                new FrameworkPropertyMetadata(false,
                    (o, e) =>
                    {
                        TextBlockEx tb = (TextBlockEx)o;
                        tb.Render(tb.Text, (bool)e.NewValue, tb.Emojis);
                    }));

        public bool ParseHTML
        {
            get => (bool)GetValue(ParseHTMLProperty);
            set => SetValue(ParseHTMLProperty, value);
        }

        public static readonly DependencyProperty EmojisProperty =
            DependencyProperty.Register(
                nameof(Emojis),
                typeof(IEnumerable<Emoji>),
                typeof(TextBlockEx),
                new FrameworkPropertyMetadata(Enumerable.Empty<Emoji>(),
                    (o, e) =>
                    {
                        TextBlockEx tb = (TextBlockEx)o;
                        tb.Render(tb.Text, tb.ParseHTML, (IEnumerable<Emoji>)e.NewValue);
                    }));

        public IEnumerable<Emoji> Emojis
        {
            get => (IEnumerable<Emoji>)GetValue(EmojisProperty);
            set => SetValue(EmojisProperty, value.ToList());
        }

        static TextBlockEx()
        {
            TextProperty.OverrideMetadata(typeof(TextBlockEx), new FrameworkPropertyMetadata(
                (string)TextProperty.GetMetadata(typeof(TextBlock)).DefaultValue,
                (o, e) =>
                {
                    TextBlockEx tb = (TextBlockEx)o;
                    tb.Render((string)e.NewValue, tb.ParseHTML, tb.Emojis);
                }));
            FontSizeProperty.OverrideMetadata(typeof(TextBlockEx), new FrameworkPropertyMetadata(
                (double)FontSizeProperty.GetMetadata(typeof(TextBlock)).DefaultValue,
                (o, e) => ((TextBlockEx)o).OnFontSizeChanged((double)e.NewValue)));
        }
        public TextBlockEx() : base() { }

        private void Render(string text, bool parseHTML, IEnumerable<Emoji> emojis)
        {
            Inlines.Clear();
            if (parseHTML)
            {
                Inlines.AddRange(htmlToInlines(text).SelectMany(inline =>
                {
                    Run run = inline as Run;
                    if (run == null) { return new List<Inline> { inline }; }
                    else { return stringToInlines(run.Text, emojis); }
                }));
            }
            else
            {
                Inlines.AddRange(stringToInlines(text, emojis));
            }
            OnFontSizeChanged(FontSize);
        }

        private void OnFontSizeChanged(double size)
        {
            foreach (Inline inline in Inlines)
            {
                var container = inline as InlineUIContainer;
                if (container != null)
                {
                    ((Image)container.Child).Height = size;
                }
            }
        }

        private static IEnumerable<Inline> stringToInlines(string text, IEnumerable<Emoji> emojis)
        {
            int head = 0;
            bool done = false;
            while (!done)
            {
                int colon1 = text.IndexOf(':', head);
                if (colon1 == -1)
                {
                    done = true;
                }
                else
                {
                    while (true)
                    {
                        int colon2 = text.IndexOf(':', colon1 + 1);
                        if (colon2 == -1)
                        {
                            done = true;
                            break;
                        }
                        Emoji e = emojis.FirstOrDefault(x => x.Shortcode == text.Substring(colon1 + 1, colon2 - colon1 - 1));
                        if (e != null)
                        {
                            yield return new Run(text.Substring(head, colon1 - head));
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(e.Url);
                            bi.EndInit();
                            Image image = new Image() { Source = bi };
                            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
                            yield return new InlineUIContainer(image);
                            head = colon2 + 1;
                            break;
                        }
                        else
                        {
                            colon1 = colon2;
                        }
                    }
                }
            }
            yield return new Run(text.Substring(head));
        }

        private static IEnumerable<Inline> htmlToInlines(string htmlText)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlText);
            var pnodes = doc.DocumentNode.ChildNodes;

            foreach (var (pnode, index) in pnodes.Select((v, i) => (v, i)))
            {
                if (index > 0)
                {
                    yield return new LineBreak();
                    yield return new LineBreak();
                }
                foreach (var child in pnode.ChildNodes)
                {
                    foreach (var inline in htmlSingleNodeToInlines(child))
                    {
                        yield return inline;
                    }
                }
            }
        }

        private static IEnumerable<Inline> htmlSingleNodeToInlines(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                yield return new Run(HtmlEntity.DeEntitize(node.InnerText));
            }
            switch (node.Name)
            {
                case "a":
                    var link = new Hyperlink();
                    link.NavigateUri = new Uri(node.Attributes["href"].Value);
                    link.RequestNavigate += (sender, eventArgs) => Process.Start(eventArgs.Uri.AbsoluteUri);
                    foreach (var child in node.ChildNodes)
                    {
                        link.Inlines.AddRange(htmlSingleNodeToInlines(child));
                    }
                    yield return link;
                    break;
                case "br":
                    yield return new LineBreak();
                    break;
                case "span":
                    foreach (var child in node.ChildNodes)
                    {
                        foreach (var inline in htmlSingleNodeToInlines(child))
                        {
                            yield return inline;
                        }
                    }
                    break;
            }
        }
    }
}
