using System.Text.RegularExpressions;

namespace Trasformazioni.Helpers
{
    /// <summary>
    /// Helper per la validazione e normalizzazione delle targhe italiane
    /// </summary>
    public static class TargaValidator
    {
        // Formato targa italiana standard: 2 lettere + 3 numeri + 2 lettere (es. AB123CD)
        private static readonly Regex TargaItalianaRegex = new Regex(
            @"^[A-Z]{2}[0-9]{3}[A-Z]{2}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Formato targa vecchio: 2 lettere + 4-6 numeri (es. TO12345)
        private static readonly Regex TargaVecchiaRegex = new Regex(
            @"^[A-Z]{2}[0-9]{4,6}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        /// <summary>
        /// Valida se una targa è nel formato italiano corretto
        /// </summary>
        /// <param name="targa">Targa da validare</param>
        /// <returns>True se la targa è valida, False altrimenti</returns>
        public static bool IsTargaValida(string? targa)
        {
            if (string.IsNullOrWhiteSpace(targa))
                return false;

            var targaNormalizzata = NormalizzaTarga(targa);

            return TargaItalianaRegex.IsMatch(targaNormalizzata) ||
                   TargaVecchiaRegex.IsMatch(targaNormalizzata);
        }

        /// <summary>
        /// Normalizza la targa rimuovendo spazi e convertendo in maiuscolo
        /// </summary>
        /// <param name="targa">Targa da normalizzare</param>
        /// <returns>Targa normalizzata</returns>
        public static string NormalizzaTarga(string? targa)
        {
            if (string.IsNullOrWhiteSpace(targa))
                return string.Empty;

            return targa
                .Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .ToUpperInvariant();
        }

        /// <summary>
        /// Formatta la targa nel formato standard italiano (AB123CD)
        /// </summary>
        /// <param name="targa">Targa da formattare</param>
        /// <returns>Targa formattata o vuota se non valida</returns>
        public static string FormattaTarga(string? targa)
        {
            if (string.IsNullOrWhiteSpace(targa))
                return string.Empty;

            var targaNormalizzata = NormalizzaTarga(targa);

            if (!IsTargaValida(targaNormalizzata))
                return targaNormalizzata; // Ritorna normalizzata anche se non valida

            // Se è formato nuovo (AB123CD), inserisce uno spazio: AB 123 CD
            if (TargaItalianaRegex.IsMatch(targaNormalizzata) && targaNormalizzata.Length == 7)
            {
                return $"{targaNormalizzata.Substring(0, 2)} {targaNormalizzata.Substring(2, 3)} {targaNormalizzata.Substring(5, 2)}";
            }

            // Se è formato vecchio, inserisce uno spazio dopo le lettere: TO 12345
            if (TargaVecchiaRegex.IsMatch(targaNormalizzata))
            {
                return $"{targaNormalizzata.Substring(0, 2)} {targaNormalizzata.Substring(2)}";
            }

            return targaNormalizzata;
        }

        /// <summary>
        /// Ottiene un messaggio di errore descrittivo per targhe non valide
        /// </summary>
        /// <returns>Messaggio di errore</returns>
        public static string GetErrorMessage()
        {
            return "La targa deve essere nel formato italiano (es. AB123CD o TO12345)";
        }
    }
}