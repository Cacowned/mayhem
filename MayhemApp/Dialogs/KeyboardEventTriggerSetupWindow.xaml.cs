using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
// using Gma.UserActivityMonitor;
using System.Diagnostics;
using MayhemApp.Low_Level;


namespace MayhemApp.Dialogs
{
    /// <summary>
    /// Interaction logic for KeyboardEventTriggerSetupWindow.xaml
    /// </summary>
    public partial class KeyboardEventTriggerSetupWindow : Window
    {

       

        private HashSet<System.Windows.Forms.Keys> keys_down = new HashSet<System.Windows.Forms.Keys>();
        private ObservableCollection<object> selected_keys_list = new ObservableCollection<object>();


        #region some classes that make drawing the "+" easier
        private class StyledLabel : Label
        {
            public StyledLabel(Style s, string content)
                : base()
            {
                this.Style = s;
                this.Content = content;

            
            }
        }

        private class PlusLabel : StyledLabel
        {
            public PlusLabel(Style s) : base(s, "+") { }
        }
        #endregion

        

        public bool scanning_keys = false;
        public bool keys_selected = false;




        InterceptKeys key_intercept = InterceptKeys.instance;


    

        public const string TAG = "[KeyboardEventTriggerSetupWindow] :";

        private InterceptKeys.KeyDownHandler keyDownHandler = null;
        private InterceptKeys.KeyUpHandler keyUpHandler = null;

        #region Key Combination updated (Save Button Clicked)

        /**<summary>
         * Holds the key combination in order to inform the trigger.
         * Gets sent when the "Save" button is clicked. 
         * </summary>
         * */
        public class KeyCombinationUpdatedArgs : EventArgs
        {

            public HashSet<System.Windows.Forms.Keys> key_combination = new  HashSet<System.Windows.Forms.Keys>();
            public KeyCombinationUpdatedArgs(HashSet<System.Windows.Forms.Keys> q)
                : base()
            {
                foreach (System.Windows.Forms.Keys k in q)
                {
                    key_combination.Add(k);
                }
            }
        }

        public delegate void KeyCombinationUpdatedHandler(object sender, KeyCombinationUpdatedArgs e);
        public event KeyCombinationUpdatedHandler OnKeyCombinationUpdated;

        #endregion



        public KeyboardEventTriggerSetupWindow()
        {
            InitializeComponent();

           // this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(KeyboardEventTriggerSetupWindow_IsVisibleChanged);


            BitmapImage one = new BitmapImage(new Uri("/MayhemApp;component/Images/Keyboard/1.png", UriKind.Relative));

            BitmapImage two = new BitmapImage(new Uri("/MayhemApp;component/Images/Keyboard/2.png", UriKind.Relative));

            PlusLabel plus = new PlusLabel((Style) FindResource("PlusStyle"));  
    
      
             
            this.keyItems.ItemsSource = selected_keys_list;
            //selected_keys_list.Add(one);
            //selected_keys_list.Add(plus);
            //selected_keys_list.Add(two);
            scanning_keys = true;
            // button1.Content = "Click when done scanning keys!";
            //HookManager.KeyDown += new System.Windows.Forms.KeyEventHandler(g_KeyDown);

            keys_down.Clear();

            keyDownHandler = new InterceptKeys.KeyDownHandler(InterceptKeys_OnInterceptKeyDown);
            keyUpHandler = new InterceptKeys.KeyUpHandler(InterceptKeys_OnInterceptKeyUp);

            InterceptKeys.OnInterceptKeyDown += keyDownHandler;
            InterceptKeys.OnInterceptKeyUp += keyUpHandler;

           

        }

       

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            this.Hide();
        }

        

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            InterceptKeys.OnInterceptKeyUp -= keyUpHandler;
            InterceptKeys.OnInterceptKeyDown -= keyDownHandler;

            keyUpHandler = null;
            keyDownHandler = null;

            scanning_keys = false;
          

            if (OnKeyCombinationUpdated != null)
            {
                OnKeyCombinationUpdated(this, new KeyCombinationUpdatedArgs( keys_down));
            }

        }

        /**<summary>
         * "Resets scanned key list"
         * </summary>
         */
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            selected_keys_list.Clear();
            keys_down.Clear();

            if (keyDownHandler == null)
            {
                keyDownHandler = new InterceptKeys.KeyDownHandler(InterceptKeys_OnInterceptKeyDown);
                InterceptKeys.OnInterceptKeyDown += keyDownHandler;
            }

            if (keyUpHandler == null)
            {
                keyUpHandler = new InterceptKeys.KeyUpHandler(InterceptKeys_OnInterceptKeyUp);
                InterceptKeys.OnInterceptKeyUp += keyUpHandler;
            }


        }

        void InterceptKeys_OnInterceptKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
           // throw new NotImplementedException();
            
           
        }

        /**<summary>
         * 
         * Used to add the keypress representation to the widget during de-serialization
         * 
         * </summary>
         */ 
        public void Deserialize_AddKey(System.Windows.Forms.Keys k)
        {
            if (selected_keys_list.Count < 1)
            {
                selected_keys_list.Add(new StyledLabel((Style)FindResource("PlusStyle"), k.ToString()));
            }
            else
            {
                selected_keys_list.Add(new PlusLabel((Style)FindResource("PlusStyle")));
                selected_keys_list.Add(new StyledLabel((Style)FindResource("PlusStyle"), k.ToString()));
            }
        }
        
        void InterceptKeys_OnInterceptKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
           // throw new NotImplementedException();

            if (!keys_down.Contains(e.KeyCode))
            {
                keys_down.Add(e.KeyCode);

                if (keys_down.Count == 1)
                {
                    selected_keys_list.Add(new StyledLabel((Style)FindResource("PlusStyle"), e.KeyCode.ToString()));
                }
                else
                {
                    selected_keys_list.Add(new PlusLabel((Style)FindResource("PlusStyle")));
                    selected_keys_list.Add(new StyledLabel((Style)FindResource("PlusStyle"), e.KeyCode.ToString()));
                }
            }


        }

        void g_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(TAG + e);
            
        }
    }
}
