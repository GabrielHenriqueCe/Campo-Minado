using System;
using System.Reflection.Metadata;

namespace Pratica
{
    #region Dados e Tabuleiro

    /// <summary>
    /// Representa o tabuleiro do jogo
    /// </summary>
    class Tabuleiro
    {
        // CAMPOS PRIVADOS (Encapsulamento - dados internos protegidos)
        private string[,] matriz; // Array 2D que armazena o estado do tabuleiro

        // PROPERTIES (Encapsulamento - acesso controlado aos dados)
        public int TamanhoTabuleiro { get; private set; }

        // CONSTRUCTOR (Inicializa o objeto quando criado com 'new Tabuleiro(3)')
        public Tabuleiro(int tamanho)
        {
            TamanhoTabuleiro = tamanho;
            matriz = new string[TamanhoTabuleiro, TamanhoTabuleiro];

            // Inicializa todas as posições com quadrados vazios
            for (int i = 0; i < TamanhoTabuleiro; i++)
            {
                for (int j = 0; j < TamanhoTabuleiro; j++)
                {
                    matriz[i, j] = "⬜";
                }
            }
        }

        // PROPERTY (Encapsulamento - acesso controlado à matriz)
        // Permite ler a matriz de fora da classe, mas não modificar diretamente
        public string[,] Matriz
        {
            get { return matriz; }  // Getter: permite ler
                                    // Não tem setter: ninguém pode fazer tabuleiro.Matriz = outraMatriz
        }

        /// <summary>
        /// Verifica se uma posição já está ocupada por um símbolo
        /// </summary>
        public bool PosicaoOcupada(int linha, int coluna)
        {
            return matriz[linha, coluna] != "⬜";
        }

        /// <summary>
        /// Coloca um símbolo em uma posição específica
        /// </summary>
        public void ColocarSimbolo(int linha, int coluna, string simbolo)
        {
            matriz[linha, coluna] = simbolo;
        }
    }

    /// <summary>
    /// Representa uma bomba escondida no tabuleiro
    /// </summary>
    class Bomba
    {
        // CAMPOS PRIVADOS (Encapsulamento) - cada objeto Bomba tem sua própria posição
        private static readonly Random random = new Random();
        private int linha;
        private int coluna;

        // CONSTRUCTOR - inicializa a bomba em posição aleatória
        public Bomba(int tamanhoTabuleiro)
        {
            int posicaoBomba = random.Next(1, tamanhoTabuleiro * tamanhoTabuleiro + 1);

            this.linha = (posicaoBomba - 1) / tamanhoTabuleiro;
            this.coluna = (posicaoBomba - 1) % tamanhoTabuleiro;

            /*DEBUG: Exibe a posição da bomba para testes
            Console.WriteLine($"Bomba na posição {posicaoBomba} (linha {linha}, coluna {coluna})");
            Console.ReadKey();
            */
        }

        /// <summary>
        /// Inicializa a bomba em uma posição específica
        /// </summary>
        public Bomba(int linha, int coluna)
        {
            this.linha = linha;
            this.coluna = coluna;
        }

        // PROPERTIES (Encapsulamento - acesso controlado)
        public int Linha { get { return linha; } }
        public int Coluna { get { return coluna; } }

        // MÉTODO DE INSTÂNCIA - verifica se a bomba está na posição informada
        /// <summary>
        /// Verifica se a bomba está na posição especificada
        /// </summary>
        public bool TemBomba(int linha, int coluna)
        {
            return this.linha == linha && this.coluna == coluna;
        }

        // MÉTODO DE INSTÂNCIA - revela a bomba no tabuleiro
        /// <summary>
        /// Revela esta bomba no tabuleiro colocando o emoji 💣
        /// </summary>
        public void RevelarBomba(Tabuleiro tabuleiro)
        {
            if (tabuleiro.Matriz[linha, coluna] != "💥" && tabuleiro.Matriz[linha, coluna] != "🧊")
                tabuleiro.ColocarSimbolo(linha, coluna, Deusa.Ativa ? Deusa.EmojiBomba : "💣");
        }
    }

    /// <summary>
    /// Representa uma armadilha posicionada no tabuleiro pelo jogador
    /// </summary>
    class Armadilha
    {
        public int Linha { get; }
        public int Coluna { get; }
        public string Simbolo { get; }
        public string MensagemVitoria { get; }
        public string? DonoSimbolo { get; }

        public Armadilha(int linha, int coluna, string simbolo, string mensagemVitoria, string? donoSimbolo = null)
        {
            Linha = linha;
            Coluna = coluna;
            Simbolo = simbolo;
            MensagemVitoria = mensagemVitoria;
            DonoSimbolo = donoSimbolo;
        }

        /// <summary>
        /// Verifica se a armadilha está na posição especificada
        /// </summary>
        public bool TemArmadilha(int linha, int coluna) =>
            this.Linha == linha && this.Coluna == coluna;
    }

    /// <summary>
    /// Representa uma carta embaralhada no tabuleiro pela Piada de Mau Gosto
    /// </summary>
    class Carta
    {
        private static readonly Random random = new Random();
        private static readonly string[] naipes = { "♠️", "♥️", "♦️", "♣️" };

        public int Linha { get; }
        public int Coluna { get; }
        public string Simbolo { get; }

        public Carta(int linha, int coluna, int indice)
        {
            Linha = linha;
            Coluna = coluna;
            Simbolo = naipes[indice % naipes.Length];
        }

        /// <summary>
        /// Verifica se a carta está na posição especificada
        /// </summary>
        public bool TemCarta(int linha, int coluna) =>
            this.Linha == linha && this.Coluna == coluna;
    }

    /// <summary>
    /// Representa uma casa garantidamente segura, revelada pelo Glitch
    /// </summary>
    class CasaSegura
    {
        public int Linha { get; }
        public int Coluna { get; }

        public CasaSegura(int linha, int coluna)
        {
            Linha = linha;
            Coluna = coluna;
        }
        /// <summary>
        /// Verifica se a casa segura está na posição especificada
        /// </summary>
        public bool TemCasaSegura(int linha, int coluna) =>
            this.Linha == linha && this.Coluna == coluna;
    }

    /// <summary>
    /// Máscara de visibilidade individual de cada jogador — controla quais bombas e casas seguras ele já viu
    /// </summary>
    class OlhosDoJogador
    {
        private bool[,] vision;
        public List<CasaSegura> CasasSeguras { get; } = new List<CasaSegura>();

        public OlhosDoJogador(int tamanho)
        {
            vision = new bool[tamanho, tamanho];
        }

        public void Revelar(int linha, int coluna)
        {
            vision[linha, coluna] = true;
        }

        public bool JaViu(int linha, int coluna)
        {
            return vision[linha, coluna];
        }

        public void Esconder(int linha, int coluna)
        {
            vision[linha, coluna] = false;
        }
    }

    /// <summary>
    /// Agrupa todos os dados de uma partida: tabuleiro, bombas, visibilidade e elementos especiais
    /// </summary>
    class Partida
    {
        public Tabuleiro Tabuleiro { get; }
        public List<Bomba> Bombas { get; }
        public OlhosDoJogador OlhosJ1 { get; }
        public OlhosDoJogador OlhosJ2 { get; }
        public List<Armadilha> Armadilhas { get; } = new List<Armadilha>();
        public List<Carta> Cartas { get; } = new List<Carta>();
        public List<Armadilha> CampoDeLama { get; } = new List<Armadilha>();

        /// <summary>
        /// Retorna os olhos do oponente com base nos olhos do jogador atual
        /// </summary>
        public OlhosDoJogador ObterOlhosOponente(OlhosDoJogador olhosAtual)
        {
            return olhosAtual == OlhosJ1 ? OlhosJ2 : OlhosJ1;
        }

        public Partida(int tamanho, int quantidadeBombas)
        {
            Tabuleiro = new Tabuleiro(tamanho);
            Bombas = InicializarBombas(tamanho, quantidadeBombas);
            OlhosJ1 = new OlhosDoJogador(tamanho);
            OlhosJ2 = new OlhosDoJogador(tamanho);
        }

        /// <summary>
        /// Gera bombas em posições únicas e aleatórias
        /// </summary>
        private static List<Bomba> InicializarBombas(int tamanhoTabuleiro, int quantidadeBombas)
        {
            List<Bomba> bombas = new List<Bomba>(quantidadeBombas);
            for (int i = 0; i < quantidadeBombas; i++)
            {
                bool bombaValida = false;
                Bomba novaBomba;
                do
                {
                    novaBomba = new Bomba(tamanhoTabuleiro);
                    bombaValida = true;
                    foreach (var b in bombas)
                    {
                        if (b.Linha == novaBomba.Linha && b.Coluna == novaBomba.Coluna)
                        {
                            bombaValida = false;
                            break;
                        }
                    }
                } while (!bombaValida);
                bombas.Add(novaBomba);
            }
            return bombas;
        }
    }

    #endregion

    #region Habilidades e Personagens

    /// <summary>
    /// Controla o cooldown de uma habilidade — quanto tempo falta para poder usar novamente
    /// </summary>
    class SkillCooldown
    {
        public int TurnosRestantes => turnosRestantes;
        public int CooldownTotal => cooldownTotal;
        private int turnosRestantes = 0;
        private int cooldownTotal;

        public SkillCooldown(int cooldown)
        {
            cooldownTotal = cooldown;
        }

        public bool Disponivel => turnosRestantes == 0;

        public void Usar()
        {
            turnosRestantes = cooldownTotal;
        }

        public void PassarTurno()
        {
            if (turnosRestantes > 0)
                turnosRestantes--;
        }
        public void Resetar()
        {
            this.turnosRestantes = 0;
        }

        /// <summary>
        /// Retorna o emoji de relógio proporcional ao progresso do cooldown
        /// </summary>
        public static string ObterRelogio(int turnosRestantes, int cooldownTotal)
        {
            if (turnosRestantes == 0) return "🕛";

            string[] relogios = { "🕐", "🕑", "🕒", "🕓", "🕔", "🕕", "🕖", "🕗", "🕘", "🕙" };
            int turnosPassados = cooldownTotal - turnosRestantes;
            int indice = (int)Math.Round((double)turnosPassados * 9 / cooldownTotal) - 1;
            indice = Math.Clamp(indice, 0, 8);
            return relogios[indice];
        }
    }

    /// <summary>
    /// Classe base para todas as habilidades — define nome, cooldown e o contrato de Ativar()
    /// </summary>
    abstract class Habilidade
    {
        public string Nome { get; }
        public int Turnos { get; }
        public string Descricao { get; }
        public SkillCooldown Cooldown { get; }

        public Habilidade(string nome, int turnos, string descricao = "")
        {
            Nome = nome;
            Turnos = turnos;
            Descricao = descricao;
            Cooldown = new SkillCooldown(turnos);
        }

        public abstract void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null);
    }

    /// <summary>
    /// Habilidade ativada automaticamente em resposta a eventos do jogo, como explosões ou início de turno
    /// </summary>
    abstract class HabilidadePassiva : Habilidade
    {
        public HabilidadePassiva(string nome, int turnos, string descricao = "") : base(nome, turnos, descricao) { }
        public virtual bool Revive() => false;
        public abstract bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado);
        public abstract string MensagemSobreviveu(Personagem personagem);
        public abstract string MensagemMorreu(Personagem personagem);
    }

    /// <summary>
    /// Habilidade ativada manualmente pelo jogador no seu turno, com tipo de navegação próprio
    /// </summary>
    abstract class HabilidadeAtiva : Habilidade
    {
        public HabilidadeAtiva(string nome, int turnos, string descricao = "") : base(nome, turnos, descricao) { }
        public abstract ExibirMatriz.TipoNavegacao Navegacao { get; }
        public virtual string MensagemNavegacao => $"\nUse a {Nome}";
    }

    /// <summary>
    /// Revela bombas vizinhas ao explodir e ressurge dos mortos
    /// </summary>
    class Necromancia : HabilidadePassiva
    {
        public override bool Revive() => true;
        public Necromancia() : base("Necromancia", 5, "Ao explodir, revela bombas vizinhas e ressurge dos mortos") { }

        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado)
        {
            return resultado == GerenciadorDeJogo.ResultadoJogada.Explodiu;
        }
        public override string MensagemSobreviveu(Personagem personagem) =>
        $"{personagem.Simbolo} Necromancia ativada! {personagem.Nome} ressurgiu dos mortos!";
        public override string MensagemMorreu(Personagem personagem) =>
        $"{personagem.Simbolo} Necromancia falhou! {personagem.Nome} não ressurgiu!";

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            for (int dl = -1; dl <= 1; dl++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int novaLinha = linha + dl;
                    int novaColuna = coluna + dc;

                    if (novaLinha >= 0 && novaLinha < partida.Tabuleiro.TamanhoTabuleiro &&
                        novaColuna >= 0 && novaColuna < partida.Tabuleiro.TamanhoTabuleiro && !(dl == 0 && dc == 0))
                    {
                        foreach (var bomba in partida.Bombas)
                        {
                            if (bomba.TemBomba(novaLinha, novaColuna))
                            {
                                olhos.Revelar(novaLinha, novaColuna);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 50% de chance de sobreviver a uma explosão
    /// </summary>
    class Intangibilidade : HabilidadePassiva
    {
        private static readonly Random random = new Random();
        public override bool Revive()
        {
            int chance = random.Next(0, 100);
            return chance < 50; // 50% chance of reviving
        }
        public Intangibilidade() : base("Intangibilidade", 2, "50% de chance de sobreviver a uma explosão") { }
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado)
        {
            return resultado == GerenciadorDeJogo.ResultadoJogada.Explodiu;
        }
        public override string MensagemSobreviveu(Personagem personagem) =>
        $"{personagem.Simbolo} Intangibilidade ativada! {personagem.Nome} se tornou intangível!";
        public override string MensagemMorreu(Personagem personagem) =>
        $"{personagem.Simbolo} Intangibilidade falhou! {personagem.Nome} não conseguiu se tornar intangível!";
        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null) { }
    }

    /// <summary>
    /// Revela bombas em área 2x2
    /// </summary>
    class LanternaDeJack : HabilidadeAtiva
    {
        public LanternaDeJack() : base("Lanterna de Jack", 3, "Revela bombas em área 2x2") { }
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.Area2x2Revelar;
        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            for (int dl = 0; dl <= 1; dl++)
            {
                for (int dc = 0; dc <= 1; dc++)
                {
                    int novaLinha = linha + dl;
                    int novaColuna = coluna + dc;

                    if (novaLinha >= 0 && novaLinha < partida.Tabuleiro.TamanhoTabuleiro &&
                        novaColuna >= 0 && novaColuna < partida.Tabuleiro.TamanhoTabuleiro)
                    {
                        foreach (var bomba in partida.Bombas)
                        {
                            if (bomba.TemBomba(novaLinha, novaColuna))
                            {
                                olhos.Revelar(novaLinha, novaColuna);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Revela bombas e cria armadilhas em área 2x2
    /// </summary>
    class Abducao : HabilidadeAtiva
    {
        public string SimboloArmadilha { get; }
        public Abducao(string simbolo = "🛸") : base("Abdução", 5, "Revela e cria armadilhas em área 2x2") { SimboloArmadilha = simbolo; }
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.Area2x2Revelar;
        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            for (int dl = 0; dl <= 1; dl++)
            {
                for (int dc = 0; dc <= 1; dc++)
                {
                    int novaLinha = linha + dl;
                    int novaColuna = coluna + dc;

                    if (novaLinha >= 0 && novaLinha < partida.Tabuleiro.TamanhoTabuleiro &&
            novaColuna >= 0 && novaColuna < partida.Tabuleiro.TamanhoTabuleiro)
                    {
                        bool temBomba = false;
                        foreach (var bomba in partida.Bombas)
                        {
                            if (bomba.TemBomba(novaLinha, novaColuna))
                            {
                                olhos.Revelar(novaLinha, novaColuna);
                                temBomba = true;
                                break;
                            }
                        }

                        if (!temBomba && partida.Tabuleiro.Matriz[novaLinha, novaColuna] == "⬜"
                        && !partida.Cartas.Exists(c => c.TemCarta(novaLinha, novaColuna)))
                        {
                            Armadilha novaArmadilha = new Armadilha(novaLinha, novaColuna, SimboloArmadilha,
                            $"\n🛸 Você foi abduzido! {jogador?.Personagem.Simbolo} VENCEU!");
                            partida.Armadilhas.Add(novaArmadilha);
                            olhos.Revelar(novaLinha, novaColuna);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Revela todas as bombas de uma coluna inteira
    /// </summary>
    class ScanVertical : HabilidadeAtiva
    {
        public ScanVertical() : base("Scan Vertical", 4, "Revela bombas em uma coluna inteira") { }
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.ColunaRevelar;
        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            for (int i = 0; i < partida.Tabuleiro.TamanhoTabuleiro; i++)
            {
                foreach (var bomba in partida.Bombas)
                {
                    if (bomba.TemBomba(i, coluna))
                    {
                        olhos.Revelar(i, coluna);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// A cada turno, revela 1 bomba aleatória e marca 1 casa segura
    /// </summary>
    class Glitch : HabilidadePassiva
    {
        private static readonly Random random = new Random();
        public Glitch() : base("Glitch", 2, "Revela 1 bomba e marca 1 casa segura por turno") { }
        public override bool Revive() => false;
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado) =>
            resultado == GerenciadorDeJogo.ResultadoJogada.InicioTurno;
        public override string MensagemSobreviveu(Personagem personagem) =>
           $"\n{personagem.Simbolo} Glitch ativado! Interferência detectada no mapa!";
        public override string MensagemMorreu(Personagem personagem) => "";

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            var bombasNaoReveladas = partida.Bombas
                .Where(b => !olhos.JaViu(b.Linha, b.Coluna))
                .ToList();

            if (bombasNaoReveladas.Count > 0)
            {
                var bomba = bombasNaoReveladas[random.Next(bombasNaoReveladas.Count)];
                olhos.Revelar(bomba.Linha, bomba.Coluna);
            }

            var casasVazias = new List<(int l, int c)>();
            for (int i = 0; i < partida.Tabuleiro.TamanhoTabuleiro; i++)
            {
                for (int j = 0; j < partida.Tabuleiro.TamanhoTabuleiro; j++)
                {
                    bool temBomba = partida.Bombas.Exists(b => b.TemBomba(i, j));
                    bool jaSegura = olhos.CasasSeguras.Exists(cs => cs.TemCasaSegura(i, j));
                    if (!temBomba && !jaSegura && !partida.Tabuleiro.PosicaoOcupada(i, j) && !partida.Cartas.Exists(c => c.TemCarta(i, j)))
                        casasVazias.Add((i, j));
                }
            }

            if (casasVazias.Count > 0)
            {
                var (l, c) = casasVazias[random.Next(casasVazias.Count)];
                olhos.CasasSeguras.Add(new CasaSegura(l, c));
            }
        }
    }

    /// <summary>
    /// Pisa em área 2x2 destruindo bombas e armadilhas
    /// </summary>
    class FuriaTerresetre : HabilidadeAtiva
    {
        public FuriaTerresetre() : base("Fúria Terrestre", 3, "Pisa em área 2x2 destruindo tudo") { }
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.Area2x2Pisar;

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            for (int dl = 0; dl <= 1; dl++)
            {
                for (int dc = 0; dc <= 1; dc++)
                {
                    int novaLinha = linha + dl;
                    int novaColuna = coluna + dc;

                    if (novaLinha >= 0 && novaLinha < partida.Tabuleiro.TamanhoTabuleiro &&
                        novaColuna >= 0 && novaColuna < partida.Tabuleiro.TamanhoTabuleiro)
                    {
                        var bomba = partida.Bombas.Find(b => b.TemBomba(novaLinha, novaColuna));
                        if (bomba != null)
                        {
                            partida.Bombas.Remove(bomba);
                            partida.Tabuleiro.ColocarSimbolo(novaLinha, novaColuna, "💥");
                        }
                        else
                        {
                            if (partida.Armadilhas.Exists(a => a.TemArmadilha(novaLinha, novaColuna)))
                            {
                                partida.Armadilhas.RemoveAll(a => a.TemArmadilha(novaLinha, novaColuna));
                                partida.Tabuleiro.ColocarSimbolo(novaLinha, novaColuna, " 🪤");

                                // Limpa dos olhos do oponente
                                OlhosDoJogador olhosOponente = partida.ObterOlhosOponente(olhos);
                                olhosOponente.Esconder(novaLinha, novaColuna);
                            }
                            else if (partida.Tabuleiro.Matriz[novaLinha, novaColuna] == "⬜")
                            {
                                partida.Tabuleiro.ColocarSimbolo(novaLinha, novaColuna, jogador.Personagem.EmojiHabilidade[0]);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Revela todas as bombas de uma linha inteira
    /// </summary>
    class VentoDeCorte : HabilidadeAtiva
    {
        public VentoDeCorte() : base("Vento de Corte", 4, "Revela bombas em uma linha inteira") { }
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.LinhaRevelar;

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            for (int j = 0; j < partida.Tabuleiro.TamanhoTabuleiro; j++)
            {
                foreach (var bomba in partida.Bombas)
                {
                    if (bomba.TemBomba(linha, j))
                    {
                        olhos.Revelar(linha, j);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Cobre casas do mapa com cartas, garantindo pelo menos 1 bomba escondida entre elas
    /// </summary>
    class PiadaDeMauGosto : HabilidadePassiva
    {
        private static readonly Random random = new Random();
        public PiadaDeMauGosto() : base("Piada de Mau Gosto", 5, "Embaralha o mapa com cartas") { }
        public override bool Revive() => false;
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado) =>
            resultado == GerenciadorDeJogo.ResultadoJogada.InicioTurno;
        public override string MensagemSobreviveu(Personagem personagem) =>
            $"\n🃏 {personagem.Simbolo} embaralhou o mapa!";
        public override string MensagemMorreu(Personagem personagem) => "";

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            // Coleta casas disponíveis (vazias e bombas não reveladas)
            var casasVazias = new List<(int l, int c)>();
            var casasBomba = new List<(int l, int c)>();

            for (int i = 0; i < partida.Tabuleiro.TamanhoTabuleiro; i++)
            {
                for (int j = 0; j < partida.Tabuleiro.TamanhoTabuleiro; j++)
                {
                    bool temCarta = partida.Cartas.Exists(c => c.TemCarta(i, j));
                    if (temCarta) continue;

                    bool temBomba = partida.Bombas.Exists(b => b.TemBomba(i, j));
                    bool ocupada = partida.Tabuleiro.PosicaoOcupada(i, j);

                    if (!ocupada && !temBomba)
                        casasVazias.Add((i, j));
                    else if (temBomba)
                        casasBomba.Add((i, j));
                }
            }

            // Embaralha as listas
            casasVazias = casasVazias.OrderBy(_ => random.Next()).ToList();
            casasBomba = casasBomba.OrderBy(_ => random.Next()).ToList();

            // Monta a lista final: preferência pelas vazias, 1 bomba obrigatória
            var selecionadas = new List<(int l, int c, bool eBomba)>();

            // Pega até 5 casas vazias
            foreach (var c in casasVazias.Take(5))
                selecionadas.Add((c.l, c.c, false));

            // Completa com bombas se necessário
            int faltam = Math.Min(5, 5 - selecionadas.Count);
            foreach (var c in casasBomba.Take(faltam))
                selecionadas.Add((c.l, c.c, true));

            // Adiciona 1 bomba obrigatória
            if (casasBomba.Count > 0)
                selecionadas.Add((casasBomba[0].l, casasBomba[0].c, true));
            else
            {
                // Cria bomba nova se não houver
                if (casasVazias.Count > selecionadas.Count)
                {
                    var nova = casasVazias[selecionadas.Count];
                    partida.Bombas.Add(new Bomba(nova.l, nova.c));
                    selecionadas.Add((nova.l, nova.c, true));
                }
            }

            // Embaralha a ordem final e coloca 1 por 1
            selecionadas = selecionadas.OrderBy(_ => random.Next()).ToList();
            int indice = 0;
            foreach (var (l, c, _) in selecionadas)
            {
                partida.Cartas.Add(new Carta(l, c, indice++));
                ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, null, null, 1, partida);
                Thread.Sleep(300);
            }
        }
    }

    /// <summary>
    /// Infecta uma linha inteira com lama e revela suas bombas
    /// </summary>
    class CampoDeFezes : HabilidadeAtiva
    {
        public CampoDeFezes() : base("Campo de Fezes", 9, "Cria lama infectando uma linha e revelando bombas") { }
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.LinhaRevelar;

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            for (int j = 0; j < partida.Tabuleiro.TamanhoTabuleiro; j++)
            {
                // Revela bombas nos olhos do MiniBoss
                foreach (var bomba in partida.Bombas)
                    if (bomba.TemBomba(linha, j))
                        olhos.Revelar(linha, j);

                // O MiniBoss se sente em casa mudando a linha
                // O jogador se sente em casa mudando a linha
                if (partida.Tabuleiro.Matriz[linha, j] == "⬜"
                && !partida.Cartas.Exists(c => c.TemCarta(linha, j)))
                {
                    string simboloLama = jogador.Personagem.EmojiHabilidade[1 + (j % (jogador.Personagem.EmojiHabilidade.Length - 1))];
                    partida.Tabuleiro.ColocarSimbolo(linha, j, simboloLama);
                    partida.CampoDeLama.Add(new Armadilha(linha, j, simboloLama, "\n🪦 Você foi enterrado na merda!", jogador.Personagem.Simbolo));
                }
            }
        }
    }

    /// <summary>
    /// Remove bombas reveladas do oponente e cria 2 novas no mapa
    /// </summary>
    class PactoInfernal : HabilidadeAtiva
    {
        private static readonly Random random = new Random();
        public PactoInfernal() : base("Pacto Infernal", 2, "Remove bombas reveladas do oponente e cria 2 novas") { }
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.Normal;

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            OlhosDoJogador olhosOponente = partida.ObterOlhosOponente(olhos);

            // Pega até 2 bombas reveladas nos olhos do oponente
            var bombasReveladas = partida.Bombas
                .Where(b => olhosOponente.JaViu(b.Linha, b.Coluna))
                .Take(2)
                .ToList();

            foreach (var bomba in bombasReveladas)
            {
                olhosOponente.Esconder(bomba.Linha, bomba.Coluna);
                partida.Bombas.Remove(bomba);
            }

            // Trata a casa escolhida
            var bombaLocal = partida.Bombas.Find(b => b.TemBomba(linha, coluna));
            if (bombaLocal != null)
            {
                partida.Bombas.Remove(bombaLocal);
                partida.Tabuleiro.ColocarSimbolo(linha, coluna, "💥");
            }
            else if (partida.Armadilhas.Exists(a => a.TemArmadilha(linha, coluna)))
            {
                partida.Armadilhas.RemoveAll(a => a.TemArmadilha(linha, coluna));
                olhosOponente.Esconder(linha, coluna);
                partida.Tabuleiro.ColocarSimbolo(linha, coluna, "🪤");
            }
            else if (partida.Tabuleiro.Matriz[linha, coluna] == "⬜" ||
                     partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna)))
            {
                partida.Tabuleiro.ColocarSimbolo(linha, coluna, jogador.Personagem.EmojiHabilidade[0]);
            }

            // Cria 2 bombas novas
            int bombasCriadas = 0;
            int tentativas = 0;
            while (bombasCriadas < 2 && tentativas < 100)
            {
                tentativas++;
                int novaLinha = random.Next(0, partida.Tabuleiro.TamanhoTabuleiro);
                int novaColuna = random.Next(0, partida.Tabuleiro.TamanhoTabuleiro);

                bool temBomba = partida.Bombas.Exists(b => b.TemBomba(novaLinha, novaColuna));
                bool temSimbolo = partida.Tabuleiro.Matriz[novaLinha, novaColuna] != "⬜";
                bool temCarta = partida.Cartas.Exists(c => c.TemCarta(novaLinha, novaColuna));
                bool temLama = partida.CampoDeLama.Exists(a => a.TemArmadilha(novaLinha, novaColuna));

                if (temBomba || temCarta || temLama) continue;

                if (partida.Armadilhas.Exists(a => a.TemArmadilha(novaLinha, novaColuna)))
                {
                    partida.Armadilhas.RemoveAll(a => a.TemArmadilha(novaLinha, novaColuna));
                    partida.Tabuleiro.ColocarSimbolo(novaLinha, novaColuna, "💥");
                    bombasCriadas++;
                    continue;
                }

                if (temSimbolo) continue;

                partida.Bombas.Add(new Bomba(novaLinha, novaColuna));
                olhos.Revelar(novaLinha, novaColuna);
                bombasCriadas++;
            }
        }
    }

    /// <summary>
    /// Copia uma habilidade ativa do oponente, incluindo navegação e comportamento, com cooldown próprio
    /// </summary>
    class CopiarHabilidadeAtiva : HabilidadeAtiva
    {
        private readonly HabilidadeAtiva habilidadeCopiada;

        public CopiarHabilidadeAtiva(HabilidadeAtiva habilidadeCopiada)
            : base(habilidadeCopiada.Nome, habilidadeCopiada.Turnos, habilidadeCopiada.Descricao)
        {
            this.habilidadeCopiada = habilidadeCopiada;
        }

        public override ExibirMatriz.TipoNavegacao Navegacao => habilidadeCopiada.Navegacao;

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
            => habilidadeCopiada.Ativar(partida, olhos, linha, coluna, jogador);
    }

    /// <summary>
    /// Copia uma habilidade passiva do oponente, incluindo condições de ativação e revive, com cooldown próprio
    /// </summary>
    class CopiarHabilidadePassiva : HabilidadePassiva
    {
        private readonly HabilidadePassiva habilidadeCopiada;

        public CopiarHabilidadePassiva(HabilidadePassiva habilidadeCopiada)
            : base(habilidadeCopiada.Nome, habilidadeCopiada.Turnos, habilidadeCopiada.Descricao)
        {
            this.habilidadeCopiada = habilidadeCopiada;
        }

        public override bool Revive() => habilidadeCopiada.Revive();
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado) => habilidadeCopiada.DeveAtivar(resultado);
        public override string MensagemSobreviveu(Personagem personagem) => habilidadeCopiada.MensagemSobreviveu(personagem);
        public override string MensagemMorreu(Personagem personagem) => habilidadeCopiada.MensagemMorreu(personagem);

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
            => habilidadeCopiada.Ativar(partida, olhos, linha, coluna, jogador);
    }

    /// <summary>
    /// Representa uma facção com bônus passivo compartilhado entre seus personagens
    /// </summary>
    class Faction
    {
        public string Nome { get; private set; }
        public string Simbolo { get; private set; }
        public string Descricao { get; private set; }
        public Habilidade? Habilidade { get; private set; }

        public Faction(string nome, string simbolo, Habilidade? habilidade, string descricao = "")
        {
            Nome = nome;
            Simbolo = simbolo;
            Habilidade = habilidade;
            Descricao = descricao;
        }

        public static readonly Faction LadoSombrio = new Faction("Lado Sombrio", "🌑", new PassivoLadoSombrio(), "A cada 4 turnos, cria 1 bomba secreta no mapa");
        public static readonly Faction Tecnologicos = new Faction("Tecnológicos", "⚙️", new PassivoTecnologicos(), "A cada 4 turnos, revela 1 bomba no mapa");
        public static readonly Faction Folclore = new Faction("Folclore", "🪬", new PassivoFolclore(), "25% de chance de reduzir cooldown em 1 por turno");
        public static readonly Faction Especial = new Faction("Especial", "⭐", new AtivaEspecial(), "A cada 6 turnos, pode pisar com imunidade total");
        public static readonly Faction Boss = new Faction("Boss", "🔱", new PassivoBoss(), "Visão total do tabuleiro do oponente");

        /*
            🌑 Lado Sombrio → Caveira, Fantasma, Abóbora
            ⚙️ Tecnológicos → Alien, Robô, Invasor
            🪬 Folclore → Ogro, Tengu, Palhaço
            ⭐ Especial → Cocô | 🎭 Mímico e ☃️ Boneco de Neve (easter eggs)
            🔱 Boss → Diabo 
        */
    }

    /// <summary>
    /// Passivo do Lado Sombrio — cria 1 bomba secreta no mapa a cada 4 turnos
    /// </summary>
    class PassivoLadoSombrio : HabilidadePassiva
    {
        private static readonly Random random = new Random();
        public PassivoLadoSombrio() : base("Lado Sombrio", 4) { }
        public override bool Revive() => false;
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado) =>
            resultado == GerenciadorDeJogo.ResultadoJogada.InicioTurno;
        public override string MensagemSobreviveu(Personagem personagem) =>
            $"\n🌑 As sombras criaram uma nova bomba!";
        public override string MensagemMorreu(Personagem personagem) => "";

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            int tentativas = 0;
            while (tentativas < 100)
            {
                tentativas++;
                int novaLinha = random.Next(0, partida.Tabuleiro.TamanhoTabuleiro);
                int novaColuna = random.Next(0, partida.Tabuleiro.TamanhoTabuleiro);

                bool temBomba = partida.Bombas.Exists(b => b.TemBomba(novaLinha, novaColuna));
                bool temSimbolo = partida.Tabuleiro.Matriz[novaLinha, novaColuna] != "⬜";
                bool temCarta = partida.Cartas.Exists(c => c.TemCarta(novaLinha, novaColuna));
                bool temLama = partida.CampoDeLama.Exists(a => a.TemArmadilha(novaLinha, novaColuna));

                if (temBomba || temCarta || temLama || temSimbolo) continue;

                if (partida.Armadilhas.Exists(a => a.TemArmadilha(novaLinha, novaColuna)))
                {
                    partida.Armadilhas.RemoveAll(a => a.TemArmadilha(novaLinha, novaColuna));
                    partida.Tabuleiro.ColocarSimbolo(novaLinha, novaColuna, "💥");
                    break;
                }

                partida.Bombas.Add(new Bomba(novaLinha, novaColuna));
                olhos.Revelar(novaLinha, novaColuna);
                break;
            }
        }
    }

    /// <summary>
    /// Passivo dos Tecnológicos — revela 1 bomba aleatória a cada 4 turnos
    /// </summary>
    class PassivoTecnologicos : HabilidadePassiva
    {
        private static readonly Random random = new Random();
        public PassivoTecnologicos() : base("Tecnológicos", 4) { }
        public override bool Revive() => false;
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado) =>
            resultado == GerenciadorDeJogo.ResultadoJogada.InicioTurno;
        public override string MensagemSobreviveu(Personagem personagem) =>
            $"\n⚙️ Interferência detectada! Uma bomba foi revelada!";
        public override string MensagemMorreu(Personagem personagem) => "";

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            var bombasNaoReveladas = partida.Bombas
                .Where(b => !olhos.JaViu(b.Linha, b.Coluna))
                .ToList();

            if (bombasNaoReveladas.Count > 0)
            {
                var bomba = bombasNaoReveladas[random.Next(bombasNaoReveladas.Count)];
                olhos.Revelar(bomba.Linha, bomba.Coluna);
            }
        }
    }

    /// <summary>
    /// Passivo do Folclore — 25% de chance de reduzir 1 cooldown por turno
    /// </summary>
    class PassivoFolclore : HabilidadePassiva
    {
        private static readonly Random random = new Random();
        public PassivoFolclore() : base("Folclore", 1) { }
        public override bool Revive() => false;
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado) =>
            resultado == GerenciadorDeJogo.ResultadoJogada.InicioTurno;
        public override string MensagemMorreu(Personagem personagem) => "";
        public override string MensagemSobreviveu(Personagem personagem) => "";

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            if (jogador == null) return;
            if (random.Next(0, 100) >= 25) return;

            if (jogador.Personagem.Habilidade != null && !jogador.Personagem.Habilidade.Cooldown.Disponivel)
            {
                jogador.Personagem.Habilidade.Cooldown.PassarTurno();
                Console.WriteLine($"\n🪬 A cultura está no coração do povo! Tempo de espera reduzido em 1!");
                CondicaoDeVitoria.EnterParaContinuar();
            }
        }
    }

    /// <summary>
    /// Ativo do Especial — pisa em uma casa com imunidade total removendo tudo que estiver lá
    /// </summary>
    class AtivaEspecial : HabilidadeAtiva
    {
        public AtivaEspecial() : base("Imunidade", 6) { }
        public override string MensagemNavegacao => "\nColoque o ⭐ em uma posição";
        public override ExibirMatriz.TipoNavegacao Navegacao => ExibirMatriz.TipoNavegacao.Normal;

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null)
        {
            OlhosDoJogador olhosOponente = partida.ObterOlhosOponente(olhos);

            // Remove bomba se houver
            var bomba = partida.Bombas.Find(b => b.TemBomba(linha, coluna));
            if (bomba != null)
            {
                partida.Bombas.Remove(bomba);
                olhosOponente.Esconder(linha, coluna); // limpa dos olhos do oponente
            }

            // Remove armadilha se houver
            if (partida.Armadilhas.Exists(a => a.TemArmadilha(linha, coluna)))
            {
                partida.Armadilhas.RemoveAll(a => a.TemArmadilha(linha, coluna));
                olhosOponente.Esconder(linha, coluna);
            }

            // Remove carta se houver
            partida.Cartas.RemoveAll(c => c.TemCarta(linha, coluna));

            // Coloca ⭐ na casa
            partida.Tabuleiro.ColocarSimbolo(linha, coluna, "⭐");
        }
    }

    /// <summary>
    /// Passivo do Boss — concede visão total do tabuleiro do oponente
    /// </summary>
    class PassivoBoss : HabilidadePassiva
    {
        public PassivoBoss() : base("Boss", 1) { }
        public override bool Revive() => false;
        public override bool DeveAtivar(GerenciadorDeJogo.ResultadoJogada resultado) =>
            resultado == GerenciadorDeJogo.ResultadoJogada.InicioTurno;
        public override string MensagemSobreviveu(Personagem personagem) => "";
        public override string MensagemMorreu(Personagem personagem) => "";

        public override void Ativar(Partida partida, OlhosDoJogador olhos, int linha, int coluna, Jogador? jogador = null) { }
    }

    /// <summary>
    /// Responsável por aplicar a transformação da Deusa quando dois jogadores escolhem o mesmo personagem
    /// </summary>
    class Deusa
    {
        private static readonly Random random = new Random();

        public static bool Ativa { get; private set; } = false;
        public static string EmojiBomba => "❄️";
        public static string EmojiExplosao => "🧊";
        public static string MensagemMorte => "🧊 Você foi congelado de frio!";

        public static void Resetar() => Ativa = false;

        public static void Aplicar(ref Jogador jogador1, ref Jogador jogador2, int tamanho)
        {
            Ativa = true;

            Jogador apostolo, oponente;
            if (random.Next(2) == 0)
            {
                apostolo = jogador1;
                oponente = jogador2;
            }
            else
            {
                apostolo = jogador2;
                oponente = jogador1;
            }

            var novoApostolo = CriarApostolo(apostolo, oponente);

            if (apostolo == jogador1) jogador1 = novoApostolo;
            else jogador2 = novoApostolo;

            ExibirCena(apostolo.Nome, novoApostolo.Personagem, tamanho);
        }

        private static void ExibirCena(string nomeJogador, Personagem apostolo, int tamanho)
        {
            Console.Clear();
            Console.WriteLine("🌬️  A deusa soprou o tabuleiro! O tabuleiro congelou 🥶\n");
            for (int i = 0; i < tamanho; i++)
            {
                for (int j = 0; j < tamanho; j++)
                    Console.Write("  🟦  ");
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n🌬️  A deusa escolheu um apóstolo!");
            Console.WriteLine($"\n{nomeJogador} virou {apostolo.Simbolo} {apostolo.Nome} e roubou as habilidades do oponente!");
            CondicaoDeVitoria.EnterParaContinuar();
        }
        private static Habilidade? CopiarHabilidade(Habilidade? habilidade) =>
            habilidade is HabilidadeAtiva ha ? new CopiarHabilidadeAtiva(ha) :
            habilidade is HabilidadePassiva hp ? new CopiarHabilidadePassiva(hp) :
            null;

        private static Jogador CriarApostolo(Jogador jogador, Jogador oponente)
        {
            var templateMimico = SelecaoSimbolo.ObterPersonagem(11);
            var templateBonecoDeNeve = SelecaoSimbolo.ObterPersonagem(12);
            var templatePapaiNoel = SelecaoSimbolo.ObterPersonagem(13);

            Personagem apostolo = jogador.Personagem.Simbolo == "😈"
                ? new Personagem(templatePapaiNoel.Nome, templatePapaiNoel.Simbolo, CopiarHabilidade(oponente.Personagem.Habilidade), Faction.Boss, templatePapaiNoel.EmojiHabilidade)
                : oponente.Personagem.Habilidade is HabilidadeAtiva haOp
                ? new Personagem(templateMimico.Nome, templateMimico.Simbolo, new CopiarHabilidadeAtiva(haOp), Faction.Especial, templateMimico.EmojiHabilidade)
                : new Personagem(templateBonecoDeNeve.Nome, templateBonecoDeNeve.Simbolo, CopiarHabilidade(oponente.Personagem.Habilidade), Faction.Especial, templateBonecoDeNeve.EmojiHabilidade);

            return new Jogador(jogador.Nome, apostolo, jogador.IndicePersonagem);
        }
    }

    /// <summary>
    /// Representa um personagem jogável com nome, símbolo, habilidade e facção
    /// </summary>
    class Personagem
    {
        public string Nome { get; private set; }
        public string Simbolo { get; private set; }
        public Habilidade Habilidade { get; private set; }
        public Faction? Faction { get; private set; }
        public string[]? EmojiHabilidade { get; private set; }
        public Habilidade? HabilidadeFaccao { get; private set; }

        public Personagem(string nome, string simbolo, Habilidade? habilidade, Faction? faction, string[]? emojiHabilidade = null)
        {
            Nome = nome;
            Simbolo = simbolo;
            Habilidade = habilidade;
            Faction = faction;
            EmojiHabilidade = emojiHabilidade;
            HabilidadeFaccao = ClonarHabilidade(faction?.Habilidade);
        }

        private static Habilidade? ClonarHabilidade(Habilidade? h) => h switch
        {
            PassivoLadoSombrio => new PassivoLadoSombrio(),
            PassivoTecnologicos => new PassivoTecnologicos(),
            PassivoFolclore => new PassivoFolclore(),
            AtivaEspecial => new AtivaEspecial(),
            PassivoBoss => new PassivoBoss(),
            _ => null
        };
    }

    /// <summary>
    /// Representa um jogador com nome, índice e personagem selecionado
    /// </summary>
    class Jogador
    {
        public string Nome { get; private set; }
        public int IndicePersonagem { get; private set; }
        public Personagem Personagem { get; private set; }

        public Jogador(string nome, Personagem personagem, int indice)
        {
            Nome = nome;
            Personagem = personagem;
            IndicePersonagem = indice;
        }
    }

    /// <summary>
    /// Responsável pela interface de seleção de personagens
    /// </summary>
    class SelecaoSimbolo
    {
        private static readonly List<Personagem> personagens = new List<Personagem>
            {
                new Personagem("Caveira", "💀", new Necromancia(), Faction.LadoSombrio),
                new Personagem("Fantasma", "👻", new Intangibilidade(), Faction.LadoSombrio),
                new Personagem("Abóbora", "🎃", new LanternaDeJack(), Faction.LadoSombrio),
                new Personagem("Alien", "👽", new Abducao(), Faction.Tecnologicos),
                new Personagem("Robô", "🤖", new ScanVertical(), Faction.Tecnologicos),
                new Personagem("Invasor", "👾", new Glitch(), Faction.Tecnologicos),
                new Personagem("Ogro", "👹", new FuriaTerresetre(), Faction.Folclore, ["👹"]),
                new Personagem("Tengu", "👺", new VentoDeCorte(), Faction.Folclore),
                new Personagem("Palhaço", "🤡", new PiadaDeMauGosto(), Faction.Folclore),
                new Personagem("Cocô", "💩", new CampoDeFezes(), Faction.Especial, ["💩","🚽", "🧻", "🪠"]),
                new Personagem("Diabo", "😈", new PactoInfernal(), Faction.Boss, ["😈"]),
                new Personagem("Mímico", "🎭", null, Faction.Especial, ["🎭","🪞","🚪","🪟","🧱"]),
                new Personagem("Boneco de Neve", "☃️", null, Faction.Especial),
                new Personagem("Papai Noel", "🎅", null, Faction.Boss,["🎅"])
            };
        /// <summary>
        /// Retorna o número total de símbolos disponíveis
        /// </summary>
        /// <returns>Quantidade de símbolos no array</returns>
        public static int TotalSimbolos()
        {
            return personagens.Count;
        }

        /// <summary>
        /// Retorna o personagem no índice especificado
        /// </summary>
        public static Personagem ObterPersonagem(int indice)
        {
            return personagens[indice];
        }

        /// <summary>
        /// Exibe a lista de símbolos disponíveis com cursor na linha especificada
        /// </summary>
        /// <param name="player">Nome do jogador (ex: "Jogador 1")</param>
        /// <param name="linhaAtual">Índice do símbolo selecionado</param>
        public static void ExibirSimbolo(string player, int linhaAtual, int fase = 0)
        {
            Console.Clear();
            Console.WriteLine($"Selecione o personagem - {player}\n");
            Console.WriteLine("Use W, S para mover | ENTER para confirmar\n");

            Faction? faccaoAtual = null;

            for (int i = 0; i < personagens.Count; i++)
            {
                var p = personagens[i];

                // Cabeçalho da facção
                if (p.Faction != faccaoAtual)
                {
                    faccaoAtual = p.Faction;
                    Console.WriteLine($"\n{faccaoAtual?.Simbolo} {faccaoAtual?.Nome} — {faccaoAtual?.Descricao}");
                }

                string cursor = i == linhaAtual ? "✅" : "☑️";

                if (p.Simbolo == "😈" && fase < 210)
                {
                    Console.WriteLine($"  {cursor} {p.Simbolo} {"???",-12} Desbloqueável na fase 210");
                }
                else if (p.Simbolo == "💩" && fase < 110)
                {
                    Console.WriteLine($"  {cursor} {p.Simbolo} {"???",-12} Desbloqueável na fase 110");
                }
                else
                {
                    string habilidade = p.Habilidade != null
                        ? $"{p.Habilidade.Nome} 🕛 cd{p.Habilidade.Turnos} — {p.Habilidade.Descricao}"
                        : "Sem habilidade";
                    Console.WriteLine($"  {cursor} {p.Simbolo} {p.Nome,-12} {habilidade}");
                }
            }
        }

        /// <summary>
        /// Permite que o jogador navegue e escolha seu personagem
        /// </summary>
        /// <param name="player">Nome do jogador (ex: "Jogador 1")</param>
        /// <returns>Personagem escolhido e seu índice</returns>
        public static (Personagem personagem, int indice) Selecionar(string player, int fase = 0)
        {
            int total = fase >= 210 ? personagens.Count - 4
            : fase >= 110 ? personagens.Count - 5
            : personagens.Count - 6;

            int linhaAtual = 0;
            ExibirSimbolo(player, linhaAtual, fase);

            do
            {
                bool entradaValida = false;
                ConsoleKeyInfo input;
                do
                {
                    input = Console.ReadKey(true);

                    if (input.Key == ConsoleKey.W ||
                        input.Key == ConsoleKey.S ||
                        input.Key == ConsoleKey.Enter)
                    {
                        entradaValida = true;
                    }
                    else
                    {
                        entradaValida = false;
                    }
                } while (!entradaValida);

                if (input.Key == ConsoleKey.S && linhaAtual < total)
                {
                    linhaAtual++;
                    ExibirSimbolo(player, linhaAtual, fase);
                }
                else if (input.Key == ConsoleKey.W && linhaAtual > 0)
                {
                    linhaAtual--;
                    ExibirSimbolo(player, linhaAtual, fase);
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    ExibirSimbolo(player, linhaAtual, fase);
                    return (personagens[linhaAtual], linhaAtual);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Posição inválida, digite novamente\n");
                    ExibirSimbolo(player, linhaAtual, fase);
                }
            } while (true);
        }
    }

    #endregion

    #region Jogo e Infraestrutura

    /// <summary>
    /// Representa o oponente controlado pela CPU na campanha
    /// </summary>
    class Bot
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Escolhe uma posição aleatória válida, evitando casas ocupadas e bombas já reveladas
        /// </summary>
        public (int linha, int coluna) EscolherPosicao(Tabuleiro tabuleiro, List<Bomba> bombas, OlhosDoJogador olhos)
        {
            // Tenta encontrar casa livre sem bomba conhecida
            var candidatas = new List<(int, int)>();
            for (int i = 0; i < tabuleiro.TamanhoTabuleiro; i++)
                for (int j = 0; j < tabuleiro.TamanhoTabuleiro; j++)
                    if (!tabuleiro.PosicaoOcupada(i, j) && !olhos.JaViu(i, j))
                        candidatas.Add((i, j));

            // Se não houver, aceita casas com bomba conhecida
            if (candidatas.Count == 0)
                for (int i = 0; i < tabuleiro.TamanhoTabuleiro; i++)
                    for (int j = 0; j < tabuleiro.TamanhoTabuleiro; j++)
                        if (!tabuleiro.PosicaoOcupada(i, j))
                            candidatas.Add((i, j));

            var escolha = candidatas[random.Next(candidatas.Count)];
            return (escolha.Item1, escolha.Item2);
        }

        /// <summary>
        /// Retorna o personagem do bot baseado na fase e rodada atual
        /// </summary>
        public Personagem ObterPersonagem(int fase, bool primeiraRodada)
        {
            if (fase % 10 == 0)
                return primeiraRodada
                    ? SelecaoSimbolo.ObterPersonagem(9)  // 💩
                    : SelecaoSimbolo.ObterPersonagem(10); // 😈
            return SelecaoSimbolo.ObterPersonagem((fase - 1) % 10);
        }
    }

    /// <summary>
    /// Controla o progresso da campanha — fase atual, tamanho do tabuleiro e dificuldade
    /// </summary>
    class Campanha
    {
        public int FaseAtual { get; private set; } = 1;
        public int TamanhoTabuleiro => 4 + (FaseAtual - 1) / 30;
        public bool IsBoss => FaseAtual % 10 == 0;
        public bool PrimeiraRodada { get; private set; } = true;
        public int QuantidadeBombas => DificuldadeNaFase switch
        {
            0 => (int)Math.Max(1, Math.Round(TamanhoTabuleiro * TamanhoTabuleiro * 0.1)),
            1 => (int)Math.Max(1, Math.Round(TamanhoTabuleiro * TamanhoTabuleiro * 0.25)),
            2 => (int)Math.Max(1, Math.Round(TamanhoTabuleiro * TamanhoTabuleiro * 0.4)),
            _ => 1
        };
        public int DificuldadeNaFase => ((FaseAtual - 1) % 30) / 10;

        public void AvancarRodada()
        {
            if (IsBoss && PrimeiraRodada)
                PrimeiraRodada = false;
            else
            {
                PrimeiraRodada = true;
                FaseAtual++;
            }
        }

        public void SalvarProgresso()
        {
            File.WriteAllText("save.txt", FaseAtual.ToString());
        }

        public void CarregarProgresso()
        {
            if (File.Exists("save.txt"))
                FaseAtual = int.Parse(File.ReadAllText("save.txt"));
        }
    }

    /// <summary>
    /// Registra e persiste os troféus conquistados na campanha em trofeus.txt
    /// </summary>
    class Trofeus
    {
        public bool CocoBloqueado { get; private set; } = false;
        public bool DiaboBloqueado { get; private set; } = false;

        public void Desbloquear(int fase)
        {
            if (fase >= 110) CocoBloqueado = true;
            if (fase >= 210) DiaboBloqueado = true;
        }

        public void Salvar()
        {
            File.WriteAllText("trofeus.txt", $"{CocoBloqueado}:{DiaboBloqueado}");
        }

        public void Carregar()
        {
            if (File.Exists("trofeus.txt"))
            {
                var partes = File.ReadAllText("trofeus.txt").Split(':');
                CocoBloqueado = bool.Parse(partes[0]);
                DiaboBloqueado = bool.Parse(partes[1]);
            }
        }
    }

    /// <summary>
    /// Registra e persiste o placar do modo Versus em placar.txt
    /// </summary>
    class Placar
    {
        public int PlacarJ1 { get; private set; } = 0;
        public int PlacarJ2 { get; private set; } = 0;
        public void PlacarJogador1() { PlacarJ1++; }
        public void PlacarJogador2() { PlacarJ2++; }

        public void SalvarPlacar()
        {
            File.WriteAllText("placar.txt", $"{PlacarJ1}:{PlacarJ2}");
        }

        public void CarregarPlacar()
        {
            if (File.Exists("placar.txt"))
            {
                var placar = File.ReadAllText("placar.txt").Split(':');
                PlacarJ1 = int.Parse(placar[0]);
                PlacarJ2 = int.Parse(placar[1]);
            }
        }
    }

    /// <summary>
    /// Armazena as configurações do modo Versus — tamanho do tabuleiro e quantidade de bombas
    /// </summary>  
    class Config
    {
        public enum Dificuldade { Fácil = 1, Médio = 2, Difícil = 3, Personalizado = 4 }
        public static int TamanhoTabuleiro { get; private set; } = 3;
        public static int QuantidadeBombas { get; private set; } = 1;
        public static void ConfiguracaoJxJ()
        {
            Menu.ExibirConfiguracao();
            Menu.OpcoesConfiguracao opcaoConfig = Menu.LerOpcao<Menu.OpcoesConfiguracao>();

            switch (opcaoConfig)
            {
                case Menu.OpcoesConfiguracao.Dificuldade:
                    EscolherDificuldade();
                    break;
                case Menu.OpcoesConfiguracao.TamanhoTabuleiro:
                    EscolherTamanhoTabuleiro();
                    break;
                case Menu.OpcoesConfiguracao.Voltar:
                    break;
                default:
                    Console.WriteLine("Opção inválida, digite um número válido.");
                    break;
            }
        }

        public static void EscolherDificuldade()
        {
            Menu.MenuDificuldade();
            while (true)
                if (int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Dificuldade dificuldade = (Dificuldade)opcao;
                    switch (dificuldade)
                    {
                        case Dificuldade.Fácil:
                            QuantidadeBombas = (int)Math.Max(1, Math.Round(TamanhoTabuleiro * TamanhoTabuleiro * 0.1));
                            break;
                        case Dificuldade.Médio:
                            QuantidadeBombas = (int)Math.Max(1, Math.Round(TamanhoTabuleiro * TamanhoTabuleiro * 0.25));
                            break;
                        case Dificuldade.Difícil:
                            QuantidadeBombas = (int)Math.Max(1, Math.Round(TamanhoTabuleiro * TamanhoTabuleiro * 0.4));
                            break;
                        case Dificuldade.Personalizado:
                            int maxBombas = TamanhoTabuleiro * TamanhoTabuleiro - 1;
                            Console.WriteLine($"Digite o número de bombas desejado (1-{maxBombas}):");
                            while (true)
                                if (int.TryParse(Console.ReadLine(), out int bombasPersonalizadas) && bombasPersonalizadas >= 1 && bombasPersonalizadas <= maxBombas)
                                {
                                    QuantidadeBombas = bombasPersonalizadas;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine($"Número inválido, digite um valor entre 1 e {maxBombas}.");
                                    continue;
                                }
                            break;
                        default:
                            Console.WriteLine("Opção inválida, digite um número válido.");
                            continue;
                    }
                    break; // Sai do loop após escolher a dificuldade
                }
                else
                {
                    Console.WriteLine("Opção inválida, digite um número válido.");
                    continue;
                }
        }

        public static void EscolherTamanhoTabuleiro()
        {
            Console.Clear();
            Console.WriteLine("Digite o tamanho da matriz do tabuleiro:");
            Console.WriteLine("Valor mínimo: 3 | Valor máximo: 9");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int tamanho) && tamanho >= 4 && tamanho <= 10)
                {
                    TamanhoTabuleiro = tamanho;
                    break;
                }
                else
                {
                    Console.WriteLine("Tamanho inválido, digite um valor entre 3 e 9.");
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// Responsável por exibir os menus e ler as opções do jogador
    /// </summary>
    class Menu
    {
        public enum SimOuNao { Sim = 1, Nao = 2 }
        public enum SubMenuJogo { JogarNovamente = 1, TrocarPersonagem = 2, VoltarMenu = 3 }
        public enum OpcoesCampanha { Continuar = 1, Reiniciar = 2 }
        public enum OpcoesPlacar { Continuar = 1, Reiniciar = 2 }
        public enum OpcoesMenu { JogarCampanha = 1, VersusMode = 2, ConfiguracaoJxJ = 3, Sair = 4 }
        public enum OpcoesConfiguracao { Dificuldade = 1, TamanhoTabuleiro = 2, Voltar = 3 }
        public static void ExibirMenu()
        {
            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            Console.WriteLine("1 - Jogar Campanha");
            Console.WriteLine("2 - Versus Mode 2 Jogadores");
            Console.WriteLine("3 - Configuração JxJ");
            Console.WriteLine("4 - Sair");
            Console.WriteLine("\nDigite o número da opção desejada:");
        }

        /// <summary>
        /// Lê e converte a entrada do jogador para o enum especificado
        /// </summary>
        public static T LerOpcao<T>()
        {
            while (true)
                if (int.TryParse(Console.ReadLine(), out int opcao))
                {
                    return (T)Enum.ToObject(typeof(T), opcao);
                }
                else
                {
                    Console.WriteLine("Opção inválida, digite um número válido.");
                    continue;
                }
        }

        public static void ExibirConfiguracao()
        {
            Console.Clear();
            Console.WriteLine("=====Configurações JxJ=====\n");
            Console.WriteLine("1 - Dificuldade");
            Console.WriteLine("2 - Tamanho do Tabuleiro");
            Console.WriteLine("3 - Voltar ao Menu Principal");
            Console.WriteLine("\nDigite o número da opção desejada:");
        }

        public static void MenuDificuldade()
        {
            Console.Clear();
            Console.WriteLine("Escolha a dificuldade do jogo:");
            Console.WriteLine("1 - Fácil");
            Console.WriteLine("2 - Médio");
            Console.WriteLine("3 - Difícil");
            Console.WriteLine("4 - Personalizado (defina o número de bombas)");
            Console.WriteLine("\nDigite o número da dificuldade desejada:");
        }

        public static bool MenuCampanha(int faseAtual)
        {
            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            Console.WriteLine($"1 - Continuar Fase {faseAtual}");
            Console.WriteLine("2 - Reiniciar Campanha");
            Console.WriteLine("\nDigite o número da opção desejada:");
            return LerOpcao<OpcoesCampanha>() == OpcoesCampanha.Reiniciar;
        }

        public static bool MenuPlacar(int PlacarJ1, int PlacarJ2)
        {
            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            Console.WriteLine($"1 - Continuar {PlacarJ1}:{PlacarJ2}");
            Console.WriteLine("2 - Reiniciar Placar");
            Console.WriteLine("\nDigite o número da opção desejada:");
            return LerOpcao<OpcoesPlacar>() == OpcoesPlacar.Reiniciar;
        }
    }

    /// <summary>
    /// Verifica condições de fim de jogo e exibe menus de navegação horizontal
    /// </summary>
    class CondicaoDeVitoria
    {
        /// <summary>
        /// Verifica empate: apenas quando não há bombas, não há lama e não há casas livres
        /// </summary>
        public static bool VerificarEmpate(Tabuleiro tabuleiro, List<Bomba> bombas, List<Armadilha> campoDeLama)
        {
            if (campoDeLama.Count > 0) return false;

            for (int i = 0; i < tabuleiro.TamanhoTabuleiro; i++)
                for (int j = 0; j < tabuleiro.TamanhoTabuleiro; j++)
                    if (!tabuleiro.PosicaoOcupada(i, j))
                        return false;

            return true;
        }

        /// <summary>
        /// Revela todas as bombas e exibe a mensagem de derrota
        /// </summary>
        public static void ExibirDerrota(Tabuleiro tabuleiro, List<Bomba> bombas, string mensagem)
        {
            foreach (var b in bombas) b.RevelarBomba(tabuleiro);
            ExibirMatriz.ExibirTabuleiro(tabuleiro);
            Console.WriteLine(mensagem);
            CondicaoDeVitoria.EnterParaContinuar();
        }

        /// <summary>
        /// Revela todas as bombas e exibe a mensagem de empate
        /// </summary>
        public static void ExibirEmpate(Tabuleiro tabuleiro, List<Bomba> bombas)
        {
            foreach (var b in bombas)
            {
                b.RevelarBomba(tabuleiro);
            }

            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            ExibirMatriz.ExibirTabuleiro(tabuleiro);
            Console.WriteLine();
            Console.WriteLine("\n=======EMPATE!=======");
            Console.WriteLine("💣 Os dois Morreram 💣");
            CondicaoDeVitoria.EnterParaContinuar();
        }

        /// <summary>
        /// Exibe opções navegáveis horizontalmente com A/D e retorna o índice escolhido
        /// </summary>
        public static int SelecionarHorizontal(string[] opcoes, string mensagem = "")
        {
            int colunaAtual = 0;

            do
            {
                Console.Clear();
                if (mensagem != "") Console.WriteLine(mensagem + "\n");
                Console.WriteLine("Use A, D para mover | ENTER para confirmar\n");

                ExibirOpcoes(opcoes, colunaAtual, mensagem);

                ConsoleKeyInfo input;
                do
                {
                    input = Console.ReadKey(true);
                } while (input.Key != ConsoleKey.A && input.Key != ConsoleKey.D && input.Key != ConsoleKey.Enter);

                if (input.Key == ConsoleKey.D && colunaAtual < opcoes.Length - 1) colunaAtual++;
                else if (input.Key == ConsoleKey.A && colunaAtual > 0) colunaAtual--;
                else if (input.Key == ConsoleKey.Enter) return colunaAtual;
            } while (true);
        }

        /// <summary>
        /// Renderiza as opções na tela com cursor destacado
        /// </summary>
        public static void ExibirOpcoes(string[] opcoes, int colunaAtual, string mensagem = "")
        {
            for (int j = 0; j < opcoes.Length; j++)
            {
                if (j == colunaAtual)
                {
                    Console.Write($"  ✅ - {opcoes[j]}  ");
                }
                else
                {
                    Console.Write($"  🟪 - {opcoes[j]}  ");
                }
            }
        }

        /// <summary>
        /// Pausa o jogo até que o jogador pressione ENTER
        /// </summary>
        public static void EnterParaContinuar()
        {
            Console.WriteLine("\nPressione ENTER para continuar...");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;
        }
    }

    /// <summary>
    /// Responsável por renderizar o tabuleiro e capturar a navegação do jogador
    /// </summary>
    class ExibirMatriz
    {
        public enum TipoNavegacao { Normal, Area2x2Revelar, Area2x2Pisar, ColunaRevelar, ColunaPisar, LinhaRevelar, LinhaPisar }

        /// <summary>
        /// Captura a navegação do jogador e retorna a posição e modo escolhidos
        /// </summary>
        public static (int linha, int coluna, int modoAtual) ExibirJogada(Tabuleiro tabuleiro, Jogador jogador, OlhosDoJogador? olhos = null, Partida? partida = null, OlhosDoJogador? olhosExtra = null)
        {
            int linhaAtual = 0;
            int colunaAtual = 0;
            int modoAtual = 1;

            ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, olhosExtra: olhosExtra);

            do
            {
                TipoNavegacao nav = TipoNavegacao.Normal;
                bool entradaValida = false;
                ConsoleKeyInfo input;
                do
                {
                    input = Console.ReadKey(true);

                    if (input.Key == ConsoleKey.D ||
                        input.Key == ConsoleKey.A ||
                        input.Key == ConsoleKey.W ||
                        input.Key == ConsoleKey.S ||
                        input.Key == ConsoleKey.Enter ||
                        input.Key == ConsoleKey.D1 ||
                        input.Key == ConsoleKey.D2 ||
                         input.Key == ConsoleKey.D3)
                    {
                        entradaValida = true;
                    }
                    else
                    {
                        entradaValida = false;
                    }
                } while (!entradaValida);

                nav = (modoAtual == 2 && jogador?.Personagem.Habilidade is HabilidadeAtiva ha)
                ? ha.Navegacao
                : TipoNavegacao.Normal;

                if (input.Key == ConsoleKey.D1)
                {
                    modoAtual = 1;
                    nav = TipoNavegacao.Normal;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                    continue;
                }
                else if (input.Key == ConsoleKey.D2)
                {
                    if (jogador?.Personagem.Habilidade is HabilidadeAtiva && jogador.Personagem.Habilidade.Cooldown.Disponivel)
                    {
                        modoAtual = 2;
                        nav = (jogador?.Personagem.Habilidade is HabilidadeAtiva ha2) ? ha2.Navegacao : TipoNavegacao.Normal;
                        ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                    }
                    continue;
                }
                else if (input.Key == ConsoleKey.D3)
                {
                    if (jogador?.Personagem.HabilidadeFaccao is HabilidadeAtiva haFaccao &&
                        jogador.Personagem.HabilidadeFaccao.Cooldown.Disponivel)
                    {
                        modoAtual = 3;
                        nav = haFaccao.Navegacao;
                        ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                    }
                    continue;
                }

                if (input.Key == ConsoleKey.D && colunaAtual < tabuleiro.TamanhoTabuleiro - 1)
                {
                    colunaAtual++;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                }
                else if (input.Key == ConsoleKey.A && colunaAtual > 0)
                {
                    colunaAtual--;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                }
                else if (input.Key == ConsoleKey.S && linhaAtual < tabuleiro.TamanhoTabuleiro - 1)
                {
                    linhaAtual++;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                }
                else if (input.Key == ConsoleKey.W && linhaAtual > 0)
                {
                    linhaAtual--;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    if (modoAtual == 2 || !tabuleiro.PosicaoOcupada(linhaAtual, colunaAtual)
                        || partida?.CampoDeLama.Exists(a => a.TemArmadilha(linhaAtual, colunaAtual)) == true)
                        return (linhaAtual, colunaAtual, modoAtual);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Posição inválida, digite novamente\n");
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual, olhos, jogador, modoAtual, partida, nav, olhosExtra);
                }

            } while (true);
        }

        /// <summary>
        /// Renderiza o tabuleiro com cursor, visibilidade do jogador e UI de habilidades
        /// </summary>
        public static void ExibirTabuleiro(Tabuleiro tabuleiro, int linhaAtual, int colunaAtual, OlhosDoJogador? olhos = null, Jogador? jogador = null, int modoAtual = 1, Partida? partida = null, TipoNavegacao navegacao = TipoNavegacao.Normal, OlhosDoJogador? olhosExtra = null)
        {
            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            Console.WriteLine("Use A, W, S, D para mover | ENTER para confirmar\n");

            for (int i = 0; i < tabuleiro.TamanhoTabuleiro; i++)
            {
                for (int j = 0; j < tabuleiro.TamanhoTabuleiro; j++)
                {
                    bool naArea2x2 = (i == linhaAtual || i == linhaAtual + 1) &&
                    (j == colunaAtual || j == colunaAtual + 1);
                    bool naColuna = (j == colunaAtual);
                    bool naLinha = (i == linhaAtual);

                    if (navegacao == TipoNavegacao.Area2x2Pisar && naArea2x2)
                        Console.Write("  🔳  ");
                    else if (navegacao == TipoNavegacao.Area2x2Revelar && naArea2x2)
                        Console.Write("  🟨  ");
                    else if (navegacao == TipoNavegacao.ColunaRevelar && naColuna)
                        Console.Write("  🟨  ");
                    else if (navegacao == TipoNavegacao.LinhaRevelar && naLinha)
                        Console.Write("  🟨  ");
                    else if (navegacao == TipoNavegacao.Normal && i == linhaAtual && j == colunaAtual)
                        Console.Write("  🔳  ");
                    else if (partida?.Cartas.Exists(c => c.TemCarta(i, j)) == true)
                        Console.Write($"  {partida.Cartas.Find(c => c.TemCarta(i, j))?.Simbolo}  ");
                    else if (((olhos != null && olhos.JaViu(i, j)) || (olhosExtra != null && olhosExtra.JaViu(i, j)))
                    && tabuleiro.Matriz[i, j] != "💥" && tabuleiro.Matriz[i, j] != Deusa.EmojiExplosao)
                    {
                        if (partida?.Armadilhas.Exists(a => a.TemArmadilha(i, j)) == true)
                            Console.Write("  🟥  ");
                        else
                            Console.Write($"  {(Deusa.Ativa ? Deusa.EmojiBomba : "💣")}  ");
                    }
                    else if (tabuleiro.Matriz[i, j] != "⬜")
                        Console.Write($"  {tabuleiro.Matriz[i, j]}  ");
                    else if (olhos != null && olhos?.CasasSeguras.Exists(cs => cs.TemCasaSegura(i, j)) == true)
                        Console.Write("  🟩  ");
                    else
                        Console.Write($"  {(Deusa.Ativa ? "🟦" : tabuleiro.Matriz[i, j])}  ");
                }
                Console.WriteLine("\n");
            }
            string cooldownDisplay = jogador?.Personagem.Habilidade != null
                ? SkillCooldown.ObterRelogio(
                    jogador.Personagem.Habilidade.Cooldown.TurnosRestantes,
                    jogador.Personagem.Habilidade.Cooldown.CooldownTotal)
                : "";

            string nomeHabilidade = jogador?.Personagem.Habilidade?.Nome ?? "Nenhuma";

            string cooldownFaccao = jogador?.Personagem.HabilidadeFaccao != null
            ? SkillCooldown.ObterRelogio(
            jogador.Personagem.HabilidadeFaccao.Cooldown.TurnosRestantes,
            jogador.Personagem.HabilidadeFaccao.Cooldown.CooldownTotal)
            : "";

            string nomeFaccao = jogador?.Personagem.HabilidadeFaccao?.Nome ?? "";

            if (modoAtual == 1)
                Console.WriteLine($"✅ 1️⃣ Minerar | {cooldownDisplay} 2️⃣ {nomeHabilidade} | {cooldownFaccao} 3️⃣ {nomeFaccao}");
            else if (modoAtual == 2)
                Console.WriteLine($"1️⃣ Minerar | ✅ {cooldownDisplay} 2️⃣ {nomeHabilidade} | {cooldownFaccao} 3️⃣ {nomeFaccao}");
            else if (modoAtual == 3)
                Console.WriteLine($"1️⃣ Minerar | {cooldownDisplay} 2️⃣ {nomeHabilidade} | ✅ {cooldownFaccao} 3️⃣ {nomeFaccao}");

            if (linhaAtual >= 0)
            {
                if (modoAtual == 2 && jogador?.Personagem.Habilidade is HabilidadeAtiva ha)
                    Console.WriteLine(ha.MensagemNavegacao);
                else if (modoAtual == 3 && jogador?.Personagem.HabilidadeFaccao is HabilidadeAtiva haFaccao)
                    Console.WriteLine(haFaccao.MensagemNavegacao);
                else
                    Console.WriteLine($"\nColoque o {jogador?.Personagem.Simbolo} em uma posição");
            }
        }

        /// <summary>
        /// Renderiza o tabuleiro sem cursor — usado para exibições finais
        /// </summary>
        public static void ExibirTabuleiro(Tabuleiro tabuleiro)
        {
            ExibirTabuleiro(tabuleiro, -1, -1);
        }
    }

    /// <summary>
    /// Coordena o fluxo principal do jogo — menu, campanha e versus
    /// </summary>
    class GerenciadorDeJogo
    {
        public enum ResultadoJogada { Explodiu, Ocupada, Valida, InicioTurno }

        /// <summary>
        /// Exibe o menu pós-partida e processa a escolha do jogador
        /// </summary>
        private static void GerenciarPosJogo(string mensagem, ref Jogador j1, ref Jogador j2, ref bool faseTerminou, bool ehVersus, Placar placar, int fase = 0)
        {
            Menu.SubMenuJogo opcao = (Menu.SubMenuJogo)(CondicaoDeVitoria.SelecionarHorizontal(
            Enum.GetNames(typeof(Menu.SubMenuJogo)), mensagem) + 1);

            switch (opcao)
            {
                case Menu.SubMenuJogo.JogarNovamente:
                    faseTerminou = true;
                    break;

                case Menu.SubMenuJogo.TrocarPersonagem:
                    // Troca o Jogador 1
                    var (p1, i1) = SelecaoSimbolo.Selecionar("Jogador 1", fase);
                    j1 = new Jogador("Jogador 1", p1, i1);

                    // Se for Versus, troca o Jogador 2 também
                    if (ehVersus && j2 != null)
                    {
                        var (p2, i2) = SelecaoSimbolo.Selecionar("Jogador 2", fase);
                        j2 = new Jogador("Jogador 2", p2, i2);

                        // Aplica a Deusa
                        if (j1.Personagem.Simbolo == j2.Personagem.Simbolo)
                            Deusa.Aplicar(ref j1, ref j2, ehVersus ? Config.TamanhoTabuleiro : 3 + (fase - 1) / 30);
                    }
                    faseTerminou = true;
                    break;

                case Menu.SubMenuJogo.VoltarMenu:
                    break;
            }
        }

        /// <summary>
        /// Loop principal do menu — inicializa os modos de jogo
        /// </summary>
        public static void Executar()
        {
            bool sair = false;
            do
            {
                Menu.ExibirMenu();
                Menu.OpcoesMenu opcao = Menu.LerOpcao<Menu.OpcoesMenu>();

                switch (opcao)
                {
                    case Menu.OpcoesMenu.JogarCampanha:
                        IniciarCampanha();
                        break;
                    case Menu.OpcoesMenu.VersusMode:
                        IniciarJogo();
                        break;
                    case Menu.OpcoesMenu.ConfiguracaoJxJ:
                        Config.ConfiguracaoJxJ();
                        break;
                    case Menu.OpcoesMenu.Sair:
                        Console.WriteLine("Obrigado por jogar! Até a próxima!");
                        Console.ReadLine();
                        sair = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida, tente novamente.");
                        break;
                }
            } while (!sair);
        }

        /// <summary>
        /// Verifica se a jogada acerta bomba, posição ocupada ou é válida
        /// </summary>
        public static ResultadoJogada ProcessarJogada(Partida partida, int linha, int coluna, string simbolo)
        {
            foreach (var b in partida.Bombas)
                if (b.TemBomba(linha, coluna)) return ResultadoJogada.Explodiu;

            if (partida.Tabuleiro.PosicaoOcupada(linha, coluna) && !partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna)))
                return ResultadoJogada.Ocupada;

            partida.Tabuleiro.ColocarSimbolo(linha, coluna, simbolo);
            return ResultadoJogada.Valida;
        }

        /// <summary>
        /// Inicializa e executa o modo Campanha contra o Bot
        /// </summary>
        public static void IniciarCampanha()
        {
            Campanha campanha = new Campanha();
            campanha.CarregarProgresso();
            Trofeus trofeus = new Trofeus();
            trofeus.Carregar();
            int faseSelecao = trofeus.DiaboBloqueado ? 210 : trofeus.CocoBloqueado ? 110 : 0;

            if (campanha.FaseAtual > 1 && Menu.MenuCampanha(campanha.FaseAtual))
            {
                campanha = new Campanha();
                campanha.SalvarProgresso();
            }

            var (personagem, indice) = SelecaoSimbolo.Selecionar("Jogador 1", faseSelecao);
            Jogador jogador1 = new Jogador("Jogador 1", personagem, indice);
            Jogador jogador1Original = jogador1;
            Console.Clear();
            Bot bot = new Bot();
            Console.Clear();

            do
            {
                jogador1 = new Jogador(jogador1Original.Nome,
                SelecaoSimbolo.ObterPersonagem(jogador1Original.IndicePersonagem),
                jogador1Original.IndicePersonagem);

                Partida partida = new Partida(campanha.TamanhoTabuleiro, campanha.QuantidadeBombas);
                Deusa.Resetar();
                jogador1.Personagem.Habilidade?.Cooldown.Resetar();
                jogador1.Personagem.HabilidadeFaccao?.Cooldown.Resetar();

                Personagem personagemBot = bot.ObterPersonagem(campanha.FaseAtual, campanha.PrimeiraRodada);
                Jogador jogadorBot = new Jogador("Bot", personagemBot, 0);

                personagemBot.Habilidade?.Cooldown.Resetar();
                personagemBot.HabilidadeFaccao?.Cooldown.Resetar();

                if (jogador1.Personagem.Simbolo == personagemBot.Simbolo)
                    Deusa.Aplicar(ref jogadorBot, ref jogador1, campanha.TamanhoTabuleiro);
                personagemBot = jogadorBot.Personagem;

                bool faseTerminou = false;
                do
                {
                    // Turno do Jogador 1
                    if (jogador1.Personagem.Habilidade is HabilidadePassiva hpInicio && hpInicio.DeveAtivar(ResultadoJogada.InicioTurno)
                    && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                    {
                        hpInicio.Ativar(partida, partida.OlhosJ1, -1, -1);
                        jogador1.Personagem.Habilidade.Cooldown.Usar();
                        ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1, jogador1, 1, partida);
                        Console.WriteLine(hpInicio.MensagemSobreviveu(jogador1.Personagem));
                        CondicaoDeVitoria.EnterParaContinuar();
                    }

                    if (jogador1.Personagem.HabilidadeFaccao is HabilidadePassiva hpFaccao && hpFaccao.DeveAtivar(ResultadoJogada.InicioTurno)
                    && jogador1.Personagem.HabilidadeFaccao.Cooldown.Disponivel)
                    {
                        hpFaccao.Ativar(partida, partida.OlhosJ1, -1, -1, jogador1);
                        jogador1.Personagem.HabilidadeFaccao.Cooldown.Usar();
                        string msgFaccao = hpFaccao.MensagemSobreviveu(jogador1.Personagem);

                        if (msgFaccao != "")
                        {
                            ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1, jogador1, 1, partida);
                            Console.WriteLine(msgFaccao);
                            CondicaoDeVitoria.EnterParaContinuar();
                        }
                    }

                    OlhosDoJogador? olhosExtra = jogador1.Personagem.Faction == Faction.Boss
                    ? partida.ObterOlhosOponente(partida.OlhosJ1)
                    : null;

                    (int linha, int coluna, int modoAtual) = ExibirMatriz.ExibirJogada(partida.Tabuleiro, jogador1, partida.OlhosJ1, partida, olhosExtra);

                    if (modoAtual == 1)
                    {
                        partida.Cartas.RemoveAll(c => c.TemCarta(linha, coluna));
                        ResultadoJogada resultado = ProcessarJogada(partida, linha, coluna, jogador1.Personagem.Simbolo);

                        if (resultado == ResultadoJogada.Explodiu)
                        {
                            partida.Tabuleiro.ColocarSimbolo(linha, coluna, Deusa.Ativa ? Deusa.EmojiExplosao : "💥");
                            bool sobreviveu = false;

                            if (jogador1.Personagem.Habilidade is HabilidadePassiva hp && hp.DeveAtivar(resultado) && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                            {
                                hp.Ativar(partida, partida.OlhosJ1, linha, coluna);
                                jogador1.Personagem.Habilidade.Cooldown.Usar();
                                ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1);
                                sobreviveu = hp.Revive();
                                Console.WriteLine(sobreviveu ? hp.MensagemSobreviveu(jogador1.Personagem) : hp.MensagemMorreu(jogador1.Personagem));
                                CondicaoDeVitoria.EnterParaContinuar();
                            }

                            if (!sobreviveu)
                            {
                                CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas,
                                Deusa.Ativa ? Deusa.MensagemMorte : "\n💥 BOOM! BOT VENCEU! 💥");
                                string msg = $"💀 Você foi derrotado na fase {campanha.FaseAtual}!";
                                Jogador? jogadorNull = null;
                                Placar? placarNull = null;
                                GerenciarPosJogo(msg, ref jogador1, ref jogadorNull, ref faseTerminou, false, placarNull);
                                jogador1Original = jogador1;
                                if (!faseTerminou) return;
                                break;
                            }
                        }

                        else if (partida.Armadilhas.Exists(a => a.TemArmadilha(linha, coluna)))
                        {
                            var armadilha = partida.Armadilhas.Find(a => a.TemArmadilha(linha, coluna));
                            partida.Tabuleiro.ColocarSimbolo(linha, coluna, partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna)) ? "🪦" : armadilha.Simbolo);
                            bool sobreviveu = false;
                            var resultadoArmadilha = ResultadoJogada.Explodiu;

                            if (jogador1.Personagem.Habilidade is HabilidadePassiva hp && hp.DeveAtivar(resultadoArmadilha) && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                            {
                                hp.Ativar(partida, partida.OlhosJ1, linha, coluna);
                                jogador1.Personagem.Habilidade.Cooldown.Usar();
                                ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1);
                                sobreviveu = hp.Revive();
                                Console.WriteLine(sobreviveu ? hp.MensagemSobreviveu(jogador1.Personagem) : hp.MensagemMorreu(jogador1.Personagem));
                                CondicaoDeVitoria.EnterParaContinuar();
                            }

                            if (!sobreviveu)
                            {
                                CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas, $"\n🛸 Você caiu em uma Armadilha! BOT VENCEU!");
                                string msg = $"💀 Você foi derrotado na fase {campanha.FaseAtual}!";
                                Jogador? jogadorNull = null;
                                Placar? placarNull = null;
                                GerenciarPosJogo(msg, ref jogador1, ref jogadorNull, ref faseTerminou, false, placarNull);
                                jogador1Original = jogador1;
                                if (!faseTerminou) return;
                                break;
                            }
                        }

                        else if (partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna) && a.DonoSimbolo != jogador1.Personagem.Simbolo))
                        {
                            var lama = partida.CampoDeLama.Find(a => a.TemArmadilha(linha, coluna));
                            partida.Tabuleiro.ColocarSimbolo(linha, coluna, "🪦");
                            bool sobreviveu = false;
                            var resultadoLama = ResultadoJogada.Explodiu;

                            if (jogador1.Personagem.Habilidade is HabilidadePassiva hp && hp.DeveAtivar(resultadoLama) && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                            {
                                hp.Ativar(partida, partida.OlhosJ1, linha, coluna);
                                jogador1.Personagem.Habilidade.Cooldown.Usar();
                                ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1);
                                sobreviveu = hp.Revive();
                                Console.WriteLine(sobreviveu ? hp.MensagemSobreviveu(jogador1.Personagem) : hp.MensagemMorreu(jogador1.Personagem));
                                CondicaoDeVitoria.EnterParaContinuar();
                            }

                            if (!sobreviveu)
                            {
                                CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas, lama.MensagemVitoria);
                                string msgFaccao = $"🪦 Você foi enterrado na merda na fase {campanha.FaseAtual}!";
                                Jogador? jogadorNull = null;
                                Placar? placarNull = null;
                                GerenciarPosJogo(msgFaccao, ref jogador1, ref jogadorNull, ref faseTerminou, false, placarNull);
                                jogador1Original = jogador1;
                                if (!faseTerminou) return;
                                break;
                            }
                        }

                        jogador1.Personagem.Habilidade?.Cooldown.PassarTurno();
                        jogador1.Personagem.HabilidadeFaccao?.Cooldown.PassarTurno();

                        if (CondicaoDeVitoria.VerificarEmpate(partida.Tabuleiro, partida.Bombas, partida.CampoDeLama))
                        {
                            CondicaoDeVitoria.ExibirEmpate(partida.Tabuleiro, partida.Bombas);
                            Menu.SubMenuJogo opcao = (Menu.SubMenuJogo)(CondicaoDeVitoria.SelecionarHorizontal(
                                Enum.GetNames(typeof(Menu.SubMenuJogo)),
                                $"💀 Você foi derrotado na fase {campanha.FaseAtual}!") + 1);
                            switch (opcao)
                            {
                                case Menu.SubMenuJogo.TrocarPersonagem:
                                    var (novoPersonagem, novoIndice) = SelecaoSimbolo.Selecionar("Jogador 1");
                                    jogador1 = new Jogador("Jogador 1", novoPersonagem, novoIndice);
                                    break;
                                case Menu.SubMenuJogo.VoltarMenu:
                                    return;
                            }
                            faseTerminou = true;
                            break;
                        }
                    }

                    else if (modoAtual == 2)
                    {
                        if (jogador1.Personagem.Habilidade is HabilidadeAtiva ha && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                        {
                            ha.Ativar(partida, partida.OlhosJ1, linha, coluna, jogador1);
                            jogador1.Personagem.Habilidade.Cooldown.Usar();
                            if (ha.Navegacao.ToString().EndsWith("Revelar"))
                            {
                                jogador1.Personagem.Habilidade.Cooldown.PassarTurno();
                                jogador1.Personagem.HabilidadeFaccao?.Cooldown.PassarTurno();
                                continue;
                            }
                        }
                    }

                    else if (modoAtual == 3)
                    {
                        if (jogador1.Personagem.HabilidadeFaccao is HabilidadeAtiva haFaccao &&
                            jogador1.Personagem.HabilidadeFaccao.Cooldown.Disponivel)
                        {
                            haFaccao.Ativar(partida, partida.OlhosJ1, linha, coluna, jogador1);
                            jogador1.Personagem.HabilidadeFaccao.Cooldown.Usar();
                            if (haFaccao.Navegacao.ToString().EndsWith("Revelar"))
                            {
                                jogador1.Personagem.HabilidadeFaccao.Cooldown.PassarTurno();
                                jogador1.Personagem.Habilidade?.Cooldown.PassarTurno();
                                continue;
                            }
                        }
                    }

                    // Turno do Bot
                    ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1);
                    Console.WriteLine($"{personagemBot.Simbolo} Bot está jogando...");
                    Thread.Sleep(1000);

                    // Passivas do bot (sempre ativas)
                    if (personagemBot.HabilidadeFaccao is HabilidadePassiva hpFaccaoBot && hpFaccaoBot.DeveAtivar(ResultadoJogada.InicioTurno)
                    && personagemBot.HabilidadeFaccao.Cooldown.Disponivel)
                    {
                        hpFaccaoBot.Ativar(partida, partida.OlhosJ2, -1, -1);
                        personagemBot.Faction.Habilidade.Cooldown.Usar();
                    }

                    if (personagemBot.Habilidade is HabilidadePassiva hpBot && hpBot.DeveAtivar(ResultadoJogada.InicioTurno)
                    && personagemBot.Habilidade.Cooldown.Disponivel)
                    {
                        hpBot.Ativar(partida, partida.OlhosJ2, -1, -1);
                        personagemBot.Habilidade.Cooldown.Usar();
                    }

                    // Ativas do bot (Médio/Difícil)
                    bool usouAtiva = false;
                    if (campanha.DificuldadeNaFase >= 1)
                    {
                        HabilidadeAtiva? haEscolhida = null;

                        if (personagemBot.HabilidadeFaccao is HabilidadeAtiva haA3 && personagemBot.HabilidadeFaccao.Cooldown.Disponivel)
                            haEscolhida = haA3;
                        else if (personagemBot.Habilidade is HabilidadeAtiva haA2 && personagemBot.Habilidade.Cooldown.Disponivel)
                            haEscolhida = haA2;

                        if (haEscolhida != null)
                        {
                            (linha, coluna) = bot.EscolherPosicao(partida.Tabuleiro, partida.Bombas, partida.OlhosJ2);
                            Console.WriteLine($"{personagemBot.Simbolo} Bot usou {haEscolhida.Nome}!");
                            Thread.Sleep(1000);
                            haEscolhida.Ativar(partida, partida.OlhosJ2, linha, coluna, jogadorBot);
                            haEscolhida.Cooldown.Usar();

                            if (haEscolhida.Navegacao.ToString().EndsWith("Revelar"))
                            {
                                haEscolhida.Cooldown.PassarTurno();
                                personagemBot.HabilidadeFaccao?.Cooldown.PassarTurno();
                                personagemBot.Habilidade?.Cooldown.PassarTurno();
                            }
                            else
                            {
                                usouAtiva = true;
                                personagemBot.HabilidadeFaccao?.Cooldown.PassarTurno();
                                personagemBot.Habilidade?.Cooldown.PassarTurno();
                            }
                        }
                    }

                    if (!usouAtiva)
                    {
                        (linha, coluna) = bot.EscolherPosicao(partida.Tabuleiro, partida.Bombas, partida.OlhosJ2);
                        partida.Cartas.RemoveAll(c => c.TemCarta(linha, coluna));
                        ResultadoJogada botResultado = ProcessarJogada(partida, linha, coluna, personagemBot.Simbolo);

                        if (botResultado == ResultadoJogada.Explodiu)
                        {
                            partida.Tabuleiro.ColocarSimbolo(linha, coluna, Deusa.Ativa ? Deusa.EmojiExplosao : "💥");
                            bool botSobreviveu = false;

                            if (personagemBot.Habilidade is HabilidadePassiva hpBotExplosao && hpBotExplosao.DeveAtivar(ResultadoJogada.Explodiu)
                            && personagemBot.Habilidade.Cooldown.Disponivel)
                            {
                                hpBotExplosao.Ativar(partida, partida.OlhosJ2, linha, coluna, jogadorBot);
                                personagemBot.Habilidade.Cooldown.Usar();
                                botSobreviveu = hpBotExplosao.Revive();
                            }

                            if (!botSobreviveu)
                            {
                                // continua para a vitória
                            }
                            else
                            {
                                continue; // bot sobreviveu, continua o jogo
                            }
                        }

                        ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro);

                        if (botResultado == ResultadoJogada.Explodiu)
                        {
                            CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas,
                            Deusa.Ativa ? $"\n🧊 O BOT CONGELOU! VITÓRIA NA FASE {campanha.FaseAtual}! 🧊"
                            : $"\n🎉 O BOT EXPLODIU! VITÓRIA NA FASE {campanha.FaseAtual}! 🎉");
                            string msgProximaFase = campanha.IsBoss && campanha.PrimeiraRodada
                            ? $"Deseja enfrentar o 😈 Diabo?"
                            : $"Deseja avançar para a próxima fase? Fase {campanha.FaseAtual + 1}";
                            Menu.SimOuNao escolha = (Menu.SimOuNao)(CondicaoDeVitoria.SelecionarHorizontal(
                                Enum.GetNames(typeof(Menu.SimOuNao)),
                                msgProximaFase) + 1);

                            campanha.AvancarRodada();
                            campanha.SalvarProgresso();
                            trofeus.Desbloquear(campanha.FaseAtual);
                            trofeus.Salvar();
                            faseTerminou = true;

                            if (escolha == Menu.SimOuNao.Nao) return;
                            break;
                        }

                        else if (partida.Armadilhas.Exists(a => a.TemArmadilha(linha, coluna)))
                        {
                            var armadilha = partida.Armadilhas.Find(a => a.TemArmadilha(linha, coluna));
                            partida.Tabuleiro.ColocarSimbolo(linha, coluna, partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna)) ? "🪦" : armadilha.Simbolo);
                            CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas, $"\n🛸 O BOT FOI ABDUZIDO! VITÓRIA NA FASE {campanha.FaseAtual}! 🛸");
                            string msgProximaFase = campanha.IsBoss && campanha.PrimeiraRodada
                            ? $"Deseja enfrentar o 😈 Diabo?"
                            : $"Deseja avançar para a próxima fase? Fase {campanha.FaseAtual + 1}";
                            Menu.SimOuNao escolha = (Menu.SimOuNao)(CondicaoDeVitoria.SelecionarHorizontal(
                                Enum.GetNames(typeof(Menu.SimOuNao)),
                                msgProximaFase) + 1);

                            campanha.AvancarRodada();
                            campanha.SalvarProgresso();
                            trofeus.Desbloquear(campanha.FaseAtual);
                            trofeus.Salvar();
                            faseTerminou = true;

                            if (escolha == Menu.SimOuNao.Nao) return;
                            break;
                        }

                        else if (partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna) && a.DonoSimbolo != personagemBot.Simbolo))
                        {
                            partida.Tabuleiro.ColocarSimbolo(linha, coluna, "🪦");
                            CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas, $"\n🪦 O BOT FOI ENTERRADO NA MERDA! VITÓRIA NA FASE {campanha.FaseAtual}! 🪦");
                            string msgProximaFase = campanha.IsBoss && campanha.PrimeiraRodada
                                ? $"Deseja enfrentar o 😈 Diabo?"
                                : $"Deseja avançar para a próxima fase? Fase {campanha.FaseAtual + 1}";
                            Menu.SimOuNao escolha = (Menu.SimOuNao)(CondicaoDeVitoria.SelecionarHorizontal(
                                Enum.GetNames(typeof(Menu.SimOuNao)),
                                msgProximaFase) + 1);

                            campanha.AvancarRodada();
                            campanha.SalvarProgresso();
                            trofeus.Desbloquear(campanha.FaseAtual);
                            trofeus.Salvar();
                            faseTerminou = true;

                            if (escolha == Menu.SimOuNao.Nao) return;
                            break;
                        }

                        if (CondicaoDeVitoria.VerificarEmpate(partida.Tabuleiro, partida.Bombas, partida.CampoDeLama))
                        {
                            CondicaoDeVitoria.ExibirEmpate(partida.Tabuleiro, partida.Bombas);
                            Menu.SubMenuJogo opcao = (Menu.SubMenuJogo)(CondicaoDeVitoria.SelecionarHorizontal(
                                Enum.GetNames(typeof(Menu.SubMenuJogo)),
                                $"💀 Você foi derrotado na fase {campanha.FaseAtual}!") + 1);
                            switch (opcao)
                            {
                                case Menu.SubMenuJogo.TrocarPersonagem:
                                    var (novoPersonagem, novoIndice) = SelecaoSimbolo.Selecionar("Jogador 1");
                                    jogador1 = new Jogador("Jogador 1", novoPersonagem, novoIndice);
                                    break;
                                case Menu.SubMenuJogo.VoltarMenu:
                                    return;
                            }

                            faseTerminou = true;
                            break;
                        }
                    }

                } while (!faseTerminou);
            } while (true);
        }

        /// <summary>
        /// Inicializa e executa o modo Versus entre dois jogadores
        /// </summary>
        public static void IniciarJogo()
        {
            Campanha campanha = new Campanha();
            campanha.CarregarProgresso();
            Placar placar = new Placar();
            placar.CarregarPlacar();
            Trofeus trofeus = new Trofeus();
            trofeus.Carregar();
            int faseSelecao = trofeus.DiaboBloqueado ? 210 : trofeus.CocoBloqueado ? 110 : 0;

            if (Menu.MenuPlacar(placar.PlacarJ1, placar.PlacarJ2))
            {
                placar = new Placar();
                placar.SalvarPlacar();
            }

            var (personagem, indice) = SelecaoSimbolo.Selecionar("Jogador 1", faseSelecao);
            Jogador jogador1 = new Jogador("Jogador 1", personagem, indice);
            Console.Clear();
            var (personagem2, indice2) = SelecaoSimbolo.Selecionar("Jogador 2", faseSelecao);
            Jogador jogador2 = new Jogador("Jogador 2", personagem2, indice2);

            if (jogador1.Personagem.Simbolo == jogador2.Personagem.Simbolo)
                Deusa.Aplicar(ref jogador1, ref jogador2, Config.TamanhoTabuleiro);
            Console.Clear();

            do
            {
                Partida partida = new Partida(Config.TamanhoTabuleiro, Config.QuantidadeBombas);
                jogador1.Personagem.Habilidade?.Cooldown.Resetar();
                jogador2.Personagem.Habilidade?.Cooldown.Resetar();
                jogador1.Personagem.HabilidadeFaccao?.Cooldown.Resetar();
                jogador2.Personagem.HabilidadeFaccao?.Cooldown.Resetar();

                bool faseTerminou = false;
                bool turnoJogador1 = true;
                do
                {
                    if (turnoJogador1)
                    {
                        if (jogador1.Personagem.Habilidade is HabilidadePassiva hpInicio &&
                        hpInicio.DeveAtivar(ResultadoJogada.InicioTurno) && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                        {
                            ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1, jogador1, 1, partida);
                            hpInicio.Ativar(partida, partida.OlhosJ1, -1, -1);
                            jogador1.Personagem.Habilidade.Cooldown.Usar();
                            Console.WriteLine(hpInicio.MensagemSobreviveu(jogador1.Personagem));
                            CondicaoDeVitoria.EnterParaContinuar();
                        }

                        if (jogador1.Personagem.HabilidadeFaccao is HabilidadePassiva hpFaccao && hpFaccao.DeveAtivar(ResultadoJogada.InicioTurno)
                        && jogador1.Personagem.HabilidadeFaccao.Cooldown.Disponivel)
                        {
                            hpFaccao.Ativar(partida, partida.OlhosJ1, -1, -1, jogador1);
                            jogador1.Personagem.HabilidadeFaccao.Cooldown.Usar();
                            string msgFaccao = hpFaccao.MensagemSobreviveu(jogador1.Personagem);
                            if (msgFaccao != "")
                            {
                                ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1, jogador1, 1, partida);
                                Console.WriteLine(msgFaccao);
                                CondicaoDeVitoria.EnterParaContinuar();
                            }
                        }

                        OlhosDoJogador? olhosExtra = jogador1.Personagem.Faction == Faction.Boss
                        ? partida.ObterOlhosOponente(partida.OlhosJ1)
                        : null;

                        (int linha, int coluna, int modoAtual) = ExibirMatriz.ExibirJogada(partida.Tabuleiro, jogador1, partida.OlhosJ1, partida, olhosExtra);

                        if (modoAtual == 1)
                        {
                            partida.Cartas.RemoveAll(c => c.TemCarta(linha, coluna));
                            ResultadoJogada resultado = ProcessarJogada(partida, linha, coluna, jogador1.Personagem.Simbolo);
                            if (resultado == ResultadoJogada.Explodiu)
                            {
                                partida.Tabuleiro.ColocarSimbolo(linha, coluna, Deusa.Ativa ? Deusa.EmojiExplosao : "💥");
                                bool sobreviveu = false;

                                if (jogador1.Personagem.Habilidade is HabilidadePassiva hp1 && hp1.DeveAtivar(resultado) && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                                {
                                    hp1.Ativar(partida, partida.OlhosJ1, linha, coluna);
                                    jogador1.Personagem.Habilidade.Cooldown.Usar();
                                    ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1);
                                    sobreviveu = hp1.Revive();
                                    Console.WriteLine(sobreviveu ? hp1.MensagemSobreviveu(jogador1.Personagem) : hp1.MensagemMorreu(jogador1.Personagem));
                                    CondicaoDeVitoria.EnterParaContinuar();
                                }

                                if (!sobreviveu)
                                {
                                    CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas,
                                    Deusa.Ativa ? Deusa.MensagemMorte : $"\n💥 BOOM! {jogador2.Personagem.Simbolo} VENCEU! 💥");
                                    placar.PlacarJogador2();
                                    string msg = $"💀 Você foi derrotado pelo {jogador2.Personagem.Simbolo} {placar.PlacarJ1} x {placar.PlacarJ2}!";
                                    placar.SalvarPlacar();
                                    GerenciarPosJogo(msg, ref jogador1, ref jogador2, ref faseTerminou, true, placar, campanha.FaseAtual);
                                    if (!faseTerminou) return;
                                    break;
                                }
                            }

                            else if (partida.Armadilhas.Exists(a => a.TemArmadilha(linha, coluna)) ||
                            partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna) && a.DonoSimbolo != jogador1.Personagem.Simbolo))
                            {
                                var armadilha = partida.Armadilhas.Find(a => a.TemArmadilha(linha, coluna))
                                             ?? partida.CampoDeLama.Find(a => a.TemArmadilha(linha, coluna));
                                partida.Tabuleiro.ColocarSimbolo(linha, coluna, partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna)) ? "🪦" : armadilha.Simbolo);
                                bool sobreviveu = false;
                                var resultadoArmadilha = ResultadoJogada.Explodiu;

                                if (jogador1.Personagem.Habilidade is HabilidadePassiva hp1 && hp1.DeveAtivar(resultadoArmadilha) && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                                {
                                    hp1.Ativar(partida, partida.OlhosJ1, linha, coluna);
                                    jogador1.Personagem.Habilidade.Cooldown.Usar();
                                    ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ1);
                                    sobreviveu = hp1.Revive();
                                    Console.WriteLine(sobreviveu ? hp1.MensagemSobreviveu(jogador1.Personagem) : hp1.MensagemMorreu(jogador1.Personagem));
                                    CondicaoDeVitoria.EnterParaContinuar();
                                }

                                if (!sobreviveu)
                                {
                                    CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas, armadilha.MensagemVitoria);
                                    placar.PlacarJogador2();
                                    string msg = $"💀 Você foi derrotado pelo {jogador2.Personagem.Simbolo} {placar.PlacarJ1} x {placar.PlacarJ2}!";
                                    placar.SalvarPlacar();
                                    GerenciarPosJogo(msg, ref jogador1, ref jogador2, ref faseTerminou, true, placar, campanha.FaseAtual);
                                    if (!faseTerminou) return;
                                    break;
                                }
                            }

                            jogador1.Personagem.Habilidade.Cooldown.PassarTurno();
                            jogador1.Personagem.HabilidadeFaccao?.Cooldown.PassarTurno();
                            turnoJogador1 = false; // passa pro jogador 2

                            if (CondicaoDeVitoria.VerificarEmpate(partida.Tabuleiro, partida.Bombas, partida.CampoDeLama))
                            {
                                CondicaoDeVitoria.ExibirEmpate(partida.Tabuleiro, partida.Bombas);
                                string msg = $"💀 OS DOIS MORRERAM! 💀";
                                GerenciarPosJogo(msg, ref jogador1, ref jogador2, ref faseTerminou, true, placar, campanha.FaseAtual);
                                if (!faseTerminou) return;
                                break;
                            }
                        }

                        else if (modoAtual == 2)
                        {
                            if (jogador1.Personagem.Habilidade is HabilidadeAtiva ha && jogador1.Personagem.Habilidade.Cooldown.Disponivel)
                            {
                                ha.Ativar(partida, partida.OlhosJ1, linha, coluna, jogador1);
                                jogador1.Personagem.Habilidade.Cooldown.Usar();
                                // só passa o turno se for Pisar
                                if (!ha.Navegacao.ToString().EndsWith("Revelar"))
                                    turnoJogador1 = false;
                                continue;
                            }
                        }

                        else if (modoAtual == 3)
                        {
                            if (jogador1.Personagem.HabilidadeFaccao is HabilidadeAtiva haFaccao &&
                                jogador1.Personagem.HabilidadeFaccao.Cooldown.Disponivel)
                            {
                                haFaccao.Ativar(partida, partida.OlhosJ1, linha, coluna, jogador1);
                                jogador1.Personagem.HabilidadeFaccao.Cooldown.Usar();
                                if (!haFaccao.Navegacao.ToString().EndsWith("Revelar"))
                                    turnoJogador1 = false;
                                continue;
                            }
                        }
                    }

                    else
                    {
                        if (jogador2.Personagem.Habilidade is HabilidadePassiva hpInicio2 &&
                            hpInicio2.DeveAtivar(ResultadoJogada.InicioTurno) && jogador2.Personagem.Habilidade.Cooldown.Disponivel)
                        {
                            ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ2, jogador2, 1, partida);
                            hpInicio2.Ativar(partida, partida.OlhosJ2, -1, -1);
                            jogador2.Personagem.Habilidade.Cooldown.Usar();
                            Console.WriteLine(hpInicio2.MensagemSobreviveu(jogador2.Personagem));
                            CondicaoDeVitoria.EnterParaContinuar();
                        }

                        if (jogador2.Personagem.HabilidadeFaccao is HabilidadePassiva hpFaccao && hpFaccao.DeveAtivar(ResultadoJogada.InicioTurno)
                        && jogador2.Personagem.HabilidadeFaccao.Cooldown.Disponivel)
                        {
                            hpFaccao.Ativar(partida, partida.OlhosJ2, -1, -1, jogador2);
                            jogador2.Personagem.HabilidadeFaccao.Cooldown.Usar();
                            string msgFaccao = hpFaccao.MensagemSobreviveu(jogador2.Personagem);
                            if (msgFaccao != "")
                            {
                                ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ2, jogador2, 1, partida);
                                Console.WriteLine(msgFaccao);
                                CondicaoDeVitoria.EnterParaContinuar();
                            }
                        }

                        OlhosDoJogador? olhosExtra = jogador2.Personagem.Faction == Faction.Boss
                        ? partida.ObterOlhosOponente(partida.OlhosJ2)
                        : null;

                        (int linha, int coluna, int modoAtual) = ExibirMatriz.ExibirJogada(partida.Tabuleiro, jogador2, partida.OlhosJ2, partida, olhosExtra);

                        if (modoAtual == 1)
                        {
                            partida.Cartas.RemoveAll(c => c.TemCarta(linha, coluna));
                            ResultadoJogada resultado = ProcessarJogada(partida, linha, coluna, jogador2.Personagem.Simbolo);

                            if (resultado == ResultadoJogada.Explodiu)
                            {
                                partida.Tabuleiro.ColocarSimbolo(linha, coluna, Deusa.Ativa ? Deusa.EmojiExplosao : "💥");
                                bool sobreviveu = false;

                                if (jogador2.Personagem.Habilidade is HabilidadePassiva hp2 && hp2.DeveAtivar(resultado) && jogador2.Personagem.Habilidade.Cooldown.Disponivel)
                                {
                                    hp2.Ativar(partida, partida.OlhosJ2, linha, coluna);
                                    jogador2.Personagem.Habilidade.Cooldown.Usar();
                                    ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ2);
                                    sobreviveu = hp2.Revive();
                                    Console.WriteLine(sobreviveu ? hp2.MensagemSobreviveu(jogador2.Personagem) : hp2.MensagemMorreu(jogador2.Personagem));
                                    CondicaoDeVitoria.EnterParaContinuar();
                                }

                                if (!sobreviveu)
                                {
                                    placar.PlacarJogador1();
                                    CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas,
                                    Deusa.Ativa ? Deusa.MensagemMorte : $"\n💥 BOOM! {jogador1.Personagem.Simbolo} VENCEU! 💥");
                                    string msg = $"💀 Você foi derrotado pelo {jogador1.Personagem.Simbolo} {placar.PlacarJ1} x {placar.PlacarJ2}!";
                                    placar.SalvarPlacar();
                                    GerenciarPosJogo(msg, ref jogador1, ref jogador2, ref faseTerminou, true, placar, campanha.FaseAtual);
                                    if (!faseTerminou) return;
                                    break;
                                }
                            }

                            else if (partida.Armadilhas.Exists(a => a.TemArmadilha(linha, coluna)) ||
                            partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna) && a.DonoSimbolo != jogador2.Personagem.Simbolo))
                            {
                                var armadilha = partida.Armadilhas.Find(a => a.TemArmadilha(linha, coluna))
                                             ?? partida.CampoDeLama.Find(a => a.TemArmadilha(linha, coluna));
                                partida.Tabuleiro.ColocarSimbolo(linha, coluna, partida.CampoDeLama.Exists(a => a.TemArmadilha(linha, coluna)) ? "🪦" : armadilha.Simbolo);
                                bool sobreviveu = false;
                                var resultadoArmadilha = ResultadoJogada.Explodiu;

                                if (jogador2.Personagem.Habilidade is HabilidadePassiva hp2 && hp2.DeveAtivar(resultadoArmadilha) && jogador2.Personagem.Habilidade.Cooldown.Disponivel)
                                {
                                    hp2.Ativar(partida, partida.OlhosJ2, linha, coluna, jogador2);
                                    jogador2.Personagem.Habilidade.Cooldown.Usar();
                                    ExibirMatriz.ExibirTabuleiro(partida.Tabuleiro, -1, -1, partida.OlhosJ2);
                                    sobreviveu = hp2.Revive();
                                    Console.WriteLine(sobreviveu ? hp2.MensagemSobreviveu(jogador2.Personagem) : hp2.MensagemMorreu(jogador2.Personagem));
                                    CondicaoDeVitoria.EnterParaContinuar();
                                }

                                if (!sobreviveu)
                                {
                                    placar.PlacarJogador1();
                                    CondicaoDeVitoria.ExibirDerrota(partida.Tabuleiro, partida.Bombas, armadilha.MensagemVitoria);
                                    string msg = $"💀 Você foi derrotado pelo {jogador1.Personagem.Simbolo} {placar.PlacarJ1} x {placar.PlacarJ2}!";
                                    placar.SalvarPlacar();
                                    GerenciarPosJogo(msg, ref jogador1, ref jogador2, ref faseTerminou, true, placar, campanha.FaseAtual);
                                    if (!faseTerminou) return;
                                    break;
                                }
                            }

                            jogador2.Personagem.Habilidade?.Cooldown.PassarTurno();
                            jogador2.Personagem.HabilidadeFaccao?.Cooldown.PassarTurno();
                            turnoJogador1 = true; // passa pro jogador 1

                            if (CondicaoDeVitoria.VerificarEmpate(partida.Tabuleiro, partida.Bombas, partida.CampoDeLama))
                            {
                                CondicaoDeVitoria.ExibirEmpate(partida.Tabuleiro, partida.Bombas);
                                string msg = $"💀 OS DOIS MORRERAM! 💀";
                                GerenciarPosJogo(msg, ref jogador1, ref jogador2, ref faseTerminou, true, placar, campanha.FaseAtual);
                                if (!faseTerminou) return;
                                break;
                            }
                        }

                        else if (modoAtual == 2)
                        {
                            if (jogador2.Personagem.Habilidade is HabilidadeAtiva ha && jogador2.Personagem.Habilidade.Cooldown.Disponivel)
                            {
                                ha.Ativar(partida, partida.OlhosJ2, linha, coluna, jogador2);
                                jogador2.Personagem.Habilidade.Cooldown.Usar();
                                // só passa o turno se for Pisar
                                if (!ha.Navegacao.ToString().EndsWith("Revelar"))
                                    turnoJogador1 = true;
                                continue;
                            }
                        }

                        else if (modoAtual == 3)
                        {
                            if (jogador2.Personagem.HabilidadeFaccao is HabilidadeAtiva haFaccao &&
                                jogador2.Personagem.HabilidadeFaccao.Cooldown.Disponivel)
                            {
                                haFaccao.Ativar(partida, partida.OlhosJ2, linha, coluna, jogador2);
                                jogador2.Personagem.HabilidadeFaccao.Cooldown.Usar();
                                if (!haFaccao.Navegacao.ToString().EndsWith("Revelar"))
                                    turnoJogador1 = true;
                                continue;
                            }
                        }
                    }
                } while (!faseTerminou);
            } while (true);
        }
    }

    /// <summary>
    /// Ponto de entrada do jogo
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Permite exibir emojis corretamente
            GerenciadorDeJogo.Executar();
        }
    }

    #endregion
}
