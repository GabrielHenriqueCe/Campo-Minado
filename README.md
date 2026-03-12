# 💣 Campo Minado — C# Console Game

> Jogo de campo minado multiplayer para console, desenvolvido em C# como projeto de portfólio do programa Entra21.

---

## 🎮 Sobre o Jogo

Dois jogadores alternam turnos navegando pelo tabuleiro com **WASD**. Uma bomba está escondida — quem pisar nela perde. Simples de entender, difícil de dominar.

```
=====Campo Minado=====

  🟦  🟦  🟦  🟦
  🟦  🔳  🟦  🟦
  🟦  🟦  🟦  🟦
  🟦  🟦  🟦  🟦

✅ 1️⃣ Minerar | 🕛 2️⃣ Necromancia | 🕛 3️⃣ Lado Sombrio
Coloque o 💀 em uma posição
```

---

## ✨ Funcionalidades

- **🎭 11 personagens** com habilidades únicas divididos em 5 facções
- **⚔️ Modo Campanha** — 210 fases contra o Bot, com progressão e save
- **👥 Modo Versus** — 2 jogadores no mesmo teclado
- **🤖 Bot com IA** — usa habilidades e evita posições reveladas
- **❄️ Sistema Deusa** — evento especial quando dois jogadores escolhem o mesmo personagem
- **🏆 Sistema de troféus** — desbloqueia personagens ao avançar nas fases
- **💾 Save automático** — campanha e placar persistidos em arquivo

---

## 🧩 Personagens e Habilidades

| Facção | Personagem | Habilidade |
|--------|-----------|-----------|
| 🌑 Lado Sombrio | 💀 Caveira | Ressurge da morte e revela bombas vizinhas |
| 🌑 Lado Sombrio | 👻 Fantasma | 50% de chance de sobreviver a explosões |
| 🌑 Lado Sombrio | 🎃 Abóbora | Revela bombas em área 2x2 |
| ⚙️ Tecnológicos | 👽 Alien | Revela e cria armadilhas em área 2x2 |
| ⚙️ Tecnológicos | 🤖 Robô | Scan vertical — revela coluna inteira |
| ⚙️ Tecnológicos | 👾 Invasor | Revela 1 bomba e marca 1 casa segura por turno |
| 🪬 Folclore | 👹 Ogro | Pisa em área 2x2 destruindo bombas e armadilhas |
| 🪬 Folclore | 👺 Tengu | Revela linha inteira |
| 🪬 Folclore | 🤡 Palhaço | Embaralha o mapa com cartas por turno |
| ⭐ Especial | 💩 Cocô | Infecta linha com lama — desbloqueável na fase 110 |
| 🔱 Boss | 😈 Diabo | Visão total do oponente e cria bombas novas — desbloqueável na fase 210 |

Cada facção também tem um **passivo compartilhado** entre seus membros.

---

## 🏗️ Arquitetura

O projeto foi desenvolvido em **4 versões** com refatoração incremental:

| Versão | Paradigma | Destaque |
|--------|----------|---------|
| v1 | Procedural | Input numérico 1-9 |
| v2 | Procedural | Navegação WASD com cursor |
| v3 | POO básica | Classes, encapsulamento, separação de responsabilidades |
| v4 | POO avançada | Herança, polimorfismo, abstrações, generics, coleções |

### Principais classes (v4)

```
Partida          → agrupa Tabuleiro, Bombas, OlhosDoJogador e elementos especiais
Personagem       → nome, símbolo, habilidade pessoal e habilidade de facção
Habilidade       → classe abstrata base
  HabilidadeAtiva    → ativada manualmente pelo jogador
  HabilidadePassiva  → ativada automaticamente por eventos do jogo
Faction          → passivo compartilhado entre personagens da mesma facção
GerenciadorDeJogo → coordena campanha, versus e menu principal
```

---

## 💡 Conceitos C# Aplicados

`Classes e objetos` · `Encapsulamento` · `Herança` · `Polimorfismo` · `Classes abstratas` · `Interfaces implícitas` · `Generics` · `List<T>` · `Enum` · `Struct` · `File I/O` · `Exception handling` · `LINQ` · `Namespaces` · `Pattern matching` · `Sobrecarga de métodos`

---

## 🔭 Próximos Passos (v5)

- Refatoração da arquitetura: `JogadorBase` abstrato unificando campanha e versus
- Bot com níveis de dificuldade — fácil, médio e difícil, com lógica de decisão progressiva
- Novos personagens e um segundo evento de Deus (`🌞 Deus do Sol`)
- Reorganização completa dos arquivos por responsabilidade

---

## 🚀 Como Executar

```bash
git clone https://github.com/GabrielHenriqueCe/Campo-Minado
cd Campo-Minado
dotnet run
```

Requer **.NET 8+** e terminal com suporte a **UTF-8** para exibir os emojis corretamente.

---

## 👨‍💻 Desenvolvedor

**Gabriel Henriques Cé** — Engenharia de Software (3º semestre)

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Gabriel_Henriques_Cé-blue)](https://linkedin.com/in/gabrielhenriquece)
[![GitHub](https://img.shields.io/badge/GitHub-GabrielHenriqueCe-black)](https://github.com/GabrielHenriqueCe)
