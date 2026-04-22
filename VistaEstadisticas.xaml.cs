using System;
using System.Windows;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaEstadisticas : FluentWindow
    {
        public VistaEstadisticas()
        {
            InitializeComponent();
            
            var viewModel = new VistaEstadisticasViewModel();
            
            viewModel.CerrarVentanaAction = () =>
            {
                this.Close();
            };

            viewModel.MostrarDetalleAnimationAction = () =>
            {
                PanelGraficas.Visibility = Visibility.Collapsed;
                PanelDetalles.Visibility = Visibility.Visible;

                Storyboard sbMostrar = (Storyboard)this.FindResource("ShowPanelDetailsStoryboard");
                sbMostrar.Begin(this);
            };

            viewModel.OcultarDetalleAnimationAction = () =>
            {
                Storyboard sbOcultar = (Storyboard)this.FindResource("HidePanelDetailsStoryboard");
                EventHandler handler = null;
                handler = (s, ev) =>
                {
                    PanelDetalles.Visibility = Visibility.Collapsed;
                    PanelGraficas.Visibility = Visibility.Visible;
                    sbOcultar.Completed -= handler;
                };
                sbOcultar.Completed += handler;
                sbOcultar.Begin(this);
            };

            DataContext = viewModel;
        }
    }
}