#!/bin/bash

# Esci dallo script in caso di errore
set -e

# Funzioni di debug
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}
debug-log() {
    if [[ -v DEBUG_LOG ]]; then
        echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
    fi
}
debug-cat() {
    if [[ -v DEBUG_LOG ]]; then
        echo "[$(date '+%Y-%m-%d %H:%M:%S')] File $1"
        cat "$1"
    fi
}

# Esecuzione delle migrations
run_migration() {
    log "Rimuovo l'appsettings"
    rm appsettings*
    echo "{}" > appsettings.json

    log "Eseguo la migration..."
    
    local connection_string="Host=${Database__Host};Port=${Database__Port:-5432};Database=${Database__Name};Username=${Database__Username};Password=${Database__Password}"
    log "ConnectionString: $connection_string"
    
    dotnet ef database update --connection "$connection_string"
    
    if [[ $? -eq 0 ]]; then
        log "Migrazione eseguita con successo"
    else
        log "ERRORE: Errore nell'esecuzione della migration"
        exit 1
    fi
}

# Main execution 
main() {
    log "Output versioni dei software:"
    log "bash: $BASH_VERSION"
    log "curl: $(curl --version | head -n 1 | awk '{ print $2 }')"
    log "jq: $(jq --version)"
    log "dotnet: $(dotnet --version)"

    
    # Validate required environment variables
    if [[ -z "$Database__Host" || -z "$Database__Database" || -z "$Database__Username" || -z "$Database__Password" || -z "$Database__Schema" ]]; then
        log "ERRORE: Le variabili d'ambiente per la connessione al db sono assenti"
        exit 1
    fi

    run_migration

    if [[ -z "$SKIP_SLEEP" ]]; then
        log "A mimir (sleep necessario per deploy con docker swarm)"
        sleep 120
    fi
}

# Run the main function
main "$@"