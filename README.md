# UOL.EdTech.Gamificacao

_Worker service_ desenvolvido em C# utilizando [.NET Core 3.1](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-1) inicialmente como parte de um projeto de gamificação para a UOLEdTech

## 1. Configurações
As configurações de arquivo de origem, escrita, json e assim como os parâmetros de intervalo entre execução e os níveis escolhidos, podem ser encontradas no arquivo [appsettings.json](Uol.EdTech.Gamification.Worker\appsettings.json)

```json
  "ApplicationConfig": {
    "arquivoLeitura": "Dados do ERP.txt",
    "arquivoEscrita": "ArquivoResultado.txt",
    "arquivoJson": "Leitura.json",
    "horasEntreExecucao": 24,
    "Niveis": [ 50, 30, 10, 8 , 2]
  }
```

## 2. TODO
1. Ferramenta para auxílio durante deploy
2. Aumento da cobertura de testes
3. Utilização de sonar para detecção de code smells e análise de cobertura de testes
