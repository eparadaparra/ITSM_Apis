namespace ITSM_Apis.Conn
{
    public class Connection
    {
        private readonly string _connectionStrig = string.Empty;
        private static bool _enableDEV;

        public Connection()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            
            _enableDEV = Boolean.Parse(builder.GetSection("Settings:EnableDev").Value);
            
            var ambiente = _enableDEV ? "ConnectionStrings:connITSM_STG" : "ConnectionStrings:connITSM_PRD";
            
            _connectionStrig = builder.GetSection("ConnectionStrings:connITSM").Value;
        }

        public string SqlComm() => _connectionStrig;
    }
}
