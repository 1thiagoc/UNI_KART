# UNI_KART

O UNI_KART é um jogo de corrida desenvolvido em Unity como trabalho da disciplina de Computação Gráfica. A proposta do projeto é criar uma experiência em 3D inspirada no campus da Universidade de Fortaleza, onde o jogador controla um carro elétrico responsável por transportar alunos e funcionários entre diferentes pontos do campus.

A ideia principal do jogo é simples: buscar passageiros, levá-los até seus destinos e tentar fazer isso no menor tempo possível, sem comprometer o conforto e a satisfação de quem está sendo transportado.

## Visão geral

No UNI_KART, o jogador assume o papel de um motorista recém contratado para o transporte interno da universidade. Durante as partidas, será necessário cumprir rotas, buscar alunos em pontos específicos e deixá-los em diferentes locais do mapa.

O jogo tem uma proposta leve, com estilo visual compacto e descontraído, buscando uma atmosfera divertida e acessível para todas as idades.

## Informações do projeto

Nome do jogo: UNI_KART

Gênero: Corrida

Motor de jogo: Unity

Plataformas previstas: PC Windows, Linux e Android

Público alvo: Todas as idades

Disciplina: Computação Gráfica

## Conceito do jogo

O jogador dirige um carro elétrico dentro de uma versão reduzida e estilizada do campus da UNIFOR. O objetivo é transportar o maior número possível de passageiros, equilibrando velocidade, tempo de entrega e satisfação dos passageiros.

Cada corrida precisa ser feita com cuidado. Se o jogador dirigir rápido demais, alguns passageiros podem ficar com medo. Se dirigir devagar demais, eles podem ficar impacientes. Por isso, o desempenho não depende apenas de chegar rápido, mas também de manter uma boa experiência durante o trajeto.

## Mecânicas principais

Controlar o carrinho pelo cenário.

Buscar passageiros em pontos específicos do mapa.

Levar os passageiros até seus destinos.

Controlar a velocidade da corrida.

Manter o nível de satisfação dos passageiros.

Passar por fases com diferentes quantidades de alunos.

Lidar com passageiros de personalidades diferentes.

## Sistema de satisfação

A pontuação do jogador está relacionada ao nível de satisfação dos passageiros. Esse sistema considera principalmente a forma como o trajeto é realizado.

Se a corrida for muito rápida, o passageiro pode sentir medo e a pontuação diminui. Se a corrida for muito lenta, o passageiro pode ficar impaciente, o que também reduz a pontuação.

A ideia é fazer com que o jogador encontre um equilíbrio entre rapidez e segurança.

## Tipos de passageiros

O jogo possui diferentes perfis de passageiros, cada um com uma reação diferente durante a corrida.

Aluno atrasado: prefere corridas mais rápidas, fica impaciente com lentidão e tolera velocidades maiores.

Aluno perdido: não gosta de demora, mas também não tolera altas velocidades.

Professor: prefere equilíbrio entre velocidade, segurança e conforto.

Veterano: é mais tolerante tanto com atrasos quanto com velocidades mais altas.

Essas diferenças tornam cada corrida mais dinâmica, já que o jogador precisa adaptar sua forma de dirigir dependendo do passageiro.

## Mundo do jogo

O cenário do UNI_KART é inspirado nos principais pontos de encontro do campus da UNIFOR, como centro de convivência, biblioteca, arena de esportes, piscina e blocos de aula.

O mapa será uma recriação reduzida e focada nos locais mais importantes para a jogabilidade. A proposta não é reproduzir o campus de forma totalmente realista, mas criar um ambiente reconhecível, funcional e divertido para o jogo.

## Progressão

A estrutura do jogo é pensada em fases. Conforme o jogador avança, o número de passageiros e os desafios aumentam.

Uma das ideias do projeto é relacionar as fases com diferentes momentos do semestre. Em períodos próximos às provas, por exemplo, podem aparecer mais alunos atrasados, exigindo corridas mais rápidas e aumentando a pressão sobre o jogador.

## Arte e áudio

O estilo visual do jogo segue uma proposta simples e estilizada, com personagens e objetos em estilo pocket. A ideia é manter uma identidade visual leve, colorida e amigável.

A trilha sonora prevista segue uma linha lofi, ajudando a criar uma atmosfera tranquila durante a experiência. Além disso, o jogo também deve contar com efeitos sonoros para ações importantes, como movimentação, menus, coleta de passageiros e chegada ao destino.

## Interface do usuário

A interface do jogo deve apresentar informações importantes para o jogador durante a partida, como barra de satisfação do passageiro, indicadores de medo e impaciência, seta ou minimapa indicando o destino, menus de configuração, menu de pausa e informações básicas da corrida.

A UI deve ser simples, clara e fácil de entender, permitindo que o jogador acompanhe a situação da corrida sem atrapalhar a jogabilidade.

## Considerações técnicas

O projeto está sendo desenvolvido na Unity, utilizando C# para a programação dos sistemas do jogo.

Algumas técnicas previstas para melhorar o desempenho são o uso de Occlusion Culling para evitar renderização de objetos ocultos, compressão de texturas, organização dos objetos e cenas, além de otimização para diferentes plataformas.

## Metodologia

A equipe utiliza a metodologia Kanban para organizar as tarefas do projeto. As atividades são divididas em etapas, facilitando o acompanhamento do que precisa ser feito, do que está em andamento e do que já foi concluído.

## Próximas etapas

Implementar a movimentação básica do carro.

Criar os mapas principais.

Adicionar áudios e efeitos sonoros.

Criar a interface do usuário.

Implementar os modelos 3D finais.

Criar sistema para salvar estado.

Adicionar novos mapas e melhorias ao jogo.

## Integrantes

Andressa Evelyn Lima de Luna

Caio Ciriaco Ribeiro

João Alex Vieira de Almeida

Thiago Cavalcante da Silva

## Como abrir o projeto

Para abrir o projeto na sua máquina, clone o repositório usando o comando abaixo:

git clone https://github.com/1thiagoc/UNI_KART.git

Depois, abra o Unity Hub, clique em Add ou Open, selecione a pasta do projeto e aguarde o carregamento dos arquivos.

Com o projeto aberto, selecione a cena principal e clique em Play para executar.

## Status

Projeto em desenvolvimento.
