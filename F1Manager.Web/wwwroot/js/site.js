// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Lógica de Pesquisa Global Instantânea na Navbar
document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("globalSearchInput");
    const resultsContainer = document.getElementById("globalSearchResults");

    if (searchInput && resultsContainer) {
        let debounceTimer;

        searchInput.addEventListener("input", function () {
            clearTimeout(debounceTimer);
            const query = this.value.trim();

            // Só pesquisa se tiver digitado pelo menos 2 caracteres
            if (query.length < 2) {
                resultsContainer.style.display = "none";
                resultsContainer.innerHTML = "";
                return;
            }

            // Atraso de 300ms (Debounce) para evitar sobrecarregar o servidor com pedidos
            debounceTimer = setTimeout(() => {
                fetch(`/api/search?q=${encodeURIComponent(query)}`)
                    .then(response => response.json())
                    .then(data => {
                        resultsContainer.innerHTML = "";

                        if (data.length === 0) {
                            resultsContainer.innerHTML = `<div class="p-3 text-muted small italic">Sem resultados encontrados</div>`;
                            resultsContainer.style.display = "block";
                            return;
                        }

                        data.forEach(item => {
                            // Define o estilo de cor para o tipo de elemento
                            let badgeClass = "badge-piloto";
                            if (item.type === "Equipa") badgeClass = "badge-equipa";
                            else if (item.type === "Campeonato") badgeClass = "badge-campeonato";
                            else if (item.type === "Corrida") badgeClass = "badge-corrida";

                            const element = document.createElement("a");
                            element.href = item.url;
                            element.className = "f1-search-item";
                            element.innerHTML = `
                                    <div>
                                        <strong class="d-block small text-dark">${item.title}</strong>
                                        <span class="text-muted" style="font-size: 0.75rem;">${item.subtitle}</span>
                                    </div>
                                    <span class="f1-search-type-badge ${badgeClass}">${item.type}</span>
                                `;
                            resultsContainer.appendChild(element);
                        });

                        resultsContainer.style.display = "block";
                    })
                    .catch(err => console.error("Erro na pesquisa:", err));
            }, 300);
        });

        // Oculta os resultados se clicar fora da caixa de pesquisa
        document.addEventListener("click", function (e) {
            if (!searchInput.contains(e.target) && !resultsContainer.contains(e.target)) {
                resultsContainer.style.display = "none";
            }
        });

        // Apresenta novamente os resultados ao focar de novo na caixa (se tiver texto)
        searchInput.addEventListener("focus", function () {
            if (this.value.trim().length >= 2 && resultsContainer.children.length > 0) {
                resultsContainer.style.display = "block";
            }
        });
    }
});


// Monitorização Global de Simulação (SignalR)
if (typeof signalR !== 'undefined') {
    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/raceHub")
        .withAutomaticReconnect()
        .build();

    const liveBadge = document.getElementById("globalLiveRaceBadge");
    const liveLink = document.getElementById("globalLiveRaceLink");

    function mostrarAlertaGlobal(state) {
        if (liveBadge && liveLink) {
            liveLink.href = `/Corridas/LiveTiming/${state.corridaId}`;
            liveBadge.classList.remove("d-none"); // Mostra o selo a piscar na navbar
        }
    }

    function ocultarAlertaGlobal() {
        if (liveBadge) {
            liveBadge.classList.add("d-none"); // Oculta o selo
        }
    }

    // Escuta os eventos do servidor SignalR
    hubConnection.on("ReceiveRaceStart", (state) => {
        mostrarAlertaGlobal(state);
    });

    hubConnection.on("ReceiveRaceEnd", () => {
        ocultarAlertaGlobal();
    });

    // Inicia a ligação e valida se já existe alguma corrida a decorrer
    hubConnection.start()
        .then(() => {
            hubConnection.invoke("GetActiveLiveRaces").then((races) => {
                if (races && races.length > 0) {
                    mostrarAlertaGlobal(races[0]);
                }
            });
        })
        .catch(err => console.error("Erro na escuta global da corrida:", err));
}