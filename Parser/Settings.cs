namespace Parser
{
    static class Settings
    {
        #region MySQL
        static public string Host = "s2.kts.tu-bryansk.ru";
        static public string Port = "3306";
        static public string User = "IAS16.SokolovAJU";
        static public string Password = "f{7F?2Rr3Q";
        static public string DefaultSchema = "IAS16_SokolovAJU";
        static public string CharSet = "UTF8";
        #endregion

        #region Parser
        static public string URL = "https://author.today/work/genres";
        static public int Threads = 8;
        #endregion
    }
}
