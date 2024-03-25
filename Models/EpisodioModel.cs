using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U2_Topicos_Actividad_1.Models
{
    public class Episodio
    {
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string URLImagen { get; set; } = "";
        public int Duracion { get; set; } = 1;
        public int Numero { get; set; } = 1;
        public DateTime FechaEmision { get; set; }

    }
}
