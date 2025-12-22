# Campo-Minado-Csharp

**💣 Navegue pelo tabuleiro com WASD e evite a bomba escondida.**

---

## 📖 Sobre o Projeto

Campo Minado Multiplayer desenvolvido em C# para console. Dois jogadores alternam turnos navegando pelo tabuleiro com as teclas WASD, tentando preencher posições, mas há uma bomba escondida aleatoriamente - quem pisar nela, perde!

O projeto evoluiu da versão 1.0 (digitação numérica) para a versão 2.0 (navegação por cursor), aplicando conceitos avançados de captura de teclas e redesenho dinâmico de interface.

---

## 🎯 Funcionalidades

✅ **Sistema de navegação visual** com cursor móvel (◼️)  
✅ **Controles WASD** para movimentação fluida  
✅ **Símbolos personalizados** com emojis (💀 👽 💩 🤖 👹 👻 👾)  
✅ **Bomba aleatória** gerada a cada partida 💣  
✅ **Sistema de empate** com revelação de bombas  
✅ **Validação robusta** de teclas (apenas WASD + Enter)  
✅ **Verificação dinâmica** de posição ocupada  
✅ **Easter egg divertido** para símbolos repetidos 💅  
✅ **Interface visual clara** com instruções permanentes  
✅ **Sistema de replay** para múltiplas partidas  
✅ **Debug opcional** para testar posições das bombas  

---

## 🎮 Como Jogar

1. Escolha seu símbolo (Jogador 1 e Jogador 2)
2. Use **W, A, S, D** para mover o cursor ◼️
3. Pressione **ENTER** para colocar seu símbolo
4. **Cuidado!** Uma bomba está escondida no tabuleiro
5. Alterne turnos até alguém explodir ou empatar
6. Jogue novamente ou saia

```
=====Campo Minado=====

Use A, W, S, D para mover | ENTER para confirmar

  ◼️    ◻️    ◻️

  ◻️    ◻️    ◻️

  ◻️    ◻️    ◻️


 Coloque o 💀 em uma posição
```

### 🕹️ Controles

- **W** → Mover para cima ⬆️
- **A** → Mover para esquerda ⬅️
- **S** → Mover para baixo ⬇️
- **D** → Mover para direita ➡️
- **ENTER** → Confirmar jogada ✅

### 🏆 Condições de Vitória/Derrota

- **💥 Explosão:** Quem pisar na bomba **perde** (adversário vence)
- **🤝 Empate:** Se o tabuleiro encher sem ninguém explodir, empate! As bombas são reveladas

---

## 🔥 Destaques Técnicos

### 1️⃣ Sistema de Captura de Teclas Especiais

```csharp
ConsoleKeyInfo input;
do
{
    input = Console.ReadKey(true); // true = não exibe a tecla
    
    if (input.Key == ConsoleKey.W || 
        input.Key == ConsoleKey.A || 
        input.Key == ConsoleKey.S || 
        input.Key == ConsoleKey.D || 
        input.Key == ConsoleKey.Enter)
    {
        entradaValida = true;
    }
} while (!entradaValida);
```

**Diferencial:** `Console.ReadKey(true)` captura teclas sem necessidade de Enter, permitindo navegação fluida.

---

### 2️⃣ Cursor Visual Dinâmico

```csharp
public static void ExibirTabuleiro(string[,] matriz, int linhaAtual, int colunaAtual)
{
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            // Posição do cursor = ◼️ (destacado)
            if (i == linhaAtual && j == colunaAtual)
            {
                Console.Write("  ◼️  ");
            }
            // Outras posições = conteúdo normal
            else
            {
                Console.Write($"  {matriz[i, j]}  ");
            }
        }
    }
}
```

**Lógica elegante:** Cursor sobrepõe visualmente a posição atual sem modificar a matriz de dados.

---

### 3️⃣ Navegação com Limites de Borda

```csharp
if (input.Key == ConsoleKey.D && colunaAtual < 2)
{
    colunaAtual++; // Move direita (se não estiver na borda)
}
else if (input.Key == ConsoleKey.A && colunaAtual > 0)
{
    colunaAtual--; // Move esquerda (se não estiver na borda)
}
else if (input.Key == ConsoleKey.S && linhaAtual < 2)
{
    linhaAtual++; // Move baixo (se não estiver na borda)
}
else if (input.Key == ConsoleKey.W && linhaAtual > 0)
{
    linhaAtual--; // Move cima (se não estiver na borda)
}
```

**Validação inteligente:** Impede que o cursor saia do tabuleiro 3x3.

---

### 4️⃣ Sistema de Bombas Ocultas (v1.0)

```csharp
static bool[,] bombas;

public static void GerarBombaAleatoria()
{
    // Limpa array anterior (evita acúmulo entre partidas)
    bombas = new bool[3, 3];
    
    Random random = new Random();
    int posicaoBomba = random.Next(1, 10);
    
    // Converte posição (1-9) para índices da matriz (0-2)
    int linha = (posicaoBomba - 1) / 3;
    int coluna = (posicaoBomba - 1) % 3;
    
    bombas[linha, coluna] = true;
}
```

**Desafio resolvido:** Array `static` acumulava bombas entre partidas → Solução: recriar array dentro do método.

---

### 5️⃣ Detecção Inteligente de Empate

```csharp
public static bool VerificarEmpate(string[,] matriz)
{
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            // Se tem ◻️ vazio E NÃO É bomba = ainda tem jogadas possíveis
            if (matriz[i, j] == "◻️")
            {
                int posicao = i * 3 + j + 1;
                if (!Bomba.TemBomba(posicao))
                {
                    return false; // Ainda dá pra jogar
                }
            }
        }
    }
    return true; // Só sobrou a bomba = empate!
}
```

**Lógica elegante:** Ignora bombas escondidas ao verificar espaços livres.

---

### 6️⃣ Validação de Posição Ocupada

```csharp
else if (input.Key == ConsoleKey.Enter)
{
    int posicao = linhaAtual * 3 + colunaAtual + 1;
    
    // Verifica bomba primeiro
    if (Bomba.TemBomba(posicao))
    {
        matriz[linhaAtual, colunaAtual] = "💣";
        ExibirTabuleiro(matriz, -1, -1);
        return true; // Explosão
    }
    // Verifica se está ocupada (não é ◻️)
    else if (matriz[linhaAtual, colunaAtual] != "◻️")
    {
        Console.WriteLine("\nPosição já ocupada, escolha outra!\n");
        // Continua no loop, não retorna
    }
    else
    {
        // Posição livre, coloca símbolo
        matriz[linhaAtual, colunaAtual] = simbolo;
        return false;
    }
}
```

**Hierarquia de verificação:**
1. Bomba? → Explode
2. Ocupada? → Avisa e continua
3. Livre? → Coloca símbolo

---

### 7️⃣ Revelação no Empate

```csharp
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
```

No empate, todas as bombas são reveladas - jogadores veem o quão perto estiveram da explosão!

---

### 8️⃣ Fórmula Matemática para Conversão (Herança v1.0)

```csharp
// Converte posição da matriz (0-2) para número do tabuleiro (1-9)
int valor = i * 3 + j + 1;

// Explicação:
// i * 3 = elementos "pulados" nas linhas anteriores
// + j = posição dentro da linha atual
// + 1 = ajuste (índices começam em 0, queremos começar de 1)
```

**Exemplo:** [1,1] → 1 * 3 + 1 + 1 = 5 ✅

---

## 🏗️ Arquitetura

```
Pratica/
├── ExibirMatriz        → Navegação, captura de teclas e exibição
├── CondicaoDeVitoria   → Verificação de empate e exibição de resultado
├── Jogador             → Seleção de símbolos personalizados
├── Bomba               → Geração, verificação e revelação de bombas
└── Program             → Orquestração do fluxo do jogo
```

### 📐 Separação de Responsabilidades

- **ExibirMatriz:** Interface interativa com navegação por cursor
- **CondicaoDeVitoria:** Lógica de término e exibição de resultados
- **Jogador:** Sistema de personalização de símbolos
- **Bomba:** Toda a lógica de armadilhas (encapsulada)
- **Program:** Fluxo principal e loop de partidas

---

## 💡 Conceitos C# Aplicados

✅ **Matrizes Bidimensionais** (`string[,]` para tabuleiro, `bool[,]` para bombas)  
✅ **Arrays Estáticos** (`static bool[,]`) compartilhados entre métodos  
✅ **ConsoleKeyInfo** para captura de teclas especiais  
✅ **Enumerações ConsoleKey** (UpArrow, DownArrow, Enter, etc)  
✅ **Console.ReadKey(true)** para leitura sem echo  
✅ **Console.Clear()** para redesenho dinâmico de tela  
✅ **Random** para geração aleatória de bombas  
✅ **TryParse** para validação sem exceções (seleção de símbolos)  
✅ **String Interpolation** (`$"{variavel}"`)  
✅ **Métodos Estáticos** e organização modular  
✅ **Loops Aninhados** para percorrer matrizes 2D  
✅ **Flags Booleanas** para controle de fluxo (explosão, empate)  
✅ **Parâmetros para controle visual** (linhaAtual, colunaAtual)  

---

## 🎓 Aprendizados

### Técnicos
✅ Captura de teclas sem necessidade de Enter (`Console.ReadKey`)  
✅ Redesenho eficiente de interface (`Console.Clear` + loop)  
✅ Gerenciamento de cursor visual sem modificar dados  
✅ Validação de limites de matriz (bordas)  
✅ Separação de matriz visual vs matriz lógica  
✅ Gerenciamento de estado entre múltiplas partidas  

### Lógica de Jogos
✅ Sistema de navegação 2D com WASD  
✅ Feedback visual instantâneo (cursor em tempo real)  
✅ Mecânica de risco/recompensa (cada jogada pode ser fatal)  
✅ Balanceamento: 1 bomba em 9 posições = ~11% de risco por turno  
✅ Experiência de usuário fluida e intuitiva  

### Boas Práticas
✅ **DRY:** Método `ExibirTabuleiro()` reutilizado em todos os momentos  
✅ **SRP:** Cada classe com responsabilidade única e bem definida  
✅ **Encapsulamento:** Array de bombas privado, acesso via métodos públicos  
✅ **Defensive Programming:** Validações em teclas e limites de borda  
✅ **UX:** Instruções permanentes na tela, feedback claro  
✅ **Escalabilidade:** Fácil adaptar para tabuleiros maiores  

---

## 🔍 Desafios Superados

### 1. **Migração de Sistema Numérico para Visual** 🎯
**Desafio v1 → v2:** Transformar input numérico (1-9) em navegação por cursor.  
**Solução:** Sistema de coordenadas (linhaAtual, colunaAtual) + redesenho dinâmico.

```csharp
// v1.0: Input direto
int posicao = int.Parse(Console.ReadLine());

// v2.0: Navegação + conversão
int posicao = linhaAtual * 3 + colunaAtual + 1;
```

---

### 2. **Captura de Teclas Especiais** ⌨️
**Problema:** `Console.ReadLine()` exige Enter, quebrando fluidez da navegação.  
**Solução:** `Console.ReadKey(true)` captura teclas individuais sem echo.

```csharp
ConsoleKeyInfo input = Console.ReadKey(true);
if (input.Key == ConsoleKey.W) { /* move */ }
```

---

### 3. **Cursor Visual sem Modificar Dados** 🔳
**Problema:** Como mostrar cursor sem sobrescrever o conteúdo da matriz?  
**Solução:** Parâmetros separados (linhaAtual, colunaAtual) passados para `ExibirTabuleiro()`.

```csharp
// Cursor é visual, não altera matriz[i, j]
if (i == linhaAtual && j == colunaAtual)
    Console.Write("  ◼️  ");
else
    Console.Write($"  {matriz[i, j]}  ");
```

---

### 4. **Acúmulo de Bombas Entre Partidas** 🐛 (v1.0)
**Problema:** Array `static bool[,] bombas = new bool[3, 3]` inicializava apenas uma vez.  
**Sintoma:** Partidas subsequentes acumulavam bombas anteriores.  
**Solução:** Mover `new bool[3, 3]` para dentro de `GerarBombaAleatoria()`.

---

### 5. **Empate Ignorando Bomba Escondida** 🎯
**Problema:** Verificação contava posição da bomba como "livre".  
**Solução:** Verificar se `◻️` **E** não é bomba simultaneamente.

```csharp
if (matriz[i, j] == "◻️")
{
    int posicao = i * 3 + j + 1;
    if (!Bomba.TemBomba(posicao)) // Ignora bombas!
        return false;
}
```

---

### 6. **Validação de Bordas** 🚧
**Problema:** Cursor pode sair do tabuleiro 3x3.  
**Solução:** Validação antes de incrementar/decrementar coordenadas.

```csharp
if (input.Key == ConsoleKey.D && colunaAtual < 2) // Não sai pela direita
    colunaAtual++;
```

---

## 💻 Tecnologias

- **Linguagem:** C# 12 (.NET 8)
- **Paradigma:** Programação Orientada a Objetos
- **IDE:** Visual Studio 2022 / VS Code
- **Conceitos:** Matrizes, Randomização, Captura de Teclas, Interface Dinâmica

---

## 📊 Changelog

### **v2.0** (Atual) - Sistema de Navegação Visual
- ✅ Navegação com WASD ao invés de digitação numérica
- ✅ Cursor visual com ◼️ e ◻️
- ✅ Captura de teclas com `Console.ReadKey()`
- ✅ Interface mais intuitiva e fluida
- ✅ Instruções permanentes na tela
- ✅ Validação de limites de borda

### **v1.0** - Versão Base Numérica
- ✅ Sistema de digitação 1-9
- ✅ Bomba aleatória única
- ✅ Sistema de empate
- ✅ Símbolos personalizados
- ✅ Validação com TryParse

---

## 🚀 Evoluções Futuras (Roadmap v3)

### Em Planejamento:
- [ ] **Tabuleiros maiores** (4x4, 5x5, customizável)
- [ ] **Múltiplas bombas** (dificuldade escalável)
- [ ] **Setas do teclado** como alternativa ao WASD
- [ ] **Placar persistente** entre partidas
- [ ] **Modos de dificuldade** (Fácil, Médio, Difícil, Insano)
- [ ] **Animações** de explosão no console
- [ ] **Sons** (se migrar para interface gráfica)
- [ ] **Campo Minado tradicional** com números adjacentes

---

## 🎮 Gameplay Único

### 🔥 Por que este jogo é diferente?

**Campo Minado tradicional:** Puzzle solo de dedução lógica  
**Este jogo:** Competição multiplayer com risco compartilhado

- **Tensão constante:** Qualquer jogada pode ser a última
- **Controle direto:** Navegação física pelo tabuleiro
- **Sorte + Estratégia:** Evitar posições arriscadas sem saber onde está a bomba
- **Partidas rápidas:** 2-5 minutos de pura adrenalina
- **Rejogabilidade alta:** Cada partida é completamente diferente
- **Interface intuitiva:** Visual limpo e controles responsivos

### 💭 Filosofia do Design

> "E se o Campo Minado fosse uma competição ao invés de um quebra-cabeça?  
> E se dois amigos tivessem que navegar pelo perigo juntos, mas apenas um perdesse?  
> E se a experiência fosse tão fluida quanto um jogo moderno, mas rodasse no console?"

Este projeto nasceu da fusão de três elementos: **Jogo da Velha** (turnos) + **Campo Minado** (perigo oculto) + **Navegação moderna** (WASD fluido).

---

## 👨‍💻 Sobre o Desenvolvedor

**Gabriel Henrique Cé**  
LinkedIn: [Gabriel Henrique Cé](https://linkedin.com/in/gabrielhenriquece)  
GitHub: [@GabrielHenriqueCe](https://github.com/GabrielHenriqueCe)

---

## 📝 Licença

MIT License - Código aberto para fins educacionais

---

## 🎮 Da Mecânica Simples à Experiência Polida

_"Começou como um Jogo da Velha com uma bomba.  
Evoluiu para um sistema de navegação visual fluido.  
Mas o coração continua o mesmo: dois amigos, um tabuleiro, e a tensão de não saber onde vai explodir."_

---

**Status:** 🟢 **v2.0 Completo** - Jogo Funcional com Navegação Visual  
**Última Atualização:** Dezembro 2025  
**Linhas de Código:** 321 
**Baseado em:** [Jogo-da-Velha-Console-Csharp](https://github.com/GabrielHenriqueCe/Jogo-da-Velha)

---

## 🎲 Estatísticas de Jogo

- **Probabilidade de explodir no 1º turno:** ~11% (1/9)
- **Probabilidade de empate:** ~22% (2/9 posições sobram)
- **Partidas médias até explosão:** 4-5 turnos
- **Tempo médio por partida:** 2-3 minutos
- **Teclas pressionadas por partida:** ~15-25 (navegação + confirmação)

---

### 💣 Navegue com cuidado... e boa sorte! 🍀
