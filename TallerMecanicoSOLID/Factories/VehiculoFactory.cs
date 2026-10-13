using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Factories
{
    public class VehiculoFactory : EntidadFactory<Vehiculo>
    {
        public override Vehiculo Crear()
        {
            return new Vehiculo
            {
                Marca = "Marca del vehículo",
                Modelo = "Modelo del vehículo",
                Anio = 2020,
                Color = "Color del vehículo",
                Placa = "ABC-1234",
                Tipo = "Tipo de vehículo"
            };
        }
    }
}
