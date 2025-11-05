using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class SistemaGestaoPadaria
{
    private List<ItemEstoque> listaEstoque = new();
    private List<ItemReposicao> listaReposicao = new();
    private List<ItemSaida> historicoSaidas = new();
    private List<LogAuditoria> logsAuditoria = new();

    private string usuarioAtual;
    private const string arquivoEstoque = "estoque.json";
    private const string arquivoReposicao = "reposicao.json";
    private const string arquivoSaidas = "saidas.json";
    private const string arquivoAuditoria = "auditoria.json";

    public SistemaGestaoPadaria(string usuario)
    {
        usuarioAtual = usuario;
        CarregarDados();
    }

    public void Executar()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== SISTEMA DE GESTÃO DE ESTOQUE ===");
            Console.WriteLine($"Usuário: {usuarioAtual}");
            Console.WriteLine("\n1. Adicionar item ao estoque");
            Console.WriteLine("2. Liberar insumo");
            Console.WriteLine("3. Relatórios");
            Console.WriteLine("4. Sair");
            Console.Write("Escolha uma opção: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": AdicionarItemEstoque(); break;
                case "2": LiberarInsumo(); break;
                case "3": MenuRelatorios(); break;
                case "4": SalvarDados(); return;
                default: Console.WriteLine("Opção inválida."); Pausa(); break;
            }
        }
    }

    private void AdicionarItemEstoque()
    {
        Console.Clear();
        Console.WriteLine("=== ADICIONAR ITEM AO ESTOQUE ===");
        Console.Write("Nome do item: ");
        string nome = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(nome)) { Console.WriteLine("Nome inválido."); Pausa(); return; }

        Console.Write("Unidade (ex: kg, un): ");
        string unidade = Console.ReadLine()?.Trim() ?? "un";

        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out int qtd) || qtd <= 0) { Console.WriteLine("Quantidade inválida."); Pausa(); return; }

        Console.Write("Data de validade (dd/mm/aaaa): ");
        DateTime validade = DateTime.TryParse(Console.ReadLine(), out DateTime val) ? val : DateTime.Now.AddMonths(1);

        var item = listaEstoque.FirstOrDefault(i => i.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            listaEstoque.Add(new ItemEstoque(nome, unidade, qtd, validade, DateTime.Now));
        }
        else
        {
            item.Quantidade += qtd;
            item.DataUltimaAtualizacao = DateTime.Now;
        }

        RegistrarAuditoria($"Adicionado {qtd} {unidade} de {nome} (Val: {validade:dd/MM/yyyy})");
        SalvarDados();
        Console.WriteLine("\nItem adicionado com sucesso!");
        Pausa();
    }

    private void LiberarInsumo()
    {
        Console.Clear();
        Console.WriteLine("=== LIBERAR INSUMO ===");
        Console.Write("Nome do item: ");
        string nome = Console.ReadLine()?.Trim() ?? "";
        var item = listaEstoque.FirstOrDefault(i => i.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
        if (item == null) { Console.WriteLine("Item não encontrado."); Pausa(); return; }

        Console.Write($"Quantidade disponível: {item.Quantidade} {item.Unidade}. Liberar quanto: ");
        if (!int.TryParse(Console.ReadLine(), out int qtd) || qtd <= 0) { Console.WriteLine("Quantidade inválida."); Pausa(); return; }
        if (qtd > item.Quantidade) { Console.WriteLine("Quantidade insuficiente."); Pausa(); return; }

        Console.Write("Destinatário: ");
        string dest = Console.ReadLine()?.Trim() ?? "";
        item.Quantidade -= qtd;
        item.DataUltimaAtualizacao = DateTime.Now;
        historicoSaidas.Add(new ItemSaida(nome, item.Unidade, qtd, dest, DateTime.Now));
        RegistrarAuditoria($"Liberado {qtd} {item.Unidade} de {nome} para {dest}");
        SalvarDados();

        Console.WriteLine("\nInsumo liberado.");
        Pausa();
    }

    private void MenuRelatorios()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== RELATÓRIOS ===");
            Console.WriteLine("1. Estoque Atual");
            Console.WriteLine("2. Reposições Pendentes");
            Console.WriteLine("3. Histórico de Saídas");
            Console.WriteLine("4. Auditoria");
            Console.WriteLine("B. Voltar");
            Console.Write("Escolha: ");
            switch (Console.ReadLine()?.Trim().ToUpper())
            {
                case "1": MostrarEstoqueAtual(); break;
                case "2": MostrarReposicaoPendente(); break;
                case "3": MostrarHistoricoSaidas(); break;
                case "4": MostrarAuditoria(); break;
                case "B": return;
                default: Console.WriteLine("Opção inválida."); Pausa(); break;
            }
        }
    }

    private void MostrarEstoqueAtual()
    {
        Console.Clear();
        Console.WriteLine("=== ESTOQUE ATUAL ===");
        if (!listaEstoque.Any()) { Console.WriteLine("Estoque vazio."); Pausa(); return; }

        Console.WriteLine($"{"Item",-25} {"Qtd",6} {"Unid",-5} {"Validade",-12}");
        foreach (var i in listaEstoque.OrderBy(x => x.Nome))
            Console.WriteLine($"{i.Nome,-25} {i.Quantidade,6} {i.Unidade,-5} {i.Validade:dd/MM/yyyy}");
        Pausa();
    }

    private void MostrarReposicaoPendente()
    {
        Console.Clear();
        Console.WriteLine("=== REPOSIÇÕES PENDENTES ===");
        if (!listaReposicao.Any()) { Console.WriteLine("Nenhuma reposição pendente."); Pausa(); return; }

        foreach (var i in listaReposicao)
            Console.WriteLine($"{i.Nome,-25} {i.Quantidade,6} {i.Unidade,-5} Solicitado em {i.DataSolicitacao:dd/MM/yyyy}");
        Pausa();
    }

    private void MostrarHistoricoSaidas()
    {
        Console.Clear();
        Console.WriteLine("=== HISTÓRICO DE SAÍDAS ===");
        if (!historicoSaidas.Any()) { Console.WriteLine("Nenhuma saída registrada."); Pausa(); return; }

        foreach (var s in historicoSaidas)
            Console.WriteLine($"{s.DataSaida:dd/MM/yyyy HH:mm} | {s.Nome,-20} | {s.Quantidade,5} {s.Unidade,-5} -> {s.Destinatario}");
        Pausa();
    }

    private void MostrarAuditoria()
    {
        Console.Clear();
        Console.WriteLine("=== LOG DE AUDITORIA ===");
        if (!logsAuditoria.Any()) { Console.WriteLine("Sem registros."); Pausa(); return; }

        foreach (var log in logsAuditoria)
            Console.WriteLine($"{log.DataHora:dd/MM/yyyy HH:mm:ss} | {log.Usuario,-10} | {log.Descricao}");
        Pausa();
    }

    private void RegistrarAuditoria(string descricao)
    {
        logsAuditoria.Add(new LogAuditoria(Guid.NewGuid(), usuarioAtual, descricao, DateTime.Now));
    }

    private void CarregarDados()
    {
        listaEstoque = LerArquivo<List<ItemEstoque>>(arquivoEstoque) ?? new();
        listaReposicao = LerArquivo<List<ItemReposicao>>(arquivoReposicao) ?? new();
        historicoSaidas = LerArquivo<List<ItemSaida>>(arquivoSaidas) ?? new();
        logsAuditoria = LerArquivo<List<LogAuditoria>>(arquivoAuditoria) ?? new();
    }

    private void SalvarDados()
    {
        EscreverArquivo(arquivoEstoque, listaEstoque);
        EscreverArquivo(arquivoReposicao, listaReposicao);
        EscreverArquivo(arquivoSaidas, historicoSaidas);
        EscreverArquivo(arquivoAuditoria, logsAuditoria);
    }

    private static T? LerArquivo<T>(string caminho)
    {
        if (!File.Exists(caminho)) return default;
        try { return JsonSerializer.Deserialize<T>(File.ReadAllText(caminho)); }
        catch { return default; }
    }

    private static void EscreverArquivo<T>(string caminho, T dados)
    {
        File.WriteAllText(caminho, JsonSerializer.Serialize(dados, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static void Pausa()
    {
        Console.WriteLine("\nPressione ENTER para continuar...");
        Console.ReadLine();
    }

    private class ItemEstoque
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public int Quantidade { get; set; }
        public DateTime Validade { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }

        public ItemEstoque(string nome, string unidade, int qtd, DateTime validade, DateTime atualizacao)
        {
            Nome = nome; Unidade = unidade; Quantidade = qtd; Validade = validade; DataUltimaAtualizacao = atualizacao;
        }
    }

    private class ItemReposicao
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataSolicitacao { get; set; }

        public ItemReposicao(string nome, string unidade, int qtd, DateTime data)
        {
            Nome = nome; Unidade = unidade; Quantidade = qtd; DataSolicitacao = data;
        }
    }

    private class ItemSaida
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public int Quantidade { get; set; }
        public string Destinatario { get; set; }
        public DateTime DataSaida { get; set; }

        public ItemSaida(string nome, string unidade, int qtd, string dest, DateTime data)
        {
            Nome = nome; Unidade = unidade; Quantidade = qtd; Destinatario = dest; DataSaida = data;
        }
    }

    private class LogAuditoria
    {
        public Guid Id { get; set; }
        public string Usuario { get; set; }
        public string Descricao { get; set; }
        public DateTime DataHora { get; set; }

        public LogAuditoria(Guid id, string usuario, string descricao, DateTime data)
        {
            Id = id; Usuario = usuario; Descricao = descricao; DataHora = data;
        }
    }

    public static void Main()
    {
        string usuario = "admin"; // login automático
        var sistema = new SistemaGestaoPadaria(usuario);
        sistema.Executar();
    }
}
