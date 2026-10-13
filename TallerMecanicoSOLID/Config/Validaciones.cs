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
            string? texto,
            int maxLongitud = 255)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            // Normaliza espacios y elimina espacios exteriores.
            string limpio = Regex.Replace(
                texto.Trim(),
                @"\s+",
                " ");

            // No truncamos el texto: la validación de longitud
            // debe rechazar entradas demasiado largas.
            return limpio;
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
                @"^[0-9+()\-\s]+$");
        }

        public static bool ValidarCiNit(string? ciNit)
        {
            if (string.IsNullOrWhiteSpace(ciNit))
            {
                return false;
            }

            ciNit = ciNit.Trim();

            return ciNit.Length <= 20 &&
                   SoloNumeros(ciNit);
        }

        public static bool ValidarPlaca(string? placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                return false;
            }

            placa = placa.Trim().ToUpperInvariant();

            // Formato definido para este proyecto:
            // 3 o 4 números seguidos de 3 letras.
            return Regex.IsMatch(
                placa,
                @"^[0-9]{3,4}[A-Z]{3}$");
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
            return anio >= 1901 &&
                   anio <= DateTime.Today.Year + 1;
        }

        public static bool ValidarKilometraje(int kilometraje)
        {
            return kilometraje >= 0 &&
                   kilometraje <= 2000000;
        }
    }
}