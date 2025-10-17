using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace ITSM_Apis.Conn
{
    public class CustomException
    {
        private readonly string logPathString = string.Empty;

        public CustomException()
        {
            var conexion = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            logPathString = conexion.GetSection("Settings:logPath").Value;
            logPathString = Path.Combine(Directory.GetCurrentDirectory(), logPathString);
        }

        public string LogPath() => logPathString;

        public class FileService
        {
            #region CADA SOLICITUD CREA .JSON
            public void CreaJsonFile(string json, string obj = "", string logID = "")
            {
                JsonNode data = JsonNode.Parse(json);
                string formatoFecha = DateTime.Now.ToString("yyMMdd_HHmmss");
                string nombre = string.Empty;

                if (obj != "") 
                {
                    obj = obj.ToUpper();

                    switch (obj)
                    {
                        case "ACCOUNT":
                            nombre = $"{char.ToUpper(obj.ToLower()[0]) + obj.ToLower().Substring(1)} {data["CustID"]?.GetValue<string>()}";
                            break;
                        case "LOCATION":
                            nombre = $"{char.ToUpper(obj.ToLower()[0]) + obj.ToLower().Substring(1)} {data["EX_IdSitio"]?.GetValue<string>()}";
                            break;
                        case "CI_SERVICE":
                            nombre = $"{char.ToUpper(obj.ToLower()[0]) + obj.ToLower().Substring(1)} {data["Name"]?.GetValue<string>()}";
                            break;
                        case "EX_OBJTIPOSCONTRATOS":
                            nombre = $"{char.ToUpper(obj.ToLower()[0]) + obj.ToLower().Substring(1)} {data["EX_TipoContrato"]?.GetValue<string>()}";
                            break;
                        case "CI_CONTRACT":
                            nombre = $"{char.ToUpper(obj.ToLower()[0]) + obj.ToLower().Substring(1)} {data["ivnt_InternalID"]?.GetValue<string>()}";
                            break;
                        case "CI":
                            nombre = $"{char.ToUpper(obj.ToLower()[0]) + obj.ToLower().Substring(1)} {data["ivnt_AssetFullType"]?.GetValue<string>()} {data["Name"]?.GetValue<string>().Replace(":", "_")}";
                            break;
                    }
                }

                // ASIGNA NOMBRE DE ARCHIVO 
                string nameFile = String.Concat(formatoFecha, " ", (logID != "") ? logID : nombre);

                // MANDA A LLAMAR FUNCION QUE CREA JSON
                SaveJson(JsonConvert.SerializeObject(JObject.Parse(json), Formatting.Indented), nameFile);
            }
            #endregion

            #region GUARDA Archivo CREAO
            public void SaveJson(string content, string nameFile)
            {
                string pathField = ( content.Contains("ISM_4") || content.Contains("ISM_5"))? "\\Errors" : "\\Processed";
                //Obtiene ubicación para guardar JSON
                string folderPath = new CustomException().LogPath() + pathField;

                //Si no existe ubicación la crea
                if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }

                //Crea ruta completa del archivo JSON
                string filePath = Path.Combine(folderPath, nameFile + ".json");

                //Crea archivo JSON
                File.WriteAllText(filePath, content);
            }
            #endregion
        }

        public class Logs
        {
            #region Escribe en Log de Eventos
            public void WriteLog(List<string> parameters = null, string strHeader = "", string strSubTitle = "")
            {
                string folderPath = new CustomException().LogPath() + "\\Events";
                if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }
                string logfile = Path.Combine(folderPath, "00 EventLog.txt");

                StringBuilder sb = new StringBuilder();

                sb.Append('*', 30).Append(" ").Append(DateTime.Now.ToString()).Append(" ").Append('*', 30).Append("\n");
                if (strHeader != "") sb.AppendLine(strHeader.ToUpper());
                if (strSubTitle != "") sb.AppendLine(strSubTitle);

                if (parameters != null)
                {
                    foreach (string p in parameters)
                    {
                        sb.AppendLine(p);
                    }
                }

                using (StreamWriter sw = new StreamWriter(logfile, true))
                {
                    sw.WriteLine(sb.ToString());
                    sw.Close();
                }
            }
            #endregion

            #region Escribe en Log de Errores
            public void WriteExc(Exception? exc = null, string source = "", List<string>? parameters = null, string logID = "")
            {
                string folderPath = new CustomException().LogPath() + "\\Errors";
                if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }
                string errorFile = Path.Combine(folderPath, "00 ErrorLog.txt");

                StringBuilder sb = new StringBuilder();

                sb.Append('*', 30).Append(" ").Append(DateTime.Now.ToString()).Append(" ").Append('*', 30).Append("\n");
                if (logID != "") sb.AppendLine($"ID LOG: {logID}\n");
                if (exc != null)
                {
                    if (exc.InnerException != null)
                    {
                        sb.AppendLine("Inner Exception Type: " + exc.InnerException.GetType().ToString());
                        sb.AppendLine("Inner Exception: " + exc.InnerException.Message);
                        sb.AppendLine("Inner Source: " + exc.InnerException.Source);
                        if (exc.InnerException.StackTrace != null)
                        {
                            sb.AppendLine("Inner Stack Trace: ");
                            sb.AppendLine(exc.InnerException.StackTrace);
                        }
                    }
                    sb.AppendLine("Exception Type: " + exc.GetType().ToString());
                    sb.AppendLine("Exception Msg: " + exc.Message);
                }

                if (source != "") sb.AppendLine("Source: " + source);

                if (parameters != null)
                {
                    foreach (string p in parameters)
                    {
                        sb.AppendLine(p);
                    }
                }

                if (exc != null)
                {
                    sb.AppendLine("Stack Trace: ");
                    if (exc.StackTrace != null)
                    {
                        sb.AppendLine(exc.StackTrace);
                    }
                }

                using ( StreamWriter sw = new StreamWriter(errorFile, true) )
                {
                    sw.WriteLine(sb.ToString());
                    sw.Close();
                }
            }
            #endregion
        }

        public class utcToDatetime
        {
            public static string castUtcDatetime(long seconds = 0, long nanoseconds = 0)
            {
                // Datos de la línea JSON
                //long seconds = 1727137428;
                //int nanoseconds = 557000000;

                // Convertir los segundos en un DateTime a partir del 1 de enero de 1970 (Unix Epoch)
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;

                // Agregar los nanosegundos como una fracción de segundo
                dateTime = dateTime.AddTicks(nanoseconds / 100); // 1 tick = 100 nanosegundos

                // Convertir a string (puedes cambiar el formato según lo que necesites)
                string formattedDate = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Imprimir la fecha formateada
                Console.WriteLine(formattedDate);
                return formattedDate;
            }
        }

    }
}
