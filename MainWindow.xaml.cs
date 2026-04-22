using System;
using System.Windows;
using Proyecto_GRRLN_expediente.db;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new MainWindowViewModel();

            // Configuración de navegaciones
            _viewModel.AbrirNuevoAvisoAction = () => new VistaCrearAviso().ShowDialog();
            _viewModel.AbrirExpedientesAction = () => new VistaConsultaGeneral().ShowDialog();
            _viewModel.AbrirNuevoCasoAction = () => new VistaNuevoExpediente().ShowDialog();
            _viewModel.AbrirConsultasAction = () => new VistaConsultas().ShowDialog();
            _viewModel.AbrirEstadisticasAction = () => new VistaEstadisticas().ShowDialog();
            _viewModel.AbrirAdministracionAction = () => new VentanaAdministrador().ShowDialog();
            
            _viewModel.CerrarSesionAction = () =>
            {
                DatabaseConnection.CloseConnection();
                Application.Current.Shutdown();
            };

            this.DataContext = _viewModel;
            this.Closing += (s, e) => _viewModel.RegistrarSalidaBitacora();

            // Al iniciar, cargamos la VistaInicio (el tablero de avisos)
            ContenedorPrincipal.Content = new VistaInicio();
        }
    }
}