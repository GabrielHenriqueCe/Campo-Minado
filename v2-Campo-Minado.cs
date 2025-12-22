using System;

namespace Pratica
{
    /// <summary>
    /// Responsável por exibir e gerenciar as jogadas no tabuleiro
    /// </summary>
    class ExibirMatriz
    {
        public static bool ExibirJogada(string[,] matriz, string simbolo)
        {
            int linhaAtual = 0;
            int colunaAtual = 0;

            ExibirTabuleiro(matriz, linhaAtual, colunaAtual);

            do
            {
                Console.WriteLine();
                Console.WriteLine($"\nColoque o {simbolo} em uma posição\n");
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
                    ExibirTabuleiro(matriz, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.A && colunaAtual > 0)
                {
                    colunaAtual--;
                    ExibirTabuleiro(matriz, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.S && linhaAtual < 2)
                {
                    linhaAtual++;
                    ExibirTabuleiro(matriz, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.W && linhaAtual > 0)
                {
                    linhaAtual--;
                    ExibirTabuleiro(matriz, linhaAtual, colunaAtual);
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    int posicao = linhaAtual * 3 + colunaAtual + 1;
                    
                    if (Bomba.TemBomba(posicao))
                    {
                        // BOOM! 💣
                        matriz[linhaAtual, colunaAtual] = "💣";
                        ExibirTabuleiro(matriz, -1, -1);
                        Console.WriteLine();
                        return true;
                    }
                    else if (matriz[linhaAtual, colunaAtual] != "◻️")
                    {
                        ExibirTabuleiro(matriz, linhaAtual, colunaAtual);
                        Console.WriteLine("\nPosição já ocupada, escolha outra!\n");
                    }
                    else
                    {
                        // Posição livre, coloca símbolo
                        matriz[linhaAtual, colunaAtual] = simbolo;
                        return false;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Posição inválida, digite novamente\n");
                    ExibirTabuleiro(matriz, linhaAtual, colunaAtual);
                }

            } while (true);
        }

        public static void ExibirTabuleiro(string[,] matriz, int linhaAtual, int colunaAtual)
        {
            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            Console.WriteLine("Use A, W, S, D para mover | ENTER para confirmar\n");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == linhaAtual && j == colunaAtual)
                    {
                        Console.Write("  ◼️  ");
                    }
                    else
                    {
                        Console.Write($"  {matriz[i, j]}  ");
                    }
                }
                Console.WriteLine("\n");
            }

        }
    }

    class CondicaoDeVitoria
    {
        public static bool VerificarEmpate(string[,] matriz)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (matriz[i, j] == "◻️")
                    {
                        int posicao = i * 3 + j + 1;
                        if (!Bomba.TemBomba(posicao))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static void ExibirEmpate(string[,] campoMinado)
        {
            // Revela todas as bombas no tabuleiro
            Bomba.RevelarBombas(campoMinado);

            // Mostra tabuleiro final com bombas reveladas
            Console.Clear();
            Console.WriteLine("=====Campo Minado=====\n");
            ExibirMatriz.ExibirTabuleiro(campoMinado, -1, -1);
            Console.WriteLine();
            Console.WriteLine("\n=======EMPATE!=======");
            Console.WriteLine("💣 Os dois Morreram 💣");
        }
    }

    class Jogador
    {
        // Array de emojis disponíveis para escolha
        private static string[] simbolos = { "💀", "👽", "💩", "🤖", "👹", "👻", "👾" };

        // Exibe lista numerada de símbolos disponíveis
        static void ExibirSimbolos()
        {
            for (int i = 0; i < simbolos.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {simbolos[i]}");
            }
        }

        public static string EscolherSimbolo(string player)
        {
            int min = 1;
            int max = simbolos.Length;
            int selecionar;
            bool entradaValida;
            do
            {
                // ... código de validação ...
                Console.Clear();
                Console.WriteLine($"\nEscolha o símbolo do {player}");
                ExibirSimbolos();

                string input = Console.ReadLine();

                entradaValida = int.TryParse(input, out selecionar);

                if (!entradaValida)
                {
                    Console.WriteLine("Digite apenas números! Aperte qualquer tecla para continuar");
                    Console.ReadKey();
                }
                else if (selecionar < min || selecionar > max)
                {
                    Console.WriteLine($"Opção inválida, digite entre {min} e {max} ! Aperte qualquer tecla para continuar");
                    Console.ReadKey();
                }

            } while (!entradaValida || selecionar < min || selecionar > max);

            // Retorna o emoji selecionado (ajusta índice de 1-based para 0-based)
            return simbolos[selecionar - 1];
        }
    }

    class Bomba
    {
        static bool[,] bombas;
        public static void GerarBombaAleatoria()
        {
            //Matriz para guardar as bombas (oculta dos jogadores)
            bombas = new bool[3, 3];

            Random random = new Random();
            int posicaoBomba = random.Next(1, 10); // Gera número de 1 a 9

            // Converte a posição (1-9) para índices da matriz (0-2, 0-2)
            int linha = (posicaoBomba - 1) / 3;
            int coluna = (posicaoBomba - 1) % 3;

            bombas[linha, coluna] = true;
            //DEBUG SE QUISER TESTAR == Console.WriteLine("DEBUG: Bomba gerada na posição " + posicaoBomba);
        }

        public static bool TemBomba(int posicao)
        {
            int linha = (posicao - 1) / 3;
            int coluna = (posicao - 1) % 3;

            return bombas[linha, coluna];
        }

        public static void RevelarBombas(string[,] matriz)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int posicao = i * 3 + j + 1;
                    if (Bomba.TemBomba(posicao))
                    {
                        matriz[i, j] = "💣";
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool jogarNovamente = true;
            do
            {
                //==================== SELEÇÃO DE SÍMBOLOS ====================
                string player1 = Jogador.EscolherSimbolo("Jogador 1");
                Console.Clear();
                string player2 = Jogador.EscolherSimbolo("Jogador 2");
                if (player1 == player2)
                {
                    // Easter egg: se escolherem o mesmo símbolo, Player 2 é "punido" 💅
                    player2 = "💅";
                }
                Console.Clear();

                //==================== INICIALIZAÇÃO DO TABULEIRO ====================
                Console.Clear();
                Console.WriteLine("=====Campo Minado=====\n");
                string[,] campoMinado = new string[3, 3];
                // Preenche matriz com ◻️
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        campoMinado[i, j] = "◻️";
                    }
                }
                // Gera bomba aleatória e exibe tabuleiro
                Bomba.GerarBombaAleatoria();
                ExibirMatriz.ExibirTabuleiro(campoMinado, 0, 0);

                //==================== LOOP PRINCIPAL DO JOGO ====================
                do
                {
                    // Turno do Jogador 1
                    bool explodiu = ExibirMatriz.ExibirJogada(campoMinado, player1);
                    if (explodiu)
                    {
                        Console.WriteLine($"\n💥 BOOM! {player2} VENCEU! 💥");
                        break;
                    }
                    if (CondicaoDeVitoria.VerificarEmpate(campoMinado))
                    {
                        CondicaoDeVitoria.ExibirEmpate(campoMinado);
                        break;
                    }

                    // Turno do Jogador 2
                    explodiu = ExibirMatriz.ExibirJogada(campoMinado, player2);
                    if (explodiu)
                    {
                        Console.WriteLine($"\n💥 BOOM! {player1} VENCEU! 💥");
                        break;
                    }
                    if (CondicaoDeVitoria.VerificarEmpate(campoMinado))
                    {
                        CondicaoDeVitoria.ExibirEmpate(campoMinado);
                        break;
                    }

                } while (true);

                Console.WriteLine("\nDeseja jogar novamente? (S/N)");
                string resposta = Console.ReadLine().ToUpper();
                if (resposta != "S")
                {
                    jogarNovamente = false;
                }
            } while (jogarNovamente);
        }
    }
}
