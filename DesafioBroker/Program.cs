/*
 * Requisitos
 * O objetivo do sistema é avisar, via e-mail, caso a cotação de um ativo da B3 caia mais do que certo nível, ou suba acima de outro.
 * O programa deve ser uma aplicação de console (não há necessidade de interface gráfica).
 * Ele deve ser chamado via linha de comando com 3 parâmetros.
 * 1.	O ativo a ser monitorado
 * 2.	O preço de referência para venda
 * 3.	O preço de referência para compra
 * Ex.
 * > stock-quote-alert.exe PETR4 22.67 22.59 
 * Ele deve ler de um arquivo de configuração com:
 * 1.	O e-mail de destino dos alertas
 * 2.	As configurações de acesso ao servidor de SMTP que irá enviar o e-mail
 * A escolha da API de cotação é livre.
 * O programa deve ficar continuamente monitorando a cotação do ativo enquanto estiver rodando.
 */

namespace DesafioBroker
{
    class Program
    {
        const string PROGRAM_EXECUTABLE_NAME = "DesafioBroker.exe";

        const string CONFIG_FILE_PATH = "Data";
        const string EMAIL_CONFIG_FILE_NAME = "EmailConfig.json";

        const int ARGS_LENGTH = 3;

        const int SUCCESS_EXIT_CODE = 0;
        const int INIT_FAIL_EXIT_CODE = 1;

        static void Main(string[] args)
        {
            // Incorrect args length, print usage and ends the program.
            if (args.Length != ARGS_LENGTH)
            {
                PrintUsage();
                Environment.Exit(SUCCESS_EXIT_CODE);
            }

            // Regular program execution.
            AssetConfig assetConfig;
            EmailConfig emailConfig;
            try
            {
                ProcessArgs(args, out assetConfig);
                LoadConfigs(out emailConfig);
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(INIT_FAIL_EXIT_CODE);
            }

        }

        static void PrintUsage()
        {
            Console.WriteLine($"Usage: {PROGRAM_EXECUTABLE_NAME} [ASSET] [MAX PRICE] [MIN PRICE]");
            Console.WriteLine();
            Console.WriteLine($" Example: {PROGRAM_EXECUTABLE_NAME} PETR4 22.67 22.59");
        }

        static void ProcessArgs(string[] args, out AssetConfig assetConfig)
        {
            assetConfig = new AssetConfig(args);
        }

        static void LoadConfigs(out EmailConfig emailConfig)
        {
            if (!Directory.Exists(CONFIG_FILE_PATH))
            {
                Directory.CreateDirectory(CONFIG_FILE_PATH);
            }

            string emailConfigPath = Path.Join(CONFIG_FILE_PATH, EMAIL_CONFIG_FILE_NAME);
            if (!File.Exists(emailConfigPath))
            {
                using (StreamWriter emailConfigFile = new StreamWriter(emailConfigPath))
                {
                    emailConfigFile.Write(EmailConfig.DEFAULT_CONFIG);
                }
            }

            emailConfig = new EmailConfig(emailConfigPath);
        }
    }
}