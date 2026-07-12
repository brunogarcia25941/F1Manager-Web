using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace F1Manager.Web.Hubs
{
    public class RaceHub : Hub
    {
        // Para guardar o estado das corridas em tempo real na memória do servidor
        private static readonly ConcurrentDictionary<int, LiveRaceState> LiveRaces = new();

        // Obtém o estado atual da simulação para novos utilizadores que abram a página
        public LiveRaceState? GetRaceState(int corridaId)
        {
            LiveRaces.TryGetValue(corridaId, out var state);
            return state;
        }

        public List<LiveRaceState> GetActiveLiveRaces()
        {
            return LiveRaces.Values.Where(r => r.Iniciada).ToList();
        }

        // Chamado pelo Administrador para dar início à simulação em direto
        public async Task StartRace(int corridaId, string nomeGrandePremio, int totalVoltas, List<LivePilotState> pilotosIniciais)
        {
            var state = new LiveRaceState
            {
                CorridaId = corridaId,
                NomeGrandePremio = nomeGrandePremio,
                TotalVoltas = totalVoltas,
                VoltaAtual = 1,
                Iniciada = true,
                Pilotos = pilotosIniciais
            };

            // Atribui posições iniciais ordenadas de 1 a N
            for (int i = 0; i < state.Pilotos.Count; i++)
            {
                state.Pilotos[i].PosicaoFinal = i + 1;
            }

            LiveRaces[corridaId] = state;

            // Notifica todos os adeptos conectados que a corrida começou
            await Clients.All.SendAsync("ReceiveRaceStart", state);
        }

        // Chamado pelo Administrador para atualizar o número da volta (+1 ou -1)
        public async Task UpdateLap(int corridaId, int voltaAtual)
        {
            if (LiveRaces.TryGetValue(corridaId, out var state))
            {
                state.VoltaAtual = Math.Clamp(voltaAtual, 1, state.TotalVoltas);
                await Clients.All.SendAsync("ReceiveLapUpdate", state.VoltaAtual);
            }
        }

        // Chamado pelo Administrador ao carregar nas setas de Subir ou Descer posição
        public async Task ChangePosition(int corridaId, int pilotoId, bool subir)
        {
            if (LiveRaces.TryGetValue(corridaId, out var state))
            {
                var pilotos = state.Pilotos;
                var index = pilotos.FindIndex(p => p.PilotoId == pilotoId);
                if (index == -1) return;

                if (subir && index > 0)
                {
                    // Troca de posição com o piloto da frente (ultrapassagem)
                    var temp = pilotos[index];
                    pilotos[index] = pilotos[index - 1];
                    pilotos[index - 1] = temp;
                }
                else if (!subir && index < pilotos.Count - 1)
                {
                    // Troca de posição com o piloto de trás
                    var temp = pilotos[index];
                    pilotos[index] = pilotos[index + 1];
                    pilotos[index + 1] = temp;
                }

                // Corrige o índice de classificação de todos os pilotos
                for (int i = 0; i < pilotos.Count; i++)
                {
                    pilotos[i].PosicaoFinal = i + 1;
                }

                // Envia a tabela reordenada para todos os ecrãs
                await Clients.All.SendAsync("ReceiveRaceUpdateLive", state.Pilotos);
            }
        }

        // Chamado pelo Administrador quando atualiza o input da volta mais rápida de um piloto
        public async Task UpdateFastestLap(int corridaId, int pilotoId, string tempo)
        {
            if (LiveRaces.TryGetValue(corridaId, out var state))
            {
                var piloto = state.Pilotos.FirstOrDefault(p => p.PilotoId == pilotoId);
                if (piloto != null)
                {
                    piloto.TempoVoltaRapida = tempo;
                    await Clients.All.SendAsync("ReceiveRaceUpdateLive", state.Pilotos);
                }
            }
        }

        // Chamado pelo Administrador para remover o estado live da memória do servidor
        public async Task EndRace(int corridaId)
        {
            if (LiveRaces.TryRemove(corridaId, out _))
            {
                await Clients.All.SendAsync("ReceiveRaceEnd");
            }
        }
    }

    // Classes auxiliares para a simulação na memória do servidor
    public class LiveRaceState
    {
        public int CorridaId { get; set; }
        public string NomeGrandePremio { get; set; } = string.Empty; 
        public int TotalVoltas { get; set; }
        public int VoltaAtual { get; set; }
        public bool Iniciada { get; set; }
        public List<LivePilotState> Pilotos { get; set; } = new();
    }

    public class LivePilotState
    {
        public int PilotoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string EquipaNome { get; set; } = string.Empty;
        public int PosicaoFinal { get; set; }
        public string TempoVoltaRapida { get; set; } = "--:--.---";
    }
}