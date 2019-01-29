using System;
using System.Linq;
using System.Text;
public class JobLogger
{
    private static bool _logToFile;
    private static bool _logToConsole;
    private static bool _logMessage;
    private static bool _logWarning;
    private static bool _logError;    
    private static bool _logToDatabase; /*cambié el nombre de esta variable de LogToConsole a _logToConsole*/
    // private bool _initialized; /*variable comentada porque no se usa*/
    public JobLogger(bool logToFile, bool logToConsole, bool logToDatabase, bool logMessage, bool logWarning, bool logError)
    {
        _logError = logError;
        _logMessage = logMessage;
        _logWarning = logWarning;
        _logToDatabase = logToDatabase;
        _logToFile = logToFile;
        _logToConsole = logToConsole;  /*cambié el nombre de esta variable de LogToConsole a _logToConsole*/
    }
    public static void LogMessage(string content, bool message, bool warning, bool error)
    {
        content.Trim();
        if (content == null || content.Length == 0)
        {
            return;
        }
        if (!_logToConsole && !_logToFile && !_logToDatabase)
        {
            throw new Exception("Invalid configuration");
        }

        // if ((!_logError && !_logMessage && !_logWarning) || (!message && !warning && !error)) /* esta de mas realizar la comprobacion de los logs porque se usa mas arriba*/
        if (!message && !warning && !error)
        {
            throw new Exception("Error or Warning or Message must be specified");
        }

        System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
        connection.Open();

        int t = 0; /*inicializo la variable t*/
        if (message && _logMessage)
        {
            t = 1;
        }
        if (error && _logError)
        {
            t = 2;
        }
        if (warning && _logWarning)
        {
            t = 3;
        }
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("Insert into Log Values('" + message + "', " + t.ToString() + ")");
        command.ExecuteNonQuery();

        string l = ""; /*incializo la variable l*/
        if (System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt")) /*Saqué la negación en el condicional que evalúa si existe el archivo log porque trataba de leer el contenido del archivo cuando no existe y eso no tiene sentido*/
        {
            l = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt");
        }

        if (error && _logError)
        {
            l = l + DateTime.Now.ToShortDateString() + message;
        }
        if (warning && _logWarning)
        {
            l = l + DateTime.Now.ToShortDateString() + message;
        }
        if (message && _logMessage)
        {
            l = l + DateTime.Now.ToShortDateString() + message;
        }

        /*El problema de usar este método para escribir texto en el archivo es que si el archivo ya existe, se sobreescribe y creo que la idea de un log no es sobreescribir los log anteriores*/
        /*En todo caso usaría FileStream para escribir sobre el archivo y no sobreescribirlo*/
        System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt", l);

        if (error && _logError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        if (warning && _logWarning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        if (message && _logMessage)
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.WriteLine(DateTime.Now.ToShortDateString() + message);
        connection.Close(); /*cierre de conexion a base de datos*/
    }
}