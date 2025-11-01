// PADARIA SÃO VICENTE - GESTÃO DE ESTOQUE APP

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class GestaoEstoque
{
    private class ItemEstoque
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public int Quantidade { get; set; }
        public DateTime Validade { get; set; }
        public string Operador { get; set; }
        public DateTime DataCompra { get; set; }
    }

    private class ItemReposicao
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public string Operador { get; set; }
        public DateTime Data { get; set; }
    }

    private List<ItemEstoque> listaEstoque = new();
    private List<ItemReposicao> listaReposicao = new();

    public void ReceberInsumo(string nome, int quantidade, string unidade, DateTime validade, string operador)
    {
        var existente = listaEstoque.FirstOrDefault(i => i.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (existente != null)
        {
            existente.Quantidade += quantidade;
            existente.Validade = validade;
            existente.DataCompra = DateTime.Now;
            Console.WriteLine($"Insumo '{nome}' atualizado. Quantidade total: {existente.Quantidade} {existente.Unidade}.");
        }
        else
        {
            listaEstoque.Add(new ItemEstoque
            {
                Nome = nome,
                Quantidade = quantidade,
                Unidade = unidade,
                Validade = validade,
                Operador = operador,
                DataCompra = DateTime.Now
            });
            Console.WriteLine($"Insumo '{nome}' recebido e registrado. Quantidade: {quantidade} {unidade}.");
        }

        listaReposicao.RemoveAll(r => r.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    public void SolicitarCompra(string nome, string unidade, int minimo, string operador)
    {
        var item = listaEstoque.FirstOrDefault(i => i.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (item == null || item.Quantidade <= minimo)
        {
            if (!listaReposicao.Any(r => r.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            {
                listaReposicao.Add(new ItemReposicao
                {
                    Nome = nome,
                    Unidade = unidade,
                    Operador = operador,
                    Data = DateTime.Now
                });
                Console.WriteLine($"Solicitação de compra gerada para o insumo '{nome}'.");
            }
        }
        else
        {
            Console.WriteLine($"Estoque de '{nome}' ainda acima do mínimo ({item.Quantidade} {item.Unidade}). Nenhuma solicitação criada.");
        }
    }

    public void MostrarEstoque()
    {
        Console.WriteLine($"\n=== ESTOQUE ATUAL ({DateTime.Now:dd/MM/yyyy HH:mm}) ===");

        var grupos = listaEstoque.GroupBy(e => e.Unidade);

        foreach (var grupo in grupos)
        {
            Console.WriteLine($"\n--- Unidade: {grupo.Key} ---");
            foreach (var item in grupo)
            {
                Console.WriteLine($"{item.Nome}: {item.Quantidade} {item.Unidade} | " +
                                  $"Validade: {item.Validade:dd/MM/yyyy} | Compra: {item.DataCompra:dd/MM/yyyy HH:mm} | Operador: {item.Operador}");
            }
        }

        if (listaReposicao.Count > 0)
        {
            Console.WriteLine("\n=== REPOSIÇÃO PENDENTE ===");
            foreach (var item in listaReposicao)
            {
                Console.WriteLine($"{item.Nome} ({item.Unidade}) | Solicitado em: {item.Data:dd/MM/yyyy HH:mm} | Por: {item.Operador}");
            }
        }
        else
        {
            Console.WriteLine("\nNenhuma reposição pendente no momento.");
        }
    }

    static void Main()
    {
        var sistema = new GestaoEstoque();
        int opcao;

        do
        {
            Console.WriteLine("\n===================================");
            Console.WriteLine("  PADARIA SÃO VICENTE - ESTOQUE");
            Console.WriteLine("===================================");
            Console.WriteLine("1. Receber insumo");
            Console.WriteLine("2. Solicitar compra/reposição");
            Console.WriteLine("3. Mostrar estoque");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha: ");

            if (!int.TryParse(Console.ReadLine(), out opcao)) opcao = -1;

            switch (opcao)
            {
                case 1:
                    Console.Write("Nome: ");
                    var nome = Console.ReadLine().Trim();

                    int qtd;
                    while (true)
                    {
                        Console.Write("Quantidade: ");
                        if (int.TryParse(Console.ReadLine(), out qtd) && qtd > 0) break;
                        Console.WriteLine("Quantidade inválida. Digite um número inteiro positivo.");
                    }

                    Console.Write("Unidade (kg, g, L, etc.): ");
                    var unidade = Console.ReadLine().Trim();

                    DateTime validade;
                    while (true)
                    {
                        Console.Write("Validade (dd/mm/aaaa): ");
                        string dataEntrada = Console.ReadLine().Trim();

                        if (!DateTime.TryParseExact(dataEntrada, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validade))
                        {
                            Console.WriteLine("Data inválida. Use o formato dd/mm/aaaa.");
                            continue;
                        }
                        if (validade < DateTime.Today)
                        {
                            Console.WriteLine("Data inválida. Digite uma data futura.");
                            continue;
                        }
                        break;
                    }

                    Console.Write("Operador: ");
                    var operador = Console.ReadLine().Trim();

                    sistema.ReceberInsumo(nome, qtd, unidade, validade, operador);
                    break;

                case 2:
                    Console.Write("Nome: ");
                    nome = Console.ReadLine().Trim();

                    Console.Write("Unidade (kg, g, L, etc.): ");
                    unidade = Console.ReadLine().Trim();

                    int minimo;
                    while (true)
                    {
                        Console.Write("Quantidade mínima: ");
                        if (int.TryParse(Console.ReadLine(), out minimo) && minimo >= 0) break;
                        Console.WriteLine("Quantidade mínima inválida. Digite um número inteiro igual ou maior que zero.");
                    }

                    Console.Write("Operador solicitante: ");
                    operador = Console.ReadLine().Trim();

                    sistema.SolicitarCompra(nome, unidade, minimo, operador);
                    break;

                case 3:
                    sistema.MostrarEstoque();
                    break;

                case 0:
                    Console.WriteLine("Encerrando o sistema...");
                    break;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        } while (opcao != 0);
    }
}

class GestaoEstoque
{
    private class ItemEstoque
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public int Quantidade { get; set; }
        public DateTime Validade { get; set; }
    }

    private class RegistroSaida
    {
        public string Nome { get; set; }
        public int QuantidadeLiberada { get; set; }
        public string Unidade { get; set; }
        public string Responsavel { get; set; }
        public string Destino { get; set; }
        public DateTime DataSaida { get; set; }
        public string Aprovador { get; set; }
    }

    private List<ItemEstoque> listaEstoque = new();
    private List<RegistroSaida> historicoSaidas = new();

    // Dicionário de usuários e permissões (exemplo simples)
    private Dictionary<string, string> usuarios = new()
    {
        { "joao", "operador" },
        { "maria", "supervisor" },
        { "ana", "gerente" }
    };

    public GestaoEstoque()
    {
        // Estoque inicial (exemplo)
        listaEstoque.Add(new ItemEstoque { Nome = "Farinha", Unidade = "kg", Quantidade = 50, Validade = DateTime.Now.AddMonths(3) });
        listaEstoque.Add(new ItemEstoque { Nome = "Açúcar", Unidade = "kg", Quantidade = 30, Validade = DateTime.Now.AddMonths(2) });
        listaEstoque.Add(new ItemEstoque { Nome = "Leite", Unidade = "L", Quantidade = 20, Validade = DateTime.Now.AddDays(15) });
    }

    public void LiberarInsumo(string nome, int quantidade, string destino, string responsavel)
    {
        // Verifica se o usuário existe
        if (!usuarios.ContainsKey(responsavel))
        {
            Console.WriteLine($"Usuário '{responsavel}' não encontrado no sistema.");
            return;
        }

        string cargo = usuarios[responsavel];
        var item = listaEstoque.FirstOrDefault(i => i.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (item == null)
        {
            Console.WriteLine($"Insumo '{nome}' não encontrado no estoque.");
            return;
        }

        if (quantidade <= 0)
        {
            Console.WriteLine("A quantidade liberada deve ser maior que zero.");
            return;
        }

        if (item.Quantidade < quantidade)
        {
            Console.WriteLine($"Quantidade insuficiente de '{nome}' no estoque. Disponível: {item.Quantidade} {item.Unidade}.");
            return;
        }

        // Controle de autorização
        string aprovador = "";
        if (cargo == "operador")
        {
            Console.Write("Operador não possui permissão direta. Informe o nome do aprovador: ");
            aprovador = Console.ReadLine().Trim();

            if (!usuarios.ContainsKey(aprovador) || usuarios[aprovador] == "operador")
            {
                Console.WriteLine("Aprovador inválido ou sem permissão.");
                return;
            }
        }

        // Liberação confirmada
        item.Quantidade -= quantidade;

        historicoSaidas.Add(new RegistroSaida
        {
            Nome = nome,
            QuantidadeLiberada = quantidade,
            Unidade = item.Unidade,
            Responsavel = responsavel,
            Destino = destino,
            DataSaida = DateTime.Now,
            Aprovador = aprovador
        });

        Console.WriteLine($"\nInsumo '{nome}' liberado com sucesso!");
        Console.WriteLine($"Quantidade: {quantidade} {item.Unidade} | Destino: {destino} | Responsável: {responsavel}");
        if (!string.IsNullOrEmpty(aprovador))
            Console.WriteLine($"Aprovado por: {aprovador}");
    }

    public void MostrarHistoricoSaidas()
    {
        Console.WriteLine("\n=== HISTÓRICO DE SAÍDAS ===");
        if (historicoSaidas.Count == 0)
        {
            Console.WriteLine("Nenhuma liberação registrada.");
            return;
        }

        foreach (var r in historicoSaidas)
        {
            Console.WriteLine($"{r.DataSaida:dd/MM/yyyy HH:mm} - {r.Nome} ({r.QuantidadeLiberada} {r.Unidade}) " +
                              $"| Destino: {r.Destino} | Responsável: {r.Responsavel}" +
                              (string.IsNullOrEmpty(r.Aprovador) ? "" : $" | Aprovador: {r.Aprovador}"));
        }
    }

    static void Main()
    {
        var sistema = new GestaoEstoque();
        int opcao;

        do
        {
            Console.WriteLine("\n===================================");
            Console.WriteLine("  PADARIA SÃO VICENTE - LIBERAÇÃO DE INSUMOS");
            Console.WriteLine("===================================");
            Console.WriteLine("1. Liberar insumo");
            Console.WriteLine("2. Mostrar histórico de saídas");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha: ");

            if (!int.TryParse(Console.ReadLine(), out opcao)) opcao = -1;

            switch (opcao)
            {
                case 1:
                    Console.Write("Nome do insumo: ");
                    var nome = Console.ReadLine().Trim();

                    Console.Write("Quantidade a liberar: ");
                    int qtd;
                    if (!int.TryParse(Console.ReadLine(), out qtd))
                    {
                        Console.WriteLine("Valor inválido.");
                        break;
                    }

                    Console.Write("Destino (ex: produção, venda, descarte): ");
                    var destino = Console.ReadLine().Trim();

                    Console.Write("Responsável pela liberação: ");
                    var responsavel = Console.ReadLine().Trim();

                    sistema.LiberarInsumo(nome, qtd, destino, responsavel);
                    break;

                case 2:
                    sistema.MostrarHistoricoSaidas();
                    break;

                case 0:
                    Console.WriteLine("Encerrando o sistema...");
                    break;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        } while (opcao != 0);
    }
}

class GestaoArmazenamento
{
    private class ItemEstoque
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public int Quantidade { get; set; }
        public string Localizacao { get; set; } // ex: "Prateleira A", "Câmara Fria"
    }

    private class RegistroAuditoria
    {
        public DateTime DataHora { get; set; }
        public string Usuario { get; set; }
        public string Operacao { get; set; } // Inserção, Alteração, Remoção, etc.
        public string Item { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNovo { get; set; }
    }

    private List<ItemEstoque> listaEstoque = new();
    private List<RegistroAuditoria> logsAuditoria = new();

    // Controle de usuários e permissões básicas
    private Dictionary<string, string> usuarios = new()
    {
        { "joao", "operador" },
        { "maria", "supervisor" },
        { "ana", "gerente" }
    };

    // --- Função para registrar qualquer operação no log ---
    private void RegistrarAuditoria(string usuario, string operacao, string item, string valorAntigo, string valorNovo)
    {
        logsAuditoria.Add(new RegistroAuditoria
        {
            DataHora = DateTime.Now,
            Usuario = usuario,
            Operacao = operacao,
            Item = item,
            ValorAnterior = valorAntigo,
            ValorNovo = valorNovo
        });
    }

    // --- Função para armazenar (ou atualizar) um insumo ---
    public void ArmazenarInsumo(string nome, int quantidade, string unidade, string localizacao, string usuario)
    {
        if (!usuarios.ContainsKey(usuario))
        {
            Console.WriteLine("Usuário não autorizado.");
            return;
        }

        var existente = listaEstoque.FirstOrDefault(i => i.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (existente != null)
        {
            // Atualiza o item existente e registra log da alteração
            string valorAntigo = $"{existente.Quantidade} {existente.Unidade} em {existente.Localizacao}";
            existente.Quantidade += quantidade;
            existente.Localizacao = localizacao;
            existente.Unidade = unidade;

            RegistrarAuditoria(usuario, "ATUALIZAÇÃO DE ESTOQUE", nome, valorAntigo, $"{existente.Quantidade} {existente.Unidade} em {existente.Localizacao}");
            Console.WriteLine($"Insumo '{nome}' atualizado. Novo total: {existente.Quantidade} {existente.Unidade}.");
        }
        else
        {
            // Cria novo registro de item e registra auditoria
            listaEstoque.Add(new ItemEstoque
            {
                Nome = nome,
                Quantidade = quantidade,
                Unidade = unidade,
                Localizacao = localizacao
            });

            RegistrarAuditoria(usuario, "ARMAZENAMENTO NOVO", nome, "N/A", $"{quantidade} {unidade} em {localizacao}");
            Console.WriteLine($"Insumo '{nome}' armazenado com sucesso ({quantidade} {unidade} em {localizacao}).");
        }
    }

    // --- Função para alterar quantidade manualmente (com log obrigatório) ---
    public void AlterarQuantidade(string nome, int novaQuantidade, string usuario)
    {
        if (!usuarios.ContainsKey(usuario))
        {
            Console.WriteLine("Usuário não autorizado.");
            return;
        }

        var item = listaEstoque.FirstOrDefault(i => i.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            Console.WriteLine("Item não encontrado no estoque.");
            return;
        }

        string valorAntigo = $"{item.Quantidade} {item.Unidade}";
        item.Quantidade = novaQuantidade;

        RegistrarAuditoria(usuario, "ALTERAÇÃO MANUAL DE QUANTIDADE", nome, valorAntigo, $"{item.Quantidade} {item.Unidade}");
        Console.WriteLine($"Quantidade de '{nome}' atualizada para {item.Quantidade} {item.Unidade}.");
    }

    // --- Exibir Estoque Atual ---
    public void MostrarEstoque()
    {
        Console.WriteLine("\n=== ESTOQUE ATUAL ===");
        if (listaEstoque.Count == 0)
        {
            Console.WriteLine("Nenhum insumo armazenado.");
            return;
        }

        foreach (var item in listaEstoque)
        {
            Console.WriteLine($"{item.Nome}: {item.Quantidade} {item.Unidade} | Localização: {item.Localizacao}");
        }
    }

    // --- Exibir Log de Auditoria ---
    public void MostrarAuditoria()
    {
        Console.WriteLine("\n=== LOG DE AUDITORIA ===");
        if (logsAuditoria.Count == 0)
        {
            Console.WriteLine("Nenhuma operação registrada ainda.");
            return;
        }

        foreach (var log in logsAuditoria)
        {
            Console.WriteLine($"{log.DataHora:dd/MM/yyyy HH:mm:ss} | Usuário: {log.Usuario} | Operação: {log.Operacao}");
            Console.WriteLine($"Item: {log.Item}");
            Console.WriteLine($"De: {log.ValorAnterior} -> Para: {log.ValorNovo}\n");
        }
    }

    // --- Programa Principal ---
    static void Main()
    {
        var sistema = new GestaoArmazenamento();
        int opcao;

        do
        {
            Console.WriteLine("\n===================================");
            Console.WriteLine(" PADARIA SÃO VICENTE - ARMAZENAMENTO DE INSUMOS ");
            Console.WriteLine("===================================");
            Console.WriteLine("1. Armazenar / Atualizar insumo");
            Console.WriteLine("2. Alterar quantidade manualmente");
            Console.WriteLine("3. Mostrar estoque atual");
            Console.WriteLine("4. Mostrar log de auditoria");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha: ");

            if (!int.TryParse(Console.ReadLine(), out opcao)) opcao = -1;

            switch (opcao)
            {
                case 1:
                    Console.Write("Nome do insumo: ");
                    var nome = Console.ReadLine().Trim();

                    Console.Write("Quantidade: ");
                    int qtd;
                    if (!int.TryParse(Console.ReadLine(), out qtd) || qtd <= 0)
                    {
                        Console.WriteLine("Quantidade inválida.");
                        break;
                    }

                    Console.Write("Unidade (kg, L, g, etc.): ");
                    var unidade = Console.ReadLine().Trim();

                    Console.Write("Localização física: ");
                    var local = Console.ReadLine().Trim();

                    Console.Write("Usuário responsável: ");
                    var usuario = Console.ReadLine().Trim();

                    sistema.ArmazenarInsumo(nome, qtd, unidade, local, usuario);
                    break;

                case 2:
                    Console.Write("Nome do insumo: ");
                    nome = Console.ReadLine().Trim();

                    Console.Write("Nova quantidade: ");
                    int novaQtd;
                    if (!int.TryParse(Console.ReadLine(), out novaQtd))
                    {
                        Console.WriteLine("Valor inválido.");
                        break;
                    }

                    Console.Write("Usuário responsável: ");
                    usuario = Console.ReadLine().Trim();

                    sistema.AlterarQuantidade(nome, novaQtd, usuario);
                    break;

                case 3:
                    sistema.MostrarEstoque();
                    break;

                case 4:
                    sistema.MostrarAuditoria();
                    break;

                case 0:
                    Console.WriteLine("Encerrando o sistema...");
                    break;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        } while (opcao != 0);
    }
}
