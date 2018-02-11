using System;
using System.IO;
using System.Xml.Serialization;
using tester_server;
using tester_server.Connection.Authentification;
using tester_server.Tester;

namespace Tester_server_GUI.Data
{
    public class Dataset
    {
        public static readonly string DATA_SET_FOLDER = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Test_DataSet");
        private static readonly string CONFIG_FILE_PATH = System.IO.Path.Combine(DATA_SET_FOLDER, "config.conf");

        //list of data
        public Account admin;
        public Start start;
        public LoginManager login_manager;
        public TestManager test_manager;
        public int TestID { get { return TestID++; } private set { TestID = value; } }
        //end

        private static Dataset db;

        static Dataset()
        {
            //ak neexistuje adresar vytvor ho
            if (!Directory.Exists(DATA_SET_FOLDER))
                Directory.CreateDirectory(DATA_SET_FOLDER);
            //ak neexistuje conf subor vytvor ho
            if (!File.Exists(CONFIG_FILE_PATH))
                File.Create(CONFIG_FILE_PATH);
            db = RetrieveFromFile();
            //ak pred tym nebolo v subore nic ulozene vytvor objekt
            if (db == null)
                db = new Dataset();
        }

        private Dataset() {
            admin = new Account { Is_LogedOn = false, password_hash = "password".GetHashCode(), user_name = "admin" };
            start = new Start();
            login_manager = new LoginManager();
            test_manager = new TestManager()
            TestID = 0;
        }

        ~Dataset()
        {
            StoreToFile();
        }

        public static Dataset Instance()
        {
            return db;
        }

        private void StoreToFile()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Dataset));
            using (Stream str = File.Create(CONFIG_FILE_PATH))
                ser.Serialize(str, this);
        }

        private static Dataset RetrieveFromFile()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Dataset));
            try
            {
                using (Stream str = File.Open(CONFIG_FILE_PATH, FileMode.Open))
                    return (Dataset)ser.Deserialize(str);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
