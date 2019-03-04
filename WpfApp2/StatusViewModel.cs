using HtmlAgilityPack;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace WpfApp2
{
    class StatusViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Status Status { get; private set; }
        public string StaticAvatarUrl { get; }
        public string DisplayName { get; }
        public List<Inline> ContentFlow { get; }

        public StatusViewModel(Status s)
        {
            Status = s;

            Status originalStatus = s.Reblog ?? s;

            StaticAvatarUrl = originalStatus.Account.StaticAvatarUrl;
            DisplayName = originalStatus.Account.DisplayName + (s.Reblog == null ? "" : $"(RT:{s.Account.AccountName})");
            ContentFlow = ConvertHtmlToFlow(s.Content).ToList();
        }

        #region Emoji Processing

        private static IEnumerable<Inline> UnfoldEmojis(string text, IEnumerable<Emoji> emojis)
        {
            List<(int i, Emoji e)> occs = FindEmojis(text, emojis)
                .OrderBy(x => x.Item1)
                .ToList();
            if (occs.Count() == 0)
            {
                yield return new Run(text);
            }
            else
            {
                yield return new Run(text.Substring(0, occs[0].i));
            }
            for (int i = 0; i < occs.Count(); i++)
            {
                (int i, Emoji e) occ = occs[i];

                yield return new InlineUIContainer(new Image { Source = new BitmapImage(new Uri(occ.e.StaticUrl)) });

                int runStartIndex = occ.i + occ.e.Shortcode.Length + 2;
                if (i + 1 < occs.Count())
                {
                    (int i, Emoji e) next = occs[i + 1];
                    yield return new Run(text.Substring(runStartIndex, next.i - runStartIndex));
                }
                else
                {
                    yield return new Run(text.Substring(runStartIndex));
                }
            }
        }

        private static IEnumerable<(int, Emoji)> FindEmojis(string text, IEnumerable<Emoji> emojis)
        {
            foreach (Emoji emoji in emojis)
            {
                string emojiString = ":" + emoji.Shortcode + ":";
                int searchStartIndex = 0;
                while (true)
                {
                    searchStartIndex = text.IndexOf(emojiString, searchStartIndex + 1);
                    if (searchStartIndex == -1)
                    {
                        break;
                    }
                    else
                    {
                        yield return (searchStartIndex, emoji);
                    }
                }
            }
        }

        #endregion
        #region HTML Processing
        private static IEnumerable<Inline> ConvertHtmlToFlow(string text)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
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
                    foreach (var inline in ConvertSingleNode(child))
                    {
                        yield return inline;
                    }
                }
            }
        }

        private static IEnumerable<Inline> ConvertSingleNode(HtmlNode node)
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
                        link.Inlines.AddRange(ConvertSingleNode(child));
                    }
                    yield return link;
                    break;
                case "br":
                    yield return new LineBreak();
                    break;
                case "span":
                    foreach (var child in node.ChildNodes)
                    {
                        foreach (var inline in ConvertSingleNode(child))
                        {
                            yield return inline;
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
