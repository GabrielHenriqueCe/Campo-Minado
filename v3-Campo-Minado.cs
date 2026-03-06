using System;

namespace Pratica
{
    /// <summary>
    /// Representa o tabuleiro do jogo
    /// </summary>
    class Tabuleiro
    {
        // CAMPOS PRIVADOS (Encapsulamento - dados internos protegidos)
        private string[,] matriz; // Array 2D que armazena o estado do tabuleiro
        
        // PROPERTY AUTO-IMPLEMENTADA (Encapsulamento simplificado)
        public int TamanhoMatriz { get; private set; }

        // CONSTRUCTOR (Inicializa o objeto quando criado com 'new Tabuleiro(3)')
        public Tabuleiro(int tamanho)
        {
            TamanhoMatriz = tamanho;
            matriz = new string[TamanhoMatriz, TamanhoMatriz];

            // Inicializa todas as posições com quadrados vazios
            for (int i = 0; i < TamanhoMatriz; i++)
            {
                for (int j = 0; j < TamanhoMatriz; j++)
                {
                    matriz[i, j] = "◻️";
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
            return matriz[linha, coluna] != "◻️";
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
    /// Responsável por exibir e gerenciar as jogadas no tabuleiro
    /// </summary>
    class ExibirMatriz
    {
        /// <summary>
        /// Permite que o jogador navegue pelo tabuleiro e faça sua jogada
        /// </summary>
        /// <param name="matriz">Matriz 3x3 do tabuleiro do jogo</param>
        /// <param name="simbolo">Emoji do jogador atual</param>
        /// <returns>True se o jogador acertou uma bomba, False caso contrário</returns>
        public static bool ExibirJogada(Tabuleiro tabuleiro, Bomba bomba, string simbolo)
        {
            int linhaAtual = 0;
            int colunaAtual = 0;

            ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);

            do
            {
                Console.WriteLine();
                Console.WriteLine($"\nColoque o {simbolo} em uma posição\n");

                // Valida entrada (apenas WASD e Enter)
                bool entradaValida = false;
                ConsoleKeyInfo input;
                do
                {
                    input = Console.ReadKey(true);

                    if (input.Key == ConsoleKey.D ||
                        input.Key == ConsoleKey.A ||
                        input.Key == ConsoleKey.W ||
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

                if (input.Key == ConsoleKey.D && colunaAtual < 2)
                {
                    colunaAtual++;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.A && colunaAtual > 0)
                {
                    colunaAtual--;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.S && linhaAtual < 2)
                {
                    linhaAtual++;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.W && linhaAtual > 0)
                {
                    linhaAtual--;
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    if (bomba.TemBomba(linhaAtual, colunaAtual))
                    {
                        // BOOM! 💣
                        bomba.RevelarBomba(tabuleiro);
                        Console.WriteLine();
                        return true;
                    }
                    else if (tabuleiro.PosicaoOcupada(linhaAtual, colunaAtual))
                    {
                        ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);
                        Console.WriteLine("\nPosição já ocupada, escolha outra!\n");
                    }
                    else
                    {
                        tabuleiro.ColocarSimbolo(linhaAtual, colunaAtual, simbolo);
                        return false;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Posição inválida, digite novamente\n");
                    ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);
                }

            } while (true);
        }

        /// <summary>
        /// Exibe o tabuleiro com o cursor na posição atual
        /// </summary>
        /// <param name="tabuleiro">Objeto Tabuleiro contendo a matriz e tamanho</param>
        /// <param name="linhaAtual">Linha onde está o cursor (-1 para ocultar)</param>
        /// <param name="colunaAtual">Coluna onde está o cursor (-1 para ocultar)</param>
        public static void ExibirTabuleiro(Tabuleiro tabuleiro, int linhaAtual, int colunaAtual)
        {
            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            Console.WriteLine("Use A, W, S, D para mover | ENTER para confirmar\n");

            for (int i = 0; i < tabuleiro.TamanhoMatriz; i++)
            {
                for (int j = 0; j < tabuleiro.TamanhoMatriz; j++)
                {
                    if (i == linhaAtual && j == colunaAtual)
                    {
                        Console.Write("  ◼️  ");
                    }
                    else
                    {
                        Console.Write($"  {tabuleiro.Matriz[i, j]}  ");
                    }
                }
                Console.WriteLine("\n");
            }
        }
        public static void ExibirTabuleiro(Tabuleiro tabuleiro)
        {
            ExibirTabuleiro(tabuleiro, -1, -1);
        }
    }

    /// <summary>
    /// Responsável por verificar condições de vitória, empate e controlar o fluxo do jogo
    /// </summary>
    class CondicaoDeVitoria
    {
        /// <summary>
        /// Verifica se houve empate (todas as posições livres são bombas)
        /// </summary>
        /// <param name="matriz">Matriz 3x3 do tabuleiro</param>
        /// <returns>True se houve empate, False caso contrário</returns>
        public static bool VerificarEmpate(Tabuleiro tabuleiro, Bomba bomba)
        {
            for (int i = 0; i < tabuleiro.TamanhoMatriz; i++)
            {
                for (int j = 0; j < tabuleiro.TamanhoMatriz; j++)
                {
                    // Se a posição está vazia E não é bomba, ainda dá pra jogar
                    if (!tabuleiro.PosicaoOcupada(i, j) && !bomba.TemBomba(i, j))
                    {
                        return false;  // Não é empate, ainda tem jogada válida
                    }
                }
            }
            return true;  // Empate! Só sobrou a bomba
        }

        /// <summary>
        /// Revela todas as bombas e exibe mensagem de empate
        /// </summary>
        /// <param name="campoMinado">Matriz 3x3 do tabuleiro</param>
        public static void ExibirEmpate(Tabuleiro tabuleiro, Bomba bomba)
        {
            bomba.RevelarBomba(tabuleiro);

            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            ExibirMatriz.ExibirTabuleiro(tabuleiro);
            Console.WriteLine();
            Console.WriteLine("\n=======EMPATE!=======");
            Console.WriteLine("💣 Os dois Morreram 💣");
        }

        private static readonly string[] resposta = { "Sim", "Não" };

        /// <summary>
        /// Exibe as opções Sim/Não com cursor na posição atual
        /// </summary>
        /// <param name="colunaAtual">Posição do cursor (0=Sim, 1=Não)</param>
        public static void ExibirSimouNao(int colunaAtual)
        {
            Console.Clear();
            Console.WriteLine("Use A, D para mover | ENTER para confirmar\n");

            for (int j = 0; j < resposta.Length; j++)
            {
                if (j == colunaAtual)
                {
                    Console.Write($"  ✅ - {resposta[j]}  ");
                }
                else
                {
                    Console.Write($"  🟪 - {resposta[j]}  ");
                }
            }
        }

        /// <summary>
        /// Pergunta ao jogador se deseja jogar novamente usando A/D para navegar
        /// </summary>
        /// <returns>True se escolheu Sim, False se escolheu Não</returns>
        public static bool JogarNovamente()
        {
            int colunaAtual = 0;
            ExibirSimouNao(colunaAtual);

            do
            {
                bool entradaValida = false;
                ConsoleKeyInfo input;
                do
                {
                    input = Console.ReadKey(true);

                    if (input.Key == ConsoleKey.A ||
                        input.Key == ConsoleKey.D ||
                        input.Key == ConsoleKey.Enter)
                    {
                        entradaValida = true;
                    }
                    else
                    {
                        entradaValida = false;
                    }
                } while (!entradaValida);

                if (input.Key == ConsoleKey.D && colunaAtual < resposta.Length - 1)
                {
                    colunaAtual++;
                    ExibirSimouNao(colunaAtual);
                }
                else if (input.Key == ConsoleKey.A && colunaAtual > 0)
                {
                    colunaAtual--;
                    ExibirSimouNao(colunaAtual);
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    ExibirSimouNao(colunaAtual);
                    if (colunaAtual == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Posição inválida, digite novamente\n");
                    ExibirSimouNao(colunaAtual);
                }
            } while (true);
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
    /// Representa um jogador com nome e símbolo
    /// </summary>
    class Jogador
    {
        // Properties (encapsulamento)
        public string Nome { get; private set; }
        public string Simbolo { get; private set; }

        // Constructor
        public Jogador(string nome, string simbolo)
        {
            Nome = nome;
            Simbolo = simbolo;
        }
    }

    /// <summary>
    /// Responsável pela interface de seleção de símbolos
    /// </summary>
    class SelecaoSimbolo
    {
        private static readonly string[] simbolos = { "💀", "👽", "💩", "🤖", "👹", "👻", "👾" };

        /// <summary>
        /// Retorna o número total de símbolos disponíveis
        /// </summary>
        /// <returns>Quantidade de símbolos no array</returns>
        public static int TotalSimbolos()
        {
            return simbolos.Length;
        }

        /// <summary>
        /// Exibe a lista de símbolos disponíveis com cursor na linha especificada
        /// </summary>
        /// <param name="player">Nome do jogador (ex: "Jogador 1")</param>
        /// <param name="linhaAtual">Índice do símbolo selecionado</param>
        public static void ExibirSimbolo(string player, int linhaAtual)
        {
            Console.Clear();
            Console.WriteLine("Use W, S para mover | ENTER para confirmar\n");

            for (int i = 0; i < simbolos.Length; i++)
            {
                if (i == linhaAtual)
                {
                    Console.Write($"✅ - {simbolos[i]}");
                }
                else
                {
                    Console.Write($"☑️ - {simbolos[i]}");
                }
                Console.WriteLine("\n");
            }
        }

        /// <summary>
        /// Permite que o jogador escolha seu símbolo navegando com W/S
        /// </summary>
        /// <param name="player">Nome do jogador (ex: "Jogador 1")</param>
        /// <returns>Emoji escolhido pelo jogador</returns>
        public static string Selecionar(string player)
        {
            int linhaAtual = 0;
            ExibirSimbolo(player, linhaAtual);

            do
            {
                Console.WriteLine();
                Console.WriteLine($"\nSelecione o Simbolo do {player}\n");

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

                if (input.Key == ConsoleKey.S && linhaAtual < simbolos.Length - 1)
                {
                    linhaAtual++;
                    ExibirSimbolo(player, linhaAtual);
                }
                else if (input.Key == ConsoleKey.W && linhaAtual > 0)
                {
                    linhaAtual--;
                    ExibirSimbolo(player, linhaAtual);
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    ExibirSimbolo(player, linhaAtual);
                    return simbolos[linhaAtual];
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Posição inválida, digite novamente\n");
                    ExibirSimbolo(player, linhaAtual);
                }
            } while (true);
        }
    }

    /// <summary>
    /// Representa uma bomba escondida no tabuleiro
    /// </summary>
    class Bomba
    {
        // CAMPOS PRIVADOS (Encapsulamento) - cada objeto Bomba tem sua própria posição
        private int linha;
        private int coluna;

        // CONSTRUCTOR - inicializa a bomba em posição aleatória
        public Bomba(int tamanhoTabuleiro)
        {
            Random random = new Random();
            int posicaoBomba = random.Next(1, tamanhoTabuleiro * tamanhoTabuleiro + 1);

            this.linha = (posicaoBomba - 1) / tamanhoTabuleiro;
            this.coluna = (posicaoBomba - 1) % tamanhoTabuleiro;
            /*DEBUG: 
            Console.WriteLine($"Bomba na posição {posicaoBomba} (linha {linha}, coluna {coluna})");
            Console.ReadKey();
            */
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
            tabuleiro.ColocarSimbolo(linha, coluna, "💣");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool jogarNovamente = true;

            // LOOP EXTERNO - permite jogar múltiplas partidas
            do
            {
                //==================== SELEÇÃO DE SÍMBOLOS ====================
                // Criar OBJETOS Jogador (não apenas strings)
                Jogador jogador1 = new Jogador("Jogador 1", SelecaoSimbolo.Selecionar("Jogador 1"));
                Console.Clear();
                Jogador jogador2 = new Jogador("Jogador 2", SelecaoSimbolo.Selecionar("Jogador 2"));

                // Easter egg: se escolherem o mesmo, Player 2 vira ☃️
                if (jogador1.Simbolo == jogador2.Simbolo)
                {
                    jogador2 = new Jogador(jogador2.Nome, "☃️");
                }

                Console.Clear();

                //==================== INICIALIZAÇÃO DO TABULEIRO ====================
                // Criar OBJETOS Tabuleiro e Bomba para esta partida
                Tabuleiro tabuleiro = new Tabuleiro(3);
                Bomba bomba = new Bomba(3);

                //==================== LOOP PRINCIPAL DO JOGO ====================
                do
                {
                    // Turno do Jogador 1
                    bool explodiu = ExibirMatriz.ExibirJogada(tabuleiro, bomba, jogador1.Simbolo);
                    if (explodiu)
                    {
                        ExibirMatriz.ExibirTabuleiro(tabuleiro);
                        Console.WriteLine($"\n💥 BOOM! {jogador2.Simbolo} {jogador2.Nome} VENCEU! 💥");
                        CondicaoDeVitoria.EnterParaContinuar();
                        break;
                    }
                    // Verifica empate após jogada do jogador 1
                    if (CondicaoDeVitoria.VerificarEmpate(tabuleiro, bomba))
                    {
                        CondicaoDeVitoria.ExibirEmpate(tabuleiro, bomba);
                        CondicaoDeVitoria.EnterParaContinuar();
                        break;
                    }

                    // Turno do Jogador 2
                    explodiu = ExibirMatriz.ExibirJogada(tabuleiro, bomba, jogador2.Simbolo);
                    if (explodiu)
                    {
                        ExibirMatriz.ExibirTabuleiro(tabuleiro);
                        Console.WriteLine($"\n💥 BOOM! {jogador1.Simbolo} {jogador1.Nome} VENCEU! 💥");
                        CondicaoDeVitoria.EnterParaContinuar();
                        break;
                    }
                    // Verifica empate após jogada do jogador 2
                    if (CondicaoDeVitoria.VerificarEmpate(tabuleiro, bomba))
                    {
                        CondicaoDeVitoria.ExibirEmpate(tabuleiro, bomba);
                        CondicaoDeVitoria.EnterParaContinuar();
                        break;
                    }

                } while (true);

                 // Pergunta se quer jogar novamente
                jogarNovamente = CondicaoDeVitoria.JogarNovamente();
            } while (jogarNovamente);
        }
    }
}
