using Blazor.IndexedDB;
using Microsoft.JSInterop;
using System;

namespace Vokabel_Teller.OwnClasses
{
    public class IndexedDBConfig : IndexedDb
    {
        

        // Der Konstruktor der Klasse
        public IndexedDBConfig(IJSRuntime jsRuntime, string name, int version): base(jsRuntime, name, version)
        {

        }

        //Fester Eintrag für die Tabellenverwaltung
        public IndexedSet<AdministrationClass>? AdministrationTable { get; set; }

        //Tabellen
        public IndexedSet<TableLists>? WordSentenceTable { get; set; }


    }


    //Modellklassen für die Einzelnen Datenbanken. 
    

    //Zum Switchen der verschiedenen Tabellen, wenn mehr als eine verwendet wird. 
    public class AdministrationClass
    {
        [System.ComponentModel.DataAnnotations.Key] // Primärschlüssel der Tabelle
        public long Id { get; set; }

        public string? TabellenName { get; set; }
        public string? ListName { get; set; }
    }

    //Tabelle Klasse für reine Wort/Satz gegenüberstellungen z.b. Vokabeln Deutsch Englisch
    public class TableLists
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long Id { get; set; }
        public string? Listname { get; set; }

        // Als JSON-String speichern
        public string? JasonList { get; set; }

    }
}

