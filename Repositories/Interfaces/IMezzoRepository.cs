using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per l'accesso ai dati dei mezzi aziendali
    /// </summary>
    public interface IMezzoRepository
    {
        /// <summary>
        /// Ottiene tutti i mezzi non cancellati
        /// </summary>
        Task<IEnumerable<Mezzo>> GetAllAsync();

        /// <summary>
        /// Ottiene un mezzo per ID
        /// </summary>
        Task<Mezzo?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene un mezzo per targa
        /// </summary>
        Task<Mezzo?> GetByTargaAsync(string targa);

        /// <summary>
        /// Ottiene mezzi filtrati per stato
        /// </summary>
        Task<IEnumerable<Mezzo>> GetByStatoAsync(StatoMezzo stato);

        /// <summary>
        /// Ottiene mezzi filtrati per tipo di proprietà
        /// </summary>
        Task<IEnumerable<Mezzo>> GetByTipoProprietaAsync(TipoProprietaMezzo tipoProprieta);

        /// <summary>
        /// Ottiene mezzi filtrati per tipo veicolo
        /// </summary>
        Task<IEnumerable<Mezzo>> GetByTipoAsync(TipoMezzo tipo);

        /// <summary>
        /// Cerca mezzi per targa (ricerca parziale)
        /// </summary>
        Task<IEnumerable<Mezzo>> SearchByTargaAsync(string targa);

        /// <summary>
        /// Verifica se esiste già una targa (per validazione unicità)
        /// </summary>
        /// <param name="targa">Targa da verificare</param>
        /// <param name="excludeId">ID da escludere dal controllo (per edit)</param>
        Task<bool> ExistsTargaAsync(string targa, Guid? excludeId = null);

        /// <summary>
        /// Verifica se esiste già una Device IMEI (per validazione unicità)
        /// </summary>
        /// <param name="deviceIMEI">Device IMEI da verificare</param>
        /// <param name="excludeId">ID da escludere dal controllo (per edit)</param>
        Task<bool> ExistsDeviceIMEIAsync(string deviceIMEI, Guid? excludeId = null);

        /// <summary>
        /// Ottiene mezzi con assicurazione in scadenza
        /// </summary>
        Task<IEnumerable<Mezzo>> GetMezziConAssicurazioneInScadenzaAsync(int giorniPreavviso = 30);

        /// <summary>
        /// Ottiene mezzi con revisione in scadenza
        /// </summary>
        Task<IEnumerable<Mezzo>> GetMezziConRevisioneInScadenzaAsync(int giorniPreavviso = 30);

        /// <summary>
        /// Aggiunge un nuovo mezzo
        /// </summary>
        Task AddAsync(Mezzo mezzo);

        /// <summary>
        /// Aggiorna un mezzo esistente
        /// </summary>
        Task UpdateAsync(Mezzo mezzo);

        /// <summary>
        /// Elimina un mezzo (soft delete)
        /// </summary>
        Task DeleteAsync(Mezzo mezzo);

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}