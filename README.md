# Campo-Minado-Csharp V3 (POO)

**💣 Navegue pelo tabuleiro com WASD e evite a bomba escondida - Agora com Programação Orientada a Objetos!**

---

## 📖 Sobre o Projeto

Campo Minado Multiplayer desenvolvido em C# para console com arquitetura orientada a objetos. Dois jogadores alternam turnos navegando pelo tabuleiro com as teclas WASD, tentando preencher posições, mas há uma bomba escondida aleatoriamente - quem pisar nela, perde!

O projeto evoluiu através de três versões:
- **V1:** Sistema procedural com input numérico (1-9)
- **V2:** Navegação por cursor com WASD (procedural)
- **V3:** Refatoração completa para POO com encapsulamento e separação de responsabilidades

---

## 🎯 Funcionalidades

✅ **Sistema de navegação visual** com cursor móvel (◼️)  
✅ **Controles WASD** para movimentação fluida  
✅ **Símbolos personalizados** com emojis (💀 👽 💩 🤖 👹 👻 👾)  
✅ **Bomba aleatória** gerada a cada partida 💣  
✅ **Sistema de empate** com revelação de bombas  
✅ **Validação robusta** de teclas (apenas WASD + Enter)  
✅ **Verificação dinâmica** de posição ocupada  
✅ **Easter egg divertido** para símbolos repetidos ☃️  
✅ **Interface visual clara** com instruções permanentes  
✅ **Sistema de replay** para múltiplas partidas  
✅ **Arquitetura POO** com classes especializadas  
✅ **Encapsulamento** de dados e comportamentos  
✅ **Sobrecarga de métodos** para flexibilidade de exibição  

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

## 🏗️ Evolução Arquitetural: De Procedural para POO

### 📊 Comparação de Paradigmas

#### **V1/V2 (Procedural)**
```
Main()
├── string[,] matriz (passada por parâmetro em todo lugar)
├── bool[,] bombas (variável estática global)
├── Métodos estáticos soltos
└── Dados e comportamentos separados
```

#### **V3 (Orientado a Objetos)**
```
Main()
├── Tabuleiro (objeto com estado próprio)
│   ├── matriz[,] (privado/encapsulado)
│   ├── TamanhoMatriz (property)
│   └── métodos: PosicaoOcupada(), ColocarSimbolo()
│
├── Bomba (objeto com posição encapsulada)
│   ├── linha, coluna (privados)
│   ├── Properties de acesso
│   └── métodos: TemBomba(), RevelarBomba()
│
├── Jogador (identidade completa)
│   ├── Nome (property)
│   └── Simbolo (property)
│
└── Classes especializadas de UI e lógica
```

### ✨ Benefícios da Refatoração POO

#### **1. Encapsulamento de Dados**
```csharp
// ❌ Antes (V2): Matriz exposta, qualquer um pode modificar
string[,] matriz = new string[3,3];
matriz[0,0] = "qualquer coisa"; // Sem controle!

// ✅ Agora (V3): Acesso controlado através do objeto
Tabuleiro tabuleiro = new Tabuleiro(3);
tabuleiro.ColocarSimbolo(0, 0, "💀"); // Método controlado
```

#### **2. Responsabilidade Única**
```csharp
// ❌ Antes: Bomba era apenas um array global
static bool[,] bombas;

// ✅ Agora: Bomba é um objeto com comportamento próprio
Bomba bomba = new Bomba(3);
if (bomba.TemBomba(linha, coluna))
    bomba.RevelarBomba(tabuleiro);
```

#### **3. Escalabilidade**
```csharp
// ❌ Antes: Para múltiplas bombas, precisaria refatorar tudo
// ✅ Agora: Basta criar uma lista de objetos
List<Bomba> bombas = new List<Bomba>();
bombas.Add(new Bomba(3));
bombas.Add(new Bomba(3));
// Cada bomba gerencia sua própria posição!
```

#### **4. Código Mais Limpo**
```csharp
// ❌ Antes: Parâmetros sendo passados repetidamente
ExibirJogada(matriz, simbolo);
VerificarEmpate(matriz);
ExibirEmpate(matriz);

// ✅ Agora: Objetos carregam seu próprio estado
ExibirJogada(tabuleiro, bomba, simbolo);
VerificarEmpate(tabuleiro, bomba);
ExibirEmpate(tabuleiro, bomba);
// tabuleiro já "sabe" sua matriz internamente
```

---

## 📦 Arquitetura de Classes (V3)

### **Classe `Tabuleiro`**
Encapsula toda a lógica relacionada ao estado do tabuleiro.

```csharp
class Tabuleiro
{
    private string[,] matriz;              // Dados protegidos
    public int TamanhoMatriz { get; }      // Property somente leitura
    
    public Tabuleiro(int tamanho)          // Constructor
    public bool PosicaoOcupada(int linha, int coluna)
    public void ColocarSimbolo(int linha, int coluna, string simbolo)
}
```

**Responsabilidades:**
- ✅ Gerenciar estado da matriz
- ✅ Validar posições ocupadas
- ✅ Modificar conteúdo de forma controlada
- ✅ Proteger dados internos (encapsulamento)

---

### **Classe `Bomba`**
Cada bomba é um objeto independente com sua própria posição.

```csharp
class Bomba
{
    private int linha;                     // Posição encapsulada
    private int coluna;
    
    public Bomba(int tamanhoTabuleiro)     // Constructor com Random
    public int Linha { get; }              // Property somente leitura
    public int Coluna { get; }
    
    public bool TemBomba(int linha, int coluna)
    public void RevelarBomba(Tabuleiro tabuleiro)
}
```

**Responsabilidades:**
- ✅ Gerar posição aleatória no constructor
- ✅ Verificar se está em coordenada específica
- ✅ Revelar-se no tabuleiro quando necessário
- ✅ Cada instância é uma bomba independente (preparado para múltiplas bombas!)

---

### **Classe `Jogador`**
Representa a identidade completa de um jogador.

```csharp
class Jogador
{
    public string Nome { get; private set; }     // Só pode ser setado internamente
    public string Simbolo { get; private set; }
    
    public Jogador(string nome, string simbolo)  // Constructor
}
```

**Responsabilidades:**
- ✅ Armazenar identidade do jogador
- ✅ Encapsular nome e símbolo juntos
- ✅ Facilitar passagem de dados relacionados

**Vantagem sobre strings soltas:**
```csharp
// ❌ Antes: Dados espalhados
string player1 = "💀";
string nomePlayer1 = "Jogador 1"; // Se existisse

// ✅ Agora: Tudo em um objeto
Jogador jogador1 = new Jogador("Jogador 1", "💀");
Console.WriteLine($"{jogador1.Simbolo} {jogador1.Nome} VENCEU!");
```

---

### **Classe `ExibirMatriz`**
Responsável por toda interação visual e captura de input.

```csharp
class ExibirMatriz
{
    public static bool ExibirJogada(Tabuleiro tabuleiro, Bomba bomba, string simbolo)
    public static void ExibirTabuleiro(Tabuleiro tabuleiro, int linhaAtual, int colunaAtual)
    public static void ExibirTabuleiro(Tabuleiro tabuleiro)  // Sobrecarga!
}
```

**Responsabilidades:**
- ✅ Captura de teclas (WASD + Enter)
- ✅ Navegação com cursor
- ✅ Exibição dinâmica do tabuleiro
- ✅ Validação de movimentos e bordas

**Sobrecarga de Método:**
```csharp
// Durante navegação (com cursor)
ExibirTabuleiro(tabuleiro, linhaAtual, colunaAtual);

// Exibição final (sem cursor)
ExibirTabuleiro(tabuleiro);  // Internamente chama a versão com -1, -1
```

---

### **Classe `CondicaoDeVitoria`**
Gerencia condições de término e exibição de resultados.

```csharp
class CondicaoDeVitoria
{
    public static bool VerificarEmpate(Tabuleiro tabuleiro, Bomba bomba)
    public static void ExibirEmpate(Tabuleiro tabuleiro, Bomba bomba)
    public static bool JogarNovamente()
    public static void EnterParaContinuar()
}
```

**Responsabilidades:**
- ✅ Verificar condição de empate
- ✅ Exibir tela de empate com bombas reveladas
- ✅ Interface de "Jogar Novamente" (navegação Sim/Não)
- ✅ Pausas e controle de fluxo

---

### **Classe `SelecaoSimbolo`**
Interface especializada para escolha de símbolos.

```csharp
class SelecaoSimbolo
{
    private static readonly string[] simbolos = { "💀", "👽", "💩", ... };
    
    public static int TotalSimbolos()
    public static void ExibirSimbolo(string player, int linhaAtual)
    public static string Selecionar(string player)
}
```

**Responsabilidades:**
- ✅ Gerenciar lista de símbolos disponíveis
- ✅ Interface de navegação W/S para seleção
- ✅ Validação de input
- ✅ Retorno do emoji escolhido

---

## 🔥 Destaques Técnicos de POO

### 1️⃣ Encapsulamento com Properties

```csharp
class Tabuleiro
{
    private string[,] matriz;  // Campo privado - ninguém acessa diretamente
    
    public string[,] Matriz    // Property pública - acesso controlado
    {
        get { return matriz; }  // Permite leitura
        // Sem setter - ninguém pode substituir a matriz inteira
    }
}
```

**Vantagem:** Protege dados internos enquanto permite leitura controlada.

---

### 2️⃣ Constructors para Inicialização

```csharp
// Antes: Inicialização manual em vários lugares
string[,] matriz = new string[3, 3];
for (int i = 0; i < 3; i++)
    for (int j = 0; j < 3; j++)
        matriz[i, j] = "◻️";

// Agora: Inicialização automática no constructor
Tabuleiro tabuleiro = new Tabuleiro(3);
// Já vem com tudo inicializado internamente!
```

```csharp
public Tabuleiro(int tamanho)
{
    TamanhoMatriz = tamanho;
    matriz = new string[TamanhoMatriz, TamanhoMatriz];
    
    for (int i = 0; i < TamanhoMatriz; i++)
        for (int j = 0; j < TamanhoMatriz; j++)
            matriz[i, j] = "◻️";
}
```

---

### 3️⃣ Sobrecarga de Métodos (Method Overloading)

```csharp
// Versão completa (para navegação)
public static void ExibirTabuleiro(Tabuleiro tabuleiro, int linhaAtual, int colunaAtual)
{
    // ... lógica com cursor
}

// Versão simplificada (para exibição final)
public static void ExibirTabuleiro(Tabuleiro tabuleiro)
{
    ExibirTabuleiro(tabuleiro, -1, -1);  // Reutiliza a versão completa
}
```

**Vantagem:** 
- Mesma lógica de exibição em um lugar só (DRY)
- Interface mais limpa para o chamador
- Fácil manutenção (mudanças em um lugar afetam ambas)

---

### 4️⃣ Separação de Responsabilidades (SRP)

Cada classe tem **UMA** responsabilidade clara:

| Classe | Responsabilidade | O que NÃO faz |
|--------|-----------------|---------------|
| `Tabuleiro` | Gerenciar estado da matriz | ❌ Não desenha na tela |
| `Bomba` | Saber sua posição e revelar-se | ❌ Não desenha o tabuleiro |
| `ExibirMatriz` | Interface visual e input | ❌ Não valida empate |
| `CondicaoDeVitoria` | Verificar fim de jogo | ❌ Não gerencia símbolos |
| `Jogador` | Identidade do jogador | ❌ Não interage com tabuleiro |

---

### 5️⃣ Objetos com Estado Próprio

```csharp
// Cada objeto carrega seu próprio estado
Bomba bomba1 = new Bomba(3);  // Posição: [1, 2]
Bomba bomba2 = new Bomba(3);  // Posição: [0, 0]
Bomba bomba3 = new Bomba(3);  // Posição: [2, 1]

// Não precisam de array global compartilhado!
// Cada bomba "sabe" onde está
```

**Preparado para evolução:**
```csharp
List<Bomba> bombas = new List<Bomba>
{
    new Bomba(3),
    new Bomba(3),
    new Bomba(3)
};

// Verificar todas
foreach (var bomba in bombas)
{
    if (bomba.TemBomba(linha, coluna))
        return true;
}
```

---

## 💡 Conceitos C# Aplicados

### **Fundamentos**
✅ **Matrizes Bidimensionais** (`string[,]`)  
✅ **Classes e Objetos** (instanciação com `new`)  
✅ **Encapsulamento** (`private` fields + `public` properties)  
✅ **Constructors** para inicialização automática  
✅ **Properties** (`get`, `private set`)  
✅ **Method Overloading** (sobrecarga)  

### **POO Avançado**
✅ **Separação de Responsabilidades** (SRP do SOLID)  
✅ **Coesão** (cada classe tem propósito único)  
✅ **Baixo Acoplamento** (classes conversam por interfaces claras)  
✅ **Objetos com Estado** (cada instância é independente)  

### **Boas Práticas**
✅ **DRY** (Don't Repeat Yourself) - sobrecarga de método  
✅ **Naming Conventions** (PascalCase para classes/métodos)  
✅ **XML Comments** (`/// <summary>`)  
✅ **Defensive Programming** (validações em métodos públicos)  

### **Console/Input**
✅ **ConsoleKeyInfo** para captura de teclas  
✅ **Console.ReadKey(true)** para leitura sem echo  
✅ **Console.Clear()** para redesenho dinâmico  
✅ **Random** para geração aleatória  

---

## 🎓 Aprendizados e Desafios Superados

### **1. Migração de Procedural para POO** 🏗️

**Desafio:** Transformar código procedural funcional em POO sem quebrar nada.

**Solução:**
1. Identificar entidades naturais (Tabuleiro, Bomba, Jogador)
2. Mover dados relacionados para dentro das classes
3. Transformar funções em métodos das classes apropriadas
4. Testar incrementalmente cada refatoração

```csharp
// Antes: Dados e funções separados
string[,] matriz = new string[3,3];
bool PosicaoOcupada(string[,] matriz, int linha, int coluna) { ... }

// Depois: Dados e comportamento juntos
class Tabuleiro {
    private string[,] matriz;
    public bool PosicaoOcupada(int linha, int coluna) { ... }
}
```

---

### **2. Encapsulamento Efetivo** 🔒

**Desafio:** Proteger dados sem perder funcionalidade.

**Solução:** Properties com `get` público e `set` privado ou inexistente.

```csharp
class Bomba
{
    private int linha;  // Completamente privado
    
    public int Linha { get { return linha; } }  // Leitura permitida
    // Sem setter - ninguém pode mudar a posição depois de criada
}
```

---

### **3. Sobrecarga de Métodos Inteligente** 🎯

**Desafio:** Evitar duplicação de código na exibição do tabuleiro (com/sem cursor).

**Solução:** Método sobrecarregado que reutiliza a lógica completa.

```csharp
// Evita isto (código duplicado):
public static void ExibirTabuleiro(Tabuleiro tabuleiro, int l, int c) { ... 20 linhas ... }
public static void ExibirTabuleiroSemCursor(Tabuleiro tabuleiro) { ... 20 linhas repetidas ... }

// Faz isto (DRY):
public static void ExibirTabuleiro(Tabuleiro tabuleiro, int l, int c) { ... 20 linhas ... }
public static void ExibirTabuleiro(Tabuleiro tabuleiro) {
    ExibirTabuleiro(tabuleiro, -1, -1);  // 1 linha!
}
```

---

### **4. Responsabilidade de Revelação** 💣

**Desafio:** Quem deve revelar a bomba? A bomba mesmo ou o tabuleiro?

**Solução:** A bomba se revela, mas precisa do tabuleiro para fazê-lo.

```csharp
class Bomba
{
    public void RevelarBomba(Tabuleiro tabuleiro)
    {
        tabuleiro.ColocarSimbolo(linha, coluna, "💣");
    }
}
```

**Princípio:** Bomba conhece sua posição, Tabuleiro conhece como modificar a matriz. Colaboração!

---

### **5. Constructor com Lógica** 🎲

**Desafio:** Gerar posição aleatória quando o objeto é criado.

**Solução:** Lógica completa no constructor.

```csharp
public Bomba(int tamanhoTabuleiro)
{
    Random random = new Random();
    int posicaoBomba = random.Next(1, tamanhoTabuleiro * tamanhoTabuleiro + 1);
    
    this.linha = (posicaoBomba - 1) / tamanhoTabuleiro;
    this.coluna = (posicaoBomba - 1) % tamanhoTabuleiro;
}
```

**Vantagem:** `Bomba bomba = new Bomba(3);` já cria a bomba pronta para uso!

---

## 📊 Comparação de Código: Antes vs Depois

### **Criando uma Partida**

#### ❌ V2 (Procedural)
```csharp
string[,] campoMinado = new string[3, 3];
for (int i = 0; i < 3; i++)
    for (int j = 0; j < 3; j++)
        campoMinado[i, j] = "◻️";

Bomba.GerarBombaAleatoria();  // Modifica variável estática global
```

#### ✅ V3 (POO)
```csharp
Tabuleiro tabuleiro = new Tabuleiro(3);  // Já inicializa tudo
Bomba bomba = new Bomba(3);              // Já gera posição aleatória
```

**Benefício:** Mais limpo, mais claro, menos linhas.

---

### **Verificando Bomba**

#### ❌ V2 (Procedural)
```csharp
int posicao = linhaAtual * 3 + colunaAtual + 1;
if (Bomba.TemBomba(posicao))  // Acessa array estático global
{
    matriz[linhaAtual, colunaAtual] = "💣";
}
```

#### ✅ V3 (POO)
```csharp
if (bomba.TemBomba(linhaAtual, colunaAtual))  // Pergunta ao objeto
{
    bomba.RevelarBomba(tabuleiro);  // Objeto se revela
}
```

**Benefício:** Leitura natural, bomba gerencia seu próprio comportamento.

---

### **Jogadores**

#### ❌ V2 (Procedural)
```csharp
string player1 = "💀";
string player2 = "👽";
// Nome não existe, só o símbolo
Console.WriteLine($"{player1} VENCEU!");  // Só mostra emoji
```

#### ✅ V3 (POO)
```csharp
Jogador jogador1 = new Jogador("Jogador 1", "💀");
Jogador jogador2 = new Jogador("Jogador 2", "👽");
Console.WriteLine($"{jogador1.Simbolo} {jogador1.Nome} VENCEU!");
```

**Benefício:** Identidade completa, mais informação, código mais expressivo.

---

## 💻 Tecnologias

- **Linguagem:** C# 12 (.NET 8)
- **Paradigma:** Programação Orientada a Objetos (POO)
- **IDE:** Visual Studio 2022 / VS Code
- **Conceitos:** Classes, Encapsulamento, Properties, Constructors, Sobrecarga, Separação de Responsabilidades

---

## 📊 Changelog

### **v3.0** (Atual) - Refatoração POO
- ✅ Arquitetura orientada a objetos completa
- ✅ Classe `Tabuleiro` com encapsulamento da matriz
- ✅ Classe `Bomba` com posição encapsulada e comportamento próprio
- ✅ Classe `Jogador` com identidade completa (nome + símbolo)
- ✅ Sobrecarga de método `ExibirTabuleiro()` para flexibilidade
- ✅ Separação clara de responsabilidades (SRP)
- ✅ Preparado para escalabilidade (múltiplas bombas, tamanhos variados)
- ✅ Código mais manutenível e legível

### **v2.0** - Sistema de Navegação Visual
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

## 🎯 Jornada de Aprendizado

### **Por que esta evolução importa?**

Este projeto demonstra a capacidade de:

✅ **Evoluir código incrementalmente** sem reescrever do zero  
✅ **Aplicar POO em código procedural existente** (refatoração)  
✅ **Entender trade-offs** entre simplicidade inicial e arquitetura escalável  
✅ **Implementar conceitos avançados** (encapsulamento, sobrecarga, SRP)  
✅ **Pensar em design** além de "fazer funcionar"  

### **Habilidades Demonstradas**

- **Análise:** Identificar entidades naturais em um problema
- **Design:** Separar responsabilidades de forma coerente
- **Refatoração:** Melhorar código sem quebrar funcionalidade
- **POO:** Aplicar princípios de encapsulamento e coesão
- **Escalabilidade:** Código preparado para crescer (múltiplas bombas, etc)

---

## 👨‍💻 Sobre o Desenvolvedor

**Gabriel Henriques Cé**  
LinkedIn: [Gabriel Henrique Cé](https://linkedin.com/in/gabrielhenriquece)  
GitHub: [@GabrielHenriqueCe](https://github.com/GabrielHenriqueCe)

---

## 📝 Licença

MIT License - Código aberto para fins educacionais

---

## 🎮 Da Simplicidade à Arquitetura

_"V1 fez funcionar.  
V2 tornou intuitivo.  
V3 estruturou para crescer._

_A mesma bomba, o mesmo tabuleiro, a mesma diversão...  
Mas agora com uma fundação sólida para evoluir."_

---

**Status:** 🟢 **v3.0 Completo** - Arquitetura POO Sólida  
**Última Atualização:** Dezembro 2024  
**Linhas de Código:** ~450  
**Classes:** 7 (Tabuleiro, Bomba, Jogador, ExibirMatriz, CondicaoDeVitoria, SelecaoSimbolo, Program)  
**Baseado em:** [Campo-Minado V2](https://github.com/GabrielHenriqueCe/Campo-Minado-Csharp)

---

### 💣 Código limpo, arquitetura sólida, jogo divertido! 🚀
