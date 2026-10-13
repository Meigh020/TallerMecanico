using System.ComponentModel.DataAnnotations;

namespace TallerMecanicoSOLID.Models
{
    public class Vehiculo : EntidadBase
    {
        [Required(ErrorMessage = "La placa es obligatoria")]
        [StringLength(7, MinimumLength = 6,
            ErrorMessage = "La placa debe tener entre 6 y 7 caracteres")]
        public string Placa { get; set; } = "";

        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50,
            ErrorMessage = "La marca no puede superar 50 caracteres")]
        public string Marca { get; set; } = "";

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(50,
            ErrorMessage = "El modelo no puede superar 50 caracteres")]
        public string Modelo { get; set; } = "";

        [Range(1901, 2100,
            ErrorMessage = "Ingrese un año válido")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30,
            ErrorMessage = "El color no puede superar 30 caracteres")]
        public string Color { get; set; } = "";

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public string Tipo { get; set; } = "";

        [Range(0, int.MaxValue,
            ErrorMessage = "El kilometraje no puede ser negativo")]
        public int Kilometraje { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un cliente")]
        public int ClienteId { get; set; }

        public string ClienteNombre { get; set; } = "";

        public DateTime CreadoEn { get; set; }
    }
}