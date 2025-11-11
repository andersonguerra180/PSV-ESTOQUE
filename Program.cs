using System;
using System.IO;

class InventarioBasico
{
    static void Main()
    {
        Console.WriteLine("===================================");
        Console.WriteLine("    INVENTÁRIO DO COMPUTADOR");
        Console.WriteLine("===================================");
        Console.WriteLine("1 - Inventário de Hardware");
        Console.WriteLine("2 - Inventário de Software");
        Console.WriteLine("3 - Inventário de Hardware e Software");
        Console.WriteLine("===================================");
        Console.Write("Escolha uma opção: ");
        string opcao = Console.ReadLine();

        string arquivo = "inventario.dat";
        StreamWriter sw = new StreamWriter(arquivo);

        if (opcao == "1")
        {
            sw.WriteLine("=== INVENTÁRIO DE HARDWARE ===");
            sw.WriteLine("Processador: Intel Core i5");
            sw.WriteLine("Memória RAM: 8 GB");
            sw.WriteLine("Disco Rígido: 500 GB SSD");
            sw.WriteLine("Placa de Rede: Realtek PCIe");
            sw.WriteLine("Placa de Vídeo: Integrada Intel UHD");
        }
        else if (opcao == "2")
        {
            sw.WriteLine("=== INVENTÁRIO DE SOFTWARE ===");
            sw.WriteLine("Sistema Operacional: Windows 10 Pro");
            sw.WriteLine("Versão: 22H2");
            sw.WriteLine("Antivírus: Windows Defender");
            sw.WriteLine("Pacote Office: Microsoft Office 365");
            sw.WriteLine("Navegador Padrão: Microsoft Edge");
        }
        else if (opcao == "3")
        {
            sw.WriteLine("=== INVENTÁRIO COMPLETO ===");
            sw.WriteLine("\n--- HARDWARE ---");
            sw.WriteLine("Processador: Intel Core i5");
            sw.WriteLine("Memória RAM: 8 GB");
            sw.WriteLine("Disco Rígido: 500 GB SSD");
            sw.WriteLine("Placa de Rede: Realtek PCIe");
            sw.WriteLine("Placa de Vídeo: Integrada Intel UHD");

            sw.WriteLine("\n--- SOFTWARE ---");
            sw.WriteLine("Sistema Operacional: Windows 10 Pro");
            sw.WriteLine("Versão: 22H2");
            sw.WriteLine("Antivírus: Windows Defender");
            sw.WriteLine("Pacote Office: Microsoft Office 365");
            sw.WriteLine("Navegador Padrão: Microsoft Edge");
        }
        else
        {
            Console.WriteLine("Opção inválida!");
            sw.Close();
            return;
        }

        sw.Close();
        Console.WriteLine("\nInventário gerado com sucesso!");
        Console.WriteLine("Arquivo salvo como: inventario.dat");
    }
}
