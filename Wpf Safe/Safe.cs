using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace mySafe
{
    class Safe
    {
        public int iIntr = 0;
        public Boolean[,] safeArr; //здесь храним двоичное представление нашего сейа
        private int safeSize;

        public Safe( int sfSize)
        {
            iIntr = 0;
            safeArr = new Boolean[sfSize, sfSize];
            safeSize = sfSize;
            for (int i = 0; i < sfSize; i++)
                for (int j = 0; j < sfSize; j++)
                    safeArr[i,j] = false; //инициализируем массив 0( все рычаги горизонтально)
        }
 
        public Boolean Check() // Проверка выполнения условия на открытие сейфа
        {
            Boolean allChecked = true, allUnchecked = false;
            for (int i = 0; i < safeSize; i++)
                for (int j = 0; j < safeSize; j++)
                {
                    allChecked &= safeArr[i, j];
                    allUnchecked |= safeArr[i, j];
                }
            return allChecked | !allUnchecked;
        }

        public void SwitchLever(int xPos, int yPos) // поворот элемента
        {
            iIntr++;
            for (int i = 0; i < safeSize; i++)
            {
                if (i != xPos) safeArr[i, yPos] = !safeArr[i, yPos];
            }
            for (int j = 0; j < safeSize; j++)
                safeArr[xPos, j] = !safeArr[xPos, j];
        }

        public void Generate()  //Создание псевдослучайного положения рукояток
                                  /*  Данный метод случйно нажимает рукоятки, и если сейф не пришелк открытому состоянию случайно считает генерацию
                                   *  случайного положения выполненой, так как всегда можно провести обратные манипуляции и получить решение.
                                   *  Такой способ менее ресурсозатратен, проще, и в игре я сделал бы именно так.
                                   *  (здесь можно было бы закончить класс, но для сравнения реализовал второй вариант генерации ниже)
                                   */
        {
            Random rand = new Random();
            for (int i = 1; i < safeSize*2; i++)
             { 
                 SwitchLever(rand.Next(0, safeSize), rand.Next(0, safeSize));
             }
            iIntr = 0;
        }


        /********************************************************************************************************/
        /*                           Создаем честную генерацию первоначального поля                             */
        /********************************************************************************************************/

        public Boolean Check(Boolean[,] tstarr) // Перегружаем функцию для любого массива safeSize
        {
            Boolean allChecked = true, allUnchecked = false;
            for (int i = 0; i < safeSize; i++)
                for (int j = 0; j < safeSize; j++)
                {
                    allChecked &= tstarr[i, j];
                    allUnchecked |= tstarr[i, j];
                }
            return allChecked | !allUnchecked;
        }

        public void SwitchLever(int xPos, int yPos, Boolean[,] tstarr) // перегружаем поворот элемента 
        {
            iIntr++;
            for (int i = 0; i < safeSize; i++)
            {
                if (i != xPos) tstarr[i, yPos] = !tstarr[i, yPos];
            }
            for (int j = 0; j < safeSize; j++)
                tstarr[xPos, j] = !tstarr[xPos, j];
        }

        public void Generate2() // Генерация начального поля через задание случайного положения переключателей+проверку решаемости
        {
            Random rand = new Random();
            Boolean solvFound = false;
            Boolean[,] testArray = new Boolean[safeSize, safeSize];
            Boolean[,] changermtx = new Boolean[safeSize, safeSize];
            do
            {
                for (int i = 0; i < safeSize; i++) // заполняем тестовый массив случайными значениями
                    for (int j = 0; j < safeSize; j++)
                    {
                        testArray[i, j] = rand.Next(0, 2) == 1 ? true : false;
                        changermtx[i, j] = testArray[i, j]; // в алгоритме будем по очереди нажимать все вертикальные рукоятки, для этого делаем матрицу урпавления нажатиями
                        safeArr[i, j] = testArray[i, j];
                    }

                for(int trycount=1;trycount<=3;trycount++) // пытаемся за 4 прохода поля решить задачу
                {
                    for (int i = 0; i < safeSize; i++)
                    {
                        for (int j = 0; j < safeSize; j++)
                        {
                            if (changermtx[i, j])
                            {
                                SwitchLever(i, j, testArray);
                                if (Check(testArray))
                                    { solvFound = true; break; } //Если найдено решение выходим из поиска
                            }
                        }
                        if (solvFound) break;                      
                    }
                    if (solvFound) break;

                    for (int i = 0; i < safeSize; i++) // Перезаписываем матрицу управления рычагами для повтороения операции нажатия на все рычаги
                        for (int j = 0; j < safeSize; j++)
                        {
                            changermtx[i, j] = testArray[i, j]; 
                        }
                }

                if (Check()) solvFound = false; // Проверяем, что случайно расположенные рукоятки не соотвтетвуют открытому сейфу
            } while (solvFound == false); // если мы выходим из цикла - значит алгоритм нашел решение и в afeArr[i, j] сохранен правильный массив состояний
            iIntr = 0;
        }

    }
}
