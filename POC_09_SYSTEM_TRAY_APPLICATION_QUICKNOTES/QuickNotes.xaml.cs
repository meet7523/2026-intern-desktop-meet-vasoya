using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES
{
    /// <summary>
    /// Interaction logic for QuickNotes.xaml
    /// </summary>
    /// 

    public class Notes
    {
        internal string txtTags;
        internal string txttags;
        internal string txttag;

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Notetag { get; set; }
        public string Notemsg { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsFavorite { get; set; }

    }
    public partial class QuickNotes : Window
    {
        private readonly Notes _note;
        private TaskbarIcon trayIcon;
        private object note;
        private object notesListBox;
        private object notesList;
        private string jsonPath;
        private object notifyIcon;

        public int NotifyIcon_BalloonTipClicked { get; }

        public QuickNotes()
        {
            InitializeComponent();

        }

       
        public QuickNotes(Notes note)
        {
            InitializeComponent();

            _note = note;

            txttitle.Text = note.Title;
            txttag.Text = note.Notetag;
            txtNote.Text = note.Notemsg;

        }

        public bool ValidateNote()
        {
            if (string.IsNullOrWhiteSpace(txttitle.Text))
            {
                MessageBox.Show("Enter a title.");
                txttitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txttag.Text))
            {
                MessageBox.Show("Enter a tag.");
                txttag.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNote.Text))
            {
                MessageBox.Show("Enter a note.");
                txtNote.Focus();
                return false;
            }

          
            return true;
        }




        private void titlebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHint.Visibility = string.IsNullOrWhiteSpace(txttitle.Text)
            ? Visibility.Visible
            : Visibility.Hidden;

            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txttitle.Focus();
            Keyboard.Focus(txttitle);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
                {
                    if (!ValidateNote())
                        return;

                    SaveNote();
                    this.Close();
                    e.Handled = true;
                }
                else
                {
                    if (e.Key == Key.Escape)
                    {
                        txttitle.Clear();
                        txtNote.Clear();

                        this.Close();

                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        

        

        public void SaveNote()
        {
            try
            {
                string jsonFilePath = "D:\\meet\\notes.json";

                List<Notes> notesList = new List<Notes>();

                if (File.Exists(jsonFilePath))
                {
                    string existingJson = File.ReadAllText(jsonFilePath);

                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        notesList = JsonConvert.DeserializeObject<List<Notes>>(existingJson) ?? new List<Notes>();
                    }
                }

                    if (_note != null)
                    {
                        var noteedit = notesList.FirstOrDefault(x => x.Id == _note.Id);

                        if (noteedit != null)
                        {
                            noteedit.Title = txttitle.Text;
                            noteedit.Notetag = txttag.Text;
                            noteedit.Notemsg = txtNote.Text;
                            noteedit.CreateDate = DateTime.Now;
                        } 

                   
                        trayIcon = (TaskbarIcon)FindResource("TrayIcon");

                    ((App)Application.Current).ShowNotification("Quick Notes","Notes updated successfully",BalloonIcon.Info);

                }

                    else
                    {

                        var note = new Notes
                        {
                            Title = txttitle.Text,
                            Notetag = txttag.Text,
                            Notemsg = txtNote.Text,
                            CreateDate = DateTime.Now
                        };

                    if (!ValidateNote())
                        return;


                    notesList.Add(note);

                        trayIcon = (TaskbarIcon)FindResource("TrayIcon");

                    ((App)Application.Current).ShowNotification("Quick Notes", "Notes saved successfully", BalloonIcon.Info);


                    CustomBalloon balloon = new CustomBalloon();

                        trayIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 8000);
                }

                if (!ValidateNote())
                    return;

                string updatedJson = JsonConvert.SerializeObject(notesList, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, updatedJson);


                txttitle.Clear();
                txttag.Clear();
                txtNote.Clear();

                txttitle.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtNote_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtnote.Visibility = string.IsNullOrWhiteSpace(txtnote.Text)
            ? Visibility.Visible
            : Visibility.Hidden;
        }

        private void txttag_TextChanged(object sender, TextChangedEventArgs e)
        {
            txttaghint.Visibility = string.IsNullOrWhiteSpace(txttag.Text)
           ? Visibility.Visible
           : Visibility.Hidden;
        }

        public void ApplyFontSize(int size)
        {
            txtNote.FontSize = size;
        }

       
    }
}
    
