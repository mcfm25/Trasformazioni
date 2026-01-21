/* ============================================================================
   LOTTI WORKFLOW - JAVASCRIPT
   Da aggiungere a: /wwwroot/js/lotti-workflow.js
   Dipendenze: jQuery, Bootstrap 5
   ============================================================================ */

(function ($) {
    'use strict';

    // Namespace per funzioni workflow lotti
    window.LottiWorkflow = window.LottiWorkflow || {};

    /**
     * Cambia stato lotto tramite AJAX
     * @param {string} lottoId - GUID del lotto
     * @param {number} nuovoStato - Valore enum StatoLotto
     * @param {function} onSuccess - Callback successo (opzionale)
     * @param {function} onError - Callback errore (opzionale)
     */
    LottiWorkflow.cambiaStato = function (lottoId, nuovoStato, onSuccess, onError) {
        // Ottieni nome stato per conferma
        var nomeStato = LottiWorkflow.getStatoName(nuovoStato);

        if (!confirm(`Confermi il cambio stato a: ${nomeStato}?\n\nQuesta azione aggiornerà il workflow del lotto.`)) {
            return;
        }

        // Mostra loader
        LottiWorkflow.showLoader('Cambio stato in corso...');

        $.ajax({
            url: '/Lotti/CambiaStato',
            type: 'POST',
            data: {
                id: lottoId,
                nuovoStato: nuovoStato,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                LottiWorkflow.hideLoader();

                if (response.success) {
                    // Success callback o reload
                    if (typeof onSuccess === 'function') {
                        onSuccess(response);
                    } else {
                        LottiWorkflow.showSuccess('Stato cambiato con successo!', function () {
                            location.reload();
                        });
                    }
                } else {
                    // Error message dal server
                    LottiWorkflow.showError(response.message || 'Errore nel cambio stato.');
                    if (typeof onError === 'function') {
                        onError(response);
                    }
                }
            },
            error: function (xhr, status, error) {
                LottiWorkflow.hideLoader();
                LottiWorkflow.showError('Errore di connessione. Riprova più tardi.');
                
                if (typeof onError === 'function') {
                    onError({ message: error });
                }
            }
        });
    };

    /**
     * Ottieni nome stato da enum value
     * @param {number} statoValue - Valore numerico enum
     * @returns {string} Nome stato
     */
    LottiWorkflow.getStatoName = function (statoValue) {
        var stati = {
            0: 'Bozza',
            1: 'In Valutazione',
            2: 'Approvato',
            3: 'Rifiutato',
            4: 'In Elaborazione',
            5: 'Presentato',
            6: 'In Esame',
            7: 'Richiesta Integrazione',
            8: 'Vinto',
            9: 'Perso',
            10: 'Scartato',
            11: 'Annullato'
        };
        return stati[statoValue] || 'Sconosciuto';
    };

    /**
     * Verifica se uno stato è terminale
     * @param {number} statoValue - Valore numerico enum
     * @returns {boolean}
     */
    LottiWorkflow.isStatoTerminale = function (statoValue) {
        // 3: Rifiutato, 8: Vinto, 9: Perso, 10: Scartato, 11: Annullato
        return [3, 8, 9, 10, 11].includes(parseInt(statoValue));
    };

    /**
     * Ottieni badge class per stato
     * @param {number} statoValue - Valore numerico enum
     * @returns {string} Bootstrap badge class
     */
    LottiWorkflow.getStatoBadgeClass = function (statoValue) {
        var classes = {
            0: 'bg-secondary',
            1: 'bg-info',
            2: 'bg-success',
            3: 'bg-danger',
            4: 'bg-warning text-dark',
            5: 'bg-primary',
            6: 'bg-primary',
            7: 'bg-warning text-dark',
            8: 'bg-success',
            9: 'bg-danger',
            10: 'bg-dark',
            11: 'bg-secondary'
        };
        return classes[statoValue] || 'bg-secondary';
    };

    /**
     * Anima transizione timeline
     * @param {number} fromStato - Stato precedente
     * @param {number} toStato - Nuovo stato
     */
    LottiWorkflow.animateTimeline = function (fromStato, toStato) {
        var $timeline = $('.workflow-timeline');
        if ($timeline.length === 0) return;

        // Trova step coinvolti
        var $steps = $timeline.find('.timeline-step');
        
        $steps.each(function (index) {
            var $step = $(this);
            var stepStato = $step.data('stato');

            // Anima step se tra from e to
            if (stepStato >= fromStato && stepStato <= toStato) {
                setTimeout(function () {
                    $step.addClass('animating');
                    setTimeout(function () {
                        $step.removeClass('animating');
                    }, 600);
                }, index * 100);
            }
        });
    };

    /**
     * Mostra loader overlay
     * @param {string} message - Messaggio da mostrare
     */
    LottiWorkflow.showLoader = function (message) {
        var $loader = $('#workflowLoader');
        
        if ($loader.length === 0) {
            $loader = $('<div id="workflowLoader" class="workflow-loader">')
                .append('<div class="spinner-border text-primary" role="status">')
                .append('<span class="ms-2 loader-text">Caricamento...</span>');
            $('body').append($loader);
        }

        $loader.find('.loader-text').text(message || 'Caricamento...');
        $loader.fadeIn(200);
    };

    /**
     * Nascondi loader
     */
    LottiWorkflow.hideLoader = function () {
        $('#workflowLoader').fadeOut(200);
    };

    /**
     * Mostra messaggio successo
     * @param {string} message - Messaggio
     * @param {function} callback - Callback dopo chiusura (opzionale)
     */
    LottiWorkflow.showSuccess = function (message, callback) {
        var alert = `
            <div class="alert alert-success alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" 
                 style="z-index: 9999; min-width: 300px;" role="alert">
                <i class="bi bi-check-circle"></i> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        
        var $alert = $(alert);
        $('body').append($alert);

        setTimeout(function () {
            $alert.fadeOut(300, function () {
                $(this).remove();
                if (typeof callback === 'function') {
                    callback();
                }
            });
        }, 2000);
    };

    /**
     * Mostra messaggio errore
     * @param {string} message - Messaggio errore
     */
    LottiWorkflow.showError = function (message) {
        var alert = `
            <div class="alert alert-danger alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" 
                 style="z-index: 9999; min-width: 300px;" role="alert">
                <i class="bi bi-exclamation-triangle"></i> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        
        var $alert = $(alert);
        $('body').append($alert);

        setTimeout(function () {
            $alert.fadeOut(300, function () {
                $(this).remove();
            });
        }, 4000);
    };

    /**
     * Conferma eliminazione lotto
     * @param {string} lottoId - GUID lotto
     * @param {string} codiceLotto - Codice per messaggio conferma
     */
    LottiWorkflow.confirmDelete = function (lottoId, codiceLotto) {
        if (!confirm(`Sei sicuro di voler eliminare il lotto "${codiceLotto}"?\n\n` +
                     `Questa azione eliminerà anche:\n` +
                     `- Tutte le valutazioni associate\n` +
                     `- Tutte le elaborazioni\n` +
                     `- Tutti i partecipanti\n\n` +
                     `L'operazione NON può essere annullata.`)) {
            return;
        }

        LottiWorkflow.showLoader('Eliminazione in corso...');

        $.ajax({
            url: '/Lotti/Delete',
            type: 'POST',
            data: {
                id: lottoId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                LottiWorkflow.hideLoader();

                if (response.success) {
                    LottiWorkflow.showSuccess('Lotto eliminato con successo!', function () {
                        window.location.href = response.redirectUrl;
                    });
                } else {
                    LottiWorkflow.showError(response.message || 'Errore nell\'eliminazione.');
                }
            },
            error: function () {
                LottiWorkflow.hideLoader();
                LottiWorkflow.showError('Errore di connessione. Riprova.');
            }
        });
    };

    /**
     * Inizializza tooltips timeline
     */
    LottiWorkflow.initTooltips = function () {
        var tooltipTriggerList = [].slice.call(
            document.querySelectorAll('.timeline-circle[data-bs-toggle="tooltip"]')
        );
        
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            new bootstrap.Tooltip(tooltipTriggerEl);
        });
    };

    /**
     * Inizializza click handlers timeline
     */
    LottiWorkflow.initTimelineClickHandlers = function () {
        $('.timeline-step.clickable .timeline-circle').on('click', function () {
            var $step = $(this).closest('.timeline-step');
            var nuovoStato = $step.data('stato');
            var lottoId = $step.closest('.workflow-timeline').data('lotto-id');

            if (lottoId && nuovoStato !== undefined) {
                LottiWorkflow.cambiaStato(lottoId, nuovoStato);
            }
        });
    };

    // Document ready
    $(document).ready(function () {
        // Inizializza tooltips
        LottiWorkflow.initTooltips();

        // Inizializza click handlers se presente timeline
        if ($('.workflow-timeline').length > 0) {
            LottiWorkflow.initTimelineClickHandlers();
        }
    });

    // Esponi anche le funzioni globali per backward compatibility
    window.cambiaStato = LottiWorkflow.cambiaStato;
    window.confirmDelete = LottiWorkflow.confirmDelete;

})(jQuery);

/* ============================================================================
   LOADER CSS (da includere inline o in CSS)
   ============================================================================ */
/*
.workflow-loader {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: none;
    justify-content: center;
    align-items: center;
    z-index: 9998;
    color: white;
}

.workflow-loader.show {
    display: flex;
}
*/
