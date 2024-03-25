using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U2_Topicos_Actividad_1.Views;

namespace U2_Topicos_Actividad_1.Models
{
    public class Temporada
    {
        public int Numero { get; set; } = 1;
        public string Imagen { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public DateTime FechaLanzamiento { get; set; }

        public List<Episodio> Episodios = new();
    }
}
