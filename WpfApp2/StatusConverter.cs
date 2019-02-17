using HtmlAgilityPack;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;

namespace WpfApp2
{
    class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml((string)value);
            var pnodes = doc.DocumentNode.ChildNodes;

            var result = pnodes[0].ChildNodes.Select(ConvertSingleNode).ToList<Inline>();
            result.AddRange(pnodes.Skip(1).SelectMany(p =>
            {
                var list = new List<Inline> { new LineBreak(), new LineBreak() };
                list.AddRange(p.ChildNodes.Select(ConvertSingleNode));
                return list;
            }));
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Inline ConvertSingleNode(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                return new Run(node.InnerText);
            }
            switch (node.Name)
            {
                case "a":
                    var link = new Hyperlink();
                    link.NavigateUri = new Uri(node.Attributes["href"].Value);
                    foreach (var child in node.ChildNodes)
                    {
                        link.Inlines.Add(ConvertSingleNode(child));
                    }
                    return link;
                case "br":
                    return new LineBreak();
                case "span":
                    var span = new Span();
                    foreach (var child in node.ChildNodes)
                    {
                        span.Inlines.Add(ConvertSingleNode(child));
                    }
                    return span;
                default:
                    return null;
            }
        }
    }
}
