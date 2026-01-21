namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologia di destinatario per le notifiche email
    /// </summary>
    public enum TipoDestinatarioNotifica
    {
        /// <summary>
        /// Email del reparto (Reparto.Email) - Risolve 1 email
        /// </summary>
        Reparto = 0,

        /// <summary>
        /// Tutti gli utenti attivi con un determinato ruolo - Risolve N email
        /// </summary>
        Ruolo = 1,

        /// <summary>
        /// Utente specifico (ApplicationUser) - Risolve 1 email
        /// </summary>
        Utente = 2
    }
}