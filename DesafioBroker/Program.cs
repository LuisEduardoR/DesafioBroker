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

using DesafioBroker.Config;
using DesafioBroker.Services;
using DesafioBroker.Services.QuoteTracker;

namespace DesafioBroker
{
    class Program
    {
        public const string PROGRAM_EXECUTABLE_NAME = "DesafioBroker.exe";

        public const string CONFIG_FILE_PATH = "Data";

        public const int SUCCESS_EXIT_CODE = 0;
        public const int CONFIG_LOAD_FAIL_EXIT_CODE = 1;
        public const int SERVICE_INIT_FAIL_EXIT_CODE = 2;
        public const int NOT_CONFIGURED_EXIT_CODE = 3;
        public const int RUNTIME_ERROR_CODE = 4;

        const int ARGS_LENGTH = 3;

        const int MAX_SEPARATOR_LENGTH = 80;

        static Semaphore ConsoleSemaphore;

        static Program()
        {
            ConsoleSemaphore = new Semaphore(0, 1);
        }

        static void Main(string[] args)
        {
            // Incorrect args length, print usage and ends the program.
            if (args.Length != 0 && args.Length != ARGS_LENGTH)
            {
                PrintUsage();
                Environment.Exit(SUCCESS_EXIT_CODE);
            }

            // Regular program execution.
            AssetConfig assetConfig = new AssetConfig();
            EmailConfig emailConfig = new EmailConfig();
            ApiConfig apiConfig = new ApiConfig();

            try
            {
                LoadConfig(args, assetConfig, emailConfig, apiConfig);

                try
                {
                    EmailService emailService = new EmailService(emailConfig);
                    QuoteTrackerService quoteTrackerService = new QuoteTrackerService(assetConfig, apiConfig, emailService);

                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine("Preparing to init services...");
                        Console.WriteLine();
                        Console.WriteLine("< Press ENTER at any time to finish execution >");
                        Console.WriteLine();

                        // NOTE: Use ThreadSafeWriteLine from now on.
                        ConsoleSemaphore.Release();

                        ThreadSafeWriteLine("Starting email service...");
                        Thread emailServiceThread = new Thread(emailService.Run);
                        emailServiceThread.Start();

                        ThreadSafeWriteLine("Starting tracker service...");
                        Thread quoteTrackerServiceThread = new Thread(quoteTrackerService.Run);
                        quoteTrackerServiceThread.Start();

                        Console.ReadLine();

                        emailService.Stop();
                        quoteTrackerService.Stop();

                        emailServiceThread.Join();
                        quoteTrackerServiceThread.Join();
                    }
                    catch (Exception runtimeException)
                    {
                        Console.WriteLine($"Error: {runtimeException.Message}");
                        Environment.Exit(RUNTIME_ERROR_CODE);
                    }
                }
                catch (Exception serviceInitException)
                {
                    Console.WriteLine($"Error: {serviceInitException.Message}");
                    Environment.Exit(SERVICE_INIT_FAIL_EXIT_CODE);
                }
            }
            catch (Exception configLoadException)
            {
                Console.WriteLine($"Error:\n{configLoadException.Message}");
                Environment.Exit(CONFIG_LOAD_FAIL_EXIT_CODE);
            }
        }

        static void LoadConfig(string[] args, AssetConfig assetConfig, EmailConfig emailConfig, ApiConfig apiConfig)
        {
            if (!Directory.Exists(CONFIG_FILE_PATH))
            {
                CreateConfigDirectory(assetConfig, emailConfig, apiConfig);
            }

            if (args.Length == ARGS_LENGTH)
            {
                Console.WriteLine("Loading config from program arguments...");
                assetConfig.LoadFromArgs(args);
                Console.WriteLine($"Updating {assetConfig.GetFullPath()}...");
                assetConfig.Save();
                Console.WriteLine("DONE");
            }
            else
            {
                Console.WriteLine($"Loading {assetConfig.GetFullPath()}...");
                assetConfig.Load();
                Console.WriteLine("DONE");
            }

            Console.WriteLine($"Loading {emailConfig.GetFullPath()}...");
            emailConfig.Load();
            Console.WriteLine("DONE");

            Console.WriteLine($"Loading {apiConfig.GetFullPath()}...");
            apiConfig.Load();
            Console.WriteLine("DONE");
        }

        static void CreateConfigDirectory(AssetConfig assetConfig, EmailConfig emailConfig, ApiConfig apiConfig)
        {
            Console.WriteLine($"Creating config directory in {CONFIG_FILE_PATH}...");
            Directory.CreateDirectory(CONFIG_FILE_PATH);
            Console.WriteLine("DONE");
            Console.WriteLine();

            Console.WriteLine($"Creating default config files...");
            assetConfig.Save();
            emailConfig.Save();
            apiConfig.Save();
            Console.WriteLine("DONE");
            Console.WriteLine();

            Console.WriteLine($"Please edit the config files on {CONFIG_FILE_PATH} and them execute {PROGRAM_EXECUTABLE_NAME} again!");
            Environment.Exit(NOT_CONFIGURED_EXIT_CODE);
        }

        static void PrintUsage()
        {
            Console.WriteLine($"Usage: {PROGRAM_EXECUTABLE_NAME} [ASSET] [MAX PRICE] [MIN PRICE]");
            Console.WriteLine();
            Console.WriteLine($" Example: {PROGRAM_EXECUTABLE_NAME} PETR4 22.67 22.59");
        }

        public static void ThreadSafeWriteLine(string line)
        {
            ConsoleSemaphore.WaitOne();
            Console.WriteLine(line);
            ConsoleSemaphore.Release();
        }

        public static void ThreadSafeWriteLines(string[] lines)
        {
            ConsoleSemaphore.WaitOne();
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
            ConsoleSemaphore.Release();
        }

        public static void WriteServiceMessage(string serviceName, string message)
        {
            string messageTitle = $"# {serviceName} service message:";
            string messageBody = $"# {message}";
            string separator = new String('#', Math.Min(Math.Max(messageTitle.Length, messageBody.Length), MAX_SEPARATOR_LENGTH));
            ThreadSafeWriteLines(new string[] { "", separator, messageTitle, messageBody, separator });
        }

        public static void WriteServiceMessages(string serviceName, string[] messages)
        {
            int maxMessageLength = 0;

            string messageTitle = $"# {serviceName} service message:";

            List<string> messagesBody = new List<string>() { "", messageTitle };

            foreach (string message in messages)
            {
                messagesBody.Add($"# {message}");
            }

            foreach (string message in messagesBody)
            {
                if (message.Length > maxMessageLength)
                {
                    maxMessageLength = Math.Min(message.Length, MAX_SEPARATOR_LENGTH);
                }
            }

            string messageSeparator = new String('#', maxMessageLength);

            messagesBody.Insert(1, messageSeparator);
            messagesBody.Add(messageSeparator);
            ThreadSafeWriteLines(messagesBody.ToArray());
        }

    }
}