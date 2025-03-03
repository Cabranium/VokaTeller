using Blazor.IndexedDB;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;
using Vokabel_Teller.OwnClasses;

namespace Vokabel_Teller.Pages
{
    public partial class ListSetting
    {
        //for the work access of singleton of IIndexedDbFactory connecting with Database
        [Inject]
        private IIndexedDbFactory? DbFactory { get; set; }
        [Inject]
        private NavigationManager? Navigation {  get; set; }


        private List<string> TableNames = new List<string>();
        private List<HilfsklasseEinträgeDatenbank.DataCouple> DataCoupleCopyOfList1 = new List<HilfsklasseEinträgeDatenbank.DataCouple>();
        private List<HilfsklasseEinträgeDatenbank.DataCouple> DataCoupleCopyOfList2 = new List<HilfsklasseEinträgeDatenbank.DataCouple>();
        private bool dropdownVisible1 = false; // Zustandsvariable für Sichtbarkeit des Dropdowns
        private bool dropdownVisible2 = false; // Zustandsvariable für Sichtbarkeit des Dropdowns
        private string nameOfCoiceListButton1 = "List Choice";
        private string nameOfCoiceListButton2 = "List Choice";
        private string Entry1 = "";
        private string Entry2 = "";
        private string NewListName = "";
        private string _NameOfTabelle = "WordSentenceTable";
        private string currentWorkListName1 = "";
        private string currentWorkListName2 = "";
        private string buttonColorAuswahl1 = "#092464";
        private string buttonColorAuswahl2 = "#2c0f4f";
        private string buttonColorAuswahl3 = "#2c0f4f";
        private string buttonDeletColor = "#1b6ec2";
        private string buttonDeletName = "Deleting Deactivated";
        private int optionCount = 1;
        private bool deletingConfirm = false;
        private bool sameListSelectedWordMove = false;
        private bool editMode = false;


        //Is Initialitate the Page will be initialitate the Dadabase
        protected override async Task OnInitializedAsync()
        {
            TableNames = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
            //Main Work Fields
            if (TableNames != null && TableNames.Count > 0)
            {
                currentWorkListName1 = TableNames[0];
                DataCoupleCopyOfList1 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName1);
            }
            else
            {
                Console.WriteLine("Kein Eintrag in Tabelle " + _NameOfTabelle);
                nameOfCoiceListButton1 = "No Entry";
            }
            //second Work Fields for Moving DataCouples
            if (TableNames != null && TableNames.Count > 0)
            {
                currentWorkListName2 = TableNames[0];
                DataCoupleCopyOfList2 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName2);
            }
            else
            {
                Console.WriteLine("Kein Eintrag für Tabelle 2 vorhanden");
                nameOfCoiceListButton2 = "No Entry";

            }
        }

        private void DeletingOnOff()
        {
            if (deletingConfirm == false)
            {
                deletingConfirm = true;
                buttonDeletColor = "#FF3D00";
                buttonDeletName = "Deleting Activated";
            }
            else
            {
                deletingConfirm = false;
                buttonDeletColor = "#1b6ec2";
                buttonDeletName = "Deleting Deactivated";
            }
        }

        private async Task NewListSave()
        {
            await HilfsklasseEinträgeDatenbank.NewList(DbFactory!, _NameOfTabelle, NewListName);
            //TableList updating and new rendern
            TableNames = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
            NewListName = "";
        }

        private async Task DeletingList(string buttonName)
        {
            Console.WriteLine(buttonName);
            await HilfsklasseEinträgeDatenbank.DeleteList(DbFactory!, _NameOfTabelle, NewListName, buttonName);
            //TableList updating and new rendern
            TableNames = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
            NewListName = "";
        }

        private async Task ChangeEntry(ChangeEventArgs e, string _oldEntry, int fromEntryOneOrTwo)
        {
            string _newEntry = e.Value as string ?? string.Empty; //go sure e.Value is not null but if null than use string.Empty
            //if Value was broken or empty, dont change the Entry
            if(_newEntry!= string.Empty)
            {
                await HilfsklasseEinträgeDatenbank.ChangeEntry(DbFactory!, _NameOfTabelle, currentWorkListName1, _oldEntry, _newEntry, fromEntryOneOrTwo);
                DataCoupleCopyOfList1 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName1);
            }
        }

        private async Task SaveNewDataCouple()
        {
            HilfsklasseEinträgeDatenbank.DataCouple temp = new HilfsklasseEinträgeDatenbank.DataCouple(Entry1, Entry2);
            DataCoupleCopyOfList1.Add(temp);
            await HilfsklasseEinträgeDatenbank.AddInformationCoupleToList(DbFactory!, _NameOfTabelle, currentWorkListName1, DataCoupleCopyOfList1);
            DataCoupleCopyOfList1 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName1);

            Entry1 = "";
            Entry2 = "";
        }

        private async Task DeletingDataCouple(int index)
        {
            if (deletingConfirm == true)
            {
                DataCoupleCopyOfList1.Remove(DataCoupleCopyOfList1[index]);
                await HilfsklasseEinträgeDatenbank.AddInformationCoupleToList(DbFactory!, _NameOfTabelle, currentWorkListName1, DataCoupleCopyOfList1);
            }
        }

        private void ToggleDropdown(int count)
        {

            if (count == 1)
            {
                dropdownVisible1 = !dropdownVisible1;
            }
            else if (count == 2)
            {
                dropdownVisible2 = !dropdownVisible2;
            }

        }

        //switch the Selected List for Editing
        private async void SelectList(string _name, int indexNumber, int count)
        {

            if (count == 1)
            {
                if (TableNames != null)
                {
                    currentWorkListName1 = TableNames[indexNumber];
                    nameOfCoiceListButton1 = _name;
                    dropdownVisible1 = false;
                    DataCoupleCopyOfList1 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName1);
                    if (nameOfCoiceListButton1 == nameOfCoiceListButton2)
                    {
                        sameListSelectedWordMove = true;
                    }
                    else
                    {
                        sameListSelectedWordMove = false;
                    }
                }
                else
                {
                    currentWorkListName1 = "No List Exists";
                }
            }
            else if (count == 2)
            {
                if (TableNames != null)
                {
                    currentWorkListName2 = TableNames[indexNumber];
                    nameOfCoiceListButton2 = _name;
                    dropdownVisible2 = false;
                    DataCoupleCopyOfList2 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName2);
                    if (nameOfCoiceListButton1 == nameOfCoiceListButton2)
                    {
                        sameListSelectedWordMove = true;
                    }
                    else
                    {
                        sameListSelectedWordMove = false;
                    }
                }
                else
                {
                    currentWorkListName2 = "No List Exists";
                }
            }
            StateHasChanged();
        }

        // Menübar for changing different Work Processes

        async Task MenueWorkBarSwitchOption(int number)
        {
            if (number == 1)
            {
                optionCount = Convert.ToInt32(number);
                buttonColorAuswahl1 = "#092464";
                buttonColorAuswahl2 = "#2c0f4f";
                buttonColorAuswahl3 = "#2c0f4f";
                TableNames = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
                DataCoupleCopyOfList1 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName1);


            }
            else if (number == 2)
            {
                optionCount = Convert.ToInt32(number);
                buttonColorAuswahl1 = "#2c0f4f";
                buttonColorAuswahl2 = "#092464";
                buttonColorAuswahl3 = "#2c0f4f";
                TableNames = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
                DataCoupleCopyOfList1 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName1);

            }
            else if (number == 3)
            {
                optionCount = Convert.ToInt32(number);
                buttonColorAuswahl1 = "#2c0f4f";
                buttonColorAuswahl2 = "#2c0f4f";
                buttonColorAuswahl3 = "#092464";
                TableNames = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
                DataCoupleCopyOfList1 = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentWorkListName1);

            }
            else
            {
                //advance the Menue here
            }
        }

        private async Task MoveWordToList(int index)
        {
            if (nameOfCoiceListButton1 != "List Choice" && nameOfCoiceListButton2 != "List Choice")
            {
                //Moving from One to a other List
                if (sameListSelectedWordMove == false)
                {
                    DataCoupleCopyOfList2.Add(DataCoupleCopyOfList1[index]);
                    DataCoupleCopyOfList1.Remove(DataCoupleCopyOfList1[index]);
                    await HilfsklasseEinträgeDatenbank.AddInformationCoupleToList(DbFactory!, _NameOfTabelle, currentWorkListName1, DataCoupleCopyOfList1);
                    await HilfsklasseEinträgeDatenbank.AddInformationCoupleToList(DbFactory!, _NameOfTabelle, currentWorkListName2, DataCoupleCopyOfList2);
                }
                //Moving from a List to the same List (change the Order)
                else
                {
                    DataCoupleCopyOfList1.Add(DataCoupleCopyOfList1[index]);
                    DataCoupleCopyOfList1.Remove(DataCoupleCopyOfList1[index]);
                    await HilfsklasseEinträgeDatenbank.AddInformationCoupleToList(DbFactory!, _NameOfTabelle, currentWorkListName1, DataCoupleCopyOfList1);
                }
            }
            else
            {
                Console.WriteLine("Keine Selected Listen");
            }
        }

        private async void ListNameChange(ChangeEventArgs e, string _oldName)
        {
            //give the new Name
            string _newName = e.Value as string ?? string.Empty; //go sure e.Value is not null but if null than use string.Empty
            //updated the new Name in the Database entry
            await HilfsklasseEinträgeDatenbank.ListNameChange(DbFactory!, _NameOfTabelle, _oldName, _newName);
            TableNames = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
            //check if change of currentWorkListName1 if yes, update this in UI
            if (currentWorkListName1 == _oldName)
            {
                currentWorkListName1 = _newName;
                nameOfCoiceListButton1 = _newName;
            }
            //check if change of currentWorkListName2 if yes, update this in UI
            if (currentWorkListName2 == _oldName)
            {
                currentWorkListName2 = _newName;
                nameOfCoiceListButton2 = _newName;
            }
        }
    }
}
