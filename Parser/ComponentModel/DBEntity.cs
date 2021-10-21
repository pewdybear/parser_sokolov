using System.Collections.Generic;

namespace Parser.ComponentModel
{
    public abstract class DBEntity
    {
        abstract public string TableName { get; protected set; }
        abstract public string[] Fields { get; protected set; }
        public long? Id { get; set; }

        public DBEntity()
        {
        }

        public DBEntity(long? id)
        {
            this.Id = id;
        }

        public string GetFields()
        {
            string fields = "";
            for (int i = 1; i < fields.Length; i++)
            {
                if (i != fields.Length - 1)
                    fields += fields[i] + ", ";
                else fields += fields[i];
            }
            return fields;
        }

        abstract public long? Add(DBEntity dBEntity);
        abstract public CollectionModel Fill(Dictionary<string, List<string>> allRow);
        abstract public void PrintTable();
        abstract public void Save();
        abstract public CollectionModel ReturnCollection();
        abstract public long? Contain(string url);
        abstract public DBEntity SaveLast();
        abstract public DBEntity ReturnStr(long? index);
    }
}
