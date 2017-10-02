using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SpaceCommander
{
    public class INIHandler
    {
        private string path; //Имя файла.

        [DllImport("kernel32")] // Подключаем kernel32.dll и описываем его функцию WritePrivateProfilesString
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")] // Еще раз подключаем kernel32.dll, а теперь описываем функцию GetPrivateProfileString
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        // С помощью конструктора записываем пусть до файла и его имя.
        public INIHandler(string IniPath)
        {
            path = new FileInfo(IniPath).FullName.ToString();
        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string ReadINI(string Section, string Key)
        {
            return ReadINI(Section, Key, 255);
        }
        public string ReadINI(string Section, string Key, string Default)
        {
            return ReadINI(Section, Key, Default, 255);
        }
        public string ReadINI(string Section, string Key, int Size)
        {
            return ReadINI(Section, Key, "", 255);
        }
        public string ReadINI(string Section, string Key, string Default, int Size)
        {
            var RetVal = new StringBuilder(Size);
            GetPrivateProfileString(Section, Key, Default, RetVal, Size, path);
            return RetVal.ToString();
        }

        //Записываем в ini-файл. Запись происходит в выбранную секцию в выбранный ключ.
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }

        //Удаляем ключ из выбранной секции.
        public void DeleteKey(string Key, string Section = null)
        {
            Write(Section, Key, null);
        }
        //Удаляем выбранную секцию
        public void DeleteSection(string Section = null)
        {
            Write(Section, null, null);
        }
        //Проверяем, есть ли такой ключ, в этой секции
        public bool KeyExists(string Key, string Section = null)
        {
            return ReadINI(Section, Key).Length > 0;
        }

        //not work
        //public List<string> GetSections()
        //{
        //    StringBuilder returnString = new StringBuilder(32768);
        //    GetPrivateProfileString(null, null, null, returnString, 32768, this.path);
        //    List<string> result = new List<string>(returnString.ToString().Split('\0'));
        //    if (result.Count>3) result.RemoveRange(result.Count - 2, 2);
        //    return result;
        //}

        //not work
        //public List<string> GetKeys(string section)
        //{
        //    StringBuilder returnString = new StringBuilder(32768);
        //    GetPrivateProfileString(section, null, null, returnString, 32768, this.path);
        //    List<string> result = new List<string>(returnString.ToString().Split('\0'));
        //    if (result.Count > 3) result.RemoveRange(result.Count - 2, 2);
        //    return result;
        //}
    }
}
