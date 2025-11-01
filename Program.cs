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
