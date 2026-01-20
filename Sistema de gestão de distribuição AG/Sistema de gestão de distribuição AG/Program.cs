using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SistemaAguaUige
{
    
    public abstract class Pessoa
    {
        public string Nome { get; set; }
        public string BI { get; set; }

       
        public abstract string ObterDados();
    }

  
    public class Beneficiario : Pessoa
    {
        public int ID { get; set; }
        public string Municipio { get; set; }
        public string Bairro { get; set; }
        public int NumeroFamiliares { get; set; }

        public double CalcularAgua()
        {
            return NumeroFamiliares * 50.0;
        }

       
        public override string ObterDados()
        {
            return string.Format("ID: {0} | {1} | BI: {2} | {3}/{4} | Familiares: {5} | Água: {6}L/dia",
                ID, Nome, BI, Municipio, Bairro, NumeroFamiliares, CalcularAgua());
        }

        public string ParaCSV()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}",
                ID, Nome, BI, Municipio, Bairro, NumeroFamiliares);
        }
    }

    
    public class Gestor
    {
 
        private List<Beneficiario> lista;
        private string arquivo = "dados.csv";
        private int idAtual;

        public Gestor()
        {
            lista = new List<Beneficiario>();
            Carregar();
            idAtual = lista.Count > 0 ? lista[lista.Count - 1].ID + 1 : 1;
        }

        void Carregar()
        {
            if (!File.Exists(arquivo)) return;

            string[] linhas = File.ReadAllLines(arquivo, Encoding.UTF8);
            for (int i = 1; i < linhas.Length; i++)
            {
                string[] d = linhas[i].Split(';');
                if (d.Length >= 6)
                {
              
                    Beneficiario b = new Beneficiario();
                    b.ID = int.Parse(d[0]);
                    b.Nome = d[1];
                    b.BI = d[2];
                    b.Municipio = d[3];
                    b.Bairro = d[4];
                    b.NumeroFamiliares = int.Parse(d[5]);
                    lista.Add(b);
                }
            }
        }

        void Salvar()
        {
            StreamWriter w = new StreamWriter(arquivo, false, Encoding.UTF8);
            w.WriteLine("ID;Nome;BI;Municipio;Bairro;Familiares");
            foreach (var b in lista)
                w.WriteLine(b.ParaCSV());
            w.Close();
        }

   
        public void Adicionar(Beneficiario b)
        {
            b.ID = idAtual++;
            lista.Add(b);
            Salvar();
            Console.WriteLine("\nCadastrado com ID: " + b.ID);
        }

        public void Listar()
        {
            Console.WriteLine("\n=== BENEFICIÁRIOS ===\n");
            if (lista.Count == 0)
            {
                Console.WriteLine("Nenhum cadastro encontrado.");
                return;
            }
            foreach (var b in lista)
                Console.WriteLine(b.ObterDados()); 
            Console.WriteLine("\nTotal: " + lista.Count);
        }

        public Beneficiario Buscar(int id)
        {
            foreach (var b in lista)
                if (b.ID == id) return b;
            return null;
        }

        public void RelatorioCidade(string municipio)
        {
            Console.WriteLine("\n=== RELATÓRIO: " + municipio.ToUpper() + " ===\n");
            int fam = 0, pess = 0;
            double agua = 0;

            foreach (var b in lista)
            {
                if (b.Municipio.ToUpper() == municipio.ToUpper())
                {
                    fam++;
                    pess += b.NumeroFamiliares;
                    agua += b.CalcularAgua();
                }
            }

            Console.WriteLine("Famílias: " + fam);
            Console.WriteLine("Pessoas: " + pess);
            Console.WriteLine("Necessidade: " + agua + "L/dia");
        }

        public void RelatorioTotal()
        {
            Console.WriteLine("\n=== RELATÓRIO GERAL ===\n");
            if (lista.Count == 0)
            {
                Console.WriteLine("Sem dados.");
                return;
            }

            int pess = 0;
            double agua = 0;
            foreach (var b in lista)
            {
                pess += b.NumeroFamiliares;
                agua += b.CalcularAgua();
            }

            Console.WriteLine("Famílias: " + lista.Count);
            Console.WriteLine("Pessoas: " + pess);
            Console.WriteLine("Água necessária: " + agua + "L/dia");
        }
    }

    class Program
    {
        
        static Gestor g = new Gestor();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════╗");
                Console.WriteLine("║   SISTEMA ÁGUA - PROVÍNCIA UÍGE       ║");
                Console.WriteLine("╠═══════════════════════════════════════╣");
                Console.WriteLine("║ 1 - Cadastrar                         ║");
                Console.WriteLine("║ 2 - Listar todos                      ║");
                Console.WriteLine("║ 3 - Buscar por ID                     ║");
                Console.WriteLine("║ 4 - Relatório geral                   ║");
                Console.WriteLine("║ 5 - Relatório por município           ║");
                Console.WriteLine("║ 0 - Sair                              ║");
                Console.WriteLine("╚═══════════════════════════════════════╝");
                Console.Write("\nOpção: ");

                string op = Console.ReadLine();

                if (op == "1") Cadastrar();
                else if (op == "2") g.Listar();
                else if (op == "3") Buscar();
                else if (op == "4") g.RelatorioTotal();
                else if (op == "5") RelatMun();
                else if (op == "0") break;
                else Console.WriteLine("\nOpção inválida!");

                Console.WriteLine("\nEnter para continuar...");
                Console.ReadKey();
            }
        }

        static void Cadastrar()
        {
            Console.Clear();
            Console.WriteLine("\n=== NOVO CADASTRO ===\n");

        
            Beneficiario b = new Beneficiario();

            Console.Write("Nome: ");
            b.Nome = Console.ReadLine();

            Console.Write("BI: ");
            b.BI = Console.ReadLine();

            Console.Write("Município: ");
            b.Municipio = Console.ReadLine();

            Console.Write("Bairro: ");
            b.Bairro = Console.ReadLine();

            Console.Write("Familiares: ");
            b.NumeroFamiliares = int.Parse(Console.ReadLine());

            g.Adicionar(b);
            Console.WriteLine("Água diária: " + b.CalcularAgua() + "L");
        }

        static void Buscar()
        {
            Console.Write("\nID: ");
            int id = int.Parse(Console.ReadLine());
            Beneficiario b = g.Buscar(id);

            if (b != null)
                Console.WriteLine("\n" + b.ObterDados()); // 
            else
                Console.WriteLine("\nNão encontrado!");
        }

        static void RelatMun()
        {
            Console.Write("\nMunicípio: ");
            g.RelatorioCidade(Console.ReadLine());
        }
    }
}

