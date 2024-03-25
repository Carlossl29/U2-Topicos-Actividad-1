using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using U2_Topicos_Actividad_1.Models;

namespace U2_Topicos_Actividad_1.ViewModels
{
    public enum Ventanas { Temporadas, Episodios, AgregarTemporada, EditarTemporada, AgregarEpisodio, EditarEpisodio, EliminarTemporada, EliminarEpisodio }
    public class GuiaViewModel: INotifyPropertyChanged
    {
        public List<Temporada> Temporadas { get; set; } = new();
        public ObservableCollection<Temporada> ListaTemporadas { get; set; } = new();
        public ObservableCollection<Episodio> ListaEpisodios { get; set; } = new();

        public ICommand AgregarTemporadaCommand { get; set; }
        public ICommand AgregarEpisodioCommand { get; set; }
        public ICommand EliminarTemporadaCommand { get; set; }
        public ICommand EliminarEpisodioCommand { get; set; }
        public ICommand EditarTemporadaCommand { get; set; }
        public ICommand EditarEpisodioCommand { get; set; }
        public ICommand VerTemporadasCommand { get; set; }
        public ICommand VerEpisodiosCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public ICommand OrdenarTemporadaAscendenteCommand { get; set; }
        public ICommand OrdenarEpisodiosAscendenteCommand { get; set; }
        public ICommand OrdenarTemporadasDescendenteCommand { get; set; }
        public ICommand OrdenarEpisodiosDescendenteCommand { get; set; }
        public ICommand BuscarTemporadaCommand { get; set; }
        public ICommand BuscarEpisodioCommand { get; set; }
        public Episodio Episodio { get; set; } = null!;
        public Temporada Temporada { get; set; } = null!;
        public string Error { get; set; } = "";
        public Ventanas Ventana { get; set; } = Ventanas.Temporadas;
        public string BusquedaTemporada { get; set; } = "";
        public string BusquedaEpisodio { get; set; } = "";

        public GuiaViewModel()
        {
            AgregarTemporadaCommand = new RelayCommand(AgregarTemporada);
            AgregarEpisodioCommand = new RelayCommand(AgregarEpisodio);
            EliminarTemporadaCommand = new RelayCommand(EliminarTemporada);
            EliminarEpisodioCommand = new RelayCommand(EliminarEpisodio);
            EditarTemporadaCommand = new RelayCommand(EditarTemporada);
            EditarEpisodioCommand = new RelayCommand(EditarEpisodio);
            VerTemporadasCommand = new RelayCommand(VerTemporadas);
            VerEpisodiosCommand = new RelayCommand(VerEpisodios);
            CambiarVistaCommand = new RelayCommand<Ventanas>(CambiarVista);
            OrdenarTemporadaAscendenteCommand = new RelayCommand(ActualizarTemporadas);
            OrdenarEpisodiosAscendenteCommand = new RelayCommand(ActualizarEpisodios);
            OrdenarTemporadasDescendenteCommand = new RelayCommand(OrdenarTemporadasDescendente);
            OrdenarEpisodiosDescendenteCommand = new RelayCommand(OrdenarEpisodiosDescendente);
            BuscarTemporadaCommand = new RelayCommand(BuscarTemporada);
            BuscarEpisodioCommand = new RelayCommand(BuscarEpisodio);
            DeserializarXML();
        }

        int posAnterior = 0;
        private void CambiarVista(Ventanas ventana)
        {
            Ventana = ventana;

            if (Ventana == Ventanas.AgregarTemporada)
            {
                Error = "";
                Temporada = new()
                {
                    FechaLanzamiento = DateTime.Now
                };

                Actualizar(nameof(Temporada));
            }


            if (Ventana == Ventanas.AgregarEpisodio)
            {
                Error = "";
                Episodio = new()
                {
                    FechaEmision = DateTime.Now
                };

                Actualizar(nameof(Episodio));
            }

            if (Ventana == Ventanas.EditarTemporada)
            {
                Error = "";
                Temporada temporadacopia = new()
                {
                    Numero = Temporada.Numero,
                    FechaLanzamiento = Temporada.FechaLanzamiento,
                    Descripcion = Temporada.Descripcion,
                    Episodios = Temporada.Episodios,
                    Imagen = Temporada.Imagen
                };

                posAnterior = Temporadas.IndexOf(Temporada);
                Temporada = temporadacopia;
                Actualizar(nameof(Temporada));
            }

            if (Ventana == Ventanas.EditarEpisodio)
            {
                Error = "";
                Episodio episodiocopia = new()
                {
                    Nombre = Episodio.Nombre,
                    Duracion = Episodio.Duracion,
                    Numero = Episodio.Numero,
                    FechaEmision = Episodio.FechaEmision,
                    URLImagen = Episodio.URLImagen,
                    Descripcion = Episodio.Descripcion
                };
                posAnterior = Temporada.Episodios.IndexOf(Episodio);
                Episodio = episodiocopia;
                Actualizar(nameof(Episodio));
            }

            Actualizar(nameof(Error));
            Actualizar(nameof(Ventana));
        }

        private void VerEpisodios()
        {
            ActualizarEpisodios();
            Ventana = Ventanas.Episodios;
            Actualizar(nameof(Temporada));
            Actualizar(nameof(Ventana));
        }

        private void VerTemporadas()
        {
            ActualizarTemporadas();
            Actualizar(nameof(Temporada));
            Ventana = Ventanas.Temporadas;
            Actualizar(nameof(Ventana));
        }

        private void EditarEpisodio()
        {
            Error = "";
            if (Episodio != null)
            {
                if (string.IsNullOrWhiteSpace(Episodio.Nombre))
                {
                    Error += "\nEscriba el nombre del episodio.";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Descripcion))
                {
                    Error += "\nEscriba la descripción del episodio.";
                }
                if (string.IsNullOrWhiteSpace(Episodio.URLImagen) || (!Episodio.URLImagen.StartsWith("http") && !Episodio.URLImagen.EndsWith(".jpg")))
                {
                    Error += "\nEscriba la dirección de una imagen en JPEG.";
                }
                if (Episodio.Numero <= 0)
                {
                    Error += "\nVerifique el número de episodio.";
                }
                if (Temporada.Episodios.Exists(x => x.Numero == Episodio.Numero) && Episodio.Numero != Temporada.Episodios[posAnterior].Numero)
                {
                    Error += "\nEl episodio ya existe.";
                }
                if (Episodio.Duracion <= 0)
                {
                    Error += "\nVerifique la duración del episodio.";
                }
                if (Episodio.FechaEmision > DateTime.Now)
                {
                    Error += "\nLa fecha de emisión no es válida.";
                }
                if (DateOnly.FromDateTime(Episodio.FechaEmision) < DateOnly.FromDateTime(Temporada.FechaLanzamiento))
                {
                    Error += "\nLa fecha de emisión es anterior al lanzamiento de la temporada.";
                }

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Error = "";
                Temporada.Episodios.RemoveAt(posAnterior);
                Temporada.Episodios.Add(Episodio);
                ActualizarEpisodios();
                SerializarXML();
                Ventana = Ventanas.Episodios;
                Actualizar(nameof(Ventana));
            }
        }

        private void EditarTemporada()
        {
            Error = "";
            if (Temporada != null)
            {
                if (Temporada.Numero <= 0)
                {
                    Error += "\nVerifique el número de la temporada.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Imagen) || (!Temporada.Imagen.StartsWith("http") && !Temporada.Imagen.EndsWith(".jpg")))
                {
                    Error += "\nEscriba la dirección de una imagen en JPEG.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Descripcion))
                {
                    Error += "\nEscriba la descripción de la temporada.";
                }
                if (Temporada.FechaLanzamiento > DateTime.Now)
                {
                    Error += "\nLa fecha de lanzamiento no es válida.";
                }
                if (Temporadas.Exists(x => x.Numero == Temporada.Numero) && Temporada.Numero != Temporadas[posAnterior].Numero)
                {
                    Error += "\nLa temporada ya existe.";
                }
                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Error = "";
                Temporadas.RemoveAt(posAnterior);
                Temporadas.Add(Temporada);
                ActualizarTemporadas();
                SerializarXML();
                Ventana = Ventanas.Temporadas;
                Actualizar(nameof(Ventana));
            }
        }

        private void EliminarEpisodio()
        {
            if (Episodio != null)
            {
                Temporada.Episodios.Remove(Episodio);
                SerializarXML();
                VerEpisodios();
            }
        }

        private void EliminarTemporada()
        {
            if (Temporada != null)
            {
                Temporadas.Remove(Temporada);
                SerializarXML();
                VerTemporadas();
            }
        }

        private void AgregarEpisodio()
        {
            Error = "";
            if (Episodio != null)
            {
                if (string.IsNullOrWhiteSpace(Episodio.Nombre))
                {
                    Error += "\nEscriba el nombre del episodio.";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Descripcion))
                {
                    Error += "\nEscriba la descripción del episodio.";
                }
                if (string.IsNullOrWhiteSpace(Episodio.URLImagen) || (!Episodio.URLImagen.StartsWith("http") && !Episodio.URLImagen.EndsWith(".jpg")))
                {
                    Error += "\nEscriba la dirección de una imagen en JPEG.";
                }
                if (Episodio.Numero <= 0)
                {
                    Error += "\nVerifique el número de episodio.";
                }
                if (Temporada.Episodios.Exists(x => x.Numero == Episodio.Numero))
                {
                    Error += "\nEl episodio ya existe.";
                }
                if (Episodio.Duracion <= 0)
                {
                    Error += "\nVerifique la duración del episodio.";
                }
                if (Episodio.FechaEmision > DateTime.Now)
                {
                    Error += "\nLa fecha de emisión no es válida.";
                }
                if (DateOnly.FromDateTime(Episodio.FechaEmision) < DateOnly.FromDateTime(Temporada.FechaLanzamiento))
                {
                    Error += "\nLa fecha de emisión es anterior al lanzamiento de la temporada.";
                }

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Error = "";
                Temporada.Episodios.Add(Episodio);
                ActualizarEpisodios();
                SerializarXML();
                Ventana = Ventanas.Episodios;
                Actualizar(nameof(Ventana));
            }
        }

        private void AgregarTemporada()
        {
            Error = "";
            if (Temporada != null)
            {
                if (Temporada.Numero <= 0)
                {
                    Error += "\nVerifique el número de la temporada.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Imagen) || (!Temporada.Imagen.StartsWith("http") && !Temporada.Imagen.EndsWith(".jpg")))
                {
                    Error += "\nEscriba la dirección de una imagen en JPEG.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Descripcion))
                {
                    Error += "\nEscriba la descripción de la temporada.";
                }
                if (Temporada.FechaLanzamiento > DateTime.Now)
                {
                    Error += "\nLa fecha de lanzamiento no es válida.";
                }
                if(Temporadas.Exists(x=>x.Numero==Temporada.Numero))
                {
                    Error += "\nLa temporada ya existe.";
                }
                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Error = "";
                Temporadas.Add(Temporada);
                ActualizarTemporadas();
                SerializarXML();
                Ventana = Ventanas.Temporadas;
                Actualizar(nameof(Ventana));
            }
        }

        private void ActualizarTemporadas()
        {
            List<Temporada> temporadas = Temporadas.OrderBy(x => x.Numero).ToList();
            ListaTemporadas.Clear();
            foreach (Temporada temporada in temporadas)
            {
                ListaTemporadas.Add(temporada);
            }
        }

        private void ActualizarEpisodios()
        {
            List<Episodio> episodios = Temporada.Episodios.OrderBy(x=>x.Numero).ToList();
            ListaEpisodios.Clear();
            foreach (Episodio episodio in episodios)
            {
                ListaEpisodios.Add(episodio);
            }
        }

        private void OrdenarTemporadasDescendente()
        {
            List<Temporada> temporadas = Temporadas.OrderByDescending(x => x.Numero).ToList();
            ListaTemporadas.Clear();
            foreach (Temporada temporada in temporadas)
            {
                ListaTemporadas.Add(temporada);
            }
        }

        private void OrdenarEpisodiosDescendente()
        {
            List<Episodio> episodios = Temporada.Episodios.OrderByDescending(x => x.Numero).ToList();
            ListaEpisodios.Clear();
            foreach (Episodio episodio in episodios)
            {
                ListaEpisodios.Add(episodio);
            }
        }

        private void BuscarTemporada()
        {
            if (string.IsNullOrWhiteSpace(BusquedaTemporada))
            {
                ActualizarTemporadas();
            }
            else
            {
                List<Temporada> temporadas;
                int numero;
                if (int.TryParse(BusquedaTemporada, out numero))
                {
                    temporadas = Temporadas.Where(x=>x.Numero == numero).ToList();
                    ListaTemporadas.Clear();
                    foreach (Temporada temporada in temporadas)
                    {
                        ListaTemporadas.Add(temporada);
                    }
                }
                else
                {
                    temporadas = Temporadas.Where(x=>x.Descripcion.ToLower().Contains(BusquedaTemporada.ToLower())).ToList();
                    ListaTemporadas.Clear();
                    foreach (Temporada temporada in temporadas)
                    {
                        ListaTemporadas.Add(temporada);
                    }
                }
            }
        }

        private void BuscarEpisodio()
        {
            if (string.IsNullOrWhiteSpace(BusquedaEpisodio))
            {
                ActualizarEpisodios();
            }
            else
            {
                List<Episodio> episodios;
                int numero;
                if (int.TryParse(BusquedaEpisodio, out numero))
                {
                    episodios = Temporada.Episodios.Where(x=>x.Numero==numero).ToList();
                    ListaEpisodios.Clear();
                    foreach (Episodio episodio in episodios)
                    {
                       ListaEpisodios.Add(episodio);
                    }
                }
                else
                {
                    episodios = Temporada.Episodios.Where(x => x.Descripcion.ToLower().Contains(BusquedaEpisodio.ToLower()) || x.Nombre.ToLower().Contains(BusquedaEpisodio.ToLower())).ToList();
                    ListaEpisodios.Clear();
                    foreach (Episodio episodio in episodios)
                    {
                        ListaEpisodios.Add(episodio);
                    }
                }
            }
        }

        private void Actualizar(string? nombre)
        {
            PropertyChanged?.Invoke(this, new(nombre));
        }

        void SerializarXML()
        {
            FileStream archivo = File.Create("guia.xml");
            XmlSerializer xml = new(typeof(List<Temporada>));
            xml.Serialize(archivo, Temporadas);
            archivo.Close();
        }

        void DeserializarXML()
        {
            if (File.Exists("guia.xml"))
            {
                FileStream archivo = File.OpenRead("guia.xml");
                XmlSerializer xml = new(typeof(List<Temporada>));
                object? temporadas = xml.Deserialize(archivo);
                archivo.Close();

                if (temporadas != null)
                {
                    Temporadas = (List<Temporada>)temporadas;
                    ActualizarTemporadas();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
