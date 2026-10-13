using System.ComponentModel.DataAnnotations;


namespace TallerMecanicoSOLID.Models
{
    public class Cliente : EntidadBase
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar 100 caracteres")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El CI/NIT es obligatorio")]
        [StringLength(30, ErrorMessage = "El CI/NIT no puede superar 30 caracteres")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "El CI/NIT debe contener solo dígitos")]
        public string CiNit { get; set; } = "";

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(30, ErrorMessage = "El teléfono no puede superar 30 caracteres")]
        [RegularExpression(@"^[0-9+\-()\s]*$",
            ErrorMessage = "El teléfono contiene caracteres no permitidos")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(120, ErrorMessage = "El correo no puede superar 120 caracteres")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(255, ErrorMessage = "La dirección no puede superar 255 caracteres")]
        public string Direccion { get; set; } = "";

        public DateTime FechaRegistro { get; set; }
    }
}