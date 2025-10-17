using ITSM_Apis.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace ITSM_Apis.Services
{
    public interface IServices
    {
        Task<string> GetNewGuid();
        Task<Object> GetObject(string obj);
        Task<Object> DeleteObject(string objeto, string recId, string logID = "");
        Task<string> nameObject(string obj);
        Task<Object> GetObjectByRecId(string objeto, string recId, string logID = "");
        Task<Object> GetObjectFilter(string objeto, string filtro, string select, string logID = ""); 
        Task<Object> GetObjectRelationships(string objeto, string recIdObj, string relation, string logID = "");
        Task<Object> patchObjectRelationships(string objeto, string recIdObj, string relation, string recIdRelation, string logID = "");
        Task<Object> deleteObjectRelationships(string objeto, string recIdObj, string relation, string recIdRelation, string logID = "");
        Task<Object> updateObjectByRecId(string objeto, string recIdObj, Object body);
        Task<Object> createObject(string objeto, Object body, NotesRequest model = null);
        Task<Object> addNoteObject(string objNote, NotesRequest modNotes, string recId = "");

        Task<Object> UpdateObjectLst(Object body);
        Task<Object> CreateObjectLst(Object body);


        Task<Object> SRtoRelease(Object body);
        Task<Object> CIReleaseFromSR(Object body);

    }
}
