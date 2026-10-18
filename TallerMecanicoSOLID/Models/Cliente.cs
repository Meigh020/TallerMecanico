using System.ComponentModel.DataAnnotations;


namespace TallerMecanicoSOLID.Models
{
    public class Cliente : EntidadBase
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar 50 caracteres")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El CI/NIT es obligatorio")]
        [StringLength(20, ErrorMessage = "El CI/NIT no puede superar 20 caracteres")]
        [RegularExpression(@"^[0-9]+(-[A-Z]{2})?$", ErrorMessage = "Use solo números y, si corresponde, una extensión de dos letras. Ejemplo: 8815391-CB")]
        public string CiNit { get; set; } = "";

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(8, ErrorMessage = "El teléfono no puede superar 8 caracteres")]
        [RegularExpression(@"^[0-9+\-()\s]*$",
            ErrorMessage = "El teléfono contiene caracteres no permitidos")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(50, ErrorMessage = "El correo no puede superar 50 caracteres")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(255, ErrorMessage = "La dirección no puede superar 255 caracteres")]
        public string Direccion { get; set; } = "";

        public DateTime FechaRegistro { get; set; }
    }
}