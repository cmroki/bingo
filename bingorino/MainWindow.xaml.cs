using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
//using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace bingorino {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            pointsRed = 0;
            pointsBlue = 0;

            labelPointsBlue.Content = pointsBlue.ToString();
            labelPointsRed.Content = pointsRed.ToString();
            // this is immediately initiated with the program 
            // because it was causing problems otherwise
            // making codes have numbers above the value of the objectives list
            readFromFile(textFileFolder, objectives);
        }

        /* TO DO: 
         * X points system
         * X read entries from a file
         * X generate a random bingo card
         * X give the user the ability to generate a bingo card from a code
         * X make program an executable
         * X upload it to github and manage the code
         * - add settings for customising the program
         * ? add multiplayer
         */

        public int pointsBlue;
        public int pointsRed;

        public Brush blueColour = Brushes.DeepSkyBlue;
        public Brush redColour = Brushes.Crimson;

        // this list will have objectives that are from the objectives.txt file
        // and then put in the list by the placeIntoList() method
        public List<string> objectives = new List<string> { };

        // string for keeping the code, to later give it to your opponent
        public List<int> code = new List<int> { };

        // creates a window object that is used to generate bingo cards
        //Window generationOptions = new Window() {
        //    Title = "Settings",
        //    Height = 100,
        //    Width = 100,

        //};

        // path.combine - combines three different paths to files
        // 1. the main app folder, in which there is the exe file
        // 2. the objectives folder, in which there is the text file
        // 3. and the text file itself
        // it then is assigned to an easily accessible string
        string textFileFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "objectives", "objectives.txt");

        // takes two parameters,
        // colour = the colour the background will be made into
        // sender = the object that we'll be working on
        private void colourTheSquare(Brush colour, object sender) {
            var selectedTextBlock = ((TextBlock)sender);
            if (Keyboard.IsKeyDown(Key.LeftAlt) == false) {
                // checks in case background is already specified colour
                // if it's that same colour, it doesn't add new points
                // otherwise, it adds new points to the specified team
                if (selectedTextBlock.Background != colour) {
                    // makes the square's background the colour specified in params
                    selectedTextBlock.Background = colour;
                    // makes the square's font colour white
                    selectedTextBlock.Foreground = Brushes.White;
                }
            }
        }

        private int addPoints(int team) {
            if (Keyboard.IsKeyDown(Key.LeftAlt) == false) {
                // adds one point to the overall total of points for a team
                team += 1;
            }
            return team;
        }

        private int subtractPoints(int team) {
            // subtracts one point from the team
            team -= 1;
            return team;
        }

        private string showValuesInString(List<int> list) {
            string fullLine = "";
            for (int i = 0; i < list.Count; i++) {
                fullLine += list[i].ToString();
            }
            return fullLine;
        }

        private string showValuesInString(List<int> list, char seperator) {
            string fullLine = "";

            for (int i = 0; i < list.Count(); i++) {
                if (i < (list.Count() - 1)) {
                    fullLine += list[i].ToString() + seperator;
                }
                else if (i == (list.Count() - 1)) {
                    fullLine += list[i].ToString();
                }
            }

            return fullLine;
        }

        private void generateRandomBingoCards() {
            // clears the code list to make sure the codes don't exceed 25 seperate numbers
            // at one point it exceeded 300 seperate numbers
            code.Clear();
            writeToCards(objectives, codeToCopy);
        }

        // needs two parameters filled
        // textFileLocation -> the location of the text file to then output its contents
        // objectivesList -> the list, which the output(s) will be put into
        private void readFromFile(string textFileLocation, List<string> objectivesList) {
            // creates a "reader" that reads the file based on the file location
            using (StreamReader reader = new StreamReader(textFileLocation)) {
                // creates a string
                string lineOfTextFile;

                // the string is then made into be a reader of lines
                // and, when the line is read AND not null, its value is placed into the list
                while ((lineOfTextFile = reader.ReadLine()) is not null) {
                    placeIntoList(lineOfTextFile, objectivesList);
                }
            }
        }

        // takes two parameters
        // text -> text that will be put into the list
        // listToPutValuesInto -> the list the text will be put into
        private void placeIntoList(string text, List<string> listToPutValuesInto) {
            listToPutValuesInto.Add(text);
        }

        // needs one parameter
        // list -> a list type that will have random values placed into it
        private void writeToCards(List<string> list, TextBlock showCodeOnScreen) {
            // iterates through 25 "cards"
            for (int i = 0; i < 25; i++) {
                // generates a random class
                Random random = new Random();

                // this certainly is a very bad way to come back to this part of code
                // but it's the only idea i've got for now
                // at least it's done!
                random:
                // creates a random number based on the amount of objectives in the list
                int randomNumber = random.Next(list.Count());
                // using do while because it should (in theory) make it impossible to have repeated cards

                if (!code.Contains(randomNumber)) {
                    // also, randomNumber is added to the code integer list
                    // to later be given to your opponent after generating a bingo card
                    code.Add(randomNumber);

                    // and then selects the objective based on the randomNumber
                    // which is then assigned to the string selectedObjective
                    string selectedObjective = list[randomNumber];

                    // assigns to labelName names of labels in xaml
                    string labelName = $"textBlock_{i}";


                    if (grid1.FindName(labelName) is TextBlock selectedBlock) {
                        selectedBlock.Text = selectedObjective;
                        // i honestly don't know why i'm setting these here and not in xaml, but to hell with it
                        selectedBlock.TextWrapping = TextWrapping.WrapWithOverflow;
                        selectedBlock.TextAlignment = TextAlignment.Center;
                        selectedBlock.FontSize = 15;
                        selectedBlock.FontWeight = FontWeights.Bold;
                    }
                    else {
                        // in case an error occurs, which shouldn't but probably could
                        MessageBox.Show($"TextBlock {labelName} not found.");
                    }
                }
                else {
                    goto random;
                }
            }

            // after the for loop is finished, shows the code on screen
            showCodeOnScreen.Text = "Copy code";
        }

        private void writeToCardsFromCode(List<string> list, List<int> numbersList) {
            for (int i = 0; i < numbersList.Count(); i++) {
                Random random = new Random();

                string selectedObjective = list[numbersList[i]];

                string textBlockName = $"textBlock_{i}";

                if (grid1.FindName(textBlockName) is TextBlock selectedBlock) {
                    selectedBlock.Text = selectedObjective;
                    selectedBlock.TextWrapping = TextWrapping.WrapWithOverflow;
                    selectedBlock.TextAlignment = TextAlignment.Center;
                    selectedBlock.FontSize = 15;
                    selectedBlock.FontWeight = FontWeights.Bold;
                }
                else {
                    MessageBox.Show($"TextBlock {textBlockName} not found.");
                }
            }
        }

        // this one is responsible for marking the squares blue
        private void markBlue(object sender, MouseButtonEventArgs e) {
            TextBlock txtBlock = (TextBlock)sender;
            if (Keyboard.IsKeyDown(Key.LeftAlt) == false) {
                if (txtBlock.Background == blueColour) {
                    // nothing
                }
                else if (txtBlock.Background == redColour) {
                    // colours the square blue
                    colourTheSquare(blueColour, sender);
                    // adds points to blue
                    pointsBlue = addPoints(pointsBlue);
                    // shows them
                    labelPointsBlue.Content = pointsBlue.ToString();
                    // subtracts points from red
                    pointsRed--;
                    // show points from red
                    labelPointsRed.Content = pointsRed.ToString();
                }
                else {
                    colourTheSquare(blueColour, sender);
                    pointsBlue = addPoints(pointsBlue);
                    labelPointsBlue.Content = pointsBlue.ToString();
                }
            }
        }

        // this one makes squares red
        private void markRed(object sender, MouseButtonEventArgs e) {
            TextBlock txtBlock = (TextBlock)sender;
            if (Keyboard.IsKeyDown(Key.LeftAlt) == false) {
                if (txtBlock.Background == redColour) {
                    // nothing
                }
                else if (txtBlock.Background == blueColour) {
                    // colours the square red
                    colourTheSquare(redColour, sender);
                    // adds points to red
                    pointsRed = addPoints(pointsRed);
                    // shows the points from
                    labelPointsRed.Content = pointsRed.ToString();
                    // subtracts points from blue
                    pointsBlue--;
                    // shows points from blue
                    labelPointsBlue.Content = pointsBlue.ToString();
                }
                else {
                    colourTheSquare(redColour, sender);
                    pointsRed = addPoints(pointsRed);
                    labelPointsRed.Content = pointsRed.ToString();
                }
            }
        }

        // this one is responsible for unmarking the squares, making them white
        private void unmark(object sender, MouseEventArgs e) {
            // just to check if the alt key is being pressed because of keybinds
            if (Keyboard.IsKeyDown(Key.LeftAlt)) {
                // assign label for easier use
                TextBlock txtBlock = (TextBlock)sender;

                if (txtBlock.Background == blueColour) {
                    // subtract from blue
                    pointsBlue = subtractPoints(pointsBlue);
                }
                else if (txtBlock.Background == redColour) {
                    // subtract from red
                    pointsRed = subtractPoints(pointsRed);
                }

                // reset background and font
                txtBlock.Background = Brushes.White;
                txtBlock.Foreground = Brushes.Black;

                // update the labels
                labelPointsBlue.Content = pointsBlue.ToString();
                labelPointsRed.Content = pointsRed.ToString();
            }
        }

        private void generateCards(object sender, RoutedEventArgs e) {
            //generationOptions.Show();

            generateRandomBingoCards();
        }

        private async void copyBingoCode(object sender, MouseButtonEventArgs e) {
            char comma = ',';
            Clipboard.SetText(showValuesInString(code, comma));

            codeToCopy.Text = "Copied!";

            await Task.Delay(2000);

            codeToCopy.Text = "Copy code";
        }

        private void generateCardsFromCode(object sender, RoutedEventArgs e) {
            
            // this is a list created from the code input
            // code input text is first split by the comma
            // each fragment is then made into an int
            // and lastly, everything is made into a list
            List<int> randomNumbersFromCode = codeInput.Text
                                              .Split(',')
                                              .Select(int.Parse)
                                              .ToList();



            // in case something goes wrong with the code, these are used to debug it
            //Trace.WriteLine(showValuesInString(randomNumbersFromCode));
            //Trace.WriteLine(randomNumbersFromCode.Count());

            writeToCardsFromCode(objectives, randomNumbersFromCode);
        }
    }
}