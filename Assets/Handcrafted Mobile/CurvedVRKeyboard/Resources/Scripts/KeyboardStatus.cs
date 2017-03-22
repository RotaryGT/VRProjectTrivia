using UnityEngine;
using UnityEngine.UI;

namespace CurvedVRKeyboard {
    [SelectionBase]
    public class KeyboardStatus: KeyboardComponent {

        //------SET IN UNITY-------
        [Tooltip("Text field receiving input from the keyboard")]
        public Text output;
        //@SENIORVRPROJECT
        [Tooltip("bowser receiving input from the keyboard")]
        public ZenFulcrum.EmbeddedBrowser.VRBrowserUI browserUI;
        //@SENIORVRPROJECT
        [Tooltip("Maximum output text length")]
        public int maxOutputLength;

        //----CurrentKeysStatus----
        private KeyboardItem[] keys;
        private bool areLettersActive = true;
        private bool isLowercase = true;
        private static readonly char BLANKSPACE = ' ';



        
        /// <summary>
        /// Handles click on keyboarditem
        /// </summary>
        /// <param name="clicked">keyboard item clicked</param>
        public void HandleClick ( KeyboardItem clicked ) {
            string value = clicked.GetValue();
            if(value.Equals(QEH) || value.Equals(ABC)) { // special signs pressed
                ChangeSpecialLetters();
            } else if(value.Equals(UP) || value.Equals(LOW)) { // upper/lower case pressed
                LowerUpperKeys();
            } else if(value.Equals(SPACE)) {
                TypeKey(BLANKSPACE);
            } else if(value.Equals(BACK)) {
                BackspaceKey();
            } else {// Normal letter
                TypeKey(value[0]);
            }
        }

        /// <summary>
        /// Displays special signs
        /// </summary>
        private void ChangeSpecialLetters () {
            KeyLetterEnum ToDisplay = areLettersActive ? KeyLetterEnum.NonLetters : KeyLetterEnum.LowerCase;
            areLettersActive =!areLettersActive;
            isLowercase = true;
            for(int i = 0;i < keys.Length;i++) {
                keys[i].SetKeyText(ToDisplay);
            }
        }

        /// <summary>
        /// Changes between lower and upper keys
        /// </summary>
        private void LowerUpperKeys () {
            KeyLetterEnum ToDisplay = isLowercase ? KeyLetterEnum.UpperCase : KeyLetterEnum.LowerCase;
            isLowercase = !isLowercase;
            for(int i = 0;i < keys.Length;i++) {
                keys[i].SetKeyText(ToDisplay);
            }
        }

        private void BackspaceKey()
        {
            //@SENIORVRPROJECT
            if (browserUI != null)
            {
                Event ev = new Event();
                ev.keyCode = KeyCode.Backspace;
                browserUI.InjectKeyEvent(ev);
            }
            if (output != null && output.text.Length >= 1)
                output.text = output.text.Remove(output.text.Length - 1, 1);
            //@SENIORVRPROJECT
        }

        private void TypeKey(char key)
        {
            //@SENIORVRPROJECT
            if (browserUI != null)
            {
                Event ev = new Event();
                ev.character = key;
                ev.keyCode = CharToKeyUnityKeyCode(key);
                browserUI.InjectKeyEvent(ev);
            }

            if (output != null && output.text.Length < maxOutputLength)
                output.text = output.text + key.ToString();
            //@SENIORVRPROJECT
        }
        //@SENIORVRPROJECT
        private KeyCode CharToKeyUnityKeyCode(char key)
        {
            switch (key)
            {
                case 'a':
                    return KeyCode.A;
                case 'b':
                    return KeyCode.B;
                case 'c':
                    return KeyCode.C;
                case 'd':
                    return KeyCode.D;
                case 'e':
                    return KeyCode.E;
                case 'f':
                    return KeyCode.F;
                case 'g':
                    return KeyCode.G;
                case 'h':
                    return KeyCode.H;
                case 'i':
                    return KeyCode.I;
                case 'j':
                    return KeyCode.J;
                case 'k':
                    return KeyCode.K;
                case 'l':
                    return KeyCode.L;
                case 'm':
                    return KeyCode.M;
                case 'n':
                    return KeyCode.N;
                case 'o':
                    return KeyCode.O;
                case 'p':
                    return KeyCode.P;
                case 'q':
                    return KeyCode.Q;
                case 'r':
                    return KeyCode.R;
                case 's':
                    return KeyCode.S;
                case 't':
                    return KeyCode.T;
                case 'u':
                    return KeyCode.U;
                case 'v':
                    return KeyCode.V;
                case 'w':
                    return KeyCode.W;
                case 'x':
                    return KeyCode.X;
                case 'y':
                    return KeyCode.Y;
                case 'z':
                    return KeyCode.Z;
                case '0':
                    return KeyCode.Alpha0;
                case '1':
                    return KeyCode.Alpha1;
                case '2':
                    return KeyCode.Alpha2;
                case '3':
                    return KeyCode.Alpha3;
                case '4':
                    return KeyCode.Alpha4;
                case '5':
                    return KeyCode.Alpha5;
                case '6':
                    return KeyCode.Alpha6;
                case '7':
                    return KeyCode.Alpha7;
                case '8':
                    return KeyCode.Alpha8;
                case '9':
                    return KeyCode.Alpha9;
                case '?':
                    return KeyCode.Question;
                case '!':
                    return KeyCode.Exclaim;
                case '#':
                    return KeyCode.Hash;
                case '.':
                    return KeyCode.Period;
                case '@':
                    return KeyCode.At;
                case '_':
                    return KeyCode.Underscore;
                case '&':
                    return KeyCode.Ampersand;
                case '-':
                    return KeyCode.Minus;
                case '+':
                    return KeyCode.Plus;
                case '(':
                    return KeyCode.LeftParen;
                case ')':
                    return KeyCode.RightParen;
                case '*':
                    return KeyCode.Asterisk;
                case '\"':
                    return KeyCode.DoubleQuote;
                case '\'':
                    return KeyCode.Quote;
                case '/':
                    return KeyCode.Slash;
                case ':':
                    return KeyCode.Colon;
                case ';':
                    return KeyCode.Semicolon;
                case ' ':
                default:
                    return KeyCode.Space;
            }
        }
        //@SENIORVRPROJECT

        public void SetKeys ( KeyboardItem[] keys ) {
            this.keys = keys;
        }
    }
}
