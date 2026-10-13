namespace TallerMecanicoSOLID.Models
{
    public abstract class EntidadBase
    {
        public int Id { get; set; }
        public bool Estado { get; set; } = true;
    }
}
