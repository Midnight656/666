using ConsoleApp32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp32
{
    internal class Program
    {
        //C:\Users\User\OneDrive\Рабочий стол\rere.txt
        public string path;
        public string fileType;
        public int objectsAmount = 0;
        public List<Persons> personsToTransfer;
        static void Main(string[] args)
        {
            Program program = new Program();
            program.start();

        }
        public void start()
        {
            writeInstructionForOpening();
            getPathInfo();
            Console.Clear();
            openFile();
            writeNextChoises();
            writePersonsProperties();
            ConsoleKeyInfo key = Console.ReadKey();
            while (key.Key != ConsoleKey.Escape && key.Key != ConsoleKey.F1)
            {
                key = Console.ReadKey();
            }
            if (key.Key == ConsoleKey.F1)
            {
                Console.Clear();
                writeSavingInstruction();
                getPathInfo();
                SaveInFile();
                writeSavingConfirmation();
                Console.ReadLine();
            }
            key = Console.ReadKey();
            while (key.Key != ConsoleKey.Escape)
            {
                key = Console.ReadKey();
            }


        }
        private void writeInstructionForOpening()
        {
            Console.WriteLine("Введите путь до файла (вместе с названием), который хотите открыть");
            Console.WriteLine("------------------------------------------------------------------");
        }
        private void getPathInfo()
        {
            while (Console.CursorTop == 3) { }
            getPath();
            getFileType();
        }
        private void getPath()
        {
            path = Console.ReadLine();
        }
        private void writeNextChoises()
        {
            Console.WriteLine("Сохранить файл в одном из трёх вариантов (txt, json, xml) - F1, Закрыть программу-Escape");
            Console.WriteLine("------------------------------------------------------------------");
        }
        private void writeSavingInstruction()
        {
            Console.WriteLine("Введите путь до файла (вместе с названием), в который вы хотите сохранить");
            Console.WriteLine("------------------------------------------------------------------");
        }
        private void writeSavingConfirmation()
        {
            Console.WriteLine("Ваши данные сохранены, можете закрыть программу");
        }
        private void writePersonsProperties()
        {
            foreach (Persons persona in personsToTransfer)
            {
                Type type = persona.GetType();
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string name = property.Name;
                    object value = property.GetValue(persona);
                    Console.WriteLine(value);
                }
            }
        }
        private void getFileType()
        {
            string[] pathInArray = path.Split('.');
            fileType = pathInArray[1];
        }
        private void openFile()
        {
            switch (fileType)
            {
                case "txt":
                    string[] fileTextToArray = parseTxtFileText();
                    turnTxtTextToPersonProperty(fileTextToArray);
                    break;
                case "xml":
                    getJsonFileTextAndTurnToPersonProperty();
                    break;
                case "json":
                    string fileText = getJsonFileText();
                    turnJsonTextToPersonProperty(fileText);
                    break;

            }
        }
        private string[] parseTxtFileText()
        {
            string[] fileTextToArray = new string[] { "0" };
            int counter = 0;
            string[] clone = new string[] { "0" };
            using (StreamReader reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    fileTextToArray[counter] = line;
                    counter += 1;
                    clone = fileTextToArray;
                    fileTextToArray = new string[clone.Length + 1];
                    Array.Copy(clone, 0, fileTextToArray, 0, clone.Length);
                }
            }
            return fileTextToArray;


        }
        private void turnTxtTextToPersonProperty(string[] fileTextToArray)
        {
            List<Persons> persons = new List<Persons>();
            for (int i = 0; i + 3 < fileTextToArray.Length; i += 3)
            {
                Persons person = new Persons();
                person.Name = fileTextToArray[i];
                person.Age = Convert.ToInt32(fileTextToArray[i + 1]);
                person.Group = fileTextToArray[i + 2];
                persons.Add(person);
            }
            personsToTransfer = persons;
        }
        private void getJsonFileTextAndTurnToPersonProperty()
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Persons>));
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                personsToTransfer = (List<Persons>)xml.Deserialize(fs);
            }
        }
        private string getJsonFileText()
        {
            return File.ReadAllText(path);
        }
        private void turnJsonTextToPersonProperty(string fileText)
        {
            personsToTransfer = JsonConvert.DeserializeObject<List<Persons>>(fileText);
        }

        private void SaveInFile()
        {
            switch (fileType)
            {
                case "txt":
                    saveInTextFile();
                    break;
                case "xml":
                    saveInXmlFile();
                    break;
                case "json":
                    saveInJsonFile();
                    break;

            }
        }

        private void saveInTextFile()
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (Persons persona in personsToTransfer)
                {
                    Type type = persona.GetType();
                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        string name = property.Name;
                        object value = property.GetValue(persona);
                        writer.WriteLine(value);
                    }
                }
            }
        }
        private void saveInXmlFile()
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Persons>));
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                xml.Serialize(fs, personsToTransfer);
            }
        }
        private void saveInJsonFile()
        {
            string json = JsonConvert.SerializeObject(personsToTransfer);
            File.WriteAllText(path, json);
        }
    }
    public class Persons
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Group { get; set; }
    }
}
