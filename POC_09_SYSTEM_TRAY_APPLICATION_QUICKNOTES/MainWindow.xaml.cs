using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
namespace POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsFavorite { get; set; }

        public static bool IsExit = false;
        private TaskbarIcon trayIcon;

        public QuickNotes QuickNotes { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            LoadNotes();
            ApplySavedOpacity();

            ApplyFontSize(Properties.Settings.Default.NotesFontSize);
            ApplyTheme(Properties.Settings.Default.AppTheme);


        }

        public void ApplyTheme(string theme)
        {
            switch (theme)
            {
                case "Dark":

                    Background = System.Windows.Media.Brushes.Black;

                    Apptitle.Background = System.Windows.Media.Brushes.White;
              
                    notesListBox.Background = System.Windows.Media.Brushes.DimGray;

                    notesListBox.Foreground = System.Windows.Media.Brushes.White;

                    txtSearch.Background = System.Windows.Media.Brushes.White;
                    txtSearch.Foreground = System.Windows.Media.Brushes.White;

                    break;

                default:

                    Background = System.Windows.Media.Brushes.White;

                    Apptitle.Background = System.Windows.Media.Brushes.White;

                    notesListBox.Background = System.Windows.Media.Brushes.White;

                    notesListBox.Foreground = System.Windows.Media.Brushes.Black;

                    txtSearch.Background = System.Windows.Media.Brushes.White;

                    txtSearch.Foreground = System.Windows.Media.Brushes.Black;

                    break;
            }
        }

        public void ApplyFontSize(double fontSize)
        {
            if (fontSize < 12 || fontSize > 20)
                fontSize = 14;

            notesListBox.FontSize = fontSize;
        }

        private void ApplySavedOpacity()
        {
            double opacity = Properties.Settings.Default.WindowOpacity;

            if (opacity < 0.30 || opacity > 1.00)
            {
                opacity = 1.00;
            }

            this.Opacity = opacity;
        }

      


        public void OpenNote(int noteId)
        {
           
            MessageBox.Show("Opening Note : " + noteId);   
        }


        private void Window_StateChanged(object sender, System.EventArgs e)
        {

            if (WindowState == WindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
            }

        }

        private void txtSearch_TextChanged(object sender,TextChangedEventArgs e)
        {
            txtHint.Visibility = string.IsNullOrWhiteSpace(txtSearch.Text)
            ? Visibility.Visible
            : Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsExit)
            {
                e.Cancel = true;
                Hide();

            }
        }
            


        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (QuickNotes == null || !QuickNotes.IsVisible)
                {
                    QuickNotes = new QuickNotes();

                    QuickNotes.WindowStartupLocation = WindowStartupLocation.Manual;

                    QuickNotes.Left = SystemParameters.WorkArea.Width - QuickNotes.Width - 10;
                    QuickNotes.Top = SystemParameters.WorkArea.Height - QuickNotes.Height - 10;

                    QuickNotes.Show();
                }
                else
                {
                    QuickNotes.WindowState = WindowState.Normal;
                    QuickNotes.Activate();

                }

                QuickNotes.WindowState = WindowState.Normal;
                QuickNotes.SaveNote();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Opening QuickNotes window" + ex.Message);
            }
        }

        public void LoadNotes()
        {
            try
            {
                string jsonFilePath = "D:\\meet\\notes.json";

                if (!File.Exists(jsonFilePath))
                {
                    File.WriteAllText(jsonFilePath, "[]");
                    notesListBox.ItemsSource = new List<Notes>();
                    return;
                }

                string existingJson = File.ReadAllText(jsonFilePath);

                List<Notes> notesList;

                if (string.IsNullOrWhiteSpace(existingJson))
                {
                    notesList = new List<Notes>();
                }
                else
                {
                    notesList = JsonConvert.DeserializeObject<List<Notes>>(existingJson)
                                ?? new List<Notes>();
                }

                notesListBox.ItemsSource = notesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Load Notes fron json File" + ex.Message);
            }


        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Notes selectedNote = notesListBox.SelectedItem as Notes;

                if (selectedNote == null)
                {
                    MessageBox.Show("Please select a note to edit.");
                    return;
                }

                QuickNotes quickNotes = new QuickNotes(selectedNote);
                quickNotes.ShowDialog();

                LoadNotes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Editing Note" + ex.Message);
            }
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (notesListBox.SelectedItem == null)
                {
                    MessageBox.Show("select a note to delete.");
                    return;
                }

                Notes selectedNote = (Notes)notesListBox.SelectedItem;

                string jsonFilePath = @"D:\meet\notes.json";

                if (!File.Exists(jsonFilePath))
                    return;

                string json = File.ReadAllText(jsonFilePath);

                List<Notes> notesList = JsonConvert.DeserializeObject<List<Notes>>(json) ?? new List<Notes>();

                notesList.RemoveAll(n => n.Title == selectedNote.Title && n.Notemsg == selectedNote.Notemsg);

                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(notesList, Formatting.Indented));

                notesListBox.ItemsSource = null;
                notesListBox.ItemsSource = notesList;

                trayIcon = (TaskbarIcon)FindResource("TrayIcon");

                ((App)Application.Current).ShowNotification("Quick Notes","Notes deleted successfully",BalloonIcon.Info);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Deleting Note" + ex.Message);
            }
        }

        private void btnbackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string source = "D:\\meet\\notes.json";
                string backup = "D:\\meet\\POC09_Backup\\notes_backup.json";

                if (File.Exists(source))
                {
                    File.Copy(source, backup, true);
                }

                trayIcon = (TaskbarIcon)FindResource("TrayIcon");

                trayIcon.ShowBalloonTip("Quick Notes", "Backup created successfully", BalloonIcon.Info);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Creating Backup" + ex.Message);
            }

        }

        private void btnsortbydate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonFilePath = @"D:\meet\notes.json";

                if (!File.Exists(jsonFilePath))
                    return;

                string json = File.ReadAllText(jsonFilePath);

                List<Notes> notesList = JsonConvert.DeserializeObject<List<Notes>>(json) ?? new List<Notes>();

                notesList = notesList.OrderByDescending(n => n.CreateDate).ToList();

                notesListBox.ItemsSource = null;
                notesListBox.ItemsSource = notesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Sorting Notes by Date" + ex.Message);
            }
        }

        private void btnsortbytitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonFilePath = @"D:\meet\notes.json";

                if (!File.Exists(jsonFilePath))
                    return;

                string json = File.ReadAllText(jsonFilePath);

                List<Notes> notesList = JsonConvert.DeserializeObject<List<Notes>>(json) ?? new List<Notes>();

                notesList = notesList
                    .OrderBy(n => n.Title)
                    .ToList();

                notesListBox.ItemsSource = null;
                notesListBox.ItemsSource = notesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Sorting Notes by Title" + ex.Message);
            }
        }

        private void btntagnotes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchtag = txtSearch.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchtag))
                {
                    MessageBox.Show("Enter a tag.");
                    return;
                }

                string jsonFilePath = @"D:\meet\notes.json";

                if (!File.Exists(jsonFilePath))
                    return;

                string json = File.ReadAllText(jsonFilePath);

                List<Notes> notesList = JsonConvert.DeserializeObject<List<Notes>>(json)
                                       ?? new List<Notes>();

                var filteredNotes = notesList
                    .Where(n => !string.IsNullOrWhiteSpace(n.Notetag) && n.Notetag.Split(',').Any(t => t.Trim().Equals(searchtag, StringComparison.OrdinalIgnoreCase))).ToList();

                notesListBox.ItemsSource = null;
                notesListBox.ItemsSource = filteredNotes;

                if (filteredNotes.Count == 0)
                {
                    MessageBox.Show("No notes found for this tag.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: searching Notes by Tag" + ex.Message);
            }
        }

        private void btnview_Click(object sender, RoutedEventArgs e)
        {
            string jsonFilePath = @"D:\meet\notes.json";

            if (!File.Exists(jsonFilePath))
            {
                MessageBox.Show("No notes found.");
                return;
            }

            string json = File.ReadAllText(jsonFilePath);

            List<Notes> notesList = JsonConvert.DeserializeObject<List<Notes>>(json)
                                   ?? new List<Notes>();

            notesListBox.ItemsSource = null;
            notesListBox.ItemsSource = notesList;
        }

        private void btnsearchbycontent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    LoadNotes();
                    return;
                }

                string jsonFilePath = @"D:\meet\notes.json";

                if (!File.Exists(jsonFilePath))
                    return;

                string json = File.ReadAllText(jsonFilePath);

                List<Notes> notesList = JsonConvert.DeserializeObject<List<Notes>>(json)
                                       ?? new List<Notes>();

                var filteredNotes = notesList
                    .Where(n => !string.IsNullOrWhiteSpace(n.Notemsg) &&
                                n.Notemsg.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                notesListBox.ItemsSource = null;
                notesListBox.ItemsSource = filteredNotes;

                if (filteredNotes.Count == 0)
                {
                    MessageBox.Show("No matching notes found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: searching Notes by Notes Contain" + ex.Message);
            }
        }

        public void SaveAllNotes()
        {
            string path = @"D:\meet\notes.json";

            List<Notes> notes = notesListBox.ItemsSource as List<Notes>;

            string json = JsonConvert.SerializeObject(notes, Formatting.Indented);

            File.WriteAllText(path, json);
        }

        private void btnfavoritenotes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (notesListBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a note.");
                    return;
                }

                Notes note = (Notes)notesListBox.SelectedItem;

                note.IsFavorite = !note.IsFavorite;

                SaveAllNotes();
                LoadNotes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Marking Note as Favorite" + ex.Message);
            }


        }

     
    }
}



