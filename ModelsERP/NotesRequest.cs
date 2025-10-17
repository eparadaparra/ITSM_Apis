namespace ITSM_Apis.Models
{
    public class NotesRequest
    {
        /// <example>Change</example>
        public string Object { get; set; }

        /// <example>10046</example>
        public string ObjectNumber { get; set; }

        /// <example>subject de Cambio 10046</example>
        public string Subject { get; set; }

        /// <example>Notas de cambio 10046</example>
        public string Note { get; set; }
    }
}
