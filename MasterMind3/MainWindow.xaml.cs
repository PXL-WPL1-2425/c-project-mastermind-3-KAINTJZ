
// MasterMind game by SpicyGames, schoolproject

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MasterMind3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> playerNames = new List<string>();
        private int currentPlayerIndex = 0;


        // Lijst van beschikbare kleuren voor het spel
        private readonly List<ColorItem> availableColors = new List<ColorItem>
        {
            new ColorItem { Name = "Rood", Color = Colors.Red },
            new ColorItem { Name = "Geel", Color = Colors.Yellow },
            new ColorItem { Name = "Oranje", Color = Colors.Orange },
            new ColorItem { Name = "Wit", Color = Colors.White },
            new ColorItem { Name = "Groen", Color = Colors.Green },
            new ColorItem { Name = "Blauw", Color = Colors.Blue }
        };


        private List<string> secretCode = new List<string>(); // Geheime kleurcombinatie
        private List<string> userCode = new List<string>(); // Door de gebruiker gekozen kleurcombinatie
        private List<string> attemptsHistory = new List<string>(); // Historie van code 


        // Variabelen gerelateerd aan pogingen
        private int remainingAttempts = 10;
        private int currentAttempt = 0;
        private bool gameEnded = false;
        private int maxAttempts = 10;
        private int currentScore = 10; //  Score gerelateerde variabele 



        // Lijsten voor ComboBox-besturingselementen en hun bijbehorende labels
        private List<ComboBox> comboBoxes;
        private List<Label> selectedLabels;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlayers();// Roep aan het begin een functie aan om namen te verzamelen

            // Initialiseer ComboBox- en Label-lijsten
            comboBoxes = new List<ComboBox> { RandomColorComboBox1, RandomColorComboBox2, RandomColorComboBox3, RandomColorComboBox4 };
            selectedLabels = new List<Label> { SelectedColorLabel1, SelectedColorLabel2, SelectedColorLabel3, SelectedColorLabel4 };

            GenerateRandomKleur(); // Genereer geheime kleurcombinatie
            PopulateComboBoxesWithColors(); // Vul de ComboBoxen met beschikbare kleuren
            UpdateAttemptsLabel(); // Werk de pogingen-label bij

        }



        private void InitializePlayers()
        {
            // Vraag het aantal spelers
            int playerCount = 0;
            while (playerCount < 2) // Minimum 2 spelers
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Hoeveel spelers doen mee? (Minimaal 2)", "Aantal spelers");
                if (int.TryParse(input, out playerCount) && playerCount >= 2)
                {
                    break;
                }
                MessageBox.Show("Voer een geldig aantal spelers in (minimaal 2).", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            // Verzamel namen van spelers
            for (int i = 1; i <= playerCount; i++)
            {
                string playerName;
                do
                {
                    playerName = Microsoft.VisualBasic.Interaction.InputBox($"Voer de naam in van speler {i}:", "Speler Naam");
                    if (string.IsNullOrWhiteSpace(playerName))
                        MessageBox.Show("Naam mag niet leeg zijn. Probeer opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                } while (string.IsNullOrWhiteSpace(playerName));
                playerNames.Add(playerName);
            }
            MessageBox.Show($"Spelers toegevoegd: {string.Join(", ", playerNames)}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void EndTurn(string code, bool isSolved, int attempts)
        {
            string currentPlayer = playerNames[currentPlayerIndex];
            string nextPlayer = playerNames[(currentPlayerIndex + 1) % playerNames.Count];

            string messageTitle = currentPlayer;
            string messageBody = isSolved
                ? $"Code is gekraakt in {attempts} pogingen.\nNu is speler {nextPlayer} aan de beurt."
                : $"Je hebt verloren! De correcte code was {code}.\nNu is speler {nextPlayer} aan de beurt.";

            MessageBox.Show(messageBody, messageTitle, MessageBoxButton.OK, MessageBoxImage.Information);


            // Update current player index
            currentPlayerIndex = (currentPlayerIndex + 1) % playerNames.Count;
            ResetGame();
            return;


            // Initialiseer ComboBox- en Label-lijsten
            comboBoxes = new List<ComboBox> { RandomColorComboBox1, RandomColorComboBox2, RandomColorComboBox3, RandomColorComboBox4 };
            selectedLabels = new List<Label> { SelectedColorLabel1, SelectedColorLabel2, SelectedColorLabel3, SelectedColorLabel4 };

            // Genereer een willekeurige geheime kleurcombinatie
            GenerateRandomKleur();

            // Vul de ComboBoxen met beschikbare kleuren
            PopulateComboBoxesWithColors();

            // Werk het score bij in het label
            UpdateAttemptsLabel();




        }


        





        /*==================== Kleurgerelateerde Functies ====================*/

        // Genereert een willekeurige kleurcombinatie voor de geheime code
        private void GenerateRandomKleur()
        {
            Random randomColorGenerator = new Random();

            // Schud de lijst van beschikbare kleuren en selecteer de eerste 4
            secretCode = availableColors
                .OrderBy(_ => randomColorGenerator.Next())
                .Take(4)
                .Select(c => c.Name)
                .ToList();

            // Toon de geheime code in de venstertitel (voor debugging)
            this.Title = "Geheime kleurcombinatie is: " + string.Join(", ", secretCode);
        }

        // Vult de ComboBox-besturingselementen met de lijst van beschikbare kleuren
        private void PopulateComboBoxesWithColors()
        {
            foreach (var comboBox in comboBoxes)
            {
                comboBox.ItemsSource = availableColors;
            }
        }

        // Vertegenwoordigt een kleuritem met een naam en kleurwaarde
        public class ColorItem
        {
            public string Name { get; set; } // Naam van de kleur
            public Color Color { get; set; } // Kleurwaarde
        }

        /*==================== Pogingsgerelateerde Functies ====================*/

        // Werkt het label bij dat het aantal resterende pogingen toont
        private void UpdateAttemptsLabel()
        {
            AttemptsLeftLabel.Content = $"POGINGEN OVER: {maxAttempts - currentAttempt}";
        }

        // Updaten van het score
        private void UpdateScoreLabel()
        {
            ScoreLabel.Content = $"SCORE: {currentScore}";
        }


        // Vermindert het aantal resterende pogingen en controleert op game over
        private void ReduceAttempts()
        {
            currentAttempt++;
            UpdateAttemptsLabel();

            if (currentAttempt >= maxAttempts && !gameEnded)
            {
                // Toon een game over-bericht en reset het spel
                MessageBox.Show($"Je hebt verloren! De geheime code was: {string.Join(", ", secretCode)}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                EndGame(false);
            }
        }

        /*==================== Pogingen Geschiedenis ====================*/

        // Voegt een poging toe aan de geschiedenis
        private void AddToAttemptsHistory(string userInput, string feedback)
        {
            attemptsHistory.Add($"Poging {currentAttempt + 1}: {userInput} - Feedback: {feedback}");
            UpdateAttemptsListBox();
        }

        // Werkt de ListBox bij om pogingen te tonen
        private void UpdateAttemptsListBox()
        {
            AttemptsListBox.ItemsSource = null;
            AttemptsListBox.ItemsSource = attemptsHistory;

        }



        /*==================== Gebruikersinteractie Functies ====================*/

        // Behandelt de selectie wijzigingsevent voor ComboBox-besturingselementen
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gameEnded) // Als het spel al voorbij is, informeer de gebruiker
            {
                MessageBox.Show("Het spel is al voorbij. Start een nieuw spel om verder te spelen.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null) return;

            // Haal de index van de ComboBox op en werk het bijbehorende label bij
            int index = comboBoxes.IndexOf(comboBox);
            if (index >= 0)
            {
                ColorItem selectedItem = comboBox.SelectedItem as ColorItem;
                selectedLabels[index].Background = new SolidColorBrush(selectedItem?.Color ?? Colors.White);
            }
        }

        // Behandelt de knopklik om de kleurcombinatie van de gebruiker te controleren
        private void Button_CheckColorCombination(object sender, RoutedEventArgs e)
        {
            CheckColorCombination();
        }

        // Controleert de kleurcombinatie van de gebruiker tegen de geheime code
        private void CheckColorCombination()
        {
            if (gameEnded) // Als het spel al voorbij is, informeer de gebruiker
            {
                MessageBox.Show("Het spel is al voorbij. Start een nieuw spel om verder te spelen.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Haal de door de gebruiker geselecteerde kleurennamen op
            userCode = comboBoxes
                .Select(cb => (cb.SelectedItem as ColorItem)?.Name ?? "")
                .ToList();

            // Zorg ervoor dat alle ComboBoxen een geselecteerde waarde hebben
            if (userCode.Contains(""))
            {
                MessageBox.Show("Selecteer een kleur in alle velden!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool allCorrect = true;
            StringBuilder feedbackBuilder = new StringBuilder();
            int penalty = 0; // Strafpunten voor deze poging


            // Vergelijk de code van de gebruiker met de geheime code
            for (int i = 0; i < 4; i++)
            {
                if (userCode[i] == secretCode[i]) // Correcte kleur en positie
                {
                    // 0 strafpuntyen: kleur op juiste positie
                    SetBorderColor(i, Colors.DarkRed);
                    feedbackBuilder.Append("Rood ");
                }
                else if (secretCode.Contains(userCode[i])) // Correcte kleur, verkeerde positie
                {
                    // 1 strafpunt: kleur correct, verkeerde positie
                    SetBorderColor(i, Colors.Wheat);
                    feedbackBuilder.Append("Wit ");
                    penalty += 1;
                    allCorrect = false;
                }
                else // Onjuiste kleur
                {
                    // 2 strafpunten: kleur komt niet voor
                    SetBorderColor(i, Colors.Transparent);
                    penalty += 2;
                    allCorrect = false;
                }
            }

            string feedback = feedbackBuilder.ToString().Trim();

            AddToAttemptsHistory(string.Join(", ", userCode), feedback);



            // Verminder de score met de berekende strafpunten
            currentScore -= penalty;

            // Score mag niet negatief worden
            if (currentScore < 0) currentScore = 0;

            // Werkt de score bij in de UI
            UpdateScoreLabel();


            if (allCorrect) // Als alle kleuren correct zijn, wint de gebruiker
            {
                MessageBox.Show("Gefeliciteerd! Je hebt de geheime code geraden!", "Winnaar", MessageBoxButton.OK, MessageBoxImage.Information);
                EndGame(true);
            }
            else
            {
                ReduceAttempts(); // Anders, verminder pogingen
            }
        }

        // Stelt de randkleur in voor het label dat aan de opgegeven index is gekoppeld
        private void SetBorderColor(int index, Color borderColor)
        {
            if (index >= 0 && index < selectedLabels.Count)
            {
                selectedLabels[index].BorderBrush = new SolidColorBrush(borderColor);
            }
        }

        // Reset het spel naar de beginstatus
        private void ResetGame()
        {
            currentAttempt = 0;
            gameEnded = false;
            GenerateRandomKleur();
            attemptsHistory.Clear();
            UpdateAttemptsListBox();
            ClearUI();
            UpdateScoreLabel();
            currentScore = 10; // zet de standaard waarde van score naar 10
        }

        // Leegt de UI-componenten en reset labels en ComboBoxen

        private void ClearUI()
        {
            foreach (var comboBox in comboBoxes)
            {
                comboBox.SelectedItem = null;
            }

            foreach (var label in selectedLabels)
            {
                label.Background = new SolidColorBrush(Colors.White);
                label.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            UpdateAttemptsLabel();
        }

        // Melding als de game is gewonnen of verloren:
        private void EndGame(bool isWin)
        {
            gameEnded = true;

            string message = isWin ? "Wil je opnieuw spelen?" : $"Je hebt verloren! De geheime code was: {string.Join(", ", secretCode)}. Wil je opnieuw spelen?";

            MessageBoxResult result = MessageBox.Show(message, "Einde spel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ResetGame();
            }
            else
            {
                Close();
            }
        }



        // Behandelt de knopklik om de applicatie te sluiten
        private void Button_LeaveGame(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Als de gebruiker de applicatie probeert te sluiten
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!gameEnded)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Weet je zeker dat je de applicatie wilt afsluiten?",
                    "Bevestiging afsluiten",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; // Voorkomt dat het venster wordt gesloten
                }
            }

            base.OnClosing(e);
        }

        





    }
}
