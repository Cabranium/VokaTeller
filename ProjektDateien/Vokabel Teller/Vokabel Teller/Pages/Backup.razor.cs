using Blazor.IndexedDB;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Vokabel_Teller.OwnClasses;




namespace Vokabel_Teller.Pages

{
    public partial class Backup
    {
        //for the work access to download.js
        [Inject]
        private IJSRuntime? JS { get; set; }
        //for the work access to NavigationManager
        [Inject]
        private NavigationManager? Navigation { get; set; }
        //for the work access of singleton of IIndexedDbFactory connecting with Database
        [Inject]
        private IIndexedDbFactory? DbFactory { get; set; }
        //for selections the Backup Data
        private IBrowserFile? BackupSelection = null;
        private bool isDataSelect = false;
        private string labelNameBackupStaus = "";
        private string labelBackupBackgroundColor = "";
        private bool isBackupButtonDisabled = false;
        //if selected not a json, the count raise and reload and reset this InputFile element
        private int fileKeyErrorReloadCounter = 0;



        //for creating the Backup over download.JS Methode
        private async void BackUpBecomesCreate()
        {
            var DatenFürBackup = await HilfsklasseEinträgeDatenbank.BackupDownloadStart(DbFactory!);
            await JS!.InvokeVoidAsync("downloadFile", "BackupDataBase.json", DatenFürBackup);

        }

        //after the User Selected a Backupfile becomes a check whether File ends with .json
        private void CheckSelectedBackup(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                isDataSelect = true;
                BackupSelection = file;
                labelNameBackupStaus = "";
                labelBackupBackgroundColor = "";
            }
            else
            {
                isDataSelect = false;
                BackupSelection = null;
                //reset the InputFile Element for new selection
                fileKeyErrorReloadCounter++;
                labelBackupBackgroundColor = "orangered";
                labelNameBackupStaus = "You have not Seleced a json File. Try again!";
            }
        }





        //Backup reload routine
        private async Task LoadBackupFile()
        {
            if (isDataSelect == true && BackupSelection != null)
            {
                // open Stream
                using var stream = BackupSelection.OpenReadStream();
                using var reader = new StreamReader(stream);
                // read the content
                var jsonContent = await reader.ReadToEndAsync();
                // rebuild the Backup from Json  
                var backupData = System.Text.Json.JsonSerializer.Deserialize<BackupData>(jsonContent);
                //creating Lists for reload the Database for recovering
                List<HilfsklasseEinträgeDatenbank.DataCouple> reBuildAdminTable = new List<HilfsklasseEinträgeDatenbank.DataCouple>();
                List<TableLists> reBuildTableList = new List<TableLists>();
                //check is Database empty
                if (backupData != null)
                {
                    //check whether AdministrationTable exist
                    if (backupData.AdministrationTable != null)
                    {
                        //fill new AdministrationTable
                        foreach (var temp in backupData.AdministrationTable)
                        {
                            reBuildAdminTable.Add(new HilfsklasseEinträgeDatenbank.DataCouple
                            {
                                Data1 = temp.TabellenName!,
                                Data2 = temp.ListName!
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("StammdatenTabelle ist null");
                    }
                    //Check whether SentenceTable exit
                    if (backupData.SentenceTabelle != null)
                    {
                        ///fill new SentenceTable
                        foreach (var temp in backupData.SentenceTabelle)
                        {
                            reBuildTableList.Add(new TableLists
                            {
                                Listname = temp.Listname,
                                JasonList = temp.JasonList
                            });
                        }

                    }
                    else
                    {
                        Console.WriteLine("tabelle ist null");

                    }
                    //overwrite old Database with new Database
                    await HilfsklasseEinträgeDatenbank.BackupImplemintate(DbFactory!, reBuildAdminTable, reBuildTableList);
                    labelBackupBackgroundColor = "greenyellow";
                    labelNameBackupStaus = "Backup Successful!";
                    isBackupButtonDisabled = true;
                    //Backup successful



                }
                else
                {
                    labelBackupBackgroundColor = "orangered";
                    labelNameBackupStaus = "Backup Failed! Rebuid var is null";
                    isBackupButtonDisabled = true;
                }

            }
            else
            {
                labelBackupBackgroundColor = "orangered";
                labelNameBackupStaus = "Backup Failed! No Backup selected or Backup File Empty.";
            }

        }

    }





    // Datastruct for the BackupFile 
    public class BackupData
    {
        public List<AdministrationClass>? AdministrationTable { get; set; }
        public List<TableLists>? SentenceTabelle { get; set; }
    }
}









