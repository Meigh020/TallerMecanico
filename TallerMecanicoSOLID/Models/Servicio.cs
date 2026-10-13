using System.ComponentModel.DataAnnotations;

namespace TallerMecanicoSOLID.Models
{
    public class Servicio : EntidadBase
    {    
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(120, ErrorMessage = "El nombre no puede superar 120 caracteres")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "Debe seleccionar un tipo")]
        public string Tipo { get; set; } = "";

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(255, ErrorMessage = "La descripción no puede superar 255 caracteres")]
        public string Descripcion { get; set; } = "";

        [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(1, 10000, ErrorMessage = "La duración debe ser mayor a 0")]
        public int DuracionEstimada { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una unidad de duración")]
        public string UnidadDuracion { get; set; } = "";

    }
}