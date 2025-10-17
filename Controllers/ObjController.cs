using ITSM_Apis.Models;
using ITSM_Apis.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace ITSM_Apis.Controllers
{
    [EnableCors("RulesCors")]
    [ApiController]

    [Route("api/[controller]")]
    public class ObjController : ControllerBase
    {
        private readonly IServices _services;
        private readonly string _objeto = "CI".ToUpper();

        public ObjController(IServices servicesAPI)
        {
            _services = servicesAPI;
        }

        [EnableCors("RulesCors")]

        #region NEW_ID
        [HttpGet]
        [Route("NewGuid")]
        public async Task<IActionResult> GetNewGuid()
        {
            //string guidSinGuiones = await _services.GetNewGuid();
            return StatusCode(StatusCodes.Status200OK, new { RecID = await _services.GetNewGuid() });
        }
        #endregion

        #region Obj
        [HttpGet]
        [Route("Object")]
        public async Task<IActionResult> getObject(string objeto)
        {
            var objName = await _services.nameObject(objeto);
            
            var result = await _services.GetObject(objName);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        #region Obj by RecId
        [HttpGet]
        [Route("RecId")]
        public async Task<IActionResult> getObjectByRecId(string objeto, string recId)
        {
            string logID = await _services.GetNewGuid();

            var result = await _services.GetObjectByRecId(objeto, recId, logID);

            return StatusCode( StatusCodes.Status200OK, result.ToString() );
        }
        #endregion

        #region Obj Delete
        [HttpDelete]
        [Route("RecId")]
        public async Task<IActionResult> DeleteObject(string objeto, string recId)
        {
            string logID = await _services.GetNewGuid();

            var result = await _services.DeleteObject(objeto, recId, logID);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        #region Obj Filter
        [HttpGet]
        [Route("Filter")]
        public async Task<IActionResult> getObjectByFilter(string objeto, [BindRequired] string filter, string select = "")
        {
            objeto = await _services.nameObject(objeto);
            string seleccion = string.IsNullOrEmpty(select) ? "*" : select;
            string logID = await _services.GetNewGuid();

            var result = await _services.GetObjectFilter(objeto, filter, seleccion, logID);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        #region Obj Relationships
        [HttpGet]
        [Route("Relationships")]
        public async Task<IActionResult> getObjectRelation(string objeto, string recIdObj, string relation)
        {
            objeto = await _services.nameObject(objeto);
            string logID = await _services.GetNewGuid();
            var result = await _services.GetObjectRelationships(objeto, recIdObj, relation, logID);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        #region Obj Delete Relationships
        [HttpDelete]
        [Route("Relationships")]
        public async Task<IActionResult> deleteObjectRelationships(string objeto, string recIdObj, string relation, string recIdRelation)
        {
            string logID = await _services.GetNewGuid();
            var result = await _services.deleteObjectRelationships(objeto, recIdObj, relation, recIdRelation, logID);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        #region Obj Patch Relationships
        [HttpPatch]
        [Route("Relationships")]
        public async Task<IActionResult> patchObjectRelationships(string objeto, string recIdObj, string relation, string recIdRelation)
        {
            string logID = await _services.GetNewGuid();
            var result = await _services.patchObjectRelationships(objeto, recIdObj, relation, recIdRelation, logID);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        #region Obj Update Object
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> updateObjectByRecId(string objeto, string recId, [FromBody] Object body)
        {
            objeto = await _services.nameObject(objeto.Trim().ToUpper());
            
            var result = await _services.updateObjectByRecId(objeto, recId.Trim(), body);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        #region Obj Create Object
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> createObject([FromBody] Object body)  
        {
            object result = null;
            string obj = String.Empty;
            string val = String.Empty;

            var items = JsonConvert.DeserializeObject<List<Dictionary<string, JObject>>>(body.ToString());

            foreach (var item in items)
            {
                foreach (var kvp in item)
                {
                    obj = await _services.nameObject(kvp.Key);  //Console.WriteLine(obj);
                    val = kvp.Value.ToString(Formatting.None);  //Console.WriteLine(kvp.Value.ToString(Formatting.Indented));
                }
            }

            result = await _services.createObject(obj, val);

            return StatusCode( StatusCodes.Status200OK, result.ToString() );
        }
        #endregion

        #region Add Note Objects
        [HttpPost]
        [Route("AddNote")]
        public async Task<IActionResult> AddNoteObject([BindRequired][FromBody] NotesRequest json)
        {
            var result = await _services.addNoteObject("Journal__Notes", json);

            return StatusCode(StatusCodes.Status200OK, result.ToString());
        }
        #endregion

        //#region Import List Objects
        //[HttpPost]
        //[Route("ImportLstObj")]
        //public async Task<IActionResult> ImportLstObject([FromBody] Object body) 
        //{
        //    var result = await _services.ImportLstObject(body);

        //    return StatusCode(StatusCodes.Status200OK, result);
        //}
        //#endregion

        #region Update List Objects
        [HttpPost]
        [Route("UpdateObjLst")]
        public async Task<IActionResult> UpdateObjectLst([FromBody] Object body)
        {
            var result = await _services.UpdateObjectLst(body);
            return StatusCode(StatusCodes.Status200OK, result);
        }
        #endregion

        #region Create List Objects
        [HttpPost]
        [Route("CreateObjLst")]
        public async Task<IActionResult> CreateObjectLst([FromBody] Object body)
        {
            var result = await _services.CreateObjectLst(body);
            return StatusCode(StatusCodes.Status200OK, result);
        }
        #endregion

    }
}
