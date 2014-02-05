using mitest.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página de elementos agrupados está documentada en http://go.microsoft.com/fwlink/?LinkId=234231

namespace mitest
{
    /// <summary>
    /// Página en la que muestra una colección de elementos agrupados.
    /// </summary>
    public sealed partial class GroupedItemsPage : mitest.Common.LayoutAwarePage
    {
        public GroupedItemsPage()
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
            // TODO: Crear un modelo de datos adecuado para el dominio del problema para reemplazar los datos de ejemplo
            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        /// <summary>
        /// Se invoca al hacer clic en un encabezado de grupo.
        /// </summary>
        /// <param name="sender">Objeto Button usado como encabezado del grupo seleccionado.</param>
        /// <param name="e">Datos de evento que describen cómo se inició el clic.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determinar el grupo al que representa la instancia de Button
            var group = (sender as FrameworkElement).DataContext;

            // Navegar a la página de destino adecuada y configurar la nueva página
            // al pasar la información requerida como parámetro de navegación
            this.Frame.Navigate(typeof(GroupDetailPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Se invoca al hacer clic en un elemento contenido en un grupo.
        /// </summary>
        /// <param name="sender">Objeto GridView (o ListView cuando la aplicación está en estado Snapped)
        /// que muestra el elemento en el que se hizo clic.</param>
        /// <param name="e">Datos de evento que describen el elemento en el que se hizo clic.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navegar a la página de destino adecuada y configurar la nueva página
            // al pasar la información requerida como parámetro de navegación
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }
    }
}
