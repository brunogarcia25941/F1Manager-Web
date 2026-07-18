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

// Tradução dinâmica da página de gestão de conta do ASP.NET Identity (Manage Account)
document.addEventListener("DOMContentLoaded", function () {
    if (window.location.pathname.toLowerCase().includes("/account/manage")) {
        const t = {
            // Títulos e cabeçalhos
            "Manage your account": "Gerir a Minha Conta",
            "Change your account settings": "Alterar Definições da Conta",
            "Profile": "Perfil",
            "Email": "Email",
            "Password": "Palavra-passe",
            "Two-factor authentication": "Autenticação de Dois Fatores",
            "Personal data": "Dados Pessoais",
            "Change password": "Alterar Palavra-passe",
            "Manage email": "Gerir Email",
            
            // Labels e formulários
            "Username": "Nome de Utilizador",
            "Phone number": "Número de Telemóvel",
            "New email": "Novo Endereço de Email",
            "Current password": "Palavra-passe Atual",
            "New password": "Nova Palavra-passe",
            "Confirm new password": "Confirmar Nova Palavra-passe",
            
            // Botões de submissão
            "Save": "Guardar Alterações",
            "Change email": "Atualizar Email",
            "Update password": "Atualizar Palavra-passe",
            "Send verification email": "Enviar Email de Verificação",
            "Download": "Descarregar Dados",
            "Delete": "Eliminar Conta",
            
            // Textos informativos de Dados Pessoais
            "Your account contains personal data that you have given us. This page allows you to download or delete that data.": "A sua conta contém dados pessoais que nos forneceu. Esta página permite-lhe descarregar ou eliminar esses dados.",
            "Deleting this data will permanently delete your account, and this cannot be recovered.": "Eliminar estes dados irá apagar permanentemente a sua conta. Esta operação é irreversível.",
            
            // Mensagens de sucesso ou alertas
            "Verification email sent. Please check your email.": "Email de verificação enviado. Por favor verifique a sua caixa de entrada.",
            "Your profile has been updated": "O seu perfil foi atualizado com sucesso.",
            "Your password has been changed": "A sua palavra-passe foi alterada com sucesso.",
            "Your email has been changed": "O seu email foi alterada com sucesso."
        };

        // Função recursiva para traduzir nós de texto no DOM
        function traduzirElemento(elemento) {
            if (elemento.nodeType === Node.TEXT_NODE) {
                const textoOriginal = elemento.textContent.trim();
                if (!textoOriginal) return;

                const temDoisPontos = textoOriginal.endsWith(":");
                const textoLimpo = temDoisPontos ? textoOriginal.slice(0, -1).trim() : textoOriginal;

                if (t[textoLimpo]) {
                    elemento.textContent = t[textoLimpo] + (temDoisPontos ? ":" : "");
                }
            } else {
                // Traduz inputs e placeholders
                if (elemento.tagName === "INPUT" || elemento.tagName === "TEXTAREA") {
                    const placeholder = elemento.getAttribute("placeholder");
                    if (placeholder && t[placeholder]) {
                        elemento.setAttribute("placeholder", t[placeholder]);
                    }
                }
                
                // Traduz labels
                if (elemento.tagName === "LABEL") {
                    const textoOriginal = elemento.textContent.trim();
                    const temDoisPontos = textoOriginal.endsWith(":");
                    const textoLimpo = temDoisPontos ? textoOriginal.slice(0, -1).trim() : textoOriginal;

                    if (t[textoLimpo]) {
                        elemento.textContent = t[textoLimpo] + (temDoisPontos ? ":" : "");
                    }
                }

                // Recursão para nós filhos
                elemento.childNodes.forEach(traduzirElemento);
            }
        }

        // Traduz todo o corpo do documento de gestão
        document.body.childNodes.forEach(traduzirElemento);
        
        // Garante a tradução de botões e links de submissão adicionais
        const botoesSubmit = document.querySelectorAll("button, a.btn");
        botoesSubmit.forEach(btn => {
            const texto = btn.textContent.trim();
            if (t[texto]) {
                btn.textContent = t[texto];
            }
        });
    }
});