using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Collections.ObjectModel;
using mySafe;

namespace Wpf_Safe
{
    
    
    public partial class MainWindow : Window
    {

        private int safeSize;
        private List<List<CheckBox>> ChkBxGrid;
        private Safe safe;
      
        
        private ObservableCollection<string> results;
      
        public MainWindow()
        {
            InitializeComponent();
            results = new ObservableCollection<string> { "Результат:"};
            listBox.ItemsSource = results;
        }

        private  void DrawSafe()  //Функция вывода на экран
        {

            for (int i = 0; i < safeSize; i++)
                for (int j = 0; j < safeSize; j++)
                     ChkBxGrid[i][j].IsChecked = safe.safeArr[i, j]; 
       
        }

        private async void button_Click(object sender, RoutedEventArgs e) // Если ведено целое значение - создаем новый сейф
        {
            try 
            {
                safeSize = Convert.ToInt32(textBox.Text);
                unfGrid.Children.Clear();
                safe = new Safe(safeSize);

                label1.Content = "";


                ChkBxGrid = new List<List<CheckBox>>();     // в этот раз воспользумеся List при хранении массива для разнообразия
                for (int i = 0; i < safeSize; i++)          //Создаем массив чекбоксов, который будет отображать состояние сейфа
                {
                    ChkBxGrid.Add(new List<CheckBox>());
                    for (int j = 0; j < safeSize; j++)
                    {
                        ChkBxGrid[i].Add(new CheckBox());
                        ChkBxGrid[i][j].Content = i.ToString()+":"+j.ToString(); //запоминаем индексы, чтобы легче обрабатывать нажатие на рычаг
                        ChkBxGrid[i][j].Style = (Style)FindResource("CheckBoxStyle1"); // используем стиль чекбоксов в виде рычагов, который создали в Blend. Пытаемся достич максимально масштабируемости
                        ChkBxGrid[i][j].Click += Safe_Click;
                        ChkBxGrid[i][j].Loaded += Safe_Loaded;
                        ChkBxGrid[i][j].IsChecked = false;
                    }
                }
                unfGrid.Rows = safeSize; //Разбиваем наш UniformGrid на массив NxN и выводим в каждую ячейку по рычажку
                unfGrid.Columns = safeSize;
                for (int i = 0; i < safeSize; i++)
                    for (int j = 0; j < safeSize; j++)
                        unfGrid.Children.Add(ChkBxGrid[i][j]);
                safe.Generate2();  // тут используем второй способ генерации начального состояния
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат ввода размера сейфа");
            }

        }



        private void Safe_Click(object sender, RoutedEventArgs e) //Обработка нажатия на рычаг
        {
           
            string ss = (string)(sender as CheckBox).Content; //Получаем индекс нажатого элемента
            string[] s = ss.Split(new char[] { ':' });
            safe.SwitchLever(int.Parse(s[0]), int.Parse(s[1]));
            DrawSafe();
            if (safe.Check())
                {
                    results.Add("Удачно разложено за " + safe.iIntr + "ходов"); 
                    
                    unfGrid.Children.Clear();
                label1.Content = "Сейф открыт!!!";
                    
                }

        }

        private void Safe_Loaded(object sender, RoutedEventArgs e) //По окончанию создания рычагов выводим первоначальное состояние
        {
            string ss = (string)(sender as CheckBox).Content; 
            string[] s = ss.Split(new char[] { ':' });
            ChkBxGrid[int.Parse(s[0])][int.Parse(s[1])].IsChecked = safe.safeArr[int.Parse(s[0]), int.Parse(s[1])];


        }


    }
}
