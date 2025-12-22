# Campo-Minado-Csharp

**💣 Turnos alternados onde uma bomba escondida no tabuleiro pode decidir tudo.**

---

## 📖 Sobre o Projeto

Campo Minado Multiplayer desenvolvido em C# para console, combinando a mecânica do meu Jogo da Velha com a tensão de um campo minado. Dois jogadores alternam turnos tentando preencher o tabuleiro, mas há uma bomba escondida aleatoriamente - quem pisar nela, perde!

O projeto aplica conceitos fundamentais de programação com foco em lógica de jogos, validação robusta e experiência multiplayer competitiva.

---

## 🎯 Funcionalidades

✅ **Símbolos personalizados** com emojis (💀 👽 💩 🤖 👹 👻 👾)  
✅ **Bomba aleatória** gerada a cada partida 💣  
✅ **Sistema de empate** com revelação de bombas  
✅ **Validação robusta** com TryParse (evita crash ao digitar letras)  
✅ **Verificação dinâmica** de posição ocupada  
✅ **Easter egg divertido** para símbolos repetidos 💅  
✅ **Interface visual clara** com separadores  
✅ **Sistema de replay** para múltiplas partidas  
✅ **Debug opcional** para testar posições das bombas  

---

## 🎮 Como Jogar

1. Escolha seu símbolo (Jogador 1 e Jogador 2)
2. Digite um número de 1-9 para colocar seu símbolo
3. **Cuidado!** Uma bomba está escondida no tabuleiro
4. Alterne turnos até alguém explodir ou empatar
5. Jogue novamente ou saia

```
=====Campo Minado=====

   1    |   2    |   3  
----------------------
   4    |   5    |   6  
----------------------
   7    |   8    |   9  

 Coloque o 💀 em uma posição
```

### 🏆 Condições de Vitória/Derrota

- **💥 Explosão:** Quem pisar na bomba **perde** (adversário vence)
- **🤝 Empate:** Se o tabuleiro encher sem ninguém explodir, empate! As bombas são reveladas

---

## 🔥 Destaques Técnicos

### 1️⃣ Sistema de Bombas Ocultas

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

### 2️⃣ Detecção Inteligente de Empate

```csharp
public static bool VerificarEmpate(string[,] matriz)
{
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            // Se tem número E NÃO É bomba = ainda tem jogadas possíveis
            if (int.TryParse(matriz[i, j], out int posicao) && !Bomba.TemBomba(posicao))
            {
                return false;
            }
        }
    }
    return true; // Só sobrou a bomba = empate!
}
```

**Lógica elegante:** Não conta casas ocupadas - verifica se ainda existem posições **jogáveis** (números que não são bombas).

---

### 3️⃣ Revelação Dramática no Empate

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

No empate, todas as bombas são reveladas no tabuleiro final - jogadores veem o quão perto estiveram da explosão!

---

### 4️⃣ Verificação de Explosão Integrada

```csharp
if (valor == posicao)
{
    // Verifica se posição tem bomba ANTES de colocar símbolo
    if (Bomba.TemBomba(posicao))
    {
        matriz[i, j] = "💣"; // BOOM!
        ExibirMatriz.ExibirTabuleiro(matriz);
        return true; // Sinaliza explosão
    }
    // Posição segura, coloca símbolo normalmente
    else if (int.TryParse(matriz[i, j], out _))
    {
        matriz[i, j] = simbolo;
    }
}
```

---

### 5️⃣ Fórmula Matemática para Conversão (Herança do Jogo da Velha)

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
├── ExibirMatriz        → Gerenciamento de jogadas e exibição do tabuleiro
├── CondicaoDeVitoria   → Verificação de empate e exibição de resultado
├── Jogador             → Seleção de símbolos personalizados
├── Bomba               → Geração, verificação e revelação de bombas
└── Program             → Orquestração do fluxo do jogo
```

### 📐 Separação de Responsabilidades

- **ExibirMatriz:** Interface e interação com o usuário
- **CondicaoDeVitoria:** Lógica de término do jogo
- **Jogador:** Sistema de personalização
- **Bomba:** Toda a lógica de armadilhas (encapsulada)
- **Program:** Fluxo principal e loop de jogo

---

## 💡 Conceitos C# Aplicados

✅ **Matrizes Bidimensionais** (`string[,]` para tabuleiro, `bool[,]` para bombas)  
✅ **Arrays Estáticos** (`static bool[,]`) compartilhados entre métodos  
✅ **TryParse** para validação sem exceções  
✅ **Random** para geração aleatória de bombas  
✅ **Operador Discard** (`_`) quando não precisamos do valor parseado  
✅ **String Interpolation** (`$"{variavel}"`)  
✅ **Métodos Estáticos** e organização modular  
✅ **Loops Aninhados** para percorrer matrizes 2D  
✅ **Flags Booleanas** para controle de fluxo (explosão, empate)  

---

## 🎓 Aprendizados

### Técnicos
✅ Gerenciamento de estado entre múltiplas partidas (limpar arrays `static`)  
✅ Lógica de detecção contextual (número + não-bomba = jogável)  
✅ Separação de matriz visual vs matriz lógica (tabuleiro vs bombas)  
✅ Integração de sistemas independentes (jogadas + armadilhas)  

### Lógica de Jogos
✅ Mecânica de risco/recompensa (cada jogada pode ser fatal)  
✅ Balanceamento: 1 bomba em 9 posições = ~11% de risco por turno  
✅ Feedback visual progressivo (revelação dramática no empate)  
✅ Sistema de turnos com condições de término claras  

### Boas Práticas
✅ **DRY:** Método `RevelarBombas()` reutilizável  
✅ **SRP:** Classe `Bomba` isolada com responsabilidade única  
✅ **Encapsulamento:** Array de bombas privado, acesso via métodos públicos  
✅ **Defensive Programming:** Validações em todos os inputs  
✅ **UX:** Mensagens claras de vitória/derrota/empate  

---

## 🔍 Desafios Superados

### 1. **Acúmulo de Bombas Entre Partidas** 🐛
**Problema:** Array `static bool[,] bombas = new bool[3, 3]` inicializava apenas uma vez.  
**Sintoma:** Partidas subsequentes acumulavam bombas anteriores.  
**Solução:** Mover `new bool[3, 3]` para dentro de `GerarBombaAleatoria()`.

```csharp
// ❌ Antes (inicializa 1 vez só)
static bool[,] bombas = new bool[3, 3];

// ✅ Depois (recria a cada jogo)
static bool[,] bombas;
public static void GerarBombaAleatoria()
{
    bombas = new bool[3, 3]; // Limpa bombas anteriores
    // ...
}
```

---

### 2. **Empate Ignorando Bomba Escondida** 🎯
**Problema:** Verificação de empate contava posição da bomba como "livre".  
**Solução:** Verificar se número **E** não é bomba simultaneamente.

```csharp
// Lógica: posição jogável = É número E NÃO é bomba
if (int.TryParse(matriz[i, j], out int posicao) && !Bomba.TemBomba(posicao))
```

---

### 3. **Revelação Visual das Bombas** 💣
**Desafio:** Mostrar onde estavam as bombas no empate sem estragar a jogabilidade.  
**Solução:** Método dedicado que só é chamado no final do jogo.

```csharp
if (CondicaoDeVitoria.VerificarEmpate(campoMinado))
{
    CondicaoDeVitoria.ExibirEmpate(campoMinado); // Revela tudo
    break;
}
```

---

### 4. **Código Duplicado nas Verificações** 🔄
**Problema:** Verificação de empate após jogador 1 e jogador 2 (código idêntico).  
**Solução:** Extrair método `ExibirEmpate()` para eliminar duplicação.

---

## 💻 Tecnologias

- **Linguagem:** C# 12 (.NET 8)
- **Paradigma:** Programação Orientada a Objetos
- **IDE:** Visual Studio 2022 / VS Code
- **Conceitos:** Matrizes, Randomização, Validação, Lógica de Jogos

---

## 🚀 Evoluções Futuras (Roadmap v2)

### Em Planejamento:
- [ ] **Tabuleiros maiores** (4x4, 5x5, customizável)
- [ ] **Múltiplas bombas** (dificuldade escalável)
- [ ] **Sistema de navegação** com setas do teclado (◻️🔳◻️)
- [ ] **Placar persistente** entre partidas
- [ ] **Modos de dificuldade** (Fácil, Médio, Difícil, Insano)
- [ ] **Animações** de explosão no console

---

## 🎮 Gameplay Único

### 🔥 Por que este jogo é diferente?

**Campo Minado tradicional:** Puzzle solo de dedução lógica  
**Este jogo:** Competição multiplayer com risco compartilhado

- **Tensão constante:** Qualquer jogada pode ser a última
- **Sorte + Estratégia:** Evitar posições arriscadas sem saber onde está a bomba
- **Partidas rápidas:** 2-5 minutos de pura adrenalina
- **Rejogabilidade alta:** Cada partida é completamente diferente

### 💭 Filosofia do Design

> "E se o Campo Minado fosse uma competição ao invés de um quebra-cabeça?  
> E se dois amigos tivessem que arriscar juntos, mas apenas um perdesse?"

Este projeto nasceu da fusão de dois clássicos: **Jogo da Velha** (turnos) + **Campo Minado** (perigo oculto).

---

## 👨‍💻 Sobre o Desenvolvedor

**Gabriel Henrique Cé**  
LinkedIn: [Gabriel Henrique Cé](https://linkedin.com/in/gabrielhenriquece)  
GitHub: [@GabrielHenriqueCe](https://github.com/GabrielHenriqueCe)

---

## 📝 Licença

MIT License - Código aberto para fins educacionais

---

## 🎮 Da Mecânica Simples à Experiência Única

_"Pegar um Jogo da Velha comum e adicionar uma bomba aleatória  
poderia ser só uma piada. Mas aplicar validações robustas,  
sistema de empate inteligente e código modular transforma  
uma ideia boba em um jogo genuinamente divertido."_

---

**Status:** 🟢 **v1.0 Completo** - Jogo Funcional e Testado  
**Última Atualização:** Dezembro 2025  
**Linhas de Código:** 301
**Baseado em:** [Jogo-da-Velha-Console-Csharp](https://github.com/GabrielHenriqueCe/Jogo-da-Velha)

---

## 🎲 Estatísticas de Jogo

- **Probabilidade de explodir no 1º turno:** ~11% (1/9)
- **Probabilidade de empate:** ~22% (2/9 posições sobram)
- **Partidas médias até explosão:** 4-5 turnos
- **Tempo médio por partida:** 2-3 minutos

---

### 💣 Divirta-se... e boa sorte! 🍀
