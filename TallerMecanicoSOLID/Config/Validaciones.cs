using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TallerMecanicoSOLID.Configuracion
{
    /// <summary>
    /// Centraliza las validaciones y normalizaciones comunes
    /// de las entradas del usuario.
    /// </summary>
    public static class Validaciones
    {

        public static string SanitizarTexto(
            string textoIngresado,
            int cantidadMaximaDeCaracteres = 255)
        {
            // Si el texto está vacío, devolver una cadena vacía.
            if (string.IsNullOrWhiteSpace(textoIngresado))
            {
                return string.Empty;
            }

            // Quitar los espacios exteriores y normalizar los espacios internos.
            string textoLimpio = Regex.Replace(
                textoIngresado.Trim(),
                @"\s+",
                " ");

            // Devolver el texto limpio.
            // La longitud se comprobará mediante las validaciones del modelo.
            return textoLimpio;
        }

        public static bool EsIdValido(int id)
        {
            return id > 0;
        }

        public static bool SoloLetras(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            foreach (char caracter in texto)
            {
                if (!char.IsLetter(caracter) &&
                    !char.IsWhiteSpace(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool SoloNumeros(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            foreach (char caracter in texto)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool LetrasNumerosEspacios(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            foreach (char caracter in texto)
            {
                if (!char.IsLetterOrDigit(caracter) &&
                    !char.IsWhiteSpace(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValidarEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            email = email.Trim();

            if (email.Length > 120)
            {
                return false;
            }

            // Comprobación práctica de formato, no de existencia
            // real del buzón.
            const string patron =
                @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$";

            return Regex.IsMatch(
                email,
                patron,
                RegexOptions.CultureInvariant);
        }

        public static string Capitalizar(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            TextInfo cultura = CultureInfo.GetCultureInfo("es-BO").TextInfo;

            return cultura.ToTitleCase(texto.Trim().ToLowerInvariant());
        }

        public static bool ValidarTelefono(string? telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
            {
                return false;
            }

            telefono = telefono.Trim();

            if (telefono.Length > 20)
            {
                return false;
            }

            // Permite números, espacios, +, - y paréntesis.
            return Regex.IsMatch(
                telefono,
                @"^[0-9]+$");
        }


        public static bool ValidarCiNit(string ciNitIngresado)
        {
            // Comprobar que el CI/NIT no esté vacío.
            if (string.IsNullOrWhiteSpace(ciNitIngresado))
            {
                return false;
            }

            // Eliminar los espacios del principio y del final.
            string ciNitSinEspacios = ciNitIngresado.Trim();

            // Comprobar que no supere los 20 caracteres.
            if (ciNitSinEspacios.Length > 20)
            {
                return false;
            }

            // Comprobar que el CI/NIT contenga solamente números.
            if (!SoloNumeros(ciNitSinEspacios))
            {
                return false;
            }

            return true;
        }



        public static bool ValidarPlaca(string placaIngresada)
        {
            // Comprobar que la placa no esté vacía.
            if (string.IsNullOrWhiteSpace(placaIngresada))
            {
                return false;
            }

            // Quitar los espacios exteriores y convertir las letras a mayúsculas.
            string placaNormalizada = placaIngresada.Trim().ToUpperInvariant();

            // Comprobar que la placa tenga entre 3 y 4 números,
            // seguidos de exactamente 3 letras.
            bool tieneFormatoValido = Regex.IsMatch(
                placaNormalizada,
                @"^[0-9]{3,4}[A-Z]{3}$");

            if (!tieneFormatoValido)
            {
                return false;
            }

            return true;
        }


        public static string FormatearPlaca(string? placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                return string.Empty;
            }

            return placa.Trim().ToUpperInvariant();
        }

        public static bool ValidarMarca(string? marca)
        {
            if (string.IsNullOrWhiteSpace(marca))
            {
                return false;
            }

            marca = marca.Trim();

            return marca.Length <= 60 &&
                   Regex.IsMatch(
                       marca,
                       @"^[\p{L}\s\-]+$");
        }

        public static bool ValidarModelo(string? modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
            {
                return false;
            }

            modelo = modelo.Trim();

            return modelo.Length <= 60 &&
                   Regex.IsMatch(
                       modelo,
                       @"^[\p{L}0-9\s\-]+$");
        }

        public static bool ValidarAnio(int anio)
        {
            return anio >= 1960 &&
                   anio <= DateTime.Today.Year + 1;
        }

        public static bool ValidarKilometraje(int kilometraje)
        {
            return kilometraje >= 0 &&
                   kilometraje <= 2000000;
        }

        public static bool ValidarTipoServicio(string tipoServicioIngresado)
        {
            // Comprobar que se haya seleccionado un tipo de servicio.
            if (string.IsNullOrWhiteSpace(tipoServicioIngresado))
            {
                return false;
            }

            // Eliminar los espacios del principio y del final.
            string tipoServicioNormalizado = tipoServicioIngresado.Trim();

            // Aceptar únicamente los tres tipos definidos para el sistema.
            if (tipoServicioNormalizado.Equals(
                "preventivo",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (tipoServicioNormalizado.Equals(
                "correctivo",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (tipoServicioNormalizado.Equals(
                "revisión simple",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Si no coincide con ninguna opción, el tipo no es válido.
            return false;
        }

    }
}