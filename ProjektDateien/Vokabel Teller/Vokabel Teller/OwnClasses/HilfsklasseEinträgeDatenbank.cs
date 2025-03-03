using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Blazor.IndexedDB;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace Vokabel_Teller.OwnClasses
{
    public static class HilfsklasseEinträgeDatenbank
    {

        //if change the Database Komponents, than huckup the Versionnumber ++ for start a Database update next App start. 
        private static int Versionsnummer = 1;
        //Methoden

        //Datenstruktur zum Speichern der Informataionspaare
        public class DataCouple
        {
            public string Data1 { get; set; }
            public string Data2 { get; set; }

            public DataCouple(string data1, string data2)
            {
                Data1 = data1;
                Data2 = data2;
            }
            //only using a standard Construktor

            public DataCouple()
            {
                Data1 = string.Empty;
                Data2 = string.Empty;
            }

        }

        //UpdateMethode
        public static async Task UpdateIndexedDBStart(IIndexedDbFactory dbFactory)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>("VokaTeller", Versionsnummer))
            {
                await db.SaveChanges();

            }
        }

        public static async Task<string> NewList(IIndexedDbFactory dbFactory, string _NameOfTabelle, string _newListName)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>(_NameOfTabelle))
            {
                // 1. Überprüfen, ob die Tabelle in der Datenbank existiert
                var _GetTableProperty = db.GetType().GetProperty(_NameOfTabelle); // Hier ist "WordSentenceTable" die angenommene Tabellen-Property, passe sie ggf. an.
                if (_GetTableProperty != null)
                {
                    // 2. Lese die Tabelle (Wert der Property) aus
                    var _GetTable = _GetTableProperty.GetValue(db) as IEnumerable<TableLists>;
                    if (_GetTable != null)
                    {
                        // 3. Überprüfen, ob bereits ein Eintrag mit demselben Listnamen existiert
                        var existingList = _GetTable.FirstOrDefault(t => t.Listname == _newListName);
                        if (existingList != null)
                        {
                            // Wenn ein Eintrag mit demselben Listennamen existiert, abbrechen und Nachricht zurückgeben
                            return "Liste existiert bereits. Doppelte Namen nicht erlaubt";
                        }

                        // 4. Wenn kein Eintrag mit demselben Listnamen existiert, neuen Eintrag erstellen
                        var newList = new TableLists
                        {
                            Listname = _newListName, // Korrektur: _newListName sollte verwendet werden, nicht _NameOfTabelle
                            JasonList = "" // Leerer JSON-String
                        };

                        // 5. Neuen Eintrag zur Tabelle hinzufügen
                        db.WordSentenceTable!.Add(newList);

                        // 6. Änderungen in der Datenbank speichern
                        await db.SaveChanges();

                        // 7. Erfolgsnachricht zurückgeben
                        return $"Die Liste '{_newListName}' wurde erfolgreich erstellt.";
                    }
                }

                // Wenn die Tabelle nicht gefunden wird oder der Zugriff fehlschlägt
                return $"Die Tabelle mit dem Namen '{_NameOfTabelle}' existiert nicht in der Datenbank.";
            }
        }

        public static async Task AddInformationCoupleToList(IIndexedDbFactory dbFactory, string _NameOfTabelle, string _ListName, List<DataCouple> _NewList)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>(_NameOfTabelle))
            {
                string neuerJasonString = JsonSerializer.Serialize(_NewList);
                List<DataCouple> resultList = (JsonSerializer.Deserialize<List<DataCouple>>(neuerJasonString))!;
                var _GetListProperty = db.GetType().GetProperty(_NameOfTabelle);
                if (_GetListProperty != null)
                {
                    var _GetTablle = _GetListProperty.GetValue(db) as IEnumerable<TableLists>;
                    if (_GetTablle != null)
                    {
                        var _GetList = _GetTablle.FirstOrDefault(t => t.Listname == _ListName);
                        if (_GetList != null)
                        {
                            _GetList.JasonList = neuerJasonString;
                            await db.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine($"Eintrag mit dem Namen '{_ListName}' wurde nicht in der Tabelle '{_NameOfTabelle}' gefunden.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Die Tabelle mit dem Namen '{_NameOfTabelle}' konnte nicht ausgelesen werden.");
                    }
                }
                else
                {
                    Console.WriteLine($"Die Tabelle mit dem Namen '{_NameOfTabelle}' existiert nicht in der Datenbank.");
                }
            }
        }

        public static async Task ChangeEntry(IIndexedDbFactory dbFactory, string _NameOfTabelle, string ListName, string oldEntry1, string newEntry1, int fromEntryOneOrTwo)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>(_NameOfTabelle))
            {
                var _GetTableProperty = db.GetType().GetProperty(_NameOfTabelle);
                if (_GetTableProperty != null)
                {
                    var _GetTable = _GetTableProperty.GetValue(db) as IEnumerable<TableLists>;
                    if (_GetTable != null)
                    {
                        var existingList = _GetTable.FirstOrDefault(t => t.Listname == ListName);
                        if (existingList != null)
                        {
                            int countList = 0;
                            List<DataCouple> resultList = (JsonSerializer.Deserialize<List<DataCouple>>(existingList.JasonList!))!;
                            foreach (var _temp in resultList)
                            {
                                string newJson = "";
                                Console.WriteLine("Davor "+_temp.Data1 +" und " + newEntry1);
                                if (_temp.Data1 == oldEntry1 && fromEntryOneOrTwo==1)
                                {
                                    resultList[countList].Data1 = newEntry1;
                                    newJson = JsonSerializer.Serialize(resultList);
                                    existingList.JasonList = newJson;
                                    await db.SaveChanges();
                                    Console.WriteLine("If 1 Davor " + _temp.Data1 + " und " + newEntry1);
                                    return;
                                }
                                else if (_temp.Data2 == oldEntry1 && fromEntryOneOrTwo==2)
                                {
                                    resultList[countList].Data2 = newEntry1;
                                    newJson = JsonSerializer.Serialize(resultList);
                                    existingList.JasonList = newJson;
                                    await db.SaveChanges();
                                    Console.WriteLine("If 2 Davor " + _temp.Data1 + " und " + newEntry1);
                                    return;
                                }
                                countList++;
                            }
                        }
                        else
                        {
                            Console.WriteLine("ExistingList is null");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Table is null");
                    }
                }
                else
                {
                    Console.WriteLine("Proberty is null");
                }
            }

        }

        public static async Task<List<DataCouple>> GetInformationJasonStringFromList(IIndexedDbFactory dbFactory, string _NameOfTabelle, string _ListName)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>(_NameOfTabelle))
            {
                // Überprüfen, ob die Tabelle existiert, indem man sie abruft
                var _GetTableProperty = db.GetType().GetProperty(_NameOfTabelle);
                if (_GetTableProperty != null)
                {
                    // Laden der Tabelle
                    var _GetTable = _GetTableProperty.GetValue(db) as IEnumerable<TableLists>;
                    if (_GetTable != null)
                    {
                        // Suchen des Eintrags in der Tabelle
                        var _GetList = _GetTable.FirstOrDefault(t => t.Listname == _ListName);

                        if (_GetList != null)
                        {
                            // JSON-String aus dem Eintrag abrufen
                            string jsonString = _GetList.JasonList!;
                            if (jsonString != null && jsonString != "")
                            {
                                // Den JSON-String in eine List<string> umwandeln
                                List<DataCouple> resultList = (JsonSerializer.Deserialize<List<DataCouple>>(jsonString))!;

                                // Die Liste zurückgeben
                                return resultList!;
                            }
                            else
                            {
                                // Json ist null oder ""
                                Console.WriteLine($"GetInformationJasonStringFromList() : Json String ist null oder leer.");
                            }
                        }
                        else
                        {
                            // Ausgabe, wenn kein passender Eintrag gefunden wurde
                            Console.WriteLine($"Eintrag mit dem Namen '{_ListName}' wurde nicht in der Tabelle '{_NameOfTabelle}' gefunden.");
                        }
                    }
                    else
                    {
                        // Ausgabe, wenn die Tabelle nicht abgerufen werden konnte
                        Console.WriteLine($"Die Tabelle mit dem Namen '{_NameOfTabelle}' konnte nicht ausgelesen werden.");
                    }
                }
                else
                {
                    // Ausgabe, wenn keine Tabelle mit dem Namen `NameOfTabelle` existiert
                    Console.WriteLine($"Die Tabelle mit dem Namen '{_NameOfTabelle}' existiert nicht in der Datenbank.");
                }

                // Wenn keine Liste gefunden wurde, eine leere Liste zurückgeben
                return new List<DataCouple>();
            }
        }

        public static async Task ListNameChange(IIndexedDbFactory dbFactory, string _NameOfTabelle, string _oldListName, string _newListName)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>(_NameOfTabelle))
            {
                // 1. Überprüfen, ob die Tabelle existiert, indem man sie abruft
                var _GetTableProperty = db.GetType().GetProperty(_NameOfTabelle);
                if (_GetTableProperty != null)
                {
                    // 2. Lese die Tabelle (Wert der Property) aus
                    var _GetTable = _GetTableProperty.GetValue(db) as IEnumerable<TableLists>;
                    if (_GetTable != null)
                    {
                        // 3. Suchen des Eintrags in der Tabelle

                        var existingList = _GetTable.FirstOrDefault(t => t.Listname == _oldListName);
                        if (existingList != null)
                        {
                            // 4. Name ändern
                            existingList.Listname = _newListName;
                            // 5. Änderungen in der Datenbank speichern
                            await db.SaveChanges();


                        }
                        else
                        {
                            Console.WriteLine("ExistingList is null");
                        }

                    }
                    else
                    {
                        Console.WriteLine("GetTable in ListNameChange is null");
                    }
                }
                else
                {
                    Console.WriteLine("Proberty in ListNameChange is null");
                }
            }

        }

        public static async Task DeleteList(IIndexedDbFactory dbFactory, string _NameOfTabelle, string _ListName, string buttonName)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>(_NameOfTabelle))
            {
                var _GetTableProperty = db.GetType().GetProperty(_NameOfTabelle);
                if (_GetTableProperty != null)
                {
                    var _GetTable = _GetTableProperty.GetValue(db) as IEnumerable<TableLists>;
                    if (_GetTable != null)
                    {
                        if (buttonName == _ListName)
                        {
                            var existingList = _GetTable.FirstOrDefault(t => t.Listname == _ListName);
                            if (existingList != null)
                            {
                                db.WordSentenceTable!.Remove(existingList);
                                await db.SaveChanges();
                            }
                            else
                            {
                                Console.WriteLine("ExistingList in DeleteList is null");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong Button in DeleteList is null");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Table in DeleteList is null");
                    }
                }
                else
                {
                    Console.WriteLine("Proberty in DeleteList is null");
                }
            }

        }

        // Diese Methode gibt alle Listennamen aus der WordSentenceTable zurück. Sie ist nicht dynamisch auf mehrere Tabellen anwendbar!
        public static async Task<List<string>> GetAllListNamesAsync(IIndexedDbFactory dbFactory, string _NameOfTabelle)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>(_NameOfTabelle))
            {
                if (db == null)
                {
                    Console.WriteLine("Datenbank konnte nicht erstellt werden oder ist null.");
                    return new List<string>(); // return empty list if database can be created
                }
                var table = db.WordSentenceTable;
                if (table != null)
                {
                    if (table.Any())
                    {
                        return table.Select(t => t.Listname).ToList()!;
                    }
                    else
                    {
                        Console.WriteLine($"Die Tabelle {_NameOfTabelle} ist leer.");
                    }
                }
                else
                {
                    Console.WriteLine($"Die Tabelle {_NameOfTabelle} existiert nicht.");
                }
            }
            // Falls die Tabelle nicht existiert, leer ist oder ein Fehler aufgetreten ist, eine leere Liste zurückgeben
            return new List<string>();
        }

        public static async Task<string> BackupDownloadStart(IIndexedDbFactory dbFactory)
        {
            //Indexöffnen und auslesen


            using (var db = await dbFactory.Create<IndexedDBConfig>("WordSentenceTable"))
            {
                var JsonObjekt = new
                {
                    AdministrationTable = db.AdministrationTable,
                    SentenceTabelle = db.WordSentenceTable,
                };

                var JsonData = System.Text.Json.JsonSerializer.Serialize(JsonObjekt);
                return JsonData;
            }

        }

        public static async Task BackupImplemintate(IIndexedDbFactory dbFactory, List<DataCouple> _JasonAdministrationClass, List<TableLists> _JasonTableLists)
        {
            using (var db = await dbFactory.Create<IndexedDBConfig>("WordSentenceTable"))
            {
                var ListAdmin = db.WordSentenceTable;
                ListAdmin!.Clear();
                for (int i = 0; i < _JasonAdministrationClass.Count; i++)
                {
                    await NewList(dbFactory, _JasonAdministrationClass[i].Data1, _JasonAdministrationClass[i].Data2);
                }
                var ListTable = db.WordSentenceTable;
                ListTable!.Clear();
                for (int i = 0; i < _JasonTableLists.Count; i++)
                {
                    db.WordSentenceTable!.Add(_JasonTableLists[i]);
                }
                await db.SaveChanges();
            }

        }
    }
}





