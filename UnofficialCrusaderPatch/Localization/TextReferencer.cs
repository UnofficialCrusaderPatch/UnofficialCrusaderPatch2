using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Diagnostics;

namespace UnofficialCrusaderPatch
{
    static class TextReferencer
    {
        struct TextRef
        {
            string keyword;
            public string Keyword { get { return this.keyword; } }

            string reference;
            public string Reference { get { return this.reference; } }

            public TextRef(string keyword, string reference)
            {
                this.keyword = keyword;
                this.reference = reference;
            }
        }
        static List<TextRef> references = new List<TextRef>()
        {
            new TextRef("[ref]", @"https://git.io/fxyw1"),
            new TextRef("[ref2]", @"https://git.io/fxNMZ"),
        };

        public static void SetText(TextBlock tb, string ident)
        {
            string text = Localization.Get(ident);

            foreach (TextRef r in references)
            {
                int index = text.IndexOf(r.Keyword);
                if (index >= 0)
                {
                    string part = text.Remove(index);
                    if (!string.IsNullOrWhiteSpace(part))
                        tb.Inlines.Add(part);

                    Hyperlink hyperlink = new Hyperlink(new Run(r.Reference));
                    hyperlink.NavigateUri = new Uri(r.Reference);
                    hyperlink.RequestNavigate += (s, e) => Process.Start(r.Reference);
                    tb.Inlines.Add(hyperlink);



                    part = text.Substring(index + r.Keyword.Length);
                    if (!string.IsNullOrWhiteSpace(part))
                        tb.Inlines.Add(part);
                    return;
                }
            }

            tb.Text = text;
        }
    }
}
