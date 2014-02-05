using mitest.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página de detalles de elemento está documentada en http://go.microsoft.com/fwlink/?LinkId=234232

namespace mitest
{
    /// <summary>
    /// Página en la que se muestran los detalles de un único elemento contenido en un grupo y se permite realizar gestos para
    /// pasar a otros elementos pertenecientes al mismo grupo.
    /// </summary>
    public sealed partial class ItemDetailPage : mitest.Common.LayoutAwarePage
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Rellena la página con el contenido pasado durante la navegación. Cualquier estado guardado se
        /// proporciona también al crear de nuevo una página a partir de una sesión anterior.
        /// </summary>
        /// <param name="navigationParameter">Valor de parámetro pasado a
        /// <see cref="Frame.Navigate(Type, Object)"/> cuando se solicitó inicialmente esta página.
        /// </param>
        /// <param name="pageState">Diccionario del estado mantenido por esta página durante una sesión
        /// anterior. Será null la primera vez que se visite una página.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Permitir que el estado guardado de la página invalide el elemento inicial para mostrarse
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: Crear un modelo de datos adecuado para el dominio del problema para reemplazar los datos de ejemplo
            var item = SampleDataSource.GetItem((String)navigationParameter);
            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Items"] = item.Group.Items;
            this.flipView.SelectedItem = item;
        }

        /// <summary>
        /// Mantiene el estado asociado con esta página en caso de que se suspenda la aplicación o
        /// se descarte la página de la memoria caché de navegación. Los valores deben cumplir los requisitos
        /// de serialización de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Diccionario vacío para rellenar con un estado serializable.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (SampleDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;
        }


    }
    public class HtmlToRtfConverter
    {


        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        // Using a DependencyProperty as the backing store for Html.
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(HtmlToRtfConverter), 
                new PropertyMetadata("", OnHtmlChanged));

        private static void OnHtmlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            RichTextBlock parent = (RichTextBlock)sender;
            parent.Blocks.Clear();

            XmlDocument document = new XmlDocument();
            document.LoadXml((string)eventArgs.NewValue);

            ParseElement((XmlElement)(document.GetElementsByTagName("body")[0]), new RichTextBlockTextContainer(parent));
        }

        private static void ParseElement(XmlElement element, ITextContainer parent)
        {
            foreach (var child in element.ChildNodes)
            {
                if (child is Windows.Data.Xml.Dom.XmlText)
                {
                    if (string.IsNullOrEmpty(child.InnerText) ||
                        child.InnerText == "\n")
                    {
                        continue;
                    }

                    parent.Add(new Run { Text = child.InnerText });
                }
                else if (child is XmlElement)
                {
                    XmlElement e = (XmlElement)child;
                    switch (e.TagName.ToUpper())
                    {
                        case "P":
                            var paragraph = new Paragraph();
                            parent.Add(paragraph);
                            ParseElement(e, new ParagraphTextContainer(paragraph));
                            break;
                        case "STRONG":
                            var bold = new Bold();
                            parent.Add(bold);
                            ParseElement(e, new SpanTextContainer(bold));
                            break;
                        case "U":
                            var underline = new Underline();
                            parent.Add(underline);
                            ParseElement(e, new SpanTextContainer(underline));
                            break;
                        case "A":
                            ParseElement(e, parent);
                            break;
                        case "BR":
                            parent.Add(new LineBreak());
                            break;
                    }
                }


            }
        }

        private interface ITextContainer
        {
            void Add(Inline text);
            void Add(Block paragraph);
        }

        private sealed class SpanTextContainer : ITextContainer
        {
            private readonly Span _span;

            public SpanTextContainer(Span span)
            {
                _span = span;
            }

            public void Add(Inline text)
            {
                _span.Inlines.Add(text);
            }

            public void Add(Block paragraph)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class ParagraphTextContainer : ITextContainer
        {
            private readonly Paragraph _block;

            public ParagraphTextContainer(Paragraph block)
            {
                _block = block;
            }

            public void Add(Inline text)
            {
                _block.Inlines.Add(text);
            }

            public void Add(Block paragraph)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class RichTextBlockTextContainer : ITextContainer
        {
            private readonly RichTextBlock _richTextBlock;

            public RichTextBlockTextContainer(RichTextBlock richTextBlock)
            {
                _richTextBlock = richTextBlock;
            }

            public void Add(Inline text)
            {
                //throw new NotSupportedException();
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(text);

                _richTextBlock.Blocks.Add(paragraph);
            }

            public void Add(Block paragraph)
            {
                _richTextBlock.Blocks.Add(paragraph);
            }
        }

    }
}
