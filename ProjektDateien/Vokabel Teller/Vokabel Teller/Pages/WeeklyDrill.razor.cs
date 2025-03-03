using Blazor.IndexedDB;
using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;
using Vokabel_Teller.OwnClasses;

namespace Vokabel_Teller.Pages
{
    public partial class WeeklyDrill
    {
        [Inject]
        private IIndexedDbFactory? DbFactory { get; set; }

        private List<HilfsklasseEinträgeDatenbank.DataCouple> WortpaarListeKopie = new List<HilfsklasseEinträgeDatenbank.DataCouple>();
        private List<string> TableName = new List<string>();
        private List<int> ShuffelList = new List<int>();
        private string labelIssues1 = "";
        private string labelIssues2 = "";
        private string labelIssues3 = "";
        private string labelIssues4 = "";
        private string buttonNextContent = "Show next Word";
        private string _NameOfTabelle = "WordSentenceTable";
        private string currentList = "";
        private string buttonColorSelection1 = "#092464";
        private string buttonColorSelection2 = "#2c0f4f";
        private string buttonColorSelection3 = "#2c0f4f";
        private string buttonColor = "#1b6ec2";
        private string InputWord = "";
        private string nameOfCoiceListButton = "List Choice";
        private int count = 0;
        private int optionCount = 1;
        private bool WriteOnOff = false;
        private bool inputFieldOnOFF = true;
        private bool dropdownVisible = false; 
        private bool randomeMode = RandomNumberGenerator.GetInt32(0, 2) == 0;





        //Initialisation Methode overwriting for loading the first current TabelName, if not exist. Set a Info for User.
        protected override async Task OnInitializedAsync()
        {
            TableName = await HilfsklasseEinträgeDatenbank.GetAllListNamesAsync(DbFactory!, _NameOfTabelle);
            if (TableName != null && TableName.Count > 0)
            {
                SelectListInDropDownMenue("List Choice", 0);

                WortpaarListeKopie = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory!, _NameOfTabelle, currentList);
            }
            else
            {
                Console.WriteLine("No Entry in Tabelle " + _NameOfTabelle);
                buttonNextContent = "No Entrys Avaible";
            }
        }

        private void ToggleDropdown()
        {
            dropdownVisible = !dropdownVisible; 
        }
 
        private void SelectListInDropDownMenue(string _name, int indexNumber)
        {
            if (TableName != null)
            {
                currentList = TableName[indexNumber];
                nameOfCoiceListButton = _name;
                dropdownVisible = false;
                labelIssues1 = "";
                labelIssues2 = "";
                count = 0;
                buttonNextContent = "Show next Word";
                buttonColor = "#1b6ec2";
                InputWord = "";
                inputFieldOnOFF = true;
                labelIssues3 = "";
                labelIssues4 = "";
                DrillListLoad(DbFactory!);
            }
            else
            {
                nameOfCoiceListButton = "No List Exists";
                buttonNextContent = "No Entrys Avaible";

            }
            StateHasChanged();
        }

        private async void DrillListLoad(IIndexedDbFactory DbFactory)
        {
            WortpaarListeKopie = await HilfsklasseEinträgeDatenbank.GetInformationJasonStringFromList(DbFactory, _NameOfTabelle, currentList);
            if (WortpaarListeKopie.Count == 0)
            {
                buttonNextContent = "No Entrys Avaible";
            }
            //this Suffel a counter  with Numbers if parts like the size of the wordlist. For randome direction testing
            ShuffelList = Enumerable.Range(0, WortpaarListeKopie.Count).OrderBy(x => Guid.NewGuid()).ToList();
            StateHasChanged();
        }

        void MenuBarSwitching(int number)
        {
            if (number == 1)
            {
                optionCount = Convert.ToInt32(number);
                buttonColorSelection1 = "#092464";
                buttonColorSelection2 = "#2c0f4f";
                buttonColorSelection3 = "#2c0f4f";
                DrillListLoad(DbFactory!);
                labelIssues1 = "";
                labelIssues2 = "";
                labelIssues3 = "";
                labelIssues4 = "";
                count = 0;
                buttonNextContent = "Show next Word";
                buttonColor = "#1b6ec2";
                Console.WriteLine(buttonColor);

            }
            else if (number == 2)
            {
                optionCount = Convert.ToInt32(number);
                buttonColorSelection1 = "#2c0f4f";
                buttonColorSelection2 = "#092464";
                buttonColorSelection3 = "#2c0f4f";
                DrillListLoad(DbFactory!);
                labelIssues1 = "";
                labelIssues2 = "";
                labelIssues3 = "";
                labelIssues4 = "";
                count = 0;
                buttonNextContent = "Show next Word";
                buttonColor = "#1b6ec2";

            }
            else if (number == 3)
            {
                optionCount = Convert.ToInt32(number);
                buttonColorSelection1 = "#2c0f4f";
                buttonColorSelection2 = "#2c0f4f";
                buttonColorSelection3 = "#092464";
                DrillListLoad(DbFactory!);
                labelIssues1 = "";
                labelIssues2 = "";
                labelIssues3 = "";
                labelIssues4 = "";
                count = 0;
                buttonNextContent = "Show next Word";
                buttonColor = "#1b6ec2";
            }
            else
            {
                //Add more options
            }
        }

        public void groundSetting()
        {

            count = 0;
            labelIssues1 = "";
            labelIssues2 = "";
            buttonNextContent = "Show next Word";
            InputWord = "";
            inputFieldOnOFF = true;
            buttonColor = "#1b6ec2";
            labelIssues3 = "";

        }

        public void ButtonInputDeaktivated()
        {
            //NormalMode
            if (optionCount == 1)
            {
                if (buttonNextContent != "New Test")
                {
                    if (count >= 0 && count < ShuffelList.Count)
                    {
                        if (buttonNextContent == "Show next Word")
                        {
                            // Show next Entry
                            labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data1;
                            buttonNextContent = "Show Solution";
                            labelIssues2 = "";
                        }
                        else if (buttonNextContent == "Show Solution")
                        {
                            labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data2;
                            buttonNextContent = "Show next Word";
                            count++;
                            if (count == WortpaarListeKopie.Count)
                            {
                                buttonNextContent = "New Test";
                            }
                        }
                    }
                }
                else
                {
                    count = 0;
                    labelIssues1 = "";
                    labelIssues2 = "";
                    buttonNextContent = "Show next Word";
                }
            }
            //OppositeMode
            else if (optionCount == 2)
            {
                if (buttonNextContent != "New Test")
                {
                    if (count >= 0 && count < ShuffelList.Count)
                    {
                        if (buttonNextContent == "Show next Word")
                        {
                            // show next Entry
                            labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data2;
                            buttonNextContent = "Show Solution";
                            labelIssues2 = "";
                        }
                        else if (buttonNextContent == "Show Solution")
                        {
                            labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data1;
                            buttonNextContent = "Show next Word";
                            count++;
                            if (count == WortpaarListeKopie.Count)
                            {
                                buttonNextContent = "New Test";
                            }
                        }
                    }
                }
                else
                {
                    count = 0;
                    labelIssues1 = "";
                    labelIssues2 = "";
                    buttonNextContent = "Show next Word";
                }
            }
            //RandomeMode
            else if (optionCount == 3)
            {
                if (buttonNextContent != "New Test")
                {
                    if (randomeMode == true)
                    {


                        if (count >= 0 && count < ShuffelList.Count)
                        {
                            if (buttonNextContent == "Show next Word")
                            {
                                // show next entry
                                labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data1;
                                buttonNextContent = "Show Solution";
                                labelIssues2 = "";
                            }
                            else if (buttonNextContent == "Show Solution")
                            {
                                labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data2;
                                buttonNextContent = "Show next Word";
                                count++;
                                randomeMode = RandomNumberGenerator.GetInt32(0, 2) == 0;
                                if (count == WortpaarListeKopie.Count)
                                {
                                    buttonNextContent = "New Test";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (count >= 0 && count < ShuffelList.Count)
                        {
                            if (buttonNextContent == "Show next Word")
                            {
                                // show next entry
                                labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data2;
                                buttonNextContent = "Show Solution";
                                labelIssues2 = "";
                            }
                            else if (buttonNextContent == "Show Solution")
                            {
                                labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data1;
                                buttonNextContent = "Show next Word";
                                count++;
                                randomeMode = RandomNumberGenerator.GetInt32(0, 2) == 0;
                                if (count == WortpaarListeKopie.Count)
                                {
                                    buttonNextContent = "New Test";
                                }
                            }
                        }
                    }
                }
                else
                {
                    count = 0;
                    labelIssues1 = "";
                    labelIssues2 = "";
                    buttonNextContent = "Show next Word";
                }
            }
            StateHasChanged();
        }


        public void ButtonInputActivated()
        {
            //NormalMode
            if (optionCount == 1)
            {
                if (buttonNextContent != "New Test")
                {
                    if (count >= 0 && count < ShuffelList.Count)
                    {
                        if (buttonNextContent == "Show next Word" || buttonNextContent == "CORRECT!" || buttonNextContent == "WRONG!")
                        {
                            // show next entry
                            labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data1;
                            labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data2;
                            buttonNextContent = "Check Input";
                            InputWord = "";
                            inputFieldOnOFF = false;
                            buttonColor = "#1b6ec2";
                            labelIssues3 = "";
                            labelIssues4 = "";
                        }
                        else if (buttonNextContent == "Check Input")
                        {
                            if (labelIssues2.ToLower().Split('/').Any(teil => teil == InputWord.ToLower()))
                            {
                                buttonNextContent = "CORRECT!";
                                count++;
                                inputFieldOnOFF = true;
                                buttonColor = "#2a7f47";
                                labelIssues3 = labelIssues2;
                                labelIssues4 = InputWord;
                                if (count == WortpaarListeKopie.Count)
                                {
                                    buttonNextContent = "New Test";
                                    buttonColor = "#2a7f47";
                                }
                            }
                            else
                            {
                                buttonNextContent = "WRONG!";
                                count++;
                                inputFieldOnOFF = true;
                                buttonColor = "#8b0000";
                                labelIssues3 = labelIssues2;
                                labelIssues4 = InputWord;
                                if (count == WortpaarListeKopie.Count)
                                {
                                    buttonNextContent = "New Test";
                                    buttonColor = "#8b0000";
                                }
                            }
                        }
                    }
                }
                else
                {
                    count = 0;
                    labelIssues1 = "";
                    InputWord = "";
                    inputFieldOnOFF = true;
                    buttonNextContent = "Show next Word";
                    buttonColor = "#1b6ec2";
                    labelIssues3 = "";
                    labelIssues4 = "";


                }
            }
            //OppositeMode
            else if (optionCount == 2)
            {
                if (buttonNextContent != "New Test")
                {
                    if (count >= 0 && count < ShuffelList.Count)
                    {
                        if (buttonNextContent == "Show next Word" || buttonNextContent == "CORRECT!" || buttonNextContent == "WRONG!")
                        {
                            // show next entry
                            labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data2;
                            labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data1;
                            buttonNextContent = "Check Input";
                            InputWord = "";
                            inputFieldOnOFF = false;
                            buttonColor = "#1b6ec2";
                            labelIssues3 = "";
                            labelIssues4 = "";
                        }
                        else if (buttonNextContent == "Check Input")
                        {
                            if (labelIssues2.ToLower().Split('/').Any(teil => teil == InputWord.ToLower()))
                            {
                                buttonNextContent = "CORRECT!";
                                count++;
                                inputFieldOnOFF = true;
                                buttonColor = "#2a7f47";
                                labelIssues3 = labelIssues2;
                                labelIssues4 = InputWord;
                                if (count == WortpaarListeKopie.Count)
                                {
                                    buttonNextContent = "New Test";
                                    buttonColor = "#2a7f47";
                                }
                            }
                            else
                            {
                                buttonNextContent = "WRONG!";
                                count++;
                                inputFieldOnOFF = true;
                                buttonColor = "#8b0000";
                                labelIssues3 = labelIssues2;
                                labelIssues4 = InputWord;
                                if (count == WortpaarListeKopie.Count)
                                {
                                    buttonNextContent = "New Test";
                                    buttonColor = "#8b0000";
                                }
                            }
                        }
                    }
                }
                else
                {
                    count = 0;
                    labelIssues1 = "";
                    InputWord = "";
                    inputFieldOnOFF = true;
                    buttonNextContent = "Show next Word";
                    buttonColor = "#1b6ec2";
                    labelIssues3 = "";
                    labelIssues4 = "";
                }
            }
            //RandomeMode
            else if (optionCount == 3)
            {
                if (buttonNextContent != "New Test")
                {
                    if (randomeMode == true)
                    {


                        if (count >= 0 && count < ShuffelList.Count)
                        {
                            if (buttonNextContent == "Show next Word" || buttonNextContent == "CORRECT!" || buttonNextContent == "WRONG!")
                            {
                                // show next entry
                                labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data1;
                                labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data2;
                                buttonNextContent = "Check Input";
                                InputWord = "";
                                inputFieldOnOFF = false;
                                buttonColor = "#1b6ec2";
                                labelIssues3 = "";
                                labelIssues4 = "";
                            }
                            else if (buttonNextContent == "Check Input")
                            {
                                if (labelIssues2.ToLower().Split('/').Any(teil => teil == InputWord.ToLower()))
                                {
                                    buttonNextContent = "CORRECT!";
                                    count++;
                                    inputFieldOnOFF = true;
                                    buttonColor = "#2a7f47";
                                    labelIssues3 = labelIssues2;
                                    labelIssues4 = InputWord;
                                    randomeMode = RandomNumberGenerator.GetInt32(0, 2) == 0;
                                    if (count == WortpaarListeKopie.Count)
                                    {
                                        buttonNextContent = "New Test";
                                        buttonColor = "#2a7f47";
                                    }
                                }
                                else
                                {
                                    buttonNextContent = "WRONG!";
                                    count++;
                                    inputFieldOnOFF = true;
                                    buttonColor = "#8b0000";
                                    labelIssues3 = labelIssues2;
                                    labelIssues4 = InputWord;
                                    randomeMode = RandomNumberGenerator.GetInt32(0, 2) == 0;
                                    if (count == WortpaarListeKopie.Count)
                                    {
                                        buttonNextContent = "New Test";
                                        buttonColor = "#8b0000";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (count >= 0 && count < ShuffelList.Count)
                        {
                            if (buttonNextContent == "Show next Word" || buttonNextContent == "CORRECT!" || buttonNextContent == "WRONG!")
                            {
                                // show next entry
                                labelIssues1 = WortpaarListeKopie[ShuffelList[count]].Data2;
                                labelIssues2 = WortpaarListeKopie[ShuffelList[count]].Data1;
                                buttonNextContent = "Check Input";
                                InputWord = "";
                                inputFieldOnOFF = false;
                                buttonColor = "#1b6ec2";
                                labelIssues3 = "";
                                labelIssues4 = "";
                            }
                            else if (buttonNextContent == "Check Input")
                            {
                                if (labelIssues2.ToLower().Split('/').Any(teil => teil == InputWord.ToLower()))
                                {
                                    buttonNextContent = "CORRECT!";
                                    count++;
                                    inputFieldOnOFF = true;
                                    buttonColor = "#2a7f47";
                                    labelIssues3 = labelIssues2;
                                    labelIssues4 = InputWord;
                                    randomeMode = RandomNumberGenerator.GetInt32(0, 2) == 0;
                                    if (count == WortpaarListeKopie.Count)
                                    {
                                        buttonNextContent = "New Test";
                                        buttonColor = "#2a7f47";
                                    }
                                }
                                else
                                {
                                    buttonNextContent = "WRONG!";
                                    count++;
                                    inputFieldOnOFF = true;
                                    buttonColor = "#8b0000";
                                    labelIssues3 = labelIssues2;
                                    labelIssues4 = InputWord;
                                    randomeMode = RandomNumberGenerator.GetInt32(0, 2) == 0;
                                    if (count == WortpaarListeKopie.Count)
                                    {
                                        buttonNextContent = "New Test";
                                        buttonColor = "#8b0000";
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    count = 0;
                    labelIssues1 = "";
                    InputWord = "";
                    inputFieldOnOFF = true;
                    buttonNextContent = "Show next Word";
                    buttonColor = "#1b6ec2";
                    labelIssues3 = "";
                    labelIssues4 = "";
                }
            }
            StateHasChanged();
        }
    }
}
