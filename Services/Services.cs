using ITSM_Apis.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using static ITSM_Apis.Conn.CustomException;

namespace ITSM_Apis.Services
{
    public class Services : IServices
    {
        #region Declaración de Variables

        private static string? _url;
        private static string? _apiKey;

        private static string? _pathUrlBizObj;

        private static List<string> _lstLogEvent = [];

        #endregion

        public Services()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            _url = Boolean.Parse(builder.GetSection("Settings:EnableDev").Value!)
                ? builder.GetSection("ITSM_Connect_Dev:url").Value
                : builder.GetSection("ITSM_Connect_Pro:url").Value;

            _apiKey = Boolean.Parse(builder.GetSection("Settings:EnableDev").Value!)
                ? builder.GetSection("ITSM_Connect_Dev:apikey").Value
                : builder.GetSection("ITSM_Connect_Pro:apikey").Value;
            
            _pathUrlBizObj = builder.GetSection("Settings:PathUrlBizObj").Value;
        }

        private HttpClient CreateHttpClient()
        {
            HttpClient _client = new HttpClient();

            _client.DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");

            _client.BaseAddress = new Uri( _url! );

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _apiKey );

            return _client;
        }

        #region Genera NewGuid
        public async Task<string> GetNewGuid() => Guid.NewGuid().ToString("N").ToUpper();
        #endregion

        #region Object
        public async Task<object> GetObject(string objeto)
        {
            try
            {
                objeto = await nameObject(objeto);
                HttpClient client = CreateHttpClient();
                var response = await client.GetAsync($"{_pathUrlBizObj}/{objeto}s");

                string content = await response.Content.ReadAsStringAsync();
                object respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto);
                }
                else
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto);
                }

                return respActivity;

            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex);
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Object by ID
        public async Task<object> GetObjectByRecId(string objeto, string recId, string logID = "")
        {
            try
            {
                objeto = await nameObject(objeto);
                HttpClient client = CreateHttpClient();
                var response = await client.GetAsync($"{_pathUrlBizObj}/{objeto}s('{recId}')");

                string content = await response.Content.ReadAsStringAsync();
                object respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                else
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }

                return respActivity;

            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: logID);
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Object Delete
        public async Task<object> DeleteObject(string objeto, string recId, string logID = "")
        {
            try
            {
                objeto = await nameObject(objeto);
                HttpClient client = CreateHttpClient();

                var response = await client.DeleteAsync($"{_pathUrlBizObj}/{objeto}s('{recId}')");
                string content = await response.Content.ReadAsStringAsync();
                var respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {
                    //new FileService().CreaJsonFile(respFil.ToString(), objeto, logID);
                    return JObject.Parse(@"{
                          ""code"": ""200"",
                          ""message"": ""Registro Eliminado""
                        }");
                }
                else
                {
                    //new FileService().CreaJsonFile(respFil.ToString(), objeto, logID);
                    return respActivity;
                }


            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: logID);
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Object Filter
        public async Task<object> GetObjectFilter(string objeto, string filtro, string select, string logID = "") //, object objFilter
        {
            try
            {
                objeto = await nameObject(objeto);
                HttpClient client = CreateHttpClient();
                var response = await client.GetAsync($"{_pathUrlBizObj}/{objeto}s?$filter={filtro}&$Select={select}");

                string content = await response.Content.ReadAsStringAsync();
                object? respActivity = JsonConvert.DeserializeObject(content);

                respActivity = (respActivity == null)
                    ? JsonConvert.DeserializeObject("{\"value\":[{\"logID\": \"" + logID + "\", \"value\": null }]}")
                    : respActivity;

                if (response.IsSuccessStatusCode)
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                else
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }

                return respActivity;
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: logID);
                return new { ExMessage = ex.Message };
            }
        }
        #endregion

        #region Object Relationships
        public async Task<object> GetObjectRelationships(string objeto, string recId, string relacion, string logID = "")
        {
            try
            {
                objeto = await nameObject(objeto);
                HttpClient client = CreateHttpClient();
                var response = await client.GetAsync($"{_pathUrlBizObj}/{objeto}s('{recId}')/{relacion}");

                string content = await response.Content.ReadAsStringAsync();
                object respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                else
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }

                return respActivity;
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: logID);
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Delete Object Relationships
        public async Task<object> deleteObjectRelationships(string objeto, string recIdObj, string relation, string recIdRelation, string logID = "")
        {
            try
            {
                objeto = await nameObject(objeto);
                HttpClient client = CreateHttpClient();
                var response = await client.DeleteAsync($"{_pathUrlBizObj}/{objeto}s('{recIdObj}')/{relation}('{recIdRelation}')/$Ref");
                string content = await response.Content.ReadAsStringAsync();
                object respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                else
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                return respActivity;
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: logID);
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Create Object Relationships
        public async Task<object> patchObjectRelationships(string objeto, string recIdObj, string relation, string recIdRelation, string logID = "")
        {
            try
            {
                objeto = await nameObject(objeto);
                HttpClient client = CreateHttpClient();
                var response = await client.PatchAsync($"{_pathUrlBizObj}/{objeto}s('{recIdObj}')/{relation}('{recIdRelation}')/$Ref", null);
                string content = await response.Content.ReadAsStringAsync();
                object respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                else
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                return respActivity;
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: logID);
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Update Object
        public async Task<object> updateObjectByRecId(string objeto, string recId, Object body)
        {
            objeto = await nameObject(objeto);
            if (_lstLogEvent.Count == 0)
            {
                CreateLogEvents(objeto, "", JObject.Parse(body.ToString()), "", "", "", "ACTUALIZA", "", "updateObjectByRecId");
            }

            string logID = _lstLogEvent[0].ToString().Replace("ID LOG: ", "");

            try
            {
                Console.WriteLine(body);
                HttpClient client = CreateHttpClient();
                var response = await client.PutAsJsonAsync($"{_pathUrlBizObj}/{objeto}s('{recId}')", body);

                string content = await response.Content.ReadAsStringAsync();
                object respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                else
                {
                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, logID);
                }
                return respActivity;
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: logID);
                ClearLogEvents();
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Create Object
        public async Task<object> createObject(string objeto, Object body, NotesRequest modelNote = null)
        {
            if ( _lstLogEvent.Count == 0 ) 
            { 
                CreateLogEvents(objeto, "", JObject.Parse(body.ToString()), "", "", "", "CARGA", "", "createObject"); 
            }

            #region Declara variables locales            
            JsonNode? json = JsonNode.Parse(body.ToString());

            if (modelNote is null) { modelNote = new NotesRequest() { Subject = "*** API REST ***", Note = "Objeto creado por API REST" }; }

            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(body.ToString());
            List<string> lstUp = dict.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToList();

            object respActivity;
            string content = string.Empty;
            string statusCode;
            string recId;
            string idlog = _lstLogEvent[0].ToString().Replace("ID LOG: ", "");

            string? ex_CustID_Link = json["EX_CustID_Link"]?.GetValue<string>();
            string? ivnt_AssetLocation = json["ivnt_AssetLocation"]?.GetValue<string>();
            string? recId_Contract = json["EX_ParentLink"]?.GetValue<string>();
            string? recId_Service = json["EX_Servicio_Link"]?.GetValue<string>();
            string? recId_TipoContrato = json["EX_TipoContrato_Link"]?.GetValue<string>();
            #endregion

            try
            {
                #region Search recId Customer
                if (objeto == "CI".ToUpper() || objeto == "Incident".ToUpper() || objeto == "Location".ToUpper() || objeto == "Ci__Contract".ToUpper())
                {
                    if (!String.IsNullOrEmpty(ex_CustID_Link))
                    {

                        object? respAccount = await GetObjectFilter("Account", $"CustId eq '{ex_CustID_Link}'", "recId", idlog);

                        if (respAccount != null)
                        {
                            JObject contAccount = JObject.FromObject(respAccount); //JObject.Parse(respAccount.ToString());
                            ex_CustID_Link = contAccount["value"][0]["RecId"]?.ToString();
                            json["EX_CustID_Link"] = ex_CustID_Link;

                            _lstLogEvent.Add($"\t---> EX_CustID_Link: {ex_CustID_Link}");
                        }
                        else
                        {
                            _lstLogEvent.Add($"\t---> CustId {ex_CustID_Link} - No se encontró recId para variable 'EX_CustID_Link'");
                            _lstLogEvent.Add("\n<--- FINALIZA CREACIÓN");
                            new Logs().WriteLog(_lstLogEvent);
                            ClearLogEvents();
                            return respActivity = new { Message = $"CustId {ex_CustID_Link} - No se encontró recId para variable 'EX_CustID_Link'" };
                        }
                    }
                }
                #endregion

                #region Search recId Location
                if (objeto == "CI".ToUpper() || objeto == "Incident".ToUpper() || objeto == "Ci__Contract".ToUpper())
                {
                    if (!String.IsNullOrEmpty(ivnt_AssetLocation))
                    {
                        object? respLocation = await GetObjectFilter("Location", $"EX_IdSitio eq '{ivnt_AssetLocation}' AND EX_CustID_Link_RecID eq '{ex_CustID_Link}'", "recId", idlog);
                        if (respLocation != null)
                        {
                            JObject contLocation = JObject.FromObject(respLocation); //JObject.Parse(respLocation.ToString());
                            ivnt_AssetLocation = contLocation["value"][0]["RecId"]?.ToString();
                            json["ivnt_AssetLocation"] = ivnt_AssetLocation;

                            _lstLogEvent.Add($"\t---> ivnt_AssetLocation: {ivnt_AssetLocation}");
                        }
                        else
                        {
                            _lstLogEvent.Add($"\t---> IdSitio {ivnt_AssetLocation} - No se encontró recId para variable 'ivnt_AssetLocation'");
                            _lstLogEvent.Add("\n<--- FINALIZA CREACIÓN");
                            new Logs().WriteLog(_lstLogEvent);
                            ClearLogEvents();
                            return respActivity = new { Message = $"IdSitio {ivnt_AssetLocation} - No se encontró recId para variable 'ivnt_AssetLocation'" };
                        }
                    }
                }
                #endregion

                #region Search recId Contract
                if (objeto == "CI".ToUpper())
                {
                    if (!String.IsNullOrEmpty(recId_Contract))
                    {
                        object? respContrato = await GetObjectFilter("CI__Contract", $"ivnt_InternalID eq '{recId_Contract}'", "recId", idlog);
                        if (respContrato != null)
                        {
                            JObject contContrato = JObject.FromObject(respContrato); //JObject.Parse(respContrato.ToString());
                            recId_Contract = contContrato["value"][0]["RecId"]?.ToString();
                            json["EX_ParentLink"] = recId_Contract;

                            _lstLogEvent.Add($"\t---> EX_ParentLink: {recId_Contract}");
                        }
                        else
                        {
                            _lstLogEvent.Add($"\t---> Contrato {recId_Contract} - No se encontró recId para variable 'EX_ParentLink'");
                            _lstLogEvent.Add("\n<--- FINALIZA CREACIÓN");
                            new Logs().WriteLog(_lstLogEvent);
                            ClearLogEvents();
                            return respActivity = new { Message = $"Contrato {recId_Contract} - No se encontró recId para variable 'EX_ParentLink'" };
                        }
                    }
                }
                #endregion

                #region Search recId Servicio
                if (objeto == "EX_ObjTiposContratos".ToUpper())
                {
                    if (!String.IsNullOrEmpty(recId_Service))
                    {
                        object? respService = await GetObjectFilter("CI__Service", $"Name eq '{recId_Service}'", "recId", idlog);
                        if (respService != null)
                        {
                            JObject contService = JObject.FromObject(respService); //JObject.Parse(respContrato.ToString());
                            recId_Service = contService["value"][0]["RecId"]?.ToString();
                            json["EX_Servicio_Link"] = recId_Service;

                            _lstLogEvent.Add($"\t---> EX_Servicio_Link: {recId_Service}");
                        }
                        else
                        {
                            _lstLogEvent.Add($"\t---> Servicio {recId_Service} - No se encontró recId para variable 'EX_Servicio_Link'");
                            _lstLogEvent.Add("\n<--- FINALIZA CREACIÓN");
                            new Logs().WriteLog(_lstLogEvent);
                            ClearLogEvents();
                            return respActivity = new { Message = $"Servicio {recId_Service} - No se encontró recId para variable 'EX_Servicio_Link'" };
                        }
                    }
                }
                #endregion

                #region Search recId TipoContrato
                if (objeto == "Ci__Contract".ToUpper())
                {
                    if (!String.IsNullOrEmpty(recId_TipoContrato))
                    {
                        object? respTipoContrato = await GetObjectFilter("EX_ObjTiposContratos", $"EX_TipoContrato eq '{recId_TipoContrato}'", "recId");
                        if (respTipoContrato != null)
                        {
                            JObject? contTipoContrato = JObject.FromObject(respTipoContrato); //JObject.Parse(respContrato.ToString());
                            recId_TipoContrato = contTipoContrato["value"][0]["RecId"]?.ToString();
                            json["EX_TipoContrato_Link"] = recId_TipoContrato;

                            _lstLogEvent.Add($"\t---> EX_TipoContrato_Link: {recId_TipoContrato}");
                        }
                        else
                        {
                            _lstLogEvent.Add($"\t---> Tipo Contrato {recId_TipoContrato} - No se encontró recId para variable 'EX_TipoContrato_Link'");
                            _lstLogEvent.Add("\n<--- FINALIZA CREACIÓN");
                            new Logs().WriteLog(_lstLogEvent);
                            ClearLogEvents();
                            return respActivity = new { Message = $"Tipo Contrato {recId_TipoContrato} - No se encontró recId para variable 'EX_TipoContrato_Link'" };
                        }
                    }
                }
                #endregion

                #region Crea Nuevo Objeto en invanti
                var client = CreateHttpClient();
                objeto = await nameObject(objeto);
                var response = await client.PostAsJsonAsync($"{_pathUrlBizObj}/{objeto}s", json);
                statusCode = $"Status: {(int)response.StatusCode}";
                content = await response.Content.ReadAsStringAsync();

                respActivity = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode)
                {

                    #region Obtiene Datos Generales de Objeto Creado
                    JObject recIdObj = JObject.FromObject(respActivity);
                    recId = recIdObj["RecId"]?.ToString();
                    #endregion

                    _lstLogEvent.Add($"\n\t*** RESPUESTA ***");
                    _lstLogEvent.Add($"\t{statusCode}");
                    _lstLogEvent.Add($"\tObjeto {char.ToUpper(objeto.ToLower()[0]) + objeto.ToLower().Substring(1)} Creado, RecId: {recId}");

                    modelNote.Object = objeto;
                    modelNote.Note = modelNote.Note + "\n\n" + json + "\n";

                    await addNoteObject("Journal__Notes", modelNote, recId);
                }
                else
                {
                    JsonNode? msgJson = JsonNode.Parse(content.ToString());
                    var msg = msgJson["message"][0]?.GetValue<string>();

                    _lstLogEvent.Add($"\n\t*** RESPUESTA ***");
                    _lstLogEvent.Add($"\tStatus: {(int)response.StatusCode}");
                    _lstLogEvent.Add($"\tMessage: {msg}");

                    _lstLogEvent.Add("\n<--- FINALIZA CREACIÓN");
                    new Logs().WriteLog(_lstLogEvent);
                    ClearLogEvents();

                    new FileService().CreaJsonFile(respActivity.ToString(), objeto, idlog);
                    return respActivity = JsonConvert.DeserializeObject(content);
                }
                #endregion

                #region CI - (LINK) Relaciona Contrato a CI
                if (response.IsSuccessStatusCode && objeto == "CI".ToUpper() && String.IsNullOrEmpty(recId_TipoContrato))
                {
                    object? respPatch = await patchObjectRelationships(objeto, recId, "EX_CIAssocCIContract2", recId_Contract);

                    JObject contLink = JObject.FromObject(respPatch); //JObject.Parse(respContrato.ToString()); 
                    var codeLink = contLink["code"]?.ToString();
                    var descLink = contLink["description"]?.ToString();
                    var msgLink = contLink["message"][0]?.ToString();

                    _lstLogEvent.Add($"\n\t***  LINK  ***");
                    _lstLogEvent.Add($"\tStatus: {codeLink}");
                    _lstLogEvent.Add($"\tDescripción: {descLink}");
                    _lstLogEvent.Add($"\tMessage: {msgLink}");

                }
                #endregion

                _lstLogEvent.Add("\n<--- FINALIZA CREACIÓN");
                new Logs().WriteLog(_lstLogEvent);
                ClearLogEvents();

                return respActivity;
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: idlog);
                return new { message = ex.Message };
            }
        }
        #endregion

        #region Add Note Object
        public async Task<Object> addNoteObject(string objNote, NotesRequest model, string recId = "")
        {
            #region INICIA LOGS DE EVENTOS si no hay uno
            if (_lstLogEvent.Count == 0)
            {
                CreateLogEvents(objNote, "", null, "", "", "", "AGREGAR NOTA", "", "AddNoteObject");
            }
            else
            {
                _lstLogEvent.Add($"\n   *** INSERTA NOTA ***\n");
            }
            #endregion

            object respActivity = null;
            string idlog = _lstLogEvent[0].ToString().Replace("ID LOG: ", "");
            string parentLink = string.Empty;
            var obj = await nameObject(model.Object);

            var json = JsonNode.Parse(@"{   
                        ""ParentLink"": """",
                        ""JournalType"": ""Notes"",
                        ""Category"": ""Memo"",
                        ""Source"": ""Other"",
                        ""Subject"": """",
                        ""NotesBody"": """",
                        ""DisplayText"": """",
                        ""OwnerTeam"":""Network Support"",
                        ""CreatedBy"":""ATG"",
                        ""LastModBy"":""ATG""
                    }");

            json["Subject"] = model.Subject;
            json["NotesBody"] = model.Note;
            json["DisplayText"] = model.Note;

            var (fieldName, fieldValue) = FieldName(model.Object.ToUpper(), model.ObjectNumber);
            object? getRecIdObj = null;
            JObject contRecIdObj = null;

            if (recId == "")
            {
                getRecIdObj = await GetObjectFilter(obj, $"{fieldName} eq {fieldValue}", "recId", idlog);
                contRecIdObj = JObject.FromObject(getRecIdObj);
            }

            if (getRecIdObj != null || recId != "")
            {

                parentLink = (recId != "") ? recId : contRecIdObj["value"][0]["RecId"]?.ToString();
                json["ParentLink"] = parentLink;

                _lstLogEvent.Add(json + "\n");
                _lstLogEvent.Add($"\t---> {obj} {fieldName}: {fieldValue} --> ParentLink: {parentLink}");

                #region Inserta Nota en Objeto
                var client = CreateHttpClient();
                var response = await client.PostAsJsonAsync($"{_pathUrlBizObj}/{objNote}s", json);

                var statusCode = $"Status: {(int)response.StatusCode}";
                var content = await response.Content.ReadAsStringAsync();

                respActivity = JsonConvert.DeserializeObject(content);
                #endregion

                if (response.IsSuccessStatusCode)
                {
                    #region Obtiene Datos Generales de Objeto Creado
                    JObject recIdObj = JObject.FromObject(respActivity);
                    var recIdObjNote = recIdObj["RecId"]?.ToString();
                    #endregion

                    _lstLogEvent.Add($"\n\t*** RESPUESTA ***");
                    _lstLogEvent.Add($"\t{statusCode}");
                    _lstLogEvent.Add($"\tNota Creada, {objNote} RecId: {recIdObjNote}");
                }
                else
                {
                    JsonNode? msgJson = JsonNode.Parse(content.ToString());
                    var msg = msgJson["message"][0]?.GetValue<string>();

                    _lstLogEvent.Add($"\n\t*** RESPUESTA ***");
                    _lstLogEvent.Add($"\tStatus: {(int)response.StatusCode}");
                    _lstLogEvent.Add($"\tMessage: {msg}");

                    _lstLogEvent.Add("\n<--- FINALIZA INSERCIÓN DE NOTA");
                    new Logs().WriteLog(_lstLogEvent);
                    ClearLogEvents();

                    new FileService().CreaJsonFile(respActivity.ToString(), "Journal Notes", idlog);
                    return respActivity = JsonConvert.DeserializeObject(content);
                }
            }
            else
            {
                _lstLogEvent.Add($"\t---> {fieldName} {fieldValue} - No se encontró recId para variable 'ParentLink'");
                _lstLogEvent.Add("\n<--- FINALIZA INSERCIÓN DE NOTA");
                new Logs().WriteLog(_lstLogEvent);
                ClearLogEvents();
                return respActivity = new { Message = $"{fieldName} {fieldValue} - No se encontró recId para variable 'ParentLink'" };
            }

            if (recId == "")
            {
                _lstLogEvent.Add("\n<--- FINALIZA INSERCIÓN DE NOTA");
                new Logs().WriteLog(_lstLogEvent);
                ClearLogEvents();
            }
            else
            {
                _lstLogEvent.Add("\n   *** NOTA INSERTADA ***\n");
            }

            return respActivity;
        }

        #endregion

        #region Funtions
        public async Task<string> nameObject(string objectType)
        {
            return objectType.ToUpper() switch
            {
                "CONTRACT"        => "CI__CONTRACT",
                "CONTRATO"        => "CI__CONTRACT",
                "CI CONTRACT"     => "CI__CONTRACT",
                "CICONTRACT"      => "CI__CONTRACT",
                "KNOWLEDGE"       => "FRS_KNOWLEDGE",
                "PROJECT"         => "FRS_PROJECT",
                "RELEASE"         => "RELEASEPROJECT",
                "MILESTONE"       => "RELEASEMILESTONE",
                "SERVICE REQUEST" => "SERVICEREQ",
                "SERVICEREQUEST"  => "SERVICEREQ",
                "SERVICE REQ"     => "SERVICEREQ",
                "SERVICEREQ"      => "SERVICEREQ",
                "INC"             => "INCIDENT",
                "INCIDENT"        => "INCIDENT",
                "INCIDENTE"       => "INCIDENT",
                "TASK"            => "TASK__ASSIGNMENT",
                "TAREA"           => "TASK__ASSIGNMENT",
                "TAREA EXTERNA"   => "TASK__WORKORDER",
                "TAREAEXTERNA"    => "TASK__WORKORDER",
                "EXTERNAL TASK"   => "TASK__WORKORDER",
                "SERVICE"         => "CI__SERVICE",
                "SERVICIO"        => "CI__SERVICE",
                "CI SERVICE"      => "CI__SERVICE",
                "CISERVICE"       => "CI__SERVICE",
                "RETIRED"         => "CI",
                _ => objectType.ToUpper()
            };
        }

        public (string fieldName, string fieldValue) FieldName(string obj, string numberObject)
        {
            string fieldName = string.Empty;
            string fieldValue = string.Empty;

            switch (obj)
            {
                case "CHANGE":
                    {
                        fieldName = "ChangeNumber";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "CI":
                    {
                        fieldName = "SerialNumber";
                        fieldValue = $"'{numberObject}'";
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "CONTRACT":
                    {
                        fieldName = "ivnt_InternalID";
                        fieldValue = $"'{numberObject}'";
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "INCIDENT":
                    {
                        fieldName = "IncidentNumber";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "KNOWLEDGE":
                    {
                        fieldName = "KnowledgeNumber";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "PROBLEM":
                    {
                        fieldName = "ProblemNumber";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "PROJECT":
                    {
                        fieldName = "ProjectNumber";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "MILESTONE":
                case "RELEASE":
                    {
                        fieldName = "ReleaseNumber";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "SERVICEREQ":
                    {
                        fieldName = "ServiceReqNumber";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "TASK":
                    {
                        fieldName = "AssignmentID";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
                case "EXTERNAL TASK":
                    {
                        fieldName = "WorkOrderID";
                        fieldValue = numberObject;
                        return (fieldName.ToUpper(), fieldValue);
                    }
            }
            return (fieldName.ToUpper(), fieldValue);
        }

        private (string recId, string program, string dateTime, string user) ExtractMetadata(JObject item)
        {
            return (
                item["RecId"]?.ToString() ?? string.Empty,
                item["Program"]?.ToString() ?? string.Empty,
                item["DateTime"]?.ToString() ?? string.Empty,
                item["User"]?.ToString() ?? string.Empty
            );
        }

        private static void RemoveMetadataFields(JObject item, string fnc = "")
        {
            if (fnc == "SRtoRelease")
            {
                item.Remove("CustID");
                item.Remove("IDSitio");
                item.Remove("CI");
                item.Remove("MILESTONE");
                item.Remove("CONTRACT");
                item.Remove("LOCATION");
                item.Remove("SrNumber");
            }
            else
            {
                item.Remove("RecId");
                item.Remove("Program");
                item.Remove("DateTime");
                item.Remove("User");
            }
        }

        private NotesRequest CreateModelNote(string program = "", string dateTime = "", string user = "", string logType = "")
        {

            return new NotesRequest
            {
                Object = "",
                ObjectNumber = "",
                Subject = $"*** DATOS ERP {logType} ***",
                Note = string.Join("\n", new List<string>
                    {
                        $"*** DATOS ERP {logType} ***",
                        $"PROGRAMA: {program}",
                        $"FECHA: {dateTime}",
                        $"USUARIO: {user}"
                    })
            };
        }

        private async Task CreateLogEvents(string objectType = "", string recId = "", JObject? item = null, string program = "", string dateTime = "", string user = "", string logType = "", string extra = "", string fnc = "")
        {
            var displayName = objectType.ToUpper(); 
            string logID = await GetNewGuid();
            _lstLogEvent ??= new List<string>();

            var loglist = new List<string> {
                        $"ID LOG: {logID}",
                        $"PROCESO DE {logType} DE OBJETO {displayName} EN IVANTI \"**{fnc}**\"\n",
                        $"INICIA {logType} {extra} --->\n",
                    };

            if (new[] { "ACTUALIZACIÓN", "CREACIÓN", "IMPORTACIÓN" }.Contains(logType) )
            {
                loglist.Add($"RecId {displayName}: {recId}");
                loglist.Add($"{item}");
            } else
            {
                loglist.Add($"{item}");
            }

            if (program != "")
            {
                loglist.Add($"\n\t*** DATOS ERP {logType} ***");
                loglist.Add($"\tPROGRAMA: {program}");
                loglist.Add($"\tFECHA: {dateTime}");
                loglist.Add($"\tUSUARIO: {user} \n");
            }

            _lstLogEvent.AddRange(loglist);
        }

        private static void ClearLogEvents()
        {
            _lstLogEvent.Clear();
        }

        #endregion

        #region UPDATE Object List
        public async Task<object> UpdateObjectLst(Object body)
        {
            //string logID = string.Empty;
            string objName = string.Empty;

            int x = 0;
            List<Dictionary<string, JObject>> objResp = new List<Dictionary<string, JObject>>();


            // Convertir a string JSON
            var jsonString = body.ToString();
            var jsonBody = JObject.Parse(jsonString);

            JArray dataArray = (JArray)jsonBody["data"];
            try
            {
                foreach (JObject objItem in dataArray)
                {
                    foreach (var property in objItem.Properties())
                    {
                        int i = 0;
                        objName = await nameObject(property.Name);

                        JArray itemsArray = property.Value as JArray;

                        foreach (JObject item in itemsArray)
                        {
                            //logID = await GetNewGuid();
                            int itemNumber = ++i;

                            var (recIdItem, ProgramItemERP, DateTimeItemERP, UserItemERP) = ExtractMetadata(item);
                            RemoveMetadataFields(item);

                            var modelNote = CreateModelNote(ProgramItemERP, DateTimeItemERP, UserItemERP, "ACTUALIZACIÓN");
                            CreateLogEvents(objName, recIdItem, item, ProgramItemERP, DateTimeItemERP, UserItemERP, "ACTUALIZACIÓN", $"{itemNumber} de {itemsArray.Count}", "UpdateObjectList");
                            
                            // - ELIAS: REGRESA LA PROPIEDAD DEL OBJETO AL ITEM
                            using JsonDocument doc = JsonDocument.Parse(item.ToString());
                            var newItem = doc.RootElement;

                            var updObject = await updateObjectByRecId(objName, recIdItem, newItem);

                            objResp.Add(new Dictionary<string, JObject>
                            {
                                [objName] = JObject.FromObject(updObject)
                            });

                            if (JsonNode.Parse(updObject.ToString())["RecId"]?.GetValue<string>() != "")
                            {
                                _lstLogEvent.Add($"\n\tMessage {objName}: Actualizado satisfactoriamente");
                                var recId = JsonNode.Parse(updObject.ToString())["RecId"]?.GetValue<string>();
                                modelNote.Object = objName;
                                modelNote.Note = modelNote.Note + "\n\n" + newItem + "\n";

                                await addNoteObject("Journal__Notes", modelNote, recId);
                            }
                            else
                            {
                                _lstLogEvent.Add($"\n\tMessage {objName}: WARNING - {JsonNode.Parse(updObject.ToString())["message"]?.GetValue<string>()}");
                            }

                            _lstLogEvent.Add("\n<--- FINALIZA ACTUALIZACIÓN");
                            new Logs().WriteLog(_lstLogEvent);
                            ClearLogEvents();
                        }
                    }
                }
                return JsonConvert.SerializeObject(objResp, Formatting.Indented);
            }

            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: _lstLogEvent[0].ToString().Replace("ID LOG: ", "") );
                return new { message = ex.Message };
            }

        }
        #endregion

        #region CREATE Object List
        public async Task<object> CreateObjectLst(Object body)
        {
            string objName = string.Empty;
            int x = 0;
            List<Dictionary<string, JObject>> objResp = new List<Dictionary<string, JObject>>();

            var jsonString = body.ToString();
            var jsonBody = JObject.Parse(jsonString);

            JArray dataArray = (JArray)jsonBody["data"];
            try
            {
                foreach (JObject objItem in dataArray)
                {
                    foreach (var property in objItem.Properties())
                    {
                        int i = 0;
                        objName = await nameObject(property.Name);

                        JArray itemsArray = property.Value as JArray;

                        foreach (JObject item in itemsArray)
                        {
                            //logID = await GetNewGuid();
                            int itemNumber = ++i;

                            var (recIdItem, ProgramItemERP, DateTimeItemERP, UserItemERP) = ExtractMetadata(item);
                            RemoveMetadataFields(item);

                            var modelNote = CreateModelNote(ProgramItemERP, DateTimeItemERP, UserItemERP, "CREACIÓN");
                            CreateLogEvents(objName, recIdItem, item, ProgramItemERP, DateTimeItemERP, UserItemERP, "CREACIÓN", $"{itemNumber} de {itemsArray.Count}", "CreateObjectLst");

                            // - ELIAS: REGRESA LA PROPIEDAD DEL OBJETO AL ITEM
                            using JsonDocument doc = JsonDocument.Parse(item.ToString());
                            var newItem = doc.RootElement;

                            var objCreate = await createObject(objName, newItem, modelNote);

                            objResp.Add(new Dictionary<string, JObject>
                            {
                                [objName] = JObject.FromObject(objCreate)
                            });
                        }
                    }
                }
                return JsonConvert.SerializeObject(objResp, Formatting.Indented);
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: _lstLogEvent[0].ToString().Replace("ID LOG: ", "") );
                return new { message = ex.Message };
            }
        }
        #endregion



        #region Servide Request to Create Release
        public async Task<object> SRtoRelease(Object body)
        {
            string ex_LocationID_Link_RecID = string.Empty;
            string contrato = string.Empty;
            string objName = string.Empty;
            int x = 0;
            var modelNote = new NotesRequest();
            List<Dictionary<string, JObject>> objResp = new List<Dictionary<string, JObject>>();

            var jsonString = body.ToString();
            var jsonBody = JObject.Parse(jsonString);

            JArray dataArray = (JArray)jsonBody["data"];
            try
            {
                foreach (JObject objItem in dataArray)
                {
                    foreach (var property in objItem.Properties())
                    {
                        int i = 0;
                        objName = await nameObject(property.Name);

                        JArray itemsArray = property.Value as JArray;

                        foreach (JObject itemRLS in itemsArray)
                        {
                            int itemNumber = ++i;

                            ex_LocationID_Link_RecID = itemRLS["EX_LocationID_Link_RecID"]?.ToString().Trim() ?? string.Empty;
                            if (ex_LocationID_Link_RecID.Length != 32)
                            {
                                #region LOCATION and CONTRACT
                                var (nameLoc, itemsLoc) = await ExtractObjectsRelease("LOCATION", itemRLS);

                                JsonNode? jsonLoc = JsonNode.Parse(itemsLoc.ToString());
                                var custIdSitio = jsonLoc["CustID"]?.ToString().Trim() ?? string.Empty;
                                var exIdSitio = jsonLoc["EX_IdSitio"]?.ToString().Trim() ?? string.Empty;
                                var exNameSitio = jsonLoc["Name"]?.ToString().Trim() ?? string.Empty;
                                object? respLoc = await GetObjectFilter(nameLoc, $"EX_IdSitio eq '{exIdSitio}' AND CustId eq '{custIdSitio}'", "recId");

                                JObject contLoc = JObject.FromObject(respLoc);
                                ex_LocationID_Link_RecID = contLoc["value"][0]["RecId"]?.ToString().Trim();

                                if (ex_LocationID_Link_RecID == null)
                                {
                                    modelNote = CreateModelNote($"API", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM");
                                    RemoveMetadataFields(itemsLoc);
                                    respLoc = await createObject(nameLoc, itemsLoc, modelNote);

                                    #region Obtiene Datos Generales de Objeto Creado
                                    JObject recIdObj = JObject.FromObject(respLoc);
                                    ex_LocationID_Link_RecID = recIdObj["RecId"]?.ToString().Trim();
                                    #endregion

                                    itemRLS["Subject"] = String.Concat(exIdSitio, "_", exNameSitio).Trim();
                                    itemRLS["EX_LocationID_Link_RecID"] = ex_LocationID_Link_RecID;
                                    itemRLS["MILESTONE"]["EX_LocationID_Link_RecID"] = ex_LocationID_Link_RecID;

                                }
                                else
                                {
                                    itemRLS["Subject"] = String.Concat(exIdSitio, "_", exNameSitio).Trim();
                                    itemRLS["EX_LocationID_Link_RecID"] = ex_LocationID_Link_RecID;
                                    itemRLS["MILESTONE"]["EX_LocationID_Link_RecID"] = ex_LocationID_Link_RecID;
                                }
                            }

                            contrato = itemRLS["CONTRACT"]?.ToString() ?? null;
                            if (contrato != null)
                            {
                                var (nameCnt, itemsCnt) = await ExtractObjectsRelease("CONTRACT", itemRLS);

                                JsonNode? jsonCnt = JsonNode.Parse(itemsCnt.ToString());
                                contrato = jsonCnt["ivnt_InternalID"]?.ToString() ?? string.Empty;
                                object? respCnt = await GetObjectFilter(nameCnt, $"ivnt_InternalID eq '{contrato}'", "recId");
                                JObject contCnt = JObject.FromObject(respCnt);
                                var recIdCnt = contCnt["value"][0]["RecId"]?.ToString();

                                if (recIdCnt == null)
                                {
                                    modelNote = CreateModelNote($"API", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM");
                                    RemoveMetadataFields(itemsCnt);
                                    respCnt = await createObject(nameCnt, itemsCnt, modelNote);
                                }
                            }
                            #endregion

                            var (nameMltn, itemsMltn) = await ExtractObjectsRelease("MILESTONE", itemRLS);

                            #region REALEASE PROCESS
                            var srNumber = itemRLS["SrNumber"]?.ToString().Trim() ?? string.Empty;
                            var recIdItem = itemRLS["RecId"]?.ToString().Trim() ?? string.Empty;
                            RemoveMetadataFields(itemRLS, "SRtoRelease");

                            modelNote = CreateModelNote($"API SR {srNumber}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM");
                            CreateLogEvents(objName, recIdItem, itemRLS, $"API SR {srNumber}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM", $"{itemNumber} de {itemsArray.Count}", "SRtoRelease");

                            // - ELIAS: REGRESA LA PROPIEDAD DEL OBJETO AL ITEM
                            using JsonDocument doc = JsonDocument.Parse(itemRLS.ToString());
                            var newItem = doc.RootElement;

                            var objReleaseCreated = await CreateReleaseProcess(objName, newItem, null, modelNote);

                            JObject contRel = JObject.FromObject(objReleaseCreated);
                            var codeRel = contRel["code"]?.ToString();

                            objResp.Add(new Dictionary<string, JObject>
                            {
                                [objName] = JObject.FromObject(objReleaseCreated)
                            });
                            #endregion

                            if (codeRel != "ISM_4000")
                            {
                                #region ADD PROCESS
                                modelNote = CreateModelNote($"API SR {srNumber}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM");
                                CreateLogEvents(nameMltn, recIdItem, itemsMltn, $"API SR {srNumber}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM", $"{itemNumber} de {itemsArray.Count}", "Milestone");
                            
                                var objMilestoneCreated = await CreateReleaseProcess(nameMltn, itemsMltn, null, modelNote);

                                objResp.Add(new Dictionary<string, JObject>
                                {
                                    [nameMltn] = JObject.FromObject(objMilestoneCreated)
                                });
                                #endregion
                            }

                            _lstLogEvent.Add("\n<--- FINALIZA IMPORTACIÓN");
                            new Logs().WriteLog(_lstLogEvent);
                            ClearLogEvents();
                        }
                    }
                }

                return JsonConvert.SerializeObject(objResp, Formatting.Indented);
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: _lstLogEvent[0].ToString().Replace("ID LOG: ", ""));
                return new { message = ex.Message };
            }
        }
        #endregion

        #region RELACIONA CI a RELEASE FROM SR OLD ITSM
        public async Task<object> CIReleaseFromSR(Object body)
        {
            string objName = string.Empty;
            var modelNote = new NotesRequest();
            JObject recIdObj = new JObject();
            int x = 0;
            List<Dictionary<string, JObject>> objResp = new List<Dictionary<string, JObject>>();

            var jsonBody = JObject.Parse(body.ToString());
            var recIdRealease = jsonBody["data"]?[0]?["RELEASE"]?[0]?["RecId"].ToString();

            JArray ciArray = (JArray)jsonBody["data"]?[0]?["RELEASE"]?[0]?["CI"];
            try
            {
                foreach (JObject ciItem in ciArray)
                {
                    var contrato = ciItem["EX_Contrato"].ToString();
                    var serialNumber = ciItem["SerialNumber"].ToString();

                    object? respCI = await GetObjectFilter("CI", $"EX_Contrato eq '{contrato}' AND SerialNumber eq '{serialNumber}'", "recId");

                    JObject contCI = JObject.FromObject(respCI);
                    var recIdCI = contCI["value"][0]["RecId"]?.ToString();

                    if (recIdCI == null)
                    {
                        modelNote = CreateModelNote($"API", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM");
                        CreateLogEvents("CI", "", ciItem, $"API", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ATG", "IMPORTACIÓN OLD ITSM", "", "CI");

                        using JsonDocument doc = JsonDocument.Parse(ciItem.ToString());
                        var newItem = doc.RootElement;

                        respCI = await createObject("CI", newItem, modelNote);
                                                
                        #region Obtiene Datos Generales de Objeto Creado
                        recIdObj = JObject.FromObject(respCI);
                        recIdCI = recIdObj["RecId"]?.ToString();
                        #endregion

                        if (recIdCI != null)
                        {                            
                            var result = await patchObjectRelationships("CI", recIdCI, "ReleaseProjectAssocCI", recIdRealease);
                        }
                    }
                    else
                    {

                        var result = await GetObjectRelationships("CI", recIdCI, "ReleaseProjectAssocCI");
                        JObject contRel = JObject.FromObject(result);

                        await patchObjectRelationships("CI", recIdCI, "ReleaseProjectAssocCI", recIdRealease);
                    }
                }

                return JsonConvert.SerializeObject(objResp, Formatting.Indented);
            }
            catch (Exception ex)
            {
                new Logs().WriteExc(ex, logID: _lstLogEvent[0].ToString().Replace("ID LOG: ", ""));
                return new { message = ex.Message };
            }

        }
        #endregion

        private async Task<(string objeto, JObject itemChildren)> ExtractObjectsRelease(string extract, Object item)
        {
            var nameobj = await nameObject(extract);

            JsonNode? json = JsonNode.Parse(item.ToString());
            var itemObj = JObject.Parse(json[extract].ToString());

            return (nameobj, itemObj);
        }

        private async Task<object> CreateReleaseProcess(string objeto, Object body, List<string>? lstLogEvent = null, NotesRequest modelNote = null)
        {
            JsonNode? json = JsonNode.Parse(body.ToString());

            #region Crea Nuevo Objeto en Ivanti (Release Proccess)
            var client = CreateHttpClient();
            var response = await client.PostAsJsonAsync($"{_pathUrlBizObj}/{objeto}s", json);
            string statusCode = $"Status: {(int)response.StatusCode}";
            var content = await response.Content.ReadAsStringAsync();

            var respActivity = JsonConvert.DeserializeObject(content);

            if (response.IsSuccessStatusCode)
            {

                #region Obtiene Datos Generales de Objeto Creado
                JObject recIdObj = JObject.FromObject(respActivity);
                
                var recId = recIdObj["RecId"]?.ToString();
                #endregion

                _lstLogEvent.Add($"\n\t*** RESPUESTA ***");
                _lstLogEvent.Add($"\t{statusCode}");
                _lstLogEvent.Add($"\tObjeto {char.ToUpper(objeto.ToLower()[0]) + objeto.ToLower().Substring(1)} Creado, RecId: {recId}");

                modelNote.Object = objeto;
                modelNote.Note = modelNote.Note + "\n\n" + json + "\n";

                await addNoteObject("Journal__Notes", modelNote, recId);

                return respActivity;

            }
            else
            {
                JsonNode? msgJson = JsonNode.Parse(content.ToString());
                var msg = msgJson["message"][0]?.GetValue<string>();

                _lstLogEvent.Add($"\n\t*** RESPUESTA ***");
                _lstLogEvent.Add($"\tStatus: {(int)response.StatusCode}");
                _lstLogEvent.Add($"\tMessage: {msg}");

                return respActivity = JsonConvert.DeserializeObject(content);
            }
            #endregion            
        }
    }
}